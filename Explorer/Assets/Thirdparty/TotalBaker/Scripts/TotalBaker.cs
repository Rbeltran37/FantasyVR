/*====================================================
*
* TOTAL BAKER
*
* Francesco Cucchiara - 3POINT SOFT
*
=====================================================*/

#if UNITY_EDITOR

using UnityEngine.Rendering;
using System;
using System.Linq;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace TB
{
    public class TotalBaker : EditorWindow
    {

        #region Core public variables

        public static bool IsInitialized = false;

        public const string logPrefix = "<color=brown><b>TB|</b></color> ";
        
        public Light[] lights;
        public TBLight[] TBLights;
        
        
        
        
        private static readonly int _BumpMap = Shader.PropertyToID("_BumpMap");
        private static readonly int _ParallaxMap = Shader.PropertyToID("_ParallaxMap");
        private static readonly int _EmissionMap = Shader.PropertyToID("_EmissionMap");
        private static readonly int _OcclusionMap = Shader.PropertyToID("_OcclusionMap");
        private static readonly int _MetallicGlossMap = Shader.PropertyToID("_MetallicGlossMap");
        private static readonly int _SpecGlossMap = Shader.PropertyToID("_SpecGlossMap");
        private static readonly int _EmissionColor = Shader.PropertyToID("_EmissionColor");
        private static readonly int _FillColor = Shader.PropertyToID("_FillColor");
        private static readonly int _LineColor = Shader.PropertyToID("_LineColor");
        private static readonly int _WireThickness = Shader.PropertyToID("_WireThickness");
        private static readonly int _Smoothness = Shader.PropertyToID("_Smoothness");
        private static readonly int _Metallic = Shader.PropertyToID("_Metallic");
        private static readonly int _SpecColor = Shader.PropertyToID("_SpecColor");
        private static readonly int _Glossiness = Shader.PropertyToID("_Glossiness");
        private static readonly int _GlossMapScale = Shader.PropertyToID("_GlossMapScale");
        private static readonly int _LightsValidation = Shader.PropertyToID("_LightsValidation");
        private static readonly int _LightsTypes = Shader.PropertyToID("_LightsTypes");
        private static readonly int _LightsPositions = Shader.PropertyToID("_LightsPositions");
        private static readonly int _LightsDirections = Shader.PropertyToID("_LightsDirections");
        private static readonly int _LightsColors = Shader.PropertyToID("_LightsColors");
        private static readonly int _LightsIntensities = Shader.PropertyToID("_LightsIntensities");
        private static readonly int _LightsRanges = Shader.PropertyToID("_LightsRanges");
        private static readonly int _LightsOuterCosAngles = Shader.PropertyToID("_LightsOuterCosAngles");
        private static readonly int _LightsInnerCosAngles = Shader.PropertyToID("_LightsInnerCosAngles");
        private static readonly int _LightsCount = Shader.PropertyToID("_LightsCount");
        private static readonly int _AmbientColor = Shader.PropertyToID("_AmbientColor");
        private static readonly int _High2LowWpos = Shader.PropertyToID("_High2LowWpos");
        private static readonly int _High2LowNormal = Shader.PropertyToID("_High2LowNormal");
        private static readonly int _WorldNormalMap = Shader.PropertyToID("_WorldNormalMap");
        private static readonly int _Density = Shader.PropertyToID("_Density");
        private static readonly int _Color1 = Shader.PropertyToID("_Color1");
        private static readonly int _Color2 = Shader.PropertyToID("_Color2");
        private static readonly int _LayerMask = Shader.PropertyToID("_LayerMask");
        private static readonly int _Color = Shader.PropertyToID("_Color");
        private static readonly int _Cutoff = Shader.PropertyToID("_Cutoff");
        private static readonly int _GlossyReflections = Shader.PropertyToID("_GlossyReflections");
        private static readonly int _TangentNormalMap = Shader.PropertyToID("_TangentNormalMap");
        private static readonly int _MainTex = Shader.PropertyToID("_MainTex");
        
        
        #endregion

        #region Core private variables

        private const bool debug = false;
        private int resolution = 512; //enum to int
        private bool hasLights;
        private Texture2D outputTex_uv;
        private Texture2D outputTex_diffuse;
        private Texture2D outputTex_height;
        private Texture2D outputTex_normalMain;
        private Texture2D outputTex_normalCombined;
        private Texture2D outputTex_normalDetails;
        private Texture2D outputTex_metallic;
        private Texture2D outputTex_specular;
        private Texture2D outputTex_ao;
        private Texture2D outputTex_emissive;
        private Texture2D outputTex_curvature;
//        private Texture2D outputTex_mask;
        internal static RenderTexture outputRTDiffuse;
        private RenderTexture outputRTDiffuse_original;
        public static RenderTexture outputRTHeight;
        private RenderTexture outputRTHeight_original;
        public static RenderTexture outputRTNormalMain;
        private RenderTexture outputRTNormalDetails;
        private RenderTexture outputRTNormalMain_original;
        private RenderTexture outputRTNormalDetails_original;
        public static RenderTexture outputRTNormalCombined;
        private RenderTexture outputRTMetallic_original;
        private RenderTexture outputRTSpecular_original;
        public static RenderTexture outputRTMetallic;
        public static RenderTexture outputRTSpecular;
        private RenderTexture outputRTAO_original;
        public static RenderTexture outputRTAO;
        private RenderTexture outputRTEmissive_original;
        public static RenderTexture outputRTEmissive;
        private RenderTexture outputRTCurvature_original;
        public static RenderTexture outputRTCurvature;
        public static RenderTexture outputRTMask;
        private RenderTexture packedNormalMap; //only used for scene preview
        
        private Texture2D originMap_cage;
        private Texture2D directionMap_cage;
        private Texture2D highpoly_WorldSpaceNormals;
        private Texture2D originMap_highpoly;
        private Texture2D directionMap_highpoly;
        private Texture2D layersMap;
        private Material[] materials;
        private Mesh highMesh;
        private int[] highMeshTris;
        private Vector3[] highMeshVertices;
        private bool _finished = false;
        private bool _aborted = false;
//        private  bool _unreadableMaps = false;
        private List<ImportUtility.ModelToReimport> lowpolyModelsToReimport; // all the models whose meshes are in the lowpoly object (a prefab may contain meshes from different objects)
        private List<ImportUtility.ModelToReimport> highpolyModelsToReimport; // all the models whose meshes are in the highpoly object (a prefab may contain meshes from different objects)
        private List<ImportUtility.ModelToReimport> cageModelsToReimport; // all the models whose meshes are in the cage object (a prefab may contain meshes from different objects)
        private GameObject last_lowpoly;
        private GameObject last_highpoly;
        private GameObject last_cage;
        private bool hasOneMaterialPerMesh_highpoly = true;
        private bool hasOneMaterialPerMesh_lowpoly = true;
        private bool hasMissingMaterials;
        private Vector3[] aoRaysDirections; //ao direction rays

        private Texture[] maps;

        //temporary scene variables
        private Camera instance_camera;
        private Vector3 camPos;

        private Quaternion camRot;

        //private bool firstSpawn;
//        private List<Mesh> instantiatedMeshes;
        
        //lightning
        private int maxSupportedLights = 100;
        private Texture2D high2LowWpos; //each pixel of this texture will contain the highpoly's corresponding point's w-space position after the bake
        private Texture2D high2LowNormal; //each pixel of this texture will contain the highpoly's corresponding point's w-space normal after the bake
        private float[] lightsValidation;
        private float[] lightsTypes;
        private Vector4[] lightsPositions;
        private Vector4[] lightsDirections;
        private Color[] lightsColors;
        private float[] lightsIntensities;
        private float[] lightsRanges;
        private float[] lightsOuterCosAngles; //precomputed: cos(spot_angle)
        private float[] lightsInnerCosAngles; //precomputed: lightsOuterCosAngle - lightsOuterCosAngles*0.25

   
        
        #endregion



        #region Window variables

        private static TotalBaker window;
        //Opening management
        public bool openedByToolbox;
        private static bool canOpen = true;
        private static string bakingScenePath = "Assets/TotalBaker/Scenes/Total Baker.unity";
        private static Scene[] loadedScenes;
        private static string[] loadedScenesPaths;
        private static Scene previewScene;
        private static string previewScenePath;
        
        private static List<string> fatalErrors;
        private static bool _shadersNotFound; //this should be true whenever there's not something that makes TB not to work

        private const int leftBarWidth = 352;
        private Vector2 scrollPosLightingSettings;
        private Vector2 scrollPosMapsSettings;
        private Vector2 scrollPosTexture;
        private Vector2 scrollPosOptions;
        private bool _dragging = false;
        private readonly string[] optionsToolbarStrings = {"3D Models", "Settings", "Lights", "Output"};
        private int mapsToolbarInt_1 = 0;
        private int mapsToolbarInt_2 = -1;
        private string[] mapsToolbarStrings_1 = {"Diffuse", "Height", "Normal", "Metallic", "Specular"};
        private string[] mapsToolbarStrings_2 = {"AO", "Emissive", "Curvature", "Mask (HDRP)"};
        private int optionsToolbarInt = 0;
        private readonly string[] channelsToolbarStrings = {"RGBA", "RGB", "R", "G", "B", "A"};
        private int channelsToolbarInt = 0;
        private RenderTexture previewTexture = null;
        private Texture2D checkerTex = null;
        private GUIStyle chessArea;
        private GUIStyle tempStyle;
        private GUIStyle visibilityStyle;
        private GUIStyle invisibilityStyle;
        private GUIStyle saveCageStyle;
        private GUIStyle reflectionsOnStyle;
        private GUIStyle reflectionsOffStyle;
        private int scrollSize = 512;
        private float progress = 0;
        private float checkPoint = 0;
        private string bakePercentage = "0";
        private GameObject instance_highpoly;
        private GameObject instance_cage;
        private GameObject instance_lowpoly;
        private GameObject instance_directional;
        private GameObject instance_probe;
        private Material mat_cage;
        private Material mat_lowpoly;
        private List<Material> mats_highpoly;
        private Mesh cageMesh;
        private Dictionary<Mesh, CachedMesh> highpolyMeshes;
        private Dictionary<Texture2D, Texture2D> highpolyReadableTextures;
        private Color tangentColor;
        private ColorSpace lastColorSpace;
        private static RenderTextureReadWrite RTColorSpace;
        private Material mat_cageMeshHighlight;
        private Material mat_highpolyMeshHighlight;
        private Texture originalPreviewTex = null;
        private static Texture2D splitterImage;

        //icons
        private Texture2D icon_bake;
        private Texture2D icon_saveTextures;
        private Texture2D icon_newConfig;
        private Texture2D icon_saveConfig;
        private Texture2D icon_loadConfig;
        private Texture2D icon_folder;
        private Texture2D icon_visible;
        private Texture2D icon_invisible;
        private Texture2D icon_reflectionsOn;
        private Texture2D icon_reflectionsOff;
        private Texture2D icon_refresh;
        private Texture2D icon_saveCage;
        
        private Vector2 mainScrollViewPos;
        
        private bool needsBindingRecalculation;
        private Vector2 cageMeshesScrollViewPos;
        private Vector2 highpolyMeshesScrollViewPos;
        private bool allHighpolyMeshesSelected;
        private bool mixedToggle;
        private SplitterState bindings_splitterState;

        GUIStyle style_labelBold;
        
        
        //these variables are used to save temporary values before Undo.RecordObject
        private string _string;
        private int _int;
        private float _float;
        private bool _bool;
        private Color _color;
        private GameObject _gameObject;
        private Settings.NormalsImportType _normalsImportType;
        private Settings.PreviewMaterialType _previewMaterialType;
        private Settings.PreviewMaterialLegacyBlend _previewMaterialLegacyBlend;
        private Settings.PreviewMaterialStandardBlend _previewMaterialStandardBlend;
        private Settings.NormalsDetectionMode _normalsDetectionMode;
        private Settings.VertexColorsMode _vertexColorsMode;
        private Settings.AOType _AOType;
        private Settings.Resolution _resolution;
        private Settings.DilationMode _dilationMode;
        private Settings.CurvatureChannels _curvatureChannels;
        private Settings.CurvatureDetectionMode _curvatureDetectionMode;

        private bool originalQueriesHitBackfaces;
        private bool _changed;


        #endregion


        #region TB_Settings

        internal static Settings settings;
        

        #endregion

        #region Init window

        [MenuItem("Tools/Total Baker/Open Total Baker")]
        static void Init()
        {
            if (window != null)
            {
                Debug.LogWarning(logPrefix+"Opening multiple Total Baker windows is forbidden.");
                return;
            }
            ManageOpening();
        }

        private static void ManageOpening()
        {
            if (PlayerSettings.colorSpace == ColorSpace.Linear)
            {
                int choice = EditorUtility.DisplayDialogComplex(
                    "Total Baker", 
                    "The color space is set to Linear, but Total Baker only supports Gamma color space. Please, switch to Gamma in order to open Total Baker. When you don't need to use Total Baker you can use Linear color space as usual.", 
                    "Switch to Gamma",
                    "Ignore",
                    "Cancel"
                );
                if (choice == 0)
                {
                    PlayerSettings.colorSpace = ColorSpace.Gamma;
                }
                else
                {
                    return;
                }
            }
            
            bool scenesAreDirty = false;
            int loadedScenesCount = SceneManager.sceneCount;
            loadedScenes = new Scene[loadedScenesCount];
            loadedScenesPaths = new string[loadedScenesCount];
            for (int i = 0; i < loadedScenesCount; i++)
            {
                Scene scene =  SceneManager.GetSceneAt(i);
                loadedScenes[i] = scene;
                loadedScenesPaths[i] = scene.path;
                if(scene.isDirty)
                {
                    scenesAreDirty = true;
                }
            }

            if (scenesAreDirty)
            {
                int answer = EditorUtility.DisplayDialogComplex(
                    "Total Baker",
                    "Total Baker will open a dedicated scene. Do you want to save the currently open scene(s) before they get closed?",
                    "Yes",
                    "No",
                    "Cancel"
                );
                if (answer == 0){
                    EditorSceneManager.SaveScenes(loadedScenes);
                    canOpen = true;
                }
                else if(answer == 1)
                {
                    canOpen = true;
                }
                else
                {
                    canOpen = false;
                }
            }

            if (canOpen)
            {
                string directory = Application.dataPath + "/TotalBaker/Scenes";
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                previewScenePath = AssetDatabase.GenerateUniqueAssetPath(bakingScenePath);
                previewScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                RenderSettings.ambientMode = AmbientMode.Flat;
                RenderSettings.ambientLight = Color.gray;
                RenderSettings.ambientIntensity = 1;

                bool saveSuccessful = EditorSceneManager.SaveScene(previewScene, previewScenePath, true);
                if (!saveSuccessful)
                {
                    Debug.LogError(logPrefix + "Couldn't save temp scene");
                }

                previewScene = EditorSceneManager.OpenScene(previewScenePath);
                //TODO: save all the collected TBLights (if any) in order to respawn them after after a configuration is loaded
                SceneView lastActiveSceneView = SceneView.lastActiveSceneView;
                if (lastActiveSceneView != null)
                {
                    #if UNITY_2019
                    lastActiveSceneView.sceneLighting = true;
                    #else
                    lastActiveSceneView.m_SceneLighting = true;
                    #endif
                
                }
                OpenWindow(); 
            }
        }
        
        private static void OpenWindow()
        {
            window = (TotalBaker) GetWindow(typeof(TotalBaker));
            window.minSize = new Vector2(250, 250);
            window.maxSize = new Vector2(900, 694);
            window.titleContent = new GUIContent("Total Baker");
            window.Show();
            IsInitialized = true;
        }
        

        //This function will manage missing shaders by closing the window and logging an error
        private static Shader GetShaderIfExists(string shader)
        {
            Shader s = Shader.Find(shader);
            if (s == null)
            {
                string message = "Shader \"" + shader + "\" does not exist. Please, reimport the Total Baker package.";
                _shadersNotFound = true;
                fatalErrors.Add(message);
                Debug.LogError(logPrefix + message);
            }

            return s;
        }
        
        private void OnBeforeAssemblyReload()
        {
            SaveSession();
            if (instance_highpoly != null) 
            {
                DestroyImmediate(instance_highpoly);
            }
            
            if (instance_lowpoly != null)
            {
                DestroyImmediate(instance_lowpoly);
            }
            
            if (instance_cage != null)
            {
                DestroyImmediate(instance_cage);
            }
        }
        
        private void OnAfterAssemblyReload()
        {
            previewScenePath = EditorPrefs.GetString("TB_PreviewScenePath");
            previewScene = SceneManager.GetSceneByPath(previewScenePath);
            loadedScenesPaths = EditorPrefs_GetStringArray("TB_LoadedScenesPaths");
            loadedScenes = new Scene[loadedScenesPaths.Length];
            for (int i = 0; i < loadedScenes.Length; i++)
            {
                loadedScenes[i] = SceneManager.GetSceneByPath(loadedScenesPaths[i]);
            }
            Initialize();
            AssignBakedTextures();
            SetLowpolyMaterialKeywords();
        }

        private void SetLowpolyMaterialKeywords()
        {
            if (settings.createHeightMap) mat_lowpoly.EnableKeyword("_PARALLAXMAP");
            if (settings.createNormalMap) mat_lowpoly.EnableKeyword("_NORMALMAP");
            if (settings.createMetallicMap) mat_lowpoly.EnableKeyword("_METALLICGLOSSMAP");
            if (settings.createSpecularMap) mat_lowpoly.EnableKeyword("_SPECGLOSSMAP");
            if (settings.createEmissiveMap)
            {
                mat_lowpoly.EnableKeyword("_EMISSION");
                mat_lowpoly.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                if (mat_lowpoly.HasProperty("_EmissionColor"))
                {
                    mat_lowpoly.SetColor(_EmissionColor, Color.white); 
                }
            }
        }

        private void AssignBakedTextures()
        {
            if (settings.createDiffuseMap) mat_lowpoly.SetTexture(_MainTex, outputRTDiffuse);
            if (settings.createHeightMap) mat_lowpoly.SetTexture(_ParallaxMap, outputRTHeight);
            if (settings.createNormalMap)
            {
                if (settings.bakeExistingNormalMap)
                {
                    //pack normals only if the current version doesn't support auto-packing  
                    #if !UNITY_2017_3_OR_NEWER 
                        Pack(outputRTNormalCombined, packedNormalMap);
                        lowpolyMat.SetTexture(_BumpMap", packedNormalMap);
                    #else
                    mat_lowpoly.SetTexture(_BumpMap, outputRTNormalCombined);
                    #endif
                }
                else
                {
                    //pack normals only if the current version doesn't support auto-packing
                    #if !UNITY_2017_3_OR_NEWER
                        Pack(outputRTNormalMain, packedNormalMap);
                        lowpolyMat.SetTexture(_BumpMap", packedNormalMap);
                    #else
                    mat_lowpoly.SetTexture(_BumpMap, outputRTNormalMain);
                    #endif
                }
            }

            if (settings.createMetallicMap) mat_lowpoly.SetTexture(_MetallicGlossMap, outputRTMetallic);
            if (settings.createSpecularMap) mat_lowpoly.SetTexture(_SpecGlossMap, outputRTSpecular);
            if (settings.createAOMap && !settings.bakeAOIntoDiffuse) mat_lowpoly.SetTexture(_OcclusionMap, outputRTAO);
            if (settings.createEmissiveMap) mat_lowpoly.SetTexture(_EmissionMap, outputRTEmissive);
        }


        void OnEnable() 
        {
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
        }

        void OnDisable()
        { 
            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
        }
        
        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
        
            Undo.undoRedoPerformed += OnUndoPerformed;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            if (fatalErrors != null) fatalErrors.Clear();

            fatalErrors = new List<string>();

            //Check existence of all the needed shaders
            bool missingShaders = Filters.Init();

            if (missingShaders) return;

            RTColorSpace = PlayerSettings.colorSpace == ColorSpace.Linear ? RenderTextureReadWrite.Linear : RenderTextureReadWrite.sRGB;
            lastColorSpace = PlayerSettings.colorSpace;

            SetupMaterials();

            if (checkerTex != null) DestroyImmediate(checkerTex);
            checkerTex = CreateCheckerTexture(resolution);

            LoadIcons();

            tangentColor = new Color(0.5f, 0.5f, 1f, 1f);

            SetupLightsDataArrays();

            DestroyInstances();

            if (!openedByToolbox)
            {
                PrepareScene(false);
                LoadLastSessionOrCreateNew();
            }
            
            CenterView();
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                EditorApplication.isPlaying = false;
                Debug.LogWarning(logPrefix + "Play Mode was disabled by Total Baker. Close the Total Baker window to enter Play Mode.");
            }
        }

        private void OnDestroy()
        {
            if (Undo.undoRedoPerformed != null)
            {
                Undo.undoRedoPerformed -= OnUndoPerformed;
            }
            
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;

            if (!openedByToolbox)
            {
                SaveSession();
            }

            if (outputRTDiffuse_original != null)  DestroyImmediate(outputRTDiffuse_original);
            if (outputRTNormalMain_original != null)  DestroyImmediate(outputRTNormalMain_original);
            if (outputRTNormalDetails_original != null)  DestroyImmediate(outputRTNormalDetails_original);
            if (outputRTMetallic_original != null)  DestroyImmediate(outputRTMetallic_original);
            if (outputRTSpecular_original != null)  DestroyImmediate(outputRTSpecular_original);
            if (outputRTAO_original != null)  DestroyImmediate(outputRTAO_original);
            if (outputRTEmissive_original != null)  DestroyImmediate(outputRTEmissive_original);
            if (outputRTCurvature_original != null)  DestroyImmediate(outputRTCurvature_original);
            if (previewTexture != null) DestroyImmediate(previewTexture);
            if (outputRTDiffuse != null) DestroyImmediate(outputRTDiffuse);
            if (outputRTHeight != null) DestroyRT(outputRTHeight);
            if (outputRTNormalMain_original != null) DestroyRT(outputRTNormalMain_original);
            if (outputRTNormalMain != null) DestroyRT(outputRTNormalMain);
            if (outputRTNormalDetails != null) DestroyRT(outputRTNormalDetails);
            if (outputRTNormalCombined != null) DestroyRT(outputRTNormalCombined);
            if (outputRTAO != null) DestroyRT(outputRTAO);
            if (outputRTAO_original != null) DestroyRT(outputRTAO_original);
            if (outputRTEmissive != null) DestroyRT(outputRTEmissive);
            if (outputRTCurvature != null) DestroyRT(outputRTCurvature);
            if (outputRTMask != null) DestroyRT(outputRTMask);
            if (outputTex_uv != null) DestroyImmediate(outputTex_uv);
            if (outputTex_diffuse != null) DestroyImmediate(outputTex_diffuse);
            if (outputTex_height != null) DestroyImmediate(outputTex_height);
            if (outputTex_normalMain != null) DestroyImmediate(outputTex_normalMain);
            if (outputTex_normalCombined != null) DestroyImmediate(outputTex_normalCombined);
            if (outputTex_normalDetails != null) DestroyImmediate(outputTex_normalDetails);
            if (outputRTNormalCombined != null) DestroyImmediate(outputRTNormalCombined);
            if (outputTex_metallic != null) DestroyImmediate(outputTex_metallic);
            if (outputTex_specular != null) DestroyImmediate(outputTex_specular);
            if (outputTex_ao != null) DestroyImmediate(outputTex_ao);
            if (outputTex_emissive != null) DestroyImmediate(outputTex_emissive);
            if (outputTex_curvature != null) DestroyImmediate(outputTex_curvature);
            if (originMap_cage != null) DestroyImmediate(originMap_cage);
            if (layersMap != null) DestroyImmediate(layersMap);
            if (directionMap_cage != null) DestroyImmediate(directionMap_cage);
            if (highpoly_WorldSpaceNormals != null) DestroyImmediate(highpoly_WorldSpaceNormals);
            if (originMap_highpoly != null) DestroyImmediate(originMap_highpoly);
            if (directionMap_highpoly != null) DestroyImmediate(directionMap_highpoly);
            if (mat_cage != null) DestroyImmediate(mat_cage);
            if (mat_lowpoly != null) DestroyImmediate(mat_lowpoly);
            if (mat_cageMeshHighlight != null) DestroyImmediate(mat_cageMeshHighlight);
            if (mat_highpolyMeshHighlight != null) DestroyImmediate(mat_highpolyMeshHighlight);
            if (packedNormalMap != null) DestroyImmediate(packedNormalMap);
            if (high2LowWpos != null) DestroyImmediate(high2LowWpos);
            if (high2LowNormal != null) DestroyImmediate(high2LowNormal);
            if (checkerTex != null) DestroyImmediate(checkerTex);

            Filters.Destroy();
            
            if (checkerTex != null) DestroyImmediate(checkerTex);
            if (splitterImage != null) DestroyImmediate(splitterImage);

            if (highpolyMeshes != null && highpolyMeshes.Count > 0)
            {
                foreach (CachedMesh m in highpolyMeshes.Values)
                {
                    m.DestroyAllCachedTextures();
                }
                highpolyMeshes.Clear(); //Garbage collector should delete the CachedMesh instances in this dictionary
                highpolyMeshes = null;
            }
            
            if (mats_highpoly != null)
            {
                for (int i = 0; i < mats_highpoly.Count; i++)
                {
                    DestroyImmediate(mats_highpoly[i]);
                }
            }
            RemoveTBLights();
            
            high2LowWpos = null;
            high2LowNormal = null;
            lightsValidation = null;
            lightsTypes = null;
            lightsPositions = null;
            lightsDirections = null;
            lightsColors = null;
            lightsIntensities = null;
            lightsRanges = null;
            lightsOuterCosAngles = null;
            lightsInnerCosAngles = null;
            lightsColors = null;
            TBLights = null;
            aoRaysDirections = null;

            DestroyInstances();
             
            if (loadedScenesPaths != null && loadedScenesPaths.Length > 0)
            {
                for (var i = 0; i < loadedScenesPaths.Length; i++)
                {
                    string scenePath = loadedScenesPaths[i];
                    if(scenePath != "") //should also check if the scene is valid
                    {
                        EditorSceneManager.OpenScene(scenePath, i==0?OpenSceneMode.Single:OpenSceneMode.Additive);
                    }
                }
            }

            if (File.Exists(previewScenePath))
            {
                File.Delete(previewScenePath);
                string metaPath = previewScenePath + ".meta";
                if (File.Exists(metaPath))
                {
                    File.Delete(metaPath);
                }
            }

            ImportUtility.ReimportModels(lowpolyModelsToReimport, false);
            ImportUtility.ReimportModels(highpolyModelsToReimport, false);
            ImportUtility.ReimportModels(cageModelsToReimport, false);
            
            AssetDatabase.Refresh();

            window = null;
            
            GC.Collect(); 
        } 

        #endregion



        #region GUI functions


        private void OnGUI()
        {
            //show errors if any
            if (_shadersNotFound)
            {
                foreach (string error in fatalErrors)
                {
                    EditorGUILayout.HelpBox(error, MessageType.Error);
                }

                return;
            }

            mainScrollViewPos = GUI.BeginScrollView(new Rect(0,0, position.width, position.height), mainScrollViewPos, new Rect(0, 0, 900, 670), false, false);
            GUILayout.BeginArea(new Rect(0,0,900, 688));

            if (lastColorSpace != PlayerSettings.colorSpace)
            {
                if (checkerTex != null)
                {
                    DestroyImmediate(checkerTex);
                }
                checkerTex = CreateCheckerTexture(resolution);
                if (PlayerSettings.colorSpace == ColorSpace.Linear)
                {
                    int choice = EditorUtility.DisplayDialogComplex(
                        "Total Baker", 
                        "The color space has been changed to Linear, but Total Baker only supports Gamma color space. Please, switch back to Gamma. When you don't need to use Total Baker you can use Linear color space as usual.", 
                        "Switch to Gamma",
                         "Ignore",
                         "Cancel"
                    );
                    if (choice == 0)
                    {
                        PlayerSettings.colorSpace = ColorSpace.Gamma;
                        if (checkerTex != null)
                        {
                            DestroyImmediate(checkerTex);
                        }
                        checkerTex = CreateCheckerTexture(resolution);
                    }
                }
            }
            lastColorSpace = PlayerSettings.colorSpace;
            
            ScriptableObject target = this;
            SerializedObject serializedObject = new SerializedObject(target);
            SerializedProperty lightsProperty = serializedObject.FindProperty("lights");

            serializedObject.Update();

            //GUI Styles
            chessArea = new GUIStyle(GUI.skin.box);
            chessArea.normal.background = checkerTex;
            chessArea.margin = new RectOffset(0, 0, 0, 0);
            chessArea.padding = new RectOffset(0, 0, 0, 0);
            style_labelBold = new GUIStyle
            {
                fontStyle = FontStyle.Bold, 
                padding = new RectOffset(4,4,4,4)
            };

            tempStyle = new GUIStyle(GUI.skin.label);

            if (splitterImage == null)
            {
                splitterImage = new Texture2D(1, 1, TextureFormat.ARGB32, false)
                {
                    hideFlags = HideFlags.HideAndDontSave,
                    anisoLevel = 0,
                    filterMode = FilterMode.Point,
                    wrapMode = TextureWrapMode.Clamp
                };
                splitterImage.SetPixels(new[] {Color.gray});
                splitterImage.Apply();
            }

            GUIStyle splitterStyle = new GUIStyle()
            {
                normal = new GUIStyleState() {background = splitterImage},
                imagePosition = ImagePosition.ImageOnly,
                wordWrap = false,
                alignment = TextAnchor.MiddleCenter,
            }
            .Named("HSplitter")
            .Size(1, 0, false, true)
            .ResetBoxModel()
            .Margin(3, 3, 0, 0)
            .ClipText();
        // *INDENT-ON*
        

            GUIContent content; //use for labels and tooltips
            
            if (settings == null)
            {
                LoadLastSessionOrCreateNew();
            }
            
            GUILayout.BeginArea(new Rect(4,4, leftBarWidth, 684));
            
            GUILayout.BeginHorizontal(new GUIStyle(EditorStyles.helpBox));
            
            
            content = new GUIContent(icon_newConfig, "New configuration");
            if (GUILayout.Button(content, GUILayout.Width(32), GUILayout.Height(32)))
            {
                CreateNewConfig();
            }
            
            content = new GUIContent(icon_loadConfig, "Load configuration");
            if (GUILayout.Button(content, GUILayout.Width(32), GUILayout.Height(32)))
            {
                LoadConfiguration();
            }
            
            content = new GUIContent(icon_saveConfig, "Save configuration");
            if (GUILayout.Button(content, GUILayout.Width(32), GUILayout.Height(32)))
            {
                SaveConfiguration();
            }
            
            
            GUI.enabled = 
                (settings.autoGenerateCage && settings.lowpoly || settings.cage) &&
                settings.highpoly &&
                (settings.createDiffuseMap || settings.createHeightMap || settings.createNormalMap || settings.createMetallicMap || settings.createSpecularMap || settings.createAOMap || settings.createEmissiveMap || settings.createCurvatureMap);
            
            content = new GUIContent(icon_bake, "Bake");

            if (GUILayout.Button(content, GUILayout.Width(34), GUILayout.Height(32)))
            {
                bool success = Bake();
                if (settings.showConsoleLog)
                {
                    if (success) Debug.Log(logPrefix + "Successfully baked texture maps.");
                    else Debug.Log(logPrefix + "Process aborted.");
                }
            }

            if (settings == null)
            {
                GUILayout.Space(30);
                EditorGUILayout.HelpBox("Please, assign a configuration.", MessageType.Error);
                return;
            }
            
            GUI.enabled = _finished;

            content = new GUIContent(icon_saveTextures, "Save all generated textures");
            if (GUILayout.Button(content, GUILayout.Width(32), GUILayout.Height(32)))
            {
                SaveManager.SaveTextures();
            }
            
            GUI.enabled = true;
            GUI.backgroundColor = Color.white;
            GUIStyle style;
            
            
            //config name 
            GUILayout.Space(5);
            GUILayout.Label("", splitterStyle, GUILayout.Width(2), GUILayout.Height(36));
            GUILayout.Space(5);
            GUILayout.BeginVertical();
            GUILayout.Label("Current configuration", EditorStyles.boldLabel);
            GUILayout.Label(settings.name+(settings.isTemporary?"*":""));
            GUILayout.EndVertical();
            
            
            GUILayout.EndHorizontal();
            
            optionsToolbarInt = GUILayout.Toolbar(optionsToolbarInt, optionsToolbarStrings);
            
            #region 3D Models Panel
            
            if (optionsToolbarInt == 0)
            {
                //LOWPOLY
                GUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox));
                EditorGUILayout.LabelField("Lowpoly model", EditorStyles.boldLabel);
                content = new GUIContent("Object", "The low poly object.");

                _gameObject = (GameObject) EditorGUILayout.ObjectField(content, settings.lowpoly, typeof(GameObject), false);
                if (_gameObject != settings.lowpoly)
                {
                    RecordUndo( "Lowpoly object");
                    settings.lowpoly = _gameObject;
                    if (settings.lowpoly != last_lowpoly)
                    {
                        OnLowpolyChanged(true);
                    }
                }
                
                _normalsImportType = (Settings.NormalsImportType) EditorGUILayout.EnumPopup("Normals", settings.importNormalsLowpoly);
                if (_normalsImportType != settings.importNormalsLowpoly)
                {
                    RecordUndo("Lowpoly normals import type");
                    settings.importNormalsLowpoly = _normalsImportType;
                }

                if (settings.importNormalsLowpoly == Settings.NormalsImportType.Calculate)
                {
                    _int = EditorGUILayout.IntSlider("Smoothing Angle", settings.smoothAngleLowpoly, 0, 180);
                    if (_int != settings.smoothAngleLowpoly)
                    {
                        RecordUndo("Lowpoly smoothing angle");
                        settings.smoothAngleLowpoly = _int;
                    }
                }
                
                RecalculateReflectionStyles();
                
                GUILayout.BeginHorizontal();
            
                _previewMaterialType = (Settings.PreviewMaterialType) EditorGUILayout.EnumPopup("Shader", settings.previewMaterialTypeLowpoly);

                //show reflections icon only if the selected shader is Standard
                if (ShaderUtils.IsStandard(settings.previewMaterialTypeLowpoly))
                {
                    style = settings.reflectionsLowpoly ? reflectionsOnStyle : reflectionsOffStyle;
                    _bool = EditorGUILayout.Toggle(settings.reflectionsLowpoly, style, GUILayout.Width(16));
                    if (_bool != settings.reflectionsLowpoly)
                    {
                        RecordUndo("Show lowpoly reflections");
                        settings.reflectionsLowpoly = _bool;
                        UpdateLowpolyReflections();
                    }
                }

                GUILayout.EndHorizontal();  


                if (_previewMaterialType != settings.previewMaterialTypeLowpoly)
                {
                    RecordUndo("Lowpoly preview material type");
                    settings.previewMaterialTypeLowpoly = _previewMaterialType;
                    UpdateLowpolyShader();
                }

                if (ShaderUtils.IsStandard(_previewMaterialType))
                {
                    _previewMaterialStandardBlend = (Settings.PreviewMaterialStandardBlend) EditorGUILayout.EnumPopup("Blend mode", settings.previewMaterialBlendStandardLowpoly);
                    if (_previewMaterialStandardBlend != settings.previewMaterialBlendStandardLowpoly)
                    {
                        RecordUndo("Lowpoly preview material blend mode");
                        settings.previewMaterialBlendStandardLowpoly = _previewMaterialStandardBlend;
                        UpdateLowpolyShader();
                    }
                }
                else
                {
                    _previewMaterialLegacyBlend = (Settings.PreviewMaterialLegacyBlend) EditorGUILayout.EnumPopup("Blend mode", settings.previewMaterialBlendLegacyLowpoly);
                    if (_previewMaterialLegacyBlend != settings.previewMaterialBlendLegacyLowpoly)
                    {
                        RecordUndo("Lowpoly preview material blend mode");
                        settings.previewMaterialBlendLegacyLowpoly = _previewMaterialLegacyBlend;
                        UpdateLowpolyShader();
                    }
                }
                

                GUILayout.EndVertical();

                //HIGHPOLY
                GUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox));
                EditorGUILayout.LabelField("Highpoly model", EditorStyles.boldLabel);
                content = new GUIContent("Object","The high poly object. It can be both the parent object of multiple submeshes or a single high poly mesh.");

                _gameObject = (GameObject) EditorGUILayout.ObjectField(content, settings.highpoly, typeof(GameObject), false);
                if (_gameObject != settings.highpoly)
                {
                    RecordUndo( "Highpoly object");
                    settings.highpoly = _gameObject;
                    if (settings.highpoly != last_highpoly)
                    {
                        OnHighpolyChanged(true);
                    }
                }

                if (hasMissingMaterials)
                {
                    GUILayout.BeginVertical();
                    EditorGUILayout.HelpBox("The high poly model has missing materials. Please, assign all the needed materials.", MessageType.Error);
                    GUILayout.EndVertical();
                }
                
                _normalsImportType = (Settings.NormalsImportType) EditorGUILayout.EnumPopup("Normals", settings.importNormalsHighpoly);
                if (_normalsImportType != settings.importNormalsHighpoly)
                {    
                    RecordUndo("Highpoly normals import type");
                    settings.importNormalsHighpoly = _normalsImportType;
                }

                if (settings.importNormalsHighpoly == Settings.NormalsImportType.Calculate)
                {
                    _int = EditorGUILayout.IntSlider("Smoothing Angle", settings.smoothAngleHighpoly, 0, 180);
                    if (_int != settings.smoothAngleHighpoly)
                    {    
                        RecordUndo("Highpoly smoothing angle");
                        settings.smoothAngleHighpoly = _int;
                    }
                }
                
                
                GUILayout.BeginHorizontal();
                
                _previewMaterialType = (Settings.PreviewMaterialType) EditorGUILayout.EnumPopup("Shader", settings.previewMaterialTypeHighpoly);
                
                style = settings.reflectionsHighpoly ? reflectionsOnStyle : reflectionsOffStyle;
                _bool = EditorGUILayout.Toggle(settings.reflectionsHighpoly, style, GUILayout.Width(16));
                if (_bool != settings.reflectionsHighpoly)
                {
                    RecordUndo("Show highpoly reflections");
                    settings.reflectionsHighpoly = _bool;
                    UpdateHighpolyReflections();
                }
                
                GUILayout.EndHorizontal();
                
                if (_previewMaterialType != settings.previewMaterialTypeHighpoly)
                {
                    RecordUndo("Highpoly preview material type");
                    settings.previewMaterialTypeHighpoly = _previewMaterialType;
                    UpdateHighpolyShader();
                }
                
                if (ShaderUtils.IsStandard(_previewMaterialType))
                {
                    _previewMaterialStandardBlend = (Settings.PreviewMaterialStandardBlend) EditorGUILayout.EnumPopup("Blend mode", settings.previewMaterialBlendStandardHighpoly);
                    if (_previewMaterialStandardBlend != settings.previewMaterialBlendStandardHighpoly)
                    {
                        RecordUndo("Highpoly preview material blend mode");
                        settings.previewMaterialBlendStandardHighpoly = _previewMaterialStandardBlend;
                        UpdateHighpolyShader();
                    }
                }
                else
                {
                    _previewMaterialLegacyBlend = (Settings.PreviewMaterialLegacyBlend) EditorGUILayout.EnumPopup("Blend mode", settings.previewMaterialBlendLegacyHighpoly);
                    if (_previewMaterialLegacyBlend != settings.previewMaterialBlendLegacyHighpoly)
                    {
                        RecordUndo("Highpoly preview material blend mode");
                        settings.previewMaterialBlendLegacyHighpoly = _previewMaterialLegacyBlend;
                        UpdateHighpolyShader();
                    }
                }

                GUILayout.EndVertical();
                
                
                //CAGE
                RecalculateVisibilityToggleStyles();
                
                saveCageStyle = new GUIStyle();
                saveCageStyle.active.background = icon_saveCage;
                saveCageStyle.normal.background = icon_saveCage;
                saveCageStyle.focused.background = icon_saveCage;
                saveCageStyle.hover.background = icon_saveCage;
                saveCageStyle.border = new RectOffset(0, 0, 0, 0);

                GUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox));

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Cage", EditorStyles.boldLabel, GUILayout.Width(36));
                
                style = settings.showCage ? visibilityStyle : invisibilityStyle;

                _bool = EditorGUILayout.Toggle(settings.showCage, style, GUILayout.Width(13));
                if (_bool != settings.showCage && instance_cage != null)
                {
//                    RecordUndo("Show cage");
                    settings.showCage = _bool;
                }

                if (instance_cage != null)
                {
                    if (settings.showCage != instance_cage.activeInHierarchy)
                    {
                        instance_cage.SetActive(settings.showCage);
                        SceneView.RepaintAll();
                    }
                }

