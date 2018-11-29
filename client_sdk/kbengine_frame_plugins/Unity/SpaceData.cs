using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KBEngine
{
    public class SpaceData : CBSingleton<SpaceData>
    {
        public FrameSyncReportBase localPlayer = null;

        public List<FrameSyncReportBase> SpacePlayers = new List<FrameSyncReportBase>();

        public Queue<FS_FRAME_DATA> frameList = new Queue<FS_FRAME_DATA>();
  
    }
}

