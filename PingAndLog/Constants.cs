using System.Net;
using System.Text;

namespace PingAndLog
{
    public static class Constants
    {
        public static readonly string DefaultOutputFileName = "output.xml";
        
        public static readonly IPAddress[] testIPAddresses = new IPAddress[]
        {
            IPAddress.Parse("127.0.0.1"),
            IPAddress.Parse("128.0.0.1"),
            IPAddress.Parse("192.168.0.1"),
            IPAddress.Parse("192.168.0.2"),
            IPAddress.Parse("192.168.0.3"),
            IPAddress.Parse("192.168.0.100"),
            IPAddress.Parse("192.168.0.101"),
            IPAddress.Parse("192.168.0.102"),
            IPAddress.Parse("192.168.0.103"),
            IPAddress.Parse("192.168.0.104"),
            IPAddress.Parse("192.168.0.105")
        };

        public static int testPingsPerAddressCount = 1000;
        
        // XMLHeader
        public static readonly string XMLHeaderString = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>";
        public static readonly byte[] XMLHeader = Encoding.UTF8.GetBytes(XMLHeaderString);
        
        // ResultsOpen
        public static readonly string ResultsOpenString = "<Results xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:noNamespaceSchemaLocation=\"PingResultsSchema.xsd\">\n";
        public static readonly byte[] ResultsOpen = Encoding.UTF8.GetBytes(ResultsOpenString);
        
        // ResultsClose
        public static readonly string ResultsCloseString = "</Results>";
        public static readonly byte[] ResultsClose = Encoding.UTF8.GetBytes(ResultsCloseString);

        // PingReplyRecordOpen
        public static readonly string PingReplyRecordOpenString = "<PingReplyRecord>";
        public static readonly byte[] PingReplyRecordOpenEncoded = Encoding.UTF8.GetBytes(PingReplyRecordOpenString);

        // PingReplyRecordClose
        public static readonly string PingReplyRecordCloseString = "</PingReplyRecord>";
        public static readonly byte[] PingReplyRecordCloseEncoded = Encoding.UTF8.GetBytes(PingReplyRecordCloseString);

        // TargetAddressOpen
        public static readonly string TargetAddressOpenString = "<TargetAddress>";
        
        // TargetAddressClose
        public static readonly string TargetAddressCloseString = "</TargetAddress>";

        // StatusOpen
        public static readonly string StatusOpenString = "<Status>";

        // StatusClose
        public static readonly string StatusCloseString = "</Status>";

        // RoundtripTimeOpen
        public static readonly string RoundtripTimeOpenString = "<RoundtripTime>";
        public static readonly byte[] RoundtripTimeOpen = Encoding.UTF8.GetBytes(RoundtripTimeOpenString);

        // RoundtripTimeClose
        public static readonly string RoundtripTimeCloseString = "</RoundtripTime>";
        public static readonly byte[] RoundtripTimeClose = Encoding.UTF8.GetBytes(RoundtripTimeCloseString);
    }
}