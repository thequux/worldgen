using System;
using System.Collections.Generic;

namespace Worldgen.Geom
{
	public struct Face
	{
		public int[] V;

		public Face(int v0, int v1, int v2)
		{
			V = new int[] { v0, v1, v2 };
		}
	}

	public abstract class Grid
	{
		/**
		 * IGrid is a mapping of points to a sphere. Each point has a set of neigbors and a 3-dimensional location
		 */

		public abstract GridPoint this [int index] {
			get;
		}

		public abstract int Count {
			get;
		}

		public abstract Point3d Location(int index);

		public abstract IList<int> Neighbors(int index);

		public abstract IEnumerable<Face> Faces {
			get;
		}

		public static void Dump(Grid grid, IList<double> overlay, System.IO.TextWriter output)
		{
			// Dump in skeleton
			output.WriteLine("OFF");
			IList<Face> faces = new List<Face>(grid.Faces);
			output.WriteLine("{0} {1} {2}", grid.Count, faces.Count, 0);
			for (int i = 0; i < grid.Count; i++) {
				var v = grid[i].Location * (overlay == null ? 1.0 : overlay[i]);
				output.WriteLine("{0} {1} {2}", v.X, v.Y, v.Z);
			}
			for (int i = 0; i < faces.Count; i++) {
				var f = faces[i];

				output.WriteLine("3 {0} {1} {2}", f.V[0], f.V[1], f.V[2]);
			}
		}
	}
}

