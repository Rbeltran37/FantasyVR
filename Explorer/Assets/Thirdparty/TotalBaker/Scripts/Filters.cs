/*====================================================
*
* TOTAL BAKER
*
* Francesco Cucchiara - 3POINT SOFT
*
=====================================================*/

#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;

namespace TB
{

    public static class Filters
    {
        //shaders
        public static Shader shader_IsolateChannels;
        public static Shader shader_Wireframe;
        public static Shader shader_StandardMetallic;
        public static Shader shader_StandardSpecular;
        public static Shader shader_Diffuse;
        public static Shader shader_Unlit;
        public static Shader shader_UV2WorldPos;
        public static Shader shader_UV2Normal;
        public static Shader shader_World2Tangent;
        public static Shader shader_Tangent2World;
        public static Shader shader_Checkerboard;
        public static Shader shader_PackNormals;
        public static Shader shader_AddLights;
        public static Shader shader_AddBackground;

        //public static Shader shader_Linear2Gamma;
        public static Shader shader_InvertChannels;
        public static Shader shader_GaussianBlur;
        public static Shader shader_LinearBlur;
        public static Shader shader_BilateralBlur;
        public static Shader shader_OverwriteAlpha;
        public static Shader shader_Normal2Curvature;
        public static Shader shader_CurvatureGrayscale2Dual;
        public static Shader shader_Dilate;
        public static Shader shader_Erode;
        public static Shader shader_Contrast;
        public static Shader shader_Remap;
        public static Shader shader_Overlay;
        public static Shader shader_OverlayAO;
        public static Shader shader_CombineNormals;
        public static Shader shader_Bump;
        public static Shader shader_AOMap;
        public static Shader shader_UnpackNormals;
        public static Shader shader_MetallicAndAO2Mask;

        //materials
        public static Material mat_isolateChannels;
        public static Material mat_addLights;
        public static Material mat_PackNormals;

        public static Material mat_AddBackground;

//        public static Material mat_Linear2Gamma;
        public static Material mat_InvertChannels;
        public static Material mat_GaussianBlur;
        public static Material mat_LinearBlur;
        public static Material mat_BilateralBlur;
        public static Material mat_OverwriteAlpha;
        public static Material mat_Normal2Curvature;
        public static Material mat_CurvatureGrayscale2Dual;
        public static Material mat_Dilate;
        public static Material mat_Erode;
        public static Material mat_Contrast;
        public static Material mat_Overlay;
        public static Material mat_OverlayAO;
        public static Material mat_Remap;
        public static Material mat_CombineNormals;
        public static Material mat_Bump;
        public static Material mat_AOMap;
        public static Material mat_UnpackNormals;
        public static Material mat_MetallicAndAO2Mask;

        //Shader properties

        private static readonly int _Texture = Shader.PropertyToID("_Texture");
        private static readonly int _Background = Shader.PropertyToID("_Background");
        private static readonly int _R = Shader.PropertyToID("_R");
        private static readonly int _G = Shader.PropertyToID("_G");
        private static readonly int _B = Shader.PropertyToID("_B");
        private static readonly int _A = Shader.PropertyToID("_A");
        private static readonly int _MainTex = Shader.PropertyToID("_MainTex");
        private static readonly int _OverlayTex = Shader.PropertyToID("_OverlayTex");
        private static readonly int _Factor = Shader.PropertyToID("_Factor");
        private static readonly int _SigmaP = Shader.PropertyToID("_SigmaP");
        private static readonly int _SigmaR = Shader.PropertyToID("_SigmaR");
        private static readonly int _Offset = Shader.PropertyToID("_Offset");
        private static readonly int _AlphaMask = Shader.PropertyToID("_AlphaMask");
        private static readonly int _MaxSteps = Shader.PropertyToID("_MaxSteps");
        private static readonly int _Brightness = Shader.PropertyToID("_Brightness");
        private static readonly int _Contrast = Shader.PropertyToID("_Contrast");
        private static readonly int _AO = Shader.PropertyToID("_AO");
        private static readonly int _UVTex = Shader.PropertyToID("_UVTex");
        private static readonly int _Details = Shader.PropertyToID("_Details");
        private static readonly int _Strength = Shader.PropertyToID("_Strength");
        private static readonly int _OriginMap = Shader.PropertyToID("_OriginMap");
        private static readonly int _DirectionMap = Shader.PropertyToID("_DirectionMap");
        private static readonly int _Intensity = Shader.PropertyToID("_Intensity");
        private static readonly int _Bias = Shader.PropertyToID("_Bias");
        private static readonly int _SampleRad = Shader.PropertyToID("_SampleRad");
        private static readonly int _Multiplier = Shader.PropertyToID("_Multiplier");
        private static readonly int _MetallicMap = Shader.PropertyToID("_MetallicMap");
        private static readonly int _AoMap = Shader.PropertyToID("_AOMap");

