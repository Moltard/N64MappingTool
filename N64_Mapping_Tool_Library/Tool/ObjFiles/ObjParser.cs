using N64Library.Tool.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N64Library.Tool.ObjFiles
{
    public class ObjParser
    {
        private delegate bool IsObjectGroup(string line);
        private static IsObjectGroup isObjectGroup;
        
        /// <summary>
        /// Parse a given list of obj file
        /// </summary>
        /// <param name="fileList"></param>
        /// <param name="parseMtl"></param>
        /// <returns></returns>
        public static List<ObjData> ParseObjs(List<string> fileList, bool parseMtl = true)
        {
            List<ObjData> objDatas = new List<ObjData>();
            foreach(string file in fileList)
            {
                ObjData objData = ParseObj(file, parseMtl);
                if (objData != null)
                    objDatas.Add(objData);
            }

            return objDatas;
        }

        /// <summary>
        /// Parse the obj file to make the data useable
        /// </summary>
        /// <param name="file">File to parse</param>
        /// <param name="parseMtl">Parse the mtl file directly</param>
        /// <returns></returns>
        public static ObjData ParseObj(string file, bool parseMtl = true)
        {
            string[] lines = Helper.RemoveBlankSpaces(Helper.ReadFile(file)); // Read the file and remove blankspaces
            if (lines != null)
            {
                UpdateParseLineGroup(lines);
                     
                AllVerticesDataObj verticesData = new AllVerticesDataObj(); // To store the vertices data

                // Store all the other data to parse
                List<Tuple<string, string>> listKeysValues = GetListKeysValues(lines, verticesData);

                ObjData objData = new ObjData(file, verticesData.MtlLib);

                lines = null; // To let the garbage collector free the file from memory
                int i = 0;
                int length = listKeysValues.Count;
                while (i < length)
                {
                    Tuple<string, string> keyValue = listKeysValues[i];
                    if (keyValue != null)
                    {
                        if (isObjectGroup(keyValue.Item1))
                        {
                            int objectStart = i;
                            int objectEnd = GetIndexEnd(listKeysValues, length, objectStart);

                            if (objectEnd != -1)
                            {
                                objData.ObjectsList.Add(ObjectData(listKeysValues, verticesData, objectStart, objectEnd));
                                i = objectEnd; // Set to next object
                                continue;
                            } 
                        }
                    }
                    i++; // Unless error or final line
                }
                if (parseMtl)
                    objData.UpdateMtlData();

                return objData;
            }
            return null;
        }


        /// <summary>
        /// Return one object from the obj file
        /// </summary>
        /// <param name="listKeysValues"></param>
        /// <param name="allVertices"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        private static ObjectObj ObjectData(List<Tuple<string, string>> listKeysValues, AllVerticesDataObj allVertices, 
                                                            int startIndex, int endIndex)
        {
            LocalIndexesObj indexesObj = new LocalIndexesObj();
            ObjectObj objectObj = new ObjectObj();
            int i = startIndex;
            while (i < endIndex)
            {
                Tuple<string, string> keyValue = listKeysValues[i];
                if (keyValue != null)
                {
                    string key = keyValue.Item1;
                    string value = keyValue.Item2.TrimEnd('\0').Trim(); // Remove the null character and any blank space
                    if (ObjHelper.IsVertData(key)) // f
                    {
                        VerticeData(value, objectObj, allVertices, indexesObj);
                    }
                    else // Other key (o, g, s, usemtl)
                    {
                        switch (key)
                        {
                            case "usemtl":
                                objectObj.MaterialName = value;
                                break;
                            case "o":
                                objectObj.ObjectName = value;
                                break;
                            case "g":
                                objectObj.GroupName = value;
                                break;
                            case "s":
                                if (value != "off")
                                {
                                    int? s = Helper.StringToInt(value);
                                    if (s != null)
                                        objectObj.Smooth = (int)s;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
                i++;
            }
            return objectObj;
        }

        /// <summary>
        /// Store the f, v, vt and vn lines into the object
        /// </summary>
        /// <param name="value"></param>
        /// <param name="objectObj"></param>
        /// <param name="allVertices"></param>
        /// <param name="indexesObj"></param>
        private static void VerticeData(string value, ObjectObj objectObj, AllVerticesDataObj allVertices, LocalIndexesObj indexesObj)
        {
            VertIndexesObj vertIndexes = new VertIndexesObj();

            foreach (string indexes in ObjHelper.SplitVertData(value))
            {
                VertIndexObj vertIndex = new VertIndexObj();

                string[] indexList = ObjHelper.SplitIndexes(indexes);

                int length = indexList.Length;

                for (int i = 0; i < length; i++)
                {
                    int index = 0;
                    if (indexList[i] != "") // the vt line can be empty
                    {
                        int? tmpIndex = Helper.StringToInt(indexList[i]); // Get the index
                        if (tmpIndex != null)
                            index = (int)tmpIndex;
                    }
                    
                    if (i == 0) // v
                    {
                        if (index != 0)
                        {
                            vertIndex.V = indexesObj.vIndex;
                            string vLine = allVertices.GetVIndex(index - 1); // Obj index start at 1
                            if (vLine != null)
                            {
                                objectObj.VerticesList.Add(new Point(vLine));
                                indexesObj.vIndex++;
                            }
                        }
                    }
                    else if (i == 1) // vt
                    {
                        if (index != 0)
                        {
                            vertIndex.Vt = indexesObj.vtIndex;
                            string vtLine = allVertices.GetVtIndex(index - 1); // Obj index start at 1
                            if (vtLine != null)
                            {
                                objectObj.UVsList.Add(new UVCoordinates(vtLine));
                                indexesObj.vtIndex++;
                            }
                        }
                    }
                    else if (i == 2) // vn
                    {
                        if (index != 0)
                        {
                            vertIndex.Vn = indexesObj.vnIndex;
                            string vnLine = allVertices.GetVnIndex(index - 1); // Obj index start at 1
                            if (vnLine != null)
                            {
                                objectObj.NormalsList.Add(new Vector(vnLine));
                                indexesObj.vnIndex++;
                            }
                        }
                    }
                }
                vertIndexes.VertIndexList.Add(vertIndex);
            }
            objectObj.VertIndexesList.Add(vertIndexes);
        }
        

        /// <summary>
        /// Return the index of the line that start the next Object o/Group g
        /// </summary>
        /// <param name="indexStart"></param>
        /// <param name="bl_useGroup"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private static int GetIndexEnd(List<Tuple<string, string>> listKeysValues, int length, int indexStart)
        {
            int i = indexStart + 1; // We start at the line after the o/g
            while (i < length)
            {
                Tuple<string, string> keyValue = listKeysValues[i];
                if (keyValue == null) // The only null KeyValue is the last one
                    return i;
                if (isObjectGroup(keyValue.Item1))
                    return i; // We found the next 'o' / 'g'
                i++;
            }
            return -1;
        }


        /// <summary>
        /// Check if the parser will look for 'o', 'g' or 'usemtl' to separate each group
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        private static void UpdateParseLineGroup(string[] lines)
        {
            int length = lines.Length;
            for (int i = 0; i < length; i++)
            {
                if (lines[i].StartsWith("o ")) // We find the line of the first object "o"
                { 
                    isObjectGroup = ObjHelper.IsObject;
                    break;
                }
                else if (lines[i].StartsWith("g ")) // If there is no "o", we find the first "g"
                {
                    isObjectGroup = ObjHelper.IsGroup;
                    break;
                }
                else if (lines[i].StartsWith("usemtl ")) // A material could also be on top of everything
                {
                    isObjectGroup = ObjHelper.IsMaterial;
                    break;
                }
            }
        }

        /// <summary>
        /// Store every line of the file as a set of key and value for easier access
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="verticesData"></param>
        /// <returns></returns>
        private static List<Tuple<string, string>> GetListKeysValues(string[] lines, AllVerticesDataObj verticesData)
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
                        string key = keyValue.Item1;
                        string value = keyValue.Item2.TrimEnd('\0').Trim(); // Remove the null character and any blank space 

                        // Remove the null character and any blank space
                        if (key == "mtllib")
                            verticesData.MtlLib = value;
                        else if (key == "v")
                            verticesData.V.Add(value);
                        else if (key == "vt")
                            verticesData.Vt.Add(value);
                        else if (key == "vn")
                            verticesData.Vn.Add(value);
                        else
                            listKeysValues.Add(keyValue);
                    }
                }
            }
            listKeysValues.Add(null); // Insert an empty line
            return listKeysValues;
        }
    }
    
    /// <summary>
    /// Store all the v, vt and vn lines from the file to make parsing easier
    /// </summary>
    class AllVerticesDataObj
    {
        public string MtlLib { get; set; }
        public List<string> V { get; set; } 
        public List<string> Vt { get; set; }
        public List<string> Vn { get; set; }

        public AllVerticesDataObj()
        {
            MtlLib = null;
            V = new List<string>();
            Vt = new List<string>();
            Vn = new List<string>();
        }

        public string GetVIndex(int index)
        {
            if (index < V.Count)
                return V[index];
            return null;
        }
        public string GetVtIndex(int index)
        {
            if (index < Vt.Count)
                return Vt[index];
            return null;
        }
        public string GetVnIndex(int index)
        {
            if (index < Vn.Count)
                return Vn[index];
            return null;
        }
    }
}
