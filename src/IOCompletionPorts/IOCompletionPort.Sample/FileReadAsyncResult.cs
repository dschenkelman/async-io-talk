namespace IOCompletionPort.Sample
{
    using System;
    using System.Threading;

    class FileReadAsyncResult : IAsyncResult
    {
        public bool IsCompleted { get; private set; }

        public WaitHandle AsyncWaitHandle { get; private set; }

        public object AsyncState { get; private set; }

        public bool CompletedSynchronously { get; private set; }

        public Action<uint, uint> ReadCallback { get; set; }
    }
}
