using System;

namespace Worldgen.Util
{
	public class ObjectWithID : IEquatable<ObjectWithID>
	{
		public readonly Guid ObjID;

		public ObjectWithID()
		{
			ObjID = Guid.NewGuid();
		}

		public bool Equals(ObjectWithID o)
		{
			return this.ObjID == o.ObjID;
		}
	}
}

