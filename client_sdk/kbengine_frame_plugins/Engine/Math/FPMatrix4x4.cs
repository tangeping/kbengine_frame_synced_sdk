using System;
using UnityEngine;

namespace KBEngine
{
    /*
The prmath matrix convention:

; offsets

[ 0  1  2  3]
[ 4  5  6  7]
[ 8  9  0 11]
[12 13 14 15]

; scaling (sx,sy,sz)

[sx -- -- --]
[-- sy -- --]
[-- -- sz --]
[-- -- -- --]

; translation (tx,ty,tz)

[-- -- -- --]
[-- -- -- --]
[-- -- -- --]
[tx ty tz --]

; rotation

[xx xy xz --]  x-axis (xx,xy,xz)
[yx yy yz --]  y-axis (yx,yy,yz)
[zx zy zz --]  z-axis (zx,zy,zz)
[-- -- -- --]

; v = a * b

v[0][0] = a[0][0] * b[0][0] + a[0][1] * b[1][0] + a[0][2] * b[2][0] + a[0][3] * b[3][0];
v[0][1] = a[0][0] * b[0][1] + a[0][1] * b[1][1] + a[0][2] * b[2][1] + a[0][3] * b[3][1];
v[0][2] = a[0][0] * b[0][2] + a[0][1] * b[1][2] + a[0][2] * b[2][2] + a[0][3] * b[3][2];
v[0][3] = a[0][0] * b[0][3] + a[0][1] * b[1][3] + a[0][2] * b[2][3] + a[0][3] * b[3][3];
v[1][0] = a[1][0] * b[0][0] + a[1][1] * b[1][0] + a[1][2] * b[2][0] + a[1][3] * b[3][0];
v[1][1] = a[1][0] * b[0][1] + a[1][1] * b[1][1] + a[1][2] * b[2][1] + a[1][3] * b[3][1];
v[1][2] = a[1][0] * b[0][2] + a[1][1] * b[1][2] + a[1][2] * b[2][2] + a[1][3] * b[3][2];
v[1][3] = a[1][0] * b[0][3] + a[1][1] * b[1][3] + a[1][2] * b[2][3] + a[1][3] * b[3][3];
v[2][0] = a[2][0] * b[0][0] + a[2][1] * b[1][0] + a[2][2] * b[2][0] + a[2][3] * b[3][0];
v[2][1] = a[2][0] * b[0][1] + a[2][1] * b[1][1] + a[2][2] * b[2][1] + a[2][3] * b[3][1];
v[2][2] = a[2][0] * b[0][2] + a[2][1] * b[1][2] + a[2][2] * b[2][2] + a[2][3] * b[3][2];
v[2][3] = a[2][0] * b[0][3] + a[2][1] * b[1][3] + a[2][2] * b[2][3] + a[2][3] * b[3][3];
v[3][0] = a[3][0] * b[0][0] + a[3][1] * b[1][0] + a[3][2] * b[2][0] + a[3][3] * b[3][0];
v[3][1] = a[3][0] * b[0][1] + a[3][1] * b[1][1] + a[3][2] * b[2][1] + a[3][3] * b[3][1];
v[3][2] = a[3][0] * b[0][2] + a[3][1] * b[1][2] + a[3][2] * b[2][2] + a[3][3] * b[3][2];
v[3][3] = a[3][0] * b[0][3] + a[3][1] * b[1][3] + a[3][2] * b[2][3] + a[3][3] * b[3][3];

*/

    // members
    /// <summary>
    /// A standard 4x4 transformation matrix.
    /// </summary>
    public struct FPMatrix4x4 : IEquatable<FPMatrix4x4>
    {
        // memory layout:
        //
        //                row no (=vertical)
        //               |  0   1   2   3
        //            ---+----------------
        //            0  | m00 m10 m20 m30
        // column no  1  | m01 m11 m21 m31
        // (=horiz)   2  | m02 m12 m22 m32
        //            3  | m03 m13 m23 m33

        public FP m00;
        public FP m33;
        public FP m23;
        public FP m13;
        public FP m03;
        public FP m32;
        public FP m22;
        public FP m02;
        public FP m12;
        public FP m21;
        public FP m11;
        public FP m01;
        public FP m30;
        public FP m20;
        public FP m10;
        public FP m31;

