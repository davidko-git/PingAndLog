using System;
using System.Collections.Concurrent;
using System.IO;

namespace PingAndLog
{
    class PingLogger
    {
        public ConcurrentQueue<string> queue;

        public int logging;

        public PingLogger()
        {
            this.logging = 0;
            this.queue = new ConcurrentQueue<string>();
        }

        public void Start()
        {
            this.logging = 1;
            string currentItem;
            int debug_counter = 0;

            using (StreamWriter outputFile = new StreamWriter("output.xml", false)) 
            {
                outputFile.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
                outputFile.WriteLine("<results>");

                while (this.logging == 1)
                {
                    debug_counter += 1;
                    if (debug_counter % 50000 == 0)
                    {
                        ;
                    }
                    
                    if (queue.TryDequeue(out currentItem))
                    {
                        outputFile.WriteLine(currentItem);
                    }
                }

                outputFile.WriteLine("</results>");
            }

            this.LoggingProcessed.Invoke(this, null);
        }

        public event EventHandler LoggingProcessed;
    }
}