using System.Collections.Generic;
using System;
using System.Reflection;

namespace KBEngine {

    /**
    *  @brief Provides a few utilities to be used on FrameSync exposed classes.
    **/
    public class UnityUtils {

        /**
         *  @brief Comparer class to guarantee PhotonPlayer order.
         **/
//        public class PlayerComparer : Comparer<PhotonPlayer> {
//
//            public override int Compare(PhotonPlayer x, PhotonPlayer y) {
//                return x.ID - y.ID;
//            }
//
//        }

        /**
         *  @brief Instance of a {@link PlayerComparer}.
         **/
//        public static PlayerComparer playerComparer = new PlayerComparer();

        /**
         *  @brief Comparer class to guarantee {@link TSCollider} order.
         **/
        public class FPBodyComparer : Comparer<FPCollider> {

            public override int Compare(FPCollider x, FPCollider y) {
                return x.gameObject.name.CompareTo(y.gameObject.name);
            }

        }

        /**
         *  @brief Comparer class to guarantee {@link TSCollider2D} order.
         **/
        public class FPBody2DComparer : Comparer<FPCollider2D> {

            public override int Compare(FPCollider2D x, FPCollider2D y) {
                return x.gameObject.name.CompareTo(y.gameObject.name);
            }

        }

        /**
         *  @brief Instance of a {@link TSBodyComparer}.
         **/
        public static FPBodyComparer bodyComparer = new FPBodyComparer();

        /**
         *  @brief Instance of a {@link TSBody2DComparer}.
         **/
        public static FPBody2DComparer body2DComparer = new FPBody2DComparer();

    }

}