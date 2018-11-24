using System;
using UnityEngine;

namespace KBEngine
{
    // 帧同步玩家信息
	[Serializable]
	public class FPPlayerInfo
	{
		[SerializeField]
		internal byte id;

		[SerializeField]
		internal string name;

		public byte Id
		{
			get
			{
				return this.id;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public FPPlayerInfo(byte id, string name)
		{
			this.id = id;
			this.name = name;
		}
	}
}
