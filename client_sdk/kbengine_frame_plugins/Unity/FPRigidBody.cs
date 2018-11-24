using UnityEngine;
using UnityEngine.Serialization;
using KBEngine.Physics3D;

namespace KBEngine {

    /**
     *  @brief Represents a physical 3D rigid body.
     **/
    [RequireComponent(typeof(FPCollider))]
    [AddComponentMenu("FrameSync/Physics/RigidBody", 11)]
    public class FPRigidBody : MonoBehaviour {

        public enum InterpolateMode { None, Interpolate, Extrapolate };

        [FormerlySerializedAs("mass")]
        [SerializeField]
        private FP _mass = 1;

        /**
         *  @brief Mass of the body. 
         **/
        public FP mass {
            get {
                if (tsCollider._body != null) {
                    return tsCollider._body.Mass;
                }

                return _mass;
            }

            set {
                _mass = value;

                if (tsCollider._body != null) {
                    tsCollider._body.Mass = value;
                }
            }
        }

        [SerializeField]
        private bool _useGravity = true;

        /**
         *  @brief If true it uses gravity force. 
         **/
        public bool useGravity {
            get {
                if (tsCollider.IsBodyInitialized) {
                    return tsCollider.Body.TSAffectedByGravity;
                }

                return _useGravity;
            }

            set {
                _useGravity = value;

                if (tsCollider.IsBodyInitialized) {
                    tsCollider.Body.TSAffectedByGravity = _useGravity;
                }
            }
        }

        [SerializeField]
        private bool _isKinematic;

        /**
         *  @brief If true it doesn't get influences from external forces. 
         **/
        public bool isKinematic {
            get {
                if (tsCollider.IsBodyInitialized) {
                    return tsCollider.Body.TSIsKinematic;
                }

                return _isKinematic;
            }

            set {
                _isKinematic = value;

                if (tsCollider.IsBodyInitialized) {
                    tsCollider.Body.TSIsKinematic = _isKinematic;
                }
            }
        }

        [SerializeField]
        private FP _linearDrag;

        /**
         *  @brief Linear drag coeficient.
         **/
        public FP drag {
            get {
                if (tsCollider.IsBodyInitialized) {
                    return tsCollider.Body.TSLinearDrag;
                }

                return _linearDrag;
            }

            set {
                _linearDrag = value;

                if (tsCollider.IsBodyInitialized) {
                    tsCollider.Body.TSLinearDrag = _linearDrag;
                }
            }
        }

        [SerializeField]
        private FP _angularDrag = 0.05f;

        /**
         *  @brief Angular drag coeficient.
         **/
        public FP angularDrag {
            get {
                if (tsCollider.IsBodyInitialized) {
                    return tsCollider.Body.TSAngularDrag;
                }

                return _angularDrag;
            }

            set {
                _angularDrag = value;

                if (tsCollider.IsBodyInitialized) {
                    tsCollider.Body.TSAngularDrag = _angularDrag;
                }
            }
        }

        /**
         *  @brief Interpolation mode that should be used. 
         **/
        public InterpolateMode interpolation;

        [SerializeField]
        [HideInInspector]
        private FPRigidBodyConstraints _constraints = FPRigidBodyConstraints.None;

        /**
         *  @brief Freeze constraints applied to this body. 
         **/
        public FPRigidBodyConstraints constraints {
            get {
                if (tsCollider.IsBodyInitialized) {
                    return tsCollider._body.FreezeConstraints;
                }

                return _constraints;
            }

            set {
                _constraints = value;

                if (tsCollider.IsBodyInitialized) {
                    tsCollider._body.FreezeConstraints = value;
                }
            }
        }

        private FPCollider _tsCollider;

        /**
         *  @brief Returns the {@link TSCollider} attached.
         */
        public FPCollider tsCollider {
            get {
                if (_tsCollider == null) {
                    _tsCollider = GetComponent<FPCollider>();
                }

                return _tsCollider;
            }
        }

        private FPTransform _FPTransform;

