using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using N64Library.Tool.Data;
using N64Library.Tool.Utils;

namespace N64Library.Tool.Exporter
{
    public static class WrlExporter
    {
        /// <summary>
        /// Create the obj and mtl files with the given data
        /// </summary>
        /// <param name="wrlData">Data parsed from the wrl file</param>
        /// <param name="objFilename">Output file</param>
        /// <returns></returns>
        public static bool WriteObj(WrlData wrlData, string objFilename)
        {
            // objFilename has the .obj extension
            string directory = System.IO.Path.GetDirectoryName(objFilename);
            string noExtension = System.IO.Path.GetFileNameWithoutExtension(objFilename);
            string mtlRelative = $"{noExtension}.mtl";
            string mtlFilename = System.IO.Path.Combine(directory, mtlRelative);

            List<string> mtlLines = new List<string>(); // To store the lines to append to the mtl file

            try
            {
                using (StreamWriter obj = new StreamWriter(objFilename))
                {
                    // mtllib, o, v, vt, g, usemtl, s, f

                    obj.WriteLine(GenericUtils.GetCreditsFile());
                    obj.WriteLine(ObjUtils.GetNewMtlLib(mtlRelative));

                    int i_coordIndex = 0;
                    int i_texCoordIndex = 0;

                    int index = 0;

                    foreach(Transform transform in wrlData.TransformsList)
                    {
                        foreach (Shape shape in transform.ShapesList)
                        {
                            string groupName = "";
                            string urlTexture = null;
                            string diffuseColor = null;

                            if (shape.Appearance != null)
                            {
                                if (shape.Appearance.Material != null)
                                    if (shape.Appearance.Material.DiffuseColor != null)
                                        diffuseColor = shape.Appearance.Material.DiffuseColor.ToString();

                                if (shape.Appearance.Texture != null)
                                {
                                    urlTexture = shape.Appearance.Texture.Url;
                                    if (urlTexture != null)
                                        groupName = ObjUtils.GetGroupName(index, urlTexture);
                                    else
                                        groupName = ObjUtils.GetGroupName(index);
                                }
                                else
                                    groupName = ObjUtils.GetGroupName(index);
                            }

                            mtlLines.Add(MtlUtils.GetMtlData(groupName, urlTexture, diffuseColor));

                            obj.WriteLine(ObjUtils.GetNewObject(groupName)); // o


                            int sizeCoord = 0; int sizeTexCoord = 0;

                            if (shape.Geometry != null)
                            {
                                if (shape.Geometry.Coord != null)
                                {
                                    sizeCoord = shape.Geometry.Coord.PointsList.Count;
                                    foreach (Point point in shape.Geometry.Coord.PointsList)
                                        obj.WriteLine(ObjUtils.GetNewCoord(point.ToString())); // v
                                }

                                if (shape.Geometry.TexCoord != null)
                                {
                                    sizeTexCoord = shape.Geometry.TexCoord.UVList.Count;
                                    foreach (UVCoordinates uv in shape.Geometry.TexCoord.UVList)
                                        obj.WriteLine(ObjUtils.GetNewTexCoord(uv.ToString())); // vt
                                }
                            }

                            obj.WriteLine(ObjUtils.GetNewGroup(groupName)); // g
                            obj.WriteLine(ObjUtils.GetNewUseMtl(groupName)); // usemtl
                            obj.WriteLine(ObjUtils.GetNewSmoothGroup()); // s

                            if (shape.Geometry != null)
                            {
                                CoordIndexesWrl coordIndexes = shape.Geometry.CoordIndexes;
                                CoordIndexesWrl texCoordIndexes = shape.Geometry.TexCoordIndexes;

                                if (coordIndexes != null)
                                {
                                    List<CoordIndexWrl> coordIndexesList = coordIndexes.IndexesList;
                                    if (texCoordIndexes != null)
                                    {
                                        List<CoordIndexWrl> texCoordIndexesList = texCoordIndexes.IndexesList;
                                        for (int i = 0; i < coordIndexesList.Count; i++)
                                        {
                                            string strIndexesF = GetIndexesF(coordIndexesList[i], texCoordIndexesList[i], i_coordIndex, i_texCoordIndex);
                                            obj.WriteLine(ObjUtils.GetNewF(strIndexesF)); // f
                                        }
                                    }
                                    else
                                    {
                                        foreach (CoordIndexWrl coordIndex in coordIndexesList)
                                        {
                                            string strIndexesF = GetIndexesF(coordIndex, i_coordIndex);
                                            obj.WriteLine(ObjUtils.GetNewF(strIndexesF)); // f
                                        }
                                    }
                                }

                            }
                            i_coordIndex += sizeCoord;
                            i_texCoordIndex += sizeTexCoord;
                            index++;
                        }
                    }
                }
            }
            catch
            {
                return false;
            }
            return MtlExporter.WriteMtl(mtlLines, mtlFilename); // Create the mtl file
        }


        /// <summary>
        /// Return the f line string (i.e. 1/1 2/2 3/3)
        /// </summary>
        /// <param name="coordIndex"></param>
        /// <param name="texCoordIndex"></param>
        /// <param name="i_coordIndex"></param>
        /// <param name="i_texCoordIndex"></param>
        /// <returns></returns>
        private static string GetIndexesF(CoordIndexWrl coordIndex, CoordIndexWrl texCoordIndex, int i_coordIndex, int i_texCoordIndex)
        {
            string strIndexesF = "";
            List<int> coordIndexesList = coordIndex.Indexes;
            List<int> texCoordIndexesList = texCoordIndex.Indexes;
            for (int i = 0; i < coordIndexesList.Count; i++)
                // WRL index start at 0 while OBJ index start at 1
                strIndexesF += string.Format("{0}/{1} ", 1 + coordIndexesList[i] + i_coordIndex, 1 + texCoordIndexesList[i] + i_texCoordIndex);
            return strIndexesF.TrimEnd(); // Remove the ending space
        }

        /// <summary>
        /// Return the f line string (i.e. 1 2 3)
        /// </summary>
        /// <param name="coordIndex"></param>
        /// <param name="i_coordIndex"></param>
        /// <returns></returns>
        private static string GetIndexesF(CoordIndexWrl coordIndex, int i_coordIndex)
        {
            string strIndexesF = "";
            foreach (int index in coordIndex.Indexes)
                strIndexesF += string.Format("{0} ", 1 + index + i_coordIndex);
            return strIndexesF.TrimEnd(); // Remove the ending space
        }


    }
}
