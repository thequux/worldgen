using System;

namespace Util
{
	public class ProgressBar
	{
		int cur;
		int max;
		int width = 160;

		public ProgressBar(int max)
		{
			cur = 0;
			this.max = max;
		}

		public void Update(int newval = -1)
		{
			if (newval == -1)
				newval = cur + 1;
			newval = Math.Min(newval, max);

			var oldpos = width * cur / max;
			var newpos = width * newval / max;
			if (oldpos != newpos) {
				Console.Error.Write("[\r{0}\x1B[K]", "=".Repeat(newpos).PadRight(width));
				Console.Error.Flush();
			}
			cur = newval;
		}
	}
}

