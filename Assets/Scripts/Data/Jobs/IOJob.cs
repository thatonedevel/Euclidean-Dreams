using System.Text;
using Unity.Collections;
using Unity.Jobs;

namespace Data.Jobs
{
    public interface IOJob : IJob
    {
        public void SetPath(string path);
    }

    public struct ReadJob : IOJob
    {
        public NativeArray<byte> filePathBytes;

        public void SetPath(string path)
        {
            filePathBytes =  new NativeArray<byte>(Encoding.ASCII.GetBytes(path), Allocator.TempJob);
        }

        public void Execute()
        {
            
        }
    }

    public struct WriteJob: IOJob
    {
        public NativeArray<byte> filePathBytes;

        public void SetPath(string path)
        {
            filePathBytes =  new NativeArray<byte>(Encoding.ASCII.GetBytes(path), Allocator.TempJob);
        }

        public void Execute()
        {
            
        }
    }
}