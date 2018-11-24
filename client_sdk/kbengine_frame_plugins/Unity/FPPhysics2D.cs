using System;

namespace KBEngine
{

    /**
    *  @brief Helpers for 2D physics.
    **/
    public class FPPhysics2D {

        public enum TSCapsuleDirection2D {

            VERTICAL,

            HORIZONTAL

        }

        private static FP POINT_RADIUS = 0.001f;

        private static object OverlapGeneric(Physics2D.Shape shape, FPVector2 position, Physics2D.BodySpecialSensor sensorType, int layerMask) {
            Physics2D.World world = (Physics2D.World)Physics2DWorldManager.instance.GetWorld();

            Physics2D.Body body = Physics2D.BodyFactory.CreateBody(world);
            body.CreateFixture(shape);

            body.BodyType = Physics2D.BodyType.Static;
            body.IsSensor = true;
            body.CollidesWith = Physics2D.Category.All;

            body.SpecialSensor = sensorType;
            body.SpecialSensorMask = layerMask;
            body.Position = position;

            world.RemoveBody(body);
            world.ProcessRemovedBodies();

            if (body._specialSensorResults.Count > 0) {
                if (sensorType == Physics2D.BodySpecialSensor.ActiveOnce) {
                    return Physics2DWorldManager.instance.GetGameObject(body._specialSensorResults[0]).GetComponent<FPCollider2D>();
                } else {
                    FPCollider2D[] result = new FPCollider2D[body._specialSensorResults.Count];
                    for (int i = 0; i < body._specialSensorResults.Count; i++) {
                        result[i] = Physics2DWorldManager.instance.GetGameObject(body._specialSensorResults[i]).GetComponent<FPCollider2D>();
                    }

                    return result;
                }

            }

            return null;
        }

        private static object _OverlapCircle(FPVector2 point, FP radius, Physics2D.BodySpecialSensor sensorType, int layerMask = UnityEngine.Physics.DefaultRaycastLayers) {
            return OverlapGeneric(new Physics2D.CircleShape(radius, 1), point, sensorType, layerMask);
        }

        /**
        *  @brief Returns the first {@link TSCollider2D} within a circular area. Returns null if there is none.
        *  
        *  @param point Center of the circle in world space.
        *  @param radius Radius of the circle.
        *  @param layerMask Unity's layer mask to filter objects.
        **/
        public static FPCollider2D OverlapCircle(FPVector2 point, FP radius, int layerMask = UnityEngine.Physics.DefaultRaycastLayers) {
            return (FPCollider2D) _OverlapCircle(point, radius, Physics2D.BodySpecialSensor.ActiveOnce, layerMask);
        }

        /**
        *  @brief Returns all {@link TSCollider2D} within a circular area. Returns null if there is none.
        *  
        *  @param point Center of the circle in world space.
        *  @param radius Radius of the circle.
        *  @param layerMask Unity's layer mask to filter objects.
        **/
        public static FPCollider2D[] OverlapCircleAll(FPVector2 point, FP radius, int layerMask = UnityEngine.Physics.DefaultRaycastLayers) {
            return (FPCollider2D[]) _OverlapCircle(point, radius, Physics2D.BodySpecialSensor.ActiveAll, layerMask);
        }

        /**
        *  @brief Returns the first {@link TSCollider2D} within a rectangular area. Returns null if there is none.
        *  
        *  @param pointA Top-left corner of the rectangle.
        *  @param radius Bottom-right corner of the rectangle.
        *  @param layerMask Unity's layer mask to filter objects.
        **/
        public static object _OverlapArea(FPVector2 pointA, FPVector2 pointB, Physics2D.BodySpecialSensor sensorType, int layerMask = UnityEngine.Physics.DefaultRaycastLayers) {
            FPVector2 center;
            center.x = (pointA.x + pointB.x) * FP.Half;
            center.y = (pointA.y + pointB.y) * FP.Half;

            Physics2D.Vertices vertices = new Physics2D.Vertices(4);
            vertices.Add(new FPVector2(pointA.x, pointA.y) - center);
            vertices.Add(new FPVector2(pointB.x, pointA.y) - center);
            vertices.Add(new FPVector2(pointB.x, pointB.y) - center);
            vertices.Add(new FPVector2(pointA.x, pointB.y) - center);

            return OverlapGeneric(new Physics2D.PolygonShape(vertices, 1), center, sensorType, layerMask);
        }

        /**
        *  @brief Returns the first {@link TSCollider2D} within a rectangular area. Returns null if there is none.
        *  
        *  @param pointA Top-left corner of the rectangle.
        *  @param radius Bottom-right corner of the rectangle.
        *  @param layerMask Unity's layer mask to filter objects.
        **/
        public static FPCollider2D OverlapArea(FPVector2 pointA, FPVector2 pointB, int layerMask = UnityEngine.Physics.DefaultRaycastLayers) {
            return (FPCollider2D) _OverlapArea(pointA, pointB, Physics2D.BodySpecialSensor.ActiveOnce, layerMask);
        }

