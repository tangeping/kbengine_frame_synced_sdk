using UnityEngine;
using UnityEngine.Serialization;
using KBEngine.Physics3D;

namespace KBEngine {
    /**
     *  @brief Collider with a box shape. 
     **/
    [AddComponentMenu("FrameSync/Physics/BoxCollider", 0)]
    public class FPBoxCollider : FPCollider {

        [FormerlySerializedAs("size")]
        [SerializeField]
        private Vector3 _size = Vector3.one;

        /**
         *  @brief Size of the box. 
         **/
        public FPVector size {
            get {
                if (_body != null) {
                    FPVector boxSize = ((BoxShape)_body.Shape).Size;
                    boxSize.x /= lossyScale.x;
                    boxSize.y /= lossyScale.y;
                    boxSize.z /= lossyScale.z;

                    return boxSize;
                }

                return _size.ToFPVector();
            }

            set {
                _size = value.ToVector();

                if (_body != null) {
                    ((BoxShape)_body.Shape).Size = FPVector.Scale(value, lossyScale);
                }
            }
        }


        /**
         *  @brief Sets initial values to {@link #size} based on a pre-existing BoxCollider or BoxCollider2D.
         **/
        public void Reset() {
            if (GetComponent<BoxCollider2D>() != null) {
                BoxCollider2D boxCollider2D = GetComponent<BoxCollider2D>();

                size = new FPVector(boxCollider2D.size.x, boxCollider2D.size.y, 1);
                Center = new FPVector(boxCollider2D.offset.x, boxCollider2D.offset.y, 0);
                isTrigger = boxCollider2D.isTrigger;
            } else if (GetComponent<BoxCollider>() != null) {
                BoxCollider boxCollider = GetComponent<BoxCollider>();

                size = new FPVector(boxCollider.size.x, boxCollider.size.y, 1);
                Center = boxCollider.center.ToFPVector();
                isTrigger = boxCollider.isTrigger;
            }
        }

        /**
         *  @brief Create the internal shape used to represent a TSBoxCollider.
         **/
        public override Shape CreateShape() {
			return new BoxShape(FPVector.Scale(size, lossyScale));
        }

        protected override void DrawGizmos() {
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }

        protected override Vector3 GetGizmosSize() {
			return FPVector.Scale(size, lossyScale).ToVector();
        }        

    }

}