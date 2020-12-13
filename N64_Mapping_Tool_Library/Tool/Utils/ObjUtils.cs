using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using N64Library.Tool.Data;

namespace N64Library.Tool.Utils
{
    internal static class ObjUtils
    {
        
        public static bool IsComment(string key) { return key == "#"; }
        public static bool IsMtlLib(string key) { return key == "mtllib"; }
        public static bool IsObject(string key) { return key == "o"; }
        public static bool IsGroup(string key) { return key == "g"; }
        public static bool IsMaterial(string key) { return key == "usemtl"; }
        public static bool IsSmooth(string key) { return key == "s"; }
        public static bool IsVertice(string key) { return key == "v"; }
        public static bool IsTexCoord(string key) { return key == "vt"; }
        public static bool IsVertNormal(string key) { return key == "vn"; }
        public static bool IsVertData(string key) { return key == "f"; }
        
        public static string GetObjectName(int idx) {  return GetObjectName(idx, "Object"); }
        public static string GetObjectName(int idx, string name) { return GenericUtils.MergeIndexStr(idx, name); }
        public static string GetGroupName(int idx) { return GetGroupName(idx, "default"); }
        public static string GetGroupName(int idx, string name) { return GenericUtils.MergeIndexStr(idx, name); }
        
        public static string GetNewMtlLib(string value) { return "mtllib " + value; }   // Mtllib line
        public static string GetNewObject(string value) { return "o " + value; } // Object line
        public static string GetNewGroup(string value) { return "g " + value; } // Group line
        public static string GetNewCoord(string value) { return "v " + value; } // Coord line
        public static string GetNewTexCoord(string value) { return "vt " + value; } // TexCoord line
        public static string GetNewVertNormal(string value) { return "vn " + value; } // VertNorm line
        public static string GetNewUseMtl(string value) { return "usemtl " + value; } // Material line
        public static string GetNewF(string value) { return "f " + value; } // f line
        public static string GetNewSmoothGroup() { return "s off"; } // Smoothing group line
        public static string GetNewSmoothGroup(int value) { return "s " + value; }  
        
        /// <summary>
        /// Split each vertice data (1/1/1 2/2/2 3/3/3)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string[] SplitVertData(string value)
        {
            return GenericUtils.SplitBySpace(value);
        }

        /// <summary>
        /// Split each index (1/1/1)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string[] SplitIndexes(string value)
        {
            return GenericUtils.SplitBySlash(value);
        }
        
        /// <summary>
        /// Return a dictionary with each Material associated to the indexes of their Group
        /// </summary>
        /// <param name="objData"></param>
        /// <returns></returns>
        public static Dictionary<string, List<int>> GetDictMaterialGroups(ObjData objData)
        {
            var dict = new Dictionary<string, List<int>>();

            int i = 0;
            foreach (ObjectObj objectObj in objData.ObjectsList)
            {
                if (objectObj.MaterialName != null)
                {
                    if (dict.TryGetValue(objectObj.MaterialName, out List<int> groups))
                    {
                        groups.Add(i); // If the keyvalue already exists, we add the new index
                    }
                    else
                    {
                        dict.Add(objectObj.MaterialName, new List<int>() { i }); // Otherwise, we create the new keyvalue
                    }
                }
                i++;
            }
            return dict;
        }

        /// <summary>
        /// Return the join of the TextureMaterial and MaterialGroup dictionaries
        /// </summary>
        /// <param name="objData"></param>
        /// <returns></returns>
        public static Dictionary<string, List<int>> GetDictTextureGroups(ObjData objData, MtlData mtlData, 
            Dictionary<string, List<int>> dictTextureMaterials, Dictionary<string, List<int>> dictMaterialGroups)
        {
            Dictionary<string, List<int>> dictTextureGroups = new Dictionary<string, List<int>>();

            foreach (KeyValuePair<string, List<int>> textureMaterial in dictTextureMaterials)
            {
                List<int> groupIdList = new List<int>();
                foreach (int materialId in textureMaterial.Value)
                {
                    string materialName = mtlData.MaterialsList[materialId].NewMtl;

                    if (dictMaterialGroups.TryGetValue(materialName, out List<int> groups))
                    {
                        if (groups != null)
                        {
                            foreach (int groupId in groups)
                                groupIdList.Add(groupId);
                        }
                    }
                }
                dictTextureGroups.Add(textureMaterial.Key, groupIdList); // The texture is associated a list of group
            }
            return dictTextureGroups;
        }

        /// <summary>
        /// Get the Texture associated to the Material name, null if it doesn't exist
        /// </summary>
        /// <param name="map"></param>
        /// <param name="material"></param>
        /// <returns></returns>
        public static string GetTextureFromMaterial(Dictionary<string, string> dict, string material)
        {
            if (dict.TryGetValue(material, out string texture))
            {
                return texture;
            }
            return null;
        }
        
    }
    
}
