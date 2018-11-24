using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace KBEngine {
    /**
     *  @brief Abstract collider for 2D shapes.
     **/
    [RequireComponent(typeof(FPTransform2D))]
    [Serializable]
    [ExecuteInEditMode]
    public abstract class FPCollider2D : MonoBehaviour, ICollider {

        private Physics2D.Shape shape;

        /**
         *  @brief Shape used by a collider.
         **/
        public Physics2D.Shape Shape {
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
                    return _body.IsSensor;
                }

                return _isTrigger;
            }

            set {
                _isTrigger = value;

                if (_body != null) {
                    _body.IsSensor = value;
                }
            }

        }

        /**
         *  @brief Simulated material. 
         **/
        public FPMaterial tsMaterial;

        [SerializeField]
        private FPVector2 center;

        private Vector3 scaledCenter;

        internal Physics2D.Body _body;

        /**
         *  @brief Returns {@link RigidBody} associated to this FPRigidBody.         
         **/
        public IBody2D Body {
            get {
                if (_body == null) {
                    CheckPhysics();
                }

                return _body;
            }
        }

        /**
         *  @brief Center of the collider shape.
         **/
        public FPVector2 Center {
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
        public FPVector2 ScaledCenter {
			get {
                return FPVector2.Scale(center, new FPVector2(lossyScale.x, lossyScale.y));
			}
		}

        /**
         *  @brief Creates the shape related to a concrete implementation of TSCollider.
         **/
        public virtual Physics2D.Shape CreateShape() {
            return null;
        }

        public virtual Physics2D.Shape[] CreateShapes() {
            return new Physics2D.Shape[0];
        }

        private FPRigidBody2D FPRigidBody;

        [HideInInspector]
        public FPTransform2D FPTransform;

        /**
         *  @brief Returns the {@link FPRigidBody} attached.
         */
        public FPRigidBody2D attachedRigidbody {
            get {
                return FPRigidBody;
            }
        }

        /**
         *  @brief Holds an first value of the GameObject's lossy scale.
         **/
        [SerializeField]
        [HideInInspector]
        protected FPVector lossyScale = FPVector.one;

        /**
         *  @brief Creates a new {@link FPRigidBody} when there is no one attached to this GameObject.
         **/
        public void Awake() {
            FPTransform = this.GetComponent<FPTransform2D>();
            FPRigidBody = this.GetComponent<FPRigidBody2D>();

            if (lossyScale == FPVector.one) {
                lossyScale = FPVector.Abs(transform.localScale.ToFPVector());
            }
        }

        public void Update() {
            if (!Application.isPlaying) {
                lossyScale = FPVector.Abs(transform.lossyScale.ToFPVector());
            }
        }

        private void CreateBodyFixture(Physics2D.Body body, Physics2D.Shape shape) {
            Physics2D.Fixture fixture = body.CreateFixture(shape);

            if (tsMaterial != null) {
                fixture.Friction = tsMaterial.friction;
                fixture.Restitution = tsMaterial.restitution;
            }
        }

        private void CreateBody(Physics2D.World world) {
            Physics2D.Body body = Physics2D.BodyFactory.CreateBody(world);

            if (tsMaterial == null) {
                tsMaterial = GetComponent<FPMaterial>();
            }

            Physics2D.Shape shape = Shape;
            if (shape != null) {
                CreateBodyFixture(body, shape);
            } else {
                Physics2D.Shape[] shapes = CreateShapes();
                for (int index = 0, length = shapes.Length; index < length; index++) {
                    CreateBodyFixture(body, shapes[index]);
                }
            }

            if (FPRigidBody == null) {
                body.BodyType = Physics2D.BodyType.Static;
            } else {
                if (FPRigidBody.isKinematic) {
                    body.BodyType = KBEngine.Physics2D.BodyType.Kinematic;
                } else {
                    body.BodyType = Physics2D.BodyType.Dynamic;
                    body.IgnoreGravity = !FPRigidBody.useGravity;
                }

                if (FPRigidBody.mass <= 0) {
                    FPRigidBody.mass = 1;
                }

                body.FixedRotation = FPRigidBody.freezeZAxis;
                body.Mass = FPRigidBody.mass;
                body.TSLinearDrag = FPRigidBody.drag;
                body.TSAngularDrag = FPRigidBody.angularDrag;
            }

            body.IsSensor = isTrigger;
            body.CollidesWith = Physics2D.Category.All;

            _body = body;
        }

        /**
         *  @brief Initializes Shape and RigidBody and sets initial values to position and orientation based on Unity's transform.
         **/
        public void Initialize(Physics2D.World world) {
            CreateBody(world);
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

            Vector3 position = _body != null ? _body.TSPosition.ToVector() : (transform.position + ScaledCenter.ToVector());
            Quaternion rotation = _body != null ? Quaternion.Euler(0, 0, (_body.Rotation * FP.Rad2Deg).AsFloat()) : transform.rotation;

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