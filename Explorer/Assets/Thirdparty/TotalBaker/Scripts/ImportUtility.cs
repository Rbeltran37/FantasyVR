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

namespace TB
{

	public class ImportUtility
	{
        public class ModelToReimport : IEquatable<ModelToReimport>
        {
            public string assetPath;
            public ModelImporter modelImporter;
            public ModelImporterNormals normals;
            public float smoothingAngle;

            public ModelToReimport(string path)
            {
                assetPath = path;
                modelImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;
                normals = GetModelImporterNormals(modelImporter);
                smoothingAngle = GetModelImporterNormalSmoothingAngle(modelImporter);
            }

            public bool IsDirty()
            {
                if (modelImporter == null)
                {
                    return false;
                }

                return !Mathf.Approximately(modelImporter.normalSmoothingAngle, smoothingAngle) || modelImporter.importNormals != normals;
            }

            public bool Equals(ModelToReimport other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return assetPath == other.assetPath;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((ModelToReimport) obj);
            }

            public override int GetHashCode()
            {
                return (assetPath != null ? assetPath.GetHashCode() : 0);
            }

            public static bool operator ==(ModelToReimport left, ModelToReimport right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(ModelToReimport left, ModelToReimport right)
            {
                return !Equals(left, right);
            }
        }
        
        public static void ImportAsNormalMap(string path)
        {
            TextureImporter tImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            if (tImporter != null)
            {
                tImporter.textureType = TextureImporterType.NormalMap;
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            }
        }

        /// <summary>
        /// Given a mesh and smoothing angle, checks if it needs to be reimported to match the smoothingAngle.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="normalsImportType"></param>
        /// <param name="smoothingAngle"></param>
        /// <returns></returns>
        public static bool CheckReimportNeeded(Mesh mesh, Settings.NormalsImportType normalsImportType, float smoothingAngle)
        {
            ModelImporter mImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(mesh)) as ModelImporter;
            if (mImporter != null && (
                    mImporter.importTangents != ModelImporterTangents.CalculateMikk ||
                    (normalsImportType == Settings.NormalsImportType.Import &&
                     mImporter.importNormals == ModelImporterNormals.Calculate) ||
                    (normalsImportType == Settings.NormalsImportType.Calculate &&
                     mImporter.importNormals == ModelImporterNormals.Import) ||
                    (mImporter.importNormals == ModelImporterNormals.Calculate &&
                     !Mathf.Approximately(smoothingAngle, mImporter.normalSmoothingAngle))
                ))
            {
                return true;
            }
            return false; 
        }


        /// <summary>
        /// Given a root object and a smoothing angle, checks if all the meshes inside root need to be reimported to match the smoothingAngle.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="normalsImportType"></param>
        /// <param name="smoothingAngle"></param>
        /// <returns>A list of all the asset paths to be reimported</returns>
        public static List<ModelToReimport> CheckReimportNeeded(GameObject root, Settings.NormalsImportType normalsImportType, float smoothingAngle)
        {
            List<ModelToReimport> toBeReimported = new List<ModelToReimport>();
            MeshFilter[] meshFilters = root.GetComponentsInChildren<MeshFilter>();
            foreach (MeshFilter mf in meshFilters)
            {
                if (CheckReimportNeeded(mf.sharedMesh, normalsImportType, smoothingAngle))
                {
                    ModelToReimport newEntry = new ModelToReimport(AssetDatabase.GetAssetPath(mf.sharedMesh));
                    if (!toBeReimported.Contains(newEntry))
                    {
                        toBeReimported.Add(new ModelToReimport(AssetDatabase.GetAssetPath(mf.sharedMesh)));
                    }
                }
            }
            return toBeReimported;
        }

        /// <summary>
        /// Reimports the model
        /// </summary>
        /// <param name="model">The model to be reimported</param>
        /// <param name="refresh">Refresh asset database once finished?</param>
        public static void ReimportModel(ModelToReimport model, bool refresh)
        {
            ModelImporter mImporter = AssetImporter.GetAtPath(model.assetPath) as ModelImporter;
            if (mImporter == null)
            {
                return;
            }

            mImporter.importNormals = model.normals;
            mImporter.normalSmoothingAngle = model.smoothingAngle;
            AssetDatabase.ImportAsset(model.assetPath, ImportAssetOptions.ForceUpdate);
            
            if (refresh) AssetDatabase.Refresh();
        }

