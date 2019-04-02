using KBEngine;
using System.Reflection;
using UnityEngine;

/**
* @brief Extensions added by FrameSync.
**/
public static class FrameSyncExtensions {

    public static FPVector ToFPVector(this Vector3 vector) {
        return new FPVector(vector.x, vector.y, vector.z);
    }
    public static FPVector ToFPVector(this Vector3Int vector)
    {
        return new FPVector(vector.x, vector.y, vector.z);
    }

    public static FPVector2 ToFPVector2(this Vector3 vector) {
        return new FPVector2(vector.x, vector.y);
    }

    public static FPVector ToFPVector(this Vector2 vector) {
        return new FPVector(vector.x, vector.y, 0);
    }

    public static FPVector2 ToFPVector2(this Vector2 vector) {
        return new FPVector2(vector.x, vector.y);
    }
    public static FPVector2 ToFPVector2(this Vector2Int vector)
    {
        return new FPVector2(vector.x, vector.y);
    }

    public static Vector3 Abs(this Vector3 vector) {
		return new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
	}

    public static FPQuaternion ToFPQuaternion(this Quaternion rot) {
        return new FPQuaternion(rot.x, rot.y, rot.z, rot.w);
    }

    public static Quaternion ToQuaternion(this FPQuaternion rot) {
        return new Quaternion((float)rot.x, (float)rot.y, (float)rot.z, (float)rot.w);
    }

    public static FPMatrix ToTSMatrix(this Quaternion rot) {
        return FPMatrix.CreateFromQuaternion(rot.ToFPQuaternion());
    }

    public static Vector3 ToVector(this FPVector fVector) {
        return new Vector3((float) fVector.x, (float) fVector.y, (float) fVector.z);
    }
    public static Vector3Int ToVector3Int(this FPVector fVector)
    {
        return new Vector3Int((int)fVector.x, (int)fVector.y, (int)fVector.z);
    }

    public static Vector3 ToVector(this FPVector2 fVector) {
        return new Vector3((float)fVector.x, (float)fVector.y, 0);
    }



    public static void Set(this FPVector fVector, FPVector otherVector) {
        fVector.Set(otherVector.x, otherVector.y, otherVector.z);
    }

    public static Quaternion ToQuaternion(this FPMatrix fMatrix) {
        return FPQuaternion.CreateFromMatrix(fMatrix).ToQuaternion();
    }

}