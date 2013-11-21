using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Tasks
{
    class ProgramWithTasks
    {
        public void Run()
        {
            var client = new HttpClient();

            var convertiTask = client.GetStreamAsync(new Uri("http://blogs.southworks.net/mconverti")).ContinueWith(t =>
            {
                using (var reader = new StreamReader(t.Result))
                {
                    var content = reader.ReadToEnd();
                    Console.WriteLine(content.Substring(0, 300));
                }
            });

            var schenkelmanTask = client.GetStreamAsync(new Uri("http://blogs.southworks.net/dschenkelman")).ContinueWith(t =>
            {
                using (var reader = new StreamReader(t.Result))
                {
                    var content = reader.ReadToEnd();
                    Console.WriteLine(content.Substring(0, 300));
                }
            });

            Task.WhenAll(convertiTask, schenkelmanTask).ContinueWith(t =>
                {
                    Console.WriteLine("Downloads finished");
                    Program.ResetEvent.Set();
                });
        }
    }
}
