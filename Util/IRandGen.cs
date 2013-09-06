using System;

namespace Util
{
	public interface IRandGen
	{
		byte Next();
	}

	public static class RandUtils
	{
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

		public static float NextFloat(this IRandGen gen, int exp = 0)
		{
			// Uniformly distributed float between 0 and 1. 

			UInt32 val = gen.NextUint() >> 8; // 23 bits of mantissa, not including possible leading 1. This is definitely representable in a float.
			return (float)val / (1 << 23);
		}

		public static double NextDouble(this IRandGen gen, int exp = 0)
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
	}
}

