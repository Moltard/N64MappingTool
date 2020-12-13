using N64Library.Tool.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N64Library.Tool.Data
{
    /// <summary>
    /// Class used to store a int reference (Java Integer behaviour)
    /// </summary>
    public class IntWrapper
    {
        public int Value { get; set; }
        public IntWrapper(int value) { Value = value; }
    }

    /// <summary>
    /// Base Class defining 3D coordinates
    /// </summary>
    public class Coordinates3D
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Coordinates3D() : this(0.0, 0.0, 0.0) { }
        public Coordinates3D(string line) : this(line.Split(' ')) { }
        public Coordinates3D(string[] coords)
        {
            try { X = double.Parse(coords[0]); } catch { X = 0.0; }
            try { Y = double.Parse(coords[1]); } catch { Y = 0.0; }
            try { Z = double.Parse(coords[2]); } catch { Z = 0.0; }
        }
        public Coordinates3D(string x, string y, string z)
        {
            try { X = double.Parse(x); } catch { X = 0.0; }
            try { Y = double.Parse(y); } catch { Y = 0.0; }
            try { Z = double.Parse(z); } catch { Z = 0.0; }
        }
        public Coordinates3D(double x, double y, double z)
        {
            X = x; Y = y; Z = z;
        }
        public Coordinates3D(Coordinates3D cd)
        {
            X = cd.X; Y = cd.Y; Z = cd.Z;
        }

        /// <summary>
        /// Scale the model by the given value on the X axis 
        /// </summary>
        /// <param name="value">Scale Value</param>
        public void ScaleX(double value) { X *= value; }
        /// <summary>
        /// Scale the model by the given value on the Y axis
        /// </summary>
        /// <param name="value">Scale Value</param>
        public void ScaleY(double value) { Y *= value; }
        /// <summary>
        /// Scale the model by the given value on the Z axis
        /// </summary>
        /// <param name="value">Scale Value</param>
        public void ScaleZ(double value) { Z *= value; }

        /// <summary>
        /// Uniformly Scale the model by the given value
        /// </summary>
        /// <param name="value">Scale Value</param>
        public void ScaleXYZ(double value)
        {
            ScaleX(value);
            ScaleY(value);
            ScaleZ(value);
        }

        /// <summary>
        /// Non Uniformly Scale the model by the given values
        /// </summary>
        /// <param name="valueX">Scale Value X axis</param>
        /// <param name="valueY">Scale Value Y axis</param>
        /// <param name="valueZ">Scale Value Z axis</param>
        public void ScaleXYZ(double valueX, double valueY, double valueZ)
        {
            ScaleX(valueX);
            ScaleY(valueY);
            ScaleZ(valueZ);
        }

        /// <summary>
        /// Multiply the Coordinates by the given 3x3 Matrix
        /// </summary>
        /// <param name="m">Matrix</param>
        public void MultiplicationMatrix(Matrix3D m)
        {
            /*
             *  a   b   c       x       ax+by+cz
             *  d   e   f   *   y   =   dx+ey+fz
             *  g   h   i       z       gx+hy+iz
             * */
            double x = m.Row1.X * X + m.Row1.Y * Y + m.Row1.Z * Z;
            double y = m.Row2.X * X + m.Row2.Y * Y + m.Row2.Z * Z;
            double z = m.Row3.X * X + m.Row3.Y * Y + m.Row3.Z * Z;
            X = x; Y = y; Z = z;
        }

        /// <summary>
        /// Convert the coordinates to the smd format
        /// </summary>
        /// <returns></returns>
        public string ToSmd() { return string.Format("{0:0.000000} {1:0.000000} {2:0.000000}", X, -Z, Y); }

        /// <summary>
        /// Convert the coordinates into a string
        /// </summary>
        /// <returns></returns>
        public override string ToString() { return string.Format("{0:0.000000} {1:0.000000} {2:0.000000}", X, Y, Z); }

    }


    /// <summary>
    /// Base Class defining 2D coordinates
    /// </summary>
    public class Coordinates2D
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Coordinates2D() { X = 0.0; Y = 0.0; }
        public Coordinates2D(string line) : this(line.Split(' ')) { }
        public Coordinates2D(string[] coords)
        {
            try { X = double.Parse(coords[0]); } catch { X = 0.0; }
            try { Y = double.Parse(coords[1]); } catch { Y = 0.0; }
        }
        public Coordinates2D(string x, string y)
        {
            try { X = double.Parse(x); } catch { X = 0.0; }
            try { Y = double.Parse(y); } catch { Y = 0.0; }
        }
        public Coordinates2D(double x, double y)
        {
            X = x; Y = y;
        }

        /// <summary>
        /// Convert the coordinates into a string
        /// </summary>
        /// <returns></returns>
        public override string ToString() { return string.Format("{0:0.000000} {1:0.000000}", X, Y); }

    }


    /// <summary>
    /// Store UV Mapping coordinates
    /// </summary>
    public class UVCoordinates : Coordinates2D
    {
        public UVCoordinates(): base() { }
        public UVCoordinates(string line) : base(line) { }
        public UVCoordinates(string[] coords) : base(coords) { }
        public UVCoordinates(string x, string y) : base(x, y) { }
        public UVCoordinates(double x, double y) : base(x, y) { } 
    }


    /// <summary>
    /// Represents a point in a 3D space
    /// </summary>
    public class Point : Coordinates3D
    {
        public Point() : base() { }
        public Point(string line) : base(line) { }
        public Point(string[] coords) : base(coords) { }
        public Point(string x, string y, string z) : base(x, y, z) { }
        public Point(double x, double y, double z) : base(x, y, z) { }
        public Point(Coordinates3D cd) : base(cd) { }

    }


    /// <summary>
    /// Represents a vector in a 3D space
    /// </summary>
    public class Vector : Coordinates3D
    {
        public Vector() : base() { }
        public Vector(string line) : base(line) { }
        public Vector(string[] coords) : base(coords) { }
        public Vector(string x, string y, string z) : base(x, y, z) { }
        public Vector(double x, double y, double z) : base(x, y, z) { }
        public Vector(Coordinates3D cd) : base(cd) { }
        public Vector(Point p1, Point p2)
        {
            X = p2.X - p1.X;
            Y = p2.Y - p1.Y;
            Z = p2.Z - p1.Z;
        }

        /// <summary>
        /// Return the norm of the vector (positive value)
        /// </summary>
        /// <returns></returns>
        public double GetNorm()
        {
            return Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2));
        }

        /// <summary>
        /// Calculate the Unit vector
        /// </summary>
        /// <returns></returns>
        public Vector GetUnitVector()
        {
            return GetUnitVector(GetNorm());
        }

        /// <summary>
        /// Calculate the Unit vector for the given norm
        /// </summary>
        /// <param name="norm"></param>
        /// <returns></returns>
        public Vector GetUnitVector(double norm)
        {
            if (norm != 0)
                return new Vector(X / norm, Y / norm, Z / norm);
            return this; // The current vector
        }

        /// <summary>
        /// Calculate the Normal vector (perpendicular to the surface)
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Vector GetNormalVector(Vector v1, Vector v2)
        {
            return v2.GetNormalVector(v1);
        }

        /// <summary>
        /// Calculate the Normal vector (perpendicular to the surface)
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector GetNormalVector(Vector v)
        {
            double u1 = X; double u2 = Y; double u3 = Z;
            double v1 = v.X; double v2 = v.Y; double v3 = v.Z;
            /*
             * To calculate the vector:
             * u2v3 - u3v2
             * u3v1 - u1v3
             * u1v2 - u2v1
             * */
            double s1 = u2 * v3 - u3 * v2;
            double s2 = u3 * v1 - u1 * v3;
            double s3 = u1 * v2 - u2 * v1;
            return new Vector(s1, s2, s3);
        }
        

    }

    /// <summary>
    /// Represent a 3x3 Matrix
    /// </summary>
    public class Matrix3D
    {
        public Coordinates3D Row1 { get; set; }
        public Coordinates3D Row2 { get; set; }
        public Coordinates3D Row3 { get; set; }

        public Matrix3D()
        {
            Row1 = new Coordinates3D();
            Row2 = new Coordinates3D();
            Row3 = new Coordinates3D();
        }
        public Matrix3D(Coordinates3D v1, Coordinates3D v2, Coordinates3D v3)
        {
            Row1 = v1;
            Row2 = v2;
            Row3 = v3;
        }
        public Matrix3D(double r1c1, double r1c2, double r1c3,
                        double r2c1, double r2c2, double r2c3,
                        double r3c1, double r3c2, double r3c3)
        {
            Row1 = new Coordinates3D(r1c1, r1c2, r1c3);
            Row2 = new Coordinates3D(r2c1, r2c2, r2c3);
            Row3 = new Coordinates3D(r3c1, r3c2, r3c3);
        }
        public Matrix3D(Matrix3D m)
        {
            Row1 = new Coordinates3D(m.Row1);
            Row2 = new Coordinates3D(m.Row2);
            Row3 = new Coordinates3D(m.Row3);
        }
        
        /// <summary>
        /// Calculate the Rotation Matrix on the X axis for the given angle
        /// </summary>
        /// <param name="a">Angle</param>
        /// <returns>Return the Rotation Matrix</returns>
        public static Matrix3D GetXRotationMatrix(double a)
        {
            // 1    0       0
            // 0    cos a   - sin a
            // 0    sin a   cos a
            double cos = AngleUtils.Cos(a);
            double sin = AngleUtils.Sin(a);

            Coordinates3D Row1 = new Coordinates3D(1, 0, 0);
            Coordinates3D Row2 = new Coordinates3D(0, cos, -sin);
            Coordinates3D Row3 = new Coordinates3D(0, sin, cos);
            return new Matrix3D(Row1, Row2, Row3);
        }

        /// <summary>
        /// Calculate the Rotation Matrix on the Y axis for the given angle
        /// </summary>
        /// <param name="a">Angle</param>
        /// <returns>Return the Rotation Matrix</returns>
        public static Matrix3D GetYRotationMatrix(double a)
        {
            // cos a    0       sin a
            // 0        1       0
            // -sin a   0       cos a
            double cos = AngleUtils.Cos(a);
            double sin = AngleUtils.Sin(a);

            Coordinates3D Row1 = new Coordinates3D(cos, 0, sin);
            Coordinates3D Row2 = new Coordinates3D(0, 1, 0);
            Coordinates3D Row3 = new Coordinates3D(-sin, 0, cos);
            return new Matrix3D(Row1, Row2, Row3);
        }

        /// <summary>
        /// Calculate the Rotation Matrix on the Z axis for the given angle
        /// </summary>
        /// <param name="a">Angle</param>
        /// <returns>Return the Rotation Matrix</returns>
        public static Matrix3D GetZRotationMatrix(double a)
        {
            // cos a    - sin a     0    
            // sin a    cos a       0        
            // 0        0           1
            double cos = AngleUtils.Cos(a);
            double sin = AngleUtils.Sin(a);

            Coordinates3D Row1 = new Coordinates3D(cos, -sin, 0);
            Coordinates3D Row2 = new Coordinates3D(sin, cos, 0);
            Coordinates3D Row3 = new Coordinates3D(0, 0, 1);
            return new Matrix3D(Row1, Row2, Row3);
        }
    }


    /// <summary>
    /// Represents a RGB Color Code
    /// </summary>
    public class Color
    {
        public double R { get; set; }
        public double G { get; set; }
        public double B { get; set; }

        public Color() : this(0, 0, 0) { }
        public Color(string line) : this(line.Split(' ')) { }
        public Color(string[] split)
        {
            try { R = double.Parse(split[0]); } catch { R = 0.0; }
            try { G = double.Parse(split[1]); } catch { G = 0.0; }
            try { B = double.Parse(split[2]); } catch { B = 0.0; }
        }
        public Color(double r, double g, double b)
        {
            R = r; G = g; B = b;
        }
        public Color(Color c) : this(c.R, c.G, c.B) { }

        /// <summary>
        /// Convert the RGB colors into a string
        /// </summary>
        /// <returns></returns>
        public override string ToString() { return string.Format("{0:0.000000} {1:0.000000} {2:0.000000}", R, G, B); }

    }

}
