namespace IOCompletionPort.Sample
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using IOCompletionPorts;

    class Program
    {
        static unsafe void Main(string[] args)
        {
            // create completion port
            var completionPortHandle = Interop.CreateIoCompletionPort(
                new IntPtr(-1), 
                IntPtr.Zero,
                0, 
                0);

            ThreadLogger.Log("Completion port handle: {0}", completionPortHandle);

            var completionPortThread = new Thread(() => new IOCompletionWorker().Start(completionPortHandle))
                                           {
                                               IsBackground = true
                                           };
            completionPortThread.Start();

            const uint Flags = 128 | (uint)1 << 30;

            var fileHandle = Interop.CreateFile(
                "test.txt",
                /*generic read*/ (uint)1 << 31,
                /*don't share*/ 0,
                IntPtr.Zero,
                /*OPEN_EXISTS*/ 3,
                /*FILE_ATTRIBUTE_NORMAL | FILE_FLAG_OVERLAPPED */ Flags,
                IntPtr.Zero);

            ThreadLogger.Log("File handle: {0}", fileHandle);

            Interop.CreateIoCompletionPort(
                fileHandle,
                completionPortHandle,
                (uint)fileHandle.ToInt64(), 
                0);

            ThreadLogger.Log("Associated file handle with completion port");

            var readBuffer = new byte[8192];

            uint bytesRead;

            var overlapped = new Overlapped 
            {
                AsyncResult = new FileReadAsyncResult()
                {
                    ReadCallback = (read, completionKey) => 
                        ThreadLogger.Log("Bytes read: {0}, Completion Key: {1}", read, completionKey)
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