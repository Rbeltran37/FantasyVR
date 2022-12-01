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


    public class DilateTexture : EditorWindow
    {

        public Texture2D source;
        public int dilation = 0;

        int resolution;
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
        private Settings.DilationMode dilationMode;
        private RenderTexture outputRT;
        private RenderTexture previewTexture;
        private RenderTexture RTsource;
        private readonly string[] channelsToolbarStrings = {"RGBA", "RGB", "R", "G", "B", "A"};
        private int channelsToolbarInt = 0;
        



        #region init window

        [MenuItem("Tools/Total Baker/Toolbox/Dilate Texture")]
        static void Init()
        {
            //init Total Baker main window if not initialized by opening it
            if (!TotalBaker.IsInitialized) {
                TotalBaker tbWindow = (TotalBaker)GetWindow(typeof(TotalBaker));
                tbWindow.Show();
                tbWindow.openedByToolbox = true;
                tbWindow.Close();
                TotalBaker.IsInitialized = true;
            }
            
            DilateTexture window = (DilateTexture)GetWindow(typeof(DilateTexture));
            window.Show();
            window.minSize = new Vector2(892, 592);
            window.maxSize = new Vector2(892, 593);
            window.titleContent = new GUIContent("Dilate Texture");
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

            EditorGUILayout.BeginVertical(GUILayout.Width(leftBarWidth));
            GUILayout.Space(4);

            using (new GUILayout.VerticalScope(new GUIStyle(EditorStyles.helpBox)))
            {
                EditorGUILayout.HelpBox("Dilation works only with texture having a transparent background.", MessageType.Warning);
                
                EditorGUI.BeginChangeCheck();
                content = new GUIContent("Source texture", "The source texture to be converted");
                source = (Texture2D) EditorGUILayout.ObjectField(content, source, typeof(Texture2D), false);
                if (EditorGUI.EndChangeCheck() && source != null)
                {
                    RTsource = TotalBaker.Texture2D2RenderTexture(source);
                    RenderTexture.active = null;
                    if (outputRT != null) DestroyImmediate(outputRT);
                    outputRT = new RenderTexture(source.width, source.height, 0, RenderTextureFormat.ARGBFloat);
                    outputRT.Create();
                    Filters.Copy(RTsource, outputRT);
                    resolution = source.width;
                    if (checkerTex != null) DestroyImmediate(checkerTex);
                    checkerTex = TotalBaker.CreateCheckerTexture(resolution);
                    UpdateTexture();
                }

                content = new GUIContent("Dilation mode", "Enable to support transparent textures.");
                EditorGUI.BeginChangeCheck();
                    dilationMode = (Settings.DilationMode)EditorGUILayout.EnumPopup(content, dilationMode);
                if (EditorGUI.EndChangeCheck())
                {
                    UpdateTexture();
                }
                
                EditorGUI.BeginChangeCheck();

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
                    path = outputFolder + baseName + "_Dilated.png";
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
                    
                    dilation = EditorGUILayout.IntSlider("Amount", dilation, 0, 20);

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

        #endregion



        void UpdateTexture()
        {
            Filters.Copy(RTsource, outputRT);
            Filters.Dilate(outputRT, dilation, dilationMode);
            
            if (previewTexture == null)
            {
                previewTexture = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGBFloat);
                previewTexture.Create();
            }
            
            Graphics.Blit(outputRT, previewTexture);
            //show only selected channels
            Filters.IsolateChannels(previewTexture, channelsToolbarInt);
        }
    }

}

#endif
