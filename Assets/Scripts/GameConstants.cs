using UnityEngine.Rendering;

namespace GameConstants
{
    // namespace for important constants & enumerations
    public static class Constants
    {
        public const float MAX_RAYCAST_DISTANCE = 100;
        public const float MAX_RAYCAST_COUNT = 100;
    }

    namespace Enumerations
    {
        

        public enum Dimensions
        {
            THIRD,
            SECOND
        }
        public enum MovementAxisCombos
        {
            XZ,
            XY
        }

        public enum Axes
        {
            X,
            Y,
            Z
        }
    }
}
