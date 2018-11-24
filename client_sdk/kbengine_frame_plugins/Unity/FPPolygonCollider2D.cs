using System.Collections.Generic;
using UnityEngine;

namespace KBEngine {

    /**
     *  @brief Collider with a polygon 2D shape. 
     **/
    [AddComponentMenu("FrameSync/Physics/PolygonCollider2D", 0)]
    public class FPPolygonCollider2D : FPCollider2D {

        [SerializeField]
        private FPVector2[] _points;

        public FPVector2[] points {
            get {
                return _points;
            }

            set {
                if (_body == null) {
                    _points = value;
                }
            }
        }

        /**
         *  @brief Create the internal shape used to represent a PolygonCollider.
         **/
        public override KBEngine.Physics2D.Shape[] CreateShapes() {
            if (_points == null || _points.Length == 0) {
                return null;
            }


            FPVector2 lossy2D = new FPVector2(lossyScale.x, lossyScale.y);
            KBEngine.Physics2D.Vertices v = new Physics2D.Vertices();
            for (int index = 0, length = _points.Length; index < length; index++) {
                v.Add(FPVector2.Scale(_points[index], lossy2D));
            }

            List<KBEngine.Physics2D.Vertices> convexShapeVs = KBEngine.Physics2D.BayazitDecomposer.ConvexPartition(v);
            KBEngine.Physics2D.Shape[] result = new Physics2D.Shape[convexShapeVs.Count];
            for (int index = 0, length = result.Length; index < length; index++) {
                result[index] = new KBEngine.Physics2D.PolygonShape(convexShapeVs[index], 1);
            }

            return result;
        }

        protected override void DrawGizmos() {
            DrawPolygon(_points);
        }

        private void DrawPolygon(FPVector2[] allPoints) {
            if (allPoints == null || allPoints.Length == 0) {
                return;
            }

            for (int index = 0, length = allPoints.Length - 1; index < length; index++) {
                Gizmos.DrawLine(allPoints[index].ToVector(), allPoints[index+1].ToVector());
            }

            Gizmos.DrawLine(allPoints[allPoints.Length - 1].ToVector(), allPoints[0].ToVector());
        }

        protected override Vector3 GetGizmosSize() {
            return lossyScale.ToVector();
        }

    }

}