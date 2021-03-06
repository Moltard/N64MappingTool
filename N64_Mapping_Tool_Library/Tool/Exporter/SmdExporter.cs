﻿using N64Library.Tool.Data;
using N64Library.Tool.Modifier;
using N64Library.Tool.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N64Library.Tool.Exporter
{
    public static class SmdExporter
    {

        private const string DefaultMaterial = "default.bmp";
        private const string DefaultCoord = "0.000000 0.000000 0.000000";
        private const string DefaultUV = "0.000000 0.000000";

        /// <summary>
        /// Create the smd file with the given data
        /// </summary>
        /// <param name="objData">Data parsed from the obj file</param>
        /// <param name="mtlData">Data parsed from the mtl file</param>
        /// <param name="smdFilename">Output path</param>
        /// <returns></returns>
        public static bool WriteSmd(ObjData objData, MtlData mtlData, string smdFilename, bool useTextureName)
        {
            if (useTextureName)
            {
                ObjModifier.UpdateUsedTexture(objData, mtlData);
            }
            try
            {
                using (StreamWriter smd = new StreamWriter(smdFilename))
                {
                    // version 1
                    smd.WriteLine(SmdUtils.GetNewHeader());

                    // nodes
                    smd.WriteLine(SmdUtils.GetNewNodes());
                    smd.WriteLine(SmdUtils.GetNewBone());
                    smd.WriteLine(SmdUtils.GetNewEnd());

                    // skeleton
                    smd.WriteLine(SmdUtils.GetNewSkeleton());
                    smd.WriteLine(SmdUtils.GetNewTime());
                    smd.WriteLine(SmdUtils.GetNewPosition());
                    smd.WriteLine(SmdUtils.GetNewEnd());

                    // triangles
                    smd.WriteLine(SmdUtils.GetNewTriangles());
                    foreach (ObjectObj objectObj in objData.ObjectsList)
                    {
                        string smdMaterial = GetSmdMaterial(objectObj.MaterialName, objectObj.TextureName, useTextureName);

                        int coordLength = objectObj.VerticesList.Count;
                        int uvLength = objectObj.UVsList.Count;
                        int normalLength = objectObj.NormalsList.Count;

                        foreach (VertIndexesObj vertIndexes in objectObj.VertIndexesList)
                        {
                            smd.WriteLine(smdMaterial);
                            foreach (VertIndexObj vertIndex in vertIndexes.VertIndexList)
                            {
                                string coordStr = DefaultCoord;
                                string uvStr = DefaultUV;
                                string normalStr = DefaultCoord;


                                if (vertIndex.V != null)
                                {
                                    if (vertIndex.V < coordLength) // Should always be the case
                                    {
                                        // Source Engine use 'z' for the up-down axis
                                        coordStr = objectObj.VerticesList[(int)vertIndex.V].ToSmd();
                                    }
                                }

                                if (vertIndex.Vt != null)
                                {
                                    if (vertIndex.Vt < uvLength) // Should always be the case
                                    {
                                        uvStr = objectObj.UVsList[(int)vertIndex.Vt].ToString();
                                    }
                                }
                                
                                if (vertIndex.Vn != null)
                                {
                                    if (vertIndex.Vn < normalLength) // Should always be the case
                                    {
                                        // Source Engine use 'z' for the up-down axis
                                        normalStr = objectObj.NormalsList[(int)vertIndex.Vn].ToSmd();
                                    }
                                }
                                       
                                smd.WriteLine(SmdUtils.GetNewTriangle(0, coordStr, normalStr, uvStr));
                            }
                        }
                    }
                    smd.WriteLine(SmdUtils.GetNewEnd());
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Returns the name of the material used by an object in the SMD
        /// </summary>
        /// <param name="materialName">Name of the OBJ material</param>
        /// <param name="textureName">Name of the texture of the material</param>
        /// <param name="useTextureName">Should the SMD use the texture or material name</param>
        /// <returns></returns>
        private static string GetSmdMaterial(string materialName, string textureName, bool useTextureName)
        {
            string smdMaterial;
            if (useTextureName)
            {
                if (textureName != null)
                {
                    smdMaterial = textureName;
                }
                else
                {
                    smdMaterial = materialName;
                }
            }
            else
            {
                smdMaterial = materialName;
            }
            if (smdMaterial == null)
            {
                return DefaultMaterial;
            }
            return smdMaterial;
        }


    }
}
