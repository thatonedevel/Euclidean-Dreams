using System;
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

        public static bool TryAddItemToArray<T>(T[] target, T newItem)
        {
            ///<summary>Tries to add the specified newItem to target. Returns a boolean based on success</summary>
            
            int spaceIndex = Array.IndexOf(target, null);
            if (spaceIndex != -1)
            {
                target[spaceIndex] = newItem;
                return true;
            }
            return false;
        }
    }
}