        private static List<string> missingShaders = new List<string>();


        //This function will manage missing shaders by closing the window and logging an error
        private static Shader GetShaderIfExists(string shader)
        {
            Shader s = Shader.Find(shader);
            if (s == null)
            {
                string message = "Shader \"" + shader + "\" does not exist. Please, reimport the Total Baker package.";
                missingShaders.Add(message);
                Debug.LogError(TotalBaker.logPrefix + message);
            }

            return s;
        }

        internal static bool InitShaders()
        {
            shader_IsolateChannels = GetShaderIfExists("Hidden/TB/IsolateChannels");
            shader_Wireframe = GetShaderIfExists("Hidden/TB/Wireframe");
            shader_StandardMetallic = GetShaderIfExists("Standard");
            shader_StandardSpecular = GetShaderIfExists("Standard (Specular setup)");
            shader_Diffuse = GetShaderIfExists("Legacy Shaders/Bumped Diffuse");
            shader_Unlit = GetShaderIfExists("Unlit/Texture");
            shader_UV2WorldPos = GetShaderIfExists("Hidden/TB/UV2WorldPos");
            shader_UV2Normal = GetShaderIfExists("Hidden/TB/UV2Normal");
            shader_World2Tangent = GetShaderIfExists("Hidden/TB/World2Tangent");
            shader_Tangent2World = GetShaderIfExists("Hidden/TB/Tangent2World");
            shader_Checkerboard = GetShaderIfExists("Hidden/TB/Checkerboard");
            shader_PackNormals = GetShaderIfExists("Hidden/TB/PackNormals");
            shader_AddBackground = GetShaderIfExists("Hidden/TB/AddBackground");
//            shader_Linear2Gamma = GetShaderIfExists("Hidden/TB/Linear2Gamma");
            shader_InvertChannels = GetShaderIfExists("Hidden/TB/InvertChannels");
            shader_GaussianBlur = GetShaderIfExists("Hidden/TB/Blur");
            shader_LinearBlur = GetShaderIfExists("Hidden/TB/LinearBlur");
            shader_BilateralBlur = GetShaderIfExists("Hidden/TB/BilateralBlur");
            shader_OverwriteAlpha = GetShaderIfExists("Hidden/TB/OverwriteAlpha");
            shader_Normal2Curvature = GetShaderIfExists("Hidden/TB/Normal2Curvature");
            shader_CurvatureGrayscale2Dual = GetShaderIfExists("Hidden/TB/CurvatureGrayscale2Dual");
            shader_Dilate = GetShaderIfExists("Hidden/TB/Dilate");
            shader_Erode = GetShaderIfExists("Hidden/TB/Erode");
            shader_Contrast = GetShaderIfExists("Hidden/TB/Contrast");
            shader_Remap = GetShaderIfExists("Hidden/TB/Remap");
            shader_Overlay = GetShaderIfExists("Hidden/TB/Overlay");
            shader_OverlayAO = GetShaderIfExists("Hidden/TB/OverlayAO");
            shader_CombineNormals = GetShaderIfExists("Hidden/TB/CombineNormals");
            shader_Bump = GetShaderIfExists("Hidden/TB/Bump");
            shader_AOMap = GetShaderIfExists("Hidden/TB/AOMap");
            shader_UnpackNormals = GetShaderIfExists("Hidden/TB/UnpackNormals");
            shader_MetallicAndAO2Mask = GetShaderIfExists("Hidden/TB/MetallicAndAO2Mask");
            shader_AddLights = GetShaderIfExists("Hidden/TB/AddLights");

            return missingShaders.Count == 0;
        }
        
