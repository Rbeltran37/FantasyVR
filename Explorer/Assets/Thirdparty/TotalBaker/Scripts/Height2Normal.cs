/*====================================================
*
* TOTAL BAKER
*
* Francesco Cucchiara - 3POINT SOFT
*
=====================================================*/

#if UNITY_EDITOR

using System.IO;
using UnityEngine;
using UnityEditor;

namespace TB
{
    public class Height2Normal : EditorWindow
    {

        public Texture2D source;
        public float bumpStrength = 10f;

        int resolution;
        private const int leftBarWidth = 350;
        private const int rightBarStart = 372;
        Vector2 scrollPos;
        bool _dragging = false;
        int toolbarInt = 0;
        string[] toolbarStrings = new string[] {"Height", "Normal"};
        Texture selectedTex = null;
        Texture2D checkerTex = null;
        GUIStyle chessArea;
        int scrollSize = 512;
        bool autoUpdatePreview = true; //auto update texture preview
        string baseName = "";
        string outputFolder = "Assets/Textures";
        private Texture2D icon_folder;
        private bool outputFolderIsRelative;

        private RenderTexture outputRTNormal;
        private RenderTexture outputRTHeight;
        private RenderTexture RTsource;
        public int blurHeightAmount = 0;
        public float brightnessHeight = 0.0f;
        public float contrastHeight = 1.0f;



        #region init window

        [MenuItem("Tools/Total Baker/Toolbox/Height2Normal")]
        static void Init()
        {
            //init Total Baker main window if not initialized by opening it
            if (!TotalBaker.IsInitialized) {
                TotalBaker tbWindow = (TotalBaker)GetWindow(typeof(TotalBaker));
                tbWindow.Show();
                tbWindow.Close();
                TotalBaker.IsInitialized = true;
            }
            
            Height2Normal window = (Height2Normal) GetWindow(typeof(Height2Normal));
            window.Show();
            window.minSize = new Vector2(892, 635);
            window.maxSize = new Vector2(892, 636);
            window.titleContent = new GUIContent("Height2Normal");
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
            if (outputRTNormal != null) DestroyImmediate(outputRTNormal);
            if (outputRTHeight != null) DestroyImmediate(outputRTHeight);
            if (RTsource != null) DestroyImmediate(RTsource);
        }


        void OnGUI()
        {


            chessArea = new GUIStyle(GUI.skin.box);
            chessArea.normal.background = checkerTex;
            chessArea.margin = new RectOffset(0, 0, 0, 0);
            chessArea.padding = new RectOffset(0, 0, 0, 0);

            GUIContent content;


            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();

            using (new GUILayout.VerticalScope(new GUIStyle(EditorStyles.helpBox)))
            {
                EditorGUI.BeginChangeCheck();
                content = new GUIContent("Source texture", "The source texture to be converted");
                source = (Texture2D) EditorGUILayout.ObjectField(content, source, typeof(Texture2D), false);
                if (EditorGUI.EndChangeCheck() && source != null)
                {
                    RTsource = TotalBaker.Texture2D2RenderTexture(source);
                    if (outputRTNormal != null) DestroyImmediate(outputRTNormal);
                    outputRTNormal = new RenderTexture(source.width, source.height, 0, RenderTextureFormat.ARGBFloat);
                    outputRTNormal.Create();
                    Filters.Copy(RTsource, outputRTNormal);
                    if (outputRTHeight != null) DestroyImmediate(outputRTHeight);
                    outputRTHeight = new RenderTexture(source.width, source.height, 0, RenderTextureFormat.ARGBFloat);
                    outputRTHeight.Create();
                    resolution = source.width;
                    if (checkerTex != null) DestroyImmediate(checkerTex);
                    checkerTex = TotalBaker.CreateCheckerTexture(resolution);
                    UpdateHeightMap();
                    UpdateNormalMap();
                }
                
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

                content = new GUIContent("Base name", "The generated texture will be named [basename]_Normal.png");
                baseName = EditorGUILayout.TextField(content, baseName);
                content = new GUIContent("Auto Update Preview",
                    "Automatically update texture preview when changing filters values.");
                autoUpdatePreview = EditorGUILayout.Toggle(content, autoUpdatePreview, GUILayout.Width(leftBarWidth));

                //SAVE
                if (outputRTNormal != null && GUILayout.Button("Save texture"))
                {
                    if(!Directory.Exists(outputFolder)){    
                        Directory.CreateDirectory(outputFolder);
                    }
                    
                    byte[] bytes;
                    bytes = TotalBaker.RenderTexture2Texture2D(outputRTNormal).EncodeToPNG();

                    string path;
                    if (outputFolderIsRelative) path = Application.dataPath + outputFolder;
                    path = outputFolder + baseName + "_Normal.png";
                    File.WriteAllBytes(path, bytes);
                    AssetDatabase.Refresh();
                    Debug.Log(TotalBaker.logPrefix + "Successfully saved texture.");
                }
            }


            EditorGUILayout.EndVertical();


            EditorGUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Space(3);
            toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarStrings, GUILayout.Width(512));
            switch (toolbarInt)
            {
                case 0:
                    selectedTex = outputRTHeight;
                    break;
                case 1:
                    selectedTex = outputRTNormal;
                    break;
                default:
                    selectedTex = null;
                    break;
            }

