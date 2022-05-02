using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;

namespace PingAndLog
{
    class PingDirector 
    {
        private int workingCount;
        private PingWorker[] pingWorkers;
        private readonly int workersCount;
        private readonly PingLogger pingLogger;
        private readonly Semaphore loggerDoneSemaphore;
        private readonly Semaphore workesDoneSemaphore;

        public PingDirector(IPAddress[] targetAddresses, int pingsToSendCount, string outputFileName)
        {
            this.pingLogger = new PingLogger(outputFileName);
            this.workersCount = targetAddresses.Length;
            this.loggerDoneSemaphore = new Semaphore(0, 1);
            this.workesDoneSemaphore = new Semaphore(0, 1);
            this.pingLogger.LoggingProcessed += LoggerDone;
            this.MakeWorkers(targetAddresses,
                            pingsToSendCount,
                            this.pingLogger.queue,
                            this.pingLogger.canLogSemaphore);
        }

        private void MakeWorkers(IPAddress[] addresses,
                                int pingsToSendCount,
                                ConcurrentQueue<byte[]> queue,
                                Semaphore loggerCanDoJobSemaphore) 
        {
            pingWorkers = new PingWorker[addresses.Length];

            for (int i = 0; i < addresses.Length; i++)
            {
                pingWorkers[i] = new PingWorker(addresses[i],
                                                pingsToSendCount,
                                                queue,
                                                loggerCanDoJobSemaphore);
                pingWorkers[i].PingsProcessed += this.PingWorkerDone;
            }
        }

        public void Start() 
        {
            this.workingCount = this.workersCount;
            (new Thread(pingLogger.Start)).Start();
            this.EnqueueXMLStart();

            foreach (PingWorker pingWorker in this.pingWorkers)
            {
                (new Thread(pingWorker.Start)).Start();
            }
            
            this.workesDoneSemaphore.WaitOne();
            this.EnqueueXMLEnd();
            System.Threading.Interlocked.Decrement(ref this.pingLogger.loggingOn);
            this.pingLogger.canLogSemaphore.Release();
            this.loggerDoneSemaphore.WaitOne();
        }
        
        private void EnqueueXMLStart()
        {
            pingLogger.queue.Enqueue(Constants.XMLHeader);
            pingLogger.queue.Enqueue(Constants.ResultsOpen);
            pingLogger.canLogSemaphore.Release(2);
        }

        private void EnqueueXMLEnd()
        {
            pingLogger.queue.Enqueue(Constants.ResultsClose);
            pingLogger.canLogSemaphore.Release(1);
        }
        
        private void PingWorkerDone(object sender, EventArgs e)
        {
            System.Threading.Interlocked.Decrement(ref this.workingCount);
            
            if (this.workingCount == 0)
            {
                this.workesDoneSemaphore.Release();
            }
        }

        private void LoggerDone(object sender, EventArgs e)
        {
            this.loggerDoneSemaphore.Release();
        }
    }
}