/*====================================================
*
* TOTAL BAKER
*
* Francesco Cucchiara - 3POINT SOFT
*
=====================================================*/

#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace TB
{


    public class Model2Curvature : EditorWindow
    {
        private List<ImportUtility.ModelToReimport> needReimport;
        
        private GameObject source;
        private Settings.NormalsImportType normalsImportType;
        private int smoothingAngle;
        
        int resolution = 512;
        Settings.Resolution res = Settings.Resolution._512;
        int leftBarWidth = 362;
        int rightBarStart = 372;
        Vector2 scrollPos;
        bool _dragging = false;
        Texture selectedTex = null;
        Texture2D checkerTex = null;
        GUIStyle chessArea;
        int scrollSize = 512;
        bool autoUpdatePreview = true; //auto update texture preview
        string baseName = "";
        string outputFolder = "Assets/Textures";
        private Texture2D icon_folder;
        private bool outputFolderIsRelative;
        public int dilation = 10;
        private Settings.CurvatureChannels curvatureChannels = Settings.CurvatureChannels.Single;
        private float curvatureMultiplier = 1;
        private float denoise = 10;
        private int curvatureSmoothness = 8;
        private RenderTexture camRT;
        private RenderTexture sourceRT;
        private RenderTexture outputRT;
        private RenderTexture previewTexture;
        private readonly string[] channelsToolbarStrings = {"RGBA", "RGB", "R", "G", "B", "A"};
        private int channelsToolbarInt = 0;
        private Camera cam;
        private Material uv2Normal;
        private static bool shadersInitSuccessful;

        private GameObject instance;
        



        #region init window

        [MenuItem("Tools/Total Baker/Toolbox/Model2Curvature")]
        static void Init()
        {
            shadersInitSuccessful = Filters.InitShaders();
            
            Model2Curvature window = (Model2Curvature)GetWindow(typeof(Model2Curvature));
            window.Show();
            window.minSize = new Vector2(892, 650);
            window.maxSize = new Vector2(892, 651);
            window.titleContent = new GUIContent("Model2Curvature");
        }

        #endregion


        #region code

        void Awake()
        {
            if (checkerTex != null) DestroyImmediate(checkerTex);
            checkerTex = TotalBaker.CreateCheckerTexture(resolution);
            icon_folder = (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/TotalBaker/Icons/folder.png", typeof(Texture2D));
        }

        void OnDestroy()
        {
            if (checkerTex != null) DestroyImmediate(checkerTex);
            if (outputRT != null) DestroyImmediate(outputRT);
            if (camRT != null) DestroyImmediate(camRT);
            if (sourceRT != null) DestroyImmediate(sourceRT);
            if (needReimport.Count > 0)
            {
                ImportUtility.ReimportModels(needReimport, true);
            }
        }

        void OnEnable() 
        {
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
        }

        void OnDisable()
        {
            AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
        }

        private void OnAfterAssemblyReload()
        {
            shadersInitSuccessful = Filters.InitShaders(); 
        }

        void OnGUI()
        {
            if (!shadersInitSuccessful)
            {
                EditorGUILayout.HelpBox("Error: shaders initialization failed.\nPlease make sure all the shaders have been correctly imported or try to reimport the Total Baker package.", MessageType.Error);
                return;
            }
            
            chessArea = new GUIStyle(GUI.skin.box);
            chessArea.normal.background = checkerTex;
            chessArea.margin = new RectOffset(0, 0, 0, 0);
            chessArea.padding = new RectOffset(0, 0, 0, 0);

            GUIContent content;


            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical(GUILayout.Width(leftBarWidth));
            GUILayout.Space(4);

            using (new GUILayout.VerticalScope(new GUIStyle(EditorStyles.helpBox)))
            {
                EditorGUILayout.HelpBox("This tool works only with UV unwrapped models, and will produce better results if there are no overlapping faces in the UV map.", MessageType.Warning);
                
                content = new GUIContent("Model", "The model for which curvature map will be generated");
                source = (GameObject) EditorGUILayout.ObjectField(content, source, typeof(GameObject), false);
                
                normalsImportType = (Settings.NormalsImportType) EditorGUILayout.EnumPopup("Normals", normalsImportType);
               
                if (normalsImportType == Settings.NormalsImportType.Calculate)
                {
                    smoothingAngle = EditorGUILayout.IntSlider("Smoothing Angle", smoothingAngle, 0, 180);
                }
                
                GUILayout.Space(20);
                
                content = new GUIContent("Resolution", "The resolution of the generated map");
                res = (Settings.Resolution) EditorGUILayout.EnumPopup(content, res);
                resolution = (int)res;
                
                //OutputPath directory selector
                GUILayout.BeginHorizontal(GUILayout.Width(leftBarWidth));
                GUIStyle Style = EditorStyles.textField;
                Style.alignment = TextAnchor.UpperLeft;
                outputFolder = EditorGUILayout.TextField(new GUIContent("Output folder"), outputFolder, Style);
                string last = outputFolder.Substring(outputFolder.Length - 1);
                if(!last.Equals("/")) outputFolder += "/";
                GUI.SetNextControlName("OpenFolder");
                if (GUILayout.Button(
                    new GUIContent(icon_folder), 
                    EditorStyles.label, 
                    GUILayout.MaxHeight(16),
                    GUILayout.MaxWidth(19))
                ) {
                    GUI.FocusControl("OpenFolder"); //force text field to lose focus if focused
                    outputFolder = EditorUtility.OpenFolderPanel("Select output folder", "", "");
                    if (outputFolder.StartsWith(Application.dataPath)) {
                        outputFolderIsRelative = true;
                        outputFolder =  "Assets" + outputFolder.Substring(Application.dataPath.Length)+"/";
                    }
                    else {
                        outputFolder += "/";
                        outputFolderIsRelative = false;
                    }
                }

                GUILayout.EndHorizontal();
                
                content = new GUIContent("Base name", "The generated texture will be named [basename]_Blurred.png");
                baseName = EditorGUILayout.TextField(content, baseName);
                content = new GUIContent("Auto Update Preview",
                    "Automatically update texture preview when changing filters values.");
                autoUpdatePreview = EditorGUILayout.Toggle(content, autoUpdatePreview);

                if (source != null && GUILayout.Button("Bake"))
                {
                    BakeCurvature();    
                }
                
                //SAVE
                if (outputRT != null && GUILayout.Button("Save texture"))
                {
                    if(!Directory.Exists(outputFolder)){    
                        Directory.CreateDirectory(outputFolder);
                    }
                    
                    byte[] bytes;
                    bytes = TotalBaker.RenderTexture2Texture2D(outputRT).EncodeToPNG();

                    string path;
                    if (outputFolderIsRelative) path = Application.dataPath + outputFolder;
                    path = outputFolder + baseName + "_Curvature.png";
                    File.WriteAllBytes(path, bytes);
                    AssetDatabase.Refresh();
                    Debug.Log(TotalBaker.logPrefix + "Successfully saved texture.");
                }
                
            }
        

            EditorGUILayout.EndVertical();


            EditorGUILayout.BeginVertical();


            selectedTex = previewTexture;

            #region textures preview
            
            EditorGUI.BeginChangeCheck();
            channelsToolbarInt = GUILayout.Toolbar(channelsToolbarInt, channelsToolbarStrings, EditorStyles.toolbarButton, GUILayout.Width(250));
            if (EditorGUI.EndChangeCheck())
            {
                UpdateTexture();
            }
            
            scrollSize = (resolution < scrollSize) ? 512 : resolution;
            scrollPos = GUI.BeginScrollView(new Rect(rightBarStart, 24, 512, 512), scrollPos,
                new Rect(rightBarStart, 24, scrollSize, scrollSize));
            int size = (resolution > 512) ? resolution : 512;
            GUI.Box(new Rect(rightBarStart, 4, size, size), selectedTex, chessArea);
            GUI.EndScrollView();
            //handle drag
            if (new Rect(rightBarStart, 4, 512, 512).Contains(Event.current.mousePosition))
            {
                if (Event.current.type == EventType.MouseDrag) _dragging = true;
            }

            if (_dragging)
            {
                if (Event.current.type == EventType.MouseDrag)
                {
                    scrollPos += -Event.current.delta; 
                    Event.current.Use();
                }
                else if (Event.current.type == EventType.MouseUp)
                {
                    _dragging = false;
                    Event.current.Use();
                }
            }

            int slidersBoxWidth = 512;
            int slidersBoxHeight = 120;

            GUILayout.BeginArea(new Rect(rightBarStart, 548, slidersBoxWidth, slidersBoxHeight));

            if (source != null)
            {
                GUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox));

               if (outputRT != null)
                {
                    EditorGUI.BeginChangeCheck();
                    
                    dilation = EditorGUILayout.IntSlider("Dilation", dilation, 0, 20);

                    content = new GUIContent("Multiplier", "Controls the strength of the curvature colors. Raise this value to get more contrast.");
                    curvatureMultiplier = EditorGUILayout.Slider("Curvature Multiplier", curvatureMultiplier, 0, 5);

                    content = new GUIContent("Channels", "Use Single to get a grayscale curvature map where below 50% grey represents concavity and above 50% represents convexity.\nUse Dual to get a R-G curvature map where green represents convexity and red represents concavity");
                    curvatureChannels = (Settings.CurvatureChannels) EditorGUILayout.EnumPopup(content, curvatureChannels);
                   
                    denoise = EditorGUILayout.Slider("Denoise", denoise, 0, 10);
                    
                    curvatureSmoothness = EditorGUILayout.IntSlider("Smoothness", curvatureSmoothness, 0, 8);

                    if (EditorGUI.EndChangeCheck() && autoUpdatePreview)
                    {
                        UpdateTexture();
                    }

                }

                if (!autoUpdatePreview && GUILayout.Button("Update", GUILayout.Width(100)))
                {
                    UpdateTexture();
                }

                GUILayout.EndVertical();

            }

            GUILayout.EndArea();
            EditorGUILayout.EndVertical();

            GUILayout.EndHorizontal();

            #endregion

        }

        private void CreateCamera()
        {
            //create camera 
            if(cam != null) DestroyImmediate(cam.gameObject);
            cam = new GameObject("TB_Camera").AddComponent<Camera>();
            cam.gameObject.hideFlags = HideFlags.DontSave;
            cam.transform.position = new Vector3(0.5f, 0.5f, -0.5f);
            cam.clearFlags = CameraClearFlags.Color;
            cam.backgroundColor = Color.gray;
            cam.tag = "MainCamera"; 
            cam.orthographic = true;
            cam.orthographicSize = 0.5f;
            cam.clearFlags = CameraClearFlags.Color;
            cam.cullingMask = 1 << 31;
            cam.backgroundColor = new Color(0, 0, 0, 0);
        }
        
        private void BakeCurvature()
        {
            if (source == null)
            {
                return;
            }
           
            if (!Filters.InitShaders())
            {
                return;
            }
            
            instance = Instantiate(source);
            instance.layer = 31;
            
            needReimport = ImportUtility.CheckReimportNeeded(instance, normalsImportType, smoothingAngle);
            foreach (ImportUtility.ModelToReimport model in needReimport)
            {
                string message = "";
                message += "- Asset " + model.assetPath + " needs to be reimported.\n\n";
                message += "\nThe reimport process may take some time for large meshes.\n\n";


                if (EditorUtility.DisplayDialog("Warning", message, "Reimport", "Cancel"))
                {
                    ImportUtility.ReimportModels(needReimport, normalsImportType == Settings.NormalsImportType.Import ? ModelImporterNormals.Import : ModelImporterNormals.Calculate, smoothingAngle, true);
                }
                else
                {
                    return;
                }
            }

            if (outputRT != null)
            {
                if (cam != null)
                {
                    cam.targetTexture = null;
                }
                DestroyImmediate(outputRT);
            } 

            CreateCamera();
            
            outputRT = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGBFloat);
            outputRT.wrapMode = TextureWrapMode.Repeat;
            outputRT.Create();
            
            sourceRT = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGBFloat);
            sourceRT.Create();

            camRT = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGBFloat);
            camRT.Create();

            cam.targetTexture = camRT;
            
            //render world space normal map
            if (uv2Normal != null)
            {
                DestroyImmediate(uv2Normal);
            }
            
            uv2Normal = new Material(Shader.Find("Hidden/TB/UV2Normal"));
            AssignMaterialToAllMeshes(instance, uv2Normal);
            cam.Render();
            
            Graphics.Blit(camRT, sourceRT);
            
            UpdateTexture();
            
            DestroyImmediate(uv2Normal);
            DestroyImmediate(instance);
            DestroyImmediate(cam.gameObject);
        }

        private void AssignMaterialToAllMeshes(GameObject root, Material material)
        {
            Renderer[] rends = root.GetComponentsInChildren<Renderer>();
            foreach (Renderer rend in rends)
            {
                rend.sharedMaterial = material;
            }
        }

        #endregion


        void UpdateTexture()
        {
            Filters.Copy(sourceRT, outputRT);
            
            Filters.Dilate(outputRT, dilation, Settings.DilationMode.Opaque); 
            Filters.ApplyBackground(outputRT, Color.gray);
            Filters.BilateralBlur(outputRT, denoise);
            
            Filters.Normal2Curvature(outputRT, curvatureChannels, curvatureMultiplier, curvatureSmoothness);

            if (previewTexture == null)
            {
                previewTexture = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGBFloat);
                previewTexture.Create();
            }

            Graphics.Blit(outputRT, previewTexture, new Vector2(1, -1), Vector2.zero); //reverse y coord because the shader is Texture2D-friendly and already reverses the y coord
            
            //show only selected channels
            Filters.IsolateChannels(previewTexture, channelsToolbarInt);
        }
    }

}

#endif