//                todo: enable this code when the save cage to .obj will support multiple meshes 
//                content = new GUIContent(icon_saveCage, "Save this cage as .obj in Assets/Cages");
//                if (settings.autoGenerateCage && instance_cage != null && GUILayout.Button(content, EditorStyles.label, GUILayout.MaxHeight(16), GUILayout.MaxWidth(16)))
//                {
//                    string path = "Assets/Cages/" + settings.lowpoly.name + "_cage.obj";
//                    if (File.Exists(path))
//                    {
//                        if (EditorUtility.DisplayDialogComplex("Overwrite file", "The file " + path + " already exists. Overwrite file?", "Yes", "No", "Cancel") == 0)
//                        {
//                            MeshToObj(instance_cage.GetComponent<MeshFilter>(), instance_lowpoly.transform, path);
//                            AssetDatabase.Refresh();
//                            EditorUtility.DisplayDialog("Cage saved", "The cage has been saved in " + path, "Ok");
//                        }
//                    }
//                    else
//                    {
//                        MeshToObj(instance_cage.GetComponent<MeshFilter>(), instance_lowpoly.transform, path);
//                        AssetDatabase.Refresh();
//                        EditorUtility.DisplayDialog("Cage saved", "The cage has been saved in " + path, "Ok");
//                    }
//                }

                GUILayout.EndHorizontal();

                content = new GUIContent("Auto Generate Cage","If enabled, the cage will be automatically generated from the low poly object expanding its vertices by Cage Offset.");

                _bool = EditorGUILayout.Toggle(content, settings.autoGenerateCage);
                if (_bool != settings.autoGenerateCage)
                {
                    RecordUndo("Auto generate cage");
                    settings.autoGenerateCage = _bool;
                    needsBindingRecalculation = true;
                }
                if (settings.autoGenerateCage)
                {
                    content = new GUIContent("Cage Offset","To generate the cage, each vertex of the low poly model will be moved by this value along its normal.");

                    _float = EditorGUILayout.Slider(content, settings.cageOffset, 0, 0.1f);
                    if (!Mathf.Approximately(_float, settings.cageOffset))
                    {
                        RecordUndo("Cage offset");
                        settings.cageOffset = _float;
                    }
                }
                else
                {
                    content = new GUIContent("Object","The cage object. It should contain the high poly object in each point and follow its shape as much as possibile.");
                    
                    _gameObject = (GameObject) EditorGUILayout.ObjectField(content, settings.cage, typeof(GameObject), false);
                    if (_gameObject != settings.cage)
                    {
                        RecordUndo("Cage object");
                        settings.cage = _gameObject;
                        if (settings.cage != last_cage)
                        {
                            OnCageChanged(true);
                        }
                    }
                }

                if (!settings.autoGenerateCage)
                {
                    _normalsImportType = (Settings.NormalsImportType) EditorGUILayout.EnumPopup("Normals", settings.importNormalsCage);
                    if (_normalsImportType != settings.importNormalsCage)
                    {
                        RecordUndo("Cage normals import type");
                        settings.importNormalsCage = _normalsImportType;
                    }

                    if (settings.importNormalsCage == Settings.NormalsImportType.Calculate)
                    {
                        _int = EditorGUILayout.IntSlider("Smoothing Angle", settings.smoothAngleCage, 0, 180);
                        if (_int != settings.smoothAngleCage)
                        {
                            RecordUndo("Cage smoothing angle");
                            settings.smoothAngleCage = _int;
                        }
                    }
                }

                if (settings.highpoly && (settings.autoGenerateCage || (!settings.autoGenerateCage && settings.cage != null)))
                {
                    content = new GUIContent("Bindings", "Enable bindings settings. For each cage mesh, you will be able to select the highpoly meshes that will be used for raycasting. In this way you can exclude some meshes and avoid rays touch undesired surfaces.");

                    _bool = EditorGUILayout.Toggle(content, settings.bindMeshes);
                    if (_bool != settings.bindMeshes)
                    {
                        RecordUndo("Bindings");
                        settings.bindMeshes = _bool;
                        RecalculateHighlights();
                    }
                    if (settings.bindMeshes)
                    {
                        if (needsBindingRecalculation)
                        {
                            EditorGUILayout.HelpBox("Some of the input models have changed. The next time you will press 'Update models in scene' or the bake button the bindings will be reset", MessageType.Warning);
                        }

                        int rows = 0;
                        if (settings.cageMeshObjects != null && settings.highpolyMeshObjects != null)
                        {
                            rows = Mathf.Max(settings.cageMeshObjects.Length, settings.highpolyMeshObjects.Length);
                        }
                        float height = 16 + 24 * rows;
                        float maxHeight = Mathf.Min(157, height);
                        GUILayout.BeginHorizontal(new GUIStyle(EditorStyles.helpBox), GUILayout.MaxHeight(maxHeight), GUILayout.MinHeight(68));

                        if (bindings_splitterState == null) 
                        {
                            float[] splitterRelativeSizes = new float[] {120, 140};
                            int[] splitterMinWidths = new int[] {120, 140};
                            bindings_splitterState = new SplitterState(splitterRelativeSizes, splitterMinWidths, null);
                        }
                        
                        SplitterGUILayout.BeginHorizontalSplit(bindings_splitterState, GUILayout.Height(maxHeight));

                        GUILayout.BeginVertical();

                        GUILayout.BeginHorizontal();
                            GUILayout.Label("Cage meshes", style_labelBold);
                            RecalculateVisibilityToggleStyles();
                            style = settings.highlightCurrentCageMesh ? visibilityStyle : invisibilityStyle;
                            EditorGUI.BeginChangeCheck();
                            settings.highlightCurrentCageMesh = EditorGUILayout.Toggle(settings.highlightCurrentCageMesh, style, GUILayout.Width(13));
                            if (EditorGUI.EndChangeCheck())
                            {
                                RecalculateHighlights();
                            }
                            GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                        cageMeshesScrollViewPos = GUILayout.BeginScrollView(cageMeshesScrollViewPos, false, false);
                        if (instance_cage != null && settings.cageMeshObjects != null)
                        {
                            int _index = settings.selectedCageMeshIndex;
                            
                            _int =  ToggleList(settings.selectedCageMeshIndex, settings.cageMeshNames);
                            if (_int != settings.selectedCageMeshIndex)
                            {
                                RecordUndo("Selected cage mesh");
                                settings.selectedCageMeshIndex = _int;
                                RecalculateMixedToggle();
                            }
                            if (settings.selectedCageMeshIndex != _index)
                            {
                                RecalculateHighlights();
                            }
                        }

                        GUILayout.EndScrollView();
                        GUILayout.EndVertical();

                        GUILayout.BeginHorizontal();
                        
                        GUILayout.Box("", splitterStyle, GUILayout.Width(1), GUILayout.Height(maxHeight));
                        
                        GUILayout.BeginVertical();
                        
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Highpoly meshes", style_labelBold);
                        RecalculateVisibilityToggleStyles();
                        style = settings.highlightCurrentBoundMeshes ? visibilityStyle : invisibilityStyle;
                        EditorGUI.BeginChangeCheck();
                        settings.highlightCurrentBoundMeshes = EditorGUILayout.Toggle(settings.highlightCurrentBoundMeshes, style, GUILayout.Width(13));
                        if (EditorGUI.EndChangeCheck())
                        {
                            RecalculateHighlights();
                        }
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                        
                        highpolyMeshesScrollViewPos = GUILayout.BeginScrollView(highpolyMeshesScrollViewPos, false, false);
                        
                        EditorGUI.BeginChangeCheck();
                        allHighpolyMeshesSelected = GUILayout.Toggle(allHighpolyMeshesSelected, "All", mixedToggle?"ToggleMixed":EditorStyles.toggle);
                        if (EditorGUI.EndChangeCheck())
                        {
                            OnHighpolyMeshesSelectAllChanged();
                            RecalculateMixedToggle();
                        }
                        
                        if (instance_cage != null && settings.cageMeshObjects != null)
                        {
                            for (var i = 0; i < settings.highpolyMeshNames.Length; i++)
                            {
                                string meshName = settings.highpolyMeshNames[i];
                                bool isSelected = settings.meshBindings[settings.selectedCageMeshIndex].selections[i].isSelected;
                                _bool = GUILayout.Toggle(isSelected, meshName, EditorStyles.toggle);
                                if (_bool != isSelected)
                                {
                                    RecordUndo("Highpoly mesh selected");
                                    settings.meshBindings[settings.selectedCageMeshIndex].selections[i].isSelected = _bool;
                                    RecalculateHighlights();
                                    RecalculateMixedToggle();
                                }
                            }
                        }

                        GUILayout.EndScrollView();
                        GUILayout.EndHorizontal();
                        SplitterGUILayout.EndHorizontalSplit();
                        GUILayout.EndVertical();
                        
                        GUILayout.EndHorizontal();

                    }
                }
                
                GUILayout.EndVertical();

                bool validModels = ((settings.autoGenerateCage && settings.lowpoly) || settings.cage) && settings.highpoly && !hasMissingMaterials;
                
                if (validModels)
                {
                    GUILayout.BeginHorizontal(); 
                    
                    if (GUILayout.Button("Update models in scene"))
                    {
                        List<ImportUtility.ModelToReimport> needReimport = new List<ImportUtility.ModelToReimport>();
                        needReimport.AddRange(ImportUtility.CheckReimportNeeded(settings.highpoly, settings.importNormalsHighpoly, settings.smoothAngleHighpoly));
                        needReimport.AddRange(ImportUtility.CheckReimportNeeded(settings.lowpoly, settings.importNormalsLowpoly, settings.smoothAngleLowpoly));
                        if (!settings.autoGenerateCage)
                        {
                            needReimport.AddRange(ImportUtility.CheckReimportNeeded(settings.cage, settings.importNormalsCage, settings.smoothAngleCage));
                        }

                        if (needReimport.Count > 0)
                        {
                            string message = "";
                            foreach (ImportUtility.ModelToReimport toReimport in needReimport)
                                message += "- Asset " + toReimport.assetPath + " needs to be reimported.\n\n";
                            message += "\nThe reimport process may take some time for large meshes.\n\n";
                            if (EditorUtility.DisplayDialog("Warning", message, "Reimport", "Cancel"))
                            {
                                SpawnObjects();
                            }
                        }
                        else SpawnObjects();
                    }
                    
                    if (GUILayout.Button("Reset view", GUILayout.Width(100)))
                    {
                        CenterView();
                    }
                    
                    GUILayout.EndHorizontal();
                }
            }

            #endregion 


            #region Maps Panel

            else if (optionsToolbarInt == 1)
            {
                GUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox));
                scrollPosMapsSettings = EditorGUILayout.BeginScrollView(scrollPosMapsSettings, false, false, GUIStyle.none, GUI.skin.verticalScrollbar, GUIStyle.none);
                
                GUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox));
                GUILayout.Label("General Settings", EditorStyles.boldLabel);
                
                _float = EditorGUILayout.Slider("Max ray length", settings.maxRayLength, 0, 1f);
                if (!Mathf.Approximately(_float, settings.maxRayLength))
                {
                    RecordUndo("Max ray length");
                    settings.maxRayLength = _float;
                }

                GUILayout.EndVertical();
                
                GUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox));
                if (settings.bakeLights)
                {
                    settings.createDiffuseMap = true;
                    GUILayout.BeginVertical();
                    EditorGUILayout.HelpBox("The creation of the Diffuse Map is forced because the option Bake Lights is active.", MessageType.Warning);
                    GUILayout.EndVertical();
                }
                if (settings.createAOMap && settings.bakeAOIntoDiffuse)
                {
                    settings.createDiffuseMap = true;
                    GUILayout.BeginVertical();
                    EditorGUILayout.HelpBox("The creation of the Diffuse Map is forced because the Ambient Occlusion's option Bake Into Diffuse is active.", MessageType.Warning);
                    GUILayout.EndVertical();
                }
                
                _bool = EditorGUILayout.ToggleLeft("Diffuse Map", settings.createDiffuseMap, EditorStyles.boldLabel);
                if (_bool != settings.createDiffuseMap)
                {
                    RecordUndo("Create diffuse map");
                    settings.createDiffuseMap = _bool;
                }

                if (settings.createAOMap && settings.bakeAOIntoDiffuse)
                {
                    settings.createDiffuseMap = true;
                }

                if (settings.createDiffuseMap)
                {
                    _bool = EditorGUILayout.Toggle("Bake vertex colors", settings.bakeVertexColors);
                    if (_bool != settings.bakeVertexColors)
                    {
                        RecordUndo("Bake vertex colors");
                        settings.bakeVertexColors = _bool;
                    }

                    if (settings.bakeVertexColors)
                    {
                        _vertexColorsMode = (Settings.VertexColorsMode) EditorGUILayout.EnumPopup("Vertex colors mode", settings.vertexColorsMode);
                        if (_vertexColorsMode != settings.vertexColorsMode)
                        {
                            RecordUndo("Vertex colors mode");
                            settings.vertexColorsMode = _vertexColorsMode;
                        }
                    }
                }

                GUILayout.EndVertical();

                GUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox));
                if (settings.createNormalMap && settings.normalsDetectionMode == Settings.NormalsDetectionMode.HeightMap)
                {
                    GUILayout.BeginVertical();
                    EditorGUILayout.HelpBox("The creation of the Height Map is forced because the Normals Detection option is set to Height Map.", MessageType.Warning);
                    GUILayout.EndVertical();
                }
                
                _bool = EditorGUILayout.ToggleLeft("Height Map", settings.createHeightMap, EditorStyles.boldLabel);
                if (_bool != settings.createHeightMap)
                {
                    RecordUndo("Create height map");
                    settings.createHeightMap = _bool;
                }

                if (settings.createHeightMap)
                {
                    content = new GUIContent("16 bit per channel","Use 16 bpc (bits per channel) if you need a high-precision heightmap. This is useful when you turn on the normal map's parameter Normal From Height or if you want to convert the height map into a normal map later with the Height2Normal tool.");
                    _bool = EditorGUILayout.Toggle(content, settings._16bpcHeight);
                    if (_bool != settings._16bpcHeight)
                    {
                        RecordUndo("16bpc height map");
                        settings._16bpcHeight = _bool;
                    }

                }

                GUILayout.EndVertical();

                GUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox));
                
                if (settings.createCurvatureMap && settings.curvatureDetectionMode == Settings.CurvatureDetectionMode.NormalMap)
                {
                    GUILayout.BeginVertical();
                    EditorGUILayout.HelpBox("The creation of the Normal Map is forced because the Curvature Detection option is set to Normal Map.", MessageType.Warning);
                    GUILayout.EndVertical();
                }
                
                _bool = EditorGUILayout.ToggleLeft("Normal Map", settings.createNormalMap, EditorStyles.boldLabel);
                if (_bool != settings.createNormalMap)
                {
                    RecordUndo("Create normal map");
                    settings.createNormalMap = _bool;
                }

                if (settings.createCurvatureMap && settings.curvatureDetectionMode == Settings.CurvatureDetectionMode.NormalMap)
                {
                    settings.createNormalMap = true;
                }
                
                if (settings.createNormalMap)
                {
                    content = new GUIContent("Bake existing map","The high poly model can have a normal map on some of its materials. If this option is checked, existing normal maps will be combined with the computed surface normals. If this option is unchecked, only surface normals will be baked.");
                    
                    _bool = EditorGUILayout.Toggle(content, settings.bakeExistingNormalMap);
                    if (_bool != settings.bakeExistingNormalMap)
                    {
                        RecordUndo("Bake normals details");
                        settings.bakeExistingNormalMap = _bool;
                    }
                    
                    _normalsDetectionMode = (Settings.NormalsDetectionMode) EditorGUILayout.EnumPopup("Normals detection", settings.normalsDetectionMode);
                    if (_normalsDetectionMode != settings.normalsDetectionMode)
                    {
                        RecordUndo("Normals detection");
                        settings.normalsDetectionMode = _normalsDetectionMode;
                    }

                    if (settings.normalsDetectionMode == Settings.NormalsDetectionMode.HeightMap)
                    {
                        settings.createHeightMap = true; //force creation of heightmap       
                    }
                }

                GUILayout.EndVertical();

                GUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox));
                
                if (settings.createMaskMap)
                {
                    GUILayout.BeginVertical();
                    EditorGUILayout.HelpBox("The creation of the Metallic Map is forced because the Mask Map requires it.", MessageType.Warning);
                    GUILayout.EndVertical();
                }
                
                _bool = EditorGUILayout.ToggleLeft("Metallic Map", settings.createMetallicMap, EditorStyles.boldLabel);
                if (_bool != settings.createMetallicMap)
                {
                    RecordUndo("Create metallic map");
                    settings.createMetallicMap = _bool;
                    if (settings.createMetallicMap)
                    {
                        settings.createSpecularMap = false;
                    }
                }
                
                GUILayout.EndVertical();

                GUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox));
                
                _bool = EditorGUILayout.ToggleLeft("Specular Map", settings.createSpecularMap, EditorStyles.boldLabel);
                if (_bool != settings.createSpecularMap)
                {
                    RecordUndo("Create specular map");
                    settings.createSpecularMap = _bool;
                    if (settings.createSpecularMap)
                    {
                        settings.createMetallicMap = false;
                    }
                }
                
                GUILayout.EndVertical();

                GUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox));
                
                if (settings.createMaskMap)
                {
                    GUILayout.BeginVertical();
                    EditorGUILayout.HelpBox("The creation of the AO Map is forced because the Mask Map requires it.", MessageType.Warning);
                    GUILayout.EndVertical();
                }
                
                _bool = EditorGUILayout.ToggleLeft("AO Map", settings.createAOMap, EditorStyles.boldLabel);
                if (_bool != settings.createAOMap)
                {
                    RecordUndo("Create AO map");
                    settings.createAOMap = _bool;
                }

                if (settings.createAOMap)
                {
                    content = new GUIContent("AO Algorithm","Choose Fast to create the AO map using a GPU powered approximated algorithm. Choose Classic to use a raycast-based algorithm (slow).");
                    
                    _AOType = (Settings.AOType) EditorGUILayout.EnumPopup(content, settings.aoType);
                    if (_AOType != settings.aoType)
                    {
                        RecordUndo("AO algorithm");
                        settings.aoType = _AOType;
                    }
                    
                    if (settings.aoType == Settings.AOType.Classic)
                    {
                        _float = EditorGUILayout.Slider("Occlusion Strength",settings.AOStrength, 0, 1);
                        if (!Mathf.Approximately(_float, settings.AOStrength))
                        {
                            RecordUndo("AO strength");
                            settings.AOStrength = _float;
                        }
                        
                        _float = EditorGUILayout.Slider("Max Spread", settings.AOMaxRaySpread, 0, 1);
                        if (!Mathf.Approximately(_float, settings.AOMaxRaySpread))
                        {
                            RecordUndo("AO max ray spread");
                            settings.AOMaxRaySpread = _float;
                        }
                        
                        _int =EditorGUILayout.IntField("Rays Per Point", settings.AORaysPerPoint);
                        if (_int != settings.AORaysPerPoint)
                        {
                            RecordUndo("AO rays per point");
                            settings.AORaysPerPoint = _int;
                        }
                    }
                    else
                    {
                        GUILayout.BeginVertical();
                        EditorGUILayout.HelpBox(
                            "To use the Fast algorithm the high poly model needs to be " +
                            "unwrapped so that its UV Map doesn't contain overlapped faces. " +
                            "If your model doesn't fit this requirement, use the Classic algorithm.",
                            MessageType.Warning
                        );
                        GUILayout.EndVertical();
                    }
                    
                    _bool = EditorGUILayout.Toggle("Bake into diffuse", settings.bakeAOIntoDiffuse);
                    if (_bool != settings.bakeAOIntoDiffuse)
                    {
                        RecordUndo("Bake AO into diffuse");
                        settings.bakeAOIntoDiffuse = _bool;
                        
                        if (settings.bakeAOIntoDiffuse)
                        {
                            settings.createDiffuseMap = true;
                        }

                        if (outputRTDiffuse_original != null && instance_lowpoly != null)
                        {
                            mat_lowpoly.SetTexture(_OcclusionMap, settings.bakeAOIntoDiffuse ? null : outputRTAO);
                            UpdateDiffuseMap(false);
                        }
                    }

                }

                GUILayout.EndVertical();

                GUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox));
                
                _bool = EditorGUILayout.ToggleLeft("Emissive Map", settings.createEmissiveMap, EditorStyles.boldLabel);
                if (_bool != settings.createEmissiveMap)
                {
                    RecordUndo("Create emissive map");
                    settings.createEmissiveMap = _bool;
                }

                GUILayout.EndVertical();
                
                GUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox));
                
                _bool = EditorGUILayout.ToggleLeft("Curvature Map", settings.createCurvatureMap, EditorStyles.boldLabel);
                if (_bool != settings.createCurvatureMap)
                {
                    RecordUndo("Create curvature map");
                    settings.createCurvatureMap = _bool;
                }

                if (settings.createCurvatureMap)
                {
                    _curvatureDetectionMode = (Settings.CurvatureDetectionMode) EditorGUILayout.EnumPopup("Curvature detection", settings.curvatureDetectionMode);
                    if (_curvatureDetectionMode != settings.curvatureDetectionMode)
                    {
                        RecordUndo("Curvature detection mode");
                        settings.curvatureDetectionMode = _curvatureDetectionMode;
                        if (_curvatureDetectionMode == Settings.CurvatureDetectionMode.NormalMap)
                        {
                            settings.createNormalMap = true;
                        }
                    }
                }

                GUILayout.EndVertical();
                
                GUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox));
                
                _bool = EditorGUILayout.ToggleLeft("Mask Map (HDRP Channel Packed)", settings.createMaskMap, EditorStyles.boldLabel);
                if (_bool != settings.createMaskMap)
                {
                    RecordUndo("Create mask map");
                    settings.createMaskMap = _bool;
                }

                if (settings.createMaskMap)
                {
                    settings.createMetallicMap = true;
                    settings.createAOMap = true;
                }

                GUILayout.EndScrollView();
                GUILayout.EndVertical();
                GUILayout.EndVertical();

            }

            #endregion

            #region Lighting panel

            else if (optionsToolbarInt == 2)
            {

                GUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox));
                
                scrollPosLightingSettings = EditorGUILayout.BeginScrollView(scrollPosLightingSettings, false, false, GUIStyle.none, GUI.skin.verticalScrollbar, GUIStyle.none);
                EditorGUILayout.LabelField("Lighting settings", EditorStyles.boldLabel);
                content = new GUIContent("Transfer lights", "Does Total Baker have to bake lights?");
                
                EditorGUI.BeginChangeCheck();
                _bool = EditorGUILayout.Toggle(content, settings.bakeLights);
                if (EditorGUI.EndChangeCheck())
                {
                    RecordUndo("Bake lights");
                    settings.bakeLights = _bool;
                }
                
                if (settings.bakeLights)
                {
                    GUILayout.BeginVertical();
                    EditorGUILayout.HelpBox(
                        "The highpoly meshes must be UV unwrapped without overlapping triangles to successfully transfer lights.",
                        MessageType.Warning
                    );
                    GUILayout.EndVertical();
                    GUILayout.BeginVertical();
                    EditorGUILayout.HelpBox(
                        "When transferring lights, it's better to use the Unlit shader on the lowpoly model and Diffuse shader on the highpoly model. You can change these settings in the 3D Models panel.",
                        MessageType.Warning
                    );
                    GUILayout.EndVertical();
                    
                    settings.createDiffuseMap = true; //force creation of diffuse map

                    GUILayout.BeginHorizontal();
                    content = new GUIContent("Bake all active lights", "If true, the system will bake all the active lights in scene.");
                    EditorGUI.BeginChangeCheck();
                    
                    EditorGUI.BeginChangeCheck();
                    _bool = EditorGUILayout.Toggle(content, settings.bakeAllActiveLights);
                    if (EditorGUI.EndChangeCheck())
                    {
                        RecordUndo("Use all active lights");
                        settings.bakeAllActiveLights = _bool;
                    }
                    
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (settings.bakeAllActiveLights)
                        {
                            CollectAllLights();
                            UpdateLightsData();
                        }
                    }

                    if (settings.bakeAllActiveLights)
                    {

                        content = new GUIContent(icon_refresh, "Refresh all lights");

                        if (GUILayout.Button(content, EditorStyles.label, GUILayout.MaxHeight(16), GUILayout.MaxWidth(16)))
                        {
                            CollectAllLights();
                            UpdateDiffuseMap(true);
                        }

                    }

                    GUILayout.FlexibleSpace();

                    GUILayout.EndHorizontal();

                    content = new GUIContent("Custom ambient color", "Disabling this option, the ambient color will match the one set in RenderSettings");

                    bool isCustomAmbientColorToggleChanged = false;
                    
                    EditorGUI.BeginChangeCheck();
                    _bool = EditorGUILayout.Toggle(content, settings.customAmbientColor);
                    if (EditorGUI.EndChangeCheck())
                    {
                        RecordUndo("Custom ambient color");
                        settings.customAmbientColor = _bool;

                        isCustomAmbientColorToggleChanged = true;
                    }

                    if (settings.customAmbientColor)
                    {
                        content = new GUIContent("Ambient Color", "The color of the ambient lighting");


                        EditorGUI.BeginChangeCheck();
#if UNITY_2018_1_OR_NEWER
                        _color = EditorGUILayout.ColorField(content, settings.ambientColor, true, false, false);
#else
                        _color = EditorGUILayout.ColorField(content, settings.ambientColor, true, false, false, null);
#endif
                        if (EditorGUI.EndChangeCheck()) 
                        {
                            RecordUndo("Ambient color");
                            settings.ambientColor = _color;
                            if (settings.autoUpdatePreview && outputRTDiffuse != null)
                            {
                                UpdateDiffuseMap(true);
                            }
                        }
                        
                    }
                    else
                    {
                        GUI.enabled = false;
                        content = new GUIContent("Ambient Color", "The color of the ambient lighting");
#if UNITY_2018_1_OR_NEWER
                        settings.ambientColor = EditorGUILayout.ColorField(content, RenderSettings.ambientLight, false, false, false);
#else
                        settings.ambientColor = EditorGUILayout.ColorField(content, RenderSettings.ambientLight, false, false, false, null);
#endif
                        GUI.enabled = true;

                        if (isCustomAmbientColorToggleChanged)
                        {
                            UpdateDiffuseMap(true);
                        }
                    }

                    if (!settings.bakeAllActiveLights)
                    {
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(lightsProperty, true);
                        serializedObject.ApplyModifiedProperties();
                        if (EditorGUI.EndChangeCheck())
                        {
                            AssignTBLights();
                            UpdateLightsData();
                        }
                    }
                    else
                    {
                        GUI.enabled = false;
                        EditorGUILayout.PropertyField(lightsProperty, true);
                        GUI.enabled = true;
                    }
                }

                EditorGUILayout.EndScrollView();
                GUILayout.EndVertical();
            }

            #endregion

            #region Output Options Panel

            else if (optionsToolbarInt == 3)
            {

                GUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox));
                EditorGUILayout.LabelField("Output", EditorStyles.boldLabel);

                //OutputPath directory selector
                GUILayout.BeginHorizontal();
                GUIStyle Style = EditorStyles.textField;
                Style.alignment = TextAnchor.UpperLeft;
                
                _string = EditorGUILayout.TextField(new GUIContent("Output folder"), settings.outputFolder, Style);
                if (_string != settings.outputFolder)
                {
                    RecordUndo("Output folder");
                    settings.outputFolder = _string;
                }
                
                string last = settings.outputFolder.Length > 0 ? settings.outputFolder.Substring(settings.outputFolder.Length - 1) : "";
                if (!last.Equals("/")) settings.outputFolder += "/";
                GUI.SetNextControlName("OpenFolder");
                content = new GUIContent(icon_folder);
                if (GUILayout.Button(content, EditorStyles.label, GUILayout.MaxHeight(16), GUILayout.MaxWidth(19)))
                {
                    GUI.FocusControl("OpenFolder"); //force text field to lose focus if focused
                    settings.outputFolder = EditorUtility.OpenFolderPanel("Select output folder", settings.outputFolder, "");
                    if (settings.outputFolder.Length > 0 && settings.outputFolder.StartsWith(Application.dataPath))
                    {
                        settings.outputFolder = "Assets" + settings.outputFolder.Substring(Application.dataPath.Length) + "/";
                    }
                    else
                    {
                        settings.outputFolder += "/";
                    }
                }

                GUILayout.EndHorizontal();

                content = new GUIContent("Base name","All the generated textures will be named [basename]_[mapname].[extension]\nFor example, if the base name is 'Foo' and the chosen maps are Diffuse and Normal, the system will generate the files Foo_Diffuse.png and Foo_Normal.png");
                
                _string = EditorGUILayout.TextField(content, settings.outputBaseName);
                if (_string != settings.outputBaseName)
                {
                    RecordUndo("Output base name");
                    settings.outputBaseName = _string;
                }
                
                content = new GUIContent("Resolution", "Resolution of all the output maps.");
                
                _resolution = (Settings.Resolution) EditorGUILayout.EnumPopup(content, settings.resolution);
                if (_resolution != settings.resolution)
                {
                    RecordUndo("Resolution");
                    settings.resolution = _resolution;
                }
                
