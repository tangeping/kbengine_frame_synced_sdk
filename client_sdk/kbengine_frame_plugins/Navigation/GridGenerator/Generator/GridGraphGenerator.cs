using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KBEngine;
namespace KBEngine {
    public enum NeighboursNumType
    {
        Four,
        Eight,
        Six
    }

    public class GridNavGraph : GraphBase
    {

        #region 参数

        /** 网格大小*/
        public Vector2 gridDefineSize;

        public Vector2 gridSize;

        /** 网格中心点 */
        public Vector3 gridCenter;

        /** 世界单位下节点的大小 */
        public float nodeSize;

        /** 网格宽度的节点数 */
        public int nodesInWidth;

        /** 网格高度/深度的节点数 */
        public int nodesInDepth;

        /** 网格纵横比 */
        public float gridAspectRatio = 1F;

        /** 网格旋转的角度 */
        public Vector3 gridRotation;

        /** 网格外框 */
        public Bounds outLineBound;

        /**网格外框等距角度，调节外框缩放形状等*/
        public float boundsIsometricAngle;

        /** outLineBoundMatrix的矩阵 */
        public Matrix4x4 outLineBoundMatrix { get; protected set; }


        /** 节点可行走的最大坡度 */
        public float maxSlope = 50F;

        /** 两个节点间连接允许的最大位置差 */
        public float maxDiffPosition = 0.4F;

        /** 节点间的位置差以哪根轴为基本计算， X = 0, Y = 1, Z = 2*/
        public int useAxisOfMaxDiffPosition = 1;

        /** 节点数 */
        public GridGraphNode[] nodes;

        /** 每个节点的邻居数,分别有4、6、8个连接,6个连接主要用于仿真六边形图形。*/
        public NeighboursNumType neighbours = NeighboursNumType.Eight;

        /** 邻居方向偏移, 八个方向
         *  6  2   5
         *  3 node 1
         *  7  0   4
         */
        public readonly int[] neighbourNodeOffsets = new int[8];

        /** 邻居节点的花费代价 */
        public readonly uint[] neighbourNodeCosts = new uint[8];

        /** 相邻节点的X方向偏移量。只有1 0或 -1 */
        public readonly int[] neighbourNodeXOffsets = new int[8];

        /** 相邻节点的Z方向偏移量。只有1 0或 -1 */
        public readonly int[] neighbourNodeZOffsets = new int[8];

        /** 当neighbors = 6时即六角邻居节点，将使用哪些邻居 */
        static readonly int[] hexagonNeighbourNodeIndices = { 0, 1, 2, 3, 5, 7 };


        /** 处理边缘的成本标志：true为所有边缘成本都一样，false为对角成本更高，长度跟法线流一致 */
        public bool uniformEdgeCosts;

        /** 节点间连接的颜色 */
        public Color nodeColorOfConnection;

        /** 处理图形冲突的类*/
        public GraphicCollisionHandle graphicCollision;

        /** 在障碍物的拐角处是否要进行裁剪*/
        public bool cutCornersOfObstacles = true;

        /** 临时的缓存，用于减少内存分配*/
        public int[] buffers;
        public bool useRaycastNormal { get { return Math.Abs(90 - maxSlope) > float.Epsilon; } }


        /** 是否侵蚀标志*/
        public bool gridErosionFlag = false;
        public int numOfErosion = 2;

        /*存储的网格数据*/
        public int[] gridDatas;
        #endregion


        public GridNavGraph(int graphIndex)
        {
            this.graphIndex = graphIndex;
            initGraphData();
            ScanGraphicInternal();
        }

        /// <summary>
        /// 初始化图层数据
        /// </summary>
        public void initGraphData()
        {
            nodesInWidth = 200;
            nodesInDepth = 200;
            nodeSize = 2.5F;
            gridDefineSize = new Vector2(nodesInWidth, nodesInDepth) * nodeSize;
            gridSize = gridDefineSize;
            gridCenter = Matrix.MultiplyPoint3x4( new Vector3((nodesInWidth / 2 * nodeSize ), 0, (nodesInDepth / 2 * nodeSize)));

            //默认为绿色
            nodeColorOfConnection = new Color(0, 1, 0, 0.5f);
            graphicCollision = new GraphicCollisionHandle();
            graphicCollision.collisionCheck = true;
            graphicCollision.maskOfCollisionCheck.value = LayerMask.GetMask("TransparentFX");
            graphicCollision.heightCheck = true;
            graphicCollision.maskOfHeightCheck.value = LayerMask.GetMask("Default");

    }

