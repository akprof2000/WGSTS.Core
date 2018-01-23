using Core.Base;
using InsideAppWatcher;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using WGSTS.Logger;

namespace Core.WebServer
{
    public class Program
    {
        static AutoResetEvent _mrs = new AutoResetEvent(true);
        private static bool _isSecond;

        public static void Main(string[] args)
        {
            _mrs.Reset();
            Console.CancelKeyPress += console_CancelKeyPress;

            SendReceiveAndStartApp.RevercePort();

            SendReceiveAndStartApp.Sart();

            Console.WriteLine("Start Core, for Exit press: CTRL+C");

#if DEBUG
            Logger.ConsoleLevel = LogLevel.Trace;
#else
            Logger.ConsoleLevel = LogLevel.Warn;
#endif


            Logger.FileFullName = "Combine.log";
            CoreDispetcher.Logger = Logger.GetLogger("Core.log");
            
            CoreDispetcher.OnNeedClose += coreDispetcher_OnNeedClose;
            CoreDispetcher.Start();

            var data = BuildWebHost(args);

            data.StartAsync();

            _mrs.WaitOne();
            
            var w = data.StopAsync();
            w.Wait(60000);

            SendReceiveAndStartApp.Stop();
            CoreDispetcher.Stop();
            Logger.Flush();

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

            if (_isSecond)
            {
                Task.Run(() => { CoreDispetcher.Stop(); SendReceiveAndStartApp.Stop(); });

                Thread.Sleep(3000);
                Environment.Exit(-1);
            }
            _isSecond = true;

        }
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
