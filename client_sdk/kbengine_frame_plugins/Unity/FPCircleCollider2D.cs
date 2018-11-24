using UnityEngine;
using UnityEngine.Serialization;

namespace KBEngine {
    /**
     *  @brief Collider with a circle shape. 
     **/
    [AddComponentMenu("FrameSync/Physics/CircleCollider2D", 0)]
    public class FPCircleCollider2D : FPCollider2D {

        [FormerlySerializedAs("radius")]
        [SerializeField]
        private FP _radius = 0.5f;

        /**
         *  @brief Radius of the sphere. 
         **/
        public FP radius {
            get {
                if (_body != null) {
                    return ((Physics2D.CircleShape) _body.FixtureList[0].Shape).Radius;
                }

                return _radius;
            }
            set {
                _radius = value;

                if (_body != null) {
                    Physics2D.CircleShape cShape = (Physics2D.CircleShape) _body.FixtureList[0].Shape;
                    if (cShape.Radius != _radius) {
                        cShape.Radius = _radius;
                    }
                }
            }
        }

        /**
         *  @brief Sets initial values to {@link #radius} based on a pre-existing SphereCollider or CircleCollider2D.
         **/
        public void Reset() {
            if (GetComponent<CircleCollider2D>() != null) {
                CircleCollider2D circleCollider2D = GetComponent<CircleCollider2D>();

                radius = circleCollider2D.radius;
                Center = new FPVector2(circleCollider2D.offset.x, circleCollider2D.offset.y);
                isTrigger = circleCollider2D.isTrigger;
            } else if (GetComponent<SphereCollider>() != null) {
                SphereCollider sphereCollider = GetComponent<SphereCollider>();

                radius = sphereCollider.radius;
                Center = sphereCollider.center.ToFPVector2();
                isTrigger = sphereCollider.isTrigger;
            }
        }

        /**
         *  @brief Create the internal shape used to represent a TSSphereCollider.
         **/
        public override KBEngine.Physics2D.Shape CreateShape() {
            return new KBEngine.Physics2D.CircleShape(radius, 1);
        }

        protected override void DrawGizmos() {
            Gizmos.DrawWireSphere(Vector3.zero, 1);
        }

        protected override Vector3 GetGizmosSize() {
            return Vector3.one * radius.AsFloat();
        }

    }

}