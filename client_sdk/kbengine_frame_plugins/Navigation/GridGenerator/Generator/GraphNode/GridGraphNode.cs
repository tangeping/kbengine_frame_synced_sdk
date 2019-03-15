using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KBEngine;
public class GridGraphNode : GraphNodeBase
{
    /** node 当前位置*/
    public Int3 position;
  
    private static GridNavGraph[] _gridNavGraphs = new GridNavGraph[0];
    public static void SetGridNavGraph(int graphIndex, GridNavGraph graph)
    {
        if (_gridNavGraphs.Length <= graphIndex)
        {
            var graphObj = new GridNavGraph[graphIndex + 1];
            for (int i = 0; i < _gridNavGraphs.Length; i++) graphObj[i] = _gridNavGraphs[i];
            _gridNavGraphs = graphObj;
        }

        _gridNavGraphs[graphIndex] = graph;
    }


    protected ushort gridFlags;

    const int GridFlagsConnectionOffset = 0;
    const int GridFlagsConnectionBit0 = 1 << GridFlagsConnectionOffset;
    const int GridFlagsConnectionMask = 0xFF << GridFlagsConnectionOffset;

    const int GridFlagsWalkableErosionOffset = 8;
    const int GridFlagsWalkableErosionMask = 1 << GridFlagsWalkableErosionOffset;

    const int GridFlagsWalkableTmpOffset = 9;
    const int GridFlagsWalkableTmpMask = 1 << GridFlagsWalkableTmpOffset;

    const int GridFlagsEdgeNodeOffset = 10;
    const int GridFlagsEdgeNodeMask = 1 << GridFlagsEdgeNodeOffset;

   

    public bool GetConnectionInternal(int dir)
    {
        return (gridFlags >> dir & GridFlagsConnectionBit0) != 0;
    }

    public void SetConnectionInternal(int dir, bool value)
    {
        unchecked { gridFlags = (ushort)(gridFlags & ~((ushort)1 << GridFlagsConnectionOffset << dir) | (value ? (ushort)1 : (ushort)0) << GridFlagsConnectionOffset << dir); }
    }

    public void ResetConnectionsInternal()
    {
        unchecked
        {
            gridFlags = (ushort)(gridFlags & ~GridFlagsConnectionMask);
        }
    }

    public bool EdgeNode
    {
        get
        {
            return (gridFlags & GridFlagsEdgeNodeMask) != 0;
        }
        set
        {
            unchecked { gridFlags = (ushort)(gridFlags & ~GridFlagsEdgeNodeMask | (value ? GridFlagsEdgeNodeMask : 0)); }
        }
    }

    public bool WalkableErosion
    {
        get
        {
            return (gridFlags & GridFlagsWalkableErosionMask) != 0;
        }
        set
        {
            unchecked { gridFlags = (ushort)(gridFlags & ~GridFlagsWalkableErosionMask | (value ? (ushort)GridFlagsWalkableErosionMask : (ushort)0)); }
        }
    }

    public bool TmpWalkable
    {
        get
        {
            return (gridFlags & GridFlagsWalkableTmpMask) != 0;
        }
        set
        {
            unchecked { gridFlags = (ushort)(gridFlags & ~GridFlagsWalkableTmpMask | (value ? (ushort)GridFlagsWalkableTmpMask : (ushort)0)); }
        }
    }

    public override void Destory()
    {
        base.Destory();
        gridFlags = 0;
    }

}
