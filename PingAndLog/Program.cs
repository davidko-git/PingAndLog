using System;
using System.Net;

namespace PingAndLog
{
    public class Tests
    {
        public void Test1()
        {
            IPAddress[] addresses = new IPAddress[]
            {
                new IPAddress(new byte[] { 127, 0, 0, 1 }),
                new IPAddress(new byte[] { 128, 0, 0, 1 }),
                new IPAddress(new byte[] { 192, 168, 0, 1 }),
                new IPAddress(new byte[] { 192, 168, 0, 2 }),
                new IPAddress(new byte[] { 192, 168, 0, 3 })
            };
            
            int pingsPerAddress = 1;
            int testAddressesDuplCount = 1;
            IPAddress[] testAdresses = new IPAddress[testAddressesDuplCount * addresses.Length];
            
            for (int i = 0; i < testAdresses.Length; i++)
            {
                testAdresses[i] = new IPAddress(addresses[i%addresses.Length].GetAddressBytes());
            }
            
            PingDirector director = new PingDirector(testAdresses, pingsPerAddress);

            director.Start();
            Console.WriteLine("Test1 finish");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Tests tests = new Tests();
            tests.Test1();
        }
    }
}
