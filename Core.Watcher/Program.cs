using InsideAppWatcher;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Watcher
{
    class Program
    {
        static AutoResetEvent _mrs = new AutoResetEvent(true);
        private static bool _isSecond;

        public static void Main(string[] args)
        {
            Console.CancelKeyPress += console_CancelKeyPress;
            _mrs.Reset();
            SendReceiveAndStartApp.Sart();
            Console.WriteLine("Start Core.Watcher, for Exit press: CTRL+C");
            _mrs.WaitOne();

            SendReceiveAndStartApp.Stop();
            Thread.Sleep(1000);
        }

        private static void console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            _mrs.Set();
            Console.WriteLine("Begin exit Core.Watcher");
            e.Cancel = true;

            if (_isSecond)
            {
                Task.Run(() => { SendReceiveAndStartApp.Stop(); });
                Thread.Sleep(3000);
                Environment.Exit(-1);
            }
            _isSecond = true;
        }
    }
}