        internal static void InitMaterials()
        {
            mat_isolateChannels = new Material(shader_IsolateChannels);
            mat_PackNormals = new Material(shader_PackNormals);
            mat_AddBackground = new Material(shader_AddBackground);
//            mat_Linear2Gamma = new Material(shader_Linear2Gamma);
            mat_InvertChannels = new Material(shader_InvertChannels);
            mat_GaussianBlur = new Material(shader_GaussianBlur);
            mat_LinearBlur = new Material(shader_LinearBlur);
            mat_BilateralBlur = new Material(shader_BilateralBlur);
            mat_OverwriteAlpha = new Material(shader_OverwriteAlpha);
            mat_Normal2Curvature = new Material(shader_Normal2Curvature);
            mat_CurvatureGrayscale2Dual = new Material(shader_CurvatureGrayscale2Dual);
            mat_Dilate = new Material(shader_Dilate);
            mat_Erode = new Material(shader_Erode);
            mat_Contrast = new Material(shader_Contrast);
            mat_OverlayAO = new Material(shader_OverlayAO);
            mat_Remap = new Material(shader_Remap);
            mat_CombineNormals = new Material(shader_CombineNormals);
            mat_Bump = new Material(shader_Bump);
            mat_AOMap = new Material(shader_AOMap);
            mat_UnpackNormals = new Material(shader_UnpackNormals);
            mat_MetallicAndAO2Mask = new Material(shader_MetallicAndAO2Mask);
        }
        
        internal static bool Init()
        {
            bool shadersInitSuccessful = InitShaders();
            
            if (shadersInitSuccessful == false)
            {
                return true;
            }

            InitMaterials();

            return false;
        }

        


        public static void IsolateChannels(RenderTexture source, int mode)
        {
            if (source == null)
            {
                return;
            }

            if (mat_isolateChannels == null)
            { //this should happen only when recompiling while the window is still open
                mat_isolateChannels = new Material(shader_IsolateChannels);
            }

            switch (mode)
            {
                case 0: //rgba
                    mat_isolateChannels.EnableKeyword("_MODE_RGBA");
                    mat_isolateChannels.DisableKeyword("_MODE_RGB");
                    mat_isolateChannels.DisableKeyword("_MODE_R");
                    mat_isolateChannels.DisableKeyword("_MODE_G");
                    mat_isolateChannels.DisableKeyword("_MODE_B");
                    mat_isolateChannels.DisableKeyword("_MODE_A");
                    break;
                case 1: //rgb
                    mat_isolateChannels.DisableKeyword("_MODE_RGBA");
                    mat_isolateChannels.EnableKeyword("_MODE_RGB");
                    mat_isolateChannels.DisableKeyword("_MODE_R");
                    mat_isolateChannels.DisableKeyword("_MODE_G");
                    mat_isolateChannels.DisableKeyword("_MODE_B");
                    mat_isolateChannels.DisableKeyword("_MODE_A");
                    break;
                case 2: //r
                    mat_isolateChannels.DisableKeyword("_MODE_RGBA");
                    mat_isolateChannels.DisableKeyword("_MODE_RGB");
                    mat_isolateChannels.EnableKeyword("_MODE_R");
                    mat_isolateChannels.DisableKeyword("_MODE_G");
                    mat_isolateChannels.DisableKeyword("_MODE_B");
                    mat_isolateChannels.DisableKeyword("_MODE_A");
                    break;
                case 3: //g
                    mat_isolateChannels.DisableKeyword("_MODE_RGBA");
                    mat_isolateChannels.DisableKeyword("_MODE_RGB");
                    mat_isolateChannels.DisableKeyword("_MODE_R");
                    mat_isolateChannels.EnableKeyword("_MODE_G");
                    mat_isolateChannels.DisableKeyword("_MODE_B");
                    mat_isolateChannels.DisableKeyword("_MODE_A");
                    break;
                case 4: //b
                    mat_isolateChannels.DisableKeyword("_MODE_RGBA");
                    mat_isolateChannels.DisableKeyword("_MODE_RGB");
                    mat_isolateChannels.DisableKeyword("_MODE_R");
                    mat_isolateChannels.DisableKeyword("_MODE_G");
                    mat_isolateChannels.EnableKeyword("_MODE_B");
                    mat_isolateChannels.DisableKeyword("_MODE_A");
                    break;
                case 5: //a
                    mat_isolateChannels.DisableKeyword("_MODE_RGBA");
                    mat_isolateChannels.DisableKeyword("_MODE_RGB");
                    mat_isolateChannels.DisableKeyword("_MODE_R");
                    mat_isolateChannels.DisableKeyword("_MODE_G");
                    mat_isolateChannels.DisableKeyword("_MODE_B");
                    mat_isolateChannels.EnableKeyword("_MODE_A");
                    break;
            }

            RenderTexture blit = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGBFloat);
            Graphics.Blit(source, blit, mat_isolateChannels);
            Graphics.Blit(blit, source);
            RenderTexture.ReleaseTemporary(blit);
        }

