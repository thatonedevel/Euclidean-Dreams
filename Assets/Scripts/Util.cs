using UnityEngine;

namespace EDreams
{
    public static class Util
    {
        ///<summary>
        /// Utility class to help with important game functions
        ///</summary>
        
        public static Vector3Int ConvertVec3ToInt(Vector3 target)
        {
            ///<summary>Converts the supplied <para>target</para> Vector3 to a Vector3Int
            ///</summary>

            int x, y, z = 0;

            x = (int)target.x;
            y = (int)target.y;
            z = (int)target.z;

            return new Vector3Int(x, y, z);
        }
    }
}
