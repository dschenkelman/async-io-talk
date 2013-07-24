namespace IOCompletionPort.Sample
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;

    public static class ThreadLogger
    {
        private const string ThreadInfoFormat = "Thread {0}:";

        private static Queue<ConsoleColor> colors = new Queue<ConsoleColor>();

        private static Dictionary<int, ConsoleColor> threadToColor = new Dictionary<int, ConsoleColor>();

        private static object lockObject = new object();

        static ThreadLogger()
        {
            colors.Enqueue(ConsoleColor.Red);
            colors.Enqueue(ConsoleColor.Green);
            colors.Enqueue(ConsoleColor.Cyan);
        }

        public static void Log(string message)
        {
            lock (lockObject)
            {
                var threadId = Thread.CurrentThread.ManagedThreadId;

                if (!threadToColor.ContainsKey(threadId))
                {
                    threadToColor[threadId] = colors.Dequeue();
                }

                var oldColor = Console.ForegroundColor;

                Console.ForegroundColor = threadToColor[threadId];

                Console.WriteLine(string.Format(ThreadInfoFormat, threadId) + " " + message);

                Console.ForegroundColor = oldColor;
            }
        }

        public static void Log(string messageFormat, params object[] placeholders)
        {
            ThreadLogger.Log(string.Format(messageFormat, placeholders));
        }
    }
}
