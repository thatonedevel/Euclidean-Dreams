using UnityEngine;
using Unity.Jobs;

namespace Data.Jobs
{
    public static class IOJobFactory
    {
        public static IOJob CreateJob(string path, IOJobType opType, string dataToWrite="")
        {
            IOJob j;
            
            if (opType == IOJobType.WRITE)
            {
                j = new WriteJob();
                // give the data to the job that we want to write to disk
                ((WriteJob)j).SetWriteBytes(dataToWrite);
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
