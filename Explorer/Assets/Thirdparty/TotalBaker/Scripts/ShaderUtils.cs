/*====================================================
*
* TOTAL BAKER
*
* Francesco Cucchiara - 3POINT SOFT
*
=====================================================*/

#if UNITY_EDITOR

using UnityEngine;
using UnityEngine.Rendering;

namespace TB
 {
     public static class ShaderUtils
     {
         private static readonly int _Mode = Shader.PropertyToID("_Mode");
         private static readonly int _SrcBlend = Shader.PropertyToID("_SrcBlend");
         private static readonly int _DstBlend = Shader.PropertyToID("_DstBlend");
         private static readonly int _ZWrite = Shader.PropertyToID("_ZWrite");

         public static bool IsStandard(Settings.PreviewMaterialType type)
         {
             return type == Settings.PreviewMaterialType.StandardMetallic || type == Settings.PreviewMaterialType.StandardSpecular;
         }
         public static void SetStandardMaterialRenderMode(Material mat, Settings.PreviewMaterialStandardBlend blendMode)
         {
             mat.SetInt(_Mode, (int)blendMode);
             
             switch (blendMode)
             {
                 case Settings.PreviewMaterialStandardBlend.Opaque:
                     mat.SetInt(_SrcBlend, (int)BlendMode.One);
                     mat.SetInt(_DstBlend, (int)BlendMode.Zero);
                     mat.SetInt(_ZWrite, 1);
                     mat.DisableKeyword("_ALPHATEST_ON");
                     mat.DisableKeyword("_ALPHABLEND_ON");
                     mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                     mat.renderQueue = -1;
                     break;
                 case Settings.PreviewMaterialStandardBlend.Cutout:
                     mat.SetInt(_SrcBlend, (int)BlendMode.One);
                     mat.SetInt(_DstBlend, (int)BlendMode.Zero);
                     mat.SetInt(_ZWrite, 1);
                     mat.EnableKeyword("_ALPHATEST_ON");
                     mat.DisableKeyword("_ALPHABLEND_ON");
                     mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                     mat.renderQueue = 2450;
                     break;
                 case Settings.PreviewMaterialStandardBlend.Fade:
                     mat.SetInt(_SrcBlend, (int)BlendMode.SrcAlpha);
                     mat.SetInt(_DstBlend, (int)BlendMode.OneMinusSrcAlpha);
                     mat.SetInt(_ZWrite, 0);
                     mat.DisableKeyword("_ALPHATEST_ON");
                     mat.EnableKeyword("_ALPHABLEND_ON");
                     mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                     mat.renderQueue = 3000;
                     break;
                 case Settings.PreviewMaterialStandardBlend.Transparent:
                     mat.SetInt(_SrcBlend, (int)BlendMode.One);
                     mat.SetInt(_DstBlend, (int)BlendMode.OneMinusSrcAlpha);
                     mat.SetInt(_ZWrite, 0);
                     mat.DisableKeyword("_ALPHATEST_ON");
                     mat.DisableKeyword("_ALPHABLEND_ON");
                     mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                     mat.renderQueue = 3000;
                     break;
             }
         }

         public static void SetDiffuseRenderMode(Material mat, Settings.PreviewMaterialLegacyBlend blendMode)
         {
             switch (blendMode)
             {
                 case Settings.PreviewMaterialLegacyBlend.Opaque:
                     mat.shader = Shader.Find("Legacy Shaders/Bumped Diffuse");
                     break;
                 case Settings.PreviewMaterialLegacyBlend.Cutout:
                     mat.shader = Shader.Find("Legacy Shaders/Transparent/Cutout/Bumped Diffuse");
                     break;
                 case Settings.PreviewMaterialLegacyBlend.Transparent:
                     mat.shader = Shader.Find("Legacy Shaders/Transparent/Bumped Diffuse");
                     break;
             }
         }
         
         public static void SetUnlitRenderMode(Material mat, Settings.PreviewMaterialLegacyBlend blendMode)
         {
             switch (blendMode)
             {
                 case Settings.PreviewMaterialLegacyBlend.Opaque:
                     mat.shader = Shader.Find("Unlit/Texture");
                     break;
                 case Settings.PreviewMaterialLegacyBlend.Cutout:
                     mat.shader = Shader.Find("Unlit/Transparent Cutout");
                     break;
                 case Settings.PreviewMaterialLegacyBlend.Transparent:
                     mat.shader = Shader.Find("Unlit/Transparent");
                     break;
             }
         }
     }
 }
 
 #endif