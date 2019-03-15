using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KBEngine
{
//     [ExecuteInEditMode]
//     [AddComponentMenu("FrameSync/AstarPath")]

    public class AstarPath : MonoBehaviour
    {
        /// <summary>
        /// 当前地图地形
        /// </summary>
        public GridShapeType grapType = GridShapeType.Square;

        /// <summary>
        /// 当前地图地形
        /// </summary>
        public Terrain m_terrian;
        /// <summary>
        /// 当前地图地形行数
        /// </summary>
        public int gridX = 80;

        /// <summary>
        /// 当前地图地形宽度
        /// </summary>
        public int gridY = 80;

        /// <summary>
        /// 当前地图地形数据
        /// </summary>
        public AstarData m_data;




        private static AstarPath instance;

        public static Grid gridGraph
        {
            get
            {
                if(instance == null)
                {
                    return null;
                }
                return instance.m_data.gridGraph;
            }
        }

        public bool IsWalkable(Vector3 point)
        {
            RaycastHit outhit;
            //             int layer = 1 << LayerMask.NameToLayer("obstacle");
            //             return !Physics.Raycast(new Vector3(point.x, 100.0f, point.z), -Vector3.up, out outhit, Mathf.Infinity, layer);

            if (Physics.Raycast(new Vector3(point.x, 100.0f, point.z), -Vector3.up, out outhit))
            {
                if (outhit.collider.gameObject.layer == LayerMask.NameToLayer("ground"))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 加载地图数据
        /// </summary>
        public void LoadMap()
        {
            if (this.m_terrian == null)
            {
                Debug.LogError("地形为空!");
                return;
            }
            if (!(gridY > 0 && gridX > 0) || !(gridX % 2 == 0 && gridY % 2 == 0))
            {
                Debug.LogError("矩阵节点数必须大于0且被2整除！");
                return;
            }
            TerrainData data = m_terrian.terrainData;
            //Debug.Log("m_terrian:" + m_terrian.transform.position +",1:"+ ((FP)1).RawValue);

            this.m_data.gridGraph = new Grid(gridX, gridY, m_terrian.transform.position.ToFPVector(), 
                new Vector2Int((int)data.size.x, (int)data.size.z));

            this.SetWalkable();
        }

        public void SetWalkable()
        {
            for (int i = 0; i < this.gridX; i++)
            {
                for (int j = 0; j < this.gridY; j++)
                {
                    FPVector point = this.m_data.gridGraph.GetCenterPoint(i, j);
                    Vector2Int nodeIndex = new Vector2Int(i, j);

                    if (IsWalkable(point.ToVector()))
                    {
                        this.m_data.gridGraph.UnblockCell(nodeIndex);
                        //Debug.Log("point:" + point + ",nodeIndex:" + nodeIndex);
                    }
                    else
                    {
                        this.m_data.gridGraph.BlockCell(nodeIndex);
                    }
                }
            }
        }
 
        /// <summary>
        /// 序列化网格数据
        /// </summary>
        /// 
        public string SerializeGrapData()
        {
            LoadMap();
            return this.m_data.SeriliazeData();
        }

        private void Awake()
        {          
            m_data.LoadFromCache();

            instance = this;
        }

        private void Start()
        {
                
        }
    }

    public enum GridShapeType
    {
        /// <summary>
        /// 正方形
        /// </summary>
        Square,
        /// <summary>
        /// 正六边形
        /// </summary>
        RegularHexagon,
    }

}

