using N64Library.Tool.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N64Library.Tool.Utils
{
    internal static class MtlUtils
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

        public static string GetNewNewMtl(string value) { return "newmtl " + value; }
        public static string GetNewKa() { return "Ka 0.000000 0.000000 0.000000"; }
        public static string GetNewKd() { return "Kd 0.000000 0.000000 0.000000"; }
        public static string GetNewKs() { return "Ks 0.000000 0.000000 0.000000"; }
        public static string GetNewKe() { return "Ke 0.000000 0.000000 0.000000"; }
        public static string GetNewKa(string value) { return "Ka " + value; }
        public static string GetNewKd(string value) { return "Kd " + value; }
        public static string GetNewKs(string value) { return "Ks " + value; }
        public static string GetNewKe(string value) { return "Ke " + value; }
        public static string GetNewNs(double value) { return $"Ns {value:0.000000}"; }
        public static string GetNewNi(double value) { return $"Ni {value:0.000000}"; }
        public static string GetNewD(double value) { return $"d {value:0.000000}"; }
        public static string GetNewIllum(int value) { return "illum " + value; }
        public static string GetNewMapKd(string value) { return "map_Kd " + value; }
        public static string GetNewMapKa(string value) { return "map_Ka " + value; }
        public static string GetNewMapKs(string value) { return "map_Ks " + value; }
        public static string GetNewMapKe(string value) { return "map_Ke " + value; }
        public static string GetNewMapD(string value) { return "map_d " + value; }

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
            {
                diffuse = "0.750000 0.750000 0.750000";
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("newmtl ").AppendLine(materialName);
            sb.AppendLine("Ns 0.000000");
            sb.AppendLine("Ka 0.000000 0.000000 0.000000");
            sb.Append("Kd ").AppendLine(diffuse);
            sb.AppendLine("Ks 0.000000 0.000000 0.000000");
            sb.AppendLine("Ni 1.000000");
            sb.AppendLine("d 1.000000");
            sb.AppendLine("illum 2");
            if (textureFile != null)
            {
                sb.Append("map_Kd ").AppendLine(textureFile);
            }
            return sb.ToString();
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
        public static Tuple<Dictionary<string, List<int>>, List<int>> GetTupleDictTextureMaterials(MtlData mtlData)
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
