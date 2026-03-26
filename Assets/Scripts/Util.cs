using System;
using System.Data;
using System.Threading;
using UnityEngine;
using GameConstants.Enumerations;
using UnityEditor;

namespace EDreams
{
    ///<summary>
         /// Utility class to help with important game functions
         ///</summary>
    public static class Util
    {
        
        /// <summary>Converts the supplied <para>target</para> Vector3 to a Vector3Int</summary>
        public static Vector3Int ConvertVec3ToInt(Vector3 target)
        {
            

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
                    thirdAxis = 0; // HACK: the check fails as the z rotation is stored as 90
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

    public struct Matrix
    {
        public float[,] data;
        public (int rows, int cols) order;

        public Matrix(int rows, int cols)
        {
            data = new float[rows, cols];
            order = (rows, cols);
        }

        public float this[int row, int col]
        {
            get { return data[row, col]; }
            set { data[row, col] = value; }
        }
        
        public static Matrix operator +(Matrix m1, Matrix m2)
        {
            // orders must match
            if (m1.order != m2.order)
            {
                throw new ArithmeticException("Orders do not match");
            }
            
            Matrix result = new Matrix(m1.order.rows, m1.order.cols);

            for (int i = 0; i < m1.order.rows; i++)
            {
                for (int j = 0; j < m1.order.cols; j++)
                {
                    result.data[i, j] = m2.data[i, j] + m1.data[i, j];
                }
            }
            
            return result;
        }

        public static Matrix operator *(Matrix m1, Matrix m2)
        {
            // make sure that we have the correct orders
            if (m1.order.cols != m2.order.rows)
            {
                throw new ArithmeticException("Amount of columns in first matrix must match amount of rows in second matrix");
            }
            
            (int r, int c) resOrder = (m1.order.rows, m2.order.cols);
            
            Matrix result = new Matrix(resOrder.r, resOrder.c);

            for (int i = 0; i < resOrder.r; i++)
            {
                for (int j = 0; j < resOrder.c; j++)
                {
                    // inner loop to calculate the needed summations
                    for (int k = 0; k < m1.order.cols; k++)
                    {
                        result[i, k] += m2.data[i, k] * m1.data[k, j];
                    }
                }
            }
            
            return result;
        }
    }
}
