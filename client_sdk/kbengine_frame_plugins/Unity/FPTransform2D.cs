using UnityEngine;

namespace KBEngine {

    /**
    *  @brief A deterministic version of Unity's Transform component for 2D physics. 
    **/
    [ExecuteInEditMode]
    public class FPTransform2D : MonoBehaviour {

        private const float DELTA_TIME_FACTOR = 10f;

        [SerializeField]
        [HideInInspector]
        [AddTracking]
        private FPVector2 _position;

        /**
        *  @brief Property access to position. 
        *  
        *  It works as proxy to a Body's position when there is a collider attached.
        **/
        public FPVector2 position {
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
        private FP _rotation;

        /**
        *  @brief Property access to rotation. 
        *  
        *  It works as proxy to a Body's rotation when there is a collider attached.
        **/
        public FP rotation {
            get {
                if (tsCollider != null && tsCollider.Body != null) {
                    return tsCollider.Body.TSOrientation * FP.Rad2Deg;
                }

                return _rotation;
            }
            set {
                _rotation = value;

                if (tsCollider != null && tsCollider.Body != null) {
                    tsCollider.Body.TSOrientation = _rotation * FP.Deg2Rad;
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

        private FPVector2 scaledCenter {
            get {
                if (tsCollider != null) {
                    return tsCollider.ScaledCenter;
                }

                return FPVector2.zero;
            }
        }

        [HideInInspector]
        public FPCollider2D tsCollider;

        [HideInInspector]
        public FPTransform2D tsParent;

        private bool initialized = false;

		private FPRigidBody2D rb;

        public void Start() {
            if (!Application.isPlaying) {
                return;
            }

            Initialize();
			rb = GetComponent<FPRigidBody2D> ();
        }

        /**
        *  @brief Initializes internal properties based on whether there is a {@link TSCollider2D} attached.
        **/
        public void Initialize() {
            if (initialized) {
                return;
            }

            tsCollider = GetComponent<FPCollider2D>();
            if (transform.parent != null) {
                tsParent = transform.parent.GetComponent<FPTransform2D>();
            }

            if (!_serialized) {
                UpdateEditMode();
            }

            if (tsCollider != null) {
                if (tsCollider.IsBodyInitialized) {
                    tsCollider.Body.TSPosition = _position + scaledCenter;
                    tsCollider.Body.TSOrientation = _rotation * FP.Deg2Rad;
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
                _position = transform.position.ToFPVector2();
                _rotation = transform.rotation.eulerAngles.z;
                _scale = transform.localScale.ToFPVector();

                _serialized = true;
            }
        }

        private void UpdatePlayMode() {
			if (rb != null) {
                if (rb.interpolation == FPRigidBody2D.InterpolateMode.Interpolate) {
                    transform.position = Vector3.Lerp(transform.position, position.ToVector(), Time.deltaTime * DELTA_TIME_FACTOR);
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, rotation.AsFloat()), Time.deltaTime * DELTA_TIME_FACTOR);
                    transform.localScale = Vector3.Lerp(transform.localScale, scale.ToVector(), Time.deltaTime * DELTA_TIME_FACTOR);
                    return;
                } else if (rb.interpolation == FPRigidBody2D.InterpolateMode.Extrapolate) {
                    transform.position = (position + tsCollider.Body.TSLinearVelocity * Time.deltaTime * DELTA_TIME_FACTOR).ToVector();
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, rotation.AsFloat()), Time.deltaTime * DELTA_TIME_FACTOR);
                    transform.localScale = Vector3.Lerp(transform.localScale, scale.ToVector(), Time.deltaTime * DELTA_TIME_FACTOR);
                    return;
                }
			}

            transform.position = position.ToVector();

            Quaternion rot = transform.rotation;
            rot.eulerAngles = new Vector3(0, 0, rotation.AsFloat());
            transform.rotation = rot;

            transform.localScale = scale.ToVector();
        }

    }

}