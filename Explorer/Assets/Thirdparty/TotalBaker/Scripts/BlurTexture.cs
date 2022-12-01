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
    public class BlurTexture : EditorWindow
    {

        public Texture2D source;
        public float offset = 1.3f;

        public enum BlurMode
        {
            Gaussian = 0,
            Linear = 1,
            Bilateral = 2
        }

        public BlurMode mode = BlurMode.Gaussian;


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

        private RenderTexture outputRT;
        private RenderTexture RTsource;
        public int blurAmount = 0;



        #region init window

        [MenuItem("Tools/Total Baker/Toolbox/Blur Texture")]
        static void Init()
        {
            //init Total Baker main window if not initialized by opening it
            if (!TotalBaker.IsInitialized) {
                TotalBaker tbWindow = (TotalBaker)GetWindow(typeof(TotalBaker));
                tbWindow.Show();
                tbWindow.Close();
                TotalBaker.IsInitialized = true;
            }
            
            BlurTexture window = (BlurTexture)GetWindow(typeof(BlurTexture));
            window.Show();
            window.minSize = new Vector2(892, 588);
            window.maxSize = new Vector2(892, 589);
            window.titleContent = new GUIContent("Blur Texture");
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

                content = new GUIContent("Blur mode",
                    "Gaussian Blur: the blur slider refers to pixels. This means that if the slider has a value of 10, a 10 pixels wide blur will be applied to the source image.\nLinear Blur: useful for large blurs or for small blurs when you don't need pixel precision.");
                EditorGUI.BeginChangeCheck();
                mode = (BlurMode) EditorGUILayout.EnumPopup(content, mode);
                if (EditorGUI.EndChangeCheck())
                {
                    if (outputRT != null) UpdateTexture();
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
                    path = outputFolder + baseName + "_Blurred.png";
                    File.WriteAllBytes(path, bytes);
                    AssetDatabase.Refresh();
                    Debug.Log(TotalBaker.logPrefix + "Successfully saved texture.");
                }
            }


            EditorGUILayout.EndVertical();


            EditorGUILayout.BeginVertical();


            selectedTex = outputRT;

            #region textures preview

            scrollSize = (resolution < scrollSize) ? 512 : resolution;
            scrollPos = GUI.BeginScrollView(new Rect(rightBarStart, 4, 512, 512), scrollPos,
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

            int slidersBoxHeight = 40;

            GUILayout.BeginArea(new Rect(rightBarStart, 524, autoUpdatePreview? 512:400, slidersBoxHeight));

            if (source != null)
            {
                GUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox));


                if (outputRT != null)
                {
                    EditorGUI.BeginChangeCheck();
                    blurAmount = EditorGUILayout.IntSlider("Blur", blurAmount, 0, 20);
                    if (mode == BlurMode.Linear)
                    {
                        offset = EditorGUILayout.Slider("Offset", offset, 0.01f, 10f);
                    }

                    if (EditorGUI.EndChangeCheck() && autoUpdatePreview)
                    {
                        UpdateTexture();
                    }
                }

                GUILayout.EndVertical();

            }

            GUILayout.EndArea();
            
            if (!autoUpdatePreview && GUI.Button(new Rect(rightBarStart + 412, 524, 100, slidersBoxHeight), "Update"))
            {
                UpdateTexture();
            }
            
            EditorGUILayout.EndVertical();

            GUILayout.EndHorizontal();

            #endregion

        }

        #endregion



        void UpdateTexture()
        {
            Filters.Copy(RTsource, outputRT);
            if (mode == BlurMode.Gaussian) Filters.GaussianBlur(outputRT, blurAmount);
            else if (mode == BlurMode.Linear) Filters.LinearBlur(outputRT, blurAmount, offset);
            else Filters.BilateralBlur(outputRT, blurAmount);
        }




    }

}

#endif