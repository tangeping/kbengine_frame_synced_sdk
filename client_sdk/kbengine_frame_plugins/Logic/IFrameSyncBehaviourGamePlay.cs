using System;

namespace KBEngine
{
	public interface IFrameSyncBehaviourGamePlay : IFrameSyncBehaviour
	{
		void OnPreSyncedUpdate();

		void OnSyncedInput();

		void OnSyncedUpdate();
	}
}
