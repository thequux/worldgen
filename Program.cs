using System;
using Gtk;
using Util;
using Transform;

namespace Worldgen
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			/*
			Application.Init ();
			MainWindow win = new MainWindow ();
			win.Show ();
			Application.Run ();*/
			Geom.Geodesic grid = new Geom.Geodesic(6);
			Geom.World w = new Geom.World(grid);
			Util.IRandGen rg = new Util.RC4("Key");

			SqueezeSphere xform = new SqueezeSphere(w, rg);
			xform.Apply(10000);
			Geom.Grid.Dump(grid, w.GetLayer(Geom.World.Height), System.Console.Out);

		}
	}
}
