using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Tasks
{
    public static class TaskExtensions
    {
        public static Task<Stream> CustomGetResponseStreamAsync(this HttpWebRequest webRequest)
        {
            var tcs = new TaskCompletionSource<Stream>();

            webRequest.BeginGetResponse(ar =>
            {
                var response = (HttpWebResponse)webRequest.EndGetResponse(ar);

                tcs.SetResult(response.GetResponseStream());
            }, webRequest);

            return tcs.Task;
        }

        public static Task<Stream> TaskBasedOpenReadAsync(this WebClient webClient, Uri uri)
        {
            var tcs = new TaskCompletionSource<Stream>();

            OpenReadCompletedEventHandler callback = null;
            
            callback = (sender, args) =>
                {
                    tcs.SetResult(args.Result);
                    webClient.OpenReadCompleted -= callback;
                };

            webClient.OpenReadCompleted += callback;

            webClient.OpenReadAsync(uri);

            return tcs.Task;
        }
    }
}
