/*
* Farseer Physics Engine:
* Copyright (c) 2012 Ian Qvist
*/

using FP = KBEngine.FP;

namespace KBEngine.Physics2D
{
    /// <summary>
    /// Convert units between display and simulation units.
    /// </summary>
    public static class ConvertUnits
    {
        private static FP _displayUnitsToSimUnitsRatio = 100f;
        private static FP _simUnitsToDisplayUnitsRatio = 1 / _displayUnitsToSimUnitsRatio;

        public static void SetDisplayUnitToSimUnitRatio(FP displayUnitsPerSimUnit)
        {
            _displayUnitsToSimUnitsRatio = displayUnitsPerSimUnit;
            _simUnitsToDisplayUnitsRatio = 1 / displayUnitsPerSimUnit;
        }

        public static FP ToDisplayUnits(FP simUnits)
        {
            return simUnits * _displayUnitsToSimUnitsRatio;
        }

        public static FP ToDisplayUnits(int simUnits)
        {
            return simUnits * _displayUnitsToSimUnitsRatio;
        }

        public static FPVector2 ToDisplayUnits(FPVector2 simUnits)
        {
            return simUnits * _displayUnitsToSimUnitsRatio;
        }

        public static void ToDisplayUnits(ref FPVector2 simUnits, out FPVector2 displayUnits)
        {
            FPVector2.Multiply(ref simUnits, _displayUnitsToSimUnitsRatio, out displayUnits);
        }

        public static FPVector ToDisplayUnits(FPVector simUnits)
        {
            return simUnits * _displayUnitsToSimUnitsRatio;
        }

        public static FPVector2 ToDisplayUnits(FP x, FP y)
        {
            return new FPVector2(x, y) * _displayUnitsToSimUnitsRatio;
        }

        public static void ToDisplayUnits(FP x, FP y, out FPVector2 displayUnits)
        {
            displayUnits = FPVector2.zero;
            displayUnits.x = x * _displayUnitsToSimUnitsRatio;
            displayUnits.y = y * _displayUnitsToSimUnitsRatio;
        }

        public static FP ToSimUnits(FP displayUnits)
        {
            return displayUnits * _simUnitsToDisplayUnitsRatio;
        }

        public static FP ToSimUnits(int displayUnits)
        {
            return displayUnits * _simUnitsToDisplayUnitsRatio;
        }

        public static FPVector2 ToSimUnits(FPVector2 displayUnits)
        {
            return displayUnits * _simUnitsToDisplayUnitsRatio;
        }

        public static FPVector ToSimUnits(FPVector displayUnits)
        {
            return displayUnits * _simUnitsToDisplayUnitsRatio;
        }

        public static void ToSimUnits(ref FPVector2 displayUnits, out FPVector2 simUnits)
        {
            FPVector2.Multiply(ref displayUnits, _simUnitsToDisplayUnitsRatio, out simUnits);
        }

        public static FPVector2 ToSimUnits(FP x, FP y)
        {
            return new FPVector2(x, y) * _simUnitsToDisplayUnitsRatio;
        }

        public static void ToSimUnits(FP x, FP y, out FPVector2 simUnits)
        {
            simUnits = FPVector2.zero;
            simUnits.x = x * _simUnitsToDisplayUnitsRatio;
            simUnits.y = y * _simUnitsToDisplayUnitsRatio;
        }
    }
}