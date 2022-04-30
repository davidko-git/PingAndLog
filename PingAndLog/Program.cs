using System;
using System.Net;
//using JetBrains.Profiler.Api;

namespace PingAndLog
{
    public class Tests
    {
        public void Test1()
        {
            int pingsPerAdress = 1000;
            IPAddress[] testAdresses = new IPAddress[] {new IPAddress(new byte [] { 127, 0, 0, 1}),
                                                        new IPAddress(new byte [] { 127, 0, 0, 1}),
                                                        new IPAddress(new byte [] { 127, 0, 0, 1}), 
                                                        new IPAddress(new byte [] { 127, 0, 0, 1}),
                                                        new IPAddress(new byte [] { 127, 0, 0, 1}), 
                                                        new IPAddress(new byte [] { 128, 0, 0, 1}), 
                                                        new IPAddress(new byte [] { 128, 0, 0, 1}), 
                                                        new IPAddress(new byte [] { 128, 0, 0, 1}), 
                                                        new IPAddress(new byte [] { 128, 0, 0, 1}), 
                                                        new IPAddress(new byte [] { 192, 168, 0, 1})};
            PingDirector director = new PingDirector(testAdresses, pingsPerAdress);

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
