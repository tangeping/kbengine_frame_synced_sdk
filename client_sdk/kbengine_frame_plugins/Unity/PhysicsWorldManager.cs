using UnityEngine;
using System.Collections.Generic;
using System;
using KBEngine.Physics3D;

namespace KBEngine {

    /**
     *  @brief Manages the 3D physics simulation.
     **/
    public class PhysicsWorldManager : IPhysicsManager {

        public static PhysicsWorldManager instance;

        private World world;

        Dictionary<IBody, GameObject> gameObjectMap;

        Dictionary<RigidBody, Dictionary<RigidBody, FPCollision>> collisionInfo;

        /**
         *  @brief Property access to simulated gravity.
         **/
        public FPVector Gravity {
            get;
            set;
        }

        /**
         *  @brief Property access to speculative contacts.
         **/
        public bool SpeculativeContacts {
            get;
            set;
        }

        public FP LockedTimeStep {
            get;
            set;
        }

        // Use this for initialization
        public void Init() {
            ChecksumExtractor.Init(this);

            gameObjectMap = new Dictionary<IBody, GameObject>();
            collisionInfo = new Dictionary<RigidBody, Dictionary<RigidBody, FPCollision>>();

            CollisionSystemPersistentSAP collisionSystem = new CollisionSystemPersistentSAP();
            collisionSystem.EnableSpeculativeContacts = SpeculativeContacts;

            world = new World(collisionSystem);
            collisionSystem.world = world;

            world.physicsManager = this;
            world.Gravity = Gravity;

            world.Events.BodiesBeginCollide += CollisionEnter;
            world.Events.BodiesStayCollide += CollisionStay;
            world.Events.BodiesEndCollide += CollisionExit;

            world.Events.TriggerBeginCollide += TriggerEnter;
            world.Events.TriggerStayCollide += TriggerStay;
            world.Events.TriggerEndCollide += TriggerExit;

            world.Events.RemovedRigidBody += OnRemovedRigidBody;

            instance = this;

            AddRigidBodies();
        }

        /**
         *  @brief Goes one step further on the physics simulation.
         **/
        public void UpdateStep() {
            world.Step(LockedTimeStep);
        }

        /**
         *  @brief Instance of the current simulated world.
         **/
        public IWorld GetWorld() {
            return world;
        }

        void AddRigidBodies() {
            FPCollider[] bodies = GameObject.FindObjectsOfType<FPCollider>();
            List<FPCollider> sortedBodies = new List<FPCollider>(bodies);
            sortedBodies.Sort(UnityUtils.bodyComparer);

            for (int i = 0; i < sortedBodies.Count; i++) {
                AddBody(sortedBodies[i]);
                Debug.Log(" sortedBodies:" + sortedBodies[i].transform.gameObject.name);
            }
        }

        /**
         *  @brief Add a new RigidBody to the world.
         *  
         *  @param jRigidBody Instance of a {@link FPRigidBody}.
         **/
        public void AddBody(ICollider iCollider) {
            if (!(iCollider is FPCollider)) {
                Debug.LogError("You have a 2D object but your Physics 2D is disabled.");
                return;
            }

            FPCollider tsCollider = (FPCollider) iCollider;

            if (tsCollider._body != null) {
                //already added
                return;
            }

            FPRigidBody tsRB = tsCollider.GetComponent<FPRigidBody>();
            FPRigidBodyConstraints constraints = tsRB != null ? tsRB.constraints : FPRigidBodyConstraints.None;

            tsCollider.Initialize();
            world.AddBody(tsCollider._body);
            gameObjectMap[tsCollider._body] = tsCollider.gameObject;

            if (tsCollider.gameObject.transform.parent != null && tsCollider.gameObject.transform.parent.GetComponentInParent<FPCollider>() != null) {
                FPCollider parentCollider = tsCollider.gameObject.transform.parent.GetComponentInParent<FPCollider>();
				world.AddConstraint(new ConstraintHierarchy(parentCollider.Body, tsCollider._body, (tsCollider.GetComponent<FPTransform>().position + tsCollider.ScaledCenter) - (parentCollider.GetComponent<FPTransform>().position + parentCollider.ScaledCenter)));
            }

            tsCollider._body.FreezeConstraints = constraints;
        }

        public void RemoveBody(IBody iBody) {
            world.RemoveBody((RigidBody) iBody);
        }

        public void OnRemoveBody(System.Action<IBody> OnRemoveBody){
            world.Events.RemovedRigidBody += delegate (RigidBody rb) {
                OnRemoveBody(rb);
            };
        }

        public bool Raycast(FPVector rayOrigin, FPVector rayDirection, RaycastCallback raycast, out IBody body, out FPVector normal, out FP fraction) {
            RigidBody rb;
            bool result = world.CollisionSystem.Raycast(rayOrigin, rayDirection, raycast, out rb, out normal, out fraction);
            body = rb;

            return result;
        }

