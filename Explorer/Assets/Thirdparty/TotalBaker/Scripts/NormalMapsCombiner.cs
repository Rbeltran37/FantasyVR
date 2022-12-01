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


    public class NormalMapsCombiner : EditorWindow
    {

        #region exposed variables

        public Texture2D largeDetails; //the low frequency normal map
        public Texture2D smallDetails; //the high frequency normal map

        #endregion




        #region window layout variables

        private int resolution = 512; //resolution of the output texture
        private int leftBarWidth = 350;
        private int rightBarStart = 372;
        private Vector2 scrollPos;
        private bool _dragging = false;
        private int toolbarInt = 0;
        private string[] toolbarStrings = new string[] {"Large details map", "Small details map", "Combined map"};
        private Texture selectedTex = null;
        private Texture2D checkerTex = null;
        private GUIStyle chessArea;
        private int scrollSize = 512;
        string baseName = "";
        string outputFolder = "Assets/Textures";
        private Texture2D icon_folder;
        private bool outputFolderIsRelative;

        #endregion




        #region auxiliary variables

        private RenderTexture largeDetailsRT; //render texture to store low frequency map in the GPU
        private RenderTexture smallDetailsRT; //render texture to store high frequency map in the GPU
        private RenderTexture combined; //the final combined map
        private static readonly int _Details = Shader.PropertyToID("_Details");

        #endregion



        #region init window

        [MenuItem("Tools/Total Baker/Toolbox/Normal Maps Combiner")]
        static void Init()
        {
            //init Total Baker main window if not initialized by opening it
            if (!TotalBaker.IsInitialized) {
                TotalBaker tbWindow = (TotalBaker)GetWindow(typeof(TotalBaker));
                tbWindow.Show();
                tbWindow.Close();
                TotalBaker.IsInitialized = true;
            }
            
            NormalMapsCombiner window = (NormalMapsCombiner) GetWindow(typeof(NormalMapsCombiner));
            window.Show();
            window.minSize = new Vector2(892, 545);
            window.maxSize = new Vector2(892, 546);
            window.titleContent = new GUIContent("NM Combiner");
        }

        #endregion


        #region core

        void Awake()
        {
            if (checkerTex != null) DestroyImmediate(checkerTex);
            checkerTex = TotalBaker.CreateCheckerTexture(resolution);
            icon_folder = (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/TotalBaker/Icons/folder.png", typeof(Texture2D));
        }

        void OnDestroy()
        {
            if (checkerTex != null) DestroyImmediate(checkerTex);
            if (combined != null) DestroyImmediate(combined);
        }


        void OnGUI()
        {

            chessArea = new GUIStyle(GUI.skin.box);
            chessArea.normal.background = checkerTex;
            chessArea.margin = new RectOffset(0, 0, 0, 0);
            chessArea.padding = new RectOffset(0, 0, 0, 0);

            GUIContent content;

            EditorGUILayout.BeginHorizontal();

            GUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox), GUILayout.Width(360));

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.HelpBox("This tool allows you to open both classic and \"marked as Normal Map\" textures.\nNON marked textures produce slightly better results.", MessageType.Warning);

            content = new GUIContent("Large details map",
                "The base, low frequency normal map containing large surface details.");
            largeDetails = (Texture2D) EditorGUILayout.ObjectField(content, largeDetails, typeof(Texture2D), false);

            content = new GUIContent("Small details map",
                "High frequency normal map containing small surface details. It will be merged onto the low frequency map.");
            smallDetails = (Texture2D) EditorGUILayout.ObjectField(content, smallDetails, typeof(Texture2D), false);

            if (EditorGUI.EndChangeCheck())
            {
                if (largeDetails && !smallDetails)
                {
                    resolution = largeDetails.width;
                    if (largeDetailsRT != null) DestroyImmediate(largeDetailsRT);
                    largeDetailsRT = TotalBaker.Texture2D2RenderTexture(largeDetails);
                    if (ImportUtility.IsImportedAsNormalMap(largeDetails)) Filters.Unpack(largeDetailsRT, resolution);
                    if (checkerTex != null) DestroyImmediate(checkerTex);
                    checkerTex = TotalBaker.CreateCheckerTexture(resolution);
                }

                else if (!largeDetails && smallDetails)
                {
                    resolution = smallDetails.width;
                    if (largeDetailsRT != null) DestroyImmediate(smallDetailsRT);
                    smallDetailsRT = TotalBaker.Texture2D2RenderTexture(smallDetails);
                    if (ImportUtility.IsImportedAsNormalMap(smallDetails)) Filters.Unpack(smallDetailsRT, resolution);
                    if (checkerTex != null) DestroyImmediate(checkerTex);
                    checkerTex = TotalBaker.CreateCheckerTexture(resolution);
                }
                else if (largeDetails && smallDetails)
                {
                    resolution = largeDetails.width;
                    if (largeDetailsRT != null) DestroyImmediate(largeDetailsRT);
                    largeDetailsRT = TotalBaker.Texture2D2RenderTexture(largeDetails);
                    if (ImportUtility.IsImportedAsNormalMap(largeDetails)) Filters.Unpack(largeDetailsRT, resolution);
                    resolution = smallDetails.width;
                    if (largeDetailsRT != null) DestroyImmediate(smallDetailsRT);
                    smallDetailsRT = TotalBaker.Texture2D2RenderTexture(smallDetails);
                    if (ImportUtility.IsImportedAsNormalMap(smallDetails)) Filters.Unpack(smallDetailsRT, resolution);
                    resolution = Mathf.Max(largeDetails.width, smallDetails.width);
                    if (combined != null) DestroyImmediate(combined);
                    combined = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGBFloat);
                    Filters.CombineNormals(largeDetailsRT, smallDetailsRT, combined);
                    if (checkerTex != null) DestroyImmediate(checkerTex);
                    checkerTex = TotalBaker.CreateCheckerTexture(resolution);
                }

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

            content = new GUIContent("Base name", "The generated texture will be named [basename]_Normal_combined.png");
            baseName = EditorGUILayout.TextField(content, baseName);

            //SAVE
            if (combined != null && GUILayout.Button("Save texture", GUILayout.Width(leftBarWidth)))
            {
                if(!Directory.Exists(outputFolder)){    
                    Directory.CreateDirectory(outputFolder);
                }
                
                byte[] bytes;
                bytes = TotalBaker.RenderTexture2Texture2D(combined).EncodeToPNG();

                string path;
                if (outputFolderIsRelative) path = Application.dataPath + outputFolder;
                path = outputFolder + baseName + "_Normal_Combined.png";
                File.WriteAllBytes(path, bytes);
                AssetDatabase.Refresh();
                Debug.Log(TotalBaker.logPrefix + "Successfully saved texture.");
                
            }

            EditorGUILayout.EndVertical();


            EditorGUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Space(3);
            toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarStrings, GUILayout.Width(512));
            switch (toolbarInt)
            {
                case 0:
                    selectedTex = largeDetailsRT;
                    break;
                case 1:
                    selectedTex = smallDetailsRT;
                    break;
                case 2:
                    selectedTex = combined;
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

            //handle mouse drag
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

            GUILayout.EndHorizontal();

            #endregion

        }

        #endregion

        //Send a blit request to the GPU with the two maps as input. They will be processed with the shader Hidden/CombineNormals.
        public static void CombineNormals(RenderTexture main, Texture details, RenderTexture dest)
        {
            Material mat = new Material(Shader.Find("Hidden/CombineNormals"));
            mat.SetTexture(_Details, details);
            Graphics.Blit(main, dest, mat);
            DestroyImmediate(mat);
        }

    }

}

#endif