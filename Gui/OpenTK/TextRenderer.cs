using System;
using System.Collections.Generic;
using OpenTK;

namespace Worldgen.Gui.OpenTK
{
	public class TextRenderer
	{
		private struct Span
		{
			public int x;
			public int y;
			public string content;
			public Vector3 color;

			public Span(int x, int y, string content, Vector3 color)
			{
				this.x = x;
				this.y = y;
				this.content = content;
				this.color = color;
			}
		}

		protected int curx, cury;
		protected IList<Span> spans;

		public TextRenderer()
		{
			spans = new IList<Span>();
			curx = cury = 0;
		}

		public void Print(int x, int y, string content)
		{
			spans.Add(new Span(x, y, content, Vector3.One));
			curx = x + content.Length;
			cury = y;
		}

		public void Print(string content)
		{
			// TODO: handle newlines.
			this.Print(curx, cury, content);
		}

		public void Nl()
		{
			this.curx = 0;
			this.cury++;
		}

		public void Clear()
		{
			this.curx = this.cury = 0;
			this.spans.Clear();
		}

		public void Render()
		{
			// TODO: implement this
		}
	}
}