//                content = new GUIContent("Auto Update Preview", "Automatically update texture preview when changing filters values.");
//                
//                _bool = EditorGUILayout.Toggle(content, settings.autoUpdatePreview);
//                if (_bool != settings.autoUpdatePreview)
//                {
//                    RecordUndo("Auto update preview");
//                    settings.autoUpdatePreview = _bool;
//                }
                
                content = new GUIContent("Show Console Log","Show console messages about the baking process when it finishes.");
                
                _bool = EditorGUILayout.Toggle(content, settings.showConsoleLog);
                if (_bool != settings.showConsoleLog)
                {
                    RecordUndo("Show console log");
                    settings.showConsoleLog = _bool;
                }
                
                GUILayout.EndVertical();
            }

            #endregion

            GUILayout.EndArea();



            GUILayout.Space(20);

            GUILayout.BeginArea(new Rect(leftBarWidth+8,4, 520, 688));
            
            GUILayout.BeginVertical();

            #region textures preview
            
            GUILayout.BeginArea(new Rect(0, 0, 250, 20));
            
            EditorGUI.BeginChangeCheck();
            channelsToolbarInt = GUILayout.Toolbar(channelsToolbarInt, channelsToolbarStrings, EditorStyles.toolbarButton);
            if (EditorGUI.EndChangeCheck())
            {
                UpdatePreviewTexture();
            }

            GUILayout.EndArea();

            scrollSize = (resolution < scrollSize) ? 512 : resolution;
            scrollPosTexture = GUI.BeginScrollView(new Rect(0, 20, 512, 512), scrollPosTexture, new Rect(0, 24, scrollSize, scrollSize));
            int size = (resolution > 512) ? resolution : 512;
            GUI.Box(new Rect(0, 24, size, size), previewTexture, chessArea); 
            GUI.EndScrollView();
            //handle drag
            if (new Rect(0, 24, 512, 512).Contains(Event.current.mousePosition))
            {
                if (Event.current.type == EventType.MouseDrag) _dragging = true;
            }

            if (_dragging)
            {
                switch (Event.current.type)
                {
                    case EventType.MouseDrag:
                        scrollPosTexture += -Event.current.delta;
                        Event.current.Use();
                        break;
                    case EventType.MouseUp:
                        _dragging = false;
                        Event.current.Use();
                        break;
                }
            }
            
            
            GUILayout.BeginArea(new Rect(0, 534, 512, 34));
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            
            //line 1
            EditorGUI.BeginChangeCheck();
            mapsToolbarInt_1 = GUILayout.Toolbar(mapsToolbarInt_1, mapsToolbarStrings_1, EditorStyles.miniButton, GUILayout.Width(512));
            if(EditorGUI.EndChangeCheck())
            {
                mapsToolbarInt_2 = -1;
                OnPreviewMapSelected();
            }
            
            GUILayout.Space(-2);
            
            //line 2
            EditorGUI.BeginChangeCheck();
            mapsToolbarInt_2 = GUILayout.Toolbar(mapsToolbarInt_2, mapsToolbarStrings_2, EditorStyles.miniButton, GUILayout.Width(512));
            if(EditorGUI.EndChangeCheck())
            {
                mapsToolbarInt_1 = -1;
                OnPreviewMapSelected();
            }
            
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
            
            if (mapsToolbarInt_1 != -1) 
            {
                _changed = false;
                
                //Diffuse
                if (mapsToolbarInt_1 == 0)
                {
                    GUILayout.BeginArea(new Rect(404, 568, 108, 110));
                    GUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox), GUILayout.ExpandHeight(true));

                    if (outputRTDiffuse != null)
                    {
                        settings.autoUpdatePreview = GUILayout.Toggle(settings.autoUpdatePreview, "Auto update");
                        if (settings.autoUpdatePreview)
                        {
                            GUI.enabled = false;
                        }

                        content = new GUIContent("Update", "Update the current texture's preview");
                        if (GUILayout.Button(content))
                        {
                            UpdateDiffuseMap(settings.bakeLights);
                        }

                        if (outputRTDiffuse != null)
                        {
                            GUI.enabled = true;
                        }

                        if (GUILayout.Button("Save"))
                        {
                            string path = settings.outputFolder + settings.outputBaseName + "_Diffuse.png";
                            SaveManager.SaveRenderTexture(path, outputRTDiffuse, SaveManager.Extension.PNG, true, true, true, true);
                        }

                        GUI.enabled = true;
                    }

                    GUILayout.EndVertical();
                    GUILayout.EndArea();

                    GUILayout.BeginArea(new Rect(0, 568, 400, 110));
                    GUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox), GUILayout.ExpandHeight(false));
                    scrollPosOptions = GUILayout.BeginScrollView(scrollPosOptions);

                    if (outputRTDiffuse != null)
                    {
                        GUILayout.BeginHorizontal();
                        content = new GUIContent("Dilation", "Use Opaque mode to dilate colors over the transparent areas. Use Alpha Mask mode to dilate only the RGB channels and maintain the original alpha");
                        _dilationMode = (Settings.DilationMode) EditorGUILayout.EnumPopup(content, settings.dilationModeDiffuse, GUILayout.Width(230));
                        if (_dilationMode != settings.dilationModeDiffuse)
                        {
                            RecordUndo("Dilation mode diffuse");
                            settings.dilationModeDiffuse = _dilationMode;
                            if (settings.autoUpdatePreview)
                            {
                                UpdateDiffuseMap(false);
                            }
                        }

                        _int = EditorGUILayout.IntSlider("", settings.dilationDiffuse, 0, 20, GUILayout.Width(150));
                        if (_int != settings.dilationDiffuse)
                        {
                            RecordUndo("Dilation diffuse");
                            settings.dilationDiffuse = _int;
                            if (settings.autoUpdatePreview)
                            {
                                UpdateDiffuseMap(false);
                            }
                        }

                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        tempStyle.normal.textColor = Color.gray;
                        GUILayout.Label("No options available for this map, it has not been baked yet.", tempStyle);
                    }


                    GUILayout.EndScrollView();
                    GUILayout.EndVertical();
                    GUILayout.EndArea();
                }

                //Height
                else if (mapsToolbarInt_1 == 1)
                {
                    _changed = false;
                    
                    GUILayout.BeginArea(new Rect(404, 568, 108, 110));
                    GUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox), GUILayout.ExpandHeight(true));

                    if (outputRTHeight != null)
                    {
                        settings.autoUpdatePreview = GUILayout.Toggle(settings.autoUpdatePreview, "Auto update");
                        if (settings.autoUpdatePreview)
                        {
                            GUI.enabled = false;
                        }

                        content = new GUIContent("Update", "Update the current texture's preview");
                        if (GUILayout.Button(content))
                        {
                            UpdateHeightMap();
                            if (settings.normalsDetectionMode == Settings.NormalsDetectionMode.HeightMap && outputRTNormalMain != null)
                            {
                                if (settings.autoUpdatePreview)
                                {
                                    UpdateNormalMap();
                                }
                            }
                        }

                        if (outputRTHeight != null)
                        {
                            GUI.enabled = true;
                        }

                        if (GUILayout.Button("Save"))
                        {
                            string ext = settings._16bpcHeight ? "exr" : "png";
                            string path = settings.outputFolder + settings.outputBaseName + "_Height." + ext;
                            SaveManager.SaveRenderTexture(path, outputRTHeight, settings._16bpcHeight ? SaveManager.Extension.EXR : SaveManager.Extension.PNG, true, true, true, true);
                        }

                        GUI.enabled = true;
                    }

                    GUILayout.EndVertical();
                    GUILayout.EndArea();

                    GUILayout.BeginArea(new Rect(0, 568, 400, 110));
                    GUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox), GUILayout.ExpandHeight(false));
                    scrollPosOptions = GUILayout.BeginScrollView(scrollPosOptions);

                    if (outputRTHeight_original != null)
                    {
                        _changed = false;

                        GUILayout.BeginHorizontal();
                        content = new GUIContent("Dilation", "Use Opaque mode to dilate colors over the transparent areas. Use Alpha Mask mode to dilate only the RGB channels and maintain the original alpha");
                        _dilationMode = (Settings.DilationMode) EditorGUILayout.EnumPopup(content, settings.dilationModeHeight, GUILayout.Width(230));
                        if (_dilationMode != settings.dilationModeHeight)
                        {
                            RecordUndo("Dilation mode height");
                            settings.dilationModeHeight = _dilationMode;
                            _changed = true;
                        }

                        _int = EditorGUILayout.IntSlider("", settings.dilationHeight, 0, 20, GUILayout.Width(150));
                        if (_int != settings.dilationHeight)
                        {
                            RecordUndo("Dilation height");
                            settings.dilationHeight = _int;
                            _changed = true;
                        }

                        GUILayout.EndHorizontal();

                        _float = EditorGUILayout.Slider("Blur", settings.blurHeightAmount, 0, 20);
                        if (!Mathf.Approximately(_float, settings.blurHeightAmount))
                        {
                            RecordUndo("Blur height amount");
                            settings.blurHeightAmount = _float;
                            _changed = true;
                        }

                        _float = EditorGUILayout.Slider("Brightness", settings.brightnessHeight, -5, 5);
                        if (!Mathf.Approximately(_float, settings.brightnessHeight))
                        {
                            RecordUndo("Brightness height");
                            settings.brightnessHeight = _float;
                            _changed = true;
                        }

                        _float = EditorGUILayout.Slider("Contrast", settings.contrastHeight, 0, 20);
                        if (!Mathf.Approximately(_float, settings.contrastHeight))
                        {
                            RecordUndo("Contrast height");
                            settings.contrastHeight = _float;
                            _changed = true;
                        }


                        if (_changed && settings.autoUpdatePreview)
                        {
                            UpdateHeightMap();
                            if (settings.normalsDetectionMode == Settings.NormalsDetectionMode.HeightMap && outputRTNormalMain != null)
                            {
                                UpdateNormalMap();
                            }
                        }
                    }
                    else
                    {
                        tempStyle.normal.textColor = Color.gray;
                        GUILayout.Label("No options available for this map, it has not been baked yet.", tempStyle);
                    }

                    GUILayout.EndScrollView();
                    GUILayout.EndVertical();
                    GUILayout.EndArea();
                }

                //Normal
                else if (mapsToolbarInt_1 == 2)
                {
                    _changed = false;
                    
                    GUILayout.BeginArea(new Rect(404, 568, 108, 110));
                    GUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox), GUILayout.ExpandHeight(true));

                    if (outputRTNormalMain != null)
                    {
                        settings.autoUpdatePreview = GUILayout.Toggle(settings.autoUpdatePreview, "Auto update");
                        if (settings.autoUpdatePreview)
                        {
                            GUI.enabled = false;
                        }

                        content = new GUIContent("Update", "Update the current texture's preview");
                        if (GUILayout.Button(content))
                        {
                            UpdateNormalMap();
                        }

                        if (outputRTNormalMain != null)
                        {
                            GUI.enabled = true;
                        }

                        if (GUILayout.Button("Save"))
                        {
                            string path = settings.outputFolder + settings.outputBaseName + "_Normal.png";
                            SaveManager.SaveRenderTexture(path, settings.bakeExistingNormalMap ? outputRTNormalCombined : outputRTNormalMain, SaveManager.Extension.PNG, true, true, true, true);
                            ImportUtility.ImportAsNormalMap(path);
                        }

                        GUI.enabled = true;
                    }

                    GUILayout.EndVertical();
                    GUILayout.EndArea();

                    GUILayout.BeginArea(new Rect(0, 568, 400, 110));
                    GUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox), GUILayout.ExpandHeight(false));
                    scrollPosOptions = GUILayout.BeginScrollView(scrollPosOptions);

                    if (outputRTNormalMain != null)
                    {

                        if (settings.normalsDetectionMode == Settings.NormalsDetectionMode.HeightMap && outputRTHeight_original != null)
                        {
                            _changed = false;

                            GUILayout.BeginHorizontal();
                            content = new GUIContent("Height Map Dilation", "Use Opaque mode to dilate colors over the transparent areas. Use Alpha Mask mode to dilate only the RGB channels and maintain the original alpha");
                            _dilationMode = (Settings.DilationMode) EditorGUILayout.EnumPopup(content, settings.dilationModeHeight);
                            if (_dilationMode != settings.dilationModeHeight)
                            {
                                RecordUndo("Dilation mode height (normals)");
                                settings.dilationModeHeight = _dilationMode;
                                _changed = true;
                            }

                            _int = EditorGUILayout.IntSlider("", settings.dilationHeight, 0, 20, GUILayout.Width(150));
                            if (_int != settings.dilationHeight)
                            {
                                RecordUndo("Height map dilation (normals)");
                                settings.dilationHeight = _int;
                                _changed = true;
                            }

                            GUILayout.EndHorizontal();

                            _float = EditorGUILayout.Slider("Height Map Blur", settings.blurHeightAmount, 0, 20);
                            if (!Mathf.Approximately(_float, settings.blurHeightAmount))
                            {
                                RecordUndo("Height map blur amount (normals)");
                                settings.blurHeightAmount = _float;
                                _changed = true;
                            }

                            _float = EditorGUILayout.Slider("Height Map Brightness", settings.brightnessHeight, -5, 5);
                            if (!Mathf.Approximately(_float, settings.brightnessHeight))
                            {
                                RecordUndo("Height map brightness (normals)");
                                settings.brightnessHeight = _float;
                                _changed = true;
                            }

                            _float = EditorGUILayout.Slider("Height Map Contrast", settings.contrastHeight, 0, 20);
                            if (!Mathf.Approximately(_float, settings.contrastHeight))
                            {
                                RecordUndo("Height map contrast (normals)");
                                settings.contrastHeight = _float;
                                _changed = true;
                            }

                            if (_changed && settings.autoUpdatePreview)
                            {
                                UpdateHeightMap();
                                UpdateNormalMap();
                            }
                        }

                        if (settings.normalsDetectionMode == Settings.NormalsDetectionMode.HeightMap && outputRTHeight_original != null)
                        {
                            GUI.Label(new Rect(0, 568, 150, 16), "Invert channels");

                            GUILayout.BeginHorizontal(GUILayout.Width(120));
                            EditorGUILayout.PrefixLabel("Invert channels");

                            _changed = false;

                            _bool = GUILayout.Toggle(settings.invertNormalsRed, "R");
                            if (_bool != settings.invertNormalsRed)
                            {
                                RecordUndo("Invert normals Red");
                                settings.invertNormalsRed = _bool;
                                _changed = true;
                            }

                            _bool = GUILayout.Toggle(settings.invertNormalsGreen, "G");
                            if (_bool != settings.invertNormalsGreen)
                            {
                                RecordUndo("Invert normals Green");
                                settings.invertNormalsGreen = _bool;
                                _changed = true;
                            }

                            GUILayout.EndHorizontal();
                        }

                        if (_changed && settings.autoUpdatePreview)
                        {
                            UpdateNormalMap();
                        }

                        if (settings.normalsDetectionMode == Settings.NormalsDetectionMode.HeightMap && outputRTHeight_original != null)
                        {
                            _float = EditorGUILayout.Slider("Bump Strength", settings.bumpStrength, 0.03f, 20);
                            if (!Mathf.Approximately(_float, settings.bumpStrength))
                            {
                                RecordUndo("Bump strength");
                                settings.bumpStrength = _float;
                                if (settings.autoUpdatePreview)
                                {
                                    UpdateNormalMap();
                                }
                            }
                        }
                        else if (settings.normalsDetectionMode == Settings.NormalsDetectionMode.SurfaceFlat || settings.normalsDetectionMode == Settings.NormalsDetectionMode.SurfaceSmooth)
                        {
                            GUILayout.BeginHorizontal();
                            content = new GUIContent("Dilation", "Use Opaque mode to dilate colors over the transparent areas. Use Alpha Mask mode to dilate only the RGB channels and maintain the original alpha");
                            _dilationMode = (Settings.DilationMode) EditorGUILayout.EnumPopup(content, settings.dilationModeNormal, GUILayout.Width(230));
                            if (_dilationMode != settings.dilationModeNormal)
                            {
                                RecordUndo("Dilation mode normal");
                                settings.dilationModeNormal = _dilationMode;
                                if (settings.autoUpdatePreview)
                                {
                                    UpdateNormalMap();
                                }
                            }

                            _int = EditorGUILayout.IntSlider("", settings.dilationNormal, 0, 20, GUILayout.Width(150));
                            if (_int != settings.dilationNormal)
                            {
                                RecordUndo("Dilation normal");
                                settings.dilationNormal = _int;
                                if (settings.autoUpdatePreview)
                                {
                                    UpdateNormalMap();
                                }
                            }

                            GUILayout.EndHorizontal();
                        }

                        _float = EditorGUILayout.Slider("Denoise Large Details", settings.denoiseNormalMainAmount, 0, 10);
                        if (!Mathf.Approximately(_float, settings.denoiseNormalMainAmount))
                        {
                            RecordUndo("Denoise normal large details");
                            settings.denoiseNormalMainAmount = _float;
                            if (settings.autoUpdatePreview)
                            {
                                UpdateNormalMap();
                            }
                        }

                        if (settings.bakeExistingNormalMap)
                        {
                            _float = EditorGUILayout.Slider("Denoise Small Details", settings.denoiseNormalDetailsAmount, 0, 10);
                            if (!Mathf.Approximately(_float, settings.denoiseNormalDetailsAmount))
                            {
                                RecordUndo("Denoise normal small details");
                                settings.denoiseNormalDetailsAmount = _float;
                                if (settings.autoUpdatePreview)
                                {
                                    UpdateNormalMap();
                                }
                            }
                        }
                    }
                    else
                    {
                        tempStyle.normal.textColor = Color.gray;
                        GUILayout.Label("No options available for this map, it has not been baked yet.", tempStyle);
                    }

                    GUILayout.EndScrollView();
                    GUILayout.EndVertical();
                    GUILayout.EndArea();
                }
                
                //Metallic
                else if (mapsToolbarInt_1 == 3)
                {
                    _changed = false;
                    
                    GUILayout.BeginArea(new Rect(404, 568, 108, 110));
                    GUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox), GUILayout.ExpandHeight(true));

                    if (outputRTMetallic != null)
                    {
                        settings.autoUpdatePreview = GUILayout.Toggle(settings.autoUpdatePreview, "Auto update");
                        if (settings.autoUpdatePreview)
                        {
                            GUI.enabled = false;
                        }

                        content = new GUIContent("Update", "Update the current texture's preview");
                        if (GUILayout.Button(content))
                        {
                            UpdateMetallicMap();
                        }

                        if (outputRTMetallic != null)
                        {
                            GUI.enabled = true;
                        }

                        if (GUILayout.Button("Save"))
                        {
                            string path = settings.outputFolder + settings.outputBaseName + "_Metallic.png";
                            SaveManager.SaveRenderTexture(path, outputRTMetallic, SaveManager.Extension.PNG, true, true, true, true);
                        }

                        GUI.enabled = true;
                    }

                    GUILayout.EndVertical();
                    GUILayout.EndArea();

                    GUILayout.BeginArea(new Rect(0, 568, 400, 110));
                    GUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox), GUILayout.ExpandHeight(false));
                    scrollPosOptions = GUILayout.BeginScrollView(scrollPosOptions);

                    if (outputRTMetallic_original != null)
                    {
                        GUILayout.BeginHorizontal();
                        content = new GUIContent("Dilation", "Use Opaque mode to dilate colors over the transparent areas. Use Alpha Mask mode to dilate only the RGB channels and maintain the original alpha");
                        _dilationMode = (Settings.DilationMode) EditorGUILayout.EnumPopup(content, settings.dilationModeMetallic, GUILayout.Width(230));
                        if (_dilationMode != settings.dilationModeMetallic)
                        {
                            RecordUndo("Dilation mode metallic");
                            settings.dilationModeMetallic = _dilationMode;
                            if (settings.autoUpdatePreview)
                            {
                                UpdateMetallicMap();
                            }
                        }

                        _int = EditorGUILayout.IntSlider("", settings.dilationMetallic, 0, 20, GUILayout.Width(150));
                        if (_int != settings.dilationMetallic)
                        {
                            RecordUndo("Dilation metallic");
                            settings.dilationMetallic = _int;
                            if (settings.autoUpdatePreview)
                            {
                                UpdateMetallicMap();
                            }
                        }

                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        tempStyle.normal.textColor = Color.gray;
                        GUILayout.Label("No options available for this map, it has not been baked yet.", tempStyle);
                    }

                    GUILayout.EndScrollView();
                    GUILayout.EndVertical();
                    GUILayout.EndArea();
                }
                
                //specular
                else if (mapsToolbarInt_1 == 4)
                {
                    _changed = false;
                    
                    GUILayout.BeginArea(new Rect(404, 568, 108, 110));
                    GUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox), GUILayout.ExpandHeight(true));

                    if (outputRTSpecular != null)
                    {
                        settings.autoUpdatePreview = GUILayout.Toggle(settings.autoUpdatePreview, "Auto update");
                        if (settings.autoUpdatePreview)
                        {
                            GUI.enabled = false;
                        }

                        content = new GUIContent("Update", "Update the current texture's preview");
                        if (GUILayout.Button(content))
                        {
                            UpdateSpecularMap();
                        }

                        if (outputRTSpecular != null)
                        {
                            GUI.enabled = true;
                        }

                        if (GUILayout.Button("Save"))
                        {
                            string path = settings.outputFolder + settings.outputBaseName + "_Specular.png";
                            SaveManager.SaveRenderTexture(path, outputRTSpecular, SaveManager.Extension.PNG, true, true, true, true);
                        }

                        GUI.enabled = true;
                    }

                    GUILayout.EndVertical();
                    GUILayout.EndArea();

                    GUILayout.BeginArea(new Rect(0, 568, 400, 110));
                    GUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox), GUILayout.ExpandHeight(false));
                    scrollPosOptions = GUILayout.BeginScrollView(scrollPosOptions);

                    if (outputRTSpecular_original != null)
                    {
                        GUILayout.BeginHorizontal();
                        content = new GUIContent("Dilation", "Use Opaque mode to dilate colors over the transparent areas. Use Alpha Mask mode to dilate only the RGB channels and maintain the original alpha");
                        _dilationMode = (Settings.DilationMode) EditorGUILayout.EnumPopup(content, settings.dilationModeSpecular, GUILayout.Width(230));
                        if (_dilationMode != settings.dilationModeSpecular)
                        {
                            RecordUndo("Dilation mode specular");
                            settings.dilationModeSpecular = _dilationMode;
                            if (settings.autoUpdatePreview)
                            {
                                UpdateSpecularMap();
                            }
                        }

                        _int = EditorGUILayout.IntSlider("", settings.dilationSpecular, 0, 20, GUILayout.Width(150));
                        if (_int != settings.dilationSpecular)
                        {
                            RecordUndo("Dilation specular");
                            settings.dilationSpecular = _int;
                            if (settings.autoUpdatePreview)
                            {
                                UpdateSpecularMap();
                            }
                        }

                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        tempStyle.normal.textColor = Color.gray;
                        GUILayout.Label("No options available for this map, it has not been baked yet.", tempStyle);
                    }

                    GUILayout.EndScrollView();
                    GUILayout.EndVertical();
                    GUILayout.EndArea();
                }
            }
            else
            {
                //AO
                if (mapsToolbarInt_2 == 0)
                {
                    _changed = false;
                    
                    GUILayout.BeginArea(new Rect(404, 568, 108, 110));
                    GUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox), GUILayout.ExpandHeight(true));

                    if (outputRTAO != null)
                    {
                        settings.autoUpdatePreview = GUILayout.Toggle(settings.autoUpdatePreview, "Auto update");
                        if (settings.autoUpdatePreview)
                        {
                            GUI.enabled = false;
                        }

                        content = new GUIContent("Update", "Update the current texture's preview");
                        if (GUILayout.Button(content))
                        {
                            UpdateAOMap();
                        }

                        if (outputRTAO != null)
                        {
                            GUI.enabled = true;
                        }

                        if (GUILayout.Button("Save"))
                        {
                            string path = settings.outputFolder + settings.outputBaseName + "_AO.png";
                            SaveManager.SaveRenderTexture(path, outputRTAO, SaveManager.Extension.PNG, true, true, true, true);
                        }

                        GUI.enabled = true;
                    }

                    GUILayout.EndVertical();
                    GUILayout.EndArea();

                    GUILayout.BeginArea(new Rect(0, 568, 400, 110));
                    GUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox), GUILayout.ExpandHeight(false));
                    scrollPosOptions = GUILayout.BeginScrollView(scrollPosOptions);

                    if (outputRTAO_original != null)
                    {
                        _changed = false;

                        GUILayout.BeginHorizontal();
                        content = new GUIContent("Dilation", "Use Opaque mode to dilate colors over the transparent areas. Use Alpha Mask mode to dilate only the RGB channels and maintain the original alpha");
                        _dilationMode = (Settings.DilationMode) EditorGUILayout.EnumPopup(content, settings.dilationModeAO, GUILayout.Width(230));
                        if (_dilationMode != settings.dilationModeAO)
                        {
                            RecordUndo("Dilation mode AO");
                            settings.dilationModeAO = _dilationMode;
                            _changed = true;
                        }

                        _int = EditorGUILayout.IntSlider("", settings.dilationAO, 0, 20, GUILayout.Width(150));
                        if (_int != settings.dilationAO)
                        {
                            RecordUndo("Dilation AO");
                            settings.dilationAO = _int;
                            _changed = true;
                        }

                        GUILayout.EndHorizontal();

                        _float = EditorGUILayout.Slider("Denoise", settings.denoiseAOAmount, 0, 10);
                        if (!Mathf.Approximately(_float, settings.denoiseAOAmount))
                        {
                            RecordUndo("Denoise AO");
                            settings.denoiseAOAmount = _float;
                            _changed = true;
                        }

                        _int = EditorGUILayout.IntSlider("Blur", settings.blurAOAmount, 0, 20);
                        if (!Mathf.Approximately(_int, settings.blurAOAmount))
                        {
                            RecordUndo("Blur AO");
                            settings.blurAOAmount = _int;
                            _changed = true;
                        }

                        _float = EditorGUILayout.Slider("Brightness", settings.brightnessAO, -5, 5);
                        if (!Mathf.Approximately(_float, settings.brightnessAO))
                        {
                            RecordUndo("Brightness AO");
                            settings.brightnessAO = _float;
                            _changed = true;
                        }

                        _float = EditorGUILayout.Slider("Contrast", settings.contrastAO, 0, 20);
                        if (!Mathf.Approximately(_float, settings.contrastAO))
                        {
                            RecordUndo("Contrast AO");
                            settings.contrastAO = _float;
                            _changed = true;
                        }

                        if (settings.aoType == Settings.AOType.Fast)
                        {
                            _float = EditorGUILayout.Slider("Occlusion Strength", settings.AOStrength, 0, 1);
                            if (!Mathf.Approximately(_float, settings.AOStrength))
                            {
                                RecordUndo("AO Strength");
                                settings.AOStrength = _float;
                                _changed = true;
                            }

                            _float = EditorGUILayout.Slider("Sample radius", settings.sampleRadiusAO, 0.001f, 0.1f);
                            if (!Mathf.Approximately(_float, settings.sampleRadiusAO))
                            {
                                RecordUndo("AO sample radius");
                                settings.sampleRadiusAO = _float;
                                _changed = true;
                            }

                            _float = EditorGUILayout.Slider("Bias", settings.biasAO, -1, 1);
                            if (!Mathf.Approximately(_float, settings.biasAO))
                            {
                                RecordUndo("AO bias");
                                settings.biasAO = _float;
                                _changed = true;
                            }
                        }

                        if (_changed && settings.autoUpdatePreview)
                        {
                            UpdateAOMap();
                        }
                    }
                    else
                    {
                        tempStyle.normal.textColor = Color.gray;
                        GUILayout.Label("No options available for this map, it has not been baked yet.", tempStyle);
                    }

                    GUILayout.EndScrollView();
                    GUILayout.EndVertical();
                    GUILayout.EndArea();
                }

                //Emissive
                else if (mapsToolbarInt_2 == 1)
                {
                    _changed = false;
                    
                    GUILayout.BeginArea(new Rect(404, 568, 108, 110));
                    GUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox), GUILayout.ExpandHeight(true));

                    if (outputRTEmissive != null)
                    {
                        settings.autoUpdatePreview = GUILayout.Toggle(settings.autoUpdatePreview, "Auto update");
                        if (settings.autoUpdatePreview)
                        {
                            GUI.enabled = false;
                        }

                        content = new GUIContent("Update", "Update the current texture's preview");
                        if (GUILayout.Button(content))
                        {
                            UpdateEmissiveMap();
                        }

                        if (outputRTEmissive != null)
                        {
                            GUI.enabled = true;
                        }

                        if (GUILayout.Button("Save"))
                        {
                            string path = settings.outputFolder + settings.outputBaseName + "_Emissive.png";
                            SaveManager.SaveRenderTexture(path, outputRTEmissive, SaveManager.Extension.PNG, true, true, true, true);
                        }

                        GUI.enabled = true;
                    }

                    GUILayout.EndVertical();
                    GUILayout.EndArea();

                    GUILayout.BeginArea(new Rect(0, 568, 400, 110));
                    GUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox), GUILayout.ExpandHeight(false));
                    scrollPosOptions = GUILayout.BeginScrollView(scrollPosOptions);

                    if (outputRTEmissive_original != null)
                    {

                        GUILayout.BeginHorizontal();
                        content = new GUIContent("Dilation", "Use Opaque mode to dilate colors over the transparent areas. Use Alpha Mask mode to dilate only the RGB channels and maintain the original alpha");
                        _dilationMode = (Settings.DilationMode) EditorGUILayout.EnumPopup(content, settings.dilationModeEmissive, GUILayout.Width(230));
                        if (_dilationMode != settings.dilationModeEmissive)
                        {
                            RecordUndo("Dilation mode emissive");
                            settings.dilationModeEmissive = _dilationMode;
                            if (settings.autoUpdatePreview)
                            {
                                UpdateEmissiveMap();
                            }
                        }

                        _int = EditorGUILayout.IntSlider("", settings.dilationEmissive, 0, 20, GUILayout.Width(150));
                        if (_int != settings.dilationEmissive)
                        {
                            RecordUndo("Dilation emissive");
                            settings.dilationEmissive = _int;
                            if (settings.autoUpdatePreview)
                            {
                                UpdateEmissiveMap();
                            }
                        }

                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        tempStyle.normal.textColor = Color.gray;
                        GUILayout.Label("No options available for this map, it has not been baked yet.", tempStyle);
                    }

                    GUILayout.EndScrollView();
                    GUILayout.EndVertical();
                    GUILayout.EndArea();
                }

                //Curvature
                else if (mapsToolbarInt_2 == 2)
                {
                    _changed = false;
                    
                    GUILayout.BeginArea(new Rect(404, 568, 108, 110));
                    GUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox), GUILayout.ExpandHeight(true));

                    if (outputRTCurvature != null)
                    {
                        settings.autoUpdatePreview = GUILayout.Toggle(settings.autoUpdatePreview, "Auto update");
                        if (settings.autoUpdatePreview)
                        {
                            GUI.enabled = false;
                        }

                        content = new GUIContent("Update", "Update the current texture's preview");
                        if (GUILayout.Button(content))
                        {
                            UpdateCurvatureMap();
                        }

                        if (outputRTCurvature != null)
                        {
                            GUI.enabled = true;
                        }

                        if (GUILayout.Button("Save"))
                        {
                            string path = settings.outputFolder + settings.outputBaseName + "_Curvature.png";
                            SaveManager.SaveRenderTexture(path, outputRTCurvature, SaveManager.Extension.PNG, true, true, true, true);
                        }

                        GUI.enabled = true;
                    }

                    GUILayout.EndVertical();
                    GUILayout.EndArea();

                    GUILayout.BeginArea(new Rect(0, 568, 400, 110));
                    GUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox), GUILayout.ExpandHeight(false));
                    scrollPosOptions = GUILayout.BeginScrollView(scrollPosOptions);

                    if (outputRTCurvature_original != null)
                    {
                        content = new GUIContent("Channels", "Use Single to get a grayscale curvature map where below 50% grey represents concavity and above 50% represents convexity.\nUse Dual to get a R-G curvature map where green represents convexity and red represents concavity");
                        _curvatureChannels = (Settings.CurvatureChannels) EditorGUILayout.EnumPopup(content, settings.curvatureChannels);
                        if (_curvatureChannels != settings.curvatureChannels)
                        {
                            RecordUndo("Curvature channels");
                            settings.curvatureChannels = _curvatureChannels;
                            if (settings.autoUpdatePreview)
                            {
                                UpdateCurvatureMap();
                            }
                        }

                        content = new GUIContent("Multiplier", "Controls the strength of the curvature colors. Raise this value to get more contrast.");
                        _float = EditorGUILayout.Slider(content, settings.multiplierCurvature, 0, 5);
                        if (!Mathf.Approximately(_float, settings.multiplierCurvature))
                        {
                            RecordUndo("Multiplier Curvature");
                            settings.multiplierCurvature = _float;
                            _changed = true;
                            if (settings.autoUpdatePreview)
                            {
                                UpdateCurvatureMap();
                            }
                        }

                        GUILayout.BeginHorizontal();
                        content = new GUIContent("Dilation", "Use Opaque mode to dilate colors over the transparent areas. Use Alpha Mask mode to dilate only the RGB channels and maintain the original alpha");
                        _dilationMode = (Settings.DilationMode) EditorGUILayout.EnumPopup(content, settings.dilationModeCurvature, GUILayout.Width(230));
                        if (_dilationMode != settings.dilationModeCurvature)
                        {
                            RecordUndo("Dilation mode specular");
                            settings.dilationModeCurvature = _dilationMode;
                            if (settings.autoUpdatePreview)
                            {
                                UpdateCurvatureMap();
                            }
                        }

                        _int = EditorGUILayout.IntSlider("", settings.dilationCurvature, 0, 20, GUILayout.Width(150));
                        if (_int != settings.dilationCurvature)
                        {
                            RecordUndo("Dilation curvature");
                            settings.dilationCurvature = _int;
                            if (settings.autoUpdatePreview)
                            {
                                UpdateCurvatureMap();
                            }
                        }

                        GUILayout.EndHorizontal();

                        _float = EditorGUILayout.Slider("Denoise", settings.denoiseCurvatureAmount, 0, 10);
                        if (!Mathf.Approximately(_float, settings.denoiseCurvatureAmount))
                        {
                            RecordUndo("Denoise Curvature");
                            settings.denoiseCurvatureAmount = _float;
                            _changed = true;
                            if (settings.autoUpdatePreview)
                            {
                                UpdateCurvatureMap();
                            }
                        }

                        _int = EditorGUILayout.IntSlider("Smoothness", settings.smoothnessCurvature, 0, 8);
                        if (_int != settings.smoothnessCurvature)
                        {
                            RecordUndo("Smoothness Curvature");
                            settings.smoothnessCurvature = _int;
                            _changed = true;
                            if (settings.autoUpdatePreview)
                            {
                                UpdateCurvatureMap();
                            }
                        }
                    }
                    else
                    {
                        tempStyle.normal.textColor = Color.gray;
                        GUILayout.Label("No options available for this map, it has not been baked yet.", tempStyle);
                    }

                    GUILayout.EndScrollView();
                    GUILayout.EndVertical();
                    GUILayout.EndArea();
                }
                
                //Channel Packed HDRP
                else if (mapsToolbarInt_2 == 3)
                {
                    _changed = false;
                    
                   GUILayout.BeginArea(new Rect(404, 568, 108, 110));
                    GUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox), GUILayout.ExpandHeight(true));

                    if (outputRTMask != null)
                    {
                        settings.autoUpdatePreview = GUILayout.Toggle(settings.autoUpdatePreview, "Auto update");
                        if (settings.autoUpdatePreview)
                        {
                            GUI.enabled = false; 
                        }

                        content = new GUIContent("Update", "Update the current texture's preview");
                        if (GUILayout.Button(content))
                        {
                            UpdateMaskMap();
                        }

                        if (outputRTMask != null)
                        {
                            GUI.enabled = true;
                        }

                        if (GUILayout.Button("Save"))
                        {
                            string path = settings.outputFolder + settings.outputBaseName + "_Mask.png";
                            SaveManager.SaveRenderTexture(path, outputRTMask, SaveManager.Extension.PNG, true, true, true, true);
                        }

                        GUI.enabled = true;
                    }

                    GUILayout.EndVertical();
                    GUILayout.EndArea();

                    GUILayout.BeginArea(new Rect(0, 568, 400, 110));
                    GUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox), GUILayout.Height(110));
                    
                    tempStyle.normal.textColor = Color.gray;
                    if (outputRTMask == null)
                    {
                        GUILayout.Label("No options available for this map, it has not been baked yet.", tempStyle);
                    }
                    else
                    {
                        GUILayout.Label("No options available for this map.", tempStyle);
                    }

                    GUILayout.EndVertical();
                    GUILayout.EndArea();
                }
            }

            GUILayout.EndArea();
            
            GUILayout.EndArea();

            GUI.EndScrollView();

            #endregion

        }
        
        private void OnLowpolyChanged(bool manuallyChanged)
        {
            ImportUtility.ReimportModels(lowpolyModelsToReimport, true);
            
//            last_lowpoly = settings.lowpoly;
            
            if (settings.lowpoly != null)
            {
                hasOneMaterialPerMesh_lowpoly = ObjectUtils.HasOneMaterialPerMesh(settings.lowpoly);
            }
            
            if (manuallyChanged)
            {
                needsBindingRecalculation = true;
            }
            
            lowpolyModelsToReimport = ImportUtility.GetAllModelsImportSettings(settings.lowpoly);
        }
        private void OnHighpolyChanged(bool manuallyChanged)
        {
            ImportUtility.ReimportModels(highpolyModelsToReimport, true);
            
            last_highpoly = settings.highpoly;
            
            if (settings.highpoly != null)
            {
                hasMissingMaterials = ObjectUtils.HasMissingMaterials(settings.highpoly);

                if (!hasMissingMaterials)
                {
                    hasOneMaterialPerMesh_highpoly = ObjectUtils.HasOneMaterialPerMesh(settings.highpoly);
                }
            }

            if (manuallyChanged)
            {
                needsBindingRecalculation = true;
            }

            highpolyModelsToReimport = ImportUtility.GetAllModelsImportSettings(settings.highpoly);
        }

        private void OnCageChanged(bool manuallyChanged)
        {
            ImportUtility.ReimportModels(cageModelsToReimport, true);
            
            last_cage = settings.cage;
            if(manuallyChanged)
            {
                needsBindingRecalculation = true;
            }
            
            cageModelsToReimport = ImportUtility.GetAllModelsImportSettings(settings.cage);
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }

        #endregion


        #region Update

        private void Update()
        {
            if (settings == null)
            {
                return;
            }
            
            if (!settings.autoUpdatePreview)
            {
                return;
            }

            //Update lights and diffuse map if some lights changed
            if (TBLights != null)
            {
                for (int i = 0; i < TBLights.Length; i++)
                {
                    TBLight tbl = TBLights[i];
                    if (tbl != null && tbl.HasChanged())
                    {
                        UpdateLightsData();
                        if (outputRTDiffuse != null)
                        {
                            UpdateDiffuseMap(false);
                        }
                    }
                }
            }
        }

        #endregion


        #region Core functions

        private void OnPreviewMapSelected()
        {
            if(mapsToolbarInt_1 != -1)
            {
                switch (mapsToolbarInt_1)
                {
                    case 0:
                        originalPreviewTex = settings.createDiffuseMap ? outputRTDiffuse : null;
                        break;
                    case 1:
                        originalPreviewTex = settings.createHeightMap ? outputRTHeight : null;
                        break;
                    case 2:
                    {
                        if (settings.createNormalMap)
                        {
                            if (settings.bakeExistingNormalMap)
                            {
                                if (outputRTNormalDetails != null)
                                {
                                    originalPreviewTex = outputRTNormalCombined;
                                }
                            }
                            else
                            {
                                originalPreviewTex = outputRTNormalMain;
                            }
                        }
                        else originalPreviewTex = null;

                        break;
                    }
                    case 3:
                        originalPreviewTex = settings.createMetallicMap ? outputRTMetallic : null;
                        break;
                    case 4:
                        originalPreviewTex = settings.createSpecularMap ? outputRTSpecular : null;
                        break;
                    default:
                        originalPreviewTex = null;
                        break;
                }
            }
            else
            {
                switch (mapsToolbarInt_2)
                {
                    case 0:
                        originalPreviewTex = settings.createAOMap ? outputRTAO : null;
                        break;
                    case 1:
                        originalPreviewTex = settings.createEmissiveMap ? outputRTEmissive : null;
                        break;
                    case 2:
                        originalPreviewTex = settings.createCurvatureMap ? outputRTCurvature : null;
                        break;
                    case 3:
                        originalPreviewTex = settings.createMaskMap ? outputRTMask : null;
                        break;
                    default:
                        originalPreviewTex = null;
                        break;
                }
            }

            UpdatePreviewTexture();
        }

        private float Remap(float s, float a1, float a2, float b1, float b2)
        {
            return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
        }

        private void CollectAllLights()
        {
            List<Light> list = new List<Light>();
            GameObject[] roots = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (GameObject root in roots)
            {
                if (root.activeInHierarchy)
                {
                    Light[] components = root.GetComponentsInChildren<Light>(false);
                    foreach (Light light in components)
                    {
                        if (light.gameObject.activeInHierarchy)
                        {
                            list.Add(light);
                        }
                    }
                }

            }
            lights = list.ToArray();
            AssignTBLights();
        }

        private void AssignTBLights()
        {
            if (lights == null || lights.Length == 0)
            {
                return;
            }

            //destroy old lights' TBLight component
            RemoveTBLights();

            //recreate TBLights array
            TBLights = new TBLight[lights.Length];

            //fill new TBLights array
            for (int i = 0; i < lights.Length; i++)
            {
                Light light = lights[i];
                if (light == null)
                {
                    continue;
                }

                TBLight comp = light.GetComponent<TBLight>();
                if (comp != null)
                {
                    //copying a light with the component attached will produce an object with this component that
                    //isn't in the TBLights array, so it's better to remove it
                    DestroyImmediate(comp);
                }

                comp = light.gameObject.AddComponent<TBLight>();
                TBLights[i] = comp;

                comp.hideFlags |= HideFlags.DontSaveInEditor;
                comp.hideFlags |= HideFlags.HideInInspector;
            }
        }

        private void RemoveTBLights()
        {
            if (TBLights == null || TBLights.Length == 0)
            {
                return;
            }

            for (int i = 0; i < TBLights.Length; i++)
            {
                if (TBLights[i] != null)
                {
                    DestroyImmediate(TBLights[i]);
                }
            }
        }

        private void UpdateLightsData()
        {
            if (lights == null) return;

            hasLights = lights.Length > 0;
            
            if (!hasLights || high2LowWpos == null || high2LowNormal == null)
            {
                return;
            }

            //recreate material to allow new arrays with new sized, as described here: https://docs.unity3d.com/ScriptReference/Material.SetFloatArray.html
            if (Filters.mat_addLights != null)
            {
                DestroyImmediate(Filters.mat_addLights);
            }
            Filters.mat_addLights = new Material(Filters.shader_AddLights);
            
            int l = lights.Length;
            lightsValidation = new float[l];
            lightsTypes = new float[l];
            lightsPositions = new Vector4[l];
            lightsDirections = new Vector4[l];
            lightsColors = new Color[l];
            lightsIntensities = new float[l];
            lightsRanges = new float[l];
            lightsOuterCosAngles = new float[l];
            lightsInnerCosAngles = new float[l];

            for (int i = 0; i < l; i++)
            {
                Light light = lights[i];
                lightsValidation[i] = (light == null) ? 0 : 1;
                if (light == null)
                {
                    continue;
                }
                
                lightsTypes[i] = (int) light.type;
                Transform lightTransform = light.transform;
                lightsPositions[i] = lightTransform.position;
                lightsDirections[i] = lightTransform.forward;
                lightsColors[i] = light.color;
                lightsIntensities[i] = light.intensity;
                lightsRanges[i] = light.range;
                if (light.type == LightType.Spot)
                {
                    float spotAngle = light.spotAngle;
                    lightsOuterCosAngles[i] = Mathf.Cos(spotAngle * 0.5f * Mathf.Deg2Rad);
                    lightsInnerCosAngles[i] = Mathf.Cos((light.spotAngle * 0.5f - spotAngle * 0.5f * 0.25f) * Mathf.Deg2Rad);
                }
                else
                {
                    lightsOuterCosAngles[i] = 0.20f;
                    lightsInnerCosAngles[i] = 0.13f;
                }
            }
            
            Filters.mat_addLights.SetInt(_LightsCount, l);
            Filters.mat_addLights.SetFloatArray(_LightsValidation, lightsValidation);
            Filters.mat_addLights.SetFloatArray(_LightsTypes, lightsTypes);
            Filters.mat_addLights.SetVectorArray(_LightsPositions, lightsPositions);
            Filters.mat_addLights.SetVectorArray(_LightsDirections, lightsDirections);
            Filters.mat_addLights.SetColorArray(_LightsColors, lightsColors);
            Filters.mat_addLights.SetFloatArray(_LightsIntensities, lightsIntensities);
            Filters.mat_addLights.SetFloatArray(_LightsRanges, lightsRanges);
            Filters.mat_addLights.SetFloatArray(_LightsOuterCosAngles, lightsOuterCosAngles);
            Filters.mat_addLights.SetFloatArray(_LightsInnerCosAngles, lightsInnerCosAngles);
            Filters.mat_addLights.SetColor(_AmbientColor, settings.ambientColor);
            Filters.mat_addLights.SetTexture(_High2LowWpos, high2LowWpos);
            Filters.mat_addLights.SetTexture(_High2LowNormal, high2LowNormal);
        }

        
        private void Abort()
        {
            if (!debug)
            {
                EditorUtility.ClearProgressBar();
            }

            _aborted = true;
            resolution = 512;
            if (checkerTex != null) DestroyImmediate(checkerTex);
            checkerTex = CreateCheckerTexture(resolution);
            instance_cage.SetActive(settings.showCage);

            if (settings.createDiffuseMap)
            {
                mat_lowpoly.SetTexture(_MainTex, null);
            }

            if (settings.createNormalMap)
            {
                mat_lowpoly.SetTexture(_BumpMap, null);
            }

            if (settings.createMetallicMap)
            {
                mat_lowpoly.SetTexture(_MetallicGlossMap, null);
            }
            
            if (settings.createSpecularMap)
            {
                mat_lowpoly.SetTexture(_SpecGlossMap, null);
            }

            if (settings.createHeightMap)
            {
                mat_lowpoly.SetTexture(_ParallaxMap, null);
            }

            if (settings.createAOMap)
            {
                mat_lowpoly.SetTexture(_OcclusionMap, null);
            }

            if (settings.createEmissiveMap)
            {
                mat_lowpoly.SetTexture(_EmissionMap, null);
            }
            
            //curvature map won't be used in the lowpoly material
        }

        private bool Bake()
        {
            float startTime = Time.realtimeSinceStartup;
            float elapsedTime;

            resolution = (int) settings.resolution;
            int pixelsCount = resolution * resolution;
            checkPoint = 0;

            Filters.Init();

            _finished = false;
            _aborted = false;
            progress = 0.005f;

            originalQueriesHitBackfaces = Physics.queriesHitBackfaces;
            Physics.queriesHitBackfaces = false;
            
            if (!debug)
            {
                EditorUtility.DisplayProgressBar("Total Baker", "Preparing scene", progress);
            }

            RTColorSpace = PlayerSettings.colorSpace == ColorSpace.Linear ? RenderTextureReadWrite.Linear : RenderTextureReadWrite.sRGB;
            
            mat_lowpoly.DisableKeyword("_EMISSION");
            mat_lowpoly.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
            mat_lowpoly.DisableKeyword("_NORMALMAP");
            mat_lowpoly.DisableKeyword("_PARALLAXMAP");
            mat_lowpoly.DisableKeyword("_METALLICGLOSSMAP");
            mat_lowpoly.DisableKeyword("_SPECGLOSSMAP");
            
            PrepareScene(true);

            bool originalHighlightCurrentCageMesh = settings.highlightCurrentCageMesh;
            bool originalhighlightCurrentBoundMeshes = settings.highlightCurrentBoundMeshes;
            
            settings.highlightCurrentCageMesh = false;
            settings.highlightCurrentBoundMeshes = false;
            
            RecalculateHighlights();

            //force standard material if metallic map is selected
            if (settings.createAOMap || settings.createEmissiveMap || settings.createHeightMap || settings.createMetallicMap || settings.createSpecularMap)
            {
                if (settings.createMetallicMap)
                {
                    settings.previewMaterialTypeHighpoly = Settings.PreviewMaterialType.StandardMetallic;
                }
                else if(settings.createSpecularMap)
                {
                    settings.previewMaterialTypeHighpoly = Settings.PreviewMaterialType.StandardSpecular;
                }
                UpdateHighpolyShader();
            }


            //force enable cage just for the bake
            instance_cage.SetActive(true);
            ObjectUtils.SetLayer(instance_lowpoly, 31, true);
            ObjectUtils.SetLayer(instance_cage, 31, true);


            progress = 0.02f;
            if (!debug)
            {
                EditorUtility.DisplayProgressBar("Total Baker", "Creating lookup textures", progress);
            }
            
            if (outputRTDiffuse != null) DestroyRT(outputRTDiffuse);
            if (outputRTHeight != null) DestroyRT(outputRTHeight);
            if (outputRTNormalMain_original != null) DestroyRT(outputRTNormalMain_original);
            if (outputRTNormalMain != null) DestroyRT(outputRTNormalMain);
            if (outputRTNormalDetails != null) DestroyRT(outputRTNormalDetails);
            if (outputRTNormalDetails_original != null) DestroyRT(outputRTNormalDetails_original);
            if (outputRTNormalCombined != null) DestroyRT(outputRTNormalCombined);
            if (outputRTAO != null) DestroyRT(outputRTAO);
            if (outputRTAO_original != null) DestroyRT(outputRTAO_original);
            if (outputRTEmissive != null) DestroyRT(outputRTEmissive);
            if (outputRTCurvature != null) DestroyRT(outputRTCurvature);
            if (outputRTMask != null) DestroyRT(outputRTMask);
            if (outputTex_uv != null) DestroyImmediate(outputTex_uv);
            if (outputTex_diffuse != null) DestroyImmediate(outputTex_diffuse);
            if (outputTex_height != null) DestroyImmediate(outputTex_height);
            if (outputTex_normalMain != null) DestroyImmediate(outputTex_normalMain);
            if (outputTex_normalCombined != null) DestroyImmediate(outputTex_normalCombined);
            if (outputTex_normalDetails != null) DestroyImmediate(outputTex_normalDetails);
            if (outputRTNormalCombined != null) DestroyRT(outputRTNormalCombined);
            if (outputTex_metallic != null) DestroyImmediate(outputTex_metallic);
            if (outputTex_specular != null) DestroyImmediate(outputTex_specular);
            if (outputTex_ao != null) DestroyImmediate(outputTex_ao);
            if (outputTex_emissive != null) DestroyImmediate(outputTex_emissive);
            if (outputTex_curvature != null) DestroyImmediate(outputTex_curvature);
            if (originMap_cage != null) DestroyImmediate(originMap_cage);
            if (layersMap != null) DestroyImmediate(layersMap);
            if (directionMap_cage != null) DestroyImmediate(directionMap_cage);
            if (highpoly_WorldSpaceNormals != null) DestroyImmediate(highpoly_WorldSpaceNormals);
            if (originMap_highpoly != null) DestroyImmediate(originMap_highpoly);
            if (directionMap_highpoly != null) DestroyImmediate(directionMap_highpoly);
            if (previewTexture != null) DestroyImmediate(previewTexture);
            if(outputRTDiffuse_original != null)  DestroyImmediate(outputRTDiffuse_original);
            if(outputRTNormalMain_original != null)  DestroyImmediate(outputRTNormalMain_original);
            if(outputRTNormalDetails_original != null)  DestroyImmediate(outputRTNormalDetails_original);
            if(outputRTMetallic_original != null)  DestroyImmediate(outputRTMetallic_original);
            if(outputRTSpecular_original != null)  DestroyImmediate(outputRTSpecular_original);
            if(outputRTAO_original != null)  DestroyImmediate(outputRTAO_original);
            if(outputRTEmissive_original != null)  DestroyImmediate(outputRTEmissive_original);
            if(outputRTCurvature_original != null)  DestroyImmediate(outputRTCurvature_original);
            
            
            resolution = (int) settings.resolution;
            
            previewTexture = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGBFloat, RTColorSpace);
            previewTexture.Create();
            
            MeshFilter[] filters = instance_highpoly.GetComponentsInChildren<MeshFilter>();
            foreach (MeshFilter mf in filters)
            {
                BakingObject bo = mf.gameObject.AddComponent<BakingObject>();
                Mesh mesh = mf.sharedMesh;
                bo.vertices = mesh.vertices;
                bo.triangles = mesh.triangles;
                bo.uvs = mesh.uv;
            }

            instance_highpoly.SetActive(false);
            instance_lowpoly.SetActive(false);

            RenderTexture rt;
            rt = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGBFloat, RTColorSpace);
            rt.Create();
            Camera activeCamera = Camera.main;
            activeCamera.transform.position = new Vector3(0.5f, 0.5f, -0.5f);
            activeCamera.orthographic = true;
            activeCamera.orthographicSize = 0.5f;
            activeCamera.targetTexture = rt;
            activeCamera.clearFlags = CameraClearFlags.Color;
            activeCamera.backgroundColor = new Color(0, 0, 0, 0);
            RenderTexture.active = rt;
            Material currentMat = mat_cage;
            Shader shader = currentMat.shader;


            //Origin Map cage
            originMap_cage = new Texture2D(resolution, resolution, TextureFormat.RGBAFloat, false);
            currentMat.shader = Filters.shader_UV2WorldPos;
            activeCamera.Render();
            originMap_cage.ReadPixels(new Rect(0, 0, resolution, resolution), 0, 0);
            originMap_cage.Apply();
