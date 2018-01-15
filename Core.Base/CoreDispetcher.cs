using System;
using System.IO;
using WGSTS.Logger;

namespace Core.Base
{
    public class CoreDispetcher
    {
        private static CoreLoader _loader;

        private static PluginDispetcherClass PluginDispetcher { get; }
        private static DataDispetcherClass DataDispetcher { get; }
        private static ActionDispetcherClass ActionDispetcher { get; }

        public static ILogger Logger { get; set; } = new DummyLogger();


        static CoreDispetcher()
        {
            AppDomain.CurrentDomain.UnhandledException += currentDomain_UnhandledException;


            PluginDispetcher = new PluginDispetcherClass()
            {

            };

            DataDispetcher = new DataDispetcherClass()
            {

            };

            ActionDispetcher = new ActionDispetcherClass()
            {

            };

        }

        private static void currentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {

            Exception e = (Exception)args.ExceptionObject;
            Logger.Fatal(e);
            Logger.Info("Runtime terminating:", args.IsTerminating);
        }

        public static bool Start(bool repeat = false)
        {
            Logger.Info("Start core");
            Stop();
            var run = false;
            try
            {
                _loader = new CoreLoader(Logger);
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

        private static bool internalStart()
        {
            Logger.Trace("start InternalStart");
            var ret = false;
            ConfigDispetcherClass.Logger = Logger;
            DataDispetcherClass.Logger = Logger;
            ActionDispetcherClass.Logger = Logger;
            PluginDispetcherClass.Logger = Logger;
            ret = ConfigDispetcherClass.Start();

            if (DataDispetcher != null && ret)
                ret = DataDispetcher.Start();

            if (ActionDispetcher != null && ret)
                ret = ActionDispetcher.Start();

            if (PluginDispetcher != null && ret)
                ret = PluginDispetcher.Start();


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

            if (PluginDispetcher != null)
                ret = PluginDispetcher.Stop();

            if (DataDispetcher != null)
                ret = DataDispetcher.Stop();

            if (ActionDispetcher != null)
                ret = ActionDispetcher.Stop();


            Logger.Debug("end InternalStop", ret);
            return ret;
        }
    }
}
