namespace IOCompletionPort.Sample
{
    using System;
    using System.Threading;

    using IOCompletionPorts;

    public class IOCompletionWorker
    { 
        public void Start(IntPtr completionPort)
        {
            while (true)
            {
                uint bytesRead;
                uint completionKey;
                IntPtr overlapped;

                var result = Interop.GetQueuedCompletionStatus(
                    completionPort, 
                    out bytesRead,
                    out completionKey,
                    out overlapped, 
                    uint.MaxValue);

                if (result)
                {
                    Console.WriteLine("Byte read");
                }
                else
                {
                    Console.WriteLine(Interop.GetLastError());
                }
            }
        }
    }
}