//            SaveManager.SaveTexture2D("Assets/Textures/OriginMapCage.png", originMap_cage);

            //Direction Map cage
            directionMap_cage = new Texture2D(resolution, resolution, TextureFormat.RGBAFloat, false);
            currentMat.shader = Filters.shader_UV2Normal;
            activeCamera.Render();
            directionMap_cage.ReadPixels(new Rect(0, 0, resolution, resolution), 0, 0);
            directionMap_cage.Apply();
//            SaveManager.SaveTexture2D("Assets/Textures/DirectionMapCage.png", directionMap_cage);

            currentMat.shader = shader; //restore shader
            
            instance_highpoly.SetActive(true);
            instance_cage.SetActive(false);

            List<Material> highpolyMats = new List<Material>();
            List<Shader> highpolyShaders = new List<Shader>();

            Renderer highpolyRenderer = instance_highpoly.GetComponent<Renderer>();
            if (highpolyRenderer != null)
            {
                highpolyMats.Add(highpolyRenderer.sharedMaterial);
                highpolyShaders.Add(highpolyRenderer.sharedMaterial.shader);
            }

            if (instance_highpoly.transform.childCount != 0)
            {
                foreach (Transform t in instance_highpoly.transform)
                {
                    Renderer r = t.GetComponent<Renderer>();
                    if (r != null)
                    {
                        Material m = r.sharedMaterial;
                        if (m != null)
                        {
                            highpolyMats.Add(m);
                            highpolyShaders.Add(m.shader);
                        }
                    }
                }
            }

            if (settings.createAOMap && settings.aoType == Settings.AOType.Fast)
            {
                //Origin Map highpoly
                originMap_highpoly = new Texture2D(resolution, resolution, TextureFormat.RGBAFloat, false);
                foreach (Material m in highpolyMats)
                {
                    m.shader = Filters.shader_UV2WorldPos;
                }
                activeCamera.Render();
                originMap_highpoly.ReadPixels(new Rect(0, 0, resolution, resolution), 0, 0);
                originMap_highpoly.Apply();

                //Direction Map highpoly
                directionMap_highpoly = new Texture2D(resolution, resolution, TextureFormat.RGBAFloat, false);
                foreach (Material m in highpolyMats)
                {
                    m.shader = Filters.shader_UV2Normal;
                }
                activeCamera.Render();
                directionMap_highpoly.ReadPixels(new Rect(0, 0, resolution, resolution), 0, 0);
                directionMap_highpoly.Apply();
            }
           
            if (settings.bindMeshes)
            {
                AssignBakingLayers();
                layersMap = GenerateLayersMap();
            }

            for (int i = 0; i < highpolyMats.Count; i++)
            {
                highpolyMats[i].shader = highpolyShaders[i];
            }


            if (settings.createAOMap && settings.aoType == Settings.AOType.Classic)
            {
                aoRaysDirections = GetSphereDirections(settings.AORaysPerPoint);
            }
            
            if (highpolyMeshes != null && highpolyMeshes.Count > 0)
            {
                foreach (CachedMesh m in highpolyMeshes.Values)
                {
                    m.DestroyAllCachedTextures();
                }
                highpolyMeshes.Clear(); //Garbage collector should delete the CachedMesh instances in this dictionary
            }
            
            highpolyMeshes = new Dictionary<Mesh, CachedMesh>();
            highpolyReadableTextures = new Dictionary<Texture2D, Texture2D>();
            filters = instance_highpoly.GetComponentsInChildren<MeshFilter>();
            foreach (MeshFilter mf in filters)
            {
                Mesh m = mf.sharedMesh;
                if (m != null)
                {
                    if (highpolyMeshes.ContainsKey(m))
                    {
                        Mesh newUniqueMesh = Instantiate(m);
                        mf.sharedMesh = newUniqueMesh;
                    }
                    CachedMesh cm = new CachedMesh(mf, highpolyReadableTextures, settings.bakeVertexColors, settings.createDiffuseMap, settings.createNormalMap, settings.createMetallicMap, settings.createSpecularMap, settings.createEmissiveMap);
                    highpolyMeshes.Add(mf.sharedMesh, cm);
                }
            }
            
            if (settings.bakeLights)
            {
                instance_cage.SetActive(false);
                instance_lowpoly.SetActive(false);
                //World space normal map + tangent highpoly
                RenderTexture tmp = RenderTexture.GetTemporary(resolution, resolution, 0, RenderTextureFormat.ARGBFloat);
                activeCamera.targetTexture = tmp;
                foreach (Material m in highpolyMats)
                {
                    Texture normalMap = m.GetTexture(_BumpMap);
                    m.shader = Filters.shader_Tangent2World;
                    m.SetTexture(_TangentNormalMap, normalMap);
                }
                activeCamera.Render();
                highpoly_WorldSpaceNormals = RenderTexture2Texture2D(tmp);
                activeCamera.targetTexture = null;
                RenderTexture.ReleaseTemporary(tmp);
                
            }
            
            for (int i = 0; i < highpolyMats.Count; i++)
            {
                highpolyMats[i].shader = highpolyShaders[i];
            }
            
            instance_cage.SetActive(true);
            instance_lowpoly.SetActive(true);
            
            activeCamera.targetTexture = null;
            activeCamera.orthographic = false;
            RenderTexture.active = null;
            DestroyRT(rt);
            
            outputTex_uv = new Texture2D(resolution, resolution, TextureFormat.RGBAFloat, false);
            
            if(settings.createDiffuseMap) outputTex_diffuse = new Texture2D(resolution, resolution, TextureFormat.RGBAFloat, false);

            if(settings.createHeightMap) outputTex_height = new Texture2D(resolution, resolution, TextureFormat.RGBAFloat, false);
            
            if(settings.createNormalMap) outputTex_normalMain = new Texture2D(resolution, resolution, TextureFormat.RGBAFloat, false);
            
            if(settings.createNormalMap && settings.bakeExistingNormalMap) outputTex_normalDetails = new Texture2D(resolution, resolution, TextureFormat.RGBAFloat, false);
            
            if(settings.createMetallicMap) outputTex_metallic = new Texture2D(resolution, resolution, TextureFormat.RGBAFloat, false);
            
            if(settings.createSpecularMap) outputTex_specular = new Texture2D(resolution, resolution, TextureFormat.RGBAFloat, false);
            
            if(settings.createAOMap) outputTex_ao = new Texture2D(resolution, resolution, TextureFormat.RGBAFloat, false);
            
            if(settings.createEmissiveMap) outputTex_emissive = new Texture2D(resolution, resolution, TextureFormat.RGBAFloat, false);
            
            if(settings.createCurvatureMap) outputTex_curvature = new Texture2D(resolution, resolution, TextureFormat.RGBAFloat, false);
            
