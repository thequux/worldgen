using System;

namespace Worldgen.Util
{
	sealed public class RC4 : IRandGen
	{

		private byte[] state;
		private byte i, j;

		private void Init(byte[] key)
		{	
			int pos;
			state = new byte[256];
			for (pos = 0; pos < 256; pos++) {
				state[pos] = (byte)pos;
			}
			j = 0;
			for (pos = 0; pos < 256; pos++) {
				j = (byte)((j + state[pos] + key[pos % key.Length]) % 256);
				swap(ref state[j], ref state[pos]);
			}
			i = 0;
			j = 0;
		}

		public RC4(byte[] key) // more of a seed, really
		{
			Init(key);
		}

		public RC4(IRandGen cloneFrom)
		{
			Init(cloneFrom.NextBuffer(256));
		}

		public RC4(string key)
		{
			System.Text.Encoding enc = new System.Text.UTF8Encoding(false, true);

			Init(enc.GetBytes(key));
		}

		private void swap(ref byte x, ref byte y)
		{
			byte tmp = x;
			x = y;
			y = tmp;
		}

		public byte Next()
		{
			i = (byte)(i + 1);
			j = (byte)(j + state[i]);
			swap(ref state[i], ref state[j]);
			byte k = state[(state[i] + state[j]) % 256];
			return k;
		}

		public IRandGen Fork()
		{
			return new RC4(this);
		}
	}
}