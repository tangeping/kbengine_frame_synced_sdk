using System;
using System.Collections.Generic;

namespace KBEngine
{
    // ֡ͬ���й���Ϊ��
	public class FrameSyncManagedBehaviour : IFrameSyncBehaviourGamePlay, IFrameSyncBehaviour, IFrameSyncBehaviourCallbacks
	{
		public IFrameSyncBehaviour FrameSyncBehavior;

		[AddTracking]
		public bool disabled;

		public FPPlayerInfo localOwner;

		public FPPlayerInfo owner;

		public FrameSyncManagedBehaviour(IFrameSyncBehaviour FrameSyncBehavior)
		{
			StateTracker.AddTracking(this);
			StateTracker.AddTracking(FrameSyncBehavior);
			this.FrameSyncBehavior = FrameSyncBehavior;
		}

        #region IFrameSyncBehaviourGamePlay �ӿڷ���
        public void OnPreSyncedUpdate()
		{
			bool flag = this.FrameSyncBehavior is IFrameSyncBehaviourGamePlay;
			if (flag)
			{
				((IFrameSyncBehaviourGamePlay)this.FrameSyncBehavior).OnPreSyncedUpdate();
			}
		}

		public void OnSyncedInput()
		{
			bool flag = this.FrameSyncBehavior is IFrameSyncBehaviourGamePlay;
			if (flag)
			{
				((IFrameSyncBehaviourGamePlay)this.FrameSyncBehavior).OnSyncedInput();
			}
		}

		public void OnSyncedUpdate()
		{
			bool flag = this.FrameSyncBehavior is IFrameSyncBehaviourGamePlay;
			if (flag)
			{
				((IFrameSyncBehaviourGamePlay)this.FrameSyncBehavior).OnSyncedUpdate();
			}
		}
        #endregion IFrameSyncBehaviourGamePlay �ӿڷ���

        #region IFrameSyncBehaviour �ӿڷ���
        public void SetGameInfo(FPPlayerInfo localOwner, int numberOfPlayers)
		{
			//this.FrameSyncBehavior.SetGameInfo(localOwner, numberOfPlayers);
		}
        #endregion IFrameSyncBehaviour �ӿڷ���

        #region �������ڷ���
        // ��ʼͬ��
        public static void OnGameStarted(List<FrameSyncManagedBehaviour> generalBehaviours, Dictionary<byte, List<FrameSyncManagedBehaviour>> behaviorsByPlayer)
        {
            int i = 0;
            int count = generalBehaviours.Count;
            while (i < count)
            {
                generalBehaviours[i].OnSyncedStart();
                i++;
            }
            Dictionary<byte, List<FrameSyncManagedBehaviour>>.Enumerator enumerator = behaviorsByPlayer.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<byte, List<FrameSyncManagedBehaviour>> current = enumerator.Current;
                List<FrameSyncManagedBehaviour> value = current.Value;
                int j = 0;
                int count2 = value.Count;
                while (j < count2)
                {
                    value[j].OnSyncedStart();
                    j++;
                }
            }
        }