//            if(settings.createMaskMap) outputTex_mask = new Texture2D(resolution, resolution, TextureFormat.RGBAFloat, false);

            if (settings.bakeLights && settings.bakeAllActiveLights)
            {
                CollectAllLights();
            }

            hasLights = lights != null && lights.Length > 0;

            if (settings.bakeLights)
            {
                high2LowWpos = new Texture2D(resolution, resolution, TextureFormat.RGBAFloat, false);
                high2LowNormal = new Texture2D(resolution, resolution, TextureFormat.RGBAFloat, false);
            }

            progress = 0.05f;

            RaycastHit hit;
            RaycastHit hitAO;
            Ray ray;
            Collider coll;
            Vector2 uv01;
            Vector3 hitPoint_global;
            Vector3 hitPoint_local;
            Vector3 hitNormal;
            Material hitMaterial;
            WorldPoint p;
            Quaternion hitQuaternion;
            float rayWeightAO = 1f / settings.AORaysPerPoint;
            CachedMesh hitMesh;
            Texture2D tex;
            Vector2 offset;
            Vector2 scale;
            Vector2 samplingUV;
            Vector2Int samplingPixel;
            int tri;
            int currentTriangle;
            Color tempColor; // generic variable to store temporary colors

            progress = 0.15f;

            if (!debug)
            {
                if (EditorUtility.DisplayCancelableProgressBar("Total Baker", "Baking textures", progress))
                {
                    Abort();
                    return false;
                }
            }

            int pixelIndex = 0;
            int mappedPixels = 0;
            int collidedRays = 0;
            Color currentLayerMapPixel;

            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    p = UvToWorld(new Vector2(1 - (float) x / resolution, 1 - (float) y / resolution));

                    progress = (float) pixelIndex / pixelsCount;
                    if (progress > checkPoint)
                    {
                        checkPoint += 0.01f;
                        bakePercentage = (checkPoint * 100).ToString("F0");
                        progress = Remap(progress, 0, 1, 0.15f, 1);
                        if (!debug)
                        {
                            if (EditorUtility.DisplayCancelableProgressBar("Total Baker", "Baking textures " + bakePercentage + "%", progress))
                            {
                                Abort();
                                return false;
                            }
                        }
                    }

                    if (!p.mapped)
                    {
                        pixelIndex++;
                        continue;
                    }

                    mappedPixels++;

                    int layerMask = -1;
                    if (settings.bindMeshes)
                    {
                        currentLayerMapPixel = layersMap.GetPixel(x, resolution-y);
                        layerMask = ReconstructLayerMaskFromPixel(currentLayerMapPixel);
                    }

                    ray = new Ray(p.point, -p.normal);
                    Physics.Raycast(ray, out hit, settings.maxRayLength, layerMask);

                    coll = hit.collider;

                    if (coll != null)
                    {
                        collidedRays++;

                        tri = hit.triangleIndex;
                        currentTriangle = tri * 3;
                        
                        hitPoint_global = hit.point;
                        hitPoint_local = coll.transform.InverseTransformPoint(hitPoint_global);
                        hitNormal = hit.normal;

                        Mesh sharedMesh = hit.collider.GetComponent<MeshFilter>().sharedMesh;
                        if (!highpolyMeshes.ContainsKey(sharedMesh))
                        {
                            pixelIndex++;
                            continue;
                        }
                        
                        hitMesh = highpolyMeshes[hit.collider.GetComponent<MeshFilter>().sharedMesh];
                        hitMaterial = coll.GetComponent<Renderer>().sharedMaterial;
                        
                        uv01 = Point2UV(hitPoint_local, coll, currentTriangle);

                        if (settings.createAOMap && settings.aoType == Settings.AOType.Fast)
                        {
                            outputTex_uv.SetPixel(x,y,new Color(uv01.x, uv01.y, 0, 0.5f));
                        }

                        Vector3 hitBaryCoord = hit.barycentricCoordinate;
                        Vector3 interpolatedNormal = Vector3.forward;
                        bool needInterpolatedNormal = settings.bakeLights || settings.createNormalMap && settings.normalsDetectionMode == Settings.NormalsDetectionMode.SurfaceSmooth || settings.createCurvatureMap;
                        if (needInterpolatedNormal)
                        {
                            interpolatedNormal = GetInterpolatedNormal(hitMesh, hitBaryCoord, currentTriangle);
                        }

                        Color interpolatedVertexColor = Color.white;
                        if (settings.bakeVertexColors)
                        {
                            interpolatedVertexColor = GetInterpolatedVertexColor(hitMesh, hitBaryCoord, currentTriangle);
                        }

                        if (settings.createDiffuseMap)
                        {
                            if (settings.bakeVertexColors && settings.vertexColorsMode == Settings.VertexColorsMode.Overwrite)
                            {
                                outputTex_diffuse.SetPixel(x, y, interpolatedVertexColor);
                            }
                            else
                            {
                                //get diffuse texture of original material, if exists     
                                tex = hitMesh.mainTex.tex;
                                scale = hitMesh.mainTex.scale;
                                offset = hitMesh.mainTex.offset;
                                offset.x = 1 - offset.x;
                                if (tex != null)
                                {
                                    //if there's a texture use its color in the computed uv point
                                    samplingUV = uv01;
                                    samplingUV.Scale(scale);
                                    samplingUV += offset;
                                    samplingUV.x = 1 - samplingUV.x;
                                    samplingPixel = new Vector2Int((int) (samplingUV.x * tex.width), (int) (samplingUV.y * tex.height));
                                    tempColor = tex.GetPixel(samplingPixel.x, samplingPixel.y) * hitMaterial.color;
                                    if (settings.bakeVertexColors && settings.vertexColorsMode == Settings.VertexColorsMode.Multiply)
                                    {
                                        tempColor *= interpolatedVertexColor;
                                    }
                                    outputTex_diffuse.SetPixel(x, y, tempColor);
                                }
                                //else use material's color
                                else
                                {
                                    tempColor = hitMaterial.color;
                                    if (settings.bakeVertexColors && settings.vertexColorsMode == Settings.VertexColorsMode.Multiply)
                                    {
                                        tempColor *= interpolatedVertexColor;
                                    }
                                    outputTex_diffuse.SetPixel(x, y, tempColor);
                                }
                            }
                        }

                        if (settings.bakeLights)
                        {
                            high2LowWpos.SetPixel(x ,y, new Color(hitPoint_global.x, hitPoint_global.y, hitPoint_global.z));
                            samplingUV = uv01;
                            samplingUV.y = 1-samplingUV.y;
                            samplingPixel = new Vector2Int((int) (resolution - samplingUV.x * resolution), (int) (resolution - samplingUV.y * resolution));
                            Color c = highpoly_WorldSpaceNormals.GetPixel(samplingPixel.x, samplingPixel.y);
                            c.r = c.r * 2 - 1;
                            c.g = c.g * 2 - 1;
                            c.b = c.b * 2 - 1;
                            high2LowNormal.SetPixel(x,y, c);
                        }

                        if (settings.createHeightMap)
                        {
                            //get point grayscale depth by ray distance and build main normal map
                            float depth = 1 - hit.distance * 10;
                            outputTex_height.SetPixel(x,y, new Color(depth, depth, depth));
                        }

                        if (settings.createNormalMap)
                        {
                            if (settings.normalsDetectionMode == Settings.NormalsDetectionMode.SurfaceFlat)
                            {
                                //build normals from surface's normals                           
                                Vector3 normal = hitNormal;
                                outputTex_normalMain.SetPixel(x, y, new Color(normal.x * 0.5f + 0.5f, normal.y * 0.5f + 0.5f, normal.z * 0.5f + 0.5f));
                            }
                            else if (settings.normalsDetectionMode == Settings.NormalsDetectionMode.SurfaceSmooth)
                            {
                                outputTex_normalMain.SetPixel(x, y, new Color(interpolatedNormal.x * 0.5f + 0.5f,interpolatedNormal.y * 0.5f + 0.5f,interpolatedNormal.z * 0.5f + 0.5f));
                            }

                            //build details normal map...
                            if (settings.bakeExistingNormalMap)
                            {
                                tex = hitMesh.bumpMap.tex;
                                scale = hitMesh.bumpMap.scale;
                                offset = hitMesh.bumpMap.offset;
                                offset.x = 1 - x;
                                if (tex != null)
                                {
                                    samplingUV = uv01;
                                    samplingUV.Scale(scale);
                                    samplingUV += offset;
                                    samplingPixel = new Vector2Int((int) (tex.width - samplingUV.x * tex.width), (int) (samplingUV.y * tex.height));

                                    //if there's a texture use its color in the computed uv point                                                   
                                    outputTex_normalDetails.SetPixel(x, y, Filters.Unpack(tex.GetPixel(samplingPixel.x, samplingPixel.y)));
                                }
                            }

                            //else... don't build the high frequency texture! 
                        }


                        if (settings.createMetallicMap)
                        {
                            //get diffuse texture of original material, if exists  
                            float metallic = hitMaterial.GetFloat(_Metallic);
                            float smoothness = hitMaterial.GetFloat(_Glossiness);
                            float smoothnessScale = hitMaterial.GetFloat(_GlossMapScale);
                            tex = hitMesh.metallicGlossMap.tex;
                            scale = hitMesh.metallicGlossMap.scale;
                            offset = hitMesh.metallicGlossMap.offset;
                            offset.x = 1 - x;
                            if (tex != null)
                            {
                                samplingUV = uv01;
                                samplingUV.Scale(scale);
                                samplingUV += offset;
                                samplingPixel = new Vector2Int((int) (tex.width - samplingUV.x * tex.width), (int) (samplingUV.y * tex.height));

                                //if there's a texture use its color in the computed uv point
                                Color pixel = tex.GetPixel(samplingPixel.x, samplingPixel.y);
                                pixel.a *= smoothnessScale;
                                outputTex_metallic.SetPixel(x, y, pixel);
                            }
                            //else use material's properties
                            else outputTex_metallic.SetPixel(x, y, new Color(metallic, metallic, metallic, smoothness));
                        }
                        
                        if (settings.createSpecularMap)
                        {
                            //get diffuse texture of original material, if exists  
                            Color specularColor = hitMaterial.GetColor(_SpecColor);
                            float smoothness = hitMaterial.GetFloat(_Glossiness);
                            float smoothnessScale = hitMaterial.GetFloat(_GlossMapScale);
                            tex = hitMesh.specGlossMap.tex;
                            scale = hitMesh.specGlossMap.scale;
                            offset = hitMesh.specGlossMap.offset;
                            offset.x = 1 - x;
                            if (tex != null)
                            {
                                samplingUV = uv01;
                                samplingUV.Scale(scale);
                                samplingUV += offset;
                                samplingPixel = new Vector2Int((int) (tex.width - samplingUV.x * tex.width), (int) (samplingUV.y * tex.height));

                                //if there's a texture use its color in the computed uv point
                                Color pixel = tex.GetPixel(samplingPixel.x, samplingPixel.y);
                                pixel.a *= smoothnessScale;
                                outputTex_metallic.SetPixel(x, y, pixel);
                            }
                            //else use material's properties
                            else outputTex_specular.SetPixel(x, y, new Color(specularColor.r, specularColor.g, specularColor.b, smoothness));
                        }

                        if (settings.createAOMap)
                        {
                            if (settings.aoType == Settings.AOType.Classic)
                            {
                                hitQuaternion = Quaternion.FromToRotation(instance_highpoly.transform.up, -hitNormal) * instance_highpoly.transform.rotation;
                                float occlusion = 1;
                                for (int d = 0; d < aoRaysDirections.Length; d++)
                                {
                                    if (Physics.Raycast(hitPoint_global, hitQuaternion * aoRaysDirections[d], out hitAO, settings.AOMaxRaySpread))
                                    {
                                        occlusion -= rayWeightAO * (settings.AOStrength - (hitAO.distance / settings.AOMaxRaySpread));
                                    }
                                }
                                outputTex_ao.SetPixel(x, y,  new Color(occlusion, occlusion, occlusion, 1f));
                            }
                        }

                        if (settings.createEmissiveMap)
                        {
                            //get diffuse texture of original material, if exists                        
                            tex = hitMesh.emissionMap.tex;
                            scale = hitMesh.emissionMap.scale;
                            offset = hitMesh.emissionMap.offset;
                            offset.x = 1 - x;
                            if (tex != null)
                            {
                                //if there's a texture use its color in the computed uv point
                                samplingUV = uv01;
                                samplingUV.Scale(scale);
                                samplingUV += offset;
                                samplingPixel = new Vector2Int((int) (tex.width - samplingUV.x * tex.width), (int) (samplingUV.y * tex.height));

                                outputTex_emissive.SetPixel(x, y, tex.GetPixel(samplingPixel.x, samplingPixel.y));
                            }
                            //else use material's color
                            else outputTex_emissive.SetPixel(x, y, hitMaterial.GetColor(_EmissionColor));
                        }
                        
                        if (settings.createCurvatureMap)
                        {
                            if (settings.curvatureDetectionMode == Settings.CurvatureDetectionMode.Geometry)
                            {
                                outputTex_curvature.SetPixel(x, y, new Color(interpolatedNormal.x * 0.5f + 0.5f,interpolatedNormal.y * 0.5f + 0.5f,interpolatedNormal.z * 0.5f + 0.5f));
                            }
                            else
                            {
                                //no-op, just use the generated normal map in the UpdateCurvatureMap() function
                            }
                        }


                    }

                    pixelIndex++;
                }
            }

            progress = 1f;

            if (!debug)
            {
                if (EditorUtility.DisplayCancelableProgressBar("Total Baker", "Processing results", progress))
                {
                    Abort();
                    return false;
                }
            }

            if (settings.bakeLights)
            {
                UpdateLightsData();
                high2LowWpos.Apply();
                high2LowNormal.Apply();
            }

            if (settings.createAOMap && settings.aoType == Settings.AOType.Fast)
            {
                outputTex_uv.Apply();
            }

            if (settings.createDiffuseMap)
            {
                outputTex_diffuse.Apply();
                outputRTDiffuse_original = Texture2D2RenderTexture(outputTex_diffuse);
                outputRTDiffuse = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGBFloat, RTColorSpace);
                outputRTDiffuse.useMipMap = true;
                outputRTDiffuse.Create();
                UpdateDiffuseMap(false);
            }

            if (settings.createHeightMap)
            {
                mat_lowpoly.EnableKeyword("_PARALLAXMAP");
                outputTex_height.Apply();
                outputRTHeight_original = Texture2D2RenderTexture(outputTex_height);
                outputRTHeight = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGBFloat, RTColorSpace);
                outputRTHeight.useMipMap = true;
                outputRTHeight.Create();
                Filters.Copy(outputRTHeight_original, outputRTHeight);
                UpdateHeightMap();
            }

            if (settings.createNormalMap)
            {
                mat_lowpoly.EnableKeyword("_NORMALMAP");
                outputRTNormalMain_original = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGBFloat, RTColorSpace);
                outputRTNormalMain_original.useMipMap = true;
                outputRTNormalMain_original.Create();

                outputRTNormalMain = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGBFloat, RTColorSpace);
                outputRTNormalMain.useMipMap = true;
                outputRTNormalMain.Create();

                //create a render texture for the normal details    
                if (settings.bakeExistingNormalMap)
                {
                    outputTex_normalDetails.Apply();
                    outputRTNormalDetails_original = Texture2D2RenderTexture(outputTex_normalDetails);
                    outputRTNormalDetails = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGBFloat, RTColorSpace);
                    outputRTNormalDetails.useMipMap = true;
                    outputRTNormalDetails.Create();
                    Filters.Copy(outputRTNormalDetails_original, outputRTNormalDetails);
                }

                if (settings.normalsDetectionMode == Settings.NormalsDetectionMode.HeightMap)
                {
                    //convert height into normal (normal main)...
                    Filters.Grayscale2Normal(outputRTHeight, outputRTNormalMain_original, settings.bumpStrength);
                    Filters.Copy(outputRTNormalMain_original, outputRTNormalMain);
                    Filters.InvertChannels(outputRTNormalMain_original, settings.invertNormalsRed, settings.invertNormalsGreen, false);
                }

                else //surface flat or smooth
                {
                    outputTex_normalMain.Apply();
                    outputRTNormalMain_original = Texture2D2RenderTexture(outputTex_normalMain);
                    Filters.Dilate(outputRTNormalMain_original, 2, Settings.DilationMode.Opaque);

                    Shader shaderOriginal = mat_lowpoly.shader;

                    instance_cage.SetActive(false);
                    instance_highpoly.SetActive(false);
                    RenderTexture w2t = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGBFloat, RTColorSpace);
                    w2t.Create();
                    mat_lowpoly.shader = Filters.shader_World2Tangent;
                    mat_lowpoly.SetTexture(_WorldNormalMap, outputRTNormalMain_original);
                    Vector3 lowpolyPos = instance_lowpoly.transform.position;
                    instance_lowpoly.transform.position = Vector3.zero;
                    activeCamera.transform.position = Vector3.back * 5;
                    activeCamera.targetTexture = w2t;
                    activeCamera.Render();
                    instance_lowpoly.transform.position = lowpolyPos;

                    Filters.Copy(w2t, outputRTNormalMain_original);

                    activeCamera.targetTexture = null;
                    DestroyRT(w2t);
                    instance_cage.SetActive(true);
                    instance_highpoly.SetActive(true);
                    mat_lowpoly.shader = shaderOriginal;

                    Filters.Copy(outputRTNormalMain_original, outputRTNormalMain);
                    Filters.InvertChannels(outputRTNormalMain, settings.invertNormalsRed, settings.invertNormalsGreen, false);
                }

                packedNormalMap = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGBFloat, RTColorSpace);
                packedNormalMap.useMipMap = true;
                packedNormalMap.Create();

                if (settings.bakeExistingNormalMap)
                {
                    outputRTNormalCombined = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGBFloat, RTColorSpace);
                    outputRTNormalCombined.useMipMap = true;
                    outputRTNormalCombined.Create();
                    if (settings.normalsDetectionMode == Settings.NormalsDetectionMode.HeightMap)
                    {
                        //combine main and details normal maps
                        Filters.Copy(outputRTNormalDetails_original, outputRTNormalDetails);
                        Filters.BilateralBlur(outputRTNormalDetails, settings.denoiseNormalDetailsAmount);
                        Filters.CombineNormals(outputRTNormalMain, outputRTNormalDetails, outputRTNormalCombined);
                        //pack normals only if the current version doesn't support auto-packing
                        #if !UNITY_2017_3_OR_NEWER
                        Pack(outputRTNormalCombined, packedNormalMap);
                        #endif
                        outputTex_normalCombined = RenderTexture2Texture2D(outputRTNormalCombined);
                    }
                    else
                    {
                        Filters.Copy(outputRTNormalDetails_original, outputRTNormalCombined);
                        outputTex_normalCombined = RenderTexture2Texture2D(outputRTNormalCombined);
                    }
                }
                else
                {
                    //pack normals only if the current version doesn't support auto-packing
                    #if !UNITY_2017_3_OR_NEWER
                        Pack(outputRTNormalMain, packedNormalMap);
                                        #endif
                    outputTex_normalCombined = RenderTexture2Texture2D(outputRTNormalMain);
                }

                UpdateNormalMap();
            }

            if (settings.createMetallicMap)
            {
                mat_lowpoly.EnableKeyword("_METALLICGLOSSMAP");
                mat_lowpoly.DisableKeyword("_SPECGLOSSMAP");
                outputTex_metallic.Apply();
                outputRTMetallic_original = Texture2D2RenderTexture(outputTex_metallic);
                outputRTMetallic = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGBFloat, RTColorSpace);
                outputRTMetallic.useMipMap = true;
                outputRTMetallic.Create();
                Filters.Copy(outputRTMetallic_original, outputRTMetallic);
                UpdateMetallicMap();
            }
            
            if (settings.createSpecularMap)
            {
                mat_lowpoly.EnableKeyword("_SPECGLOSSMAP");
                mat_lowpoly.DisableKeyword("_METALLICGLOSSMAP");
                outputTex_specular.Apply();
                outputRTSpecular_original = Texture2D2RenderTexture(outputTex_specular);
                outputRTSpecular = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGBFloat, RTColorSpace);
                outputRTSpecular.useMipMap = true;
                outputRTSpecular.Create();
                Filters.Copy(outputRTSpecular_original, outputRTSpecular);
                UpdateSpecularMap();
            }

            if (settings.createAOMap)
            {
                outputRTAO_original = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGBFloat, RTColorSpace);
                outputRTAO_original.useMipMap = true;
                outputRTAO_original.Create();

                //Standard Shader doesn't have an AO keyword
                outputRTAO = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGBFloat, RTColorSpace);
                outputRTAO.useMipMap = true;
                outputRTAO.Create();
                if (settings.aoType == Settings.AOType.Fast)
                {
                    Filters.FastAO(outputRTAO_original, originMap_highpoly, directionMap_highpoly, settings.AOStrength, settings.biasAO, settings.sampleRadiusAO, Color.white);
                    Filters.RemapTexture(outputRTAO_original, outputRTAO, outputTex_uv);
                    Filters.BilateralBlur(outputRTAO, settings.blurAOAmount);
                }
                else
                {
                    outputTex_ao.Apply();
                    outputRTAO_original = Texture2D2RenderTexture(outputTex_ao);
                    Filters.Copy(outputRTAO_original, outputRTAO);
                    Filters.BilateralBlur(outputRTAO, settings.blurAOAmount);
                }

                UpdateAOMap();
            }

            if (settings.createEmissiveMap)
            {
                mat_lowpoly.EnableKeyword("_EMISSION");
                mat_lowpoly.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                outputTex_emissive.Apply();
                outputRTEmissive_original = Texture2D2RenderTexture(outputTex_emissive);
                outputRTEmissive = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGBFloat, RTColorSpace);
                outputRTEmissive.useMipMap = true;
                outputRTEmissive.Create();
                Filters.Copy(outputRTEmissive_original, outputRTEmissive);
                if (mat_lowpoly.HasProperty("_EmissionColor"))
                {
                    mat_lowpoly.SetColor(_EmissionColor, Color.white);
                }
                UpdateEmissiveMap();
            }

            if (settings.createCurvatureMap)
            {
                outputTex_curvature.Apply();
                if (settings.curvatureDetectionMode == Settings.CurvatureDetectionMode.Geometry)
                {
                    outputRTCurvature_original = Texture2D2RenderTexture(outputTex_curvature);
                }
                else
                {
                    outputRTCurvature_original =  new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGBFloat, RTColorSpace);
                    outputRTCurvature_original.useMipMap = true;
                    outputRTCurvature_original.Create();
                    Filters.Copy(outputRTNormalDetails != null ? outputRTNormalCombined : outputRTNormalMain, outputRTCurvature_original);
                }
                outputRTCurvature = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGBFloat, RTColorSpace);
                outputRTCurvature.useMipMap = true;
                outputRTCurvature.Create();
                
                Shader shaderOriginal = mat_lowpoly.shader;

                instance_cage.SetActive(false);
                instance_highpoly.SetActive(false);
                RenderTexture w2t = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGBFloat, RTColorSpace);
                w2t.Create();
                mat_lowpoly.shader = Filters.shader_World2Tangent;
                mat_lowpoly.SetTexture(_WorldNormalMap, outputRTCurvature_original);
                Vector3 lowpolyPos = instance_lowpoly.transform.position;
                instance_lowpoly.transform.position = Vector3.zero;
                activeCamera.transform.position = Vector3.back * 5;
                activeCamera.targetTexture = w2t;
                activeCamera.Render();
                instance_lowpoly.transform.position = lowpolyPos;

                Filters.Copy(w2t, outputRTCurvature_original);

                activeCamera.targetTexture = null;
                DestroyRT(w2t);
                instance_cage.SetActive(true);
                instance_highpoly.SetActive(true);
                mat_lowpoly.shader = shaderOriginal;
                
                Filters.Copy(outputRTCurvature_original, outputRTCurvature);
                UpdateCurvatureMap();
            }
            
            if (settings.createMaskMap)
            {
                outputRTMask = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGBFloat, RTColorSpace);
                outputRTMask.useMipMap = true;
                outputRTMask.Create();
                UpdateMaskMap();
            }

            elapsedTime = Time.realtimeSinceStartup - startTime; 
            if (settings.showConsoleLog)
            {
                Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
                string statsLog = logPrefix + "Baking statistics";
                statsLog += Environment.NewLine;
                statsLog += Environment.NewLine;
                statsLog += "Bake completed in: " + elapsedTime + "s";
                statsLog += Environment.NewLine;
                float mappedPixelsPercent = (float) mappedPixels / (resolution * resolution) * 100;
                float collidedRaysPercent = (float) collidedRays / mappedPixels * 100;
                statsLog += String.Format("Mapped pixels: {0:F1}%", mappedPixelsPercent);
                statsLog += Environment.NewLine;
                statsLog += String.Format("Rays intersections: {0:F1}%", collidedRaysPercent);
                Debug.Log(statsLog);
                Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.ScriptOnly);
            }

            if (instance_highpoly.transform.childCount == 0)
            {
                BakingObject bo = instance_highpoly.transform.gameObject.GetComponent<BakingObject>();
                DestroyImmediate(bo);
            }

            foreach (Transform t in instance_highpoly.transform)
            {
                BakingObject bo = t.gameObject.GetComponent<BakingObject>();
                DestroyImmediate(bo);
            }

            _finished = true;
            if (!_aborted)
            {
                if (checkerTex != null) DestroyImmediate(checkerTex);
                checkerTex = CreateCheckerTexture(resolution);
            }

            //update lowpoly's preview (assign textures to its material)        

            AssignBakedTextures();

            settings.highlightCurrentCageMesh = originalHighlightCurrentCageMesh;
            settings.highlightCurrentBoundMeshes = originalhighlightCurrentBoundMeshes;
            RecalculateHighlights();

            instance_cage.SetActive(settings.showCage); //restore cage visibility
            if (!debug)
            {
                EditorUtility.ClearProgressBar();
            }

            OnPreviewMapSelected();
            aoRaysDirections = null;
            GC.Collect(); //clean garbage created during baking
            
            GUIUtility.ExitGUI(); //avoid layout error messages
             
            Physics.queriesHitBackfaces = originalQueriesHitBackfaces;
            
            return true;
        }




        private void SpawnObjects()
        {
            bool validObjects = ((settings.autoGenerateCage && settings.lowpoly) || settings.cage) && settings.highpoly;
            if (!validObjects)
            {
                return;
            }
            
            Bounds highpolyBounds;
            Bounds lowpolyBounds = new Bounds(); 

            //Mesh currMesh;
            bool _refresh = false;

            //Restore original model's import settings
            if (settings.highpoly != last_highpoly)
            {
                if (last_highpoly != null)
                {
                    ImportUtility.ReimportModels(highpolyModelsToReimport, false);
                    _refresh = true;
                }

                last_highpoly = settings.highpoly;
                highpolyModelsToReimport = ImportUtility.GetAllModelsImportSettings(last_highpoly);
            }
            
            //Spawn highpoly
            if (instance_highpoly)
            {
                DestroyImmediate(instance_highpoly);
            }
            
            ImportUtility.ReimportModel(last_highpoly, settings.importNormalsHighpoly, settings.smoothAngleHighpoly, false);
            
            instance_highpoly = Instantiate(settings.highpoly);

            ObjectUtils.SkinnedToMesh(instance_highpoly);
            
            ObjectUtils.FixPrimitives(instance_highpoly);

            instance_highpoly.hideFlags = HideFlags.DontSave;

            instance_highpoly.name = "TB_HighPoly";

            hasOneMaterialPerMesh_highpoly = ObjectUtils.HasOneMaterialPerMesh(instance_highpoly);
            
            //split by submeshes
            if (!hasOneMaterialPerMesh_highpoly)
            {
                GameObject tmp = instance_highpoly;
                instance_highpoly = ObjectUtils.ToUniqueMaterialPerMeshObject(instance_highpoly);
                DestroyImmediate(tmp);
            }

            //instantiate materials after destroying the old ones
            if (mats_highpoly != null)
            {
                for (int i = 0; i < mats_highpoly.Count; i++)
                {
                    DestroyImmediate(mats_highpoly[i]); 
                }
            }
            Renderer[] rends = GetAllHighpolyMeshes();
            mats_highpoly.Clear();
            foreach (Renderer rend in rends)
            {
                Material m = Instantiate(rend.sharedMaterial);
                rend.sharedMaterial = m;
                mats_highpoly.Add(m);
            }
            
            
            DrawBounds boundsDrawer = instance_highpoly.AddComponent<DrawBounds>();
            boundsDrawer.color = new Color(0.5f, 0.7f, 1f, 0.5f);
            boundsDrawer.showCenter = true;


            MeshFilter[] meshFilters = instance_highpoly.GetComponentsInChildren<MeshFilter>();

            UpdateHighpolyShader();
            UpdateHighpolyReflections();

            //normalize highpoly size
            instance_highpoly.transform.localScale = Vector3.one;
            if (meshFilters.Length > 0)
            {
                ObjectUtils.NormalizeSize(instance_highpoly);
                highpolyBounds = ObjectUtils.ComputeBounds(instance_highpoly);
                instance_highpoly.transform.position = Vector3.one * 0.5f - highpolyBounds.center;
                
                foreach (MeshFilter meshFilter in meshFilters)
                {
                    if (meshFilter.GetComponent<MeshCollider>() == null)
                    {
                        meshFilter.gameObject.AddComponent<MeshCollider>();
                    }
                }
            }
            else
            {
                Debug.LogError(logPrefix + "The provided high poly model doesn't have any MeshFilter.");
            }

            Renderer[] renderers = instance_highpoly.GetComponentsInChildren<Renderer>();
            if (renderers == null || renderers.Length == 0)
            {
                Debug.LogError(logPrefix + "The provided high poly model doesn't have any Renderer.");
            }

            ImportUtility.ReimportModel(settings.lowpoly, settings.importNormalsLowpoly, settings.smoothAngleLowpoly, false);

            //Restore original model's import settings
            if (settings.cage != last_cage)
            {
                if (last_cage != null)
                {
                    ImportUtility.ReimportModels(cageModelsToReimport, false);
                    _refresh = true;
                }

                last_cage = settings.cage;
                cageModelsToReimport = ImportUtility.GetAllModelsImportSettings(last_cage);
                ImportUtility.ReimportModel(last_cage, settings.importNormalsCage, settings.smoothAngleCage, false);
            }
            
            //Spawn cage
            if (instance_cage)
            {
                DestroyImmediate(instance_cage);
            }

            if (settings.autoGenerateCage)
            {
                GameObject tmp = Instantiate(settings.lowpoly);
                ObjectUtils.SkinnedToMesh(tmp);
                Vector3 cage_size = ObjectUtils.ComputeBounds(tmp).size;
                float maxDimension = Mathf.Max(cage_size.x, Mathf.Max(cage_size.y, cage_size.z));
                float cage_scale = 1 / maxDimension;
                
                GameObject cageNoOffset = ObjectUtils.CreateCageFromRoot(tmp, 0, cage_scale);
                cageNoOffset.transform.localScale *= cage_scale;
                cageNoOffset.name = "Tmp_CageNoOffset";
                
                instance_cage = ObjectUtils.CreateCageFromRoot(tmp, settings.cageOffset, cage_scale);
                instance_cage.transform.localScale *= cage_scale;
                instance_cage.name = "TB_Cage";
                
                //find boundingBox
                highpolyBounds = ObjectUtils.ComputeBounds(instance_highpoly);
                Bounds cageBoundsNoOffset = ObjectUtils.ComputeBounds(cageNoOffset);
                instance_cage.transform.position = instance_highpoly.transform.position;
                cageNoOffset.transform.position = instance_highpoly.transform.position;
                instance_cage.transform.Translate(highpolyBounds.center - cageBoundsNoOffset.center);
                DestroyImmediate(tmp);
                DestroyImmediate(cageNoOffset);
            }
            else
            {
                instance_cage = Instantiate(settings.cage);
                instance_cage.name = "TB_Cage";
                instance_cage.transform.localScale = instance_highpoly.transform.localScale;
                instance_cage.transform.position = instance_highpoly.transform.position;
            }

            instance_cage.hideFlags = HideFlags.DontSave;
            boundsDrawer = instance_cage.AddComponent<DrawBounds>();
            boundsDrawer.color = new Color(0.5f, 0.7f, 1f, 0.5f);
            boundsDrawer.showCenter = true;

            //apply wireframe material
            ObjectUtils.ApplyMaterialToAllRenderers(instance_cage, mat_cage);

            //hide cage if needed
            instance_cage.SetActive(settings.showCage);
//            instance_cage.transform.rotation = Quaternion.identity;

            //Restore original model's import settings
            if (settings.lowpoly != last_lowpoly)
            {
                ResetLowpolyMaterial(); 
                if (last_lowpoly != null)
                {
                    ImportUtility.ReimportModels(lowpolyModelsToReimport, false);
                    _refresh = true;
                }

                last_lowpoly = settings.lowpoly;
                lowpolyModelsToReimport = ImportUtility.GetAllModelsImportSettings(last_lowpoly);
                ImportUtility.ReimportModel(last_lowpoly, settings.importNormalsLowpoly, settings.smoothAngleLowpoly, false);

            }
            
            //Spawn Lowpoly
            if (instance_lowpoly)
            {
                DestroyImmediate(instance_lowpoly);
            }
            
            instance_lowpoly = Instantiate(settings.lowpoly);
            ObjectUtils.SkinnedToMesh(instance_lowpoly);
            hasOneMaterialPerMesh_lowpoly = ObjectUtils.HasOneMaterialPerMesh(instance_lowpoly);
            
            if (!hasOneMaterialPerMesh_lowpoly)
            {
                GameObject tmp = instance_lowpoly;
                instance_lowpoly = ObjectUtils.ToUniqueMaterialPerMeshObject(instance_lowpoly);
                DestroyImmediate(tmp);
            }

            instance_lowpoly.hideFlags = HideFlags.DontSave;
            instance_lowpoly.name = "TB_Lowpoly";
            instance_highpoly.name = "TB_HighPoly";
            
            boundsDrawer = instance_lowpoly.AddComponent<DrawBounds>();
            boundsDrawer.color = new Color(0.5f, 0.7f, 1f, 0.5f);
            boundsDrawer.showCenter = true;

            UpdateLowpolyShader();
            UpdateLowpolyReflections();

            //normalize lowpoly size
            meshFilters = instance_lowpoly.GetComponentsInChildren<MeshFilter>();
            if (meshFilters != null && meshFilters.Length != 0)
            {
                foreach (MeshFilter mf in meshFilters)
                {

                    MeshCollider meshCollider = mf.gameObject.GetComponent<MeshCollider>();
                    if (meshCollider == null)
                    {
                        meshCollider = mf.gameObject.AddComponent<MeshCollider>();
                    }

                    lowpolyBounds.Encapsulate(meshCollider.bounds);
                    DestroyImmediate(meshCollider);

                    Renderer rend = mf.GetComponent<Renderer>();
                    Material[] sharedMats = rend.sharedMaterials;
                    for (int  i = 0; i < sharedMats.Length; i++)
                    {
                        sharedMats[i] = mat_lowpoly;
                    }
                    rend.sharedMaterials = sharedMats;
                }

                ObjectUtils.NormalizeSize(instance_lowpoly);
                
                lowpolyBounds = ObjectUtils.ComputeBounds(instance_lowpoly);

                instance_lowpoly.transform.position =
                    new Vector3(
                        0.5f + lowpolyBounds.max.x + 0.25f + lowpolyBounds.extents.x,
                        0.5f,
                        0.5f) - lowpolyBounds.center;
            }
            else
            {
                Debug.LogError(logPrefix + "The provided low poly model doesn't have any MeshRenderer.");
            }

//            instance_lowpoly.transform.rotation = Quaternion.identity;
            
            if (_refresh) AssetDatabase.Refresh();
            
            ObjectUtils.SetLayer(instance_lowpoly, 0, true);
            ObjectUtils.SetLayer(instance_cage, 0, true);

            UpdateLowpolyShader();
            UpdateHighpolyShader();
            
            RecalculateBindings();
            RecalculateHighlights();
        }

        private void CenterView()
        {
            if (!instance_lowpoly || !instance_highpoly || SceneView.lastActiveSceneView == null)
            {
                return;
            }
            SceneView scene_view = SceneView.lastActiveSceneView;
            Vector3 midPoint = (instance_lowpoly.transform.position + instance_highpoly.transform.position) * 0.5f;
            midPoint.z = -8f;
            Camera sceneCam = SceneView.lastActiveSceneView.camera;
            sceneCam.transform.position = midPoint;
            sceneCam.transform.rotation = Quaternion.Euler(0,180,0);
            scene_view.AlignViewToObject(sceneCam.transform);
            SceneView.lastActiveSceneView.pivot = midPoint;
            SceneView.lastActiveSceneView.Repaint();
        }

        

        private void UpdateLowpolyShader()
        {
            if (instance_lowpoly != null)
            {
                switch (settings.previewMaterialTypeLowpoly)
                {
                    case Settings.PreviewMaterialType.StandardMetallic:
                        mat_lowpoly.shader = Filters.shader_StandardMetallic;
                        ShaderUtils.SetStandardMaterialRenderMode(mat_lowpoly, settings.previewMaterialBlendStandardLowpoly);
                        break;
                    case Settings.PreviewMaterialType.StandardSpecular:
                        mat_lowpoly.shader = Filters.shader_StandardSpecular;
                        ShaderUtils.SetStandardMaterialRenderMode(mat_lowpoly, settings.previewMaterialBlendStandardLowpoly);
                        break;
                    case Settings.PreviewMaterialType.Diffuse:
                        mat_lowpoly.shader = Filters.shader_Diffuse;
                        ShaderUtils.SetDiffuseRenderMode(mat_lowpoly, settings.previewMaterialBlendLegacyLowpoly);
                        break;
                    default:
                        mat_lowpoly.shader = Filters.shader_Unlit;
                        ShaderUtils.SetUnlitRenderMode(mat_lowpoly, settings.previewMaterialBlendLegacyLowpoly);
                        break;
                }
            }
        }

        private void UpdateLowpolyReflections()
        {
            if (instance_lowpoly != null)
            {
                if (ShaderUtils.IsStandard(settings.previewMaterialTypeLowpoly))
                {
                    if (settings.reflectionsLowpoly)
                    {
                        mat_lowpoly.DisableKeyword("_GLOSSYREFLECTIONS_OFF");
                        mat_lowpoly.SetFloat(_GlossyReflections, 1f);
                    }
                    else
                    {
                        mat_lowpoly.EnableKeyword("_GLOSSYREFLECTIONS_OFF");
                        mat_lowpoly.SetFloat(_GlossyReflections, 0f);
                    }
                }
            }
        }

        private void UpdateHighpolyShader()
        {
            if (instance_highpoly != null)
            {
                Renderer[] rends = instance_highpoly.GetComponentsInChildren<Renderer>();
                foreach (Renderer r in rends)
                {
                    Material mat = r.sharedMaterial;
                    switch (settings.previewMaterialTypeHighpoly)
                    {
                        case Settings.PreviewMaterialType.StandardMetallic:
                            mat.shader = Filters.shader_StandardMetallic;
                            ShaderUtils.SetStandardMaterialRenderMode(mat, settings.previewMaterialBlendStandardHighpoly);
                            break;
                        case Settings.PreviewMaterialType.StandardSpecular:
                            mat.shader = Filters.shader_StandardSpecular;
                            ShaderUtils.SetStandardMaterialRenderMode(mat, settings.previewMaterialBlendStandardHighpoly);
                            break;
                        case Settings.PreviewMaterialType.Diffuse:
                            mat.shader = Filters.shader_Diffuse;
                            ShaderUtils.SetDiffuseRenderMode(mat, settings.previewMaterialBlendLegacyHighpoly);
                            break;
                        default:
                            mat.shader = Filters.shader_Unlit;
                            ShaderUtils.SetUnlitRenderMode(mat, settings.previewMaterialBlendLegacyHighpoly);
                            break;
                    }
                }
            }
            UpdateHighpolyReflections();
            UpdateHighpolyReflections();
        }
        
        private void UpdateHighpolyReflections()
        {
            if (instance_highpoly != null)
            {
                if (ShaderUtils.IsStandard(settings.previewMaterialTypeHighpoly))
                {
                    if (settings.reflectionsHighpoly)
                    {
                        foreach (Material material in mats_highpoly)
                        {
                            material.DisableKeyword("_GLOSSYREFLECTIONS_OFF");
                            material.SetFloat(_GlossyReflections, 1f);
                        }
                    }
                    else
                    {
                        foreach (Material material in mats_highpoly)
                        {
                            material.EnableKeyword("_GLOSSYREFLECTIONS_OFF");
                            material.SetFloat(_GlossyReflections, 0f);
                        }
                    }
                }
            }
        }

        
        private void SetupMaterials()
        {
            bool missingShaders = Filters.Init();

            if (missingShaders)
            {
                return;
            }
            
            mat_cage = new Material(Filters.shader_Wireframe);
            mat_cage.name = "TB Cage";
            mat_cage.SetColor(_LineColor, new Color(0.47f, 0.58f, 1, 0.4f));
            mat_cage.SetColor(_FillColor, new Color(0.47f, 0.58f, 1, 0.1f));
            mat_cage.SetFloat(_WireThickness, 200);
            
            mat_lowpoly = new Material(Filters.shader_StandardMetallic);
            mat_lowpoly.name = "TB Lowpoly";
            mat_lowpoly.SetFloat(_Smoothness, 0.5f);
            mat_lowpoly.SetFloat(_Metallic, 0.5f);
            
            mat_cageMeshHighlight = new Material(Filters.shader_Wireframe);
            mat_cageMeshHighlight.name = "TB Cage Mesh";
            mat_cageMeshHighlight.SetColor(_LineColor, new Color(1f, 1f, 0.15f, 0.4f));
            mat_cageMeshHighlight.SetColor(_FillColor, new Color(1f, 1f, 0.15f, 0.2f));
            mat_cageMeshHighlight.SetFloat(_WireThickness, 500);
            
            mat_highpolyMeshHighlight = new Material(Shader.Find("Legacy Shaders/Transparent/Cutout/Soft Edge Unlit"));
            mat_highpolyMeshHighlight.name = "TB Highpoly Mesh";
            mat_highpolyMeshHighlight.SetColor(_Color, new Color(1f, 0f, 0f, 0.7f));
            mat_highpolyMeshHighlight.SetFloat(_Cutoff, 0.9f);
            
            mats_highpoly = new List<Material>();
        }

        private void PrepareScene(bool spawnModels)
        {
            //create camera 
            if(instance_camera != null) DestroyImmediate(instance_camera.gameObject);
            instance_camera = new GameObject("TB_Camera").AddComponent<Camera>();
            instance_camera.gameObject.hideFlags = HideFlags.DontSave;
            instance_camera.transform.position = new Vector3(0.5f, 0.5f, -0.5f);
            instance_camera.clearFlags = CameraClearFlags.Color;
            instance_camera.backgroundColor = Color.gray;
            instance_camera.tag = "MainCamera"; 
            
            //spawn light
            if(instance_directional != null) DestroyImmediate(instance_directional);
            instance_directional = new GameObject("TB_Directional");
            instance_directional.hideFlags = HideFlags.DontSave;
            Light dirLight = instance_directional.AddComponent<Light>();
            Transform dirLightTransform = dirLight.transform;
            dirLightTransform.position = new Vector3(0, 10, 0);
            dirLightTransform.eulerAngles = new Vector3(25, -150, 0);
            dirLight.type = LightType.Directional;
            dirLight.intensity = 0.7f;
            
            //spawn reflection probe
            if(instance_probe != null) DestroyImmediate(instance_probe);
            instance_probe = new GameObject("TB_ReflectionProbe");
            instance_probe.transform.position = new Vector3(1,-1,0);
            ReflectionProbe reflectionProbe = instance_probe.AddComponent<ReflectionProbe>();
            reflectionProbe.mode = ReflectionProbeMode.Custom;
            Texture cubemap =  AssetDatabase.LoadAssetAtPath<Texture>("Assets/TotalBaker/Environment/Cubemap.jpg");
            reflectionProbe.customBakedTexture = cubemap;
            //create high, low and cage
            if (spawnModels)
            {
                SpawnObjects();
            }
        }

        private void DestroyInstances() 
        {
            //destroy high, low and cage
            if (instance_lowpoly != null) DestroyImmediate(instance_lowpoly);
            if (instance_highpoly != null) DestroyImmediate(instance_highpoly);
            if (instance_cage != null)
            {
                if (settings.autoGenerateCage)
                {
                    ObjectUtils.DestroyAllMeshes(instance_cage);
                }
                DestroyImmediate(instance_cage);
            }
            if (instance_directional != null) DestroyImmediate(instance_directional);
            if (instance_probe != null) DestroyImmediate(instance_probe);
            if (instance_camera != null) DestroyImmediate(instance_camera.gameObject); 
        }
        private void LoadIcons()
        {
            icon_saveTextures = (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/TotalBaker/Icons/saveTextures.png", typeof(Texture2D));
            icon_newConfig = (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/TotalBaker/Icons/newConfig.png", typeof(Texture2D));
            icon_saveConfig = (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/TotalBaker/Icons/saveConfig.png", typeof(Texture2D));
            icon_loadConfig = (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/TotalBaker/Icons/loadConfig.png", typeof(Texture2D));
            icon_bake = (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/TotalBaker/Icons/bake.png", typeof(Texture2D));
            icon_folder = (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/TotalBaker/Icons/folder.png", typeof(Texture2D));
            icon_visible = EditorGUIUtility.FindTexture("animationvisibilitytoggleon");
            icon_invisible = EditorGUIUtility.FindTexture("animationvisibilitytoggleoff");
            icon_reflectionsOn = (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/TotalBaker/Icons/reflectionsOn.png", typeof(Texture2D));
            icon_reflectionsOff = (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/TotalBaker/Icons/reflectionsOff.png", typeof(Texture2D));
            icon_refresh = (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/TotalBaker/Icons/refresh.png", typeof(Texture2D));
            icon_saveCage = (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/TotalBaker/Icons/saveCage.png", typeof(Texture2D));
        }
        
        private void SetupLightsDataArrays()
        {
            float[] floatArray = new float[maxSupportedLights];
            Vector4[] vectorArray = new Vector4[maxSupportedLights];

            Filters.mat_addLights = new Material(Filters.shader_AddLights);
            Filters.mat_addLights.SetFloatArray(_LightsValidation, floatArray);
            Filters.mat_addLights.SetFloatArray(_LightsTypes, floatArray);
            Filters.mat_addLights.SetVectorArray(_LightsPositions, vectorArray);
            Filters.mat_addLights.SetVectorArray(_LightsDirections, vectorArray);
            Filters.mat_addLights.SetColorArray(_LightsColors, new Color[100]);
            Filters.mat_addLights.SetFloatArray(_LightsIntensities, floatArray);
            Filters.mat_addLights.SetFloatArray(_LightsRanges, floatArray);
            Filters.mat_addLights.SetFloatArray(_LightsOuterCosAngles, floatArray);
            Filters.mat_addLights.SetFloatArray(_LightsInnerCosAngles, floatArray);
        }

        public static Texture2D CreateCheckerTexture(int res)
        {
            if (Filters.shader_Checkerboard == null)
            {
                return null;
            }

            Material mat = new Material(Filters.shader_Checkerboard);
            int size = (res > 512) ? res : 512;
            mat.SetFloat(_Density, 64 * (size / 512));
            Color col1 = new Color(0.4f, 0.4f, 0.4f);
            Color col2 = new Color(0.6f, 0.6f, 0.6f);
            if (PlayerSettings.colorSpace == ColorSpace.Linear)
            {
                col1 = col1.gamma.gamma;
                col2 = col2.gamma.gamma;
            }
            mat.SetColor(_Color1, col1);
            mat.SetColor(_Color2, col2);
            Texture2D tex = new Texture2D(size, size);
            RenderTexture rt = new RenderTexture(size, size, 0);
            rt.Create();
            RenderTexture temp = new RenderTexture(size, size, 0);
            temp.Create();
            RenderTexture active = RenderTexture.active;
            Graphics.Blit(temp, rt, mat);
            RenderTexture2Dest(rt, tex);
            RenderTexture.active = active;
            DestroyImmediate(mat);
            DestroyRT(temp);
            DestroyRT(rt);
            DestroyImmediate(mat);
            return tex;
        }

        private int ReconstructLayerMaskFromPixel(Color pixel)
        {
            return (int)(pixel.r * int.MaxValue);
        }
        
        //each pixel will contain 32 bit red channel which will represent its ray's target layermask
        //max 32 meshes allowed
        private Texture2D GenerateLayersMap()
        {
            List<Material> mats = new List<Material>();
            
            Material mat = new Material(Shader.Find("Hidden/TB/UV2Layer"));
            mats.Add(mat);
            
            foreach (MeshBinding binding in settings.meshBindings)
            {
                int layerMask = 0;
                layerMask |= 1 << 0;
                Renderer mesh = FindCageMeshByName(binding.sourceMesh);

                //get all its selected target meshes
                string[] selectedMeshesNames = binding.GetSelectedMeshesNames();
                for (int i = 0; i < selectedMeshesNames.Length; i++)
                {
                    Renderer foundMesh = FindHighpolyMeshByName(selectedMeshesNames[i]);
                    if (foundMesh != null)
                    {
                        //add the layerMask the mesh' layer
                        layerMask |= (1 << foundMesh.gameObject.layer);
//                        Debug.Log(mesh.name + " -----> " + foundMesh);
                    }
                }

                Material instance = Instantiate(mat);
                mats.Add(instance);
//                Debug.Log(mesh);
                mesh.sharedMaterials = new Material[]{instance};
//                Debug.Log(mesh.name + ": "+instance.name+" -----> " + layerMask);
                instance.SetInt(_LayerMask, layerMask);
            }
            
            instance_lowpoly.SetActive(false);
            instance_highpoly.SetActive(false);
            instance_cage.SetActive(true);
            
            RenderTexture rt;
            rt = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGBFloat, RTColorSpace);
            rt.Create();
            instance_camera.transform.position = new Vector3(0.5f, 0.5f, -10f);
            instance_camera.orthographic = true;
            instance_camera.targetTexture = rt;
            instance_camera.orthographicSize = 0.5f;
            instance_camera.clearFlags = CameraClearFlags.Color;
            instance_camera.backgroundColor = new Color(0, 0, 0, 0);
            instance_camera.Render();
            Texture2D map = RenderTexture2Texture2D(rt); //ReadPixels(new Rect(0, 0, resolution, resolution), 0, 0);
            instance_camera.targetTexture = null;
            DestroyRT(rt);

//            File.WriteAllBytes("Assets/LayersMap.png", map.EncodeToPNG());
//            AssetDatabase.Refresh();
//            
            foreach (MeshBinding binding in settings.meshBindings)
            {
                Renderer mesh = FindCageMeshByName(binding.sourceMesh);
                mesh.sharedMaterials = new Material[]{mat_cage};
            }
            
            instance_lowpoly.SetActive(true);
            instance_highpoly.SetActive(true);
            RecalculateHighlights();

            for (var i = 0; i < mats.Count; i++)
            {
                DestroyImmediate(mats[i]);
            }

            return map;
        }

        private static void DestroyRT(RenderTexture rt)
        {
            rt.Release();
            DestroyImmediate(rt);
        }
        
        private void RecordUndo(string description)
        {
            Undo.RecordObject(settings, description);
            EditorUtility.SetDirty(settings);
        }

        private void OnUndoPerformed()
        {
            RecalculateHighlights();
            if (settings.autoUpdatePreview)
            {
                if(outputRTDiffuse_original) UpdateDiffuseMap(false);
                if(outputRTHeight_original) UpdateHeightMap();
                if(outputRTNormalMain_original) UpdateNormalMap();
                if(outputRTMetallic_original) UpdateMetallicMap();
                if(outputRTSpecular_original) UpdateSpecularMap();
                if(outputRTAO_original) UpdateAOMap();
                if(outputRTEmissive_original) UpdateEmissiveMap();
                if(outputRTCurvature_original) UpdateCurvatureMap();
            }
            UpdateLowpolyShader();
            UpdateHighpolyShader();
            UpdateLowpolyReflections();
            UpdateHighpolyReflections();
        }
        
        private void AssignBakingLayers()
        {
            Renderer[] allHighpolyMeshes = GetAllHighpolyMeshes();
            for (int i = 0; i < allHighpolyMeshes.Length; i++)
            {
                allHighpolyMeshes[i].gameObject.layer = i+1; //layer 0 reserved for lowpoly and cage
            }
        }
        
        private void RestoreBakingLayers()
        {
            Renderer[] allHighpolyMeshes = GetAllHighpolyMeshes();
            for (var index = 0; index < allHighpolyMeshes.Length; index++)
            {
                allHighpolyMeshes[index].gameObject.layer = 0;
            }
        }

        private static Vector3 GetInterpolatedNormal(CachedMesh mesh, Vector3 barycenter, int triangle)
        {
            Vector3 n0 = mesh.normals[mesh.triangles[triangle]];
            Vector3 n1 = mesh.normals[mesh.triangles[triangle + 1]];
            Vector3 n2 = mesh.normals[mesh.triangles[triangle + 2]];
            Vector3 interpolatedNormal = n0 * barycenter.x + n1 * barycenter.y + n2 * barycenter.z;
            return mesh.rotation * interpolatedNormal.normalized;
        }
        
        private static Color GetInterpolatedVertexColor(CachedMesh mesh, Vector3 barycenter, int triangle)
        {
            Color c0 = mesh.colors[mesh.triangles[triangle]];
            Color c1 = mesh.colors[mesh.triangles[triangle + 1]];
            Color c2 = mesh.colors[mesh.triangles[triangle + 2]];
            return c0 * barycenter.x + c1 * barycenter.y + c2 * barycenter.z;
        }

        private WorldPoint UvToWorld(Vector2 uv)
        {
            bool isMapped;
            int x = 1 - (int) (uv.x * resolution);
            int y = (int) (uv.y * resolution);
            Color c;
            c = originMap_cage.GetPixel(x, y);
            isMapped = !Mathf.Approximately(c.a, 0);
            Vector3 worldPos = new Vector3(c.r, c.g, c.b);
            c = directionMap_cage.GetPixel(x, y);
            Vector3 normal = new Vector3(c.r * 2 - 1, c.g * 2 - 1, c.b * 2 - 1);
            return new WorldPoint(worldPos, normal, isMapped);
        }

        private Vector3[] GetSphereDirections(int numDirections)
        {
            Vector3[] pts = new Vector3[numDirections];
            float inc = Mathf.PI * (3 - Mathf.Sqrt(5));
            float off = 1f / numDirections;

            for (int k = 0; k < numDirections; k++)
            {
                float y = k * off - 1 + (off / 2);
                float r = Mathf.Sqrt(1 - y * y);
                float phi = k * inc;
                float x = Mathf.Cos(phi) * r;
                float z = Mathf.Sin(phi) * r;
                pts[k] = new Vector3(x, y, z);
            }

            return pts;
        }

        //Correct modulo formula (% returns a signed remainder)
        float Nfmod(float a, float b)
        {
            return a - b * Mathf.Floor(a / b);
        }

        private Vector2 Point2UV(Vector3 p, Collider hitMesh, int t)
        {
            int[] triangles = hitMesh.GetComponent<BakingObject>().triangles;
            Vector3[] vertices = hitMesh.GetComponent<BakingObject>().vertices;
            Vector2[] uvs = hitMesh.GetComponent<BakingObject>().uvs;
            Vector3 v1 = vertices[triangles[t]];
            Vector3 v2 = vertices[triangles[t + 1]];
            Vector3 v3 = vertices[triangles[t + 2]];
            Vector2 uv1 = uvs[triangles[t]];
            Vector2 uv2 = uvs[triangles[t + 1]];
            Vector2 uv3 = uvs[triangles[t + 2]];
            Vector2 uv = Point2UV(v1, v2, v3, uv1, uv2, uv3, p);
            return new Vector2((1.0f - Nfmod(uv.x, 1)), Nfmod(uv.y, 1));
        }

        private Vector2 Point2UV(Vector3 p1, Vector3 p2, Vector3 p3, Vector2 uv1, Vector2 uv2, Vector2 uv3, Vector3 f)
        {
            // calculate vectors from point p to vertices v1, v2 and v3:
            Vector3 f1 = p1 - f;
            Vector3 f2 = p2 - f;
            Vector3 f3 = p3 - f;
            // calculate the areas and factors (order of parameters doesn't matter):
            Vector3 va = Vector3.Cross(p1 - p2, p1 - p3); // main triangle area a
            Vector3 va1 = Vector3.Cross(f2, f3); // p1's triangle area / a
            Vector3 va2 = Vector3.Cross(f3, f1); // p2's triangle area / a 
            Vector3 va3 = Vector3.Cross(f1, f2); // p3's triangle area / a
            // calculate barycentric coordinates with sign:
            float a = va.magnitude; // main triangle area
            float a1 = va1.magnitude / a * Mathf.Sign(Vector3.Dot(va, va1));
            float a2 = va2.magnitude / a * Mathf.Sign(Vector3.Dot(va, va2));
            float a3 = va3.magnitude / a * Mathf.Sign(Vector3.Dot(va, va3));
            // find the uv corresponding to point f (uv1/uv2/uv3 are associated to p1/p2/p3):
            return uv1 * a1 + uv2 * a2 + uv3 * a3;
        }



        private Renderer[] GetAllCageMeshes()
        {
            if (!instance_cage)
            {
                return null;
            }
            return instance_cage.GetComponentsInChildren<Renderer>();
        }

        private Renderer[] GetAllLowpolyMeshes()
        {
            if (!instance_lowpoly)
            {
                return null;
            }
            return instance_lowpoly.GetComponentsInChildren<Renderer>();
        }
        
        private Renderer[] GetAllHighpolyMeshes()
        {
            if (!instance_highpoly)
            {
                return null;
            }
            return instance_highpoly.GetComponentsInChildren<Renderer>();
        }
        
        private string GetCurrentCageMeshName()
        {
            return settings.meshBindings[settings.selectedCageMeshIndex].sourceMesh;
        }
        
        private string[] GetCurrentHighpolyMeshesNames()
        {
            return settings.meshBindings[settings.selectedCageMeshIndex].selections.Where(x => x.isSelected).Select(x => x.meshName).ToArray();
        }

        private Renderer FindCageMeshByName(string meshName)
        {
            if (meshName == instance_cage.name)
            {
                return instance_cage.GetComponent<Renderer>();
            }
            Transform found = instance_cage.transform.Find(meshName);
            if (found == null)
            {
                return null;
            }
            return found.GetComponent<Renderer>();
        }
        
        private Renderer FindHighpolyMeshByName(string meshName)
        {
            if (meshName == instance_highpoly.name)
            {
                return instance_highpoly.GetComponent<Renderer>();
            }
            Transform found = instance_highpoly.transform.Find(meshName);
            if (found == null)
            {
                return null;
            }
            return found.GetComponent<Renderer>();
        }

        private void OnHighpolyMeshesSelectAllChanged()
        {
            if (instance_cage == null || settings.cageMeshObjects == null)
            {
                return;
            }

            for (var i = 0; i < settings.highpolyMeshNames.Length; i++)
            {
                settings.meshBindings[settings.selectedCageMeshIndex].selections[i].isSelected = allHighpolyMeshesSelected;
            } 
            
            RecalculateHighlights(); 
        }

        private void RecalculateMixedToggle()
        {
            MeshSelection[] selections = settings.meshBindings[settings.selectedCageMeshIndex].selections;
            if (selections == null || selections.Length < 1)
            {
                return;
            }
            bool curr = settings.meshBindings[settings.selectedCageMeshIndex].selections[0].isSelected;
            for (var i = 1; i < settings.highpolyMeshNames.Length; i++)
            {
                if (settings.meshBindings[settings.selectedCageMeshIndex].selections[i].isSelected != curr)
                {
                    mixedToggle = true;
                    allHighpolyMeshesSelected = false;
                    return;
                }
            }
            mixedToggle = false;
            allHighpolyMeshesSelected = curr;
        }
        
        private void RecalculateHighlights()
        {
            //remove highlight from all cage meshes
            Renderer[] cageMeshes = GetAllCageMeshes();
            if (cageMeshes == null)
            {
                return;
            }
            foreach (Renderer renderer in cageMeshes)
            {
                RemoveMaterialFromRenderer(renderer, mat_cageMeshHighlight);
            }
            
            //remove highlight from all highpoly meshes
            Renderer[] highpolyRends = GetAllHighpolyMeshes();
            foreach (Renderer renderer in highpolyRends)
            {
                RemoveMaterialFromRenderer(renderer, mat_highpolyMeshHighlight);
            }
            
            if (!settings.bindMeshes)
            {
                SceneView.RepaintAll();
                return;
            }
            
            //add highlight to current cage mesh
            if (settings.highlightCurrentCageMesh)
            {
                Renderer currentCageMesh = FindCageMeshByName(GetCurrentCageMeshName());
                AddMaterialToRenderer(currentCageMesh, mat_cageMeshHighlight);
            }

            //add highlight to current cage mesh
            if (settings.highlightCurrentBoundMeshes)
            {
                string[] currentHighpolyMeshNames = GetCurrentHighpolyMeshesNames();
                foreach (string meshName in currentHighpolyMeshNames)
                {
                    Renderer currentHighpolyMesh = FindHighpolyMeshByName(meshName);
                    AddMaterialToRenderer(currentHighpolyMesh, mat_highpolyMeshHighlight);
                }
            }
            
            SceneView.RepaintAll();
        }

        private void ResetLowpolyMaterial()
        {
            mat_lowpoly.SetTexture(_MainTex, null);
            mat_lowpoly.SetTexture(_BumpMap, null);
            mat_lowpoly.SetTexture(_ParallaxMap, null);
            mat_lowpoly.SetTexture(_MetallicGlossMap, null);
            mat_lowpoly.SetTexture(_SpecGlossMap, null);
            mat_lowpoly.SetTexture(_OcclusionMap, null);
            mat_lowpoly.SetTexture(_EmissionMap, null);
        }

        private void AddMaterialToRenderer(Renderer rend, Material mat)
        {
            if (rend == null)
            {
                return;
            }
            
            Material[] mats = rend.sharedMaterials;
            Material[] newMats = new Material[mats.Length+1];
            for (int i = 0; i < mats.Length; i++)
            {
                newMats[i] = mats[i];
            }

            newMats[newMats.Length-1] = mat;
            rend.sharedMaterials = newMats;
        }

        private void RemoveMaterialFromRenderer(Renderer rend, Material mat)
        {
            if (rend == null)
            {
                return;
            }
            
            Material[] mats = rend.sharedMaterials;
            List<Material> newMats = mats.ToList();
            newMats.Remove(mat);
            rend.sharedMaterials = newMats.ToArray();
        }

        private void RecalculateReflectionStyles()
        {
            if (reflectionsOnStyle == null)
            {
                reflectionsOnStyle = new GUIStyle(GUI.skin.toggle);
                reflectionsOnStyle.active.background = icon_reflectionsOn;
                reflectionsOnStyle.normal.background = icon_reflectionsOn;
                reflectionsOnStyle.focused.background = icon_reflectionsOn;
                reflectionsOnStyle.hover.background = icon_reflectionsOn;
                reflectionsOnStyle.onActive.background = icon_reflectionsOn;
                reflectionsOnStyle.onNormal.background = icon_reflectionsOn;
                reflectionsOnStyle.onFocused.background = icon_reflectionsOn;
                reflectionsOnStyle.onHover.background = icon_reflectionsOn;
            }
            if (reflectionsOffStyle == null)
            {
                reflectionsOffStyle = new GUIStyle(GUI.skin.toggle);
                reflectionsOffStyle.active.background = icon_reflectionsOff;
                reflectionsOffStyle.normal.background = icon_reflectionsOff;
                reflectionsOffStyle.focused.background = icon_reflectionsOff;
                reflectionsOffStyle.hover.background = icon_reflectionsOff;
                reflectionsOffStyle.onActive.background = icon_reflectionsOff;
                reflectionsOffStyle.onNormal.background = icon_reflectionsOff;
                reflectionsOffStyle.onFocused.background = icon_reflectionsOff;
                reflectionsOffStyle.onHover.background = icon_reflectionsOff;
            }
        }
        
        private void RecalculateVisibilityToggleStyles()
        {
            if (visibilityStyle == null)
            {
                visibilityStyle = new GUIStyle(GUI.skin.toggle);
                visibilityStyle.active.background = icon_visible;
                visibilityStyle.normal.background = icon_visible;
                visibilityStyle.focused.background = icon_visible;
                visibilityStyle.hover.background = icon_visible;
                visibilityStyle.onActive.background = icon_visible;
                visibilityStyle.onNormal.background = icon_visible;
                visibilityStyle.onFocused.background = icon_visible;
                visibilityStyle.onHover.background = icon_visible;
            }
            if (invisibilityStyle == null)
            {
                invisibilityStyle = new GUIStyle(GUI.skin.toggle);
                invisibilityStyle.active.background = icon_invisible;
                invisibilityStyle.normal.background = icon_invisible;
                invisibilityStyle.focused.background = icon_invisible;
                invisibilityStyle.hover.background = icon_invisible;
                invisibilityStyle.onActive.background = icon_invisible;
                invisibilityStyle.onNormal.background = icon_invisible;
                invisibilityStyle.onFocused.background = icon_invisible;
                invisibilityStyle.onHover.background = icon_invisible;
            }
        }

        private void RecalculateBindings()
        {
            if (!needsBindingRecalculation)
            {
                return;
            }
            
            //recalculate highpoly meshes
            settings.highpolyMeshObjects = GetAllMeshObjects(instance_highpoly);
            settings.highpolyMeshNames = settings.highpolyMeshObjects.Select(x => x.gameObject.name).ToArray();
            
            //recalculate cage meshes
            settings.cageMeshObjects = GetAllMeshObjects(instance_cage);
            settings.cageMeshNames = settings.cageMeshObjects.Select(x => x.gameObject.name).ToArray();
            
            //create new bindings
            settings.meshBindings = new MeshBinding[settings.cageMeshObjects.Length];
            for (var i = 0; i < settings.cageMeshObjects.Length; i++)
            {
                MeshFilter cageMeshObject = settings.cageMeshObjects[i];
                settings.meshBindings[i] = new MeshBinding(cageMeshObject.name, settings.highpolyMeshNames);
            }

            //reset recalculation state
            needsBindingRecalculation = false;
        }

        private MeshFilter[] GetAllMeshObjects(GameObject root)
        {
            return root.GetComponentsInChildren<MeshFilter>();
        }

        private void UpdateDiffuseMap(bool updateLightsData)
        {
            if (outputRTDiffuse == null)
            {
                return;
            }
            if (settings.bakeAOIntoDiffuse)
            {
                Filters.OverlayAO(outputRTDiffuse_original, outputRTAO, outputRTDiffuse);
            }
            else
            {
                Filters.Copy(outputRTDiffuse_original, outputRTDiffuse);
            }

            if (settings.bakeLights)
            {
                if (updateLightsData)
                {
                    UpdateLightsData();
                }
                if (hasLights) AddLights(outputRTDiffuse);
            }
            
            Filters.Dilate(outputRTDiffuse, settings.dilationDiffuse, settings.dilationModeDiffuse);

            UpdatePreviewTexture();
            SceneView.RepaintAll();
        }

        private void UpdateMetallicMap()
        {
            if (outputRTMetallic == null)
            {
                return;
            }
            Filters.Copy(outputRTMetallic_original, outputRTMetallic);
            Filters.Dilate(outputRTMetallic, settings.dilationMetallic, settings.dilationModeMetallic);

            if (settings.createMaskMap)
            {
                UpdateMaskMap();
            }
            
            UpdatePreviewTexture();
            SceneView.RepaintAll();
        }
        
        private void UpdateSpecularMap()
        {
            if (outputRTSpecular == null)
            {
                return;
            }
            Filters.Copy(outputRTSpecular_original, outputRTSpecular);
            Filters.Dilate(outputRTSpecular, settings.dilationSpecular, settings.dilationModeSpecular);

            UpdatePreviewTexture();
            SceneView.RepaintAll();
        }

        private void UpdateEmissiveMap()
        {
            if (outputRTEmissive == null)
            {
                return;
            }
            Filters.Copy(outputRTEmissive_original, outputRTEmissive);
            Filters.Dilate(outputRTEmissive, settings.dilationEmissive, settings.dilationModeEmissive); 

            UpdatePreviewTexture();
            SceneView.RepaintAll();
        }
        
        private void UpdateCurvatureMap()
        {
            if (outputRTCurvature == null)
            {
                return;
            }
            if (settings.curvatureDetectionMode == Settings.CurvatureDetectionMode.NormalMap)
            {
                Filters.Copy(outputRTNormalDetails != null ? outputRTNormalCombined : outputRTNormalMain, outputRTCurvature);
            }
            else
            {
                Filters.Copy(outputRTCurvature_original, outputRTCurvature);
            }
            Filters.Dilate(outputRTCurvature, settings.dilationCurvature, settings.dilationModeCurvature); 
            Filters.ApplyBackground(outputRTCurvature, Color.gray);
            Filters.Normal2Curvature(outputRTCurvature, settings.curvatureChannels, settings.multiplierCurvature, settings.smoothnessCurvature);
            Filters.BilateralBlur(outputRTCurvature, settings.denoiseCurvatureAmount);
            
            UpdatePreviewTexture();
            SceneView.RepaintAll();
        }
        
        private void UpdateMaskMap()
        {
            if (outputRTMask == null)
            {
                return;
            }
            
            //compose mask map by merging metallic and AO maps
            Filters.MetallicAndAO2Mask(outputRTMask, outputRTMetallic, outputRTAO);
            
            UpdatePreviewTexture();
            SceneView.RepaintAll();
        }

        private void UpdateAOMap()
        {
            if (outputRTAO == null)
            {
                return;
            }
            if (settings.aoType == Settings.AOType.Fast)
            {
                Filters.FastAO(outputRTAO_original, originMap_highpoly, directionMap_highpoly, settings.AOStrength, settings.biasAO, settings.sampleRadiusAO, Color.clear);
                Filters.RemapTexture(outputRTAO_original, outputRTAO, outputTex_uv);
                Filters.Erode(outputRTAO, 5, Color.white);
                Filters.Dilate(outputRTAO, settings.dilationAO, settings.dilationModeAO);
            }
            else
            {
                Filters.Copy(outputRTAO_original, outputRTAO);
                Filters.Dilate(outputRTAO, settings.dilationAO, settings.dilationModeAO);
            } 

            Filters.ApplyBackground(outputRTAO, Color.white);

            Filters.Contrast(outputRTAO, settings.brightnessAO, settings.contrastAO);

            Filters.BilateralBlur(outputRTAO, settings.denoiseAOAmount);
            
            Filters.GaussianBlur(outputRTAO, settings.blurAOAmount);

            if (settings.bakeAOIntoDiffuse)
            {
                UpdateDiffuseMap(false);
            }
            
            if (settings.createMaskMap)
            {
                UpdateMaskMap();
            }

            UpdatePreviewTexture();
            SceneView.RepaintAll();
            
        }

        private void UpdateNormalMap()
        {
            if (settings.normalsDetectionMode == Settings.NormalsDetectionMode.HeightMap  && outputRTHeight_original != null)
            {
                Filters.Copy(outputRTHeight_original, outputRTHeight);
                Filters.Dilate(outputRTHeight, settings.dilationHeight, settings.dilationModeHeight);
                Filters.Contrast(outputRTHeight, settings.brightnessHeight, settings.contrastHeight);
                Filters.ApplyBackground(outputRTHeight, Color.white);
                Filters.GaussianBlur(outputRTHeight, (int) settings.blurHeightAmount);
                Filters.Grayscale2Normal(outputRTHeight, outputRTNormalMain_original, settings.bumpStrength);
                Filters.Copy(outputRTNormalMain_original, outputRTNormalMain);
                Filters.BilateralBlur(outputRTNormalMain, settings.denoiseNormalMainAmount);
                Filters.InvertChannels(outputRTNormalMain, settings.invertNormalsRed, settings.invertNormalsGreen, false);
            }
            else //interpolation or geometry
            {
                Filters.Copy(outputRTNormalMain_original, outputRTNormalMain);
                Filters.Dilate(outputRTNormalMain, settings.dilationNormal, settings.dilationModeNormal);
            }

            if (settings.bakeExistingNormalMap)
            {
                Filters.Copy(outputRTNormalDetails_original, outputRTNormalDetails);
                Filters.ApplyBackground(outputRTNormalMain, tangentColor);
                Filters.BilateralBlur(outputRTNormalMain, settings.denoiseNormalMainAmount);
                Filters.ApplyBackground(outputRTNormalDetails, tangentColor);
                Filters.BilateralBlur(outputRTNormalDetails, settings.denoiseNormalDetailsAmount);
                Filters.CombineNormals(outputRTNormalMain, outputRTNormalDetails, outputRTNormalCombined);
            }
            else
            {
                if (settings.normalsDetectionMode != Settings.NormalsDetectionMode.HeightMap)
                {
                    Filters.ApplyBackground(outputRTNormalMain, tangentColor);
                    Filters.BilateralBlur(outputRTNormalMain, settings.denoiseNormalMainAmount);
                    Filters.InvertChannels(outputRTNormalMain, settings.invertNormalsRed, settings.invertNormalsGreen, false);
                }
            }
            
            //pack normals only if the current version doesn't support auto-packing
            #if !UNITY_2017_3_OR_NEWER
                    Pack(outputRTNormalCombined, packedNormalMap);
            #endif

            UpdatePreviewTexture();
            SceneView.RepaintAll();

        }

        private void UpdateHeightMap()
        {
            Filters.Copy(outputRTHeight_original, outputRTHeight);
            Filters.Dilate(outputRTHeight, settings.dilationHeight, settings.dilationModeHeight);
            Filters.Contrast(outputRTHeight, settings.brightnessHeight, settings.contrastHeight);
            Filters.ApplyBackground(outputRTHeight, Color.white);
            Filters.GaussianBlur(outputRTHeight, (int) settings.blurHeightAmount);
            
            UpdatePreviewTexture();
            SceneView.RepaintAll();
        }
        
        /// <summary>
        /// Displays a vertical list of toggles and returns the index of the selected item.
        /// </summary>
        public static int ToggleList(int selected, string[] items)
        {
            // Keep the selected index within the bounds of the items array
            selected = selected < 0 ? 0 : selected >= items.Length ? items.Length - 1 : selected;
 
            GUILayout.BeginVertical();
            for (int i = 0; i < items.Length; i++)
            {
                // Display toggle. Get if toggle changed.
                bool change = GUILayout.Toggle(selected == i, items[i], EditorStyles.radioButton);
                // If changed, set selected to current index.
                if (change)
                    selected = i;
            }
            GUILayout.EndVertical();
 
            // Return the currently selected item's index
            return selected;
        }

        #endregion



        #region Post-processing functions

        public void UpdatePreviewTexture()
        {
            if (previewTexture == null)
            {
                previewTexture = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.ARGBFloat, RTColorSpace);
                previewTexture.Create();
            }
            
            if (originalPreviewTex == null)
            {
                previewTexture = null;
                return;
            }
            
            Graphics.Blit(originalPreviewTex, previewTexture);
            
            //show only selected channels
            Filters.IsolateChannels(previewTexture, channelsToolbarInt);
        }
        
       
 


        public void AddLights(RenderTexture source)
        {
            if (source == null)
            {
                return;
            }
            if (Filters.mat_addLights == null)
            { //this should happen only when recompiling while the window is still open
                Filters.mat_addLights = new Material(Filters.shader_AddLights);
                UpdateLightsData();
            }

            RenderTexture blit = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGBFloat, RTColorSpace);
            Graphics.Blit(source, blit, Filters.mat_addLights);
            Graphics.Blit(blit, source);
            RenderTexture.ReleaseTemporary(blit);
        }


        public static RenderTexture Texture2D2RenderTexture(Texture2D source)
        {
            //for some reason, ARGBFloat produces artifacts. Switched back to ARGB64.
            RenderTexture rt = new RenderTexture(source.width, source.height, 0, RenderTextureFormat.ARGB64, RTColorSpace);
            rt.Create();
            Graphics.Blit(source, rt);
            return rt;
        }

        public static Texture2D RenderTexture2Texture2D(RenderTexture rt)
        {
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture.active = rt;
            Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGBAFloat, false);
            tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            tex.Apply();
            RenderTexture.active = currentRT;
            return tex;
        }

        public static void RenderTexture2Dest(RenderTexture rt, Texture2D dest)
        {
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture.active = rt;
            dest.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            dest.Apply();
            RenderTexture.active = currentRT;
        }

        #endregion
        

//        public static void MeshToObj(MeshFilter mf, Transform lowpoly, string path)
//        {
//            string meshToString;
//
//            Mesh m = mf.sharedMesh;
//            Material[] mats = mf.GetComponent<Renderer>().sharedMaterials;
//            Vector3 scale = lowpoly.transform.localScale;
//
//            StringBuilder sb = new StringBuilder();
//
//            sb.Append("g ").Append(mf.name).Append("\n");
//            foreach (Vector3 v in m.vertices)
//            {
//                sb.Append(string.Format("v {0} {1} {2}\n", -v.x / scale.x, v.y / scale.y, v.z / scale.z).Replace(",", "."));
//            }
//
//            sb.Append("\n");
//            foreach (Vector3 v in m.normals)
//            {
//                sb.Append(string.Format("vn {0} {1} {2}\n", -v.x, v.y, v.z).Replace(",", "."));
//            }
//
//            sb.Append("\n");
//            foreach (Vector3 v in m.uv)
//            {
//                sb.Append(string.Format("vt {0} {1}\n", v.x, v.y).Replace(",", "."));
//            }
//
//            for (int material = 0; material < m.subMeshCount; material++)
//            {
//                sb.Append("\n");
//                sb.Append("usemtl ").Append(mats[material].name).Append("\n");
//                sb.Append("usemap ").Append(mats[material].name).Append("\n");
//
//                int[] triangles = m.GetTriangles(material);
//                for (int i = 0; i < triangles.Length; i += 3)
//                {
//                    sb.Append(string.Format("f {1}/{1}/{1} {0}/{0}/{0} {2}/{2}/{2}\n",
//                        triangles[i] + 1, triangles[i + 1] + 1, triangles[i + 2] + 1));
//                }
//            }
//
//            meshToString = sb.ToString();
//
//            Directory.CreateDirectory(Path.GetDirectoryName(path));
//            File.WriteAllText(path, meshToString);
//        }

       


        #region Save/Load

        private void SaveConfiguration()
        {
            string currentSettingsAssetPath = AssetDatabase.GetAssetPath(settings);

            string directory = EditorPrefs.GetString("TB_SaveDirectory");
            if (directory == "")
            {
                directory = "Assets/TotalBaker/Configs";
            }

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            string file = EditorUtility.SaveFilePanel("Save configuration", directory, settings.name, "asset");

            if (string.IsNullOrEmpty(file))
            {
                return;
            }
            
            string directoryName = Path.GetDirectoryName(file);
            EditorPrefs.SetString("TB_SaveDirectory", directoryName);
            
            //make path relative to project folder
            if (file.StartsWith(Application.dataPath)) {
                file =  "Assets" + file.Substring(Application.dataPath.Length);
            }
            
            Settings newSettings = Instantiate(settings);
            newSettings.isTemporary = false;
            AssetDatabase.CreateAsset(newSettings, file);
            if (currentSettingsAssetPath == "")
            {
                DestroyImmediate(settings); //we were in a temporary config
            }

            settings = null;
			AssetDatabase.Refresh();
            LoadConfiguration(file);
        }
        
        private void LoadConfiguration(string path)
        {
            settings = AssetDatabase.LoadAssetAtPath<Settings>(path);

            if (last_lowpoly != settings.lowpoly)
            {
                OnHighpolyChanged(false);
            }

            if (last_highpoly != settings.highpoly)
            {
                OnLowpolyChanged(false);
            }
            if (last_cage != settings.cage)
            {
                OnCageChanged(false);
            }
            
            lowpolyModelsToReimport = ImportUtility.GetAllModelsImportSettings(settings.lowpoly);
            highpolyModelsToReimport = ImportUtility.GetAllModelsImportSettings(settings.highpoly);
            cageModelsToReimport = ImportUtility.GetAllModelsImportSettings(settings.cage);
        }

        private void CreateNewConfig()
        {
            if (settings != null)
            {
                string currentSettingsAssetPath = AssetDatabase.GetAssetPath(settings);
                if (currentSettingsAssetPath == "")
                {
                    bool saveNeeded = EditorUtility.DisplayDialog("New configuration", "The current configuration has unsaved changes. Do you want to save it?", "Yes", "No");
                    if (saveNeeded)
                    {
                        SaveConfiguration();
                    }
                    //config is temporary, destroy it
                    else
                    {
                        DestroyImmediate(settings);
                    }
                }
            }
            settings = CreateInstance<Settings>();
            settings.name = "New Baking Config";
        }

        private void LoadConfiguration()
        {
            string directory = EditorPrefs.GetString("TB_SaveDirectory");
            
            if (directory == "")
            {
                directory = "Assets/TotalBaker/Configs";
            }
            string selected = EditorUtility.OpenFilePanel("Load configuration", directory, "asset");
            GUI.FocusControl(""); //remove focus, this prevents focused fields not updating
            if (selected == "")
            {
                return;
            }

            EditorPrefs.SetString("TB_SaveDirectory", Path.GetDirectoryName(selected));
            
            //make path relative to project folder
            if (selected.StartsWith(Application.dataPath)) {
                selected =  "Assets" + selected.Substring(Application.dataPath.Length);
            }

            LoadConfiguration(selected);
            
            SpawnObjects();
        }
    
        
        private void SaveSession()
        {
            EditorPrefs.SetString("TB_PreviewScenePath", previewScenePath);
            EditorPrefs_SetStringArray("TB_LoadedScenesPaths", loadedScenesPaths);
            EditorPrefs.SetString("TB_LastConfig", AssetDatabase.GetAssetPath(settings));
        }

        private void EditorPrefs_SetStringArray(string key, string[] array)
        {
            if (array == null)
            {
                return;
            }
            string csv = string.Join(",", array);
            EditorPrefs.SetString(key, csv);
        }
        
        private string[] EditorPrefs_GetStringArray(string key)
        {
            string csv = EditorPrefs.GetString(key);
            string[] strings = csv.Split(","[0]);
            return strings;
        }

        private void LoadLastSessionOrCreateNew() 
        {
            GUI.FocusControl(""); //remove focus, this prevents focused fields not updating

            string config = EditorPrefs.GetString("TB_LastConfig");
            
            if (config != "")
            {
                settings = AssetDatabase.LoadAssetAtPath<Settings>(config); 
            }

            if (settings == null)
            {
                //if there's no settings file to be restored, create a new one
                settings = CreateInstance<Settings>();
                settings.name = "New Baking Config";
            }
 
            if (ValidModelsProvided())
            {
                hasOneMaterialPerMesh_highpoly = ObjectUtils.HasOneMaterialPerMesh(settings.highpoly);
                hasOneMaterialPerMesh_lowpoly = ObjectUtils.HasOneMaterialPerMesh(settings.lowpoly);
                
                SpawnObjects();
                
                if (last_lowpoly != settings.lowpoly)
                {
                    OnLowpolyChanged(false);
                }
                if (last_highpoly != settings.highpoly)
                {
                    OnHighpolyChanged(false);
                }
                if (last_cage != settings.cage)
                {
                    OnCageChanged(false);
                }
            }
        }

        private bool ValidModelsProvided()
        {
            bool isValidLowpoly = settings.lowpoly != null;
            bool isValidHighpoly = settings.highpoly != null;
            bool isValidCage = settings.autoGenerateCage || (!settings.autoGenerateCage && settings.cage != null);
            return isValidLowpoly && isValidHighpoly && isValidCage;
        }
        

        #endregion

    }

}

#endif