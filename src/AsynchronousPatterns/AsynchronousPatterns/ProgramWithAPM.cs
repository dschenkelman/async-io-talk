using System;
using System.IO;
using System.Net;
using System.Threading;

namespace Tasks
{
    class ProgramWithAPM : IRunnable
    {
        private int pending;

        private readonly object lockObject;

        public ProgramWithAPM()
        {
            this.pending = 0;
            this.lockObject = new object();
        }

        public void Run()
        {
            var schenkelmanRequest = (HttpWebRequest)WebRequest.Create(new Uri("http://blogs.southworks.net/dschenkelman"));
            var convertiRequest = (HttpWebRequest)WebRequest.Create(new Uri("http://blogs.southworks.net/mconverti"));

            this.pending = 2;

            schenkelmanRequest.BeginGetResponse(ar =>
                {
                    var response = (HttpWebResponse)schenkelmanRequest.EndGetResponse(ar);
                    WriteContent(response);
                }, schenkelmanRequest);

            convertiRequest.BeginGetResponse(ar =>
                {
                    var response = (HttpWebResponse)convertiRequest.EndGetResponse(ar);
                    WriteContent(response);
                }, convertiRequest);
        }

        private void WriteContent(HttpWebResponse response)
        {
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var content = reader.ReadToEnd();
                lock (this.lockObject)
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