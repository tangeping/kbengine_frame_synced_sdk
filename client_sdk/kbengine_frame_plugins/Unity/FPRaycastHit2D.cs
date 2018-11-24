namespace KBEngine
{

    /**
    *  @brief Information about a 2D cast hit.
    **/
    public class FPRaycastHit2D {

        public FPCollider2D collider;

        public FPRigidBody2D rigidbody;

        public FPTransform2D transform;

        public FPRaycastHit2D(FPCollider2D collider) {
            this.collider = collider;
            this.rigidbody = collider.GetComponent<FPRigidBody2D>();
            this.transform = collider.GetComponent<FPTransform2D>();
        }

    }

}