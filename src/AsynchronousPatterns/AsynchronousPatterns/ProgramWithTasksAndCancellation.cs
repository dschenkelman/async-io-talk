using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Tasks
{
    public class ProgramWithTasksAndCancellation : IRunnable
    {
        public void Run()
        {
            var client = new HttpClient();

            var cts = new CancellationTokenSource();

            var convertiTask = client.GetAsync(new Uri("http://blogs.southworks.net/mconverti"), cts.Token);
            convertiTask.ContinueWith(t => Console.WriteLine("Converti completed"), TaskContinuationOptions.NotOnCanceled);

            var schenkelmanTask = client.GetAsync(new Uri("http://blogs.southworks.net/dschenkelman"), cts.Token);
            schenkelmanTask.ContinueWith(t => Console.WriteLine("Schenkelman completed"), TaskContinuationOptions.NotOnCanceled);

            Task.WhenAny(convertiTask, schenkelmanTask).ContinueWith(t =>
            {
                cts.Cancel();

                var firstTaskThatCompleted = (t.Result);

                firstTaskThatCompleted.Result.Content.ReadAsStringAsync().ContinueWith(t2 =>
                    {
                        Console.WriteLine(t2.Result.Substring(0, 300));

                        Console.WriteLine("Downloads finished");
                        Program.ResetEvent.Set();
                    });
            });
        }
    }
}
