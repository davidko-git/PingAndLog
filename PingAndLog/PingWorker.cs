using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

namespace PingAndLog
{
    class PingWorker
    {
        int toSendCount;
        int remainsToRecieve;
        ConcurrentQueue<string> queue;
        readonly IPAddress assignedIPAddress;

        public PingWorker(IPAddress assignedIPAddress, int pingCount, ConcurrentQueue<string> queue)
        {
            this.queue = queue;
            this.toSendCount = pingCount;
            this.remainsToRecieve = pingCount;
            this.assignedIPAddress = assignedIPAddress;
        }

        public void Start ()
        {
            this.remainsToRecieve = this.toSendCount;

            for (int i = 0; i < toSendCount; i++)
            {
                Thread.Sleep(100);
                Ping pingSender = new Ping();
                pingSender.PingCompleted += new PingCompletedEventHandler(this.PingCompletedCallback);
                pingSender.SendAsync(this.assignedIPAddress, 300, null);
            }
        }

        public void PingCompletedCallback(object sender, PingCompletedEventArgs e)
        {
            System.Threading.Interlocked.Decrement(ref remainsToRecieve);
            string replyString = String.Format("<PingReplyRecord><TargetAddress>{0}</TargetAddress><Status>{1}</Status><RoundtripTime>{2}</RoundtripTime></PingReplyRecord>",
                                this.assignedIPAddress, e.Reply.Status, e.Reply.RoundtripTime);
            queue.Enqueue(replyString);

            if (this.remainsToRecieve == 0)
            {
                this.PingsProcessed.Invoke(this, null);
            }
        }

        public event EventHandler PingsProcessed;
    }
}
