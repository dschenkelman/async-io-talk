using System;
using System.IO;
using System.Net;

namespace Tasks
{
    public class ProgramWithEvents : IRunnable
    {
        private int pending;

        private readonly object lockObject;

        public ProgramWithEvents()
        {
            this.pending = 0;
            this.lockObject = new object();
        }

        public void Run()
        {
            var convertiClient = new WebClient();
            var schenkelmanClient = new WebClient();

            convertiClient.OpenReadCompleted += (sender, eventArgs) => this.WriteContent(eventArgs.Result);
            schenkelmanClient.OpenReadCompleted += (sender, eventArgs) => this.WriteContent(eventArgs.Result);

            this.pending = 2;

            schenkelmanClient.OpenReadAsync(new Uri("http://blogs.southworks.net/dschenkelman"));
            convertiClient.OpenReadAsync(new Uri("http://blogs.southworks.net/mconverti"));
        }

        private void WriteContent(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                var content = reader.ReadToEnd();
                
                lock (lockObject)
                {
                    Console.WriteLine(content.Substring(0, 300));

                    this.pending--;

                    if (this.pending == 0)
                    {
                        Console.WriteLine("Downloads finished");
                        Program.ResetEvent.Set();
                    }
                }
            }
        }
    }
}