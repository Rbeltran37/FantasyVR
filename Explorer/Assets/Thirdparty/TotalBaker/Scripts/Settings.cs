/*====================================================
*
* TOTAL BAKER
*
* Francesco Cucchiara - 3POINT SOFT
*
=====================================================*/

#if UNITY_EDITOR

namespace TB
{
    using System;
    using UnityEngine;

    [Serializable]
    [CreateAssetMenu(fileName = "TB Configuration", menuName = "Total Baker/Create Configuration", order = 1)]
    public class Settings : ScriptableObject
    {
        //models
        public enum PreviewMaterialType
        {
            Unlit,
            Diffuse,
            StandardMetallic,
            StandardSpecular
        }

        public enum PreviewMaterialStandardBlend
        {
            Opaque,
            Cutout,
            Fade,
            Transparent
        }
        
        public enum PreviewMaterialLegacyBlend
        {
            Opaque,
            Cutout,
            Transparent
        }

        public enum VertexColorsMode
        {
            Multiply,
            Overwrite
        }

        public enum NormalsImportType
        {
            Import,
            Calculate
        }

        public enum DilationMode
        {
            Opaque,
            AlphaMask
        }
        
        public enum CurvatureDetectionMode
        {
            Geometry,
            NormalMap
        }
        
        public enum CurvatureChannels
        {
            Single,
            Dual
        }
        
        public bool isTemporary = true;
        public float maxRayLength = 0.5f;

        public GameObject lowpoly;
        public GameObject cage;
        public GameObject highpoly;
        public NormalsImportType importNormalsCage = NormalsImportType.Calculate;
        public NormalsImportType importNormalsLowpoly = NormalsImportType.Calculate;
        public NormalsImportType importNormalsHighpoly = NormalsImportType.Calculate;
        public int smoothAngleCage = 0;
        public int smoothAngleLowpoly = 60;
        public int smoothAngleHighpoly = 60;
        public bool autoGenerateCage = false;
        public float cageOffset = 0.01f;
        public bool bindMeshes = false;
        public MeshFilter[] cageMeshObjects = null;
        public MeshFilter[] highpolyMeshObjects = null;
        public MeshBinding[] meshBindings = null;
        public string[] cageMeshNames = null;
        public string[] highpolyMeshNames = null;
        
        //cage & bindings settings
        public bool showCage = true;
        public bool highlightCurrentCageMesh = true;
        public bool highlightCurrentBoundMeshes = true;
        public int selectedCageMeshIndex;

        public PreviewMaterialType previewMaterialTypeLowpoly = PreviewMaterialType.StandardMetallic;
        public PreviewMaterialType previewMaterialTypeHighpoly = PreviewMaterialType.StandardMetallic;
        public PreviewMaterialStandardBlend previewMaterialBlendStandardLowpoly = PreviewMaterialStandardBlend.Opaque;
        public PreviewMaterialStandardBlend previewMaterialBlendStandardHighpoly = PreviewMaterialStandardBlend.Opaque;
        public PreviewMaterialLegacyBlend previewMaterialBlendLegacyLowpoly = PreviewMaterialLegacyBlend.Opaque;
        public PreviewMaterialLegacyBlend previewMaterialBlendLegacyHighpoly = PreviewMaterialLegacyBlend.Opaque;
        public bool reflectionsLowpoly = true;
        public bool reflectionsHighpoly = true;
        
        //diffuse
        public bool createDiffuseMap = true;
        public bool bakeVertexColors = false;
        public VertexColorsMode vertexColorsMode = VertexColorsMode.Multiply;
        public DilationMode dilationModeDiffuse = DilationMode.Opaque;
        public int dilationDiffuse = 10;

        //normal
        public bool createNormalMap = true;
        public NormalsDetectionMode normalsDetectionMode = NormalsDetectionMode.SurfaceSmooth;
        public bool bakeExistingNormalMap = true;
        public bool invertNormalsRed = false;
        public bool invertNormalsGreen = false;
        public float bumpStrength = 10f;
        public DilationMode dilationModeNormal = DilationMode.Opaque;
        public int dilationNormal = 10;
        public float denoiseNormalMainAmount = 0;
        public float denoiseNormalDetailsAmount = 0;

        //height 
        public bool createHeightMap = true;
        public bool _16bpcHeight = false; // use 16bpc for heightmap (useful to convert into normal map or AO map)
        public DilationMode dilationModeHeight = DilationMode.Opaque;
        public int dilationHeight = 10;
        public float blurHeightAmount = 0;
        public float brightnessHeight = 0.0f;
        public float contrastHeight = 1.0f;

        //AO
        public bool createAOMap = false;
        public AOType aoType = AOType.Classic;
        public float AOStrength = 1f;
        public float AOMaxRaySpread = 0.2f;
        public float sampleRadiusAO = 0.015f;
        public int AORaysPerPoint = 10;
        public DilationMode dilationModeAO = DilationMode.Opaque;
        public int dilationAO = 10;
        public float biasAO = -0.3f;
        public float denoiseAOAmount = 5f;
        public int blurAOAmount = 0;
        public float brightnessAO = 0.0f;
        public float contrastAO = 1.0f;
        public bool bakeAOIntoDiffuse = false;

        //metallic
        public bool createMetallicMap = false;
        public DilationMode dilationModeMetallic = DilationMode.Opaque;
        public int dilationMetallic = 10;

        //specular
        public bool createSpecularMap = false;
        public DilationMode dilationModeSpecular = DilationMode.Opaque;
        public int dilationSpecular = 10;

        //emissive
        public bool createEmissiveMap = false;
        public DilationMode dilationModeEmissive = DilationMode.Opaque;
        public int dilationEmissive = 10;
        
        //curvature
        public bool createCurvatureMap = false;
        public CurvatureDetectionMode curvatureDetectionMode = CurvatureDetectionMode.Geometry;
        public CurvatureChannels curvatureChannels = CurvatureChannels.Single;
        public float multiplierCurvature = 1;
        public DilationMode dilationModeCurvature = DilationMode.Opaque;
        public int dilationCurvature = 10;
        public float denoiseCurvatureAmount = 5f;
        public int smoothnessCurvature = 5;
        
        //Mask (HDRP Channel Packed)
        public bool createMaskMap = false;

        //output
        public enum Resolution
        {
            _32 = 32,
            _64 = 64,
            _128 = 128,
            _256 = 256,
            _512 = 512,
            _1024 = 1024,
            _2048 = 2048,
            _4096 = 4096
        }

        public enum NormalsDetectionMode
        {
            SurfaceFlat = 0,
            SurfaceSmooth = 1,
            HeightMap = 2
        }

        public enum AOType
        {
            Classic = 0,
            Fast = 1
        }

        public Resolution resolution = Resolution._512;
        public string outputBaseName = "";
        public string outputFolder = "Assets/TotalBaker/Bakes";
        public bool autoUpdatePreview = true; //auto update texture preview
        public bool showConsoleLog = true;

        //lighting
        public bool bakeLights;
        public bool bakeAllActiveLights = false;
        public bool customAmbientColor = true;
        public Color ambientColor = Color.white;
    }
}

#endif
