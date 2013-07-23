namespace IOCompletionPort.Sample
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;

    using IOCompletionPorts;

    class Program
    {
        static void Main(string[] args)
        {
            // create completion port
            var completionPortHandle = Interop.CreateIoCompletionPort(
                new IntPtr(-1), 
                IntPtr.Zero,
                0, 
                0);

            const uint Flags = 128 | (uint)1 << 30;

            var fileHandle = Interop.CreateFile(
                "test.txt",
                /*read*/ 1 << 32,
                /*don't share*/ 1,
                IntPtr.Zero,
                /*OPEN_EXISTS*/ 3,
                /*FILE_ATTRIBUTE_NORMAL | FILE_FLAG_OVERLAPPED */ Flags,
                IntPtr.Zero);

            Console.WriteLine("Completion port handle: {0}", completionPortHandle);
            Console.WriteLine("File handle: {0}", fileHandle);

            Interop.CreateIoCompletionPort(
                completionPortHandle,
                fileHandle,
                Constants.TestReadKey, 
                0);

            Console.WriteLine("Associated file handle with completion port");

            Task.Run(() => new IOCompletionWorker().Start(completionPortHandle));
            
            var readBuffer = new byte[8192];

            UInt32 bytesRead;

            var ev = new ManualResetEvent(false);
            var overlapped = new NativeOverlapped { EventHandle = ev.SafeWaitHandle.DangerousGetHandle() };

            GCHandle gch = GCHandle.Alloc(overlapped, GCHandleType.Pinned);

            Interop.ReadFile(fileHandle, readBuffer, (uint)readBuffer.Length, out bytesRead, gch.AddrOfPinnedObject());

            ev.WaitOne();

            gch.Free();

            Console.WriteLine("After read in main thread");

            Console.ReadLine();
        }
    }
}