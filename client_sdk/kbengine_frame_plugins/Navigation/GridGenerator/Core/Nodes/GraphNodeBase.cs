using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KBEngine
{
    public abstract class GraphNodeBase
    {
        public int nodeIndex;
        public int graphIndex = -1;

        public uint walkFlags;

        public bool walkAble
        {
            get
            {
                return (walkFlags) != 0;
            }
            set
            {
                walkFlags = (value ? 1U : 0U);
            }
        }

        public virtual void Destory()
        {
            if (graphIndex == -1 && nodeIndex == -1) return;
            nodeIndex = -1;
            graphIndex = -1;
            walkFlags = 0;
        }
    }

}