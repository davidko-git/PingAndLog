using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

namespace PingAndLog
{
    class PingWorker
    {
        int remainsToRecieve;
        readonly int toSendCount;
        readonly IPAddress assignedIPAddress;
        readonly ConcurrentQueue<byte[]> queue;
        readonly byte[] assignedIPAddressEncoded; 
        readonly Semaphore loggerCanDoJobSemaphore;

        public PingWorker(IPAddress assignedIPAddress,
                                    int pingCount,
                                    ConcurrentQueue<byte[]> queue,
                                    Semaphore loggerCanDoJobSemaphore)
        {
            this.queue = queue;
            this.toSendCount = pingCount;
            this.remainsToRecieve = pingCount;
            this.assignedIPAddress = assignedIPAddress;
            this.loggerCanDoJobSemaphore = loggerCanDoJobSemaphore;
            this.assignedIPAddressEncoded = Encoding.UTF8.GetBytes(Constants.TargetAddressOpenString 
                                                                    + assignedIPAddress 
                                                                    + Constants.TargetAddressCloseString);
        }

        public void Start ()
        {
            this.remainsToRecieve = this.toSendCount;

            for (int i = 0; i < toSendCount; i++)
            {
                Ping pingSender = new Ping();
                Thread.Sleep(100);

                try
                {
                    pingSender.PingCompleted += this.PingCompletedCallback;
                    pingSender.SendAsync(this.assignedIPAddress,  300, null);
                }
                catch
                {
                    this.ManagePingSend();
                }
            }
        }
        
        private void PingCompletedCallback(object sender, PingCompletedEventArgs e)
        {
            this.ManagePingSend(e);
        }

        private void ManagePingSend(PingCompletedEventArgs e = null)
        {
            this.EncodeAndEnqueue(e);
            this.loggerCanDoJobSemaphore.Release();
            System.Threading.Interlocked.Decrement(ref this.remainsToRecieve);
            
            if (this.remainsToRecieve == 0)
            {
                this.PingsProcessed?.Invoke(this, null);
            }
        }

        private void EncodeAndEnqueue(PingCompletedEventArgs e = null)
        {
            byte[] encodedReply = e is not null 
                                  ? 
                                  this.EncodeReply(e.Reply.Status, e.Reply.RoundtripTime)
                                  : this.EncodeReply(null, null);
            this.queue.Enqueue(encodedReply);
        }

        private byte[] EncodeReply(IPStatus? status, long? roundtripTime)
        {
            if (status is not null && roundtripTime is not null)
            {
                return this.MakeReplyRecord(status.Value, roundtripTime.Value);
            }
            else
            {
                return this.MakeDefaultReplyRecord();
            }
        }

        private byte[] MakeReplyRecord(IPStatus status, long roundtripTime)
        {
                int destinationIndex = 0;
                byte[] encodedRoundtripTime = Encoding.UTF8.GetBytes(roundtripTime!.ToString());
                int encodedReplyLength = + encodedRoundtripTime.Length
                                         + Constants.RoundtripTimeOpen.Length
                                         + Constants.RoundtripTimeClose.Length
                                         + this.assignedIPAddressEncoded.Length
                                         + Constants.PingReplyRecordOpenEncoded.Length
                                         + Constants.PingReplyRecordCloseEncoded.Length
                                         + Utilities.IPStatusEncodedDict[status].Length;
                byte[] encodedReply = new byte[encodedReplyLength];
                
                // PingReplyRecordOpen
                Array.Copy(Constants.PingReplyRecordOpenEncoded, 0, encodedReply, destinationIndex, Constants.PingReplyRecordOpenEncoded.Length);
                destinationIndex += Constants.PingReplyRecordOpenEncoded.Length;
                // assignedIPAddress
                Array.Copy(this.assignedIPAddressEncoded, 0, encodedReply, destinationIndex, this.assignedIPAddressEncoded.Length);
                destinationIndex += this.assignedIPAddressEncoded.Length;
                // IPStatusEncoded
                Array.Copy(Utilities.IPStatusEncodedDict[status], 0, encodedReply, destinationIndex, Utilities.IPStatusEncodedDict[status].Length);
                destinationIndex += Utilities.IPStatusEncodedDict[status].Length;
                // RoundtripTimeOpen
                Array.Copy(Constants.RoundtripTimeOpen, 0, encodedReply, destinationIndex, Constants.RoundtripTimeOpen.Length);
                destinationIndex += Constants.RoundtripTimeOpen.Length;
                // encodedRoundtripTime
                Array.Copy(encodedRoundtripTime, 0, encodedReply, destinationIndex, encodedRoundtripTime.Length);
                destinationIndex += encodedRoundtripTime.Length;
                // RoundtripTimeClose
                Array.Copy(Constants.RoundtripTimeClose, 0, encodedReply, destinationIndex, Constants.RoundtripTimeClose.Length);
                destinationIndex += Constants.RoundtripTimeClose.Length;
                // PingReplyRecordClose
                Array.Copy(Constants.PingReplyRecordCloseEncoded, 0, encodedReply, destinationIndex, Constants.PingReplyRecordCloseEncoded.Length);
                
                return encodedReply;
        }

        private byte[] MakeDefaultReplyRecord()
        {
            int destinationIndex = 0;
            int encodedReplyLength = this.assignedIPAddressEncoded.Length
                                     + Constants.PingReplyRecordOpenEncoded.Length
                                     + Constants.PingReplyRecordCloseEncoded.Length;
            byte[] encodedReply = new byte[encodedReplyLength];
            
            // PingReplyRecordOpen
            Array.Copy(Constants.PingReplyRecordOpenEncoded, 0, encodedReply, destinationIndex, Constants.PingReplyRecordOpenEncoded.Length);
            destinationIndex += Constants.PingReplyRecordOpenEncoded.Length;
            // assignedIPAddress
            Array.Copy(this.assignedIPAddressEncoded, 0, encodedReply, destinationIndex, this.assignedIPAddressEncoded.Length);
            destinationIndex += this.assignedIPAddressEncoded.Length;
            // PingReplyRecordClose
            Array.Copy(Constants.PingReplyRecordCloseEncoded, 0, encodedReply, destinationIndex, Constants.PingReplyRecordCloseEncoded.Length);
                
            return encodedReply;
        }

        public event EventHandler PingsProcessed;
    }
}
