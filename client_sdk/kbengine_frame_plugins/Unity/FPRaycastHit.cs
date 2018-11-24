using System;

namespace KBEngine
{

    /**
    *  @brief Represents few information about a raycast hit. 
    **/
    public class FPRaycastHit
	{
		public FPRigidBody rigidbody { get; private set; }
		public FPCollider collider { get; private set; }
		public FPTransform transform { get; private set; }
		public FPVector point { get; private set; }
		public FPVector normal { get; private set; }
		public FP distance { get; private set; }

		public FPRaycastHit(FPRigidBody rigidbody, FPCollider collider, FPTransform transform, FPVector normal, FPVector origin, FPVector direction, FP fraction)
		{
			this.rigidbody = rigidbody;
			this.collider = collider;
			this.transform = transform;
			this.normal = normal;
			this.point = origin + direction * fraction;
			this.distance = fraction * direction.magnitude;
		}
	}
}

