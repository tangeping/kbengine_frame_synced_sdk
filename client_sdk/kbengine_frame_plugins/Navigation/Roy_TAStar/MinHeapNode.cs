using UnityEngine;

namespace KBEngine
{
    /// <summary>
    /// Node in a heap
    /// </summary>
    internal sealed class MinHeapNode
    {        
        public MinHeapNode(Vector2Int position, FP expectedCost)
        {
            this.Position     = position;
            this.ExpectedCost = expectedCost;            
        }

        public Vector2Int Position { get; }
        public FP ExpectedCost { get; set; }                
        public MinHeapNode Next { get; set; }
    }
}
