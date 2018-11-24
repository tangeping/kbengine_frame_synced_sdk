namespace KBEngine
{

    /**
    *  @brief Represents a ray with origin and direction. 
    **/
    public class FPRay
	{
		public FPVector direction;
		public FPVector origin;

		public FPRay (FPVector origin, FPVector direction)
		{
			this.origin = origin;
			this.direction = direction;
		}

	}
}

