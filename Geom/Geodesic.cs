using System;
using System.Collections;
using System.Collections.Generic;

namespace Worldgen.Geom
{
	// Really more "topology" than "grid", but "grid" will do.
	public class Geodesic : Grid
	{

		List<Point3d> vertices;
		List<biline> edges;
		List<triangle> faces;
		SortedSet<int>[] _Neighbors;

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
				V = new int[] { 0, 0, 0 };
			}

			public void Update(IList<biline> edges)
			{
				V[0] = edges[L[0]].P1;
				V[1] = edges[L[0]].P2;
				var e = edges[L[1]];
				V[2] = (e.P1 == V[0] || e.P1 == V[1]) ? e.P2 : e.P1;
			}

			public IList<int> Vertices(IList<biline> edges)
			{
				var e0 = edges[L[0]];
				var e1 = edges[L[1]];
				return new int[] { e0.P1, e0.P2, e0.HasPoint(e1.P1) ? e1.P2 : e1.P1 };
			}
		}

		public Geodesic(int niter)
		{
			vertices = new List<Point3d>(12);
			edges = new List<biline>(30);
			faces = new List<triangle>(20);
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
				if (vp > 0) {
					faces.Add(new triangle(i * 3, i * 3 + 1, (i * 3 + 4) % 30));
					faces.Add(new triangle(i * 3 + 2, i * 3, (i * 3 + 8) % 30));
				} else {
					faces.Add(new triangle(i * 3 + 1, i * 3, (i * 3 + 4) % 30));
					faces.Add(new triangle(i * 3, i * 3 + 2, (i * 3 + 8) % 30));

				}
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
					newfaces.Add(new triangle(centerEdges[0], centerEdges[2], centerEdges[1]));
				}
				faces = newfaces;
				edges = newedges;
			}

			// Update quick access structures
			_Neighbors = new SortedSet<int>[vertices.Count];
			for (int i = 0; i < vertices.Count; i++) {
				_Neighbors[i] = new SortedSet<int>();
			}
			foreach (var edge in edges) {
				_Neighbors[edge.P1].Add(edge.P2);
				_Neighbors[edge.P2].Add(edge.P1);
			}
		}

		public override GridPoint this [int idx] {
			get {
				return new GridPoint(this, idx);
			}
		}

		public override Point3d Location(int idx)
		{
			return vertices[idx];
		}

		public override IList<int> Neighbors(int point)
		{
			return new List<int>(_Neighbors[point]);
		}

		public override int Count {
			get {
				return vertices.Count;
			}
		}
		#region Face enumeration

		protected class GeodesicFaceEnumerator : IEnumerator<Face>
		{
			protected Geodesic Parent;
			int curpos;
			bool started = false;

			public GeodesicFaceEnumerator(Geodesic parent)
			{
				Parent = parent;
				started = false;
				curpos = 0;
			}

			public Face Current {
				get {
					if (!started)
						throw new ArgumentException("Enumerator not started");
					IList<int> vs = Parent.faces[curpos].Vertices(Parent.edges);
					return new Face(vs[0], vs[1], vs[2]);
				}
			}

			object IEnumerator.Current {
				get {
					return Current;
				}
			}

			void IDisposable.Dispose()
			{
			}

			bool IEnumerator.MoveNext()
			{
				if (!started) {
					curpos = 0;
					started = true;
				} else
					curpos++;
				return curpos < Parent.faces.Count;
			}

			void IEnumerator.Reset()
			{
				started = false;
			}
		}

		protected class GeodesicFaceEnumerable : IEnumerable<Face>
		{
			protected Geodesic Parent;

			public GeodesicFaceEnumerable(Geodesic parent)
			{
				Parent = parent;
			}

			public IEnumerator<Face> GetEnumerator()
			{
				return new GeodesicFaceEnumerator(Parent);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}

		public override IEnumerable<Face> Faces {
			get {
				return new GeodesicFaceEnumerable(this);
			}
		}
		#endregion
	}
}
