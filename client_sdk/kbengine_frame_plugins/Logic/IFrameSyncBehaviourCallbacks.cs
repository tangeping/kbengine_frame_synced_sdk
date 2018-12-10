using System;

namespace KBEngine
{
	public interface IFrameSyncBehaviourCallbacks : IFrameSyncBehaviour
	{
		void OnSyncedStart();

		void OnSyncedStartLocalPlayer();

		void OnGamePaused();

		void OnGameUnPaused();

		void OnGameEnded();

		void OnPlayerDisconnection(int playerId);

	}
}