        /**
        *  @brief Returns all {@link TSCollider2D} within a rectangular area. Returns null if there is none.
        *  
        *  @param pointA Top-left corner of the rectangle.
        *  @param radius Bottom-right corner of the rectangle.
        *  @param layerMask Unity's layer mask to filter objects.
        **/
        public static FPCollider2D[] OverlapAreaAll(FPVector2 pointA, FPVector2 pointB, int layerMask = UnityEngine.Physics.DefaultRaycastLayers) {
            return (FPCollider2D[]) _OverlapArea(pointA, pointB, Physics2D.BodySpecialSensor.ActiveAll, layerMask);
        }

        /**
        *  @brief Returns the first {@link TSCollider2D} within a small circular area. Returns null if there is none.
        *  
        *  @param point Center of the point in world space.
        *  @param layerMask Unity's layer mask to filter objects.
        **/
        public static FPCollider2D OverlapPoint(FPVector2 point, int layerMask = UnityEngine.Physics.DefaultRaycastLayers) {
            return (FPCollider2D)_OverlapCircle(point, POINT_RADIUS, Physics2D.BodySpecialSensor.ActiveOnce, layerMask);
        }

        /**
        *  @brief Returns all {@link TSCollider2D} within a small circular area. Returns null if there is none.
        *  
        *  @param point Center of the point in world space.
        *  @param layerMask Unity's layer mask to filter objects.
        **/
        public static FPCollider2D[] OverlapPointAll(FPVector2 point, int layerMask = UnityEngine.Physics.DefaultRaycastLayers) {
            return (FPCollider2D[])_OverlapCircle(point, POINT_RADIUS, Physics2D.BodySpecialSensor.ActiveAll, layerMask);
        }

        private static object _OverlapBox(FPVector2 point, FPVector2 size, FP angle, Physics2D.BodySpecialSensor sensorType, int layerMask = UnityEngine.Physics.DefaultRaycastLayers) {
            size *= FP.Half;
            angle *= FP.Deg2Rad;

            return OverlapGeneric(new Physics2D.PolygonShape(Physics2D.PolygonTools.CreateRectangle(size.x, size.y, point, angle * -1), 1), point, sensorType, layerMask);
        }

        /**
        *  @brief Returns the first {@link TSCollider2D} within a box area. Returns null if there is none.
        *  
        *  @param point Center of the box in world space.
        *  @param size Size of the box.
        *  @param angle Rotation angle in degrees of the box.
        *  @param layerMask Unity's layer mask to filter objects.
        **/
        public static FPCollider2D OverlapBox(FPVector2 point, FPVector2 size, FP angle, int layerMask = UnityEngine.Physics.DefaultRaycastLayers) {
            return (FPCollider2D) _OverlapBox(point, size, angle, Physics2D.BodySpecialSensor.ActiveOnce, layerMask);
        }

        /**
        *  @brief Returns all {@link TSCollider2D} within a box area. Returns null if there is none.
        *  
        *  @param point Center of the box in world space.
        *  @param size Size of the box.
        *  @param angle Rotation angle in degrees of the box.
        *  @param layerMask Unity's layer mask to filter objects.
        **/
        public static FPCollider2D[] OverlapBoxAll(FPVector2 point, FPVector2 size, FP angle, int layerMask = UnityEngine.Physics.DefaultRaycastLayers) {
            return (FPCollider2D[]) _OverlapBox(point, size, angle, Physics2D.BodySpecialSensor.ActiveAll, layerMask);
        }

        private static object _OverlapCapsule(FPVector2 point, FPVector2 size, TSCapsuleDirection2D direction, FP angle, Physics2D.BodySpecialSensor sensorType, int layerMask = UnityEngine.Physics.DefaultRaycastLayers) {
            if (direction == TSCapsuleDirection2D.HORIZONTAL) {
                FP aux = size.y;
                size.y = size.x;
                size.x = aux;

                angle += 90;
            }

            FP radius = size.x * FP.Half;
            Physics2D.Vertices capVerts = Physics2D.PolygonTools.CreateCapsule(size.y, radius, 8, radius, 8);

            Physics2D.PolygonTools.TransformVertices(capVerts, point, angle * FP.Deg2Rad * -1);

            return OverlapGeneric(new Physics2D.PolygonShape(capVerts, 1), point, sensorType, layerMask);
        }

        /**
        *  @brief Returns the first {@link TSCollider2D} within a capsule area. Returns null if there is none.
        *  
        *  @param point Center of the capsule in world space.
        *  @param size Size of the capsule.
        *  @param direction If it is a vertical or horizontal capsule.
        *  @param angle Rotation angle in degrees of the capsule.
        *  @param layerMask Unity's layer mask to filter objects.
        **/
        public static FPCollider2D OverlapCapsule(FPVector2 point, FPVector2 size, TSCapsuleDirection2D direction, FP angle, int layerMask = UnityEngine.Physics.DefaultRaycastLayers) {
            return (FPCollider2D) _OverlapCapsule(point, size, direction, angle, Physics2D.BodySpecialSensor.ActiveOnce, layerMask);
        }

