namespace IOCompletionPort.Sample
{
    using System;
    using System.Threading;

    using IOCompletionPorts;

    public class IOCompletionWorker
    { 
        public unsafe void Start(IntPtr completionPort)
        {
            while (true)
            {
                uint bytesRead;
                uint completionKey;
                NativeOverlapped* nativeOverlapped;

                ThreadLogger.Log("About to get queued completion status on {0}", completionPort);

                var result = Interop.GetQueuedCompletionStatus(
                    completionPort, 
                    out bytesRead,
                    out completionKey,
                    &nativeOverlapped, 
                    uint.MaxValue);

                var overlapped = Overlapped.Unpack(nativeOverlapped);

                if (result)
                {
                    var asyncResult = ((FileReadAsyncResult)overlapped.AsyncResult);
                    asyncResult.ReadCallback(bytesRead, asyncResult.Buffer);
                }
                else
                {
                    ThreadLogger.Log(Interop.GetLastError().ToString());
                }

                Overlapped.Free(nativeOverlapped);
            }
        }
    }
}
