using UnityEngine;
using KBEngine.Physics3D;

namespace KBEngine {

    /**
    *  @brief Represents information about a contact point
    **/
    public class TSContactPoint {

        /**
        *  @brief Contact point between two bodies
        **/
        public FPVector point;

        /**
        *  @brief Normal vector from the contact point
        **/
        public FPVector normal;

    }

    /**
    *  @brief Represents information about a contact between two 3D bodies
    **/
    public class FPCollision {

        /**
        *  @brief An array of {@link TSContactPoint}
        **/
        public TSContactPoint[] contacts = new TSContactPoint[1];

        /**
        *  @brief {@link TSCollider} of the body hit
        **/
        public FPCollider collider;

        /**
        *  @brief GameObject of the body hit
        **/
        public GameObject gameObject;

        /**
        *  @brief {@link FPRigidBody} of the body hit, if there is one attached
        **/
        public FPRigidBody rigidbody;

        /**
        *  @brief {@link FPTransform} of the body hit
        **/
        public FPTransform transform;

        /**
        *  @brief The {@link FPTransform} of the body hit
        **/
        public FPVector relativeVelocity;

        internal void Update(GameObject otherGO, Contact c) {
            if (this.gameObject == null) {
                this.gameObject = otherGO;
                this.collider = this.gameObject.GetComponent<FPCollider>();
                this.rigidbody = this.gameObject.GetComponent<FPRigidBody>();
                this.transform = this.collider.FPTransform;
            }

            if (c != null) {
                if (contacts[0] == null) {
                    contacts[0] = new TSContactPoint();
                }

                this.relativeVelocity = c.CalculateRelativeVelocity();

                contacts[0].normal = c.Normal;
                contacts[0].point = c.p1;
            }
        }

    }

}