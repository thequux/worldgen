using System;
using Gtk;

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
			Grid.Geodesic grid = new Grid.Geodesic(3);
		}
	}
}
