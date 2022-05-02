using System;
using System.Collections.Generic;
using System.Net;

namespace PingAndLog
{
    class Program
    {
        static void Main(string[] args)
        {
            int pingsPerAddress;
            IPAddress[] targetAddresses;
            
            if (args[0] == "test")
            {
                targetAddresses = Constants.testIPAddresses;
                pingsPerAddress = Constants.testPingsPerAddressCount;
            }
            else
            {
                int runnigTime = Int32.Parse(args[0]); 
                pingsPerAddress = runnigTime * 10;
                targetAddresses = new IPAddress[args.Length - 1];
            
                for (int i = 1; i < args.Length; i++)
                {
                    targetAddresses[i - 1] = IPAddress.Parse(args[i]);
                }
            }
            
            PingDirector director = new PingDirector(targetAddresses,
                pingsPerAddress,
                Constants.DefaultOutputFileName);
            director.Start();
            Console.WriteLine("Pinging done");
            Dictionary<string, int> successfulPingsCounts = Utilities.GetSuccessfulPingsCounts(Constants.DefaultOutputFileName);
            Utilities.PrintSuccessfulPingsRates(successfulPingsCounts, pingsPerAddress) ;
        }
    }
}
