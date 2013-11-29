using System;
using System.Threading;

namespace Tasks
{
    class Program
    {
        public static AutoResetEvent ResetEvent = new AutoResetEvent(false);

        static void Main(string[] args)
        {
            // decide which asynchronous proposal to use
            Run(new ProgramWithTasksAndCancellation());
        }

        static void Run(IRunnable runnable)
        {
            runnable.Run();

            ResetEvent.WaitOne();

            Console.ReadLine();
        }
    }
}
