namespace CompletionPortThreadsSample
{
    using System;
    using System.Net;
    using System.Threading;

    class Program
    {
        static object lockObject = new object();

        static void Main(string[] args)
        {
            var responseArrived = false;
            var bodyArrived = false;

            PrintAvailableThreads("Sending request...");

            var request = HttpWebRequest.CreateHttp("http://www.microsoft.com/");
            request.BeginGetResponse(
                ar =>
                {
                    PrintAvailableThreads("Response arrived...");
                    responseArrived = true;

                    var response = (HttpWebResponse)request.EndGetResponse(ar);
                    var responseBody = response.GetResponseStream();

                    byte[] buffer = new byte[1024];
                    responseBody.BeginRead(
                        buffer,
                        0,
                        1024,
                        ars =>
                        {
                            PrintAvailableThreads("Response body read...");
                            bodyArrived = true;
                        },
                        null);
                },
                null);

            while (!responseArrived || !bodyArrived)
            {
                PrintAvailableThreads("Waiting...");
                
                Thread.Sleep(250);
            }

            PrintAvailableThreads("Done!");

            Console.ReadLine();
        }

        private static void PrintAvailableThreads(string message)
        {
            lock (lockObject)
            {
                int workerThreads = 0;
                int completionPortThreads = 0;

                ThreadPool.GetAvailableThreads(out workerThreads, out completionPortThreads);

                Console.WriteLine(message);
                Console.WriteLine("Current thread. Is pool thread: {0}, Hash: {1}", Thread.CurrentThread.IsThreadPoolThread, Thread.CurrentThread.GetHashCode());
                Console.WriteLine("Worker threads available: {0}", workerThreads);
                Console.WriteLine("Completion port threads available: {0}", completionPortThreads);
                Console.WriteLine();
            }
        }
    }
}
