using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;

namespace PingAndLog
{
    class PingLogger
    {
        public int loggingOn;
        private readonly string outputFileName;
        public readonly Semaphore canLogSemaphore;
        public readonly ConcurrentQueue<byte[]> queue;

        public PingLogger(string outputFileName)
        {
            this.loggingOn = 0;
            this.outputFileName = outputFileName;
            this.queue = new ConcurrentQueue<byte[]>();
            this.canLogSemaphore = new Semaphore(0, Int32.MaxValue);
        }

        public void Start()
        {
            this.loggingOn = 1;

            using (FileStream outputFile = File.Open(outputFileName, FileMode.Create)) 
            {
                while (this.loggingOn == 1)
                {
                    canLogSemaphore.WaitOne();

                    if (queue.TryDequeue(out byte[] currentItem))
                    {
                        outputFile.Write(currentItem);
                    }
                }
            }

            this.LoggingDone();
        }

        private void LoggingDone()
        {
            this.LoggingProcessed?.Invoke(this, null);
        }

        public event EventHandler LoggingProcessed;
    }
}