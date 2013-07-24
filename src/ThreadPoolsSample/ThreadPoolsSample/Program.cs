namespace ThreadPoolsSample
{
    using System;
    using System.Text;
    using System.Threading;

    using System.IO;

    class Program
    {
        static void Main()
        {
            ShowAvailableThreads();

            var fs = new FileStream("test.txt", FileMode.Open, FileAccess.Read, FileShare.None, 1024, true);
            var buffer = new byte[1024];

            fs.BeginRead(buffer, 0, 1024, ReadCallback, new State { Buffer = buffer, FileStream = fs });

            Console.ReadLine();
        }

        private static void ReadCallback(IAsyncResult ar)
        {
            ShowAvailableThreads();

            var state = (State)ar.AsyncState;
            var bytesRead = state.FileStream.EndRead(ar);

            Console.WriteLine(Encoding.UTF8.GetString(state.Buffer, 0, bytesRead));
        }

        static void ShowAvailableThreads()
        {
            int workerThreads, completionPortThreads;

            ThreadPool.GetAvailableThreads(out workerThreads, out completionPortThreads);
            Console.WriteLine("WorkerThreads: {0}, CompletionPortThreads: {1}", workerThreads, completionPortThreads);
        }

        private class State
        {
            public FileStream FileStream { get; set; }

            public byte[] Buffer { get; set; }
        }
    }
}