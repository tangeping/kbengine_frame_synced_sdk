using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KBEngine
{
    public abstract class GraphBase
    {
        #region 参数
 
        /** 图层的索引*/
        public int graphIndex = 0;

        /** 图层名称*/
        public string graphName;

        /** 图层的类型*/
        public int graphType = -1;

        /** 用于平移/旋转/缩放的矩阵*/
        private Matrix4x4 matrix = Matrix4x4.identity;

        /** 逆矩阵*/
        private Matrix4x4 inverseMatrix = Matrix4x4.identity;

        public Matrix4x4 Matrix
        {
            get
            {
                return matrix;
            }

            set
            {
                matrix = value;
                InverseMatrix = matrix.inverse;
            }
        }

        public Matrix4x4 InverseMatrix
        {
            get
            {
                return inverseMatrix;
            }

            set
            {
                inverseMatrix = value;
            }
        }

        
        #endregion


        /// <summary>
        /// 抽象函数:扫描图形内部
        /// </summary>
        public abstract void ScanGraphicInternal();

        /// <summary>
        ///  抽象函数:绘制图层
        /// </summary>
        /// <param name="drawFlag"></param>
        public virtual void OnDrawGizmos(bool drawFlag)
        {

        }

        public virtual bool ExportGraphData()
        {
            return true;
        }

        public virtual void LoadFromGridGraph(TextAsset f)
        {

        }
    }


    /// <summary>
    /// 处理图形冲突的类，用来设置如何检查可行走和高度的类
    /// </summary>
    public class GraphicCollisionHandle
    {
        public GraphicCollisionHandle()
        {
        }

        public Vector3 graphUp;

        /** 冲突检查*/
        public bool collisionCheck = true;

        /**检查射线长度*/
        public float height = 2F;
        private Vector3 upHeight;

        /*检测节点时的向上偏移的值*/
        public float upwardOffsetOfNode;

        public LayerMask maskOfCollisionCheck = -1;

        /** 高度检查*/
        public bool heightCheck = true;

        /** 冲突检测的高度,判断是否冲突*/
        public float collisionHeight;

        /** 高度测试允许的图层*/
        public LayerMask maskOfHeightCheck = -1;

        /** 射线发射的高度*/
        public float rayOfLaunchHeight = 100;

        public void Initialize(Matrix4x4 matrix)
        {
            if (matrix != null)
            {
                graphUp = matrix.MultiplyVector(Vector3.up);
                upHeight = graphUp * height;
            }
        }

        public bool Check(Vector3 position)
        {
            if (!collisionCheck) return true;
          //  Debug.Log("maskOfCollisionCheck::" + maskOfCollisionCheck.value);
            position += graphUp * upwardOffsetOfNode;
            return !Physics.Raycast(position, graphUp, height, maskOfCollisionCheck)
                && !Physics.Raycast(position + upHeight, -graphUp, height, maskOfCollisionCheck);
        }

        public Vector3 CheckHeight(Vector3 position, out RaycastHit hit, out bool walkAble)
        {
            walkAble = true;

            if (!heightCheck)
            {
                hit = new RaycastHit();
                return position;
            }

            if (Physics.Raycast(position + graphUp * rayOfLaunchHeight, -graphUp, out hit, rayOfLaunchHeight + 0.005F, maskOfHeightCheck))
            {
                return hit.point;
            }

            walkAble = false;
            return position;
        }
    }

}
 