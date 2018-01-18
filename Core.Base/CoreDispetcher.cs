using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WGSTS.Logger;

namespace Core.Base
{
    public class CoreDispetcher
    {
        private static CoreLoader _loader;

        public static ILogger Logger { get; set; } = new DummyLogger();


        static CoreDispetcher()
        {
            AppDomain.CurrentDomain.UnhandledException += currentDomain_UnhandledException;
            PluginDispetcherClass.OnForceRestart += pluginDispetcherClass_OnForceRestart;
            PluginDispetcherClass.OnNeedRestart += pluginDispetcherClass_OnNeedRestart;
            DataDispetcherClass.OnForceRestart += dataDispetcherClass_OnForceRestart;
            DataDispetcherClass.OnNeedRestart += dataDispetcherClass_OnNeedRestart;
            ActionDispetcherClass.OnAction += actionDispetcherClass_OnAction;
        }

        private static void dataDispetcherClass_OnNeedRestart()
        {
            Logger.Trace("Start dataDispetcherClass_OnNeedRestart");
            plugin_onNeedRestart();
            Logger.Trace("End dataDispetcherClass_OnNeedRestart");
        }

        private static void dataDispetcherClass_OnForceRestart()
        {
            Logger.Trace("Start dataDispetcherClass_OnForceRestart");
            shutdown_now();
            Logger.Trace("End dataDispetcherClass_OnForceRestart");
        }

        private static void actionDispetcherClass_OnAction(SandboxDataValue ssd)
        {
            Logger.Trace("Start actionDispetcherClass_OnAction(SandboxDataValue ", ssd, ")");
            PluginDispetcherClass.Action(ssd);
            Logger.Trace("End actionDispetcherClass_OnAction(SandboxDataValue ssd)");

        }

        private static void pluginDispetcherClass_OnNeedRestart()
        {
            Logger.Trace("Start pluginDispetcherClass_OnNeedRestart");
            plugin_onNeedRestart();
            Logger.Trace("End pluginDispetcherClass_OnNeedRestart");
        }

        private static void pluginDispetcherClass_OnForceRestart()
        {
            Logger.Trace("Start Plugin_onForceRestart");
            shutdown_now();
            Logger.Trace("End Plugin_onForceRestart");
        }

        private static void currentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {

            Exception e = (Exception)args.ExceptionObject;
            Logger.Fatal(e);
            Logger.Info("Runtime terminating:", args.IsTerminating);
            Logger.Flush();
            Task.Run(() => { Thread.Sleep(3000); Environment.Exit(-1); });
        }

        public static bool Start(bool repeat = false)
        {
            Logger.Info("Start core");
            Sandbox.Logger = Logger;
            ConfigDispetcherClass.Logger = Logger;
            DataDispetcherClass.Logger = Logger;
            ActionDispetcherClass.Logger = Logger;
            PluginDispetcherClass.Logger = Logger;

            Stop();
            var run = false;
            try
            {
                _loader = new CoreLoader(Logger);
                _loader.OnNeedRestart += _loader_OnNeedRestart;

                Logger.Debug("CoreLoader is been create");
                run = _loader.Init();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }
            if (!run && !repeat)
            {
                if (_loader == null)
                {
                    CoreLoader.RemoveConfig();
                }
                else
                {
                    _loader.EmptySettings();
                }
                Logger.Warning("reset by default settings restart");

                run = Start(true);
            }
            else if (!run)
            {
                Logger.Error("Fail by reset config");
            }

            if (run)
                run = internalStart();
            else
            {
                Logger.Error("Fail by start");
            }


            return run;
        }


        private static void _loader_OnNeedRestart()
        {
            Logger.Trace("Start _loader_OnNeedRestart");
            shutdown_now();
            Logger.Trace("End _loader_OnNeedRestart");
        }


        static void shutdown_now()
        {
            Logger.Info("Reset by 1 second");
            (new Timer(onShutdownTimer)).Change(3000, 0);
        }


        public static event Action OnNeedClose;
        private static void onShutdownTimer(object state)
        {
            Logger.Debug("Start onShutdownTimer");
            (state as Timer).Dispose();
            Task.Run(() => OnNeedClose?.Invoke());
            Logger.Trace("End onShutdownTimer");
        }

        static bool plugin_onNeedRestart()
        {
            Logger.Trace("Start Plugin_onNeedRestart");

            var ret = PluginDispetcherClass.IsRestart();
            ret &= DataDispetcherClass.IsRestart();
            if (ret)
            {
                Logger.Info("Reset by 1 second");
                (new Timer(onResetTimer)).Change(1000, 0);
            }
            else
            {
                Logger.Warning("Wait info reset by 3 second");
                (new Timer(onResetNeedTimer)).Change(3000, 0);
            }

            Logger.Trace("End Plugin_onNeedRestart", ret);
            return ret;
        }

        private static void onResetNeedTimer(object state)
        {
            Logger.Debug("Start onResetNeedTimer");
            (state as Timer).Dispose();
            plugin_onNeedRestart();
            Logger.Trace("End onResetNeedTimer");
        }

        private static void onResetTimer(object state)
        {
            Logger.Debug("Start onResetTimer");
            (state as Timer).Dispose();

            Stop();
            Start();
            Logger.Trace("End onResetTimer");
        }

        private static bool internalStart()
        {
            Logger.Trace("start InternalStart");
            var ret = false;


            ret = ConfigDispetcherClass.Start();
            ret &= PluginDispetcherClass.Start();
            ret &= DataDispetcherClass.Start();
            ret &= ActionDispetcherClass.Start();

            Logger.Debug("end InternalStart", ret);
            return ret;
        }

        public static bool Stop()
        {
            Logger.Info("Stop core");
            internalStop();
            _loader = null;
            Logger.Info("End Stop core");
            return true;
        }


        private static bool internalStop()
        {
            Logger.Trace("start InternalStop");
            var ret = false;

            ret = ConfigDispetcherClass.Stop();

            ret &= PluginDispetcherClass.Stop();

            ret &= DataDispetcherClass.Stop();


            ret &= ActionDispetcherClass.Stop();


            Logger.Debug("end InternalStop", ret);
            return ret;
        }
    }
}
