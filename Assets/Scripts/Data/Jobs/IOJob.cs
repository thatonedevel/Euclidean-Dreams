using System.IO;
using System.Text;
using Unity.Collections;
using Unity.Jobs;
using System.IO;

namespace Data.Jobs
{
    public interface IOJob : IJob
    {
        public void SetPath(string path);
        public void Free();
    }

    public struct ReadJob : IOJob
    {
        public NativeArray<byte> filePathBytes;
        public NativeArray<byte> fileContents;

        //private const int MAX_BIND_SIZE = 33000;

        public void SetPath(string path)
        {
            filePathBytes =  new NativeArray<byte>(Encoding.UTF8.GetBytes(path), Allocator.TempJob);
            // make sure to construct the filecontents array here too
            fileContents = new NativeArray<byte>(fileContents.Length, Allocator.TempJob);
        }

        public void Execute()
        {
            fileContents = new NativeArray<byte>(33000, Allocator.Temp);
        }

        public string GetFileContents()
        {
            return Encoding.UTF8.GetString(fileContents);
        }

        public void Free()
        {
            filePathBytes.Dispose();
            fileContents.Dispose();
        }
    }

    public struct WriteJob: IOJob
    {
        public NativeArray<byte> filePathBytes;
        public NativeArray<byte> writeBytes;

        public void SetPath(string path)
        {
            filePathBytes =  new NativeArray<byte>(Encoding.ASCII.GetBytes(path), Allocator.TempJob);
        }

        public void SetWriteBytes(string data)
        {
            // kinda suboptimal but for our uses this won't present much of an issue
            // assume utf 8 encoding here
            writeBytes = new NativeArray<byte>(Encoding.UTF8.GetBytes(data), Allocator.TempJob);
        }

        public void Execute()
        {
            File.WriteAllBytes(Encoding.UTF8.GetString(filePathBytes), writeBytes.ToArray());
        }

        public void Free()
        {
            filePathBytes.Dispose();
            writeBytes.Dispose();
        }
    }
}