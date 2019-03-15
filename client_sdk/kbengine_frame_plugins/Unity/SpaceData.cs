using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KBEngine
{
    public enum ROAD_TYPE { TOP_ROAD = 1,MIDDLE_ROAD=2,BOTTOM_ROAD=3};
    public enum Team { Unkown=0, Good=1, Evil=2, Neutral=3 };
    public enum GameOverType { UNKOWN = 0 , WIN = 1, LOSE = 2};

    public class FileBlock
    {
        public UInt16 id = 0;
        public bool compeleted = false;
        private int writeOffset = 0;
        public UInt32 size = 0;
        public byte[] datas = null;

        public FileBlock(UInt16 id, UInt32 size)
        {
            this.id = id;
            this.size = size;
            this.datas = new byte[size];
        }

        public void  WriteBuffer(byte[] buff)
        {
            if (!compeleted)
            {
                Buffer.BlockCopy(this.datas, this.writeOffset, buff, 0, buff.Length);
                this.writeOffset += buff.Length;
            }
        }
    }

    public class SpaceData : CBSingleton<SpaceData>
    {
        public FrameSyncReportBase localPlayer = null;

        public List<FrameSyncReportBase> SpacePlayers = new List<FrameSyncReportBase>();

        public Queue<FS_FRAME_DATA> frameList = new Queue<FS_FRAME_DATA>();

        public int MonsterCount = 0;

        public UInt32 MonsterInitID = 10001;

        public Dictionary<ROAD_TYPE, List<D_ROAD_INFOS>> RoadList = new Dictionary<ROAD_TYPE, List<D_ROAD_INFOS>>();

        public Dictionary<Int32, D_HERO_INFOS> Heros = new Dictionary<int, D_HERO_INFOS>();

        public Dictionary<Int32, D_PROPS_INFOS> Props = new Dictionary<int, D_PROPS_INFOS>();

        public Dictionary<Int32, D_SHOP_INFOS> Shops = new Dictionary<int, D_SHOP_INFOS>();

        public Dictionary<Int32, D_SKILL_INFOS> Skills = new Dictionary<int, D_SKILL_INFOS>();

        public Dictionary<Int32, D_TEAM_INFOS> Teams = new Dictionary<int, D_TEAM_INFOS>();

        public Dictionary<UInt16, FileBlock> files = new Dictionary<UInt16, FileBlock>();

        public string AvatarPerfabFile = string.Empty;

        public GameOverType GameOverResult = GameOverType.UNKOWN;

        public Team getLocalTeam()
        {
            int localTeamID = 0;
            if (localPlayer != null)
            {
                localTeamID = ((KBEngine.AvatarBase)localPlayer.owner).teamID;
            }
            if(Enum.IsDefined(typeof(Team), localTeamID))
            {
                return (Team)localTeamID;
            }
            else
            {
                return Team.Unkown;
            }
        }

        public GameObject getLocalPlayer()
        {
            if(localPlayer == null)
            {
                return null;
            }

            return ((GameObject)(SpaceData.Instance.localPlayer.owner.renderObj));
        }
    }
}

