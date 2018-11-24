namespace KBEngine.Physics3D {

	public class Constraint2D : Constraint {

        private bool freezeZAxis;

		public Constraint2D(RigidBody body, bool freezeZAxis) : base(body, null) {
            this.freezeZAxis = freezeZAxis;

        }			

		public override void PostStep() {
			FPVector pos = Body1.Position;
			pos.z = 0;
			Body1.Position = pos;

			FPQuaternion q = FPQuaternion.CreateFromMatrix(Body1.Orientation);
			q.Normalize();
			q.x = 0;
			q.y = 0;

			if (freezeZAxis) {
				q.z = 0;
			}

			Body1.Orientation = FPMatrix.CreateFromQuaternion(q);

			if (Body1.isStatic) {
				return;
			}

			FPVector vel = Body1.LinearVelocity;
			vel.z = 0;
			Body1.LinearVelocity = vel;

            FPVector av = Body1.AngularVelocity;
			av.x = 0;
			av.y = 0;

            if (freezeZAxis) {
                av.z = 0;
            }

			Body1.AngularVelocity = av;
		}

	}

}