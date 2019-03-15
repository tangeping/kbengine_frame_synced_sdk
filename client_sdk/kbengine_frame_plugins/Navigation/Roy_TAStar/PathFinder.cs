using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace KBEngine
{
    /// <summary>
    /// Computes a path in a grid according to the A* algorithm
    /// </summary>
    internal static partial class PathFinder
    {
        public static List<Vector2Int> FindPath(Grid grid, Vector2Int start, Vector2Int end, Offset[] movementPattern)
        {            

            if (start == end)
            {
                return new List<Vector2Int> {start};
            }
           
            var head = new MinHeapNode(start, ManhattanDistance(start, end));
            var open = new MinHeap();            
            open.Push(head);

            var costSoFar = new FP[grid.DimX * grid.DimY];
            var cameFrom = new Vector2Int[grid.DimX * grid.DimY];                       

            while (open.HasNext())
            {
                // Get the best candidate
                var current = open.Pop().NodeIndex;

                if (current == end)
                {
                    return ReconstructPath(grid, start, end, cameFrom);
                }

                Step(grid, open, cameFrom, costSoFar, movementPattern, current, end);

            }

            return null;
        }

        public static List<Vector2Int> FindPath(Grid grid, Vector2Int start, Vector2Int end, Offset[] movementPattern, int iterationLimit)
        {
            if (start == end)
            {
                return new List<Vector2Int> { start };
            }

            var head = new MinHeapNode(start, ManhattanDistance(start, end));
            var open = new MinHeap();
            open.Push(head);

            var costSoFar = new FP[grid.DimX * grid.DimY];
            var cameFrom = new Vector2Int[grid.DimX * grid.DimY];
            
            while (open.HasNext() && iterationLimit > 0)
            {
                // Get the best candidate
                var current = open.Pop().NodeIndex;
 
                if (current == end)
                {
                    return ReconstructPath(grid, start, end, cameFrom);
                }

                Step(grid, open, cameFrom, costSoFar, movementPattern, current, end);

                --iterationLimit;
            }

            return null;
        }

        private static void Step(
            Grid grid,
            MinHeap open,
            Vector2Int[] cameFrom,
            FP[] costSoFar,
            Offset[] movementPattern,
            Vector2Int current,
            Vector2Int end)
        {
            // Get the cost associated with getting to the current Vector2Int
            var initialCost = costSoFar[grid.GetIndexUnchecked(current.x,current.y)];

            // Get all directions we can move to according to the movement pattern and the dimensions of the grid
            foreach (var option in GetMovementOptions(current, grid.DimX, grid.DimY, movementPattern))
            {
                var position = current + option;
                var cellCost = grid.GetCellCostUnchecked(position.x,position.y);

                // Ignore this option if the cell is blocked
                if (FP.IsInfinity(cellCost))
                    continue;

                var index = grid.GetIndexUnchecked(position.x,position.y);

                // Compute how much it would cost to get to the new Vector2Int via this path
                var newCost = initialCost + cellCost * option.Cost;

                // Compare it with the best cost we have so far, 0 means we don't have any path that gets here yet
                var oldCost = costSoFar[index];
                if (!(oldCost <= 0) && !(newCost < oldCost))
                    continue;

                // Update the best path and the cost if this path is cheaper
                costSoFar[index] = newCost;
                cameFrom[index] = current;

                // Use the heuristic to compute how much it will probably cost 
                // to get from here to the end, and store the node in the open list
                var expectedCost = newCost + ManhattanDistance(position, end);
                open.Push(new MinHeapNode(position, expectedCost));
            }
        }

        private static List<Vector2Int> ReconstructPath(Grid grid, Vector2Int start, Vector2Int end, Vector2Int[] cameFrom)
        {
            var path = new List<Vector2Int> { end };
            var current = end;
            do
            {
                var previous = cameFrom[grid.GetIndexUnchecked(current.x,current.y)];               
                current = previous;
                path.Add(current);
            } while (current != start);

            return path;
        }        

        private static IEnumerable<Offset> GetMovementOptions(
            Vector2Int nodeIndex,
            int dimX,
            int dimY,
            IEnumerable<Offset> movementPattern)
        {
            return movementPattern.Where(
                m =>
                {
                    var target = nodeIndex + m;
                    return target.x >= 0 && target.x < dimX && target.y >= 0 && target.y < dimY;
                });            
        }        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static FP ManhattanDistance(Vector2Int p0, Vector2Int p1)
        {
            return FPMath.Abs(p0.x - p1.x) + FPMath.Abs(p0.y - p1.y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="startPos"></param>
        /// <param name="goalPos"></param>
        /// <param name="pathfinder"></param>
        /// <returns></returns>
        public static List<Vector2Int> Pathfind(Grid grid, Vector2Int startPos, Vector2Int goalPos,AStarPathfinder pathfinder = null)
        {
            // Reset the allocated MapSearchNode pointer
            if(pathfinder == null)
            {
                pathfinder = new AStarPathfinder(grid);
            }

            pathfinder.InitiatePathfind();

            // Create a start state
            MapSearchNode nodeStart = pathfinder.AllocateMapSearchNode(startPos);

            // Define the goal state
            MapSearchNode nodeEnd = pathfinder.AllocateMapSearchNode(goalPos);

            // Set Start and goal states
            pathfinder.SetStartAndGoalStates(nodeStart, nodeEnd);

            // Set state to Searching and perform the search
            AStarPathfinder.SearchState searchState = AStarPathfinder.SearchState.Searching;
            uint searchSteps = 0;

            do
            {
                searchState = pathfinder.SearchStep();
                searchSteps++;
            }
            while (searchState == AStarPathfinder.SearchState.Searching);

            // Search complete
            bool pathfindSucceeded = (searchState == AStarPathfinder.SearchState.Succeeded);

            if (pathfindSucceeded)
            {
                // Success
                List<Vector2Int> newPath = new List<Vector2Int>();
                int numSolutionNodes = 0;   // Don't count the starting cell in the path length

                // Get the start node
                MapSearchNode node = pathfinder.GetSolutionStart();
                newPath.Add(node.nodeIndex);
                ++numSolutionNodes;

                // Get all remaining solution nodes
                for (; ; )
                {
                    node = pathfinder.GetSolutionNext();

                    if (node == null)
                    {
                        break;
                    }

                    ++numSolutionNodes;
                    newPath.Add(node.nodeIndex);
                };

                // Once you're done with the solution we can free the nodes up
                pathfinder.FreeSolutionNodes();

                return newPath;
                //System.Console.WriteLine("Solution path length: " + numSolutionNodes);
                //System.Console.WriteLine("Solution: " + newPath.ToString());
            }
            else if (searchState == AStarPathfinder.SearchState.Failed)
            {
                // FAILED, no path to goal
                //System.Console.WriteLine("Pathfind FAILED!");
            }

            return null;
        }
    }
}
