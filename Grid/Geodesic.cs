using System;
using System.Collections.Generic;
using Geom;

namespace Grid
{
	public class Geodesic
	{
		List<Point3d> vertices;
		List<biline> edges;

		private class biline
		{
			public int P1, P2, Mid;
			public int S1, S2;

			public biline(int p1, int p2)
			{
				this.P1 = p1;
				this.P2 = p2;
				this.Mid = -1;
				this.S1 = this.S2 = -1;
			}

			public bool HasPoint(int p)
			{
				return P1 == p || P2 == p;
			}
		}

		private struct triangle
		{
			public int[] L;
			public int[] V;

			public triangle(int a, int b, int c)
			{
				L = new int[] { a, b, c };
				V = new int[] { -1, -1, -1 };
			}

			public void Update(IList<biline> edges)
			{
				V[0] = edges[L[0]].P1;
				V[1] = edges[L[0]].P2;
				var e = edges[L[1]];
				V[2] = (e.P1 == V[0] || e.P1 == V[1]) ? e.P2 : e.P1;
			}
		}

		public Geodesic(int niter)
		{
			vertices = new List<Point3d>(12);
			edges = new List<biline>(30);
			var faces = new List<triangle>(20);
			double at12 = Math.Atan(1.0/2);
			double rat12 = Math.Cos(at12);
			double hat12 = Math.Sin(at12);
			const double ang = Math.PI / 5;
			double vp = 1;
			for (int i = 0; i < 10; i++) {
				vertices.Add(new Point3d(Math.Sin(ang * i) * rat12,
				                 	    Math.Cos(ang * i) * rat12,
				                    	hat12 * vp));
				edges.Add(new biline(i, (i + 2) % 10));
				edges.Add(new biline(i, (i+1) % 10));
				edges.Add(new biline(i, (vp > 0) ? 10 : 11));
				faces.Add(new triangle(i * 3, i * 3 + 1, (i * 3 + 4) % 30));
				faces.Add(new triangle(i * 3, i * 3 + 2, (i * 3 + 8) % 30));
				vp = -vp;
			}
			vertices.Add(new Point3d(0, 0, 1));
			vertices.Add(new Point3d(0, 0, -1));


			for (int subcnt = 0; subcnt < niter; subcnt++) {
				var newfaces = new List<triangle>();
				var newedges = new List<biline>();
				for (int i = 0; i < faces.Count; i++) {
					var face = faces[i];
					face.Update(edges);
					// vs is the set of vertices, including bisectors, going around in some consistent directon.
					var vs = new int[6];
					var es = new int[6];
					var l1first = new bool[3];
					for (int j = 0; j < 3; j++) {
						var l = edges[face.L[j]];
						l1first[j] = edges[face.L[(j+2)%3]].HasPoint(l.P1);
						vs[j * 2] = l1first[j] ? l.P1 : l.P2;
						if (l.Mid == -1) {
							l.Mid = vertices.Count;
							vertices.Add(((vertices[l.P1] + vertices[l.P2]) / 2).Normal());
							// I'm going to need both subdivisions, so I'll create them now.
							l.S1 = newedges.Count;
							newedges.Add(new biline(l.P1, l.Mid));
							l.S2 = newedges.Count;
							newedges.Add(new biline(l.P2, l.Mid));
						}
						vs[j * 2 + 1] = l.Mid;
						es[j * 2] = l1first[j] ? l.S1 : l.S2;
						es[j * 2 + 1] = l1first[j] ? l.S2 : l.S1;
						// es[0] is between vs[0] and vs[1]
					}
					var centerEdges = new int[3];
					for (int j = 0; j < 3; j++) {
						centerEdges[j] = newedges.Count;
						newedges.Add(new biline(vs[2*j+1], vs[(j * 2 + 5) % 6]));
						newfaces.Add(new triangle(es[(j * 2) % 6], es[(j * 2 + 5) % 6], centerEdges[j]));
					}
					newfaces.Add(new triangle(centerEdges[0], centerEdges[1], centerEdges[2]));
				}
				faces = newfaces;
				edges = newedges;
			}
#if true
			// Dump in skeleton
			Console.WriteLine("SKEL");
			Console.WriteLine("{0} {1}", vertices.Count, edges.Count);
			for (int i = 0; i < vertices.Count; i++) {
				Console.WriteLine("{0} {1} {2}",
				                  vertices[i].X,
				                  vertices[i].Y,
				                  vertices[i].Z);
			}
			for (int i = 0; i < edges.Count; i++) {
				biline e = edges[i];
				Console.WriteLine("2 {0} {1}", e.P1, e.P2);
			}
#endif
		}
	}
}
