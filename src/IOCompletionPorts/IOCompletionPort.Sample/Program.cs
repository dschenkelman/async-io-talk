namespace IOCompletionPort.Sample
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using IOCompletionPorts;

    class Program
    {
        static unsafe void Main(string[] args)
        {
            // create completion port
            var completionPortHandle = Interop.CreateIoCompletionPort(new IntPtr(-1), IntPtr.Zero, 0,  0);

            ThreadLogger.Log("Completion port handle: {0}", completionPortHandle);

            var completionPortThread = new Thread(() => new IOCompletionWorker().Start(completionPortHandle))
            {
                IsBackground = true
            };
            completionPortThread.Start();

            const uint Flags = 128 | (uint)1 << 30;

            var fileHandle = Interop.CreateFile("test.txt", (uint)1 << 31, 0, IntPtr.Zero, 3,
                /*FILE_ATTRIBUTE_NORMAL | FILE_FLAG_OVERLAPPED */ Flags,
                IntPtr.Zero);

            ThreadLogger.Log("File handle: {0}", fileHandle);

            Interop.CreateIoCompletionPort(
                fileHandle,
                completionPortHandle,
                (uint)fileHandle.ToInt64(), 
                0);

            ThreadLogger.Log("Associated file handle with completion port");

            var readBuffer = new byte[1024];

            uint bytesRead;

            var overlapped = new Overlapped 
            {
                AsyncResult = new FileReadAsyncResult()
                {
                    ReadCallback = (bytesCount, buffer) =>
                        {
                            var contentRead = Encoding.UTF8.GetString(buffer, 0, (int)bytesCount);
                            ThreadLogger.Log(contentRead);
                        },
                    Buffer = readBuffer
                } 
            };

            NativeOverlapped* nativeOverlapped = overlapped.UnsafePack(null, readBuffer);

            ThreadLogger.Log("Before read in main thread");

            Interop.ReadFile(fileHandle, readBuffer, (uint)readBuffer.Length, out bytesRead, nativeOverlapped);

            ThreadLogger.Log("After read in main thread");

            Console.ReadLine();
        }
    }
}