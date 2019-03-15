using UnityEngine;

namespace KBEngine
{
    public class MapSearchNode
    {
        public Vector2Int nodeIndex;
        AStarPathfinder pathfinder = null;

        public MapSearchNode(AStarPathfinder _pathfinder)
        {
            nodeIndex = Vector2Int.zero;
            pathfinder = _pathfinder;
        }

        public MapSearchNode(Vector2Int pos, AStarPathfinder _pathfinder)
        {
            nodeIndex = new Vector2Int(pos.x, pos.y);
            pathfinder = _pathfinder;
        }

        // Here's the heuristic function that estimates the distance from a Node
        // to the Goal. 
        public FP GoalDistanceEstimate(MapSearchNode nodeGoal)
        {
            FP X = nodeIndex.x - nodeGoal.nodeIndex.x;
            FP Y = nodeIndex.y - nodeGoal.nodeIndex.y;
            return (FPMath.Sqrt((X * X) + (Y * Y)));
        }

        public bool IsGoal(MapSearchNode nodeGoal)
        {
            return (nodeIndex.x == nodeGoal.nodeIndex.x && nodeIndex.y == nodeGoal.nodeIndex.y);
        }

        public FP GetMap(int x, int y)
        {
            return pathfinder.gridMap.GetCellCost(new Vector2Int(x, y));
        }

        public bool ValidNeigbour(int xOffset, int yOffset)
        {
            // Return true if the node is navigable and within grid bounds
            return pathfinder.gridMap.CheckIndexValid(nodeIndex.x + xOffset, nodeIndex.y + yOffset);
        }

        void AddNeighbourNode(int xOffset, int yOffset, Vector2Int parentPos, AStarPathfinder aStarSearch)
        {
            if (ValidNeigbour(xOffset, yOffset) &&
                !(parentPos.x == nodeIndex.x + xOffset && parentPos.y == nodeIndex.y + yOffset))
            {
                Vector2Int neighbourPos = new Vector2Int(nodeIndex.x + xOffset, nodeIndex.y + yOffset);
                //Debug.Log("neighbourPos:" + neighbourPos);
                MapSearchNode newNode = pathfinder.AllocateMapSearchNode(neighbourPos);
                aStarSearch.AddSuccessor(newNode);
            }
        }

        void AddNeighbourNodeOfAttachVolume(int xOffset, int yOffset, Vector2Int parentPos, AStarPathfinder aStarSearch)
        {
            int src_xOffset = xOffset;
            int src_yOffset = yOffset;

            bool flag = true;
            int volume = aStarSearch.volume;
            if (src_xOffset != 0)
            {
                xOffset += ((xOffset >= 0) ? volume : -volume);
                for (int y = -volume; y <= volume; y++)
                {
                    flag = ValidNeigbour(xOffset, y + src_yOffset);
                    if (flag == false)
                    {
                        return;
                    }
                }
            }

            if (src_yOffset != 0)
            {
                yOffset += ((yOffset >= 0) ? volume : -volume);
                for (int x = -volume; x <= volume; x++)
                {
                    flag = ValidNeigbour(x + src_xOffset, yOffset);
                    if (flag == false)
                    {
                        return;
                    }

                }
            }

            if (flag && !(parentPos.x == nodeIndex.x + src_xOffset && parentPos.y == nodeIndex.y + src_yOffset))
            {
                Vector2Int neighbourPos = new Vector2Int(nodeIndex.x + src_xOffset, nodeIndex.y + src_yOffset);
                MapSearchNode newNode = pathfinder.AllocateMapSearchNode(neighbourPos);
                aStarSearch.AddSuccessor(newNode);
            }
        }

        // This generates the successors to the given Node. It uses a helper function called
        // AddSuccessor to give the successors to the AStar class. The A* specific initialisation
        // is done for each node internally, so here you just set the state information that
        // is specific to the application
        public bool GetSuccessors(AStarPathfinder aStarSearch, MapSearchNode parentNode)
        {
            Vector2Int parentPos = new Vector2Int(0, 0);

            if (parentNode != null)
            {
                parentPos = parentNode.nodeIndex;
            }

            if(pathfinder.volume > 0)
            {
                AddNeighbourNodeOfAttachVolume(1, 0, parentPos, aStarSearch);
                AddNeighbourNodeOfAttachVolume(-1, 0, parentPos, aStarSearch);
                AddNeighbourNodeOfAttachVolume(0, 1, parentPos, aStarSearch);
                AddNeighbourNodeOfAttachVolume(0, -1, parentPos, aStarSearch);
                AddNeighbourNodeOfAttachVolume(1, -1, parentPos, aStarSearch);
                AddNeighbourNodeOfAttachVolume(-1, 1, parentPos, aStarSearch);
                AddNeighbourNodeOfAttachVolume(1, 1, parentPos, aStarSearch);
                AddNeighbourNodeOfAttachVolume(-1, -1, parentPos, aStarSearch);
            }
            else
            {
                // push each possible move except allowing the search to go backwards
                AddNeighbourNode(-1, 0, parentPos, aStarSearch);
                AddNeighbourNode(0, -1, parentPos, aStarSearch);
                AddNeighbourNode(1, 0, parentPos, aStarSearch);
                AddNeighbourNode(0, 1, parentPos, aStarSearch);
                AddNeighbourNode(1, -1, parentPos, aStarSearch);
                AddNeighbourNode(-1, 1, parentPos, aStarSearch);
                AddNeighbourNode(1, 1, parentPos, aStarSearch);
                AddNeighbourNode(-1, -1, parentPos, aStarSearch);
            }
            return true;
        }

        // given this node, what does it cost to move to successor. In the case
        // of our map the answer is the map terrain value at this node since that is 
        // conceptually where we're moving
        public FP GetCost(MapSearchNode successor)
        {
            // Implementation specific
            return GetMap(successor.nodeIndex.x, successor.nodeIndex.y);
        }

        public bool IsSameState(MapSearchNode rhs)
        {
            return (nodeIndex.x == rhs.nodeIndex.x &&
                    nodeIndex.y == rhs.nodeIndex.y);
        }
    }
}
