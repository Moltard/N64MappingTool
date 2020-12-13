using N64Library.Tool.Data;
using N64Library.Tool.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N64Library.Tool.Modifier
{
    public static class ObjModifier
    {
        /// <summary>
        /// Iterate through the mtl to update every texture associated to each material in the obj
        /// </summary>
        /// <param name="objData">Data parsed from the obj file</param>
        /// <param name="mtlData">Data parsed from the mtl file</param>
        public static void UpdateUsedTexture(ObjData objData, MtlData mtlData)
        {
            if (mtlData != null)
            {
                Dictionary<string, string> dictMaterialTexture = MtlUtils.GetDictMaterialTexture(mtlData);
                foreach (ObjectObj objectObj in objData.ObjectsList)
                {
                    if (objectObj.MaterialName != null)
                        objectObj.TextureName = MtlUtils.GetTextureFromMaterial(dictMaterialTexture, objectObj.MaterialName);
                }
            }
        }

        /// <summary>
        /// Calculate the normal vector for each triangle
        /// </summary>
        /// <param name="objData">Data parsed from the obj file</param>
        public static void CalculateNormal(ObjData objData)
        {
            foreach (ObjectObj objectObj in objData.ObjectsList)
            {
                // A triangle is made from 3 points
                LocalIndexesObj localIndexes = new LocalIndexesObj();
                List<Vector> normalsList = objectObj.NormalsList;

                localIndexes.vnIndex = normalsList.Count; // Set the current max vnIndex

                List<Point> verticesList = objectObj.VerticesList;
                int verticesLength = verticesList.Count;

                foreach (VertIndexesObj vertIndexes in objectObj.VertIndexesList)
                {
                    if (vertIndexes.VertIndexList.Count >= 3) // Should always be the case
                    {
                        VertIndexObj vertIndex1 = vertIndexes.VertIndexList[0];
                        VertIndexObj vertIndex2 = vertIndexes.VertIndexList[1];
                        VertIndexObj vertIndex3 = vertIndexes.VertIndexList[2];

                        // If a normal needs to be calculated
                        if (vertIndex1.Vn == null || vertIndex2.Vn == null || vertIndex3.Vn == null)
                        {
                            // If all points indexes are correctly defined
                            if (vertIndex1.V != null && vertIndex2.V != null && vertIndex3.V != null)
                            {
                                Point p1 = (vertIndex1.V < verticesLength) ? verticesList[(int)vertIndex1.V] : null;
                                Point p2 = (vertIndex2.V < verticesLength) ? verticesList[(int)vertIndex2.V] : null;
                                Point p3 = (vertIndex3.V < verticesLength) ? verticesList[(int)vertIndex3.V] : null;

                                // If all indexes lead to existing points
                                if (p1 != null && p2 != null && p3 != null)
                                {
                                    Vector vector1 = new Vector(p1, p2);
                                    Vector vector2 = new Vector(p2, p3);

                                    Vector vectorNormal = Vector.GetNormalVector(vector1, vector2);
                                    normalsList.Add(vectorNormal.GetUnitVector()); // Add to the list the unit vector

                                    // We assign the index of the normal vector to 
                                    if (vertIndex1.Vn == null)
                                        vertIndex1.Vn = localIndexes.vnIndex;
                                    if (vertIndex2.Vn == null)
                                        vertIndex2.Vn = localIndexes.vnIndex;
                                    if (vertIndex3.Vn == null)
                                        vertIndex3.Vn = localIndexes.vnIndex;

                                    // Increment the index for the next normal vector
                                    localIndexes.vnIndex++;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Delete every material from each object
        /// </summary>
        /// <param name="objData">Data parsed from the obj file</param>
        public static void DeleteMaterials(ObjData objData)
        {
            foreach (ObjectObj objectObj in objData.ObjectsList)
            {
                objectObj.MaterialName = null;
                objectObj.TextureName = null;
            }
        }

        /// <summary>
        /// Delete every unused material the obj file
        /// </summary>
        /// <param name="objData">Data parsed from the obj file</param>
        /// <param name="mtlData">Data parsed from the mtl file</param>
        public static bool DeleteUnusedMaterials(ObjData objData, MtlData mtlData)
        {
            if (mtlData == null)
                return false;

            // Dictionary that associate every material to a list of groups
            Dictionary<string, List<int>> dictMaterialGroups = ObjUtils.GetDictMaterialGroups(objData);

            // Dictionary that associate every material of the mtl file to their index
            Dictionary<string, int> dictMaterialIndex = MtlUtils.GetDictMaterialIndex(mtlData);
            
            List<MaterialMtl> newMaterialsList = new List<MaterialMtl>();

            foreach (KeyValuePair<string, List<int>> keyValue in dictMaterialGroups)
            {
                if (dictMaterialIndex.TryGetValue(keyValue.Key, out int index)) // We get the index of the material
                {
                    MaterialMtl materialMtl = mtlData.MaterialsList[index];
                    newMaterialsList.Add(materialMtl);
                }
            }
            
            // Every material that wasnt used in a group was not added to the list
            mtlData.MaterialsList = newMaterialsList;
            return true;
        }
        
        /// <summary>
        /// Extract each .bmp texture from the material or group and store it into the TextureName attributes
        /// </summary>
        /// <param name="objData">Data parsed from the obj file</param>
        public static void ParseTextures(ObjData objData)
        {
            foreach (ObjectObj objectObj in objData.ObjectsList)
            {
                if (objectObj.GroupName == null)
                    if (objectObj.ObjectName != null) 
                        objectObj.GroupName = objectObj.ObjectName;
                    else // Impossible to set a group name (unlikely to happen)
                        continue;

                if (objectObj.MaterialName == null) // If no material
                    objectObj.MaterialName = objectObj.GroupName;

                if (objectObj.TextureName == null) // No texture assigned currently
                {
                    string textureName = GenericUtils.ExtractTextureNameFromMaterial(objectObj.MaterialName);
                    if (textureName == null) // No bmp found in the material
                        textureName = GenericUtils.ExtractTextureNameFromMaterial(objectObj.GroupName);
                    objectObj.TextureName = textureName;
                }
            }
        }

        /// <summary>
        /// Uniformly Scale the model by the given value
        /// </summary>
        /// <param name="objData">Data parsed from the obj file</param>
        /// <param name="value">Scale value</param>
        public static void UniformScale(ObjData objData, double value)
        {
            foreach (ObjectObj objectObj in objData.ObjectsList)
                foreach (Point p in objectObj.VerticesList)
                    p.ScaleXYZ(value);  
        }

        /// <summary>
        /// Non Uniformly Scale the model by the given values
        /// </summary>
        /// <param name="objData">Data parsed from the obj file</param>
        /// <param name="valueX">Scale value on the X axis</param>
        /// <param name="valueY">Scale value on the Y axis</param>
        /// <param name="valueZ">Scale value on the Z axis</param>
        public static void NonUniformScale(ObjData objData, double valueX, double valueY, double valueZ)
        {
            foreach (ObjectObj objectObj in objData.ObjectsList)
            {
                foreach (Point p in objectObj.VerticesList)
                {
                    p.ScaleXYZ(valueX, valueY, valueZ);
                }
            }
        }

        /// <summary>
        /// Reverse the vertex order of the model
        /// </summary>
        /// <param name="objData">Data parsed from the obj file</param>
        public static void ReverseVertexOrder(ObjData objData)
        {
            foreach (ObjectObj objectObj in objData.ObjectsList)
            {
                foreach (VertIndexesObj vertIndexes in objectObj.VertIndexesList)
                {
                    vertIndexes.VertIndexList.Reverse();
                }
            }
        }

        /// <summary>
        /// Rotate the model with the given angles
        /// </summary>
        /// <param name="objData">Data parsed from the obj file</param>
        /// <param name="aX">Rotation angle on the X axis</param>
        /// <param name="aY">Rotation angle on the Y axis</param>
        /// <param name="aZ">Rotation angle on the Z axis</param>
        public static void RotateModel(ObjData objData, double aX, double aY, double aZ)
        {
            Matrix3D matrixX = Matrix3D.GetXRotationMatrix(AngleUtils.DegreeToRadian(aX));
            Matrix3D matrixY = Matrix3D.GetXRotationMatrix(AngleUtils.DegreeToRadian(aY));
            Matrix3D matrixZ = Matrix3D.GetXRotationMatrix(AngleUtils.DegreeToRadian(aZ));

            foreach (ObjectObj objectObj in objData.ObjectsList)
            {
                foreach (Point p in objectObj.VerticesList)
                {
                    p.MultiplicationMatrix(matrixX);
                    p.MultiplicationMatrix(matrixY);
                    p.MultiplicationMatrix(matrixZ);
                }
                foreach (Vector v in objectObj.NormalsList)
                {
                    v.MultiplicationMatrix(matrixX);
                    v.MultiplicationMatrix(matrixY);
                    v.MultiplicationMatrix(matrixZ);
                }
            }
        }

        /// <summary>
        /// Sort groups alphabetically
        /// </summary>
        /// <param name="objData">Data parsed from the obj file</param>
        public static bool SortGroups(ObjData objData)
        {
            foreach (ObjectObj objectObj in objData.ObjectsList)
                if (objectObj.GroupName == null) // if a groupName is null, sorting would throw an exception
                    return false;

            objData.ObjectsList.Sort((p, q) => p.GroupName.CompareTo((q.GroupName)));
            return true;
        }

        /// <summary>
        /// Sort materials alphabetically
        /// </summary>
        /// <param name="mtlData">Data parsed from the mtl file</param>
        public static bool SortMaterials(MtlData mtlData)
        {
            if (mtlData == null)
                return false;

            foreach (MaterialMtl materialMtl in mtlData.MaterialsList)
                if (materialMtl.NewMtl == null) // if a NewMtl is null, sorting would throw an exception
                    return false;

            mtlData.MaterialsList.Sort((p, q) => p.NewMtl.CompareTo((q.NewMtl)));
            return true;
        }

        /// <summary>
        /// Merge together groups that share a texture
        /// </summary>
        /// <param name="objData">Data parsed from the obj file</param>
        /// <param name="mtlData">Data parsed from the mtl file</param>
        public static bool MergeGroups(ObjData objData, MtlData mtlData)
        {
            if (mtlData == null)
                return false;

            var tupleTextureMaterials = MtlUtils.GetTupleDictTextureMaterials(mtlData);

            // Dictionary that associate every texture to a list of materials
            Dictionary<string, List<int>> dictTextureMaterials = tupleTextureMaterials.Item1;

            // List of materials without any texture
            List<int> untexturedMaterials = tupleTextureMaterials.Item2;

            // Dictionary that associate every material to a list of groups
            Dictionary<string, List<int>> dictMaterialGroups = ObjUtils.GetDictMaterialGroups(objData);

            // Dictionary that associate every texture to a list of groups
            Dictionary<string, List<int>> dictTextureGroups = ObjUtils.GetDictTextureGroups(objData, mtlData,
                dictTextureMaterials, dictMaterialGroups);
            
            List<ObjectObj> newObjectsList = new List<ObjectObj>();
            List<MaterialMtl> newMaterialsList = new List<MaterialMtl>();

            foreach (KeyValuePair<string, List<int>> keyValue in dictTextureGroups) // Textured groups
            {
                // Merge the groups
                ObjectObj objectObj = MergeObjects(objData, keyValue.Key, keyValue.Value);
                newObjectsList.Add(objectObj);

                if (dictTextureMaterials.TryGetValue(keyValue.Key, out List<int> materials))
                {
                    if (materials != null)
                    {
                        // Merge the materials
                        MaterialMtl materialMtl = MergeMaterials(mtlData, keyValue.Key, materials);
                        newMaterialsList.Add(materialMtl);
                    }
                }
            }

            foreach (int materialId in untexturedMaterials) // The untextured materials
            {
                MaterialMtl materialMtl = mtlData.MaterialsList[materialId];
                newMaterialsList.Add(materialMtl); // Insert material in the list
                if (dictMaterialGroups.TryGetValue(materialMtl.NewMtl, out List<int> groups))
                {
                    if (groups != null)
                    {
                        foreach (int groupId in groups)
                        {
                            ObjectObj objectObj = objData.ObjectsList[groupId];
                            newObjectsList.Add(objectObj); // Insert the object in the list
                        }
                    }
                }
            }
            objData.ObjectsList = newObjectsList; 
            mtlData.MaterialsList = newMaterialsList;
            return true;
        }
        
        /// <summary>
        /// Merge together every group in the array
        /// </summary>
        /// <param name="objData">Data parsed from the obj file</param>
        /// <param name="textureName">Path to the texture file</param>
        /// <param name="groups">List of groups index</param>
        /// <returns></returns>
        private static ObjectObj MergeObjects(ObjData objData, string textureName, List<int> groups)
        {
            int length = groups.Count;
            if (length >= 1) // Should always be the case
            {
                // If the texture name is a relative/absolute path to the file, we only keep the name
                string relativeTexture = System.IO.Path.GetFileName(textureName);

                ObjectObj objectObj = new ObjectObj(objData.ObjectsList[groups[0]])
                {
                    MaterialName = relativeTexture, // Materialname will be the same as the texture file
                    GroupName = relativeTexture,
                    ObjectName = relativeTexture,
                    TextureName = textureName
                };
                if (length == 1) // Only one group
                {
                    return objectObj;
                }
                else // More group to merge with the first one
                {
                    LocalIndexesObj localIndexes = new LocalIndexesObj
                    {
                        vIndex = objectObj.VerticesList.Count,
                        vtIndex = objectObj.UVsList.Count,
                        vnIndex = objectObj.NormalsList.Count
                    };

                    for (int i = 1; i < length; i++)
                    {
                        ObjectObj nextObjectObj = objData.ObjectsList[groups[i]];

                        foreach (Point p in nextObjectObj.VerticesList)
                            objectObj.VerticesList.Add(p);
                        foreach (UVCoordinates uv in nextObjectObj.UVsList)
                            objectObj.UVsList.Add(uv);
                        foreach (Vector v in nextObjectObj.NormalsList)
                            objectObj.NormalsList.Add(v);

                        foreach (VertIndexesObj vertIndexes in nextObjectObj.VertIndexesList)
                        {
                            foreach (VertIndexObj vertIndex in vertIndexes.VertIndexList)
                            {
                                if (vertIndex.V != null)
                                    vertIndex.V += localIndexes.vIndex;
                                if (vertIndex.Vt != null)
                                    vertIndex.Vt += localIndexes.vtIndex;
                                if (vertIndex.Vn != null)
                                    vertIndex.Vn += localIndexes.vnIndex;
                            }
                            objectObj.VertIndexesList.Add(vertIndexes);
                        }
                        localIndexes.vIndex += nextObjectObj.VerticesList.Count;
                        localIndexes.vtIndex += nextObjectObj.UVsList.Count;
                        localIndexes.vnIndex += nextObjectObj.NormalsList.Count;
                    }
                    return objectObj;
                }
            }
            return null;
        }

        /// <summary>
        /// Merge together every materials in the array
        /// </summary>
        /// <param name="mtlData">Data parsed from the mtl file</param>
        /// <param name="textureName">Path to the texture file</param>
        /// <param name="materials">List of materials index</param>
        /// <returns></returns>
        private static MaterialMtl MergeMaterials(MtlData mtlData, string textureName, List<int> materials)
        {
            if (materials.Count >= 1) // Should always be the case
            {
                string relativeTexture = System.IO.Path.GetFileName(textureName);

                MaterialMtl materialMtl = new MaterialMtl(mtlData.MaterialsList[materials[0]])
                {
                    NewMtl = relativeTexture // The material name will be the same as the texture file
                };
                return materialMtl;
            }
            return null;
        }

        /// <summary>
        /// Copy every texture used by the obj file
        /// </summary>
        /// <param name="mtlData">Data parsed from the mtl file</param>
        /// <param name="outputFolder">Where the textures will be copied</param>
        /// <param name="srcDir">Where the textures are located</param>
        /// <returns>True if successful</returns>
        public static bool CopyUsedTextures(MtlData mtlData, string outputFolder, string srcDir)
        {
            if (mtlData == null) 
                // No Mtl to read texture data
                return false;

            bool relative = false;
            if (srcDir == null)
            {
                // If a textureDirectory was not specified, the textures are found relative to the mtl file
                srcDir = System.IO.Path.GetDirectoryName(mtlData.FilePath);
                relative = true;
            }

            if (!System.IO.Directory.Exists(srcDir)) 
                // The source directory doesn't exist
                return false;

            if (!System.IO.Directory.Exists(outputFolder))
            {
                if (!FileUtilsShared.TryCreateDirectory(outputFolder)) 
                    // Impossible to create the destination directory
                    return false;
            }
            
            HashSet<string> texturesList = MtlUtils.GetListTextures(mtlData);
            foreach (string texture in texturesList)
            {
                string srcFile;
                string destFile = System.IO.Path.Combine(outputFolder, System.IO.Path.GetFileName(texture));

                if (relative) // The path to the texture is inside the material
                {
                    if (System.IO.Path.IsPathRooted(texture)) // Absolute path to the picture
                        srcFile = texture;   
                    else // Relative to the mtl
                        srcFile = System.IO.Path.Combine(srcDir, texture);
                }
                else // We find the texture in a given folder
                {
                    srcFile = System.IO.Path.Combine(srcDir, System.IO.Path.GetFileName(texture));
                }

                if (!destFile.Equals(srcFile)) // Destination file is not the Source file
                {
                    if (System.IO.File.Exists(srcFile))
                    {
                        if (System.IO.File.Exists(destFile)) // The destination file already exists, we need to delete it
                        {
                            if (!FileUtilsShared.TryDeleteFile(destFile))
                                continue; // Impossible to delete the file, we go to the next texture
                        }
                        FileUtilsShared.TryCopyFile(srcFile, destFile);
                    }
                }
                   
            }
            return true;
        }

        /// <summary>
        /// Try to delete all groups that use textures from a given list (compare the textures)
        /// </summary>
        /// <param name="objData">Data parsed from the obj file</param>
        /// <param name="mtlData">Data parsed from the mtl file</param>
        /// <param name="listFileName">List of textures that should be matched</param>
        /// <returns>true if successful, false if error</returns>
        public static bool TryDeleteMatchingGroups(ObjData objData, MtlData mtlData, List<string> listFileName)
        {
            try
            {
                return DeleteMatchingGroups(objData, mtlData, listFileName);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Delete all groups that use textures from a given list (compare the textures)
        /// </summary>
        /// <param name="objData">Data parsed from the obj file</param>
        /// <param name="mtlData">Data parsed from the mtl file</param>
        /// <param name="listFileName">List of textures that should be matched</param>
        public static bool DeleteMatchingGroups(ObjData objData, MtlData mtlData, List<string> listFileName)
        {
            if (mtlData == null)
                return false;

            List<BitmapStoreData> imgList = BitmapStoreData.GetListBitmapStoreData(listFileName);

            var tupleTextureMaterials = MtlUtils.GetTupleDictTextureMaterials(mtlData);

            // Dictionary that associate every texture to a list of materials
            Dictionary<string, List<int>> dictTextureMaterials = tupleTextureMaterials.Item1;
            // List of materials without any texture
            List<int> untexturedMaterials = tupleTextureMaterials.Item2;

            // Dictionary that associate every material to a list of groups
            Dictionary<string, List<int>> dictMaterialGroups = ObjUtils.GetDictMaterialGroups(objData);

            // Dictionary that associate every texture to a list of groups
            Dictionary<string, List<int>> dictTextureGroups = ObjUtils.GetDictTextureGroups(objData, mtlData,
                dictTextureMaterials, dictMaterialGroups);
            
            List<ObjectObj> newObjectsList = new List<ObjectObj>();
            List<MaterialMtl> newMaterialsList = new List<MaterialMtl>();

            string srcDir = System.IO.Path.GetDirectoryName(mtlData.FilePath);

            foreach (KeyValuePair<string, List<int>> keyValue in dictTextureGroups)
            {
                List<int> groups = keyValue.Value;
                if (groups != null)
                {
                    if (groups.Count >= 1) // Should always be the case
                    {
                        string texturePath = keyValue.Key;
                        
                        if (!System.IO.Path.IsPathRooted(texturePath)) // Not an absolute path
                            texturePath = System.IO.Path.Combine(srcDir, texturePath);
                        
                        System.Drawing.Bitmap img = ImageUtilsShared.CreateBitmap(texturePath);
                        if (img != null)
                        {
                            BitmapStoreData bmpData = new BitmapStoreData(img);
                            if (bmpData.BData != null)
                            {
                                if (!ImageUtilsShared.SamePictures(imgList, bmpData)) // Not the same image
                                {
                                    // We can keep all these groups and materials

                                    foreach (int groupId in groups)
                                    {
                                        ObjectObj objectObj = objData.ObjectsList[groupId];
                                        newObjectsList.Add(objectObj); // Insert the object in the list
                                    }

                                    if (dictTextureMaterials.TryGetValue(keyValue.Key, out List<int> materials))
                                    {
                                        if (materials != null)
                                        {
                                            foreach (int materialId in materials)
                                            {
                                                MaterialMtl materialMtl = mtlData.MaterialsList[materialId];
                                                newMaterialsList.Add(materialMtl); // Insert the material in the list
                                            }
                                        }
                                    }
                                }
                                bmpData.UnlockBits();
                            }
                        }
                    }
                }
            }
            
            foreach (int materialId in untexturedMaterials) // The untextured materials
            {
                MaterialMtl materialMtl = mtlData.MaterialsList[materialId];
                newMaterialsList.Add(materialMtl); // Insert material in the list
                if (dictMaterialGroups.TryGetValue(materialMtl.NewMtl, out List<int> groups))
                {
                    if (groups != null)
                    {
                        foreach (int groupId in groups)
                        {
                            ObjectObj objectObj = objData.ObjectsList[groupId];
                            newObjectsList.Add(objectObj); // Insert the object in the list
                        }
                    }
                }
            }
            BitmapStoreData.BitmapDataUnlock(imgList);

            objData.ObjectsList = newObjectsList;
            mtlData.MaterialsList = newMaterialsList;
            return true;
        }

        /// <summary>
        /// Merge the list of given ObjData into a single one and returns it
        /// </summary>
        /// <param name="objFiles"></param>
        /// <returns></returns>
        public static ObjData MergeObjFiles(List<ObjData> objFiles)
        {
            if (objFiles != null)
            {
                int length = objFiles.Count;
                if (length >= 1) // Should always be the case
                {
                    ObjData objData = objFiles[0];

                    if (length == 1) // Only one objData
                    {
                        return objData;
                    }
                    else // More objData to merge with the first one
                    {
                        IntWrapper indexMaterial = new IntWrapper(0);
                        // Rename every materials to be unique
                        SetUniqueMaterialsNames(objData, objData.Mtl, indexMaterial);

                        for (int i = 1; i < length; i++)
                        {
                            ObjData nextObjData = objFiles[i];
                            SetUniqueMaterialsNames(nextObjData, nextObjData.Mtl, indexMaterial);
                            objData.MergeObjData(nextObjData);

                        }
                        return objData;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Rename every material from the obj and mtl to make them unique after the merge
        /// </summary>
        /// <param name="objData"></param>
        /// <param name="mtlData"></param>
        /// <param name="indexMaterial"></param>
        private static void SetUniqueMaterialsNames(ObjData objData, MtlData mtlData, IntWrapper indexMaterial)
        {
            // Dictionary that associate every material to a list of groups
            Dictionary<string, List<int>> dictMaterialGroups = ObjUtils.GetDictMaterialGroups(objData);

            if (mtlData == null) // Need to rename the Materials in obj only
            {
                foreach (KeyValuePair<string, List<int>> keyValue in dictMaterialGroups)
                {
                    SetUniqueMaterialsName(objData, keyValue.Value, indexMaterial);
                    indexMaterial.Value++;
                }
            }
            else // Need to rename the Materials in both obj and mtl
            {
                // Dictionary that associate every material of the mtl file to their index
                Dictionary<string, int> dictMaterialIndex = MtlUtils.GetDictMaterialIndex(mtlData);

                foreach (KeyValuePair<string, List<int>> keyValue in dictMaterialGroups)
                {
                    SetUniqueMaterialsName(objData, keyValue.Value, indexMaterial);
                    
                    if (dictMaterialIndex.TryGetValue(keyValue.Key, out int index)) // We get the index of the material
                    {
                        MaterialMtl materialMtl = mtlData.MaterialsList[index];
                        if (materialMtl.NewMtl != null)
                        {
                            materialMtl.NewMtl = GenericUtils.MergeIndexStr(indexMaterial.Value, materialMtl.NewMtl);
                        }
                    }
                    indexMaterial.Value++;
                }
            }
        }

        /// <summary>
        /// Rename every materials from the given list to make them unique
        /// </summary>
        /// <param name="objData"></param>
        /// <param name="groups"></param>
        /// <param name="indexMaterial"></param>
        private static void SetUniqueMaterialsName(ObjData objData, List<int> groups, IntWrapper indexMaterial)
        {
            foreach (int i in groups)
            {
                ObjectObj objectObj = objData.ObjectsList[i];
                if (objectObj.ObjectName == null)
                {
                    objectObj.ObjectName = objectObj.GroupName;
                }
                if (objectObj.MaterialName != null)
                {
                    objectObj.MaterialName = GenericUtils.MergeIndexStr(indexMaterial.Value, objectObj.MaterialName);
                }
            }
        }

    }
}
