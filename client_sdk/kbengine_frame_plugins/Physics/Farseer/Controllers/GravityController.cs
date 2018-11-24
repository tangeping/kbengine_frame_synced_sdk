using System.Collections.Generic;

namespace KBEngine.Physics2D
{
    public enum GravityType
    {
        Linear,
        DistanceSquared
    }

    public class GravityController : Controller
    {
        public GravityController(FP strength)
            : base(ControllerType.GravityController)
        {
            Strength = strength;
            MaxRadius = FP.MaxValue;
            GravityType = GravityType.DistanceSquared;
            Points = new List<FPVector2>();
            Bodies = new List<Body>();
        }

        public GravityController(FP strength, FP maxRadius, FP minRadius)
            : base(ControllerType.GravityController)
        {
            MinRadius = minRadius;
            MaxRadius = maxRadius;
            Strength = strength;
            GravityType = GravityType.DistanceSquared;
            Points = new List<FPVector2>();
            Bodies = new List<Body>();
        }

        public FP MinRadius { get; set; }
        public FP MaxRadius { get; set; }
        public FP Strength { get; set; }
        public GravityType GravityType { get; set; }
        public List<Body> Bodies { get; set; }
        public List<FPVector2> Points { get; set; }

        public override void Update(FP dt)
        {
            FPVector2 f = FPVector2.zero;

            foreach (Body worldBody in World.BodyList)
            {
                if (!IsActiveOn(worldBody))
                    continue;

                foreach (Body controllerBody in Bodies)
                {
                    if (worldBody == controllerBody || (worldBody.IsStatic && controllerBody.IsStatic) || !controllerBody.Enabled)
                        continue;

                    FPVector2 d = controllerBody.Position - worldBody.Position;
                    FP r2 = d.LengthSquared();

                    if (r2 <= Settings.Epsilon || r2 > MaxRadius * MaxRadius || r2 < MinRadius * MinRadius)
                        continue;

                    switch (GravityType)
                    {
                        case GravityType.DistanceSquared:
                            f = Strength / r2 * worldBody.Mass * controllerBody.Mass * d;
                            break;
                        case GravityType.Linear:
                            f = Strength / FP.Sqrt(r2) * worldBody.Mass * controllerBody.Mass * d;
                            break;
                    }

                    worldBody.ApplyForce(ref f);
                }

                foreach (FPVector2 point in Points)
                {
                    FPVector2 d = point - worldBody.Position;
                    FP r2 = d.LengthSquared();

                    if (r2 <= Settings.Epsilon || r2 > MaxRadius * MaxRadius || r2 < MinRadius * MinRadius)
                        continue;

                    switch (GravityType)
                    {
                        case GravityType.DistanceSquared:
                            f = Strength / r2 * worldBody.Mass * d;
                            break;
                        case GravityType.Linear:
                            f = Strength / FP.Sqrt(r2) * worldBody.Mass * d;
                            break;
                    }

                    worldBody.ApplyForce(ref f);
                }
            }
        }

        public void AddBody(Body body)
        {
            Bodies.Add(body);
        }

        public void AddPoint(FPVector2 point)
        {
            Points.Add(point);
        }
    }
}