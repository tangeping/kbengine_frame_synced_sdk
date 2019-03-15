using UnityEngine;

namespace KBEngine {

    /**
    *  @brief A deterministic version of Unity's Transform component for 3D physics. 
    **/
    [ExecuteInEditMode]
    public class FPTransform : MonoBehaviour {

        private const float DELTA_TIME_FACTOR = 10f;

        [SerializeField]
        [HideInInspector]
        [AddTracking]
        private FPVector _position;

        /**
        *  @brief Property access to position. 
        *  
        *  It works as proxy to a Body's position when there is a collider attached.
        **/
        public FPVector position {
            get {
                if (tsCollider != null && tsCollider.Body != null) {
					return tsCollider.Body.TSPosition - scaledCenter;
                }

				return _position;
            }
            set {
                _position = value;

                if (tsCollider != null && tsCollider.Body != null) {
                    tsCollider.Body.TSPosition = _position + scaledCenter;
                }
            }
        }

        [SerializeField]
        [HideInInspector]
        [AddTracking]
        private FPQuaternion _rotation;

        /**
        *  @brief Property access to rotation. 
        *  
        *  It works as proxy to a Body's rotation when there is a collider attached.
        **/
        public FPQuaternion rotation {
            get {
                if (tsCollider != null && tsCollider.Body != null) {
                    return FPQuaternion.CreateFromMatrix(tsCollider.Body.TSOrientation);
                }

                return _rotation;
            }
            set {
                _rotation = value;

                if (tsCollider != null && tsCollider.Body != null) {
                    tsCollider.Body.TSOrientation = FPMatrix.CreateFromQuaternion(_rotation);
                }
            }
        }

        [SerializeField]
        [HideInInspector]
        [AddTracking]
        private FPVector _scale;

        /**
        *  @brief Property access to scale. 
        **/
        public FPVector scale {
            get {
                return _scale;
            }
            set {
                _scale = value;
            }
        }

        [SerializeField]
        [HideInInspector]
        private bool _serialized;

        private FPVector scaledCenter {
            get {
                if (tsCollider != null) {
                    return tsCollider.ScaledCenter;
                }

                return FPVector.zero;
            }
        }

        /**
        *  @brief Rotates game object to point forward vector to a target position. 
        *  
        *  @param other TSTrasform used to get target position.
        **/
        public void LookAt(FPTransform other) {
            LookAt(other.position);
        }

        /**
        *  @brief Rotates game object to point forward vector to a target position. 
        *  
        *  @param target Target position.
        **/
        public void LookAt(FPVector target) {
            this.rotation = FPQuaternion.CreateFromMatrix(FPMatrix.CreateFromLookAt(position, target));
        }

        /**
        *  @brief Moves game object based on provided axis values. 
        **/
        public void Translate(FP x, FP y, FP z) {
            Translate(x, y, z, Space.Self);
        }

        /**
        *  @brief Moves game object based on provided axis values and a relative space.
        *  
        *  If relative space is SELF then the game object will move based on its forward vector.
        **/
        public void Translate(FP x, FP y, FP z, Space relativeTo) {
            Translate(new FPVector(x, y, z), relativeTo);
        }

        /**
        *  @brief Moves game object based on provided axis values and a relative {@link FPTransform}.
        *  
        *  The game object will move based on FPTransform's forward vector.
        **/
        public void Translate(FP x, FP y, FP z, FPTransform relativeTo) {
            Translate(new FPVector(x, y, z), relativeTo);
        }

        /**
        *  @brief Moves game object based on provided translation vector.
        **/
        public void Translate(FPVector translation) {
            Translate(translation, Space.Self);
        }

        /**
        *  @brief Moves game object based on provided translation vector and a relative space.
        *  
        *  If relative space is SELF then the game object will move based on its forward vector.
        **/
        public void Translate(FPVector translation, Space relativeTo) {
            if (relativeTo == Space.Self) {
                Translate(translation, this);
            } else {
                this.position += translation;
            }
        }

        /**
        *  @brief Moves game object based on provided translation vector and a relative {@link FPTransform}.
        *  
        *  The game object will move based on FPTransform's forward vector.
        **/
        public void Translate(FPVector translation, FPTransform relativeTo) {
            this.position += FPVector.Transform(translation, FPMatrix.CreateFromQuaternion(relativeTo.rotation));
        }

        /**
        *  @brief Rotates game object based on provided axis, point and angle of rotation.
        **/
        public void RotateAround(FPVector point, FPVector axis, FP angle) {
            FPVector vector = this.position;
            FPVector vector2 = vector - point;
            vector2 = FPVector.Transform(vector2, FPMatrix.AngleAxis(angle * FP.Deg2Rad, axis));
            vector = point + vector2;
            this.position = vector;

            Rotate(axis, angle);
        }

        /**
        *  @brief Rotates game object based on provided axis and angle of rotation.
        **/
        public void RotateAround(FPVector axis, FP angle) {
            Rotate(axis, angle);
        }

        /**
        *  @brief Rotates game object based on provided axis angles of rotation.
        **/
        public void Rotate(FP xAngle, FP yAngle, FP zAngle) {
            Rotate(new FPVector(xAngle, yAngle, zAngle), Space.Self);
        }

        /**
        *  @brief Rotates game object based on provided axis angles of rotation and a relative space.
        *  
        *  If relative space is SELF then the game object will rotate based on its forward vector.
        **/
        public void Rotate(FP xAngle, FP yAngle, FP zAngle, Space relativeTo) {
            Rotate(new FPVector(xAngle, yAngle, zAngle), relativeTo);
        }