        /**
        *  @brief Returns all {@link TSCollider2D} within a capsule area. Returns null if there is none.
        *  
        *  @param point Center of the capsule in world space.
        *  @param size Size of the capsule.
        *  @param direction If it is a vertical or horizontal capsule.
        *  @param angle Rotation angle in degrees of the capsule.
        *  @param layerMask Unity's layer mask to filter objects.
        **/
        public static FPCollider2D[] OverlapCapsuleAll(FPVector2 point, FPVector2 size, TSCapsuleDirection2D direction, FP angle, int layerMask = UnityEngine.Physics.DefaultRaycastLayers) {
            return (FPCollider2D[]) _OverlapCapsule(point, size, direction, angle, Physics2D.BodySpecialSensor.ActiveAll, layerMask);
        }

        public static object _CircleCast(FPVector2 origin, FP radius, FPVector2 direction, FP distance, Physics2D.BodySpecialSensor sensorType, int layerMask = UnityEngine.Physics.DefaultRaycastLayers) {
            if (distance + radius > FP.MaxValue) {
                distance = FP.MaxValue - radius;
            }

            direction.Normalize();

            FPVector2 offsetToCenter = ((direction * distance) * FP.Half);
            offsetToCenter.x = FP.Abs(offsetToCenter.x);
            offsetToCenter.y = FP.Abs(offsetToCenter.y);

            FP angle = FPVector2.Angle(direction, FPVector2.right);

            if (direction.x <= 0 && direction.y >= 0) {
                offsetToCenter.x = -offsetToCenter.x;
            } else if (direction.x <= 0 && direction.y <= 0) {
                offsetToCenter.x = -offsetToCenter.x;
                offsetToCenter.y = -offsetToCenter.y;
                angle = -angle;
            } else if (direction.x >= 0 && direction.y <= 0) {
                offsetToCenter.y = -offsetToCenter.y;
                angle = -angle;
            }

            FPVector2 center = origin + offsetToCenter;

            object result = _OverlapCapsule(center, new FPVector2(distance + radius * 2, radius * 2), TSCapsuleDirection2D.HORIZONTAL, -angle, sensorType, layerMask);

            if (result is FPCollider2D) {
                return new FPRaycastHit2D((FPCollider2D) result);
            } else {
                FPCollider2D[] resultAux = (FPCollider2D[]) result;
                FPRaycastHit2D[] resultHit = new FPRaycastHit2D[resultAux.Length];

                for (int index = 0; index < resultHit.Length; index++) {
                    resultHit[index] = new FPRaycastHit2D(resultAux[index]);
                }

                return resultHit;
            }
        }

        /**
        *  @brief Cast a circle and returns a {@link TSRaycastHit2D} with information about the first {@link TSCollider2D} found. Returns null if there is none.
        *  
        *  @param origin Origin of the circle in world space.
        *  @param radius Radius of the circle.
        *  @param direction Direction {@link FPVector2} of the cast.
        *  @param distance Max distance to reach.
        *  @param layerMask Unity's layer mask to filter objects.
        **/
        public static FPRaycastHit2D CircleCast(FPVector2 origin, FP radius, FPVector2 direction, FP distance, int layerMask = UnityEngine.Physics.DefaultRaycastLayers) {
            return (FPRaycastHit2D) _CircleCast(origin, radius, direction, distance, Physics2D.BodySpecialSensor.ActiveOnce, layerMask);
        }

        /**
        *  @brief Cast a circle and returns an array {@link TSRaycastHit2D} with information about all {@link TSCollider2D} found. Returns null if there is none.
        *  
        *  @param origin Origin of the circle in world space.
        *  @param radius Radius of the circle.
        *  @param direction Direction {@link FPVector2} of the cast.
        *  @param distance Max distance to reach.
        *  @param layerMask Unity's layer mask to filter objects.
        **/
        public static FPRaycastHit2D[] CircleCastAll(FPVector2 origin, FP radius, FPVector2 direction, FP distance, int layerMask = UnityEngine.Physics.DefaultRaycastLayers) {
            return (FPRaycastHit2D[]) _CircleCast(origin, radius, direction, distance, Physics2D.BodySpecialSensor.ActiveAll, layerMask);
        }

        public static FPRaycastHit2D Raycast(FPVector2 origin, FPVector2 direction, FP distance) {
            return Physics2DWorldManager.instance.Raycast(origin, direction, distance);
        }

        public static FPRaycastHit2D[] RaycastAll(FPVector2 origin, FPVector2 direction, FP distance) {
            return Physics2DWorldManager.instance.RaycastAll(origin, direction, distance);
        }

    }

}