using UnityEngine;

namespace KBEngine {

    /**
    *  @brief Represents information about a contact point
    **/
    public class TSContactPoint2D {

        /**
        *  @brief Contact point between two bodies
        **/
        public FPVector2 point;

        /**
        *  @brief Normal vector from the contact point
        **/
        public FPVector2 normal;

    }

    /**
    *  @brief Represents information about a contact between two 2D bodies
    **/
    public class FPCollision2D {

        /**
        *  @brief An array of {@link TSContactPoint}
        **/
        public TSContactPoint2D[] contacts = new TSContactPoint2D[1];

        /**
        *  @brief {@link TSCollider} of the body hit
        **/
        public FPCollider2D collider;

        /**
        *  @brief GameObject of the body hit
        **/
        public GameObject gameObject;

        /**
        *  @brief {@link FPRigidBody} of the body hit, if there is one attached
        **/
        public FPRigidBody2D rigidbody;

        /**
        *  @brief {@link FPTransform} of the body hit
        **/
        public FPTransform2D transform;

        /**
        *  @brief The {@link FPTransform} of the body hit
        **/
        public FPVector2 relativeVelocity;

        internal void Update(GameObject otherGO, Physics2D.Contact c) {
            if (this.gameObject == null) {
                this.gameObject = otherGO;
                this.collider = this.gameObject.GetComponent<FPCollider2D>();
                this.rigidbody = this.gameObject.GetComponent<FPRigidBody2D>();
                this.transform = this.collider.FPTransform;
            }

            if (c != null) {
                if (contacts[0] == null) {
                    contacts[0] = new TSContactPoint2D();
                }

                FPVector2 normal;
                Physics2D.FixedArray2<FPVector2> points;

                c.GetWorldManifold(out normal, out points);

                contacts[0].normal = normal;
                contacts[0].point = points[0];

                this.relativeVelocity = c.CalculateRelativeVelocity();
            }
        }

    }

}