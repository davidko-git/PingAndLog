using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace PingAndLog
{
    public static class Utilities
    {
        public static readonly Dictionary<IPStatus, Byte[]> IPStatusEncodedDict = new Dictionary<IPStatus, Byte[]>();

        static Utilities()
        {
            string value = Constants.StatusOpenString + IPStatus.DestinationUnreachable + " or " + IPStatus.DestinationProhibited + Constants.StatusCloseString;
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
    }
}