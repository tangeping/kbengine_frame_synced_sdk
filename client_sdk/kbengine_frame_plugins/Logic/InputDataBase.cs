using System;
using System.Collections.Generic;

namespace KBEngine
{
	[Serializable]
	public abstract class InputDataBase : ResourcePoolItem
	{
		public int ownerID;

		public InputDataBase()
		{
		}

		public abstract void Serialize(List<byte> bytes);

		public abstract void Deserialize(byte[] data, ref int offset);

        public abstract ENTITY_DATA Serialize();

        public abstract void Deserialize(ENTITY_DATA e);

        public abstract bool EqualsData(InputDataBase otherBase);

		public abstract void CleanUp();

		public abstract void CopyFrom(InputDataBase fromBase);
	}
}
