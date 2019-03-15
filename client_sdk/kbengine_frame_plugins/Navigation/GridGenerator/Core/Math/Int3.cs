using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KBEngine
{
    public struct Int3
    {
        public int x;
        public int y;
        public int z;

        /** 精度:1000的值表示毫米精度，1的值表示米精度(假设1个世界单位= 1米)*/
        /** 此值影响节点的最大坐标以及在两个节点之间移动的代价值*/
        public const int Precision = 1000;
        public const float FloatPrecision = 1000F;
        public const float PrecisionFactor = 0.001F;
        public Int3(Vector3 position)
        {
            x = (int)System.Math.Round(position.x * FloatPrecision);
            y = (int)System.Math.Round(position.y * FloatPrecision);
            z = (int)System.Math.Round(position.z * FloatPrecision);
        }

        public Int3(int _x, int _y, int _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }

        public static explicit operator Int3(Vector3 ob)
        {
            return new Int3(
                (int)System.Math.Round(ob.x * FloatPrecision),
                (int)System.Math.Round(ob.y * FloatPrecision),
                (int)System.Math.Round(ob.z * FloatPrecision)
                );
        }

        public static explicit operator Vector3(Int3 ob)
        {
            return new Vector3(ob.x * PrecisionFactor, ob.y * PrecisionFactor, ob.z * PrecisionFactor);
        }

        public int this[int i]
        {
            get
            {
                return i == 0 ? x : (i == 1 ? y : z);
            }
            set
            {
                if (i == 0) x = value;
                else if (i == 1) y = value;
                else z = value;
            }
        }

        public override string ToString()
        {
            return "( " + x + ", " + y + ", " + z + ")";
        }
    }

    public struct Long3
    {
        public long x;
        public long y;
        public long z;

        public Long3(long x, long y, long z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public FPVector ToFPVector()
        {
            return new FPVector(FP.FromRaw(x), FP.FromRaw(y), FP.FromRaw(z));
        }
    }
}

