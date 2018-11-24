using UnityEngine;

namespace KBEngine {

    /**
     *  @brief Simulates physical properties of a body.
     **/
    [AddComponentMenu("FrameSync/Physics/Material", 22)]
    public class FPMaterial : MonoBehaviour {

        /**
         *  @brief Static friction when in contact. 
         **/
        public FP friction = 0.25f;

        /**
         *  @brief Coeficient of restitution. 
         **/
        public FP restitution = 0;

    }

}