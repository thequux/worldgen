using System;

namespace Worldgen.Util
{
	static class StringExts
	{
		public static string Repeat(this string s, int count)
		{
			string ret = "";
			for (int i = 0; i < count; i++) {
				ret += s;
			}
			return ret;
		}
	}
}