        // ��Ϸ��ͣ
        public static void OnGamePaused(List<FrameSyncManagedBehaviour> generalBehaviours, Dictionary<byte, List<FrameSyncManagedBehaviour>> behaviorsByPlayer)
        {
            int i = 0;
            int count = generalBehaviours.Count;
            while (i < count)
            {
                generalBehaviours[i].OnGamePaused();
                i++;
            }
            Dictionary<byte, List<FrameSyncManagedBehaviour>>.Enumerator enumerator = behaviorsByPlayer.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<byte, List<FrameSyncManagedBehaviour>> current = enumerator.Current;
                List<FrameSyncManagedBehaviour> value = current.Value;
                int j = 0;
                int count2 = value.Count;
                while (j < count2)
                {
                    value[j].OnGamePaused();
                    j++;
                }
            }
        }

        // ȡ����ͣ
        public static void OnGameUnPaused(List<FrameSyncManagedBehaviour> generalBehaviours, Dictionary<byte, List<FrameSyncManagedBehaviour>> behaviorsByPlayer)
        {
            int i = 0;
            int count = generalBehaviours.Count;
            while (i < count)
            {
                generalBehaviours[i].OnGameUnPaused();
                i++;
            }
            Dictionary<byte, List<FrameSyncManagedBehaviour>>.Enumerator enumerator = behaviorsByPlayer.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<byte, List<FrameSyncManagedBehaviour>> current = enumerator.Current;
                List<FrameSyncManagedBehaviour> value = current.Value;
                int j = 0;
                int count2 = value.Count;
                while (j < count2)
                {
                    value[j].OnGameUnPaused();
                    j++;
                }
            }
        }

        // ��Ϸ����
        public static void OnGameEnded(List<FrameSyncManagedBehaviour> generalBehaviours, Dictionary<byte, List<FrameSyncManagedBehaviour>> behaviorsByPlayer)
        {
            int i = 0;
            int count = generalBehaviours.Count;
            while (i < count)
            {
                generalBehaviours[i].OnGameEnded();
                i++;
            }
            Dictionary<byte, List<FrameSyncManagedBehaviour>>.Enumerator enumerator = behaviorsByPlayer.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<byte, List<FrameSyncManagedBehaviour>> current = enumerator.Current;
                List<FrameSyncManagedBehaviour> value = current.Value;
                int j = 0;
                int count2 = value.Count;
                while (j < count2)
                {
                    value[j].OnGameEnded();
                    j++;
                }
            }
        }

        // ��ҶϿ�����
        public static void OnPlayerDisconnection(List<FrameSyncManagedBehaviour> generalBehaviours, Dictionary<byte, List<FrameSyncManagedBehaviour>> behaviorsByPlayer, byte playerId)
        {
            int i = 0;
            int count = generalBehaviours.Count;
            while (i < count)
            {
                generalBehaviours[i].OnPlayerDisconnection((int)playerId);
                i++;
            }
            Dictionary<byte, List<FrameSyncManagedBehaviour>>.Enumerator enumerator = behaviorsByPlayer.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<byte, List<FrameSyncManagedBehaviour>> current = enumerator.Current;
                List<FrameSyncManagedBehaviour> value = current.Value;
                int j = 0;
                int count2 = value.Count;
                while (j < count2)
                {
                    value[j].OnPlayerDisconnection((int)playerId);
                    j++;
                }
            }
        }
        #endregion �������ڷ���

        #region IFrameSyncBehaviourCallbacks �ӿڷ���
        // ��ʼͬ��
        public void OnSyncedStart()
		{
			bool flag = this.FrameSyncBehavior is IFrameSyncBehaviourCallbacks;
			if (flag)
			{
				((IFrameSyncBehaviourCallbacks)this.FrameSyncBehavior).OnSyncedStart();
// 				bool flag2 = this.localOwner.Id == this.owner.Id;
// 				if (flag2) // �������
// 				{
// 					((IFrameSyncBehaviourCallbacks)this.FrameSyncBehavior).OnSyncedStartLocalPlayer();
// 				}
			}
		}

        // ��ʼͬ���������
        public void OnSyncedStartLocalPlayer()
        {
            throw new NotImplementedException();
        }

        // ��Ϸ��ͣ
		public void OnGamePaused()
		{
			bool flag = this.FrameSyncBehavior is IFrameSyncBehaviourCallbacks;
			if (flag)
			{
				((IFrameSyncBehaviourCallbacks)this.FrameSyncBehavior).OnGamePaused();
			}
		}

        // ȡ����ͣ
		public void OnGameUnPaused()
		{
			bool flag = this.FrameSyncBehavior is IFrameSyncBehaviourCallbacks;
			if (flag)
			{
				((IFrameSyncBehaviourCallbacks)this.FrameSyncBehavior).OnGameUnPaused();
			}
		}

        // ��Ϸ����
		public void OnGameEnded()
		{
			bool flag = this.FrameSyncBehavior is IFrameSyncBehaviourCallbacks;
			if (flag)
			{
				((IFrameSyncBehaviourCallbacks)this.FrameSyncBehavior).OnGameEnded();
			}
		}

        // ��ҶϿ�����
		public void OnPlayerDisconnection(int playerId)
		{
			bool flag = this.FrameSyncBehavior is IFrameSyncBehaviourCallbacks;
			if (flag)
			{
				((IFrameSyncBehaviourCallbacks)this.FrameSyncBehavior).OnPlayerDisconnection(playerId);
			}
		}

        #endregion IFrameSyncBehaviourCallbacks �ӿڷ���
    }
}
