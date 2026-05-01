using UnityEngine;
using Unity.Jobs;

namespace Data.Jobs
{
    public static class IOJobFactory
    {
        public static IOJob CreateJob(string path, IOJobType opType)
        {
            IOJob j;
            
            if (opType == IOJobType.WRITE)
            {
                j = new WriteJob();
            }
            else
            {
                j = new ReadJob();
            }

            j.SetPath(path);
            return j;
        }
    }

    public enum IOJobType
    {
        READ,
        WRITE
    }
}
