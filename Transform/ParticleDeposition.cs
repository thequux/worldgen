using System;
using System.Collections.Generic;
using Worldgen.Geom;
using Worldgen.Util;

namespace Worldgen.Transform
{
	public class ParticleDeposition
	{
		// This method is likely to be significantly faster than SqueezeSphere as it's local
		IRandGen rand;
		World world;
		Layer<double> layer;

		public ParticleDeposition(World world, IRandGen rand, Layer<double> layer = null)
		{
			this.rand = rand;
			this.world = world;
			this.layer = layer ?? World.Height;
		}

		public void Apply(float pRoll, int itercount)
		{
			var hmap = world.GetLayer(layer);
			// pick a random starting point...

			int curPoint = (int)rand.NextInRange((uint)world.Grid.Count);
			for (int iter = 0; iter < itercount; iter++) {
				int minNeighbor = curPoint;
				while (rand.NextFloat() < pRoll) {
					// try rolling
					int lastpoint = minNeighbor;
					double minHeight = hmap[lastpoint];
					var neigbors = world.Grid.Neighbors(curPoint);
					for (int i = 0; i < neigbors.Count; i++) {
						if (hmap[neigbors[i]] < minHeight) {
							minHeight = hmap[neigbors[i]];
							minNeighbor = neigbors[i];
						}
					}
					if (minNeighbor == lastpoint)
						break;
				}

				// bump minNeighbor
				hmap[minNeighbor] += 1;

				// move to a random neighbor...
				var neighbors = world.Grid.Neighbors(curPoint);
				curPoint = neighbors[(int)rand.NextInRange((uint)neighbors.Count-1)];
			}
		}
	}
}