        public FPMatrix4x4(FPVector4 column0, FPVector4 column1, FPVector4 column2, FPVector4 column3)
        {
            this.m00 = column0.x; this.m01 = column1.x; this.m02 = column2.x; this.m03 = column3.x;
            this.m10 = column0.y; this.m11 = column1.y; this.m12 = column2.y; this.m13 = column3.y;
            this.m20 = column0.z; this.m21 = column1.z; this.m22 = column2.z; this.m23 = column3.z;
            this.m30 = column0.w; this.m31 = column1.w; this.m32 = column2.w; this.m33 = column3.w;
        }

        // Access element at [row, column].
        public FP this[int row, int column]
        {
            get
            {
                return this[row + column * 4];
            }

            set
            {
                this[row + column * 4] = value;
            }
        }

        // Access element at sequential index (0..15 inclusive).
        public FP this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return m00;
                    case 1: return m10;
                    case 2: return m20;
                    case 3: return m30;
                    case 4: return m01;
                    case 5: return m11;
                    case 6: return m21;
                    case 7: return m31;
                    case 8: return m02;
                    case 9: return m12;
                    case 10: return m22;
                    case 11: return m32;
                    case 12: return m03;
                    case 13: return m13;
                    case 14: return m23;
                    case 15: return m33;
                    default:
                        throw new IndexOutOfRangeException("Invalid matrix index!");
                }
            }

            set
            {
                switch (index)
                {
                    case 0: m00 = value; break;
                    case 1: m10 = value; break;
                    case 2: m20 = value; break;
                    case 3: m30 = value; break;
                    case 4: m01 = value; break;
                    case 5: m11 = value; break;
                    case 6: m21 = value; break;
                    case 7: m31 = value; break;
                    case 8: m02 = value; break;
                    case 9: m12 = value; break;
                    case 10: m22 = value; break;
                    case 11: m32 = value; break;
                    case 12: m03 = value; break;
                    case 13: m13 = value; break;
                    case 14: m23 = value; break;
                    case 15: m33 = value; break;

                    default:
                        throw new IndexOutOfRangeException("Invalid matrix index!");
                }
            }
        }

        public void Clear()
        {
            for (int i = 0; i < 16; i++)
                this[i] = 0;
        }


        public void swap_rows(ref FP[] r0, ref FP[] r1)
        {
            if (r0 == r1 || r0.Length != r1.Length || r0.Length <= 0)
            {
                return;
            }

            FP[] temp = (FP[])r0.Clone();
            Array.Copy(r0, r1, r0.Length);
            Array.Copy(r1, temp, r1.Length);
        }

        public bool InvertMatrix4x4_Full(ref FPMatrix4x4 m, out FPMatrix4x4 output)
        {
            output = new FPMatrix4x4();
            output.Clear();

            FP[,] wtmp = new FP[4, 8];
            FP m0, m1, m2, m3, s;

            FP[] r0 = new FP[8];
            FP[] r1 = new FP[8];
            FP[] r2 = new FP[8];
            FP[] r3 = new FP[8];


            r0[0] = m.m00; r0[1] = m.m01;
            r0[2] = m.m02; r0[3] = m.m03;
            r0[4] = 1.0; r0[5] = r0[6] = r0[7] = 0.0;

            r1[0] = m.m00; r1[1] = m.m01;
            r1[2] = m.m02; r1[3] = m.m03;
            r1[5] = 1.0; r1[4] = r1[6] = r1[7] = 0.0;

            r2[0] = m.m00; r2[1] = m.m01;
            r2[2] = m.m02; r2[3] = m.m03;
            r2[6] = 1.0; r2[4] = r2[5] = r2[7] = 0.0;

            r3[0] = m.m00; r3[1] = m.m01;
            r3[2] = m.m02; r3[3] = m.m03;
            r3[7] = 1.0; r3[4] = r3[5] = r3[6] = 0.0;

            /* choose pivot - or die */
            if (FPMath.Abs(r3[0]) > FPMath.Abs(r2[0])) swap_rows(ref r3, ref r2);
            if (FPMath.Abs(r2[0]) > FPMath.Abs(r1[0])) swap_rows(ref r2, ref r1);
            if (FPMath.Abs(r1[0]) > FPMath.Abs(r0[0])) swap_rows(ref r1, ref r0);

            if (0.0F == r0[0])
            {
                return false;
            }

            /* eliminate first variable     */
            m1 = r1[0] / r0[0]; m2 = r2[0] / r0[0]; m3 = r3[0] / r0[0];
            s = r0[1]; r1[1] -= m1 * s; r2[1] -= m2 * s; r3[1] -= m3 * s;
            s = r0[2]; r1[2] -= m1 * s; r2[2] -= m2 * s; r3[2] -= m3 * s;
            s = r0[3]; r1[3] -= m1 * s; r2[3] -= m2 * s; r3[3] -= m3 * s;
            s = r0[4];
            if (s != 0) { r1[4] -= m1 * s; r2[4] -= m2 * s; r3[4] -= m3 * s; }
            s = r0[5];
            if (s != 0) { r1[5] -= m1 * s; r2[5] -= m2 * s; r3[5] -= m3 * s; }
            s = r0[6];
            if (s != 0) { r1[6] -= m1 * s; r2[6] -= m2 * s; r3[6] -= m3 * s; }
            s = r0[7];
            if (s != 0) { r1[7] -= m1 * s; r2[7] -= m2 * s; r3[7] -= m3 * s; }

            /* choose pivot - or die */
            if (FPMath.Abs(r3[1]) > FPMath.Abs(r2[1])) swap_rows(ref r3, ref r2);
            if (FPMath.Abs(r2[1]) > FPMath.Abs(r1[1])) swap_rows(ref r2, ref r1);

            if (0 == r1[1])
            {
                return false;
            }

            /* eliminate second variable */
            m2 = r2[1] / r1[1]; m3 = r3[1] / r1[1];
            r2[2] -= m2 * r1[2]; r3[2] -= m3 * r1[2];
            r2[3] -= m2 * r1[3]; r3[3] -= m3 * r1[3];
            s = r1[4]; if (0.0F != s) { r2[4] -= m2 * s; r3[4] -= m3 * s; }
            s = r1[5]; if (0.0F != s) { r2[5] -= m2 * s; r3[5] -= m3 * s; }
            s = r1[6]; if (0.0F != s) { r2[6] -= m2 * s; r3[6] -= m3 * s; }
            s = r1[7]; if (0.0F != s) { r2[7] -= m2 * s; r3[7] -= m3 * s; }

            /* choose pivot - or die */
            if (FPMath.Abs(r3[2]) > FPMath.Abs(r2[2])) swap_rows(ref r3, ref r2);
            if (0.0F == r2[2])
            {
                return false;
            }

            /* eliminate third variable */
            m3 = r3[2] / r2[2];
            r3[3] -= m3 * r2[3]; r3[4] -= m3 * r2[4];
            r3[5] -= m3 * r2[5]; r3[6] -= m3 * r2[6];
            r3[7] -= m3 * r2[7];

            /* last check */
            if (0.0F == r3[3])
            {
                return false;
            }

            s = 1.0F / r3[3];             /* now back substitute row 3 */
            r3[4] *= s; r3[5] *= s; r3[6] *= s; r3[7] *= s;

            m2 = r2[3];                 /* now back substitute row 2 */
            s = 1.0F / r2[2];
            r2[4] = s * (r2[4] - r3[4] * m2); r2[5] = s * (r2[5] - r3[5] * m2);
            r2[6] = s * (r2[6] - r3[6] * m2); r2[7] = s * (r2[7] - r3[7] * m2);
            m1 = r1[3];
            r1[4] -= r3[4] * m1; r1[5] -= r3[5] * m1;
            r1[6] -= r3[6] * m1; r1[7] -= r3[7] * m1;
            m0 = r0[3];
            r0[4] -= r3[4] * m0; r0[5] -= r3[5] * m0;
            r0[6] -= r3[6] * m0; r0[7] -= r3[7] * m0;

            m1 = r1[2];                 /* now back substitute row 1 */
            s = 1.0F / r1[1];
            r1[4] = s * (r1[4] - r2[4] * m1); r1[5] = s * (r1[5] - r2[5] * m1);
            r1[6] = s * (r1[6] - r2[6] * m1); r1[7] = s * (r1[7] - r2[7] * m1);
            m0 = r0[2];
            r0[4] -= r2[4] * m0; r0[5] -= r2[5] * m0;
            r0[6] -= r2[6] * m0; r0[7] -= r2[7] * m0;

            m0 = r0[1];                 /* now back substitute row 0 */
            s = 1.0F / r0[0];
            r0[4] = s * (r0[4] - r1[4] * m0); r0[5] = s * (r0[5] - r1[5] * m0);
            r0[6] = s * (r0[6] - r1[6] * m0); r0[7] = s * (r0[7] - r1[7] * m0);

            output.m00 = r0[4]; output.m01 = r0[5]; output.m02 = r0[6]; output.m03 = r0[7];
            output.m10 = r1[4]; output.m11 = r1[5]; output.m12 = r1[6]; output.m13 = r1[7];
            output.m20 = r2[4]; output.m21 = r2[5]; output.m22 = r2[6]; output.m23 = r2[7];
            output.m30 = r3[4]; output.m31 = r3[5]; output.m32 = r3[6]; output.m33 = r3[7];

            return true;
        }

        public FPMatrix4x4 Invert_Full()
        {
            FPMatrix4x4 output;
            InvertMatrix4x4_Full(ref this, out output);
            return output;
        }

        // Returns true if the distance between f0 and f1 is smaller than epsilon
        public bool CompareApproximately(FP f0, FP f1, FP epsilon)
        {
            FP dist = (f0 - f1);
            dist = FPMath.Abs(dist);
            return dist < epsilon;
        }

        public bool CompareApproximately(FP f0, FP f1)
        {
            return CompareApproximately(f0, f1, FP.Epsilon);
        }

        bool CompareApproximately(ref FPMatrix4x4 lhs, ref FPMatrix4x4 rhs, FP dist)
        {
	        for (int i=0;i<16;i++)
	        {
		        if (!CompareApproximately(lhs[i], rhs[i], dist))
			        return false;
	        }
	        return true;
        }

        public bool IsIdentity(FP threshold)
        {
            if (CompareApproximately(Get(0, 0), 1.0f, threshold) && CompareApproximately(Get(0, 1), 0.0f, threshold) && CompareApproximately(Get(0, 2), 0.0f, threshold) && CompareApproximately(Get(0, 3), 0.0f, threshold) &&
                CompareApproximately(Get(1, 0), 0.0f, threshold) && CompareApproximately(Get(1, 1), 1.0f, threshold) && CompareApproximately(Get(1, 2), 0.0f, threshold) && CompareApproximately(Get(1, 3), 0.0f, threshold) &&
                CompareApproximately(Get(2, 0), 0.0f, threshold) && CompareApproximately(Get(2, 1), 0.0f, threshold) && CompareApproximately(Get(2, 2), 1.0f, threshold) && CompareApproximately(Get(2, 3), 0.0f, threshold) &&
                   CompareApproximately(Get(3, 0), 0.0f, threshold) && CompareApproximately(Get(3, 1), 0.0f, threshold) && CompareApproximately(Get(3, 2), 0.0f, threshold) && CompareApproximately(Get(3, 3), 1.0f, threshold))
            {
                return true;
            }
                
            return false;
        }

        public bool IsIdentity()
        {
            return IsIdentity(FP.Epsilon);
        }
        FP Get(int row, int column) { return this[row + (column * 4)]; }
        public bool isIdentity { get { return IsIdentity(); } }
        public FPMatrix4x4 inverse { get { return Invert_Full(); } }

         
        // used to allow FPMatrix4x4s to be used as keys in hash tables
        public override int GetHashCode()
        {
            return GetColumn(0).GetHashCode() ^ (GetColumn(1).GetHashCode() << 2) ^ (GetColumn(2).GetHashCode() >> 2) ^ (GetColumn(3).GetHashCode() >> 1);
        }

        // also required for being able to use FPMatrix4x4s as keys in hash tables
        public override bool Equals(object other)
        {
            if (!(other is FPMatrix4x4)) return false;

            return Equals((FPMatrix4x4)other);
        }

        public bool Equals(FPMatrix4x4 other)
        {
            return GetColumn(0).Equals(other.GetColumn(0))
                && GetColumn(1).Equals(other.GetColumn(1))
                && GetColumn(2).Equals(other.GetColumn(2))
                && GetColumn(3).Equals(other.GetColumn(3));
        }

        // Multiplies two matrices.
        public static FPMatrix4x4 operator *(FPMatrix4x4 lhs, FPMatrix4x4 rhs)
        {
            FPMatrix4x4 res;
            res.m00 = lhs.m00 * rhs.m00 + lhs.m01 * rhs.m10 + lhs.m02 * rhs.m20 + lhs.m03 * rhs.m30;
            res.m01 = lhs.m00 * rhs.m01 + lhs.m01 * rhs.m11 + lhs.m02 * rhs.m21 + lhs.m03 * rhs.m31;
            res.m02 = lhs.m00 * rhs.m02 + lhs.m01 * rhs.m12 + lhs.m02 * rhs.m22 + lhs.m03 * rhs.m32;
            res.m03 = lhs.m00 * rhs.m03 + lhs.m01 * rhs.m13 + lhs.m02 * rhs.m23 + lhs.m03 * rhs.m33;

            res.m10 = lhs.m10 * rhs.m00 + lhs.m11 * rhs.m10 + lhs.m12 * rhs.m20 + lhs.m13 * rhs.m30;
            res.m11 = lhs.m10 * rhs.m01 + lhs.m11 * rhs.m11 + lhs.m12 * rhs.m21 + lhs.m13 * rhs.m31;
            res.m12 = lhs.m10 * rhs.m02 + lhs.m11 * rhs.m12 + lhs.m12 * rhs.m22 + lhs.m13 * rhs.m32;
            res.m13 = lhs.m10 * rhs.m03 + lhs.m11 * rhs.m13 + lhs.m12 * rhs.m23 + lhs.m13 * rhs.m33;

            res.m20 = lhs.m20 * rhs.m00 + lhs.m21 * rhs.m10 + lhs.m22 * rhs.m20 + lhs.m23 * rhs.m30;
            res.m21 = lhs.m20 * rhs.m01 + lhs.m21 * rhs.m11 + lhs.m22 * rhs.m21 + lhs.m23 * rhs.m31;
            res.m22 = lhs.m20 * rhs.m02 + lhs.m21 * rhs.m12 + lhs.m22 * rhs.m22 + lhs.m23 * rhs.m32;
            res.m23 = lhs.m20 * rhs.m03 + lhs.m21 * rhs.m13 + lhs.m22 * rhs.m23 + lhs.m23 * rhs.m33;

            res.m30 = lhs.m30 * rhs.m00 + lhs.m31 * rhs.m10 + lhs.m32 * rhs.m20 + lhs.m33 * rhs.m30;
            res.m31 = lhs.m30 * rhs.m01 + lhs.m31 * rhs.m11 + lhs.m32 * rhs.m21 + lhs.m33 * rhs.m31;
            res.m32 = lhs.m30 * rhs.m02 + lhs.m31 * rhs.m12 + lhs.m32 * rhs.m22 + lhs.m33 * rhs.m32;
            res.m33 = lhs.m30 * rhs.m03 + lhs.m31 * rhs.m13 + lhs.m32 * rhs.m23 + lhs.m33 * rhs.m33;

            return res;
        }
        // Transforms a [[FPVector4]] by a matrix.
        public static FPVector4 operator *(FPMatrix4x4 lhs, FPVector4 vector)
        {
            FPVector4 res;
            res.x = lhs.m00 * vector.x + lhs.m01 * vector.y + lhs.m02 * vector.z + lhs.m03 * vector.w;
            res.y = lhs.m10 * vector.x + lhs.m11 * vector.y + lhs.m12 * vector.z + lhs.m13 * vector.w;
            res.z = lhs.m20 * vector.x + lhs.m21 * vector.y + lhs.m22 * vector.z + lhs.m23 * vector.w;
            res.w = lhs.m30 * vector.x + lhs.m31 * vector.y + lhs.m32 * vector.z + lhs.m33 * vector.w;
            return res;
        }

        //*undoc*
        public static bool operator ==(FPMatrix4x4 lhs, FPMatrix4x4 rhs)
        {
            // Returns false in the presence of NaN values.
            return lhs.GetColumn(0) == rhs.GetColumn(0)
                && lhs.GetColumn(1) == rhs.GetColumn(1)
                && lhs.GetColumn(2) == rhs.GetColumn(2)
                && lhs.GetColumn(3) == rhs.GetColumn(3);
        }

        //*undoc*
        public static bool operator !=(FPMatrix4x4 lhs, FPMatrix4x4 rhs)
        {
            // Returns true in the presence of NaN values.
            return !(lhs == rhs);
        }

        // Get a column of the matrix.
        public FPVector4 GetColumn(int index)
        {
            switch (index)
            {
                case 0: return new FPVector4(m00, m10, m20, m30);
                case 1: return new FPVector4(m01, m11, m21, m31);
                case 2: return new FPVector4(m02, m12, m22, m32);
                case 3: return new FPVector4(m03, m13, m23, m33);
                default:
                    throw new IndexOutOfRangeException("Invalid column index!");
            }
        }

        // Returns a row of the matrix.
        public FPVector4 GetRow(int index)
        {
            switch (index)
            {
                case 0: return new FPVector4(m00, m01, m02, m03);
                case 1: return new FPVector4(m10, m11, m12, m13);
                case 2: return new FPVector4(m20, m21, m22, m23);
                case 3: return new FPVector4(m30, m31, m32, m33);
                default:
                    throw new IndexOutOfRangeException("Invalid row index!");
            }
        }

        // Sets a column of the matrix.
        public void SetColumn(int index, FPVector4 column)
        {
            this[0, index] = column.x;
            this[1, index] = column.y;
            this[2, index] = column.z;
            this[3, index] = column.w;
        }

        // Sets a row of the matrix.
        public void SetRow(int index, FPVector4 row)
        {
            this[index, 0] = row.x;
            this[index, 1] = row.y;
            this[index, 2] = row.z;
            this[index, 3] = row.w;
        }

        // Transforms a position by this matrix, with a perspective divide. (generic)
        public FPVector MultiplyPoint(FPVector point)
        {
            FPVector res;
            FP w;
            res.x = this.m00 * point.x + this.m01 * point.y + this.m02 * point.z + this.m03;
            res.y = this.m10 * point.x + this.m11 * point.y + this.m12 * point.z + this.m13;
            res.z = this.m20 * point.x + this.m21 * point.y + this.m22 * point.z + this.m23;
            w = this.m30 * point.x + this.m31 * point.y + this.m32 * point.z + this.m33;

            w = 1F / w;
            res.x *= w;
            res.y *= w;
            res.z *= w;
            return res;
        }

        // Transforms a position by this matrix, without a perspective divide. (fast)
        public FPVector MultiplyPoint3x4(FPVector point)
        {
            FPVector res;
            res.x = this.m00 * point.x + this.m01 * point.y + this.m02 * point.z + this.m03;
            res.y = this.m10 * point.x + this.m11 * point.y + this.m12 * point.z + this.m13;
            res.z = this.m20 * point.x + this.m21 * point.y + this.m22 * point.z + this.m23;
            return res;
        }

        // Transforms a direction by this matrix.
        public FPVector MultiplyVector(FPVector vector)
        {
            FPVector res;
            res.x = this.m00 * vector.x + this.m01 * vector.y + this.m02 * vector.z;
            res.y = this.m10 * vector.x + this.m11 * vector.y + this.m12 * vector.z;
            res.z = this.m20 * vector.x + this.m21 * vector.y + this.m22 * vector.z;
            return res;
        }


        // Transforms a plane by this matrix.
        public Plane TransformPlane(Plane plane)
        {
            FPMatrix4x4 ittrans = this.inverse;

            FP x = plane.normal.x, y = plane.normal.y, z = plane.normal.z, w = plane.distance;
            // note: a transpose is part of this transformation
            FP a = ittrans.m00 * x + ittrans.m10 * y + ittrans.m20 * z + ittrans.m30 * w;
            FP b = ittrans.m01 * x + ittrans.m11 * y + ittrans.m21 * z + ittrans.m31 * w;
            FP c = ittrans.m02 * x + ittrans.m12 * y + ittrans.m22 * z + ittrans.m32 * w;
            FP d = ittrans.m03 * x + ittrans.m13 * y + ittrans.m23 * z + ittrans.m33 * w;

            // note: Ignore the float here , result have accuracy loss
            return new Plane((new FPVector(a.AsInt(), b.AsInt(), c.AsInt())).ToVector(), (float)(d.AsInt()));
        }

        // Creates a scaling matrix.
        public static FPMatrix4x4 Scale(FPVector vector)
        {
            FPMatrix4x4 m;
            m.m00 = vector.x; m.m01 = 0F; m.m02 = 0F; m.m03 = 0F;
            m.m10 = 0F; m.m11 = vector.y; m.m12 = 0F; m.m13 = 0F;
            m.m20 = 0F; m.m21 = 0F; m.m22 = vector.z; m.m23 = 0F;
            m.m30 = 0F; m.m31 = 0F; m.m32 = 0F; m.m33 = 1F;
            return m;
        }

        // Creates a translation matrix.
        public static FPMatrix4x4 Translate(FPVector vector)
        {
            FPMatrix4x4 m;
            m.m00 = 1F; m.m01 = 0F; m.m02 = 0F; m.m03 = vector.x;
            m.m10 = 0F; m.m11 = 1F; m.m12 = 0F; m.m13 = vector.y;
            m.m20 = 0F; m.m21 = 0F; m.m22 = 1F; m.m23 = vector.z;
            m.m30 = 0F; m.m31 = 0F; m.m32 = 0F; m.m33 = 1F;
            return m;
        }

        // Creates a rotation matrix. Note: Assumes unit quaternion
        public static FPMatrix4x4 Rotate(FPQuaternion q)
        {
            // Precalculate coordinate products
            FP x = q.x * 2.0F;
            FP y = q.y * 2.0F;
            FP z = q.z * 2.0F;
            FP xx = q.x * x;
            FP yy = q.y * y;
            FP zz = q.z * z;
            FP xy = q.x * y;
            FP xz = q.x * z;
            FP yz = q.y * z;
            FP wx = q.w * x;
            FP wy = q.w * y;
            FP wz = q.w * z;

            // Calculate 3x3 matrix from orthonormal basis
            FPMatrix4x4 m;
            m.m00 = 1.0f - (yy + zz); m.m10 = xy + wz; m.m20 = xz - wy; m.m30 = 0.0F;
            m.m01 = xy - wz; m.m11 = 1.0f - (xx + zz); m.m21 = yz + wx; m.m31 = 0.0F;
            m.m02 = xz + wy; m.m12 = yz - wx; m.m22 = 1.0f - (xx + yy); m.m32 = 0.0F;
            m.m03 = 0.0F; m.m13 = 0.0F; m.m23 = 0.0F; m.m33 = 1.0F;
            return m;
        }

        // FPMatrix4x4.zero is of questionable usefulness considering C# sets everything to 0 by default, however:
        //  1. it's consistent with other Math structs in Unity such as Vector2, FPVector and FPVector4,
        //  2. "FPMatrix4x4.zero" is arguably more readable than "new FPMatrix4x4()",
        //  3. it's already in the API ..
        static readonly FPMatrix4x4 zeroMatrix = new FPMatrix4x4(new FPVector4(0, 0, 0, 0),
            new FPVector4(0, 0, 0, 0),
            new FPVector4(0, 0, 0, 0),
            new FPVector4(0, 0, 0, 0));

        // Returns a matrix with all elements set to zero (RO).
        public static FPMatrix4x4 zero { get { return zeroMatrix; } }

        static readonly FPMatrix4x4 identityMatrix = new FPMatrix4x4(new FPVector4(1, 0, 0, 0),
            new FPVector4(0, 1, 0, 0),
            new FPVector4(0, 0, 1, 0),
            new FPVector4(0, 0, 0, 1));

        // Returns the identity matrix (RO).
        public static FPMatrix4x4 identity { get { return identityMatrix; } }

        public override string ToString()
        {
            return string.Format("{0:F5}\t{1:F5}\t{2:F5}\t{3:F5}\n{4:F5}\t{5:F5}\t{6:F5}\t{7:F5}\n{8:F5}\t{9:F5}\t{10:F5}\t{11:F5}\n{12:F5}\t{13:F5}\t{14:F5}\t{15:F5}\n", m00, m01, m02, m03, m10, m11, m12, m13, m20, m21, m22, m23, m30, m31, m32, m33);
        }

        public string ToString(string format)
        {
            return string.Format("{0}\t{1}\t{2}\t{3}\n{4}\t{5}\t{6}\t{7}\n{8}\t{9}\t{10}\t{11}\n{12}\t{13}\t{14}\t{15}\n",
                m00.AsFloat(), m01.AsFloat(), m02.AsFloat(), m03.AsFloat(),
                m10.AsFloat(), m11.AsFloat(), m12.AsFloat(), m13.AsFloat(),
                m20.AsFloat(), m21.AsFloat(), m22.AsFloat(), m23.AsFloat(),
                m30.AsFloat(), m31.AsFloat(), m32.AsFloat(), m33.AsFloat());
        }
    }
}