        /// <summary>
        /// Reimports the models if needed
        /// </summary>
        /// <param name="models">The models to be reimported</param>
        /// <param name="refresh">Refresh asset database once finished?</param>
        public static void ReimportModels(List<ModelToReimport> models, bool refresh)
        {
            if (models == null)
            {
                return;
            }

            foreach (ModelToReimport model in models)
            {
                if (model.modelImporter == null)
                {
                    continue;
                }

                if (model.IsDirty())
                {
                    ReimportModel(model, false);
                }
            }

            if (refresh) AssetDatabase.Refresh();
        }

        /// <summary>
        /// Reimports the object's assets with the given import normal settings
        /// </summary>
        /// <param name="obj">The asset to be reimported</param>
        /// <param name="normals">Normals import type</param>
        /// <param name="refresh">Refresh asset database once finished?</param>
        public static void ReimportModel(GameObject obj, Settings.NormalsImportType normals, bool refresh)
        {
            ReimportModel(obj, normals, 0, refresh);
        }

        /// <summary>
        /// Reimports the object's assets with the given import normal settings
        /// </summary>
        /// <param name="obj">The asset to be reimported</param>
        /// <param name="normals">Normals import type</param>
        /// <param name="angle">Smoothing angle</param>
        /// <param name="refresh">Refresh asset database once finished?</param>
        public static void ReimportModel(GameObject obj, Settings.NormalsImportType normals, float angle, bool refresh)
        {
            List<string> assetsToReimport = GetAllModelAssets(obj);

            foreach (string path in assetsToReimport)
            {
                ModelImporter mImporter = AssetImporter.GetAtPath(path) as ModelImporter;
                if (mImporter == null)
                {
                    continue;
                }

                if (normals == Settings.NormalsImportType.Import)
                {
                    if (mImporter.importNormals == ModelImporterNormals.Import &&
                        mImporter.importTangents == ModelImporterTangents.CalculateMikk)
                        return; //same settings, do not reimport model
                    mImporter.importNormals = ModelImporterNormals.Import;
                    mImporter.importTangents = ModelImporterTangents.CalculateMikk;
                }
                else
                {
                    if (mImporter.importNormals == ModelImporterNormals.Calculate &&
                        Mathf.Approximately(mImporter.normalSmoothingAngle, angle) &&
                        mImporter.importTangents == ModelImporterTangents.CalculateMikk
                    )
                    {
                        return; //same settings, do not reimport model
                    }

                    mImporter.importNormals = ModelImporterNormals.Calculate;
                    mImporter.normalSmoothingAngle = angle;
                    mImporter.importTangents = ModelImporterTangents.CalculateMikk;
                }
                AssetDatabase.ImportAsset(path);
            }
            
            if (refresh) AssetDatabase.Refresh();
        }
        
        public static void ReimportModel(GameObject obj, ModelImporterNormals normals, float angle, bool refresh)
        {
            List<string> assetsToReimport = GetAllModelAssets(obj);

            foreach (string path in assetsToReimport)
            {

                ModelImporter mImporter = AssetImporter.GetAtPath(path) as ModelImporter;
                if (mImporter != null)
                {
                    if (normals == ModelImporterNormals.Import)
                    {
                        if (mImporter.importNormals == ModelImporterNormals.Import &&
                            mImporter.importTangents == ModelImporterTangents.CalculateMikk)
                        {
                            return; //same settings, do not reimport model
                        }

                        mImporter.importNormals = normals;
                        mImporter.importTangents = ModelImporterTangents.CalculateMikk;
                    }
                    else
                    {
                        if (mImporter.importNormals == ModelImporterNormals.Calculate &&
                            Mathf.Approximately(mImporter.normalSmoothingAngle, angle) &&
                            mImporter.importTangents == ModelImporterTangents.CalculateMikk

                        )
                        {
                            return; //same settings, do not reimport model
                        }

                        mImporter.importNormals = ModelImporterNormals.Calculate;
                        mImporter.normalSmoothingAngle = angle;
                        mImporter.importTangents = ModelImporterTangents.CalculateMikk;
                    }
                }
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            }

            if (refresh) AssetDatabase.Refresh();
        }
        
        public static void ReimportModels(List<string> paths, ModelImporterNormals normals, float angle, bool refresh)
        {
            foreach (string path in paths)
            {
                ModelImporter mImporter = AssetImporter.GetAtPath(path) as ModelImporter;
                if (mImporter != null)
                {
                    if (normals == ModelImporterNormals.Import)
                    {
                        if (mImporter.importNormals == ModelImporterNormals.Import &&
                            mImporter.importTangents == ModelImporterTangents.CalculateMikk)
                        {
                            return; //same settings, do not reimport model
                        }

                        mImporter.importNormals = normals;
                        mImporter.importTangents = ModelImporterTangents.CalculateMikk;
                    }
                    else
                    {
                        if (mImporter.importNormals == ModelImporterNormals.Calculate &&
                            Mathf.Approximately(mImporter.normalSmoothingAngle, angle) &&
                            mImporter.importTangents == ModelImporterTangents.CalculateMikk

                        )
                        {
                            return; //same settings, do not reimport model
                        }

                        mImporter.importNormals = ModelImporterNormals.Calculate;
                        mImporter.normalSmoothingAngle = angle;
                        mImporter.importTangents = ModelImporterTangents.CalculateMikk;
                    }
                }
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            }

            if (refresh) AssetDatabase.Refresh();
        }
        
