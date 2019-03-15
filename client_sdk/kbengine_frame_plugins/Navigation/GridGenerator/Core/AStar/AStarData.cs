using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KBEngine;


namespace KBEngine
{
    public class AStarData
    {
        public enum GraphType
        {
            GridGraph = 1
        }

        static public int graphIndex = 0;
        static public List<GraphBase> graphs = new List<GraphBase>();

        static public void AddGrapData(GraphBase graphBase)
        {
            if (!graphs.Contains(graphBase))
            {   
                graphs.Add(graphBase);
            }
        }

        static public void RemoveGrapData(GraphBase graphBase)
        {
            if (graphs.Contains(graphBase))
            {
                graphs.Remove(graphBase);
            }
        }


    }
}
