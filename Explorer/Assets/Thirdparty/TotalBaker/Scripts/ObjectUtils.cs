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
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TB
 {
     public static class ObjectUtils
     {

         private static Dictionary<PrimitiveType, Mesh> builtinPrimitiveMeshes;
         private static Dictionary<PrimitiveType, Mesh> customPrimitiveMeshes;
         private static PrimitiveType[] primitiveTypes;
         
         static ObjectUtils()
         {
             primitiveTypes = (PrimitiveType[])Enum.GetValues(typeof(PrimitiveType));
             CacheBuiltinPrimitiveMeshes();
             CacheCustomPrimitiveMeshes();
         }
         
         
        internal static Mesh CombineModel(GameObject model)
        {
            MeshFilter[] meshFilters = model.GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];
            for (int i = 0; i < meshFilters.Length; i++)
            {
                if (meshFilters[i].sharedMesh == null) continue;
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            }

            Mesh combinedMesh = new Mesh();
            combinedMesh.CombineMeshes(combine);
            return combinedMesh;
        }
         
         internal static void NormalizeSize(GameObject obj)
         {
             Transform t = obj.transform;
             t.localScale = Vector3.one;
             Bounds bounds = ComputeBounds(obj);
             Vector3 size = bounds.size;
             float scale = 1 / Mathf.Max(size.x, Mathf.Max(size.y, size.z));
             t.localScale *= scale;
         }
         
         internal static GameObject ToUniqueMaterialPerMeshObject(GameObject source)
         {
             GameObject newInstance = new GameObject(source.name);
             newInstance.transform.localRotation = source.transform.localRotation;
             MeshFilter[] originalMeshFilters = source.GetComponentsInChildren<MeshFilter>();
             foreach (MeshFilter mf in originalMeshFilters)
             {
                 if (mf.sharedMesh == null)
                 {
                     continue;
                 }

                 Mesh[] submeshes = mf.sharedMesh.GetAllSubmeshes();
                 MeshRenderer rend = mf.GetComponent<MeshRenderer>();
                 Material[] shaderMaterials = rend.sharedMaterials;
                 for (int i = 0; i < submeshes.Length; i++)
                 {
                     GameObject child = new GameObject("Mesh" + i);
                     child.transform.SetParent(newInstance.transform);
                     Transform mfTransform = mf.transform;
                     child.transform.localPosition = mfTransform.localPosition;
                     child.transform.localRotation = Quaternion.identity;
                     child.AddComponent<MeshRenderer>().sharedMaterial = shaderMaterials[i];
                     child.AddComponent<MeshFilter>().sharedMesh = submeshes[i];
                 }
             }

             return newInstance;
         }

        internal static void SkinnedToMesh(GameObject obj)
        {
            SkinnedMeshRenderer[] skinnedRenderers = obj.GetComponentsInChildren<SkinnedMeshRenderer>();
            for (int i = 0; i < skinnedRenderers.Length; i++)
            {
                SkinnedMeshRenderer smr = skinnedRenderers[i];
                MeshFilter filter = smr.gameObject.AddComponent<MeshFilter>();
                filter.sharedMesh = smr.sharedMesh;
                MeshRenderer rend = smr.gameObject.AddComponent<MeshRenderer>();
                rend.sharedMaterials = smr.sharedMaterials;
                Object.DestroyImmediate(smr);
            }
        }

        internal static bool HasOneMaterialPerMesh(GameObject obj)
        {
            Renderer[] rends = obj.GetComponentsInChildren<Renderer>();
            foreach (Renderer rend in rends)
            {
                if (!HasOneMaterial(rend))
                {
                    return false;
                }
            }

            return true;
        }

        internal static bool HasOneMaterial(Renderer rend)
        {
            return rend.sharedMaterials.Length == 1;
        }

        internal static bool HasMissingMaterials(GameObject obj)
        {
            Renderer[] rends = obj.GetComponentsInChildren<Renderer>();
            if (rends == null || rends.Length == 0)
            {
                return true;
            }

            foreach (Renderer r in rends)
            {
                for (int i = 0; i < r.sharedMaterials.Length; i++)
                {
                    if (r.sharedMaterial == null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        internal static Bounds ComputeBounds(GameObject root)
        {
            Quaternion currentRotation = root.transform.rotation;
            root.transform.rotation = Quaternion.Euler(0f,0f,0f);
 
            Bounds bounds = new Bounds(root.transform.position, Vector3.zero);
 
            Renderer[] rends = root.GetComponentsInChildren<Renderer>();
            foreach(Renderer renderer in rends)
            {
                bounds.Encapsulate(renderer.bounds);
            }
 
            Vector3 localCenter = bounds.center - root.transform.position;
            bounds.center = localCenter;
            root.transform.rotation = currentRotation;

            return bounds;
        }
        
        /// <summary>
        /// Creates a copy of root, expands all children's vertices, and returns it
        /// </summary>
        /// <param name="root"></param>
        /// <param name="offset"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        internal static GameObject CreateCageFromRoot(GameObject root, float offset, float scale)
        {
            Vector3 originalScale = root.transform.localScale;
            originalScale *= scale;

            float s = Mathf.Max(originalScale.x, Mathf.Max(originalScale.y, originalScale.z));
            
            float scaledOffset = offset/s;

            //expands vertices                                    
            GameObject newCage = Object.Instantiate(root);

            MeshFilter[] meshFilters = newCage.GetComponentsInChildren<MeshFilter>();

            foreach (MeshFilter meshFilter in meshFilters)
            {
                Mesh mesh = Object.Instantiate(meshFilter.sharedMesh);
                int[] tris = mesh.triangles;
                Vector3[] verts = mesh.vertices;
                Vector3[] normals = mesh.normals;

                Vector3[] newVerts = new Vector3[verts.Length];

                for (int i = 0; i < tris.Length; i += 3)
                {
                    int i1 = tris[i]; // 1st vertex of this triangle
                    int i2 = tris[i + 1]; // 2nd vertex of this triangle
                    int i3 = tris[i + 2]; // 3rd vertex of this triangle
                    Vector3 v1 = verts[i1];
                    Vector3 v2 = verts[i2];
                    Vector3 v3 = verts[i3];
                    newVerts[i1] = v1 + normals[i1] * scaledOffset;
                    newVerts[i2] = v2 + normals[i2] * scaledOffset;
                    newVerts[i3] = v3 + normals[i3] * scaledOffset;
                }
                mesh.vertices = newVerts;
                mesh.RecalculateBounds();
                meshFilter.sharedMesh = mesh;
            }
            return newCage;
        }

        internal static void SetLayer(GameObject obj, int layer, bool recursive)
        {
            obj.layer = layer;
            if (recursive)
            {
                foreach (Transform t in obj.transform)
                {
                    SetLayer(t.gameObject, layer, true);
                }
            }
        }

        internal static void ApplyMaterialToAllRenderers(GameObject root, Material mat)
        {
            Renderer[] rends = root.GetComponentsInChildren<Renderer>();
            foreach (Renderer rend in rends)
            {
                Material[] sharedMats = rend.sharedMaterials;
                for (int  i = 0; i < sharedMats.Length; i++)
                {
                    sharedMats[i] = mat;
                }
                rend.sharedMaterials = sharedMats;
            }
        }
        
        internal static void DestroyAllMeshes(GameObject root)
        {
            MeshFilter[] filters = root.GetComponentsInChildren<MeshFilter>();
            foreach (MeshFilter filter in filters)
            {
                if (filter.sharedMesh != null)
                {
                    Object.DestroyImmediate(filter.sharedMesh);
                }
            }
        }
        
        internal static bool HasTheSameMeshesInDifferentChildren(GameObject root)
        {
            MeshFilter[] meshFilters = root.GetComponentsInChildren<MeshFilter>();
            Dictionary<Mesh, bool> meshes = new Dictionary<Mesh, bool>();
            foreach (MeshFilter mf in meshFilters)
            {
                if (mf.sharedMesh != null)
                {
                    if (meshes.ContainsKey(mf.sharedMesh))
                    {
                        return true;
                    }

                    meshes[mf.sharedMesh] = true;
                }
            }

            return false;
        }

        internal static bool HasTheSameMaterialInDifferentChildren(GameObject root)
        {
            Renderer[] renderers = root.GetComponentsInChildren<Renderer>();
            Dictionary<Material, bool> mats = new Dictionary<Material, bool>();
            foreach (Renderer rend in renderers)
            {
                if (rend.sharedMaterial != null)
                {
                    if (mats.ContainsKey(rend.sharedMaterial))
                    {
                        return true;
                    }

                    mats[rend.sharedMaterial] = true;
                }
            }

            return false;
        }

        private static void CacheBuiltinPrimitiveMeshes()
        {
            builtinPrimitiveMeshes = new Dictionary<PrimitiveType, Mesh>();
            foreach (PrimitiveType primitive in primitiveTypes)
            {
                //instantiate primitive to retrieve its mesh
                GameObject instance = GameObject.CreatePrimitive(primitive);
                instance.hideFlags = HideFlags.HideAndDontSave;
                MeshFilter rend = instance.GetComponent<MeshFilter>();
                builtinPrimitiveMeshes[primitive] = rend.sharedMesh;
                //destroy the instance
                Object.DestroyImmediate(instance);
            }
        }
        
        private static void CacheCustomPrimitiveMeshes()
        {
            customPrimitiveMeshes = new Dictionary<PrimitiveType, Mesh>();
            foreach (PrimitiveType primitive in primitiveTypes)
            {
                string assetPath = "Assets/TotalBaker/Primitives/" + primitive + ".fbx";
                Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>(assetPath);
                customPrimitiveMeshes[primitive] = mesh;
            }
        }

        internal static void FixPrimitives(GameObject root)
        {
            MeshFilter[] meshFilters = root.GetComponentsInChildren<MeshFilter>();
            foreach (MeshFilter mf in meshFilters)
            {
                foreach (PrimitiveType primitiveType in primitiveTypes)
                {
                    if (mf.sharedMesh == builtinPrimitiveMeshes[primitiveType])
                    {
                        mf.sharedMesh = customPrimitiveMeshes[primitiveType];
                    }
                }
            }
        }
        
     }
 }
 
 #endif