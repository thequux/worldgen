using System;

namespace Geom
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
	}
}