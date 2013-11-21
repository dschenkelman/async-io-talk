using System;
using System.Threading;

namespace Tasks
{
    class Program
    {
        public static AutoResetEvent ResetEvent = new AutoResetEvent(false);

        static void Main(string[] args)
        {
            new ProgramWithEvents().Run();

            ResetEvent.WaitOne();

            Console.ReadLine();
        }
    }
}