        /// <summary>
        /// 扫描图形内部
        /// </summary>
        public override void ScanGraphicInternal()
        {
            if (nodeSize <= 0) //默认设置为最小数值
                nodeSize = 0.1f;

            UpdateBaseArgs();

            if (nodesInWidth > 1024)
            {
                Debug.LogError("the grid's sides(nodesInWidth) is longer than 1024 nodes!!");
                return;
            }

            if (nodesInDepth > 1024)
            {
                Debug.LogError("the grid's sides(nodesInDepth) is longer than 1024 nodes!!");
                return;
            }

            //设置偏移值和代价
            SetValuesOfOffsetsAndCosts();

            //设置当前当前的网格图层对象
            GridGraphNode.SetGridNavGraph(this.graphIndex, this);

            if (nodes != null)
            {
                for (int i = 0; i < nodes.Length; i++)
                {
                    nodes[i].Destory();
                }
            }
         
            //创建节点
            nodes = new GridGraphNode[nodesInWidth * nodesInDepth];

            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i] = new GridGraphNode();
                nodes[i].graphIndex = this.graphIndex;
                nodes[i].nodeIndex = i;
            }

            //处理图形冲突,高度坡度检查、障碍物等
            HandingGraphicCollision();

            //处理节点间的连接
            HandingNodesConnections();

