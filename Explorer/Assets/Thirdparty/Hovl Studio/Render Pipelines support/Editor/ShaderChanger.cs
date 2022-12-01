using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class RPChanger : EditorWindow
{

    [MenuItem("Tools/RP changer for Hovl Studio assets")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(RPChanger));
    }

    public void OnGUI()
    {
        GUILayout.Label("Change VFX pipeline to:");

        if (GUILayout.Button("Standard RP"))
        {
            FindShaders();
            ChangeToSRP();
        }
        if (GUILayout.Button("Lightweight RP"))
        {
            FindShaders();
            ChangeToLWRP();
        }
        if (GUILayout.Button("HD RP (From Unity 2018.3+)"))
        {
            FindShaders();
            ChangeToHDRP();
        }
    }

    Shader LightGlow;
    Shader Lit_CenterGlow;
    Shader Blend_TwoSides;
    Shader Blend_Normals;
    Shader Ice;
    Shader Distortion;
    Shader ParallaxIce;

    Shader LightGlow_LWRP;
    Shader Lit_CenterGlow_LWRP;
    Shader Blend_TwoSides_LWRP;
    Shader Blend_Normals_LWRP;
    Shader Ice_LWRP;
    Shader Distortion_LWRP;
    Shader ParallaxIce_LWRP;

    Shader LightGlow_HDRP;
    Shader Lit_CenterGlow_HDRP;
    Shader Blend_TwoSides_HDRP;
    Shader Blend_Normals_HDRP;
    Shader Ice_HDRP;
    Shader Distortion_HDRP;
    Shader ParallaxIce_HDRP;

    Material[] shaderMaterials;

    private void FindShaders()
    {
        if (Shader.Find("ERB/Particles/LightGlow") != null) LightGlow = Shader.Find("ERB/Particles/LightGlow");
        if (Shader.Find("ERB/Particles/Lit_CenterGlow") != null) Lit_CenterGlow = Shader.Find("ERB/Particles/Lit_CenterGlow");
        if (Shader.Find("ERB/Particles/Blend_TwoSides") != null) Blend_TwoSides = Shader.Find("ERB/Particles/Blend_TwoSides");
        if (Shader.Find("ERB/Particles/Blend_Normals") != null) Blend_Normals = Shader.Find("ERB/Particles/Blend_Normals");
        if (Shader.Find("ERB/Particles/Ice") != null) Ice = Shader.Find("ERB/Particles/Ice");
        if (Shader.Find("ERB/Particles/Distortion") != null) Distortion = Shader.Find("ERB/Particles/Distortion");
        if (Shader.Find("ERB/Opaque/ParallaxIce") != null) ParallaxIce = Shader.Find("ERB/Opaque/ParallaxIce");

        if (Shader.Find("ERB/LWRP/Particles/LightGlow") != null) LightGlow_LWRP = Shader.Find("ERB/LWRP/Particles/LightGlow");
        if (Shader.Find("ERB/LWRP/Particles/Lit_CenterGlow") != null) Lit_CenterGlow_LWRP = Shader.Find("ERB/LWRP/Particles/Lit_CenterGlow");
        else { if (Shader.Find("Shader Graphs/LWRP_Lit_CenterGlow") != null) Lit_CenterGlow_LWRP = Shader.Find("Shader Graphs/LWRP_Lit_CenterGlow"); }
        if (Shader.Find("ERB/LWRP/Particles/Blend_TwoSides") != null) Blend_TwoSides_LWRP = Shader.Find("ERB/LWRP/Particles/Blend_TwoSides");
        else { if (Shader.Find("Shader Graphs/LWRP_Blend_TwoSides") != null) Blend_TwoSides_LWRP = Shader.Find("Shader Graphs/LWRP_Blend_TwoSides"); }
        if (Shader.Find("ERB/LWRP/Particles/Blend_Normals") != null) Blend_Normals_LWRP = Shader.Find("ERB/LWRP/Particles/Blend_Normals");
        else { if (Shader.Find("Shader Graphs/LWRP_Blend_Normals") != null) Blend_Normals_LWRP = Shader.Find("Shader Graphs/LWRP_Blend_Normals"); }
        if (Shader.Find("ERB/LWRP/Particles/Ice") != null) Ice_LWRP = Shader.Find("ERB/LWRP/Particles/Ice");
        else { if (Shader.Find("Shader Graphs/LWRP_Ice") != null) Ice_LWRP = Shader.Find("Shader Graphs/LWRP_Ice"); }
        if (Shader.Find("Shader Graphs/LWRP_Distortion") != null) Distortion_LWRP = Shader.Find("Shader Graphs/LWRP_Distortion");
        if (Shader.Find("Shader Graphs/LWRP_ParallaxIce") != null) ParallaxIce_LWRP = Shader.Find("Shader Graphs/LWRP_ParallaxIce");

        if (Shader.Find("ERB/HDRP/Particles/LightGlow") != null) LightGlow_HDRP = Shader.Find("ERB/HDRP/Particles/LightGlow");
        if (Shader.Find("Shader Graphs/HDRP_Lit_CenterGlow") != null) Lit_CenterGlow_HDRP = Shader.Find("Shader Graphs/HDRP_Lit_CenterGlow");
        else { if (Shader.Find("Shader Graphs/HDRP_Lit_CenterGlow") != null) Lit_CenterGlow_HDRP = Shader.Find("Shader Graphs/HDRP_Lit_CenterGlow"); }
        if (Shader.Find("ERB/HDRP/Particles/Blend_TwoSides") != null) Blend_TwoSides_HDRP = Shader.Find("ERB/HDRP/Particles/Blend_TwoSides");
        else { if (Shader.Find("Shader Graphs/HDRP_Blend_TwoSides") != null) Blend_TwoSides_HDRP = Shader.Find("Shader Graphs/HDRP_Blend_TwoSides"); }
        if (Shader.Find("ERB/HDRP/Particles/Blend_Normals") != null) Blend_Normals_HDRP = Shader.Find("ERB/HDRP/Particles/Blend_Normals");
        else { if (Shader.Find("Shader Graphs/HDRP_Blend_Normals") != null) Blend_Normals_HDRP = Shader.Find("Shader Graphs/HDRP_Blend_Normals"); }
        if (Shader.Find("ERB/HDRP/Particles/Ice") != null) Ice_HDRP = Shader.Find("ERB/HDRP/Particles/Ice");
        else { if (Shader.Find("Shader Graphs/HDRP_Ice") != null) Ice_HDRP = Shader.Find("Shader Graphs/HDRP_Ice"); }
        if (Shader.Find("Shader Graphs/HDRP_Distortion") != null) Distortion_HDRP = Shader.Find("Shader Graphs/HDRP_Distortion");
        if (Shader.Find("Shader Graphs/HDRP_ParallaxIce") != null) ParallaxIce_HDRP = Shader.Find("Shader Graphs/HDRP_ParallaxIce");

        string[] folderMat = AssetDatabase.FindAssets("t:Material", new[] { "Assets" });
        shaderMaterials = new Material[folderMat.Length];

        for (int i = 0; i < folderMat.Length; i++)
        {
            var patch = AssetDatabase.GUIDToAssetPath(folderMat[i]);
            shaderMaterials[i] = (Material)AssetDatabase.LoadAssetAtPath(patch, typeof(Material));
        }
    }

    private void ChangeToLWRP()
    {

        foreach (var material in shaderMaterials)
        {
            if (Shader.Find("ERB/LWRP/Particles/LightGlow") != null)
            {
                if (material.shader == LightGlow || material.shader == LightGlow_HDRP)
                {
                    material.shader = LightGlow_LWRP;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("ERB/LWRP/Particles/Lit_CenterGlow") != null || Shader.Find("Shader Graphs/LWRP_Lit_CenterGlow") != null)
            {
                if (material.shader == Lit_CenterGlow || material.shader == Lit_CenterGlow_HDRP)
                {
                    material.shader = Lit_CenterGlow_LWRP;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("ERB/LWRP/Particles/Blend_TwoSides") != null || Shader.Find("Shader Graphs/LWRP_Blend_TwoSides") != null)
            {
                if (material.shader == Blend_TwoSides || material.shader == Blend_TwoSides_HDRP)
                {
                    if (material.GetTextureScale("_MainTex") != null || material.GetTextureScale("_Noise") != null)
                    {
                        Vector2 MainScale = material.GetTextureScale("_MainTex");
                        Vector2 MainOffset = material.GetTextureOffset("_MainTex");
                        Vector2 NoiseScale = material.GetTextureScale("_Noise");
                        Vector2 NoiseOffset = material.GetTextureOffset("_Noise");
                        material.shader = Blend_TwoSides_LWRP;
                        if (material.GetVector("_MainTexTiling") != null)
                            material.SetVector("_MainTexTiling", new Vector4(MainScale[0], MainScale[1], MainOffset[0], MainOffset[1]));
                        if (material.GetVector("_NoiseTiling") != null)
                            material.SetVector("_NoiseTiling", new Vector4(NoiseScale[0], NoiseScale[1], NoiseOffset[0], NoiseOffset[1]));
                    }
                    else
                        material.shader = Blend_TwoSides_LWRP;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("ERB/LWRP/Particles/Blend_Normals") != null || Shader.Find("Shader Graphs/LWRP_Blend_Normals") != null)
            {
                if (material.shader == Blend_Normals || material.shader == Blend_Normals_HDRP)
                {
                    if (material.GetTextureScale("_MainTex") != null || material.GetTextureScale("_Noise") != null)
                    {
                        Vector2 MainScale = material.GetTextureScale("_MainTex");
                        Vector2 MainOffset = material.GetTextureOffset("_MainTex");
                        Vector2 NoiseScale = material.GetTextureScale("_Noise");
                        Vector2 NoiseOffset = material.GetTextureOffset("_Noise");
                        material.shader = Blend_Normals_LWRP;
                        if (material.GetVector("_MainTexTiling") != null)
                            material.SetVector("_MainTexTiling", new Vector4(MainScale[0], MainScale[1], MainOffset[0], MainOffset[1]));
                        if (material.GetVector("_NoiseTiling") != null)
                            material.SetVector("_NoiseTiling", new Vector4(NoiseScale[0], NoiseScale[1], NoiseOffset[0], NoiseOffset[1]));
                    }
                    else
                        material.shader = Blend_Normals_LWRP;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("ERB/LWRP/Particles/Ice") != null || Shader.Find("Shader Graphs/LWRP_Ice") != null)
            {
                if (material.shader == Ice || material.shader == Ice_HDRP)
                {
                    if (material.GetTextureScale("_MainTex") != null)
                    {
                        Vector2 MainScale = material.GetTextureScale("_MainTex");
                        Vector2 MainOffset = material.GetTextureOffset("_MainTex");
                        material.shader = Ice_LWRP;
                        if (material.GetVector("_Tiling") != null)
                            material.SetVector("_Tiling", new Vector4(MainScale[0], MainScale[1], MainOffset[0], MainOffset[1]));
                    }
                    else
                        material.shader = Ice_LWRP;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("Shader Graphs/LWRP_ParallaxIce") != null)
            {
                if (material.shader == ParallaxIce || material.shader == ParallaxIce_HDRP)
                {
                    if (material.GetTextureScale("_Emission") != null)
                    {
                        Vector2 MainScale = material.GetTextureScale("_Emission");
                        Vector2 MainOffset = material.GetTextureOffset("_Emission");
                        material.shader = ParallaxIce_LWRP;
                        if (material.GetVector("_EmissionTiling") != null)
                            material.SetVector("_EmissionTiling", new Vector4(MainScale[0], MainScale[1], MainOffset[0], MainOffset[1]));
                    }
                    else
                        material.shader = ParallaxIce_LWRP;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("Shader Graphs/LWRP_Distortion") != null)
            {
                if (material.shader == Distortion || material.shader == Distortion_HDRP)
                {
                    material.shader = Distortion_LWRP;
                }
            }
        }
    }


    private void ChangeToSRP()
    {

        foreach (var material in shaderMaterials)
        {
            if (Shader.Find("ERB/Particles/LightGlow") != null)
            {
                if (material.shader == LightGlow_LWRP || material.shader == LightGlow_HDRP)
                {
                    material.shader = LightGlow;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("ERB/Particles/Lit_CenterGlow") != null)
            {
                if (material.shader == Lit_CenterGlow_LWRP || material.shader == Lit_CenterGlow_HDRP)
                {
                    material.shader = Lit_CenterGlow;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("ERB/Particles/Blend_TwoSides") != null)
            {
                if (material.shader == Blend_TwoSides_LWRP || material.shader == Blend_TwoSides_HDRP)
                {
                    if (material.GetVector("_MainTexTiling") != null && material.GetVector("_NoiseTiling") != null)
                    {
                        Vector4 MainTiling = material.GetVector("_MainTexTiling");
                        Vector4 NoiseTiling = material.GetVector("_NoiseTiling");
                        material.shader = Blend_TwoSides;
                        if (material.GetTextureScale("_MainTex") != null && material.GetTextureScale("_Noise") != null)
                        {
                            material.SetTextureScale("_MainTex", new Vector2(MainTiling[0], MainTiling[1]));
                            material.SetTextureOffset("_MainTex", new Vector2(MainTiling[2], MainTiling[3]));
                            material.SetTextureScale("_Noise", new Vector2(NoiseTiling[0], NoiseTiling[1]));
                            material.SetTextureOffset("_Noise", new Vector2(NoiseTiling[2], NoiseTiling[3]));
                        }
                    }
                    else
                        material.shader = Blend_TwoSides;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("ERB/Particles/Blend_Normals") != null)
            {
                if (material.shader == Blend_Normals_LWRP || material.shader == Blend_Normals_HDRP)
                {
                    if (material.GetVector("_MainTexTiling") != null && material.GetVector("_NoiseTiling") != null)
                    {
                        Vector4 MainTiling = material.GetVector("_MainTexTiling");
                        Vector4 NoiseTiling = material.GetVector("_NoiseTiling");
                        material.shader = Blend_Normals;
                        if (material.GetTextureScale("_MainTex") != null && material.GetTextureScale("_Noise") != null)
                        {
                            material.SetTextureScale("_MainTex", new Vector2(MainTiling[0], MainTiling[1]));
                            material.SetTextureOffset("_MainTex", new Vector2(MainTiling[2], MainTiling[3]));
                            material.SetTextureScale("_Noise", new Vector2(NoiseTiling[0], NoiseTiling[1]));
                            material.SetTextureOffset("_Noise", new Vector2(NoiseTiling[2], NoiseTiling[3]));
                        }
                    }
                    else
                        material.shader = Blend_Normals;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("ERB/Particles/Ice") != null)
            {
                if (material.shader == Ice_LWRP || material.shader == Ice_HDRP)
                {
                    if (material.GetVector("_Tiling") != null)
                    {
                        Vector4 MainTiling = material.GetVector("_Tiling");
                        material.shader = Ice;
                        if (material.GetTextureScale("_MainTex") != null)
                        {
                            material.SetTextureScale("_MainTex", new Vector2(MainTiling[0], MainTiling[1]));
                            material.SetTextureOffset("_MainTex", new Vector2(MainTiling[2], MainTiling[3]));
                        }
                    }
                    else
                        material.shader = Ice;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("ERB/Opaque/ParallaxIce") != null)
            {
                if (material.shader == ParallaxIce_LWRP || material.shader == ParallaxIce_HDRP)
                {
                    if (material.GetVector("_EmissionTiling") != null)
                    {
                        Vector4 MainTiling = material.GetVector("_EmissionTiling");
                        material.shader = ParallaxIce;
                        if (material.GetTextureScale("_Emission") != null)
                        {
                            material.SetTextureScale("_Emission", new Vector2(MainTiling[0], MainTiling[1]));
                            material.SetTextureOffset("_Emission", new Vector2(MainTiling[2], MainTiling[3]));
                        }
                    }
                    else
                        material.shader = ParallaxIce;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("ERB/Particles/Distortion") != null)
            {
                if (material.shader == Distortion_LWRP || material.shader == Distortion_HDRP)
                {
                    material.shader = Distortion;
                }
            }
        }
    }

    private void ChangeToHDRP()
    {
        foreach (var material in shaderMaterials)
        {
            if (Shader.Find("ERB/HDRP/Particles/LightGlow") != null)
            {
                if (material.shader == LightGlow || material.shader == LightGlow_LWRP)
                {
                    material.shader = LightGlow_HDRP;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("Shader Graphs/HDRP_Lit_CenterGlow") != null)
            {
                if (material.shader == Lit_CenterGlow || material.shader == Lit_CenterGlow_LWRP)
                {
                    material.shader = Lit_CenterGlow_HDRP;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("ERB/HDRP/Particles/Blend_TwoSides") != null || Shader.Find("Shader Graphs/HDRP_Blend_TwoSides") != null)
            {
                if (material.shader == Blend_TwoSides || material.shader == Blend_TwoSides_LWRP)
                {
                    if (material.GetTextureScale("_MainTex") != null || material.GetTextureScale("_Noise") != null)
                    {
                        Vector2 MainScale = material.GetTextureScale("_MainTex");
                        Vector2 MainOffset = material.GetTextureOffset("_MainTex");
                        Vector2 NoiseScale = material.GetTextureScale("_Noise");
                        Vector2 NoiseOffset = material.GetTextureOffset("_Noise");
                        material.shader = Blend_TwoSides_HDRP;
                        if (material.GetVector("_MainTexTiling") != null)
                            material.SetVector("_MainTexTiling", new Vector4(MainScale[0], MainScale[1], MainOffset[0], MainOffset[1]));
                        if (material.GetVector("_NoiseTiling") != null)
                            material.SetVector("_NoiseTiling", new Vector4(NoiseScale[0], NoiseScale[1], NoiseOffset[0], NoiseOffset[1]));
                    }
                    else
                        material.shader = Blend_TwoSides_HDRP;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("ERB/HDRP/Particles/Blend_Normals") != null || Shader.Find("Shader Graphs/HDRP_Blend_Normals") != null)
            {
                if (material.shader == Blend_Normals || material.shader == Blend_Normals_LWRP)
                {
                    if (material.GetTextureScale("_MainTex") != null || material.GetTextureScale("_Noise") != null)
                    {
                        Vector2 MainScale = material.GetTextureScale("_MainTex");
                        Vector2 MainOffset = material.GetTextureOffset("_MainTex");
                        Vector2 NoiseScale = material.GetTextureScale("_Noise");
                        Vector2 NoiseOffset = material.GetTextureOffset("_Noise");
                        material.shader = Blend_Normals_HDRP;
                        if (material.GetVector("_MainTexTiling") != null)
                            material.SetVector("_MainTexTiling", new Vector4(MainScale[0], MainScale[1], MainOffset[0], MainOffset[1]));
                        if (material.GetVector("_NoiseTiling") != null)
                            material.SetVector("_NoiseTiling", new Vector4(NoiseScale[0], NoiseScale[1], NoiseOffset[0], NoiseOffset[1]));
                    }
                    else
                        material.shader = Blend_Normals_HDRP;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("ERB/HDRP/Particles/Ice") != null || Shader.Find("Shader Graphs/HDRP_Ice") != null)
            {
                if (material.shader == Ice || material.shader == Ice_LWRP)
                {
                    if (material.GetTextureScale("_MainTex") != null)
                    {
                        Vector2 MainScale = material.GetTextureScale("_MainTex");
                        Vector2 MainOffset = material.GetTextureOffset("_MainTex");
                        material.shader = Ice_HDRP;
                        if (material.GetVector("_Tiling") != null)
                            material.SetVector("_Tiling", new Vector4(MainScale[0], MainScale[1], MainOffset[0], MainOffset[1]));
                    }
                    else
                        material.shader = Ice_HDRP;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("ERB/Opaque/ParallaxIce") != null || Shader.Find("Shader Graphs/HDRP_ParallaxIce") != null)
            {
                if (material.shader == ParallaxIce || material.shader == ParallaxIce_LWRP)
                {
                    if (material.GetTextureScale("_Emission") != null)
                    {
                        Vector2 MainScale = material.GetTextureScale("_Emission");
                        Vector2 MainOffset = material.GetTextureOffset("_Emission");
                        material.shader = ParallaxIce_HDRP;
                        if (material.GetVector("_EmissionTiling") != null)
                            material.SetVector("_EmissionTiling", new Vector4(MainScale[0], MainScale[1], MainOffset[0], MainOffset[1]));
                    }
                    else
                        material.shader = ParallaxIce_HDRP;
                }
            }
            /*----------------------------------------------------------------------------------------------------*/
            if (Shader.Find("Shader Graphs/HDRP_Distortion") != null)
            {
                if (material.shader == Distortion || material.shader == Distortion_LWRP)
                {
                    material.shader = Distortion_HDRP;
                }
            }
        }
    }
}