namespace GameConstants
{
    /// <summary>
    /// Namespace for important game constants
    /// </summary>
    public static class Constants
    {
        public const float MAX_RAYCAST_DISTANCE = 100;
        public const int MAX_RAYCAST_COUNT = 100;

        // tags
        public const string TAG_PLAYER = "Player";
        public const string TAG_LEVEL_GEOMETRY = "LevelGeometry";
        public const string TAG_GEM_COLLECTIBLE = "Gem";
        public const string TAG_CAMERA = "CameraRig";
        public const string TAG_LEVEL_DATA = "LevelData";

        public const string LEVEL_PREFIX = "level_";
    }

    namespace Enumerations
    { 
        public enum Dimensions
        {
            /// <summary>
            /// Enum representing which "dimension" is being observed by the player
            /// </summary>
            THIRD,
            SECOND
        }
        public enum MovementAxisCombos
        {
            /// <summary>
            /// The combinations of axes that the player may potentially move on through cardinal input
            /// </summary>
            XZ,
            XY,
            YZ,
        }

        public enum Axes
        {
            /// <summary>
            /// Enum representing the individual axes of movement in 3D
            /// </summary>
            X,
            Y,
            Z
        }

        public enum GemOrders
        {
            FIRST,
            SECOND,
            THIRD
        }

        public enum GameStates
        {
            PLAYING,
            LEVEL_COMPLETE,
            LEVEL_SELECT,
            GAME_OVER,
            TITLE_SCREEN
        }
    }
}
