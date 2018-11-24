using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KBEngine
{
    public class SpaceData : CBSingleton<SpaceData>
    {
        public Entity localPlayer = null;

        public List<Entity> SpacePlayers = new List<Entity>();

        public Queue<FRAME_DATA> frameList = new Queue<FRAME_DATA>();
  
    }
}

