using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using N64Library.Tool.ObjFiles;

namespace N64Library.Tool.Utils
{
    public class ObjHelper
    {

        private static readonly string credits =
           "# -----------------" + Helper.newLine +
           "# N64 Mapping Tool" + Helper.newLine +
           "#    By Moltard" + Helper.newLine +
           "# -----------------";


        public static string GetCreditsFile() { return credits; }
        
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

        
        public static string GetObjectName(int idx) {
            return GetObjectName(idx, "Object");
        }
        public static string GetObjectName(int idx, string name) {
            return Helper.MergeIndexStr(idx, name);
        }

        public static string GetGroupName(int idx) {
            return GetGroupName(idx, "default");
        }
        public static string GetGroupName(int idx, string name) {
            return Helper.MergeIndexStr(idx, name);
        }


        public static string GetMtlLib(string filename) { return string.Format("mtllib {0}", filename); }
        
        public static string GetNewObject(string objectName) { return string.Format("o {0}", objectName); }
        public static string GetNewGroup(string groupName) { return string.Format("g {0}", groupName); }

        public static string GetNewCoord(string coordStr) { return string.Format("v {0}", coordStr); }  // Coord line
        public static string GetNewTexCoord(string texCoordStr) { return string.Format("vt {0}", texCoordStr); } // TexCoord line
        public static string GetNewVertNormal(string vertNormStr) { return string.Format("vn {0}", vertNormStr); } // VertNorm line

        public static string GetNewUseMtl(string materialName) { return string.Format("usemtl {0}", materialName); } // Material line

        public static string GetNewSmoothGroup() { return string.Format("s off"); }
        public static string GetNewSmoothGroup(int smoothGroup) { return string.Format("s {0}", smoothGroup); }  // Smoothing group line
        public static string GetNewSmoothGroup(string smoothGroup) {
            if (smoothGroup == null) return GetNewSmoothGroup();
            return string.Format("s {0}", smoothGroup);
        }
        
        public static string GetNewF(string strIndex) { return string.Format("f {0}", strIndex); } // f line

     
        /// <summary>
        /// Split each vertice data (1/1/1 2/2/2 3/3/3)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string[] SplitVertData(string value)
        {
            return Helper.SplitBySpace(value);
        }

