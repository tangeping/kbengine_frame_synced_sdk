using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KBEngine
{
    [System.Serializable]

    public class AstarData
    {
        /// <summary>
        /// Shortcut to the first GridGraph.
        /// Updated at scanning time
        /// </summary>
        public Grid gridGraph = null; 

        /// <summary>
        /// Serialized data for cached startup.
        /// If set, on start the graphs will be deserialized from this file.
        /// </summary>
        public TextAsset file_cachedStartup;

        /// <summary>Load from data from <see cref="file_cachedStartup"/></summary>
        public void LoadFromCache()
        {
            if (file_cachedStartup != null)
            {
                CahcheGridData cachData = JsonConvert.DeserializeObject<CahcheGridData>(file_cachedStartup.text);

                this.gridGraph =  cachData.DeserializeToGrid();

                if(this.gridGraph.DimX >0  && this.gridGraph.DimY > 0)
                {
                    Debug.Log("Load " + file_cachedStartup.name + " Successfully!!!");
                }               
            }
            else
            {
                Debug.LogError("file_cachedStartup is null");
            }
        }

        /// <summary>
        /// Serializes all graphs settings to a byte array.
        /// See: DeserializeGraphs(byte[])
        /// </summary>e
        public string SerializeGraphs()
        {
            return SeriliazeData();
        }

        public string SeriliazeData()
        {
            if(gridGraph == null)
            {
                return string.Empty;
            }

            return CBJsonUntity.SerializeToJsonString(new CahcheGridData(gridGraph));
        }
    }

    [System.Serializable]
    public struct CahcheGridData
    {
        public long[,] weights;
        public Long3 orgin;
        public Vector2Int shape;

        public CahcheGridData(Grid g)
        {
            this.weights = new long[g.DimY,g.DimX];
            FPVector gridOrgin = g.GetOrgin();
            this.orgin = new Long3(gridOrgin.x.RawValue, gridOrgin.y.RawValue, gridOrgin.z.RawValue);
            this.shape = g.GetShape();

            for (int i = 0; i < g.DimY; i++)
            {
                for (int j = 0; j < g.DimX; j++)
                {
                    Vector2Int nodeIndex = new Vector2Int(i, j);
                    this.weights[i, j] = g.GetCellCost(nodeIndex).RawValue;
                }
            }
        }

        public Grid DeserializeToGrid()
        {
            int row = this.weights.GetLength(0);
            int col = this.weights.GetLength(1);

            Grid g = new Grid( col, row, this.orgin.ToFPVector(), this.shape);

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    Vector2Int index = new Vector2Int(i, j);
                    g.SetCellCost(index, FP.FromRaw(this.weights[i, j]));
                }
            }
            return g;
        }
    }
}

