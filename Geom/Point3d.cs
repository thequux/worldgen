using System;

#if OpenTK
using OpenTK;

#endif
namespace Worldgen.Geom
{
	public struct Point3d
	{
		public double X, Y, Z;

		public Point3d(double x, double y, double z)
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
		}

		public void Normalize(double len = 1.0)
		{
			this *= len / Length;
		}

		public Point3d Normal(double len = 1.0)
		{
			return this * len / Length;
		}

		public double Length2 {
			get {
				return X * X + Y * Y + Z * Z;
			}
		}

		public double Length {
			get {
				return Math.Sqrt(Length2);
			}
		}

		public static Point3d operator*(Point3d pt, double scale)
		{
			return new Point3d(pt.X * scale, pt.Y * scale, pt.Z * scale);
		}

		public static Point3d operator*(double scale, Point3d pt)
		{
			// multiplication is commutitive, right?
			return pt * scale;
		}

		public static Point3d operator/(Point3d pt, double scale)
		{
			return pt * (1 / scale);
		}

		public static Point3d operator+(Point3d p1, Point3d p2)
		{
			return new Point3d(p1.X + p2.X,
			                   p1.Y + p2.Y,
			                   p1.Z + p2.Z);
		}

		public static Point3d operator-(Point3d p1, Point3d p2)
		{
			return new Point3d(p1.X - p2.X,
			                   p1.Y - p2.Y,
			                   p1.Z - p2.Z);
		}

		public static Point3d operator-(Point3d p)
		{
			return p * -1;
		}

		public double Dot(Point3d pt)
		{
			return this.X * pt.X 
				+ this.Y * pt.Y
				+ this.Z * pt.Z;
		}

		public override string ToString()
		{
			return string.Format("Point3d<{0},{1},{2}>", this.X, this.Y, this.Z);
		}
#if OpenTK
		public static implicit operator Vector3d(Point3d pt)
		{
			return new Vector3d(pt.X, pt.Y, pt.Z);
		}
#endif
	}
}