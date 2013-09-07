using System;
using System.Threading;
using Worldgen.Geom;
using Worldgen.Util;
using Worldgen.Transform;

namespace Worldgen
{
	class MainClass
	{
		World world;

		public static void Main(string[] args)
		{

			new MainClass(args);
		}

		private MainClass(string[] args)
		{
			// Need these for both the window and the simulation thread.
			world = new World(new Geodesic(7));

			Thread simthread = new Thread(SimLoop);
			simthread.Start();

#if OpenTK
			var gui = new Gui.OpenTK.Gui(world);
			gui.Run(30.0, 30.0);
#endif
		}

		public void SimLoop()
		{
			IRandGen rg = new RC4("Key");
			SqueezeSphere xform = new SqueezeSphere(world, rg.Fork());
			xform.Apply(1000000);
			Grid.Dump(world.Grid, world.GetLayer(World.Height), Console.Out);

		}
	}
}
