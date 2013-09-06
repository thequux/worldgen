using System;
using Worldgen.Geom;
using Worldgen.Util;

namespace Worldgen.Transform
{
	public class SqueezeSphere
	{
		IRandGen rand;
		World world;
		Layer<double> layer;

		public SqueezeSphere(World w, IRandGen rand, Layer<double> layer = null)
		{
			this.world = w;
			this.rand = rand;
			this.layer = layer ?? World.Height;
		}

		public void Apply(int itercount = 1)
		{
			// This gets applied to the basemap.
			var overlay = world.GetLayer(layer);
			var pb = new ProgressBar(itercount);
			for (int iter = 0; iter < itercount; iter++) {
				world.Mtx.WaitOne();
				pb.Update();
				// Pick a random plane.
				Point3d pt;

				do {
					pt = (rand.NextPoint3d() - new Point3d(0.5, 0.5, 0.5)) * 2;
				} while (pt.Length2 > 1);
				double adj = (rand.Next() % 2 == 0) ? 1 : -1;
				for (int i = 0; i < world.Grid.Count; i++) {
					var gpt = world.Grid[i];
					if (gpt.Location.Dot(pt) > 0) {
						overlay[i] += adj;
					} else {
						overlay[i] -= adj;
					}
				}
				world.Mtx.ReleaseMutex();
			}
		}
	}
}