        /// <summary>
        /// Split each index (1/1/1)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string[] SplitIndexes(string value)
        {
            return Helper.SplitBySlash(value);
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

    public class MtlHelper
    {
        public static bool IsNewMtl(string key) { return key == "newmtl"; }
        public static bool IsNs(string key) { return key == "Ns"; }
        public static bool IsKa(string key) { return key == "Ka"; }
        public static bool IsKd(string key) { return key == "Kd"; }
        public static bool IsKs(string key) { return key == "Ks"; }
        public static bool IsNi(string key) { return key == "Ni"; }
        public static bool IsD(string key) { return key == "d"; }
        public static bool IsIllum(string key) { return key == "illum"; }
        public static bool IsMapKd(string key) { return key == "map_Kd"; }
        public static bool IsMapKs(string key) { return key == "map_Ks"; }
        public static bool IsMapKa(string key) { return key == "map_Ka"; }
        public static bool IsMapD(string key) { return key == "map_d"; }

        private static string GetDefaultColor() { return "0.000000 0.000000 0.000000"; }

        public static string GetNewNewMtl(string materialName) { return string.Format("newmtl {0}", materialName); }
        public static string GetNewKa() { return string.Format("Ka {0}", GetDefaultColor()); }
        public static string GetNewKa(string ka) { return string.Format("Ka {0}", ka); }
        public static string GetNewKd() { return string.Format("Kd {0}", GetDefaultColor()); }
        public static string GetNewKd(string kd) { return string.Format("Kd {0}", kd); }
        public static string GetNewKs() { return string.Format("Ks {0}", GetDefaultColor()); }
        public static string GetNewKs(string ks) { return string.Format("Ks {0}", ks); }
        public static string GetNewKe() { return string.Format("Ke {0}", GetDefaultColor()); }
        public static string GetNewKe(string ke) { return string.Format("Ke {0}", ke); }

        public static string GetNewNs(double ns) { return string.Format("Ns {0:0.000000}", ns); }
        public static string GetNewNi(double ni) { return string.Format("Ni {0:0.000000}", ni); }
        public static string GetNewD(double d) { return string.Format("d {0:0.000000}", d); }
        public static string GetNewIllum(int illum) { return string.Format("illum {0}", illum); }
        public static string GetNewMapKd(string mapKd) { return string.Format("map_Kd {0}", mapKd); }
        public static string GetNewMapKa(string mapKa) { return string.Format("map_Ka {0}", mapKa); }
        public static string GetNewMapKs(string mapKs) { return string.Format("map_Ks {0}", mapKs); }
        public static string GetNewMapKe(string mapKe) { return string.Format("map_Ke {0}", mapKe); }
        public static string GetNewMapD(string mapd) { return string.Format("map_d {0}", mapd); }

        /// <summary>
        /// Return the string to be inserted in the .mtl file using the given parameters
        /// </summary>
        /// <param name="materialName"></param>
        /// <param name="textureFile"></param>
        /// <param name="diffuse"></param>
        /// <returns></returns>
        public static string GetMtlData(string materialName, string textureFile = null, string diffuse = null)
        {
            if (diffuse == null) // the Kd only matters for the 3D software, it changes the lighting
                diffuse = "0.750000 0.750000 0.750000";

            string mapKd = (textureFile == null) ? "" : GetNewMapKd(textureFile) + Helper.newLine;

            return string.Format("newmtl {0}" + Helper.newLine +
                            "Ns 0.000000" + Helper.newLine +
                            "Ka 0.000000 0.000000 0.000000" + Helper.newLine +
                            "Kd {1}" + Helper.newLine +
                            "Ks 0.000000 0.000000 0.000000" + Helper.newLine +
                            "Ni 1.000000" + Helper.newLine +
                            "d 1.000000" + Helper.newLine +
                            "illum 2" + Helper.newLine +
                            "{2}", materialName, diffuse, mapKd);
        }

        /// <summary>
        /// Return a dictionary with each Material associated to their indexes in the materials list
        /// </summary>
        /// <param name="mtlData"></param>
        /// <returns></returns>
        public static Dictionary<string, int> GetDictMaterialIndex(MtlData mtlData)
        {
            var dict = new Dictionary<string, int>();

            int i = 0;
            foreach (MaterialMtl materialMtl in mtlData.MaterialsList)
            {
                if (materialMtl.NewMtl != null)
                {
                    if (!dict.ContainsKey(materialMtl.NewMtl)) // The material wasn't added yet
                    {
                        dict.Add(materialMtl.NewMtl, i);
                    }
                }
                i++;
            }
            return dict;
        }

        /// <summary>
        /// Return a dictionary with each Material associated to a Texture
        /// </summary>
        /// <param name="mtlData"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetDictMaterialTexture(MtlData mtlData)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            foreach (MaterialMtl materialMtl in mtlData.MaterialsList)
            {
                if (materialMtl.NewMtl != null && materialMtl.MapKd != null)
                    dict.Add(materialMtl.NewMtl, materialMtl.MapKd);
            }

            return dict;
        }

        /// <summary>
        /// Return a Hash List of all Textures used in the mtl
        /// </summary>
        /// <param name="mtlData"></param>
        /// <returns></returns>
        public static HashSet<string> GetListTextures(MtlData mtlData)
        {
            HashSet<string> textureList = new HashSet<string>();
            foreach (MaterialMtl materialMtl in mtlData.MaterialsList)
            {
                if (materialMtl.NewMtl != null && materialMtl.MapKd != null)
                    textureList.Add(materialMtl.MapKd);
            }

            return textureList;
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

        /// <summary>
        /// Return a tuple with a dictionary of each Texture associated to the indexes of their Materials 
        /// and a list of Materials without any texture
        /// </summary>
        /// <param name="mtlData"></param>
        /// <returns></returns>
        public static Tuple<Dictionary<string, List<int>>,List<int>> GetTupleDictTextureMaterials(MtlData mtlData)
        {
            Dictionary<string, List<int>> dict = new Dictionary<string, List<int>>();
            List<int> untexturedMaterials = new List<int>();

            int i = 0;
            foreach (MaterialMtl materialMtl in mtlData.MaterialsList)
            {
                if (materialMtl.NewMtl != null)
                {
                    if (materialMtl.MapKd != null)
                    {
                        if (dict.TryGetValue(materialMtl.MapKd, out List<int> materials))
                        {
                            materials.Add(i); // If the keyvalue already exists, we add the new index
                        }
                        else
                        {
                            dict.Add(materialMtl.MapKd, new List<int>() { i }); // Otherwise, we create the new keyvalue
                        }
                    }
                    else // Material without any texture
                    {
                        untexturedMaterials.Add(i);
                    }
                    
                }
                i++;
            }
            return new Tuple<Dictionary<string, List<int>>, List<int>>(dict, untexturedMaterials);
        }

    }
}
