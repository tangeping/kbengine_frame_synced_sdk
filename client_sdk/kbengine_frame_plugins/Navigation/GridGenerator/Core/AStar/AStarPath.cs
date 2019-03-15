using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KBEngine;
using Newtonsoft.Json;

namespace KBEngine
{
    public class AStarPath : MonoBehaviour
    {
        public System.Action OnDrawGizmosCallback;

        public TextAsset file_cachedStartup = null;

        private Grid _gridGraph = null;

        private static AStarPath instance;

        public static GraphBase Active
        {
            get
            {
                if(AStarData.graphs != null && AStarData.graphs.Count > 0)
                {
                    return AStarData.graphs[0];
                }
                return null;
            }
        }

        public List<GraphBase> Graphs
        {
            get
            {
                return AStarData.graphs;
            }
            set
            {
                AStarData.graphs.Clear();
                for (int i = 0; i < value.Count; i++)
                {
                    value[i].graphIndex = AStarData.graphIndex++;
                }
                AStarData.graphs = value;

            }
        }

        public bool ShowGraphs
        {
            get
            {
                return showGraphs;
            }
            set
            {
                showGraphs = value;
            }
        }

        public static Grid GridGraph
        {
            get
            {
                return instance._gridGraph;
            }
        }

        private bool showGraphs = false;
        private void Awake()
        {
            instance = this;
            // AddGraph(AStarData.GraphType.GridGraph);
            LoadFromCache();          
        }

        // Use this for initialization
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void LoadFromCache()
        {
            if (file_cachedStartup != null)
            {
                CahcheGridData cachData = JsonConvert.DeserializeObject<CahcheGridData>(file_cachedStartup.text);

                this._gridGraph = cachData.DeserializeToGrid();

                if (this._gridGraph.DimX > 0 && this._gridGraph.DimY > 0)
                {
                    Debug.Log("Load " + file_cachedStartup.name + " Successfully!!!");

//                     for (int i = 0; i < 20; i++)
//                     {
//                         Debug.LogFormat("-----------node[{0}] = {1}",i,this.gridGraph.GetCenterPoint(i% this.gridGraph.DimX,i / this.gridGraph.DimX));
//                     }
                }
            }
            else
            {
                Debug.LogError("file_cachedStartup is null");
            }
        }


        public void AddGraph(int type)
        {
            AStarData.GraphType graphType = (AStarData.GraphType)type;
            AddGraph(graphType);
        }

        public void AddGraph(AStarData.GraphType graphType)
        {
            switch (graphType)
            {
                case AStarData.GraphType.GridGraph:
                    {
                        GridNavGraph gridGraph = new GridNavGraph(AStarData.graphIndex++);
                        gridGraph.graphType = (int)graphType;
                        AStarData.AddGrapData(gridGraph);
                    }
                    break;
                default:
                    break;
            }
        }
         
        private void OnDrawGizmos()
        {
            if (OnDrawGizmosCallback != null)
            {
                if (ShowGraphs)
                {
                    OnDrawGizmosCallback();
                }
                   
            }
        }


    }

}
