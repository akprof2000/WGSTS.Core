using Core.Base;
using InsideAppWatcher;
using System;
using System.Threading;
using WGSTS.Logger;

namespace Core
{
    class Program
    {
        static ManualResetEvent _mrs = new ManualResetEvent(false);

        public static void Main(string[] args)
        {
            Console.CancelKeyPress += console_CancelKeyPress;
            _mrs.Reset();

            SendReceiveAndStartApp.RevercePort();
            
            SendReceiveAndStartApp.Sart();

            Console.WriteLine("Start Core, for Exit press: CTRL+C");
            Logger.FileFullName = "Combine.log";
            CoreDispetcher.Logger = Logger.GetLogger("Core.log");
            CoreDispetcher.OnNeedClose += coreDispetcher_OnNeedClose;
            CoreDispetcher.Start();

            _mrs.WaitOne();
            SendReceiveAndStartApp.Stop();
            CoreDispetcher.Stop();            
            Thread.Sleep(1000);
        }

        private static void coreDispetcher_OnNeedClose()
        {
            _mrs.Set();
            Console.WriteLine("Begin exit by event");
        }

        private static void console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            _mrs.Set();
            Console.WriteLine("Begin exit Core");
            e.Cancel = true;
        }
    }
}
