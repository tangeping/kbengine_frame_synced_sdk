using UnityEngine;

namespace KBEngine
{
    /// <summary>
    /// Node in a heap
    /// </summary>
    internal sealed class MinHeapNode 
    {        
        public MinHeapNode(Vector2Int nodeIndex, FP expectedCost)
        {
            this.NodeIndex = nodeIndex;
            this.ExpectedCost = expectedCost;            
        }

        public Vector2Int NodeIndex { get; }
        public FP ExpectedCost { get; set; }                
        public MinHeapNode Next { get; set; }
    }
}
