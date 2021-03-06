﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using N64Library.Tool.Data;
using N64Library.Tool.Utils;

namespace N64Library.Tool.Exporter
{
    public static class ObjExporter
    {
        /// <summary>
        /// Create the obj and mtl files with the given data
        /// </summary>
        /// <param name="objData">Data parsed from the obj file</param>
        /// <param name="mtlData">Data parsed from the mtl file</param>
        /// <param name="objFilename">Output path</param>
        /// <param name="makeMtl">Create a mtl file</param>
        /// <param name="useExistingMtl">Use the mtlData to create the mtl file</param>
        /// <returns></returns>
        public static bool WriteObj(ObjData objData, MtlData mtlData, string objFilename, bool makeMtl, bool useExistingMtl)
        {
            // objFilename has the .obj extension
            string directory = System.IO.Path.GetDirectoryName(objFilename);
            string noExtension = System.IO.Path.GetFileNameWithoutExtension(objFilename);
            string mtlRelative = string.Format("{0}.mtl", noExtension);
            string mtlFilename = System.IO.Path.Combine(directory, mtlRelative);

            List<string> mtlLines = new List<string>(); // To store the lines to append to the mtl file

            try
            {
                using (StreamWriter obj = new StreamWriter(objFilename))
                {
                    // mtllib, o, v, vt, g, usemtl, s, f

                    obj.WriteLine(GenericUtils.GetCreditsFile());
                    if (makeMtl)
                        obj.WriteLine(ObjUtils.GetNewMtlLib(mtlRelative));

                    LocalIndexesObj indexesObj = new LocalIndexesObj();

                    foreach (ObjectObj objectObj in objData.ObjectsList)
                    {
                        if (objectObj.ObjectName != null)
                            obj.WriteLine(ObjUtils.GetNewObject(objectObj.ObjectName)); // o

                        foreach (Point p in objectObj.VerticesList)
                            obj.WriteLine(ObjUtils.GetNewCoord(p.ToString())); // v

                        foreach (UVCoordinates uv in objectObj.UVsList)
                            obj.WriteLine(ObjUtils.GetNewTexCoord(uv.ToString())); // vt

                        foreach (Vector v in objectObj.NormalsList)
                            obj.WriteLine(ObjUtils.GetNewVertNormal(v.ToString())); // vn

                        if (objectObj.GroupName != null)
                            obj.WriteLine(ObjUtils.GetNewGroup(objectObj.GroupName)); // g

                        if (objectObj.MaterialName != null)
                        {
                            obj.WriteLine(ObjUtils.GetNewUseMtl(objectObj.MaterialName)); // usemtl
                            // Store lines in the mtl array to append to the file later
                            mtlLines.Add(MtlUtils.GetMtlData(objectObj.MaterialName, objectObj.TextureName));
                        }

                        if (objectObj.Smooth == -1)
                            obj.WriteLine(ObjUtils.GetNewSmoothGroup()); // s
                        else
                            obj.WriteLine(ObjUtils.GetNewSmoothGroup(objectObj.Smooth)); // s

                        foreach (VertIndexesObj vertIndexes in objectObj.VertIndexesList)
                            obj.WriteLine(ObjUtils.GetNewF(GetDataF(vertIndexes, indexesObj))); // f
                        
                        indexesObj.vIndex += objectObj.VerticesList.Count;
                        indexesObj.vtIndex += objectObj.UVsList.Count;
                        indexesObj.vnIndex += objectObj.NormalsList.Count;
                    }

                }
            }
            catch
            {
                return false;
            }

            if (makeMtl) // If we dont delete materials
            {
                if (useExistingMtl && mtlData != null) // Use the parsed mtl
                {
                    return MtlExporter.WriteMtl(mtlData, mtlFilename);
                }
                else // Create a mtl from the data obtained in the obj
                {
                    return MtlExporter.WriteMtl(mtlLines, mtlFilename);
                }
            }

            return true;
        }

        /// <summary>
        /// Return the f line string (i.e. 1/1/1 2/2/2 3/3/3)
        /// </summary>
        /// <param name="vertIndexes"></param>
        /// <param name="indexesObj"></param>
        /// <returns></returns>
        private static string GetDataF(VertIndexesObj vertIndexes, LocalIndexesObj indexesObj)
        {
            const string strSpace = " ";
            string[] strIndexesF = new string[vertIndexes.VertIndexList.Count];
            int i = 0;
            foreach (VertIndexObj vertIndex in vertIndexes.VertIndexList)
            {
                int? V = vertIndex.V;
                int? Vt = vertIndex.Vt;
                int? Vn = vertIndex.Vn;
                if (V != null)
                    V = V + 1 + indexesObj.vIndex;
                if (Vt != null)
                    Vt = Vt + 1 + indexesObj.vtIndex;
                if (Vn != null)
                    Vn = Vn + 1 + indexesObj.vnIndex;

                strIndexesF[i] = GetIndexesF(V, Vt, Vn); // Add the 1/1/1
                i++;
            }
            return string.Join(strSpace, strIndexesF); 
        }

        /// <summary>
        /// Return one of the f line indexes (i.e. 1/1/1)
        /// </summary>
        /// <param name="V"></param>
        /// <param name="Vt"></param>
        /// <param name="Vn"></param>
        /// <returns></returns>
        private static string GetIndexesF(int? V, int? Vt, int? Vn)
        {
            if (Vn == null)
            {
                if (Vt == null)
                    return string.Format("{0}", V);
                else
                    return string.Format("{0}/{1}", V, Vt);
            }
            else 
            {
                if (Vt == null)
                    return string.Format("{0}//{1}", V, Vn);
                else
                    return string.Format("{0}/{1}/{2}", V, Vt, Vn);
            }
        }
        
    }

}
