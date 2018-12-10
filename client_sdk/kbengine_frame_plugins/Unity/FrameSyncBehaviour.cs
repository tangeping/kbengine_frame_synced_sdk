using System;
using UnityEngine;

namespace KBEngine {

    /**
     *  @brief Represents each player's behaviour simulated on every machine connected to the game.
     */
    public abstract class FrameSyncBehaviour : MonoBehaviour, IFrameSyncBehaviourGamePlay, IFrameSyncBehaviourCallbacks {

        /**
         * @brief Number of players connected to the game.
         **/
        [HideInInspector]
        public int numberOfPlayers;

        /**
         *  @brief Index of the owner at initial players list.
         */
		public int ownerIndex = -1;

        /**
         *  @brief Basic info about the owner of this behaviour.
         */
        [HideInInspector]
        public Entity owner;

        /**
         *  @brief Basic info about the local player.
         */
        [HideInInspector]
        public Entity localOwner;

        private FPTransform _FPTransform;

        /**
         *  @brief Returns the {@link FPTransform} attached.
         */
        public FPTransform FPTransform {
            get {
                if (_FPTransform == null) {
                    _FPTransform = this.GetComponent<FPTransform>();
                }

                return _FPTransform;
            }
        }

        private FPTransform2D _FPTransform2D;

        /**
         *  @brief Returns the {@link FPTransform2D} attached.
         */
        public FPTransform2D FPTransform2D {
            get {
                if (_FPTransform2D == null) {
                    _FPTransform2D = this.GetComponent<FPTransform2D>();
                }

                return _FPTransform2D;
            }
        }

        private FPRigidBody _FPRigidBody;

        /**
         *  @brief Returns the {@link FPRigidBody} attached.
         */
        public FPRigidBody FPRigidBody {
            get {
                if (_FPRigidBody == null) {
                    _FPRigidBody = this.GetComponent<FPRigidBody>();
                }

                return _FPRigidBody;
            }
        }

        private FPRigidBody2D _FPRigidBody2D;

        /**
         *  @brief Returns the {@link FPRigidBody2D} attached.
         */
        public FPRigidBody2D FPRigidBody2D {
            get {
                if (_FPRigidBody2D == null) {
                    _FPRigidBody2D = this.GetComponent<FPRigidBody2D>();
                }

                return _FPRigidBody2D;
            }
        }

        private FPCollider _tsCollider;

        /**
         *  @brief Returns the {@link TSCollider} attached.
         */
        public FPCollider tsCollider {
            get {
                if (_tsCollider == null) {
                    _tsCollider = this.GetComponent<FPCollider>();
                }

                return _tsCollider;
            }
        }

        private FPCollider2D _tsCollider2D;

        /**
         *  @brief Returns the {@link TSCollider2D} attached.
         */
        public FPCollider2D tsCollider2D {
            get {
                if (_tsCollider2D == null) {
                    _tsCollider2D = this.GetComponent<FPCollider2D>();
                }

                return _tsCollider2D;
            }
        }

        /**
         * @brief It is not called for instances of {@link FrameSyncBehaviour}.
         **/
        public void SetGameInfo(FPPlayerInfo localOwner, int numberOfPlayers) {}

        /**
         * @brief Called once when the object becomes active.
         **/
        public virtual void OnSyncedStart() { }

        /**
         * @brief Called once on instances owned by the local player after the object becomes active.
         **/
        public virtual void OnSyncedStartLocalPlayer() { }

        /**
         * @brief Called when the game has paused.
         **/
        public virtual void OnGamePaused() { }

        /**
         * @brief Called when the game has unpaused.
         **/
        public virtual void OnGameUnPaused() { }

        /**
         * @brief Called when the game has ended.
         **/
        public virtual void OnGameEnded() { }

        /**
         *  @brief Called before {@link #OnSyncedUpdate}.
         *  
         *  Called once every lockstepped frame.
         */
        public virtual void OnPreSyncedUpdate() { }

        /**
         *  @brief Game updates goes here.
         *  
         *  Called once every lockstepped frame.
         */
        public virtual void OnSyncedUpdate() { }

        /**
         *  @brief Get local player data.
         *  
         *  Called once every lockstepped frame.
         */
        public virtual void OnSyncedInput() { }

        /**
         * @brief Callback called when a player get disconnected.
         **/
        public virtual void OnPlayerDisconnection(int playerId) {}

    }

}