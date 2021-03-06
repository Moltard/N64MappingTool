﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using N64Library.Tool.Data;
using N64Library.Tool.Utils;

namespace N64Library.Tool.Parser
{

    public static class WrlParser
    {

        /// <summary>
        /// Try to parse the wrl file to make the data useable. Return null if error during the parsing
        /// </summary>
        /// <param name="file">File to parse</param>
        /// <returns>WrlData if successful, null otherwise</returns>
        public static WrlData TryParseWrl(string file)
        {
            WrlData wrlData = null;
            try { wrlData = ParseWrl(file); } catch { }
            return wrlData;
        }

        /// <summary>
        /// Parse the wrl file to make the data useable
        /// </summary>
        /// <param name="file">File to parse</param>
        /// <returns></returns>
        public static WrlData ParseWrl(string file)
        {
            string[] lines = GenericUtils.RemoveBlankSpaces(FileUtilsShared.ReadFile(file)); // Read the file and remove blankspace

            if (lines != null)
            {
                WrlData wrlData = new WrlData();
                
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains(("Transform")))
                    {
                        int transformStart = i;
                        int transformEnd = GetIndexEnd(lines, transformStart);
                        if (transformEnd == -1) { return null; } // Parsing error
                        wrlData.TransformsList.Add(TransformData(lines, transformStart, transformEnd));
                        i = transformEnd; // We go to the end of transform
                    }
                }
                return wrlData;
            }
            return null;
        }

        private static Transform TransformData(string[] lines, int startIndex, int endIndex)
        {
            for (int i = startIndex + 1; i < endIndex; i++)
            {
                if (lines[i].StartsWith("children")) // We find the start of the usable data
                {
                    int childrenStart = i;
                    int childrenEnd = GetIndexEnd(lines, childrenStart);
                    if (childrenEnd == -1) { break; } // Parsing error
                    return new Transform(ChildrenData(lines, childrenStart, childrenEnd));
                }
            }
            return new Transform();
        }

        private static List<Shape> ChildrenData(string[] lines, int startIndex, int endIndex)
        {
            List<Shape> shapesList = new List<Shape>();
            for (int i = startIndex + 1; i < endIndex; i++)
            {
                if (lines[i].StartsWith(("Shape")))
                {
                    int shapeStart = i;
                    int shapeEnd = GetIndexEnd(lines, shapeStart);
                    if (shapeEnd == -1) { break; } // Parsing error
                    shapesList.Add(ShapeData(lines, shapeStart, shapeEnd));
                    i = shapeEnd;
                }
            }
            return shapesList;
        }

        private static Shape ShapeData(string[] lines, int startIndex, int endIndex)
        {
            Shape shape = new Shape();
            for (int i = startIndex + 1; i < endIndex; i++)
            {
                if (lines[i].StartsWith(("appearance")))
                {
                    int appearanceStart = i;
                    int appearanceEnd = GetIndexEnd(lines, appearanceStart);
                    if (appearanceEnd == -1) { break; } // Parsing error
                    shape.Appearance = AppearenceData(lines, appearanceStart, appearanceEnd);
                    i = appearanceEnd;
                }

                if (lines[i].StartsWith(("geometry")))
                {
                    int geometryStart = i;
                    int geometryEnd = GetIndexEnd(lines, geometryStart);
                    if (geometryEnd == -1) { break; } // Parsing error
                    shape.Geometry = GeometryData(lines, geometryStart, geometryEnd);
                    i = geometryEnd;
                }
            }
            return shape;
        }

        private static Appearance AppearenceData(string[] lines, int startIndex, int endIndex)
        {
            Appearance appearence = new Appearance();

            for (int i = startIndex + 1; i < endIndex; i++)
            {
                if (lines[i].StartsWith("material Material"))
                {
                    int endIndexM = GetIndexEnd(lines, i);
                    if (endIndexM == -1) { break; } // Parsing error
                    appearence.Material = MaterialData(lines, i, endIndexM);
                    i = endIndexM; // We go to the } closing material
                }
                else if (lines[i].StartsWith("texture ImageTexture"))
                {
                    int endIndexT = GetIndexEnd(lines, i);
                    if (endIndexT == -1) { break; } // Parsing error
                    appearence.Texture = ImageTextureData(lines, i, endIndexT);
                    i = endIndexT; // We go to the } closing texture
                }
            }
            return appearence;
        }

        private static Material MaterialData(string[] lines, int startIndex, int endIndex)
        {
            Material material = new Material();
            for (int i = startIndex + 1; i < endIndex; i++)
            {
                string[] lineSplit = GenericUtils.SplitKeyValue(lines[i]);
                string k = lineSplit[0]; string v = lineSplit[1];
                
                switch (k)
                {
                    case "ambientIntensity":
                        if (Double.TryParse(v, out double tmpDouble))
                        {
                            material.AmbientIntensity = tmpDouble;
                        }
                        break;
                    case "shininess":
                        if (Double.TryParse(v, out tmpDouble))
                        {
                            material.Shininess = tmpDouble;
                        }
                        break;
                    case "transparency":
                        if (Double.TryParse(v, out tmpDouble))
                        {
                            material.Transparency = tmpDouble;
                        }
                        break;
                    case "diffuseColor":
                        material.DiffuseColor = new Color(v);
                        break;
                    case "specularColor":
                        material.SpecularColor = new Color(v);
                        break;
                    case "emissiveColor":
                        material.EmissiveColor = new Color(v);
                        break;
                    default:
                        break;
                }
            }
            return material;
        }


        private static ImageTexture ImageTextureData(string[] lines, int startIndex, int endIndex)
        {
            ImageTexture imageTexture = new ImageTexture();
            for (int i = startIndex + 1; i < endIndex; i++)
            {
                string[] lineSplit = GenericUtils.SplitKeyValue(lines[i]);
                string k = lineSplit[0]; string v = lineSplit[1];
                switch (k)
                {
                    case "repeatS":
                        imageTexture.RepeatS = v;
                        break;
                    case "repeatT":
                        imageTexture.RepeatT = v;
                        break;
                    case "url":
                        imageTexture.Url = GenericUtils.SplitQuoteValue(v); // url "3DCCECBB_c.bmp"
                        break;
                    default:
                        break;
                }
            }
            return imageTexture;
        }

        private static Geometry GeometryData(string[] lines, int startIndex, int endIndex)
        {
            Geometry geometry = new Geometry();

            int i = startIndex;
            while (i <= endIndex)
            {
                if (lines[i].StartsWith("ccw")) // ccw FALSE
                {
                    geometry.Ccw = GenericUtils.GetValueFromSplit(lines[i]);
                }
                else if (lines[i].StartsWith("solid")) // solid TRUE
                {
                    geometry.Solid = GenericUtils.GetValueFromSplit(lines[i]);
                }
                else if (lines[i].StartsWith("coord Coordinate"))
                {
                    int endIndexC = GetIndexEnd(lines, i);
                    geometry.Coord = CoordPointData(lines, i, endIndexC);
                    i = endIndexC; // We go to the } closing Coordinate
                }
                else if (lines[i].StartsWith("coordIndex"))
                {
                    int endIndexC = GetIndexEnd(lines, i);
                    geometry.CoordIndexes = CoordIndexData(lines, i, endIndexC);
                    i = endIndexC; // We go to the } closing coordIndex
                }
                else if (lines[i].StartsWith("texCoord TextureCoordinate"))
                {
                    int endIndexT = GetIndexEnd(lines, i);
                    geometry.TexCoord = TexCoordPointData(lines, i, endIndexT);
                    i = endIndexT; // We go to the } closing TextureCoordinate
                }
                else if (lines[i].StartsWith("texCoordIndex"))
                {
                    int endIndexT = GetIndexEnd(lines, i);
                    geometry.TexCoordIndexes = CoordIndexData(lines, i, endIndexT);
                    i = endIndexT; // We go to the } closing texCoordIndex
                }
                i++;
            }

            return geometry;
        }

        private static CoordinatesWrl CoordPointData(string[] lines, int startIndex, int endIndex)
        {
            CoordinatesWrl coordinates = new CoordinatesWrl();

            if (lines[startIndex + 1].StartsWith("point"))
            {
                startIndex += 1; // # We set the index to the 'point' line
                endIndex = GetIndexEnd(lines, startIndex);
                for (int i = startIndex + 1; i < endIndex; i++) // We start on the line after point
                {
                    // e.g. -0.13476259 0.11304023 0.42341546,
                    Point p = new Point(GenericUtils.SplitBySpace(GenericUtils.TrimEndComma(lines[i])));
                    coordinates.PointsList.Add(p);
                }
            }

            return coordinates;
        }


        private static TextureCoordinatesWrl TexCoordPointData(string[] lines, int startIndex, int endIndex)
        {
            TextureCoordinatesWrl textureCoordinates = new TextureCoordinatesWrl();

            if (lines[startIndex + 1].StartsWith("point"))
            {
                startIndex += 1; // # We set the index to the 'point' line
                endIndex = GetIndexEnd(lines, startIndex);
                for (int i = startIndex + 1; i < endIndex; i++) // We start on the line after point
                {
                    UVCoordinates uv = new UVCoordinates(GenericUtils.SplitBySpace(GenericUtils.TrimEndComma(lines[i])));
                    textureCoordinates.UVList.Add(uv);
                }
            }
            return textureCoordinates;
        }


        private static CoordIndexesWrl CoordIndexData(string[] lines, int startIndex, int endIndex)
        {
            CoordIndexesWrl coordIndexes = new CoordIndexesWrl();
            for (int i = startIndex + 1; i < endIndex; i++)
            {
                coordIndexes.IndexesList.Add(new CoordIndexWrl(lines[i]));
            }
            return coordIndexes;
        }


        /// <summary>
        /// Return the index of the } or ] closing the { or [
        /// </summary>
        private static int GetIndexEnd(string[] lines, int indexStart, int i_bracket = 0, int i_curlyBracket = 0)
        {
            int indexEnd = -1;
            int i = indexStart;
            while (i < lines.Length)
            {
                string line = lines[i];
                if (line.LastIndexOf("{") != -1) // We found a {
                {
                    i = GetIndexEnd(lines, i + 1, i_bracket, i_curlyBracket + 1); // We get the index of the } closing it
                }
                else if (line.LastIndexOf("[") != -1) // We found a [
                {
                    i = GetIndexEnd(lines, i + 1, i_bracket + 1, i_curlyBracket); // We get the index of the ] closing it
                }
                else if (line.LastIndexOf("}") != -1 && i_curlyBracket != 0) // We found a }, but we are still in the recursion
                {
                    indexEnd = i; // We get the current index
                    break; // We leave the current loop
                }
                else if (line.LastIndexOf("]") != -1 && i_bracket != 0) // We found a ], but we are still in the recursion
                {
                    indexEnd = i; // We get the current index
                    break; // We leave the current loop
                }
                if (i == -1) // Error in the parsing of the file which returned never changed indexEnd and returned -1
                {
                    return -1; // By returning -1, we cancel the other loops
                }
                if (i_bracket == 0 && i_curlyBracket == 0) // We are in the original loop
                {
                    indexEnd = i; // We get the current index
                    break; // We quit
                }
                i++;
            }
            return indexEnd;
        }
        
    }
}
