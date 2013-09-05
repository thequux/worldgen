using System;
using System.Collections.Generic;

namespace Geom
{
	public interface IGrid
	{
		/**
		 * IGrid is a mapping of points to a sphere. Each point has a set of neigbors and a 3-dimensional location
		 */

		Geom.Point3d this [int index] {
			get;
		}

		IEnumerable<int> Neighbors(int index);
	}
}