        public static Color Unpack(Color packednormal)
        {
            Color normal;
            normal.r = (packednormal.a * 2f - 1f) * 0.5f + 0.5f;
            normal.g = (packednormal.g * 2f - 1f) * 0.5f + 0.5f;
            normal.b = Mathf.Clamp01(packednormal.r * packednormal.r + packednormal.g * packednormal.g) * 0.5f + 0.5f;
            normal.a = 1;
            return normal;
        }

        public static void Pack(RenderTexture source, RenderTexture dest)
        {
            Graphics.Blit(source, dest, mat_PackNormals);
        }

        public static void ApplyBackground(RenderTexture source, Color background)
        {
            if (shader_AddBackground == null || source == null)
            {
                return;
            }

            if (mat_AddBackground == null)
            { //this should happen only when recompiling while the window is still open
                mat_AddBackground = new Material(shader_AddBackground);
            }

            mat_AddBackground.SetTexture(_Texture, source);
            mat_AddBackground.SetColor(_Background, background);
            RenderTexture blit = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGBFloat);
            Graphics.Blit(source, blit, mat_AddBackground);
            Graphics.Blit(blit, source);
            RenderTexture.ReleaseTemporary(blit);
        }

        public static void Copy(RenderTexture source, RenderTexture dest)
        {
            Graphics.Blit(source, dest);
        }

        public static void InvertChannels(RenderTexture source, bool r, bool g, bool b)
        {
            if (shader_InvertChannels == null)
            {
                return;
            }

            if (mat_InvertChannels == null)
            { //this should happen only when recompiling while the window is still open
                mat_InvertChannels = new Material(shader_InvertChannels);
            }

            mat_InvertChannels.SetInt(_R, r ? 1 : 0);
            mat_InvertChannels.SetInt(_G, g ? 1 : 0);
            mat_InvertChannels.SetInt(_B, b ? 1 : 0);
            RenderTexture blit = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGBFloat);
            Graphics.Blit(source, blit, mat_InvertChannels);
            Graphics.Blit(blit, source);
            RenderTexture.ReleaseTemporary(blit);
        }

        public static void BilateralBlur(RenderTexture source, float intensity)
        {
            if (intensity <= 0 || source == null)
            {
                return;
            }

            if (shader_BilateralBlur == null)
            {
                return;
            }

            if (mat_BilateralBlur == null)
            { //this should happen only when recompiling while the window is still open
                mat_BilateralBlur = new Material(shader_BilateralBlur);
            }

            mat_BilateralBlur.SetTexture(_MainTex, source);
            mat_BilateralBlur.SetFloat(_SigmaP, intensity);
            mat_BilateralBlur.SetFloat(_SigmaR, 0.1f);
            RenderTexture blit = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGBFloat);
            Graphics.Blit(source, blit, mat_BilateralBlur);
            Graphics.Blit(blit, source);
            RenderTexture.ReleaseTemporary(blit);
        }

        public static void GaussianBlur(RenderTexture source, int iterations)
        {
            if (iterations <= 0)
            {
                return;
            }

            if (shader_GaussianBlur == null)
            {
                return;
            }

            if (mat_GaussianBlur == null)
            { //this should happen only when recompiling while the window is still open
                mat_GaussianBlur = new Material(shader_GaussianBlur);
            }

            RenderTexture blit = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGBFloat);
            for (int i = 0; i < iterations; i++)
            {
                Graphics.Blit(source, blit, mat_GaussianBlur);
                Graphics.Blit(blit, source);
            }

            RenderTexture.ReleaseTemporary(blit);
        }

        public static void LinearBlur(RenderTexture source, int iterations, float offset)
        {
            if (iterations <= 0)
            {
                return;
            }

            if (shader_LinearBlur == null)
            {
                return;
            }

            if (mat_LinearBlur == null)
            { //this should happen only when recompiling while the window is still open
                mat_LinearBlur = new Material(shader_LinearBlur);
            }

            mat_LinearBlur.SetFloat(_Offset, offset);
            RenderTexture blit = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGBFloat);
            for (int i = 0; i < iterations; i++)
            {
                Graphics.Blit(source, blit, mat_LinearBlur);
                Graphics.Blit(blit, source);
            }

            RenderTexture.ReleaseTemporary(blit);
        }

        public static void Dilate(RenderTexture source, int steps, Settings.DilationMode mode)
        {
            if (source == null)
            {
                return;
            }

            if (shader_Dilate == null)
            {
                return;
            }

            if (mat_Dilate == null)
            { //this should happen only when recompiling while the window is still open
                mat_Dilate = new Material(shader_Dilate);
            }

            //create alpha mask from original texture
            RenderTexture alphaMask = null;
            if (mode == Settings.DilationMode.AlphaMask)
            {
                alphaMask = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGBFloat);
                Graphics.Blit(source, alphaMask);
            }

            //setup mode keyword
            mat_Dilate.DisableKeyword("_MODE_TRANSPARENT");
            mat_Dilate.DisableKeyword("_MODE_OPAQUE");
            mat_Dilate.EnableKeyword("_MODE_ALPHAMASK");

            RenderTexture dilated = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGBFloat);

            Graphics.Blit(source, dilated);

            for (int i = 0; i < steps; i++)
            {
                RenderTexture blit = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGBFloat);
                Graphics.Blit(dilated, blit, mat_Dilate);
                Graphics.Blit(blit, dilated);
                RenderTexture.ReleaseTemporary(blit);
            }

            Graphics.Blit(dilated, source);
            RenderTexture.ReleaseTemporary(dilated);

            if (mode == Settings.DilationMode.AlphaMask)
            {
                //apply original alpha mask, so that dilation only affects RGB
                OverwriteAlpha(source, alphaMask);
                RenderTexture.ReleaseTemporary(alphaMask);
            }
        }

        public static void OverwriteAlpha(RenderTexture source, Texture alphaMask)
        {
            if (mat_OverwriteAlpha == null)
            { //this should happen only when recompiling while the window is still open
                mat_OverwriteAlpha = new Material(shader_OverwriteAlpha);
            }

            mat_OverwriteAlpha.SetTexture(_AlphaMask, alphaMask);
            RenderTexture blit = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGBFloat);
            Graphics.Blit(source, blit, mat_OverwriteAlpha);
            Graphics.Blit(blit, source);

            RenderTexture.ReleaseTemporary(blit);
        }

        public static void Normal2Curvature(RenderTexture source, Settings.CurvatureChannels channelsMode, float multiplier, int smoothness)
        {
            if (mat_Normal2Curvature == null)
            { //this should happen only when recompiling while the window is still open
                mat_Normal2Curvature = new Material(shader_Normal2Curvature);
            }

            if (mat_CurvatureGrayscale2Dual == null)
            { //this should happen only when recompiling while the window is still open
                mat_CurvatureGrayscale2Dual = new Material(shader_CurvatureGrayscale2Dual);
            }

            mat_Normal2Curvature.SetTexture(_MainTex, source);
            mat_Normal2Curvature.SetFloat(_Multiplier, multiplier);
            RenderTexture blit = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGBFloat);
            Graphics.Blit(source, blit, mat_Normal2Curvature);
            Graphics.Blit(blit, source);

            //smooth
            for (int i = 0; i < smoothness; i++)
            {
                int pow = (int) Mathf.Pow(2, i + 1);
                GaussianBlur(blit, pow);
                Overlay(source, blit, 1f);
            }

            if (channelsMode == Settings.CurvatureChannels.Dual)
            {
                Graphics.Blit(source, blit, mat_CurvatureGrayscale2Dual);
                Graphics.Blit(blit, source);
            }

            RenderTexture.ReleaseTemporary(blit);
        }

        public static void Overlay(RenderTexture source, Texture overlayTex, float factor)
        {
            if (shader_Overlay == null)
            {
                return;
            }

            if (mat_Overlay == null)
            { //this should happen only when recompiling while the window is still open
                mat_Overlay = new Material(shader_Overlay);
            }

            mat_Overlay.SetTexture(_MainTex, source);
            mat_Overlay.SetTexture(_OverlayTex, overlayTex);
            mat_Overlay.SetFloat(_Factor, factor);
            RenderTexture blit = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGBFloat);
            Graphics.Blit(source, blit, mat_Overlay);
            Graphics.Blit(blit, source);
            RenderTexture.ReleaseTemporary(blit);
        }

        public static void Erode(RenderTexture source, int steps, Color background)
        {
            if (shader_Erode == null)
            {
                return;
            }

            if (mat_Erode == null)
            { //this should happen only when recompiling while the window is still open
                mat_Erode = new Material(shader_Erode);
            }

            mat_Erode.SetFloat(_MaxSteps, steps);
            mat_Erode.SetColor(_Background, background);
            RenderTexture blit = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGBFloat);
            Graphics.Blit(source, blit, mat_Erode);
            Graphics.Blit(blit, source);
            RenderTexture.ReleaseTemporary(blit);
        }

        public static void Contrast(RenderTexture source, float brightness, float contrast)
        {
            if (shader_Contrast == null)
            {
                return;
            }

            if (mat_Contrast == null)
            { //this should happen only when recompiling while the window is still open
                mat_Contrast = new Material(shader_Contrast);
            }

            mat_Contrast.SetFloat(_Brightness, brightness);
            mat_Contrast.SetFloat(_Contrast, contrast);
            RenderTexture blit = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGBFloat);
            Graphics.Blit(source, blit, mat_Contrast);
            Graphics.Blit(blit, source);
            RenderTexture.ReleaseTemporary(blit);
        }

        public static void OverlayAO(RenderTexture source, RenderTexture ao, RenderTexture dest)
        {
            if (shader_OverlayAO == null)
            {
                return;
            }

            if (mat_OverlayAO == null)
            { //this should happen only when recompiling while the window is still open
                mat_OverlayAO = new Material(shader_OverlayAO);
            }

            mat_OverlayAO.SetTexture(_AO, ao);
            Graphics.Blit(source, dest, mat_OverlayAO);
        }

        public static void RemapTexture(RenderTexture source, RenderTexture dest, Texture2D uvs)
        {
            if (shader_Remap == null)
            {
                return;
            }

            if (mat_Remap == null)
            { //this should happen only when recompiling while the window is still open
                mat_Remap = new Material(shader_Remap);
            }

            mat_Remap.SetTexture(_UVTex, uvs);
            Graphics.Blit(source, dest, mat_Remap);
        }

        public static void CombineNormals(RenderTexture main, Texture details, RenderTexture dest)
        {
            if (shader_CombineNormals == null)
            {
                return;
            }

            if (mat_CombineNormals == null)
            { //this should happen only when recompiling while the window is still open
                mat_CombineNormals = new Material(shader_CombineNormals);
            }

            if (details == null)
            {
                Graphics.Blit(main, dest);
                return;
            }

            mat_CombineNormals.SetTexture(_Details, details);
            Graphics.Blit(main, dest, mat_CombineNormals);
        }

        public static void Grayscale2Normal(RenderTexture source, RenderTexture dest, float strength)
        {
            if (shader_Bump == null)
            {
                return;
            }

            if (mat_Bump == null)
            { //this should happen only when recompiling while the window is still open
                mat_Bump = new Material(shader_Bump);
            }

            mat_Bump.SetFloat(_Strength, strength);
            Graphics.Blit(source, dest, mat_Bump);
        }

        public static void FastAO(RenderTexture dest, Texture2D originMap, Texture2D directionMap, float strength, float bias, float sampleRad, Color background)
        {
            if (shader_AOMap == null)
            {
                return;
            }

            if (mat_AOMap == null)
            { //this should happen only when recompiling while the window is still open
                mat_AOMap = new Material(shader_AOMap);
            }

            mat_AOMap.SetTexture(_OriginMap, originMap);
            mat_AOMap.SetTexture(_DirectionMap, directionMap);
            mat_AOMap.SetFloat(_Intensity, strength);
            mat_AOMap.SetFloat(_Bias, bias);
            mat_AOMap.SetFloat(_SampleRad, sampleRad);
            mat_AOMap.SetColor(_Background, background);
            Graphics.Blit(null, dest, mat_AOMap);
        }

        public static void Unpack(RenderTexture source, int res)
        {
            if (shader_UnpackNormals == null)
            {
                return;
            }

            if (mat_UnpackNormals == null)
            { //this should happen only when recompiling while the window is still open
                mat_UnpackNormals = new Material(shader_UnpackNormals);
            }

            RenderTexture blit = RenderTexture.GetTemporary(res, res, 0, RenderTextureFormat.ARGBFloat);
            Graphics.Blit(source, blit, mat_UnpackNormals);
            Graphics.Blit(blit, source);
            RenderTexture.ReleaseTemporary(blit);
        }
        
        public static void MetallicAndAO2Mask(RenderTexture dest, RenderTexture metallicMap, RenderTexture ambientOcclusionMap)
        {
            if (shader_MetallicAndAO2Mask == null)
            {
                return;
            }

            if (mat_MetallicAndAO2Mask == null)
            { //this should happen only when recompiling while the window is still open
                mat_MetallicAndAO2Mask = new Material(shader_MetallicAndAO2Mask);
            }
            
            mat_MetallicAndAO2Mask.SetTexture(_MetallicMap, metallicMap);
            mat_MetallicAndAO2Mask.SetTexture(_AoMap, ambientOcclusionMap);
            Graphics.Blit(null, dest, mat_MetallicAndAO2Mask);
        }

        internal static void Destroy()
        {
            // if (mat_Linear2Gamma != null) Object.DestroyImmediate(mat_Linear2Gamma);
            if (mat_InvertChannels != null) Object.DestroyImmediate(mat_InvertChannels);
            if (mat_GaussianBlur != null) Object.DestroyImmediate(mat_GaussianBlur);
            if (mat_LinearBlur != null) Object.DestroyImmediate(mat_LinearBlur);
            if (mat_OverwriteAlpha != null) Object.DestroyImmediate(mat_OverwriteAlpha);
            if (mat_Normal2Curvature != null) Object.DestroyImmediate(mat_Normal2Curvature);
            if (mat_CurvatureGrayscale2Dual != null) Object.DestroyImmediate(mat_CurvatureGrayscale2Dual);
            if (mat_Dilate != null) Object.DestroyImmediate(mat_Dilate);
            if (mat_Erode != null) Object.DestroyImmediate(mat_Erode);
            if (mat_Contrast != null) Object.DestroyImmediate(mat_Contrast);
            if (mat_Overlay != null) Object.DestroyImmediate(mat_Overlay);
            if (mat_OverlayAO != null) Object.DestroyImmediate(mat_OverlayAO);
            if (mat_Remap != null) Object.DestroyImmediate(mat_Remap);
            if (mat_CombineNormals != null) Object.DestroyImmediate(mat_CombineNormals);
            if (mat_Bump != null) Object.DestroyImmediate(mat_Bump);
            if (mat_AOMap != null) Object.DestroyImmediate(mat_AOMap);
            if (mat_UnpackNormals != null) Object.DestroyImmediate(mat_UnpackNormals);
        }
    }

}

#endif