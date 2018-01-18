using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace InsideAppWatcher
{
    public static class SendReceiveAndStartApp
    {
        private static Thread _thread;
        private static bool _break;

        public static int Port { get; set; } = 33333;
        public static int SelfPort { get; set; } = 22222;

        public static string CoreStartDll { get; set; } = "Core.dll";


        public static void RevercePort()
        {
            var port = SelfPort;
            SelfPort = Port;
            Port = port;
            CoreStartDll = "Core.Watcher.dll";
        }
        public static void Sart()
        {
            Stop();
            _break = false;
            _thread = new Thread(process);
            _thread.Start();

            new Thread(() => server()).Start();
        }

        public static void Stop()
        {
            _break = true;
            if (_thread != null && _thread.IsAlive)
                _thread.Join();
            _thread = null;

            sendRequest(SelfPort);
        }


        static void server()
        {
            try
            {
                var tcpListener = new TcpListener(IPAddress.Loopback, SelfPort);

                tcpListener.Start();
                for (; ; )
                {
                    using (var tcpClient = tcpListener.AcceptTcpClient())
                    {
                        var stream = tcpClient.GetStream();
                        var bytes = new byte[3];
                        stream.Read(bytes, 0, 3);
                        if (bytes[0] == 0 && bytes[1] == 1 && bytes[2] == 2)
                        {
                            stream.Write(new byte[] { 0x02, 0x01, 0x00 }, 0, 3);
                            if (_break)
                                break;
                        }
                        stream.Close();
                        tcpClient.Close();
                    }
                }

                tcpListener.Stop();
            }
            catch
            {
                Environment.Exit(-1);
            }
        }

        private static void process(object obj)
        {
            for (; ; )
            {
                if (_break)
                {
                    break;
                }

                for (int i = 0; i < 10; i++)
                {
                    Thread.Sleep(1000);
                    if (_break)
                    {
                        break;
                    }
                }

                if (!sendRequest(Port))
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            Arguments = CoreStartDll,
                            FileName = "dotnet",
                            WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
                        }).Dispose();
                    }
                    catch
                    { }
                }




            }
        }

        private static bool sendRequest(int port)
        {

            try
            {

                using (TcpClient tcpClient = new TcpClient())
                {
                    try
                    {
                        tcpClient.Connect(IPAddress.Loopback, port);
                        var stream = tcpClient.GetStream();
                        stream.Write(new byte[] { 0x00, 0x01, 0x02 }, 0, 3);
                        stream.Flush();
                        Thread.Sleep(10);
                        var bytes = new byte[3];
                        stream.Read(bytes, 0, 3);
                        stream.Close();
                        tcpClient.Close();
                        if (bytes[0] == 2 && bytes[1] == 1 && bytes[2] == 0)
                        {

                            return true;
                        }

                    }
                    catch (Exception)
                    {

                    }
                }

            }
            catch
            {

            }

            return false;

        }
    }
}
