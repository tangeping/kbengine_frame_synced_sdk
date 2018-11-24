namespace KBEngine {

    /**
    * @brief Represents an interface to 3D bodies.
    **/
    public interface IBody3D : IBody {

        /**
        * @brief If true the body doesn't move around by collisions.
        **/
        bool TSIsStatic {
            get; set;
        }

        /**
        * @brief Linear drag coeficient.
        **/
        FP TSLinearDrag {
            get; set;
        }

        /**
        * @brief Angular drag coeficient.
        **/
        FP TSAngularDrag {
            get; set;
        }

        /**
         *  @brief Static friction when in contact. 
         **/
        FP TSFriction {
            get; set;
        }

        /**
        * @brief Coeficient of restitution.
        **/
        FP TSRestitution {
            get; set;
        }

        /**
        * @brief Set/get body's position.
        **/
        FPVector TSPosition {
            get; set;
        }

        /**
        * @brief Set/get body's orientation.
        **/
        FPMatrix TSOrientation {
            get; set;
        }

        /**
        * @brief If true the body is affected by gravity.
        **/
        bool TSAffectedByGravity {
            get; set;
        }

        /**
        * @brief If true the body is managed as kinematic.
        **/
        bool TSIsKinematic {
            get; set;
        }

        /**
        * @brief Set/get body's linear velocity.
        **/
        FPVector TSLinearVelocity {
            get; set;
        }

        /**
        * @brief Set/get body's angular velocity.
        **/
        FPVector TSAngularVelocity {
            get; set;
        }

        /**
        * @brief Applies a force to the body's center.
        **/
        void TSApplyForce(FPVector force);

        /**
        * @brief Applies a force to the body at a specific position.
        **/
        void TSApplyForce(FPVector force, FPVector position);

        /**
        * @brief Applies a impulse to the body's center.
        **/
        void TSApplyImpulse(FPVector force);

        /**
        * @brief Applies a impulse to the body at a specific position.
        **/
        void TSApplyImpulse(FPVector force, FPVector position);

        /**
        * @brief Applies a torque force to the body.
        **/
        void TSApplyTorque(FPVector force);

    }

}