using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace PingAndLog
{
    public static class Utilities
    {
        public static readonly Dictionary<IPStatus, Byte[]> IPStatusEncodedDict = new Dictionary<IPStatus, Byte[]>();

        static Utilities()
        {
            string value = Constants.StatusOpenString + IPStatus.DestinationUnreachable + " or " +
                           IPStatus.DestinationProhibited + Constants.StatusCloseString;
            IPStatusEncodedDict.Add(IPStatus.DestinationUnreachable, Encoding.UTF8.GetBytes(value));

            foreach (IPStatus status in Enum.GetValues<IPStatus>())
            {
                if (status != IPStatus.DestinationUnreachable && status != IPStatus.DestinationProhibited)
                {
                    value = Constants.StatusOpenString + status + Constants.StatusCloseString;
                    IPStatusEncodedDict.Add(status, Encoding.UTF8.GetBytes(value));
                }
            }
        }

        public static Dictionary<string, int> GetSuccessfulPingsCounts(string inputUri)
        {
            Dictionary<string, int> successfulPingsCount = new Dictionary<string, int>();

            using (XmlReader reader = XmlReader.Create(inputUri))
            {
                reader.MoveToContent();
                Console.WriteLine("Processing pings reply records...");

                while (!reader.EOF)
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "PingReplyRecord")
                    {
                        XElement pingReplyRecord = XElement.ReadFrom(reader) as XElement;
                        string currentIPAddress = pingReplyRecord.Element("TargetAddress").Value;

                        if (!successfulPingsCount.ContainsKey(currentIPAddress))
                        {
                            successfulPingsCount[currentIPAddress] = 0;
                        }

                        if (pingReplyRecord.Elements("Status").Any()
                            && pingReplyRecord.Element("Status").Value == "Success")
                        {
                            successfulPingsCount[currentIPAddress] += 1;
                        }
                    }
                    else
                    {
                        reader.Read();
                    }
                }
                
                Console.WriteLine("Pings reply records processed");
            }

            return successfulPingsCount;
        }

        public static void PrintSuccessfulPingsRates(Dictionary<string, int> successfulPingsCount, int totalPingsCount)
        {
            string header = $"{"IP Address".PadLeft(40)}   {"Success rate"}";
            Console.WriteLine();
            Console.WriteLine(header);
            Console.WriteLine(new String(' ', 30) + "----------- -------------");
            Console.WriteLine();
            
            foreach (KeyValuePair<string, int> entry in successfulPingsCount)
            {
                double successRate = ((double)entry.Value / totalPingsCount) * 100;
                Console.WriteLine($"{entry.Key.PadLeft(40)}   {successRate:0.00}%");
            }
            
            Console.WriteLine();
        }
    }
}