﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace KBEngine
{
    /// <summary>
    /// Representation of your world for the pathfinding algorithm.
    /// Use SetCellCost to change the cost of traversing a cell.
    /// Use BlockCell to make a cell completely intraversable.
    /// </summary>
    public sealed class Grid
    {       
        private readonly byte DefaultCost;
        private readonly byte[] Weights;

        private readonly FPVector Orgin;
        private readonly byte BlockCost = 9;

		private readonly Vector2Int Shape; // the Shape of Terria
        /// <summary>
        /// Creates a grid
        /// </summary>
        /// <param name="dimX">The x-dimension of your world</param>
        /// <param name="dimY">The y-dimesion of your world</param>
        /// <param name="defaultCost">The default cost every cell is initialized with</param>
        public Grid(int dimX, int dimY, byte defaultCost = 1)
        {
            if (defaultCost < 1)
            {
                throw new ArgumentOutOfRangeException(
                    $"Argument {nameof(defaultCost)} with value {defaultCost} is invalid. The cost of traversing a cell cannot be less than one");
            }

            this.DefaultCost = defaultCost;
            this.Weights = new byte[dimX * dimY];
            this.DimX = dimX;
            this.DimY = dimY;
			this.Orgin = FPVector.zero;
			this.Shape = Vector2Int.one;
            for (var n = 0; n < this.Weights.Length; n++)
            {
                this.Weights[n] = defaultCost;
            }
        }
        public Grid(int dimX, int dimY, FPVector Orgin, Vector2Int Shape)
        {
            this.DefaultCost = 1;
            this.Weights = new byte[dimX * dimY];
            this.Orgin = Orgin;
            this.DimX = dimX;
            this.DimY = dimY;
            this.Shape = Shape;

            for (var n = 0; n < this.Weights.Length; n++)
            {
                this.Weights[n] = DefaultCost;
            }
        }

        /// <summary>
        /// X-dimension of the grid
        /// </summary>
        public int DimX { get; }
        
        /// <summary>
        /// Y-dimension of the grid
        /// </summary>
        public int DimY { get; }

        public Vector2Int GetShape() { return this.Shape; }
        public FPVector GetOrgin() { return this.Orgin; }
        /// <summary>
        /// get the width of cell
        /// </summary>
        public FP GetWidth() { return Shape.x / DimX; }

        /// <summary>
        /// get the Height of cell
        /// </summary>
        public FP GetHeight() { return Shape.y / DimY; }

        /// <summary>
        /// return the point whether in Terrain
        /// </summary>
        public bool isInArea(FPVector p)
        {
            FPVector diff = p - this.Orgin;
            return diff.x >= 0 && diff.x <= this.Shape.x && diff.z >= 0 && diff.z <= this.Shape.y;
        }

        public Vector2Int getIndex(FPVector p)
        {
            Vector2Int nodeIndex = Vector2Int.zero;
            if (!isInArea(p))
            {
                return nodeIndex;
            }
            FPVector relatiPos = p - this.Orgin;
            nodeIndex.x = FPMath.Floor(relatiPos.x / this.GetWidth()).AsInt();
            nodeIndex.y = FPMath.Floor(relatiPos.z / this.GetHeight()).AsInt();
            return nodeIndex;
        }

        public FPVector GetCenterPoint(int x, int y)
        {
            if (!(0 <= x && x <= this.DimX && 0 <= y && y <= this.DimY))
            {
                throw new ArgumentNullException("0 <= x && x <= this.DimX && 0 <= y && y <= this.DimY");
            }
            FP width = (x + (FP)0.5f) * this.GetWidth();
            FP height = (y + (FP)0.5f) * this.GetHeight();
            return new FPVector(width,0, height) + this.Orgin;
        }

        /// <summary>
        /// Sets the cost for traversing a cell
        /// </summary>
        /// <param name="position">A position inside the grid</param>
        /// <param name="cost">The cost of traversing the cell, cannot be less than one</param>
        public void SetCellCost(Vector2Int position, byte cost)
        {
            if (cost < 1)
            {
                throw new ArgumentOutOfRangeException(
                    $"Argument {nameof(cost)} with value {cost} is invalid. The cost of traversing a cell cannot be less than one");
            }

            this.Weights[GetIndex(position.x, position.y)] = cost;
        }

        /// <summary>
        /// Makes the cell intraversable
        /// </summary>
        /// <param name="position">A position inside the grid</param>
        public void BlockCell(Vector2Int position) => SetCellCost(position, BlockCost);

        /// <summary>
        /// Makes the cell traversable, gives it the default traversal cost as provided in the constructor
        /// </summary>
        /// <param name="position">A position inside the grid</param>
        public void UnblockCell(Vector2Int position) => SetCellCost(position, this.DefaultCost);

        public bool isBlockCell(Vector2Int nodeIndex) { return GetCellCost(nodeIndex) >= BlockCost; }
        /// <summary>
        /// Looks-up the cost for traversing a given cell, if a cell is blocked (<see cref="BlockCell"/>) 
        /// +infinity is returned
        /// </summary>
        /// <param name="position">A position inside the grid</param>
        /// <returns>The cost</returns>
        public byte GetCellCost(Vector2Int position)
        {
            return this.Weights[GetIndex(position.x, position.y)];
        }

        /// <summary>
        /// Looks-up the cost for traversing a given cell, does not check
        /// if the position is inside the grid
        /// </summary>
        /// <param name="position">A position inside the grid</param>
        /// <returns>The cost</returns>
        internal byte GetCellCostUnchecked(Vector2Int position)
        {
            return this.Weights[GetIndexUnchecked(position.x, position.y)];
        }

        /// <summary>
        /// Computes the lowest-cost path from start to end inside the grid for an agent that can
        /// move both diagonal and lateral
        /// </summary>
        /// <param name="start">The start position</param>
        /// <param name="end">The end position</param>        
        /// <returns>Vector2Ints along the shortest path from start to end, or an empty array if no path could be found</returns>
        public Vector2Int[] GetPath(Vector2Int start, Vector2Int end)
            => GetPath(start, end, MovementPatterns.Full);

        public Vector2Int[] GetPath(Vector2Int start, Vector2Int end, AStarPathfinder pathfinder)
            => PathFinder.Pathfind(this, start, end, pathfinder).ToArray();
        /// <summary>
        /// Computes the lowest-cost path from start to end inside the grid for an agent with a custom
        /// movement pattern
        /// </summary>
        /// <param name="start">The start position</param>
        /// <param name="end">The end position</param>
        /// <param name="movementPattern">The movement pattern of the agent, <see cref="MovementPatterns"/> for several built-in options</param>
        /// <returns>Vector2Ints along the shortest path from start to end, or an empty array if no path could be found</returns>
        public Vector2Int[] GetPath(Vector2Int start, Vector2Int end, Offset[] movementPattern)
        {
            var current = PathFinder.FindPath(this, start, end, movementPattern);

            if (current == null)
            {
                return new Vector2Int[0];
            }

            // The Pathfinder returns the positions that found the end. If we want
            // to list positions from start to end we need reverse the traversal.
            var steps = new Stack<Vector2Int>();
            
            foreach (var step in current)
            {
                steps.Push(step);
            }            

            return steps.ToArray();                        
        }

        /// <summary>
        /// Computes the lowest-cost path from start to end inside the grid for an agent with a custom
        /// movement pattern. Instructs the path finder to give up if the path is not found after a number of iterations.
        /// </summary>
        /// <param name="start">The start position</param>
        /// <param name="end">The end position</param>
        /// <param name="movementPattern">The movement pattern of the agent, <see cref="MovementPatterns"/> for several built-in options</param>
        /// <param name="iterationLimit">Maximum number of nodes to check before the path finder gives up</param>
        /// <returns>Vector2Ints along the shortest path from start to end, or an empty array if no path could be found</returns>
        public Vector2Int[] GetPath(Vector2Int start, Vector2Int end, Offset[] movementPattern, int iterationLimit)
        {
            var current = PathFinder.FindPath(this, start, end, movementPattern, iterationLimit);

            if (current == null)
            {
                return new Vector2Int[0];
            }

            // The Pathfinder returns the positions that found the end. If we want
            // to list positions from start to end we need reverse the traversal.
            var steps = new Stack<Vector2Int>();

            foreach (var step in current)
            {
                steps.Push(step);
            }

            return steps.ToArray();
        }

        /// <summary>
        /// Converts a 2d index to a 1d index and performs bounds checking
        /// </summary>        
        private int GetIndex(int x, int y)
        {
            if (x < 0 || x >= this.DimX)
            {
                throw new ArgumentOutOfRangeException(
                    $"The x-coordinate {x} is outside of the expected range [0...{this.DimX})");
            }

            if (y < 0 || y >= this.DimY)
            {
                throw new ArgumentOutOfRangeException(
                    $"The y-coordinate {y} is outside of the expected range [0...{this.DimY})");
            }

            return GetIndexUnchecked(x, y);
        }     
        
        public bool CheckIndexValid(int x, int y)
        {
            return (x >= 0 && x < this.DimX) && (y >= 0 && y < this.DimY); 
        }
        /// <summary>
        /// Converts a 2d index to a 1d index without any bounds checking
        /// </summary>        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int GetIndexUnchecked(int x, int y) => this.DimX * y + x;
    }    
}

