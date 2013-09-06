using System;
using System.Threading;
using Gtk;
using Geom;
using Util;
using Transform;

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
			Application.Init();

			// Need these for both the window and the simulation thread.
			world = new World(new Geodesic(6));

			Thread simthread = new Thread(SimLoop);
			simthread.Start();

			MainWindow win = new MainWindow();
			win.Show();
			Application.Run();
		}

		public void SimLoop()
		{
			IRandGen rg = new RC4("Key");
			SqueezeSphere xform = new SqueezeSphere(world, rg.Fork());
			xform.Apply(10000);
			Grid.Dump(world.Grid, world.GetLayer(World.Height), Console.Out);

		}
	}
}
