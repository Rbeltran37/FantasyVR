/*====================================================
*
* TOTAL BAKER
*
* Francesco Cucchiara - 3POINT SOFT
*
=====================================================*/

#if UNITY_EDITOR

using System.Collections.Generic;
using Object = UnityEngine.Object;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TB
{
    public class SaveManager
    {
        public enum Extension
        {
            PNG,
            JPG,
            EXR
        }

        /// <summary>
        /// Saves a Texture2D to a .png file
        /// </summary>
        /// <param name="tex">Texture to be saved</param>
        /// <param name="path">Save path</param>
        /// <param name="extension">Extension of file</param>
        /// <param name="askConfirmation">If true, a dialog with overwrite confirmation will appear when the file already exists</param>
        /// <param name="notifyOnSaved">If true, a simple "Ok" dialog will appear when the texture has been saved</param>
        /// <param name="refreshAssetDatabase">If true, a call to AssetDatabase.Refresh() will be called after the file has been written</param>
        /// <param name="pingAsset">If true, the newly created asset will be highlighted</param>
        public static void SaveTexture2D(string path, Texture2D tex, Extension extension = Extension.PNG, bool askConfirmation = false, bool notifyOnSaved = false, bool refreshAssetDatabase = false, bool pingAsset = false)
        {
            bool canSave = true;
            
            if (askConfirmation && File.Exists(path))
            {
                if (!EditorUtility.DisplayDialog("Save texture", "Overwrite existing file?", "Yes", "No"))
                {
                    canSave = false;
                }
            }
            
            if (canSave)
            {
                string dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                byte[] bytes = tex.EncodeToPNG();
                File.WriteAllBytes(path, bytes);

                if (refreshAssetDatabase)
                {
                    AssetDatabase.Refresh();
                }
                if (notifyOnSaved)
                {
                    EditorUtility.DisplayDialog("Success", "Texture saved as " + path, "Ok");
                }
                if (pingAsset)
                {
                    EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Texture>(path));
                }
            }
        }

        /// <summary>
        /// Saves a RenderTexture to a .png file
        /// </summary>
        /// <param name="rt">Texture to be saved</param>
        /// <param name="path">Save path</param>
        /// <param name="extension">Extension of file</param>
        /// <param name="askConfirmation">If true, a dialog with overwrite confirmation will appear when the file already exists</param>
        /// <param name="notifyOnSaved">If true, a simple "Ok" dialog will appear when the texture has been saved</param>
        /// <param name="refreshAssetDatabase">If true, a call to AssetDatabase.Refresh() will be called after the file has been written</param>
        /// <param name="pingAsset">If true, the newly created asset will be highlighted</param>
        public static void SaveRenderTexture(string path, RenderTexture rt, Extension extension = Extension.PNG, bool askConfirmation = false, bool notifyOnSaved = false, bool refreshAssetDatabase = false, bool pingAsset = false)
        {
            SaveTexture2D(path,RenderTextureUtility.RenderTexture2Texture2D(rt), extension, askConfirmation, notifyOnSaved, refreshAssetDatabase, pingAsset);
        }

        internal static void SaveTextures()
         {
             int saveCount = 0;
             List<TexToSave> texToOverwrite = new List<TexToSave>();
             string path;

             //create output folder if not exists
             if (!Directory.Exists(TotalBaker.settings.outputFolder))
             {
                 Directory.CreateDirectory(TotalBaker.settings.outputFolder);
             }

             if (TotalBaker.settings.createDiffuseMap)
             {
                 path = TotalBaker.settings.outputFolder + TotalBaker.settings.outputBaseName + "_Diffuse.png";
                 if (File.Exists(path))
                 {
                     texToOverwrite.Add(new TexToSave(path, MapType.Diffuse));
                 }
                 else
                 {
                     SaveRenderTexture(path, TotalBaker.outputRTDiffuse);
                     saveCount++;
                 }
             }

             if (TotalBaker.settings.createHeightMap)
             {
                 string ext = TotalBaker.settings._16bpcHeight ? "exr" : "png";
                 path = TotalBaker.settings.outputFolder + TotalBaker.settings.outputBaseName + "_Height."+ ext;
                 if (File.Exists(path))
                 {
                     texToOverwrite.Add(new TexToSave(path, MapType.Height));
                 }
                 else
                 {
                     Extension extension = TotalBaker.settings._16bpcHeight ? Extension.EXR : Extension.PNG;
                     SaveRenderTexture(path, TotalBaker.outputRTHeight, extension);
                     saveCount++;
                 }
             }

             if (TotalBaker.settings.createNormalMap)
             {
                 path = TotalBaker.settings.outputFolder + TotalBaker.settings.outputBaseName + "_Normal.png";
                 if (File.Exists(path))
                 {
                     texToOverwrite.Add(new TexToSave(path, MapType.Normal));
                 }
                 else
                 {
                     RenderTexture rt = TotalBaker.settings.bakeExistingNormalMap ? TotalBaker.outputRTNormalCombined : TotalBaker.outputRTNormalMain;
                     SaveRenderTexture(path, rt);
                     ImportUtility.ImportAsNormalMap(path);
                     saveCount++;
                 }
             }

             if (TotalBaker.settings.createMetallicMap)
             {
                 path = TotalBaker.settings.outputFolder + TotalBaker.settings.outputBaseName + "_Metallic.png";
                 if (File.Exists(path)) texToOverwrite.Add(new TexToSave(path, MapType.Metallic));
                 else
                 {
                     SaveRenderTexture(path, TotalBaker.outputRTMetallic);
                     saveCount++;
                 }
             }
             
             if (TotalBaker.settings.createSpecularMap)
             {
                 path = TotalBaker.settings.outputFolder + TotalBaker.settings.outputBaseName + "_Specular.png";
                 if (File.Exists(path))
                 {
                     texToOverwrite.Add(new TexToSave(path, MapType.Specular));
                 }
                 else
                 {
                     SaveRenderTexture(path, TotalBaker.outputRTSpecular);
                     saveCount++;
                 }
             }

             if (TotalBaker.settings.createAOMap)
             {
                 path = TotalBaker.settings.outputFolder + TotalBaker.settings.outputBaseName + "_AO.png";
                 if (File.Exists(path))
                 {
                     texToOverwrite.Add(new TexToSave(path, MapType.AO));
                 }
                 else
                 {
                     SaveRenderTexture(path, TotalBaker.outputRTAO);
                     saveCount++;
                 }
             }

             if (TotalBaker.settings.createEmissiveMap)
             {
                 path = TotalBaker.settings.outputFolder + TotalBaker.settings.outputBaseName + "_Emissive.png";
                 if (File.Exists(path))
                 {
                     texToOverwrite.Add(new TexToSave(path, MapType.Emissive));
                 }
                 else
                 {
                     SaveRenderTexture(path, TotalBaker.outputRTEmissive);
                     saveCount++;
                 }
             }
             
             if (TotalBaker.settings.createCurvatureMap)
             {
                 path = TotalBaker.settings.outputFolder + TotalBaker.settings.outputBaseName + "_Curvature.png";
                 if (File.Exists(path))
                 {
                     texToOverwrite.Add(new TexToSave(path, MapType.Curvature));
                 }
                 else
                 {
                     SaveRenderTexture(path, TotalBaker.outputRTCurvature);
                     saveCount++;
                 }
             }
             
             if (TotalBaker.settings.createMaskMap)
             {
                 path = TotalBaker.settings.outputFolder + TotalBaker.settings.outputBaseName + "_Mask.png";
                 if (File.Exists(path))
                 {
                     texToOverwrite.Add(new TexToSave(path, MapType.Mask));
                 }
                 else
                 {
                     SaveRenderTexture(path, TotalBaker.outputRTMask);
                     saveCount++;
                 }
             }

             if (texToOverwrite.Count > 0)
             {
                 string overwriteInfo = "";
                 foreach (TexToSave t in texToOverwrite)
                 {
                     overwriteInfo = overwriteInfo + "Assets" + t.path + "\n";
                 }
                 
                 if (EditorUtility.DisplayDialog("Save textures","The following files already exist. Overwrite them?\n\n" + overwriteInfo, "Overwrite", "Cancel"))
                 {
                     foreach (TexToSave t in texToOverwrite)
                     {
                         switch (t.type)
                         {
                             case MapType.Diffuse:
                                 SaveRenderTexture(t.path, TotalBaker.outputRTDiffuse);
                                 break;
                             case MapType.Normal:
                                 RenderTexture rt = TotalBaker.settings.bakeExistingNormalMap ? TotalBaker.outputRTNormalCombined : TotalBaker.outputRTNormalMain;
                                 SaveRenderTexture(t.path, rt);
                                 ImportUtility.ImportAsNormalMap(t.path);
                                 break;
                             case MapType.Height:
                                 Extension ext = TotalBaker.settings._16bpcHeight ? Extension.EXR : Extension.PNG;
                                 SaveRenderTexture(t.path, TotalBaker.outputRTHeight, ext);
                                 break;
                             case MapType.Metallic:
                                 SaveRenderTexture(t.path, TotalBaker.outputRTMetallic);
                                 break;
                             case MapType.Specular:
                                 SaveRenderTexture(t.path, TotalBaker.outputRTSpecular);
                                 break;
                             case MapType.Emissive:
                                 SaveRenderTexture(t.path, TotalBaker.outputRTEmissive);
                                 break;
                             case MapType.AO:
                                 SaveRenderTexture(t.path, TotalBaker.outputRTAO);
                                 break;
                             case MapType.Curvature:
                                 SaveRenderTexture(t.path, TotalBaker.outputRTCurvature);
                                 break;
                             case MapType.Mask:
                                 SaveRenderTexture(t.path, TotalBaker.outputRTMask);
                                 break;
                             default:
                                 break;
                         }

                         saveCount++;
                     }
                     AssetDatabase.Refresh();
                     if (TotalBaker.settings.showConsoleLog)
                     {
                         Debug.Log(TotalBaker.logPrefix + "Successfully saved " + saveCount + " textures.");
                     }
                 }
             }
             
         }

    }

}

#endif