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
            CoreDispetcher.Logger = Logger.GetLogger();
            CoreDispetcher.Start();
            Console.ReadLine();
            CoreDispetcher.Stop();
        }
    }
}
