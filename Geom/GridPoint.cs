using System;
using System.Collections.Generic;

namespace Worldgen.Geom
{
	public struct GridPoint
	{
		public int Index;
		private Grid grid;

		public GridPoint(Grid grid, int idx)
		{
			Index = idx;
			this.grid = grid;
		}

		public IEnumerable<GridPoint> Neighbors {
			get {
				var ns = new List<GridPoint>(6);
				foreach (var n in grid.Neighbors(Index)) {
					ns.Add(new GridPoint(grid, n));
				}
				return ns;
			}

		}

		public Point3d Location {
			get {
				return grid.Location(Index);
			}
		}
	}
}