/*====================================================
*
* TOTAL BAKER
*
* Francesco Cucchiara - 3POINT SOFT
*
=====================================================*/

#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TB
{
    public enum MapType
    {
        Diffuse,
        Normal,
        Height,
        AO,
        Metallic,
        Specular,
        Emissive,
        Curvature,
        Mask,
    }

    public class WorldPoint
    {
        public Vector3 point;
        public Vector3 normal;
        public bool mapped;

        public WorldPoint(Vector3 p, Vector3 n, bool m)
        {
            point = p;
            normal = n;
            mapped = m;
        }
    }

    public class BakingObject : MonoBehaviour
    {
        public Vector3[] vertices;
        public int[] triangles;
        public Vector2[] uvs;
    }

    public class CachedMesh
    {
        public Vector3[] normals;
        public Color[] colors;
        public int[] triangles;
        public Material sharedMaterial;
        public CachedTexture2D mainTex;
        public CachedTexture2D bumpMap;
        public CachedTexture2D metallicGlossMap;
        public CachedTexture2D specGlossMap;
        public Quaternion rotation;

        public CachedTexture2D emissionMap;
        //public Texture2D _ParallaxMap;
        //public Texture2D _OcclusionMap;

        public CachedMesh(MeshFilter original, Dictionary<Texture2D, Texture2D> readableTextures, bool cacheVertexColors, bool cacheDiffuse, bool cacheNormal, bool cacheMetallic, bool cacheSpecular, bool cacheEmissive)
        {
            Mesh sharedMesh = original.sharedMesh;
            normals = sharedMesh.normals;
            triangles = sharedMesh.triangles;
            sharedMaterial = original.GetComponent<Renderer>().sharedMaterial;
            rotation = original.transform.rotation;
            
            if (cacheVertexColors)
            {
                colors = sharedMesh.colors;
            }
            
            if (cacheDiffuse && sharedMaterial.HasProperty("_MainTex"))
            {
                mainTex = new CachedTexture2D(sharedMaterial, Shader.PropertyToID("_MainTex"), readableTextures);
            }

            if (cacheNormal && sharedMaterial.HasProperty("_BumpMap"))
            {
                bumpMap = new CachedTexture2D(sharedMaterial, Shader.PropertyToID("_BumpMap"), readableTextures);
            }

            if (cacheMetallic && sharedMaterial.HasProperty("_MetallicGlossMap"))
            {
                metallicGlossMap = new CachedTexture2D(sharedMaterial, Shader.PropertyToID("_MetallicGlossMap"), readableTextures);
            }

            if (cacheSpecular && sharedMaterial.HasProperty("_SpecGlossMap"))
            {
                specGlossMap = new CachedTexture2D(sharedMaterial, Shader.PropertyToID("_SpecGlossMap"), readableTextures);
            }

            if (cacheEmissive && sharedMaterial.HasProperty("_EmissionMap"))
            {
                emissionMap = new CachedTexture2D(sharedMaterial, Shader.PropertyToID("_EmissionMap"), readableTextures);
            }
        }

        public void DestroyAllCachedTextures()
        {
            if (mainTex != null)
            {
                mainTex.DestroyReadableCopy();
            }
            if (bumpMap != null)
            {
                bumpMap.DestroyReadableCopy();
            }
            if (metallicGlossMap != null)
            {
                metallicGlossMap.DestroyReadableCopy();
            }
            if (specGlossMap != null)
            {
                specGlossMap.DestroyReadableCopy();
            }
            if (emissionMap != null)
            {
                emissionMap.DestroyReadableCopy();
            }
        }
    }

    public class CachedTexture2D
    {
        public Texture2D tex;
        public Vector2 scale;
        public Vector2 offset;

        public CachedTexture2D(Material mat, int property, Dictionary<Texture2D, Texture2D> readableTextures)
        {
            bool isStandard = mat.shader == Shader.Find("Standard") || mat.shader == Shader.Find("shader_StandardMetallic");
            Texture2D originalTexture = (Texture2D) mat.GetTexture(property);
            if (originalTexture == null)
            {
                return;
            }
            if (readableTextures.ContainsKey(originalTexture))
            {
                tex = readableTextures[originalTexture];
                
            }
            else
            {
                tex = CreateReadableCopy((Texture2D) mat.GetTexture(property));
                readableTextures.Add(originalTexture, tex);
            }

            if (isStandard) //standard shader uses the MainTex uvs for all maps
            {
                scale = mat.GetTextureScale("_MainTex");
                offset = mat.GetTextureOffset("_MainTex");
            }
            else
            {
                scale = mat.GetTextureScale(property);
                offset = mat.GetTextureOffset(property);
            }
        }

        private Texture2D CreateReadableCopy(Texture2D source)
        {
            Texture2D copy = new Texture2D(source.width, source.height, source.format, true);
            Graphics.CopyTexture(source, copy);
            return copy;
        }

        public void DestroyReadableCopy()
        {
            UnityEngine.Object.DestroyImmediate(tex);
        }
        
        public static RenderTexture Texture2D2RenderTexture(Texture2D source)
        {
            RenderTexture rt = new RenderTexture(source.width, source.height, 0, RenderTextureFormat.ARGBFloat);
            rt.Create();
            Graphics.Blit(source, rt);
            return rt;
        }
    }

    public class TexToSave
    {

        public string path;
        public MapType type;

        public TexToSave(string p, MapType t)
        {
            path = p;
            type = t;
        }
    }

    [Serializable]
    public class MeshSelection
    {
        public string meshName;
        public bool isSelected;

        public MeshSelection(string name, bool selected)
        {
            meshName = name;
            isSelected = selected;
        }
    }

    [Serializable]
    public class MeshBinding
    {
        public string sourceMesh;
        public MeshSelection[] selections;

        public MeshBinding(string sourceMesh, string[] targetMeshes)
        {
            this.sourceMesh = sourceMesh;
            selections = new MeshSelection[targetMeshes.Length];
            for (var i = 0; i < targetMeshes.Length; i++)
            {
                string targetMesh = targetMeshes[i];
                selections[i] = new MeshSelection(targetMesh, true);
            }
        }

        public string[] GetSelectedMeshesNames()
        {
            return selections.Where(x => x.isSelected).Select(x => x.meshName).ToArray();
        }
    }
}

#endif