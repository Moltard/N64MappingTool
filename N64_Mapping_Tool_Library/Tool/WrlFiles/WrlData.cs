using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using N64Library.Tool.Utils;

namespace N64Library.Tool.WrlFiles
{
    public class WrlData
    {
        public List<Transform> TransformsList { get; set; }

        public WrlData()
        {
            TransformsList = new List<Transform>();
        }
    }

    public class Transform
    {
        public List<Shape> ShapesList { get; set; }

        public Transform()
        {
            ShapesList = new List<Shape>();
        }
        public Transform(List<Shape> shapes)
        {
            ShapesList = new List<Shape>(shapes);
        }
    }

    public class Shape
    {
        public Appearance Appearance { get; set; }
        public Geometry Geometry { get; set; }

        public Shape()
        {
            Appearance = null;
            Geometry = null;  
        }
    }


    public class Appearance
    {
        public Material Material { get; set; }
        public ImageTexture Texture { get; set; }

        public Appearance()
        {
            Material = null;
            Texture = null;
        }
    }

    public class Material
    {
        public double AmbientIntensity { get; set; }
        public double Shininess { get; set; }
        public double Transparency { get; set; }
        public Color DiffuseColor { get; set; }
        public Color SpecularColor { get; set; }
        public Color EmissiveColor { get; set; }

        public Material()
        {
            AmbientIntensity = 1;
            Shininess = 0.0;
            Transparency = 0.0;
            DiffuseColor = null;
            SpecularColor = null;
            EmissiveColor = null;
        }
    }

    public class ImageTexture
    {
        public string Url { get; set; }
        public string RepeatS { get; set; }
        public string RepeatT { get; set; }

        public ImageTexture()
        {
            RepeatS = "TRUE";
            RepeatT = "TRUE";
            Url = null;
        }
    }


    public class Geometry
    {
        public string Ccw { get; set; }
        public string Solid { get; set; }
        public CoordinatesWrl Coord { get; set; } // Coordinates for points
        public CoordIndexesWrl CoordIndexes { get; set; }
        public TextureCoordinatesWrl TexCoord { get; set; } // UV for textures
        public CoordIndexesWrl TexCoordIndexes { get; set; }

        public Geometry()
        {
            Ccw = "FALSE";
            Solid = "TRUE";
            Coord = null;
            CoordIndexes = null;
            TexCoord = null;
            TexCoordIndexes = null;
        }
    }


    public class CoordIndexWrl
    {
        public List<int> Indexes { get; set; }

        public CoordIndexWrl(string line)
        {
            Indexes = SplitCoordIndex(line);
        }

        public static List<int> SplitCoordIndex(string line)
        {
            // 0, 1, 2, -1,
            List<int> indexes = new List<int>();
            string[] lineSplit = Helper.SplitByCommaSpace(Helper.TrimEndComma(line)); // Split each index
            foreach (string c in lineSplit)
            {
                if (c != "-1") // -1 is the end of a line
                {
                    int? index = Helper.StringToInt(c);
                    if (index != null)
                        indexes.Add((int)index);
                }
            }
            return indexes;
        }
    }

    public class CoordIndexesWrl
    {
        public List<CoordIndexWrl> IndexesList { get; set; }

        public CoordIndexesWrl()
        {
            IndexesList = new List<CoordIndexWrl>();
        }
    }

    public class CoordinatesWrl
    {
        public List<Point> PointsList { get; set; }

        public CoordinatesWrl()
        {
            PointsList = new List<Point>();
        }
    }

    public class TextureCoordinatesWrl
    {
        public List<UVCoordinates> UVList { get; set; }

        public TextureCoordinatesWrl()
        {
            UVList = new List<UVCoordinates>();
        }
    }



}