        /**
         *  @brief Returns the {@link FPTransform} attached.
         */
        public FPTransform FPTransform {
            get {
                if (_FPTransform == null) {
                    _FPTransform = GetComponent<FPTransform>();
                }

                return _FPTransform;
            }
        }

        /**
         *  @brief Applies the provided force in the body. 
         *  
         *  @param force A {@link FPVector} representing the force to be applied.
         **/
        public void AddForce(FPVector force) {
            AddForce(force, ForceMode.Force);
        }

        /**
         *  @brief Applies the provided force in the body. 
         *  
         *  @param force A {@link FPVector} representing the force to be applied.
         *  @param mode Indicates how the force should be applied.
         **/
        public void AddForce(FPVector force, ForceMode mode) {
            if (mode == ForceMode.Force) {
                tsCollider.Body.TSApplyForce(force);
            } else if (mode == ForceMode.Impulse) {
                tsCollider.Body.TSApplyImpulse(force);
            }
        }

        /**
         *  @brief Applies the provided force in the body. 
         *  
         *  @param force A {@link FPVector} representing the force to be applied.
         *  @param position Indicates the location where the force should hit.
         **/
        public void AddForceAtPosition(FPVector force, FPVector position) {
            AddForceAtPosition(force, position, ForceMode.Impulse);
        }

        /**
         *  @brief Applies the provided force in the body. 
         *  
         *  @param force A {@link FPVector} representing the force to be applied.
         *  @param position Indicates the location where the force should hit.
         **/
        public void AddForceAtPosition(FPVector force, FPVector position, ForceMode mode) {
            if (mode == ForceMode.Force) {
                tsCollider.Body.TSApplyForce(force, position);
            } else if (mode == ForceMode.Impulse) {
                tsCollider.Body.TSApplyImpulse(force, position);
            }
        }

        /**
         *  @brief Returns the velocity of the body at some position in world space. 
         **/
        public FPVector GetPointVelocity(FPVector worldPoint) {
            FPVector directionPoint = position - tsCollider.Body.TSPosition;
            return FPVector.Cross(tsCollider.Body.TSAngularVelocity, directionPoint) + tsCollider.Body.TSLinearVelocity;
        }

        /**
         *  @brief Simulates the provided tourque in the body. 
         *  
         *  @param torque A {@link FPVector} representing the torque to be applied.
         **/
        public void AddTorque(FPVector torque) {
            tsCollider.Body.TSApplyTorque(torque);
        }


        /**
         *  @brief Changes orientation to look at target position. 
         *  
         *  @param target A {@link FPVector} representing the position to look at.
         **/
        public void LookAt(FPVector target) {
            rotation = FPQuaternion.CreateFromMatrix(FPMatrix.CreateFromLookAt(position, target));
        }

        /**
         *  @brief Moves the body to a new position. 
         **/
        public void MovePosition(FPVector position) {
            this.position = position;
        }

        /**
         *  @brief Rotates the body to a provided rotation. 
         **/
        public void MoveRotation(FPQuaternion rot) {
            this.rotation = rot;
        }

        /**
        *  @brief Position of the body. 
        **/
        public FPVector position {
            get {
                return FPTransform.position;
            }

            set {
                FPTransform.position = value;
            }
        }

        /**
        *  @brief Orientation of the body. 
        **/
        public FPQuaternion rotation {
            get {
                return FPTransform.rotation;
            }

            set {
                FPTransform.rotation = value;
            }
        }

        /**
        *  @brief LinearVelocity of the body. 
        **/
        public FPVector velocity {
            get {
                return tsCollider.Body.TSLinearVelocity;
            }

            set {
                tsCollider.Body.TSLinearVelocity = value;
            }
        }

        /**
        *  @brief AngularVelocity of the body. 
        **/
        public FPVector angularVelocity {
            get {
                return tsCollider.Body.TSAngularVelocity;
            }

            set {
                tsCollider.Body.TSAngularVelocity = value;
            }
        }

    }

}