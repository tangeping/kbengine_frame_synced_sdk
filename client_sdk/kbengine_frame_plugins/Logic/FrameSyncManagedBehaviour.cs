using System;
using System.Collections.Generic;

namespace KBEngine
{
    // 帧同步托管行为类
	public class FrameSyncManagedBehaviour : IFrameSyncBehaviourGamePlay, IFrameSyncBehaviour, IFrameSyncBehaviourCallbacks
	{
		public IFrameSyncBehaviour FrameSyncBehavior;

		[AddTracking]
		public bool disabled;

		public FPPlayerInfo localOwner;

		public FPPlayerInfo owner;

        public bool started = false;

		public FrameSyncManagedBehaviour(IFrameSyncBehaviour FrameSyncBehavior)
		{
			StateTracker.AddTracking(this);
			StateTracker.AddTracking(FrameSyncBehavior);
			this.FrameSyncBehavior = FrameSyncBehavior;
		}

        #region IFrameSyncBehaviourGamePlay 接口方法
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
			if (flag && started)
			{
				((IFrameSyncBehaviourGamePlay)this.FrameSyncBehavior).OnSyncedUpdate();
			}
		}
        #endregion IFrameSyncBehaviourGamePlay 接口方法

        #region IFrameSyncBehaviour 接口方法
        public void SetGameInfo(FPPlayerInfo localOwner, int numberOfPlayers)
		{
			//this.FrameSyncBehavior.SetGameInfo(localOwner, numberOfPlayers);
		}
        #endregion IFrameSyncBehaviour 接口方法

        #region 生命周期方法
        // 开始同步
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

        // 游戏暂停
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

        // 取消暂停
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

        // 游戏结束
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

        // 玩家断开连接
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
        #endregion 生命周期方法

        #region IFrameSyncBehaviourCallbacks 接口方法
        // 开始同步
        public void OnSyncedStart()
		{
			bool flag = this.FrameSyncBehavior is IFrameSyncBehaviourCallbacks;
			if (flag && !started)
			{
				((IFrameSyncBehaviourCallbacks)this.FrameSyncBehavior).OnSyncedStart();
                started = true;
// 				bool flag2 = this.localOwner.Id == this.owner.Id;
// 				if (flag2) // 本地玩家
// 				{
// 					((IFrameSyncBehaviourCallbacks)this.FrameSyncBehavior).OnSyncedStartLocalPlayer();
// 				}
			}
		}

        // 开始同步本地玩家
        public void OnSyncedStartLocalPlayer()
        {
            throw new NotImplementedException();
        }

        // 游戏暂停
		public void OnGamePaused()
		{
			bool flag = this.FrameSyncBehavior is IFrameSyncBehaviourCallbacks;
			if (flag)
			{
				((IFrameSyncBehaviourCallbacks)this.FrameSyncBehavior).OnGamePaused();
			}
		}

        // 取消暂停
		public void OnGameUnPaused()
		{
			bool flag = this.FrameSyncBehavior is IFrameSyncBehaviourCallbacks;
			if (flag)
			{
				((IFrameSyncBehaviourCallbacks)this.FrameSyncBehavior).OnGameUnPaused();
			}
		}

        // 游戏结束
		public void OnGameEnded()
		{
			bool flag = this.FrameSyncBehavior is IFrameSyncBehaviourCallbacks;
			if (flag)
			{
				((IFrameSyncBehaviourCallbacks)this.FrameSyncBehavior).OnGameEnded();
			}
		}

        // 玩家断开连接
		public void OnPlayerDisconnection(int playerId)
		{
			bool flag = this.FrameSyncBehavior is IFrameSyncBehaviourCallbacks;
			if (flag)
			{
				((IFrameSyncBehaviourCallbacks)this.FrameSyncBehavior).OnPlayerDisconnection(playerId);
			}
		}

        #endregion IFrameSyncBehaviourCallbacks 接口方法
    }
}
