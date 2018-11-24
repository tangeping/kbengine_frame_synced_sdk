using System;

namespace KBEngine
{
	public interface IBody
	{
		bool TSDisabled
		{
			get;
			set;
		}

		string Checkum();
	}
}