            //处理侵蚀的部分
            ErosionNodeOfArea(0, 0, nodesInWidth, nodesInDepth);
        }

        public void UpdateBaseArgs()
        {
           // gridDefineSize = new Vector2(nodesInWidth, nodesInDepth) * nodeSize;
           // gridSize = gridDefineSize;
           // gridCenter = Matrix.MultiplyPoint3x4(new Vector3(nodesInWidth / 2F, 0, nodesInDepth / 2F));
            HandingBaseArgAndMatrix();
        }

        /// <summary>
        /// 通过编辑器更新nodeSize的数值
        /// </summary>
        /// <param name="nodeSize"></param>
        public void UpdateNodeSizeFromEditorChange(float nodeSize)
        {
            float delta = nodeSize / this.nodeSize;
            this.nodeSize = nodeSize;
            gridDefineSize = new Vector2(nodesInWidth, nodesInDepth) * nodeSize;
            gridCenter = Matrix.MultiplyPoint3x4(new Vector3((nodesInWidth / 2F * delta), 0, (nodesInDepth / 2F * delta)));
            UpdateBaseArgs();
        }

        /// <summary>
        /// 处理基本参数
        /// </summary>
        public void HandingBaseArgAndMatrix()
        {
            var tmpSize = gridDefineSize;

            //确保为正整数
            tmpSize.x *= Mathf.Sign(tmpSize.x);
            tmpSize.y *= Mathf.Sign(tmpSize.y);

            //确保gridSize的数值不会大于1024*1024
            nodeSize = Mathf.Clamp(nodeSize, tmpSize.x / 1024F, Mathf.Infinity); // Are you sure this calculates is never larger than 1024*1024 ?
            nodeSize = Mathf.Clamp(nodeSize, tmpSize.y / 1024F, Mathf.Infinity);

            // Prevent the graph to become smaller than a single node
            tmpSize.x = tmpSize.x < nodeSize ? nodeSize : tmpSize.x;
            tmpSize.y = tmpSize.y < nodeSize ? nodeSize : tmpSize.y;

            gridSize = tmpSize;
        
            //生成一个沿着对角线收缩图形的矩阵，(isometricMatrix)等距矩阵
            var isometricMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 45, 0), Vector3.one);
            isometricMatrix = Matrix4x4.Scale(new Vector3(Mathf.Cos(Mathf.Deg2Rad * boundsIsometricAngle), 1, 1)) * isometricMatrix;
            isometricMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, -45, 0), Vector3.one) * isometricMatrix;

            outLineBoundMatrix = Matrix4x4.TRS(gridCenter, Quaternion.Euler(gridRotation), new Vector3(gridAspectRatio, 1, 1)) * isometricMatrix;
            
            //计算nodes个数
            nodesInWidth = Mathf.FloorToInt(gridSize.x / nodeSize);
            nodesInDepth = Mathf.FloorToInt(gridSize.y / nodeSize);



            // 处理数值边界的情况
            if (Mathf.Approximately(gridSize.x / nodeSize, Mathf.CeilToInt(gridSize.x / nodeSize)))
                nodesInWidth = Mathf.CeilToInt(gridSize.x / nodeSize);

            if (Mathf.Approximately(gridSize.y / nodeSize, Mathf.CeilToInt(gridSize.y / nodeSize)))
                nodesInDepth = Mathf.CeilToInt(gridSize.y / nodeSize);

            Matrix = Matrix4x4.TRS(outLineBoundMatrix.MultiplyPoint3x4(-new Vector3(gridSize.x, 0, gridSize.y) * 0.5F), Quaternion.Euler(gridRotation), new Vector3(nodeSize * gridAspectRatio, 1, nodeSize)) * isometricMatrix;
        }

        /// <summary>
        /// 设置节点邻居偏移值和代价
        /// </summary>
        public virtual void SetValuesOfOffsetsAndCosts()
        {
            //代表相邻节点,上下左右
            neighbourNodeOffsets[0] = -nodesInWidth;
            neighbourNodeOffsets[1] = 1;
            neighbourNodeOffsets[2] = nodesInWidth;
            neighbourNodeOffsets[3] = -1;

            //代表对角节点,左上左下右上右下
            neighbourNodeOffsets[4] = -nodesInWidth + 1;
            neighbourNodeOffsets[5] = nodesInWidth + 1;
            neighbourNodeOffsets[6] = nodesInWidth - 1;
            neighbourNodeOffsets[7] = -nodesInWidth - 1;



            uint straightCost = (uint)Mathf.RoundToInt(nodeSize * Int3.Precision);
            uint diagonalCost = uniformEdgeCosts ? straightCost : (uint)Mathf.RoundToInt(nodeSize * Mathf.Sqrt(2F) * Int3.Precision);

            //处理邻居节点
            neighbourNodeCosts[0] = straightCost;
            neighbourNodeCosts[1] = straightCost;
            neighbourNodeCosts[2] = straightCost;
            neighbourNodeCosts[3] = straightCost;

            //处理对角
            neighbourNodeCosts[4] = diagonalCost;
            neighbourNodeCosts[5] = diagonalCost;
            neighbourNodeCosts[6] = diagonalCost;
            neighbourNodeCosts[7] = diagonalCost;

            //x轴方向的偏移量
            neighbourNodeXOffsets[0] = 0;
            neighbourNodeXOffsets[1] = 1;
            neighbourNodeXOffsets[2] = 0;
            neighbourNodeXOffsets[3] = -1;
            neighbourNodeXOffsets[4] = 1;
            neighbourNodeXOffsets[5] = 1;
            neighbourNodeXOffsets[6] = -1;
            neighbourNodeXOffsets[7] = -1;

            //z轴方向的偏移量
            neighbourNodeZOffsets[0] = -1;
            neighbourNodeZOffsets[1] = 0;
            neighbourNodeZOffsets[2] = 1;
            neighbourNodeZOffsets[3] = 0;
            neighbourNodeZOffsets[4] = -1;
            neighbourNodeZOffsets[5] = 1;
            neighbourNodeZOffsets[6] = 1;
            neighbourNodeZOffsets[7] = -1;
        }

        /// <summary>
        /// 处理图形冲突
        /// </summary>
        public void HandingGraphicCollision()
        {
            if (graphicCollision == null)
            {
                graphicCollision = new GraphicCollisionHandle();
            }
            graphicCollision.Initialize(Matrix);

            for (int depth = 0; depth < nodesInDepth; depth++)
            {
                for (int width = 0; width < nodesInWidth; width++)
                {
                    var node = nodes[depth * nodesInWidth + width];
                    HandingCollisonOfSingleNode(node, width, depth);
                }
            }
        }

        /// <summary>
        /// 处理单个节点的图层冲突信息
        /// </summary>
        /// <param name="node"></param>
        /// <param name="x"></param>
        /// <param name="z"></param>
        public void HandingCollisonOfSingleNode(GridGraphNode node, int width, int depth)
        {
            float offestNum = 0.5f;
            node.position = (Int3)(Matrix.MultiplyPoint3x4(new Vector3(width + offestNum, 0, depth + offestNum)));

            bool walkAble = false;
            RaycastHit hitInfo;

           //处理高度和坡度检查
           Vector3 position = graphicCollision.CheckHeight((Vector3)node.position, out hitInfo, out walkAble);
           node.position = (Int3)position;

            if (walkAble && graphicCollision.heightCheck)
            {
                if (hitInfo.normal != Vector3.zero)
                {
                    float angle = Vector3.Dot(hitInfo.normal.normalized, graphicCollision.graphUp);
                     
                    if (angle < Mathf.Cos(maxSlope * Mathf.Deg2Rad))
                    {
                        walkAble = false;
                    }
                }
            }

            //检查障碍物
            bool checkObstacles = graphicCollision.Check((Vector3)node.position);

            node.walkAble = checkObstacles && walkAble;
            //   Debug.Log("ConvertedData::" + ConvertedData + ", x::" + x + ", z::" + z + ",node.walkAble::" + node.walkAble);
        }

        /// <summary>
        /// 处理节点连接
        /// </summary>
        public void HandingNodesConnections()
        {
            for (int depth = 0; depth < nodesInDepth; depth++)
            {
                for (int width = 0; width < nodesInWidth; width++)
                {
                    var node = nodes[depth * nodesInWidth + width];

                    // Recalculate connections to other nodes
                    HandingCollisonOfSingleNode(nodes, width, depth, node);
                }
            }
        }

        public void HandingCollisonOfSingleNode(GridGraphNode[] nodes, int width, int depth, GridGraphNode node)
        {
            node.ResetConnectionsInternal();

            //All connections are disabled if the node is not walkable
            if (!node.walkAble)
            {
                return;
            }

            int index = node.nodeIndex;

            if (neighbours == NeighboursNumType.Four || neighbours == NeighboursNumType.Eight)
            {
                // Reset the buffer
                if (buffers == null)
                    buffers = new int[4];
                else
                {
                    for (int i = 0; i < 4; i++)
                    {
                        buffers[i] = 0;
                    }
                }

                for (int i = 0, j = 3; i < 4; j = i, i++)
                {
                    int nx = width + neighbourNodeXOffsets[i];
                    int nz = depth + neighbourNodeZOffsets[i];

                    if (nx < 0 || nz < 0 || nx >= nodesInWidth || nz >= nodesInDepth)
                    {
                        continue;
                    }

                    var other = nodes[index + neighbourNodeOffsets[i]];

                    if (IsValidConnection(node, other))
                    {
                        node.SetConnectionInternal(i, true);

                        // Mark the diagonal/corner adjacent to this connection as used
                        buffers[i]++;
                        buffers[j]++;
                    }
                    else
                    {
                        node.SetConnectionInternal(i, false);
                    }
                }

                // Add in the diagonal connections
                if (neighbours == NeighboursNumType.Eight)
                {
                    if (cutCornersOfObstacles)
                    {
                        for (int i = 0; i < 4; i++)
                        {

                            // If at least one axis aligned connection
                            // is adjacent to this diagonal, then we can add a connection
                            if (buffers[i] >= 1)
                            {
                                int nx = width + neighbourNodeXOffsets[i + 4];
                                int nz = depth + neighbourNodeZOffsets[i + 4];

                                if (nx < 0 || nz < 0 || nx >= width || nz >= depth)
                                {
                                    continue;
                                }

                                GridGraphNode other = nodes[index + neighbourNodeOffsets[i + 4]];

                                node.SetConnectionInternal(i + 4, IsValidConnection(node, other));
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 4; i++)
                        {

                            // If exactly 2 axis aligned connections is adjacent to this connection
                            // then we can add the connection
                            //We don't need to check if it is out of bounds because if both of the other neighbours are inside the bounds this one must be too
                            if (buffers[i] == 2)
                            {
                                GridGraphNode other = nodes[index + neighbourNodeOffsets[i + 4]];

                                node.SetConnectionInternal(i + 4, IsValidConnection(node, other));
                            }
                        }
                    }
                }
            }
            else
            {
                // Hexagon layout

                // Loop through all possible neighbours and try to connect to them
                for (int j = 0; j < hexagonNeighbourNodeIndices.Length; j++)
                {
                    var i = hexagonNeighbourNodeIndices[j];

                    int nx = width + neighbourNodeXOffsets[i];
                    int nz = depth + neighbourNodeZOffsets[i];

                    if (nx < 0 || nz < 0 || nx >= width || nz >= depth)
                    {
                        continue;
                    }

                    var other = nodes[index + neighbourNodeOffsets[i]];

                    node.SetConnectionInternal(i, IsValidConnection(node, other));
                }
            }
        }

        public virtual bool IsValidConnection(GridGraphNode n1, GridGraphNode n2)
        {
            if (!n1.walkAble || !n2.walkAble)
            {
                return false;
            }

            if (maxDiffPosition > 0 && Mathf.Abs(n1.position[useAxisOfMaxDiffPosition] - n2.position[useAxisOfMaxDiffPosition]) > maxDiffPosition * Int3.Precision)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 查看节点是否有连接
        /// </summary>
        /// <param name="node"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        public bool HasConnectionOfNode(GridGraphNode node, int dir)
        {
            if (!node.GetConnectionInternal(dir)) return false;
            if (node.EdgeNode)
            {
                //算出当前的index的行列
                int nodeIndex = node.nodeIndex;
                int depth = nodeIndex / nodesInWidth;
                int width = nodeIndex - depth * nodesInWidth;

                return HasConnectionOfNode(nodeIndex, width, depth, dir);
            }
            else return true;
            
        }

        public bool HasConnectionOfNode(int nodeIndex, int width, int depth, int dir)
        {
            if (!nodes[nodeIndex].GetConnectionInternal(dir)) return false;

            //查看是否到了边界处
            int nx = width + neighbourNodeXOffsets[dir];
            if (nx < 0 || nx >= nodesInWidth) return false; 
            int nz = depth + neighbourNodeZOffsets[dir];
            if (nz < 0 || nz >= nodesInDepth) return false;

            return true;
        }

        /// <summary>
        /// 对区域中的节点做侵蚀，限制一些节点为不可走状态
        /// </summary>
        public void ErosionNodeOfArea(int widthMin, int depthMin, int widthMax, int depthMax)
        {
            widthMin = Mathf.Clamp(widthMin, 0, nodesInWidth);
            widthMax = Mathf.Clamp(widthMax, 0, nodesInWidth);
            depthMin = Mathf.Clamp(depthMin, 0, nodesInDepth);
            depthMax = Mathf.Clamp(depthMax, 0, nodesInDepth);

            if (gridErosionFlag)
            {
                for (int num = 0; num < numOfErosion; num++)
                {
                    for (int depth = depthMin; depth < depthMax; depth++)
                    {
                        for (int width = widthMin; width < widthMax; width++)
                        {
                            var node = nodes[depth * nodesInWidth + width];

                            if (node.walkAble)
                            {
                                if (HandingErosionOfOfAnyFalseConnections(node))
                                {
                                    node.walkAble = false;
                                }
                            }
                        }
                    }

                    //重新计算节点连接
                    HandingNodesConnections();
                }
            }
          
        }

        /// <summary>
        /// True if the node has any blocked connections.
        /// For 4 and 8 neighbours the 4 axis aligned connections will be checked.
        /// For 6 neighbours all 6 neighbours will be checked.
        ///
        /// Internal method used for erosion.
        /// </summary>
        /// 

        bool HandingErosionOfOfAnyFalseConnections(GridGraphNode node)
        {
            // Check the 6 hexagonal connections
            if (neighbours == NeighboursNumType.Six)
            {
                for (int i = 0; i < 6; i++)
                {
                    if (!HasConnectionOfNode(node, hexagonNeighbourNodeIndices[i]))
                        return true;
                }
            }
            else
            {
                // Check the four axis aligned connections
                for (int i = 0; i < 4; i++)
                {
                    if (!HasConnectionOfNode(node, i))
                        return true;
                }
            }
            return false;
        }

        public override void OnDrawGizmos(bool drawFlag)
        {
            if (!drawFlag || nodes == null || nodes.Length != nodesInWidth * nodesInDepth)
                return;

            //绘制outLineBound
            Gizmos.color = Color.white;
            Gizmos.matrix = outLineBoundMatrix;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(gridSize.x, 0, gridSize.y));

            Gizmos.matrix = Matrix4x4.identity;

            //绘制网格走可走区域
            //   Debug.Log("depth::" + nodesInDepth + ",width::" + nodesInWidth);
            Gizmos.color = Color.green;
            for (int depth = 0; depth < nodesInDepth; depth++)
            {
                for (int width = 0; width < nodesInWidth; width++)
                {
                    var node = nodes[depth * nodesInWidth + width];
                    //  Debug.Log("index::" + (depth * nodesInWidth + width) + "node::" + node.nodeIndex + ", depth::" + depth + ", width::" + width + ",node.walkAble::" + node.walkAble);

                    if (!node.walkAble)
                    {
                        continue;
                    }

                    Vector3 position = (Vector3)node.position;
                    for (int i = 0; i < 8; i++)
                    {
                        if (node.GetConnectionInternal(i))
                        {
                            GridGraphNode other = nodes[node.nodeIndex + neighbourNodeOffsets[i]];
                            Gizmos.DrawLine(position, (Vector3)other.position);
                            //  Debug.Log("connect::" + other.nodeIndex  + ".node::" + node.nodeIndex);
                        }
                    }
                    //   Debug.Log("node::" + node.nodeIndex + ", depth::" + depth + ", width::" + width + ",node.walkAble::" + node.walkAble );
                }
            }


            Gizmos.color = Color.blue;
            for (int i = 0; i < nodes.Length; i++)
            {
                if(nodes[i].walkAble)
                    Gizmos.color = Color.red;
                else
                    Gizmos.color = Color.gray;
                Gizmos.DrawCube((Vector3)nodes[i].position, Vector3.one *0.5f);
//                 if (i < 20)
//                 {
//                     Debug.LogFormat("node[{0}].position:{1}", i, nodes[i].position);
//                 }
            }
        }


        public static explicit operator Grid(GridNavGraph ob)
        {
            if(ob == null)
            {
                return null;
            }

            Vector2Int shape = new Vector2Int(Mathf.FloorToInt(ob.nodeSize * ob.nodesInWidth), Mathf.FloorToInt(ob.nodeSize * ob.nodesInDepth));
            Grid g = new Grid(ob.nodesInWidth, ob.nodesInDepth, FPVector.zero, shape);

            for (int i = 0; i < ob.nodes.Length; i++)
            {
                Vector2Int index = new Vector2Int(i % ob.nodesInWidth, i / ob.nodesInWidth);

                if(ob.nodes[i].walkAble)
                {
                    g.UnblockCell(index);
                }
                else
                {
                    g.BlockCell(index);
                }               
            }
            return g;
        }


        /// <summary>
        /// 获取网格数据
        /// </summary>
        public void GetGraphData()
        {
            if (nodes == null) return;
            gridDatas = new int[nodesInWidth * nodesInDepth];
            for (int i = 0; i < nodes.Length; i++)
            {
                gridDatas[i] = Convert.ToInt32(nodes[i].walkAble);
              //  Debug.Log("i::" + i + ",nodes[i].walkAble::" + nodes[i].walkAble + ",gridDatas[i]::" + gridDatas[i]);
            }
        }

        /// <summary>
        /// 导出图层数据
        /// </summary>
        public override bool ExportGraphData()
        {
            //             GetGraphData();
            //             Dictionary<string, int[]> tmpData = new Dictionary<string, int[]>();
            //             tmpData.Add("weights", gridDatas);
            //             return GraphJsonUtility.ExportGridDatas(tmpData);

            Grid g = (Grid)this;
            return GraphJsonUtility.ExportGridDatas(new CahcheGridData(g));
        }


    }


}