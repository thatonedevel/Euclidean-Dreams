using UnityEngine;
using Unity.Jobs;

namespace Data.Jobs
{
    public static class IOJobFactory
    {
        public static void CreateJob(string path, IOJobType opType)
        {
            if (opType == IOJobType.WRITE)
            {
                
            }
            else
            {
                
            }
        }
    }

    public enum IOJobType
    {
        READ,
        WRITE
    }
}
