using Core.Base;
using System;
using WGSTS.Logger;

namespace Core
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Start");
            Logger.FileFullName = "Combine.log";
            CoreDispetcher.Logger = Logger.GetLogger("Core.log");
            CoreDispetcher.Start();
            Console.ReadLine();
            CoreDispetcher.Stop();
        }
    }
}
