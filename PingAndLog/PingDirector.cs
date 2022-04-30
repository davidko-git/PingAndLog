using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;

namespace PingAndLog
{
    class PingDirector 
    {
        int workingCount;
        int workersCount;
        PingLogger pingLogger;
        PingWorker[] pingWorkers;
        private Semaphore loggerDoneSemaphore;
        private Semaphore workesDoneSemaphore;

        public PingDirector(IPAddress[] targetAdresses, int pingsToSendCount)
        {
            this.pingLogger = new PingLogger();
            this.workersCount = targetAdresses.Length;
            this.loggerDoneSemaphore = new Semaphore(0, 1);
            this.workesDoneSemaphore = new Semaphore(0, 1);
            this.pingLogger.LoggingProcessed += loggerDone;
            this.MakeWorkers(targetAdresses, pingsToSendCount, this.pingLogger.queue);
        }

        private void MakeWorkers(IPAddress[] adresses, int pingsToSendCount, ConcurrentQueue<string> queue) 
        {
            pingWorkers = new PingWorker[adresses.Length];

            for (int i = 0; i < adresses.Length; i++)
            {
                pingWorkers[i] = new PingWorker(adresses[i], pingsToSendCount, queue);
                pingWorkers[i].PingsProcessed += this.pingWorkerDone;
            }
        }

        public void Start() 
        {
            this.workingCount = this.workersCount;
            (new Thread(pingLogger.Start)).Start();

            foreach (PingWorker pingWorker in this.pingWorkers)
            {
                Thread pingWorkerThread = new Thread(pingWorker.Start);
                pingWorkerThread.Start();
            }

            this.workesDoneSemaphore.WaitOne();
            System.Threading.Interlocked.Decrement(ref this.pingLogger.logging);
            this.loggerDoneSemaphore.WaitOne();
        }

        public void pingWorkerDone(object sender, EventArgs e)
        {
            System.Threading.Interlocked.Decrement(ref this.workingCount);
            
            if (this.workingCount == 0)
            {
                this.workesDoneSemaphore.Release();
            }
        }

        public void loggerDone(object sender, EventArgs e)
        {
            this.loggerDoneSemaphore.Release();
        }
    }
}