        public static void ReimportModels(List<ModelToReimport> paths, ModelImporterNormals normals, float angle, bool refresh)
        {
            foreach (ModelToReimport model in paths)
            {
                ModelImporter mImporter = AssetImporter.GetAtPath(model.assetPath) as ModelImporter;
                if (mImporter != null)
                {
                    if (normals == ModelImporterNormals.Import)
                    {
                        if (mImporter.importNormals == ModelImporterNormals.Import &&
                            mImporter.importTangents == ModelImporterTangents.CalculateMikk)
                        {
                            return; //same settings, do not reimport model
                        }

                        mImporter.importNormals = normals;
                        mImporter.importTangents = ModelImporterTangents.CalculateMikk;
                    }
                    else
                    {
                        if (mImporter.importNormals == ModelImporterNormals.Calculate &&
                            Mathf.Approximately(mImporter.normalSmoothingAngle, angle) &&
                            mImporter.importTangents == ModelImporterTangents.CalculateMikk

                        )
                        {
                            return; //same settings, do not reimport model
                        }

                        mImporter.importNormals = ModelImporterNormals.Calculate;
                        mImporter.normalSmoothingAngle = angle;
                        mImporter.importTangents = ModelImporterTangents.CalculateMikk;
                    }
                }
                AssetDatabase.ImportAsset(model.assetPath, ImportAssetOptions.ForceUpdate);
            }

            if (refresh) AssetDatabase.Refresh();
        }
        

        /// <summary>
        /// Retrieves all the meshes in the given object and returns a list of asset paths in which each entry is the model asset of a mesh
        /// </summary>
        /// <param name="obj">The asset to be reimported</param>
        private static List<string> GetAllModelAssets(GameObject obj)
        {
            List<string> result = new List<string>();
            MeshFilter[] filters = obj.GetComponentsInChildren<MeshFilter>();
            foreach (MeshFilter meshFilter in filters)
            {
                string path = AssetDatabase.GetAssetPath(meshFilter.sharedMesh);
                if (!string.IsNullOrEmpty(path))
                {
                    result.Add(path);
                }
            }
            return result;
        }


        /// <summary>
        ///  Given a GameObject, this function gets all the models in it and returns their import settings as a list of ModelToReimport
        /// </summary>
        /// <returns></returns>
        public static List<ModelToReimport> GetAllModelsImportSettings(GameObject obj)
        {
            if (obj == null)
            {
                return null;
            }
            List<ModelToReimport> list = new List<ModelToReimport>();
            List<string> models = GetAllModelAssets(obj);
            foreach (string model in models)
            {
                ModelImporter mImporter = AssetImporter.GetAtPath(model) as ModelImporter;
                if (mImporter != null)
                {
                    list.Add(new ModelToReimport(model));
                }
            }
            return list;
        } 
        
        /// <summary>
        /// Is tex marked as normal map?
        /// </summary>
        /// <param name="tex"></param>
        /// <returns></returns>
        public static bool IsImportedAsNormalMap(Texture2D tex)
        {
            string path = AssetDatabase.GetAssetPath(tex);
            var tImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            if (tImporter != null)
            {
                return tImporter.textureType == TextureImporterType.NormalMap;

            }

            return false;
        }


        public static ModelImporterNormals GetModelImporterNormals(string path)
        {
            ModelImporter mImporter = AssetImporter.GetAtPath(path) as ModelImporter;
            return GetModelImporterNormals(mImporter);
        }
        
        public static float GetModelImporterNormalSmoothingAngle(string path)
        {
            ModelImporter mImporter = AssetImporter.GetAtPath(path) as ModelImporter;
            return GetModelImporterNormalSmoothingAngle(mImporter);
        }
        
        public static ModelImporterNormals GetModelImporterNormals(ModelImporter mImporter)
        {
            if (mImporter == null)
            {
                return ModelImporterNormals.None;
            }

            return mImporter.importNormals;
        }
        
        public static float GetModelImporterNormalSmoothingAngle(ModelImporter mImporter)
        {
            if (mImporter == null)
            {
                return -1;
            }

            return mImporter.normalSmoothingAngle;
        }
	}

}

#endif