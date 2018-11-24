using System;
using UnityEngine;
using UnityEngine.Serialization;
using KBEngine.Physics3D;

namespace KBEngine {
    /**
     *  @brief Abstract collider for 3D shapes. 
     **/
    [RequireComponent(typeof(FPTransform))]
    [Serializable]
    [ExecuteInEditMode]
    public abstract class FPCollider : MonoBehaviour, ICollider {

        private Shape shape;

        /**
         *  @brief Shape used by a collider.
         **/
        public Shape Shape {
            get {
                if (shape == null)
                    shape = CreateShape();
                return shape;
            }
            protected set { shape = value; }
        }

        [FormerlySerializedAs("isTrigger")]
        [SerializeField]
        private bool _isTrigger;

        /**
         *  @brief If it is only a trigger and doesn't interfere on collisions. 
         **/
        public bool isTrigger {
            get {
                if (_body != null) {
                    return _body.IsColliderOnly;
                }

                return _isTrigger;
            }
            set {
                _isTrigger = value;

                if (_body != null) {
                    _body.IsColliderOnly = _isTrigger;
                }
            }
        }

        /**
         *  @brief Simulated material. 
         **/
        public FPMaterial tsMaterial;

        [SerializeField]
        private FPVector center;

        private Vector3 scaledCenter;

        internal RigidBody _body;

        /**
         *  @brief Center of the collider shape.
         **/
        public FPVector Center {
            get {
                return center;
            }
            set {
                center = value;
            }
        }

        /**
         *  @brief Returns a version of collider's center scaled by parent's transform.
         */
        public FPVector ScaledCenter {
			get {
				return FPVector.Scale (Center, lossyScale);
			}
		}

        /**
         *  @brief Creates the shape related to a concrete implementation of TSCollider.
         **/
        public abstract Shape CreateShape();

        private FPRigidBody FPRigidBody;

        /**
         *  @brief Returns the {@link FPRigidBody} attached.
         */
        public FPRigidBody attachedRigidbody {
            get {
                return FPRigidBody;
            }
        }

        /**
         *  @brief Returns body's boundind box.
         */
        public TSBBox bounds {
            get {
                return this._body.BoundingBox;
            }
        }

        /**
         *  @brief Returns the body linked to this collider.
         */
        public IBody3D Body {
            get {
                if (_body == null) {
                    CheckPhysics();
                }

                return _body;
            }
        }

        /**
         *  @brief Holds an first value of the GameObject's lossy scale.
         **/
        [SerializeField]
        [HideInInspector]
        protected FPVector lossyScale = FPVector.one;

        [HideInInspector]
        public FPTransform FPTransform;

        /**
         *  @brief Creates a new {@link FPRigidBody} when there is no one attached to this GameObject.
         **/
        public void Awake() {
            FPTransform = this.GetComponent<FPTransform>();
            FPRigidBody = this.GetComponent<FPRigidBody>();

            if (lossyScale == FPVector.one) {
                lossyScale = FPVector.Abs(transform.localScale.ToFPVector());
            }
        }

        public void Update() {
            if (!Application.isPlaying) {
                lossyScale = FPVector.Abs(transform.lossyScale.ToFPVector());
            }
        }

        private void CreateBody() {
            RigidBody newBody = new RigidBody(Shape);

            if (tsMaterial == null) {
                tsMaterial = GetComponent<FPMaterial>();
            }

            if (tsMaterial != null) {
                newBody.TSFriction = tsMaterial.friction;
                newBody.TSRestitution = tsMaterial.friction;
            }

            newBody.IsColliderOnly = isTrigger;
            newBody.IsKinematic = FPRigidBody != null && FPRigidBody.isKinematic;

            bool isStatic = FPRigidBody == null || FPRigidBody.isKinematic;

            if (FPRigidBody != null) {
                newBody.AffectedByGravity = FPRigidBody.useGravity;

                if (FPRigidBody.mass <= 0) {
                    FPRigidBody.mass = 1;
                }

                newBody.Mass = FPRigidBody.mass;
                newBody.TSLinearDrag = FPRigidBody.drag;
                newBody.TSAngularDrag = FPRigidBody.angularDrag;
            } else {
                newBody.SetMassProperties();
            }

            if (isStatic) {
                newBody.AffectedByGravity = false;
                newBody.IsStatic = true;
            }

            _body = newBody;
        }

        /**
         *  @brief Initializes Shape and RigidBody and sets initial values to position and orientation based on Unity's transform.
         **/
        public void Initialize() {
            CreateBody();
        }

        private void CheckPhysics() {
            if (_body == null && PhysicsManager.instance != null) {
                PhysicsManager.instance.AddBody(this);
            }
        }

        /**
         *  @brief Do a base matrix transformation to draw correctly all collider gizmos.
         **/
        public virtual void OnDrawGizmos() {
            if (!this.enabled) {
                return;
            }

            Vector3 position = _body != null ? _body.Position.ToVector() : (transform.position + ScaledCenter.ToVector());
            Quaternion rotation = _body != null ? _body.Orientation.ToQuaternion() : transform.rotation;

            Gizmos.color = Color.yellow;

			Matrix4x4 cubeTransform = Matrix4x4.TRS(position, rotation, GetGizmosSize());
            Matrix4x4 oldGizmosMatrix = Gizmos.matrix;

            Gizmos.matrix *= cubeTransform;

            DrawGizmos();

            Gizmos.matrix = oldGizmosMatrix;
        }

        /**
         *  @brief Returns the gizmos size.
         **/
        protected abstract Vector3 GetGizmosSize();

        /**
         *  @brief Draws the specific gizmos of concrete collider (for example "Gizmos.DrawWireCube" for a {@link TSBoxCollider}).
         **/
        protected abstract void DrawGizmos();

        /**
         *  @brief Returns true if the body was already initialized.
         **/
        public bool IsBodyInitialized {
            get {
                return _body != null;
            }
        }
    }

}