        public FPRaycastHit Raycast(FPRay ray, FP maxDistance, RaycastCallback callback = null) {
            IBody hitBody;
            FPVector hitNormal;
            FP hitFraction;

            FPVector origin = ray.origin;
            FPVector direction = ray.direction;

            if (Raycast(origin, direction, callback, out hitBody, out hitNormal, out hitFraction)) {
                if (hitFraction <= maxDistance) {
                    GameObject other = PhysicsManager.instance.GetGameObject(hitBody);
                    FPRigidBody bodyComponent = other.GetComponent<FPRigidBody>();
                    FPCollider colliderComponent = other.GetComponent<FPCollider>();
                    FPTransform transformComponent = other.GetComponent<FPTransform>();
                    return new FPRaycastHit(bodyComponent, colliderComponent, transformComponent, hitNormal, ray.origin, ray.direction, hitFraction);
                }
            } else {
                direction *= maxDistance;
                if (Raycast(origin, direction, callback, out hitBody, out hitNormal, out hitFraction)) {
                    GameObject other = PhysicsManager.instance.GetGameObject(hitBody);
                    FPRigidBody bodyComponent = other.GetComponent<FPRigidBody>();
                    FPCollider colliderComponent = other.GetComponent<FPCollider>();
                    FPTransform transformComponent = other.GetComponent<FPTransform>();
                    return new FPRaycastHit(bodyComponent, colliderComponent, transformComponent, hitNormal, ray.origin, direction, hitFraction);
                }
            }
            return null;
        }

        private void OnRemovedRigidBody(RigidBody body) {
            GameObject go = gameObjectMap[body];

            if (go != null) {
                GameObject.Destroy(go);
            }
        }

        private void CollisionEnter(Contact c) {
            CollisionDetected(c.body1, c.body2, c, "OnSyncedCollisionEnter");
        }

        private void CollisionStay(Contact c) {
            CollisionDetected(c.body1, c.body2, c, "OnSyncedCollisionStay");
        }

        private void CollisionExit(RigidBody body1, RigidBody body2) {
            CollisionDetected(body1, body2, null, "OnSyncedCollisionExit");
        }

        private void TriggerEnter(Contact c) {
            CollisionDetected(c.body1, c.body2, c, "OnSyncedTriggerEnter");
        }

        private void TriggerStay(Contact c) {
            CollisionDetected(c.body1, c.body2, c, "OnSyncedTriggerStay");
        }

        private void TriggerExit(RigidBody body1, RigidBody body2) {
            CollisionDetected(body1, body2, null, "OnSyncedTriggerExit");
        }

        private void CollisionDetected(RigidBody body1, RigidBody body2, Contact c, string callbackName) {
            if (!gameObjectMap.ContainsKey(body1) || !gameObjectMap.ContainsKey(body2)) {
                return;
            }

            GameObject b1 = gameObjectMap[body1];
            GameObject b2 = gameObjectMap[body2];

            if (b1 == null || b2 == null) {
                return;
            }

            b1.SendMessage(callbackName, GetCollisionInfo(body1, body2, c), SendMessageOptions.DontRequireReceiver);
            b2.SendMessage(callbackName, GetCollisionInfo(body2, body1, c), SendMessageOptions.DontRequireReceiver);

//			FrameSyncManager.UpdateCoroutines ();
        }

        private FPCollision GetCollisionInfo(RigidBody body1, RigidBody body2, Contact c) {
            if (!collisionInfo.ContainsKey(body1)) {
                collisionInfo.Add(body1, new Dictionary<RigidBody, FPCollision>());
            }

            Dictionary<RigidBody, FPCollision> collisionInfoBody1 = collisionInfo[body1];

            FPCollision result = null;

            if (collisionInfoBody1.ContainsKey(body2)) {
                result = collisionInfoBody1[body2];
            } else {
                result = new FPCollision();
                collisionInfoBody1.Add(body2, result);
            }


            result.Update(gameObjectMap[body2], c);

            return result;
        }

        /**
         *  @brief Get the GameObject related to a specific RigidBody.
         *  
         *  @param rigidBody Instance of a {@link RigidBody}
         **/
        public GameObject GetGameObject(IBody rigidBody) {
            if (!gameObjectMap.ContainsKey(rigidBody)) {
                return null;
            }

            return gameObjectMap[rigidBody];
        }

        public int GetBodyLayer(IBody body) {
            GameObject go = GetGameObject(body);
            if (go == null) {
                return -1;
            }

            return go.layer;
        }

        /**
         *  @brief Check if the collision between two RigidBodies is enabled.
         *  
         *  @param rigidBody1 First {@link RigidBody}
         *  @param rigidBody2 Second {@link RigidBody}
         **/
        public bool IsCollisionEnabled(IBody rigidBody1, IBody rigidBody2) {
            return LayerCollisionMatrix.CollisionEnabled(gameObjectMap[rigidBody1], gameObjectMap[rigidBody2]);
        }

        public IWorldClone GetWorldClone() {
            return new WorldClone();
        }

    }

}