
using System;

namespace KBEngine
{
    public struct FPVector4 : IEquatable<FPVector4>
    {
        // *undocumented*
        private static FP kEpsilon = FPMath.Epsilon;

        // X component of the vector.
        public FP x;
        // Y component of the vector.
        public FP y;
        // Z component of the vector.
        public FP z;
        // W component of the vector.
        public FP w;


        // Access the x, y, z, w components using [0], [1], [2], [3] respectively.
        public FP this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    case 2: return z;
                    case 3: return w;
                    default:
                        throw new IndexOutOfRangeException("Invalid FPVector4 index!");
                }
            }

            set
            {
                switch (index)
                {
                    case 0: x = value; break;
                    case 1: y = value; break;
                    case 2: z = value; break;
                    case 3: w = value; break;
                    default:
                        throw new IndexOutOfRangeException("Invalid FPVector4 index!");
                }
            }
        }


        // Creates a new vector with given x, y, z, w components.
        public FPVector4(FP x, FP y, FP z, FP w) { this.x = x; this.y = y; this.z = z; this.w = w; }
        // Creates a new vector with given x, y, z components and sets /w/ to zero.
        public FPVector4(FP x, FP y, FP z) { this.x = x; this.y = y; this.z = z; this.w = 0F; }
        // Creates a new vector with given x, y components and sets /z/ and /w/ to zero.
        public FPVector4(FP x, FP y) { this.x = x; this.y = y; this.z = 0F; this.w = 0F; }

        // Set x, y, z and w components of an existing FPVector4.
        public void Set(FP newX, FP newY, FP newZ, FP newW) { x = newX; y = newY; z = newZ; w = newW; }

        // Linearly interpolates between two vectors.
        public static FPVector4 Lerp(FPVector4 a, FPVector4 b, FP t)
        {
            t = FPMath.Clamp01(t);
            return new FPVector4(
                a.x + (b.x - a.x) * t,
                a.y + (b.y - a.y) * t,
                a.z + (b.z - a.z) * t,
                a.w + (b.w - a.w) * t
            );
        }

        // Linearly interpolates between two vectors without clamping the interpolant
        public static FPVector4 LerpUnclamped(FPVector4 a, FPVector4 b, FP t)
        {
            return new FPVector4(
                a.x + (b.x - a.x) * t,
                a.y + (b.y - a.y) * t,
                a.z + (b.z - a.z) * t,
                a.w + (b.w - a.w) * t
            );
        }

        // Moves a point /current/ towards /target/.
        public static FPVector4 MoveTowards(FPVector4 current, FPVector4 target, FP maxDistanceDelta)
        {
            FPVector4 toVector = target - current;
            FP dist = toVector.magnitude;
            if (dist <= maxDistanceDelta || dist == 0)
                return target;
            return current + toVector / dist * maxDistanceDelta;
        }

        // Multiplies two vectors component-wise.
        public static FPVector4 Scale(FPVector4 a, FPVector4 b)
        {
            return new FPVector4(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
        }

        // Multiplies every component of this vector by the same component of /scale/.
        public void Scale(FPVector4 scale)
        {
            x *= scale.x;
            y *= scale.y;
            z *= scale.z;
            w *= scale.w;
        }
        // used to allow FPVector4s to be used as keys in hash tables
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2) ^ (w.GetHashCode() >> 1);
        }

        // also required for being able to use FPVector4s as keys in hash tables
        public override bool Equals(object other)
        {
            if (!(other is FPVector4)) return false;

            return Equals((FPVector4)other);
        }

        public bool Equals(FPVector4 other)
        {
            return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z) && w.Equals(other.w);
        }

        // *undoc* --- we have normalized property now
        public static FPVector4 Normalize(FPVector4 a)
        {
            FP mag = Magnitude(a);
            if (mag > kEpsilon)
                return a / mag;
            else
                return zero;
        }

        // Makes this vector have a ::ref::magnitude of 1.
        public void Normalize()
        {
            FP mag = Magnitude(this);
            if (mag > kEpsilon)
                this = this / mag;
            else
                this = zero;
        }

        // Returns this vector with a ::ref::magnitude of 1 (RO).
        public FPVector4 normalized
        {
            get
            {
                return FPVector4.Normalize(this);
            }
        }

        // Dot Product of two vectors.
        public static FP Dot(FPVector4 a, FPVector4 b) { return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w; }

        // Projects a vector onto another vector.
        public static FPVector4 Project(FPVector4 a, FPVector4 b) { return b * Dot(a, b) / Dot(b, b); }

        // Returns the distance between /a/ and /b/.
        public static FP Distance(FPVector4 a, FPVector4 b) { return Magnitude(a - b); }

        // *undoc* --- there's a property now
        public static FP Magnitude(FPVector4 a) { return FPMath.Sqrt(Dot(a, a)); }

        // Returns the length of this vector (RO).
        public FP magnitude { get { return FPMath.Sqrt(Dot(this, this)); } }

        // Returns the squared length of this vector (RO).
        public FP sqrMagnitude { get { return Dot(this, this); } }

        // Returns a vector that is made from the smallest components of two vectors.
        public static FPVector4 Min(FPVector4 lhs, FPVector4 rhs)
        {
            return new FPVector4(FPMath.Min(lhs.x, rhs.x), FPMath.Min(lhs.y, rhs.y), FPMath.Min(lhs.z, rhs.z), FPMath.Min(lhs.w, rhs.w));
        }

        // Returns a vector that is made from the largest components of two vectors.
        public static FPVector4 Max(FPVector4 lhs, FPVector4 rhs)
        {
            return new FPVector4(FPMath.Max(lhs.x, rhs.x), FPMath.Max(lhs.y, rhs.y), FPMath.Max(lhs.z, rhs.z), FPMath.Max(lhs.w, rhs.w));
        }

        static readonly FPVector4 zeroVector = new FPVector4(0F, 0F, 0F, 0F);
        static readonly FPVector4 oneVector = new FPVector4(1F, 1F, 1F, 1F);
        static readonly FPVector4 positiveInfinityVector = new FPVector4(FP.PositiveInfinity, FP.PositiveInfinity, FP.PositiveInfinity, FP.PositiveInfinity);
        static readonly FPVector4 negativeInfinityVector = new FPVector4(FP.NegativeInfinity, FP.NegativeInfinity, FP.NegativeInfinity, FP.NegativeInfinity);

        // Shorthand for writing @@FPVector4(0,0,0,0)@@
        public static FPVector4 zero { get { return zeroVector; } }
        // Shorthand for writing @@FPVector4(1,1,1,1)@@
        public static FPVector4 one { get { return oneVector; } }
        // Shorthand for writing @@Vector3(FP.PositiveInfinity, FP.PositiveInfinity, FP.PositiveInfinity)@@
        public static FPVector4 positiveInfinity { get { return positiveInfinityVector; } }
        // Shorthand for writing @@Vector3(FP.NegativeInfinity, FP.NegativeInfinity, FP.NegativeInfinity)@@
        public static FPVector4 negativeInfinity { get { return negativeInfinityVector; } }

        // Adds two vectors.
        public static FPVector4 operator +(FPVector4 a, FPVector4 b) { return new FPVector4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w); }
        // Subtracts one vector from another.
        public static FPVector4 operator -(FPVector4 a, FPVector4 b) { return new FPVector4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w); }
        // Negates a vector.
        public static FPVector4 operator -(FPVector4 a) { return new FPVector4(-a.x, -a.y, -a.z, -a.w); }
        // Multiplies a vector by a number.
        public static FPVector4 operator *(FPVector4 a, FP d) { return new FPVector4(a.x * d, a.y * d, a.z * d, a.w * d); }
        // Multiplies a vector by a number.
        public static FPVector4 operator *(FP d, FPVector4 a) { return new FPVector4(a.x * d, a.y * d, a.z * d, a.w * d); }
        // Divides a vector by a number.
        public static FPVector4 operator /(FPVector4 a, FP d) { return new FPVector4(a.x / d, a.y / d, a.z / d, a.w / d); }

        // Returns true if the vectors are equal.
        public static bool operator ==(FPVector4 lhs, FPVector4 rhs)
        {
            // Returns false in the presence of NaN values.
            return SqrMagnitude(lhs - rhs) < kEpsilon * kEpsilon;
        }

        // Returns true if vectors are different.
        public static bool operator !=(FPVector4 lhs, FPVector4 rhs)
        {
            // Returns true in the presence of NaN values.
            return !(lhs == rhs);
        }

        // Converts a [[Vector3]] to a FPVector4.
        public static implicit operator FPVector4(FPVector v)
        {
            return new FPVector4(v.x, v.y, v.z, 0.0F);
        }

        // Converts a FPVector4 to a [[Vector3]].
        public static implicit operator FPVector(FPVector4 v)
        {
            return new FPVector(v.x, v.y, v.z);
        }

        // Converts a [[Vector2]] to a FPVector4.
        public static implicit operator FPVector4(FPVector2 v)
        {
            return new FPVector4(v.x, v.y, 0.0F, 0.0F);
        }

        // Converts a FPVector4 to a [[Vector2]].
        public static implicit operator FPVector2(FPVector4 v)
        {
            return new FPVector2(v.x, v.y);
        }

        public override string ToString()
        {
            return string.Format("({0:F1}, {1:F1}, {2:F1}, {3:F1})", x, y, z, w);
        }

        public string ToString(string format)
        {
            return string.Format("({0}, {1}, {2}, {3})", x.AsFloat(), y.AsFloat(), z.AsFloat(), w.AsFloat());
        }

        // *undoc* --- there's a property now
        public static FP SqrMagnitude(FPVector4 a) { return FPVector4.Dot(a, a); }
        // *undoc* --- there's a property now
        public FP SqrMagnitude() { return Dot(this, this); }


    }
}