            GUILayout.EndHorizontal();



            #region textures preview

            scrollSize = (resolution < scrollSize) ? 512 : resolution;
            scrollPos = GUI.BeginScrollView(new Rect(rightBarStart, 24, 512, 512), scrollPos,
                new Rect(rightBarStart, 24, scrollSize, scrollSize));
            int size = (resolution > 512) ? resolution : 512;
            GUI.Box(new Rect(rightBarStart, 24, size, size), selectedTex, chessArea);
            GUI.EndScrollView();
            //handle drag
            if (new Rect(rightBarStart, 24, 512, 512).Contains(Event.current.mousePosition))
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

            GUILayout.BeginArea(new Rect(rightBarStart, 550, slidersBoxWidth, slidersBoxHeight));

            if (source != null)
            {
                GUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox));

                if (outputRTHeight != null)
                {
                    EditorGUI.BeginChangeCheck();
                    blurHeightAmount = EditorGUILayout.IntSlider("Blur", blurHeightAmount, 0, 20);
                    brightnessHeight = EditorGUILayout.Slider("Brightness", brightnessHeight, -5, 5);
                    contrastHeight = EditorGUILayout.Slider("Contrast", contrastHeight, 0, 20);
                    if (EditorGUI.EndChangeCheck() && autoUpdatePreview)
                    {
                        UpdateHeightMap();
                        UpdateNormalMap();
                    }

                    EditorGUI.BeginChangeCheck();
                    if (toolbarInt == 1 && outputRTNormal != null)
                    {
                        bumpStrength = EditorGUILayout.Slider("Bump Strength", bumpStrength, 0.03f, 20);
                    }

                    if (EditorGUI.EndChangeCheck() && autoUpdatePreview)
                    {
                        UpdateNormalMap();
                    }
                }

                if (!autoUpdatePreview && GUILayout.Button("Update", GUILayout.Width(100)))
                {
                    UpdateHeightMap();
                    UpdateNormalMap();
                }

                GUILayout.EndVertical();

            }

            GUILayout.EndArea();
            EditorGUILayout.EndVertical();

            GUILayout.EndHorizontal();

            #endregion

        }

        #endregion

        void UpdateNormalMap()
        {
            Filters.Grayscale2Normal(outputRTHeight, outputRTNormal, bumpStrength);
        }

        void UpdateHeightMap()
        {
            Filters.Copy(RTsource, outputRTHeight);
            Filters.Contrast(outputRTHeight, brightnessHeight, contrastHeight);
            Filters.GaussianBlur(outputRTHeight, blurHeightAmount);
        }



    }

}

#endif