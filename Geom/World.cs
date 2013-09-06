using System;
using System.Collections.Generic;

namespace Geom
{
	public class Layer<TValue> : Util.ObjectWithID
	{
	}

	public class World
	{
		public static readonly Layer<double> Height = new Layer<double>();
		public static readonly Layer<double> WaterTable = new Layer<double>();
		public readonly Grid Grid;
		private IDictionary<Util.ObjectWithID, System.Collections.IList> Layers;

		public World(Grid g)
		{
			Grid = g;
			Layers = new Dictionary<Util.ObjectWithID, System.Collections.IList>();
			AddLayer(Height, Uniform(6378.1));
		}

		public IList<TVal> GetLayer<TVal>(Layer<TVal> layerName)
		{
			return Layers[layerName] as IList<TVal>;
		}

		public void AddLayer<TVal>(Layer<TVal> layerName, IList<TVal> content)
		{
			if (Layers.ContainsKey(layerName)) {
				throw new ArgumentException("Layer already exists");
			}
			Layers.Add(layerName, (System.Collections.IList)content);
		}

		public IList<TValue> Uniform<TValue>(TValue def)
		{
			var ret = new TValue[Grid.Count];
			for (int i = 0; i < Grid.Count; i++) {
				ret[i] = def;
			}
			return ret;
		}
	}
}