        /**
        *  @brief Rotates game object based on provided axis angles of rotation.
        **/
        public void Rotate(FPVector eulerAngles) {
            Rotate(eulerAngles, Space.Self);
        }

        /**
        *  @brief Rotates game object based on provided axis and angle of rotation.
        **/
        public void Rotate(FPVector axis, FP angle) {
            Rotate(axis, angle, Space.Self);
        }

        /**
        *  @brief Rotates game object based on provided axis, angle of rotation and relative space.
        *  
        *  If relative space is SELF then the game object will rotate based on its forward vector.
        **/
        public void Rotate(FPVector axis, FP angle, Space relativeTo) {
            FPQuaternion result = FPQuaternion.identity;

            if (relativeTo == Space.Self) {
                result = this.rotation * FPQuaternion.AngleAxis(angle, axis);
            } else {
                result = FPQuaternion.AngleAxis(angle, axis) * this.rotation;
            }

            result.Normalize();
            this.rotation = result;
        }

        /**
        *  @brief Rotates game object based on provided axis angles and relative space.
        *  
        *  If relative space is SELF then the game object will rotate based on its forward vector.
        **/
        public void Rotate(FPVector eulerAngles, Space relativeTo) {
            FPQuaternion result = FPQuaternion.identity;

            if (relativeTo == Space.Self) {
                result = this.rotation * FPQuaternion.Euler(eulerAngles);
            } else {
                result = FPQuaternion.Euler(eulerAngles) * this.rotation;
            }

            result.Normalize();
            this.rotation = result;
        }

        /**
        *  @brief Current self forward vector.
        **/
        public FPVector forward {
            get {
                return FPVector.Transform(FPVector.forward, FPMatrix.CreateFromQuaternion(rotation));
            }
        }

        /**
        *  @brief Current self right vector.
        **/
        public FPVector right {
            get {
                return FPVector.Transform(FPVector.right, FPMatrix.CreateFromQuaternion(rotation));
            }
        }

        /**
        *  @brief Current self up vector.
        **/
        public FPVector up {
            get {
                return FPVector.Transform(FPVector.up, FPMatrix.CreateFromQuaternion(rotation));
            }
        }

        /**
        *  @brief Returns Euler angles in degrees.
        **/
        public FPVector eulerAngles {
            get {
                return rotation.eulerAngles;
            }
        }

        [HideInInspector]
        public FPCollider tsCollider;

        [HideInInspector]
        public FPTransform tsParent;

        private bool initialized = false;

		private FPRigidBody rb;

        public void Start() {
            if (!Application.isPlaying) {
                return;
            }

            Initialize();
			rb = GetComponent<FPRigidBody> ();
        }

        /**
        *  @brief Initializes internal properties based on whether there is a {@link TSCollider} attached.
        **/
        public void Initialize() {
            if (initialized) {
                return;
            }

            tsCollider = GetComponent<FPCollider>();
            if (transform.parent != null) {
                tsParent = transform.parent.GetComponent<FPTransform>();
            }

            if (!_serialized) {
                UpdateEditMode();
            }

            if (tsCollider != null) {
                if (tsCollider.IsBodyInitialized) {
                    tsCollider.Body.TSPosition = _position + scaledCenter;
                    tsCollider.Body.TSOrientation = FPMatrix.CreateFromQuaternion(_rotation);
                }
            } else {
                StateTracker.AddTracking(this);
            }

            initialized = true;
        }

        public void Update() {
            if (Application.isPlaying) {
                if (initialized) {
                    UpdatePlayMode();
                }
            } else {
                UpdateEditMode();
            }
        }

        private void UpdateEditMode() {
            if (transform.hasChanged) {
                _position = transform.position.ToFPVector();
                _rotation = transform.rotation.ToFPQuaternion();
                _scale = transform.localScale.ToFPVector();

                _serialized = true;
            }
        }

        /*private float duration = 0.0f;*/
        private void UpdatePlayMode() {
			if (rb != null) {
                if (rb.interpolation == FPRigidBody.InterpolateMode.Interpolate) {
                    transform.position = Vector3.Lerp(transform.position, position.ToVector(), Time.deltaTime * DELTA_TIME_FACTOR);
                    transform.rotation = Quaternion.Lerp(transform.rotation, rotation.ToQuaternion(), Time.deltaTime * DELTA_TIME_FACTOR);
                    transform.localScale = Vector3.Lerp(transform.localScale, scale.ToVector(), Time.deltaTime * DELTA_TIME_FACTOR);
                    return;
                } else if (rb.interpolation == FPRigidBody.InterpolateMode.Extrapolate) {
                    transform.position = (position + rb.tsCollider.Body.TSLinearVelocity * Time.deltaTime * DELTA_TIME_FACTOR).ToVector();
                    transform.rotation = Quaternion.Lerp(transform.rotation, rotation.ToQuaternion(), Time.deltaTime * DELTA_TIME_FACTOR);
                    transform.localScale = Vector3.Lerp(transform.localScale, scale.ToVector(), Time.deltaTime * DELTA_TIME_FACTOR);
                    return;
                }
			}

//             if(duration > FrameSyncManager.DeltaTime)
//             {
//                 duration = 0;
//             }
//             else
//             {
//                 duration += Time.deltaTime;
//             }
//             
//             float factor = duration / FrameSyncManager.DeltaTime.AsFloat();
//             transform.position = Vector3.Lerp(transform.position, position.ToVector(), factor);
            transform.position = position.ToVector();
            transform.rotation = rotation.ToQuaternion();
            transform.localScale = scale.ToVector();
        }

    }

}