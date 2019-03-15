using System;
using UnityEngine;

namespace KBEngine
{
    /// <summary>
    /// A 2D offset structure. you can use an array of offsets to represent the movement pattern
    /// of your agent, for example an offset of (-1, 0) means your character is able
    /// to move a single cell to the left <see cref="MovementPatterns"/> for some predefined
    /// options.
    /// </summary>
    public struct Offset : IEquatable<Offset>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="x">x-movement offset</param>
        /// <param name="y">y-movement offset</param>
        public Offset(int x, int y)
        {
            if (x < -1 || x > 1)
                throw new ArgumentOutOfRangeException(nameof(x), $"Parameter {nameof(x)} cannot have a magnitude larger than one");

            if (y < -1 || y > 1)
                throw new ArgumentOutOfRangeException(nameof(y), $"Parameter {nameof(y)} cannot have a magnitude larger than one");

            if (x == 0 && y == 0)
                throw new ArgumentException(nameof(y), $"Paramters {nameof(x)} and {nameof(y)} cannot both be zero");

            this.x = x;
            this.y = y;

            // Penalize diagonal movement
            this.Cost = (x != 0 && y != 0) ? 1 : 1.4142135623730950488016887242097f; // sqrt(2)                                  
        }

        /// <summary>
        /// x-position
        /// </summary>
        public int x { get; }

        /// <summary>
        /// y-position
        /// </summary>
        public int y { get; }

        /// <summary>
        /// Relative cost of adding this offset to a position, either 1 for lateral movement, or sqrt(2) for diagonal movement
        /// </summary>
        public FP Cost { get; }

        public override string ToString() => $"Offset: ({this.x}, {this.y})";
        
        public bool Equals(Offset other)
        {
            return this.x == other.x && this.y == other.y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            return obj is Offset && Equals((Offset)obj);
        }

        public static bool operator ==(Offset a, Offset b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Offset a, Offset b)
        {
            return !a.Equals(b);
        }      

        public static Vector2Int operator +(Offset a, Vector2Int b)
        {
            return new Vector2Int(a.x + b.x, a.y + b.y);
        }

        public static Vector2Int operator -(Offset a, Vector2Int b)
        {
            return new Vector2Int(a.x - b.x, a.y - b.y);
        }

        public static Vector2Int operator +(Vector2Int a, Offset b)
        {
            return new Vector2Int(a.x + b.x, a.y + b.y);
        }

        public static Vector2Int operator -(Vector2Int a, Offset b)
        {
            return new Vector2Int(a.x - b.x, a.y - b.y);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (this.x * 397) ^ this.y;
            }
        }
    }
}
