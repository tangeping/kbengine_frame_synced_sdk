using UnityEngine;
using UnityEngine.Serialization;

namespace KBEngine {
    /**
     *  @brief Collider with a box 2D shape. 
     **/
    [AddComponentMenu("FrameSync/Physics/BoxCollider2D", 0)]
    public class FPBoxCollider2D : FPCollider2D {

        [FormerlySerializedAs("size")]
        [SerializeField]
        private FPVector2 _size = FPVector2.one;

        /**
         *  @brief Size of the box. 
         **/
        public FPVector2 size {
            get {
                if (_body != null) {
                    FPVector2 halfVector = ((Physics2D.PolygonShape)_body.FixtureList[0].Shape).Vertices[0] * 2;
                    halfVector.x /= lossyScale.x;
                    halfVector.y /= -lossyScale.y;

                    return halfVector;
                }

                return _size;
            }
            set {
                _size = value;

                if (_body != null) {
                    FPVector size3 = new FPVector(_size.x, _size.y, 1);
                    FPVector sizeScaled = FPVector.Scale(size3, lossyScale);

                    ((Physics2D.PolygonShape)_body.FixtureList[0].Shape).Vertices = KBEngine.Physics2D.PolygonTools.CreateRectangle(sizeScaled.x * FP.Half, sizeScaled.y * FP.Half);
                }

            }
        }

        /**
         *  @brief Sets initial values to {@link #size} based on a pre-existing BoxCollider or BoxCollider2D.
         **/
        public void Reset() {
            if (GetComponent<BoxCollider2D>() != null) {
                BoxCollider2D boxCollider2D = GetComponent<BoxCollider2D>();

                size = new FPVector2(boxCollider2D.size.x, boxCollider2D.size.y);
                Center = new FPVector2(boxCollider2D.offset.x, boxCollider2D.offset.y);
                isTrigger = boxCollider2D.isTrigger;
            } else if (GetComponent<BoxCollider>() != null) {
                BoxCollider boxCollider = GetComponent<BoxCollider>();

                size = boxCollider.size.ToFPVector2();
                Center = boxCollider.center.ToFPVector2();
                isTrigger = boxCollider.isTrigger;
            }
        }

        /**
         *  @brief Create the internal shape used to represent a TSBoxCollider.
         **/
        public override KBEngine.Physics2D.Shape CreateShape() {
            FPVector size3 = new FPVector(size.x, size.y, 1);
            FPVector sizeScaled = FPVector.Scale(size3, lossyScale);
            return new KBEngine.Physics2D.PolygonShape(KBEngine.Physics2D.PolygonTools.CreateRectangle(sizeScaled.x * FP.Half, sizeScaled.y * FP.Half), 1);
        }

        protected override void DrawGizmos() {
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }

        protected override Vector3 GetGizmosSize() {
            FPVector size3 = new FPVector(size.x, size.y, 1);
            return FPVector.Scale(size3, lossyScale).ToVector();
        }

    }

}