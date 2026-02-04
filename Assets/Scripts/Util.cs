using System;
using UnityEngine;
using GameConstants.Enumerations;

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

        public static int RoundToNearest45(float val)
        {
            // takes in a value representing an angle & rounds it to the nearest factor of 45

            int lower = 0;
            int upper = 0;

            return 0;
        }

        private static bool IsValueInRange(int value, int target, int range)
        {
            // checks if the supplied value is in range to the target by the specified amount
            return Math.Abs(target - value) <= range;
        }

        public static bool IsVectorInRange(Vector3 v, int target, int range, Axes axisToCheck)
        {
            // override to do the same with a vector3 on the specified axis. will check that the other two axes are also zero

            int checkValue = 0;
            int secondAxis = 0;
            int thirdAxis = 0;

            switch (axisToCheck)
            {
                case Axes.X:
                    checkValue = (int)v.x;
                    secondAxis = (int)v.y;
                    thirdAxis = (int)v.z;
                    break;
                case Axes.Y:
                    checkValue = (int)v.y;
                    secondAxis = (int)v.z;
                    thirdAxis = (int)v.x;
                    break;
                case Axes.Z:
                    checkValue = (int)v.z;
                    secondAxis = (int)v.x;
                    thirdAxis = (int)v.y;
                    break;
            }

            return IsValueInRange(checkValue, target, range) && IsValueInRange(secondAxis, 0, 5) && IsValueInRange(thirdAxis, 0, 5);
        }
    }
}
