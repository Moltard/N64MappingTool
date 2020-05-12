using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using N64Library.Tool.Utils;

namespace N64Library.Tool.ObjFiles
{
    public class MtlParser
    {
        /// <summary>
        /// Parse the mtl used by the given obj
        /// </summary>
        /// <param name="objData">Data parsed from the obj file</param>
        /// <returns></returns>
        public static MtlData GetMtlData(ObjData objData)
        {
            if (objData != null)
            {
                string filePath = objData.FilePath;
                string mtlLib = objData.MtlLib;
                if (!string.IsNullOrEmpty(filePath) && !string.IsNullOrEmpty(mtlLib))
                {
                    string mtlFile = mtlLib;
                    if (!System.IO.Path.IsPathRooted(mtlFile)) // Not an absolute path to the mtl file
                    {
                        string directory = FileHelper.GetDirectoryName(filePath); // Directory of the obj
                        mtlFile = FileHelper.Combine(directory, mtlFile); // The mtl directory is based on the obj's
                    }
                    return ParseMtl(mtlFile);
                }
            }
            return null;
        }

        /// <summary>
        /// Parse the given mtl file
        /// </summary>
        /// <param name="file">File to parse</param>
        /// <returns></returns>
        public static MtlData ParseMtl(string file)
        {
            string[] lines = Helper.RemoveBlankSpaces(Helper.ReadFile(file)); // Read the file and remove blankspaces
            if (lines != null)
            {
                // Store all the other data to parse
                List<Tuple<string, string>> listKeysValues = GetListKeysValues(lines);

                MtlData mtlData = new MtlData(file);

                lines = null; // To let the garbage collector free the file from memory
                int i = 0;
                int length = listKeysValues.Count;

                while (i < length)
                {
                    Tuple<string, string> keyValue = listKeysValues[i];
                    if (keyValue != null) // Never null unless error or final line
                    {
                        if (MtlHelper.IsNewMtl(keyValue.Item1))
                        {
                            int materialStart = i;
                            int materialEnd = GetIndexEnd(listKeysValues, length, materialStart);

                            if (materialEnd != -1)
                            {
                                mtlData.MaterialsList.Add(MaterialData(listKeysValues, materialStart, materialEnd));
                                i = materialEnd; // Set to next material
                                continue;
                            }
                        }
                    }
                    i++; 
                }
                return mtlData;
            }
            return null;
        }

        private static MaterialMtl MaterialData(List<Tuple<string, string>> listKeysValues, int startIndex, int endIndex)
        {
            MaterialMtl materialMtl = new MaterialMtl();
            int i = startIndex;
            while (i < endIndex)
            {
                Tuple<string, string> keyValue = listKeysValues[i];
                if (keyValue != null)
                {
                    string key = keyValue.Item1;
                    string value = keyValue.Item2.TrimEnd('\0').Trim(); // Remove the null character and any blank space

                    double? tmpDouble = null;
                    int? tmpInt = null;

                    switch (key)
                    {
                        case "newmtl":
                            materialMtl.NewMtl = value;
                            break;
                        case "Kd":
                            materialMtl.Kd = new Color(value);
                            break;
                        case "Ks":
                            materialMtl.Ks = new Color(value);
                            break;
                        case "Ka":
                            materialMtl.Ka = new Color(value);
                            break;
                        case "Ke":
                            materialMtl.Ke = new Color(value);
                            break;
                        case "Ns":
                            tmpDouble = Helper.StringToDouble(value);
                            if (tmpDouble != null)
                                materialMtl.Ns = (double)tmpDouble;
                            break;
                        case "Ni":
                            tmpDouble = Helper.StringToDouble(value);
                            if (tmpDouble != null)
                                materialMtl.Ni = (double)tmpDouble;
                            break;
                        case "d":
                            tmpDouble = Helper.StringToDouble(value);
                            if (tmpDouble != null)
                                materialMtl.D = (double)tmpDouble;
                            break;
                        case "illum":
                            tmpInt = Helper.StringToInt(value);
                            if (tmpInt != null)
                                materialMtl.Illum = (int)tmpInt;
                            break;
                        case "map_Kd":
                            materialMtl.MapKd = value;
                            break;
                        case "map_Ks":
                            materialMtl.MapKs = value;
                            break;
                        case "map_Ka":
                            materialMtl.MapKa = value;
                            break;
                        case "map_Ke":
                            materialMtl.MapKe = value;
                            break;
                        case "map_d":
                            materialMtl.MapD = value;
                            break;
                        default:
                            break;
                    }
                }
                i++;
            }
            return materialMtl;
        }

        private static int GetIndexEnd(List<Tuple<string, string>> listKeysValues, int length, int indexStart)
        {
            int i = indexStart + 1; // We start at the line after the newmtl
            while (i < length)
            {
                Tuple<string, string> keyValue = listKeysValues[i];
                if (keyValue == null) // The only null KeyValue is the last one
                    return i;
                if (MtlHelper.IsNewMtl(keyValue.Item1))
                    return i;
                i++;
            }
            return -1;
        }

        private static List<Tuple<string, string>> GetListKeysValues(string[] lines)
        {
            int length = lines.Length;
            List<Tuple<string, string>> listKeysValues = new List<Tuple<string, string>>();
            for (int i = 0; i < length; i++) // Store the useful data in a key value list
            {
                string line = lines[i];
                if (!string.IsNullOrEmpty(line)) // If not empty line nor null
                {
                    Tuple<string, string> keyValue = Helper.GetKeyValueFromSplit(line.Trim());
                    if (keyValue != null)
                    {
                        listKeysValues.Add(keyValue);
                    }
                }
            }
            listKeysValues.Add(null); // Insert an empty line
            return listKeysValues;
        }
    }
}
