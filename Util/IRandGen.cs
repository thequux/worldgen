using System;

namespace Worldgen.Util
{
	public interface IRandGen
	{
		byte Next();

		IRandGen Fork();
	}

	public static class RandUtils
	{
		public static byte[] NextBuffer(this IRandGen gen, int count)
		{
			byte[] buf = new byte[count];
			return gen.NextBuffer(buf);
		}

		public static byte[] NextBuffer(this IRandGen gen, byte[] buf)
		{
			for (int i = 0; i < buf.Length; i++) {
				buf[i] = gen.Next();
			}
			return buf;
		}

		public static int NextInt(this IRandGen gen)
		{
			// Read a big-endian integer from the generator
			int ret = 0;
			for (int i = 0; i < 4; i++) {
				ret = ret << 8 | gen.Next();
			}
			return ret;
		}

		public static uint NextUint(this IRandGen gen)
		{
			uint ret = 0;
			for (int i = 0; i < 4; i++) {
				ret = ret << 8 | gen.Next();
			}
			return ret;
		}

		public static UInt64 NextUint64(this IRandGen gen)
		{
			UInt64 ret = 0;
			for (int i = 0; i < 8; i++) {
				ret = ret << 8 | gen.Next();
			}
			return ret;
		}

		public static float NextFloat(this IRandGen gen)
		{
			// Uniformly distributed float between 0 and 1. 

			UInt32 val = gen.NextUint() >> 8; // 23 bits of mantissa, not including possible leading 1. This is definitely representable in a float.
			return (float)val / (1 << 23);
		}

		public static double NextDouble(this IRandGen gen)
		{
			// Uniformly distributed double between 0 and 1. 

			double val = (double)gen.NextUint64();
			val = val / (1 << 30);
			val = val / (1 << 30);
			return val / (1 << 4); // Two separate steps to stay within range.
		}

		public static Geom.Point3d NextPoint3d(this IRandGen gen)
		{
			// Random point in the unit cube at the origin. ( [0,1), [0,1), [0,1) )
			return new Geom.Point3d(gen.NextDouble(), gen.NextDouble(), gen.NextDouble());
		}

		public static uint NextInRange(this IRandGen gen, uint max)
		{
			// returns a random int between 0 and max, inclusive

			if (max == uint.MaxValue) // get rid of a boundary condition
				return gen.NextUint();

			// How many values at the high end of the range need to be thrown away
			uint deadZone = uint.MaxValue % (max + 1) + 1;
			if (deadZone == max + 1) {
				// uint.range is evenly divisible by max+1
				return gen.NextUint() % (max + 1);
			}

			uint maxAccept = uint.MaxValue - deadZone;
			// maxaccept is now one less than a multiple of max+1
			// thus, if n is a random number in [0, maxAccept], 
			// (n%(max+1)) is a uniform random number in [0,max]

			uint rn;
			do {
				rn = gen.NextUint();
			} while (rn > maxAccept);

			return rn % (max + 1);

		}
	}
}

