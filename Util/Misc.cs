using System;

namespace Util
{
	public static class Misc
	{
		public static string ToHex(this byte b)
		{
			char[] HEXBITS = "0123456789ABCDEF".ToCharArray();
			return HEXBITS[b >> 4].ToString() + HEXBITS[b & 0xF].ToString();
		}

		public static string ToHex(this byte[] inp)
		{
			string res = "";
			for (int i = 0; i < inp.Length; i++) {
				res = res + inp[i].ToHex();
			}
			return res;
		}
	}
}

