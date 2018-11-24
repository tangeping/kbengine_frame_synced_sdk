namespace KBEngine.Physics3D {

	public class ConstraintHierarchy : Constraint
	{

		private RigidBody parent;

		private RigidBody child;

		private FPVector childOffset;

		public ConstraintHierarchy(IBody parent, IBody child, FPVector childOffset) : base((RigidBody) parent, (RigidBody) child) {
			this.parent = (RigidBody) parent;
			this.child = (RigidBody) child;

			this.childOffset = childOffset;
		}

		public override void PostStep() {
			child.Position = childOffset + parent.Position;
		}

	}

}