using Core.Substance;
using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Tasks;
using WGSTS.Logger;

namespace Core.Base
{
    public static class Sandbox
    {

        public static ILogger Logger { get; set; } = new DummyLogger();

        private static Task _task;
        static private ConcurrentQueue<SandboxDataValue> ExchangeData { get; set; }
        static private ConcurrentDictionary<Type, MethodInfo> _listDict { get; set; } = new ConcurrentDictionary<Type, MethodInfo>();
        static Sandbox()
        {

            Logger.Info("Start Sandbox");
            Logger.Trace("Start constructor Sandbox()");
            ExchangeData = new ConcurrentQueue<SandboxDataValue>();
            Logger.Trace("End constructor Sandbox()");
        }



        public static bool AddAction(SandboxDataValue value)
        {
            Logger.Trace("Start AddAction(SandboxData action)");
            Logger.Debug("Action is", value.ToJson());
            if (value != null)
                ExchangeData.Enqueue(value);
            Logger.Debug("Start ActionsStart()");


            actionsStart();
            Logger.Trace("End AddAction(SandboxData action)");
            return true;
        }

        private static void actionsStart()
        {
            Logger.Trace("Start ActionsStart()");
            try
            {
                Logger.Trace("Exchange data is empty", ExchangeData.IsEmpty);
                if (!ExchangeData.IsEmpty)
                    if (_task == null || (_task.IsCompleted && _task.Status != TaskStatus.Running && _task.Status != TaskStatus.WaitingToRun && _task.Status != TaskStatus.WaitingForActivation))
                    {
                        Logger.Trace("Need run task");
                        if (_task != null)
                        {
                            _task.Dispose();
                            Logger.Trace("task dispose");
                        }

                        _task = new Task(actionsStartAsync);
                        Logger.Trace("task start");
                        _task.Start();
                    }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            Logger.Trace("Exit ActionsStart()");
        }

        private static void actionsStartAsync()
        {
            Logger.Trace("Start ActionsStartAsync()");
            try
            {

                while (ExchangeData.TryDequeue(out SandboxDataValue ssd))
                {
                    Logger.Trace("Dequeue", ssd.TheType);

                    
                    if (ssd.Value is StorageData)
                        DataDispetcherClass.Execution(ssd.Value as StorageData);

                    Logger.Debug("Start Action", ssd.TheType);
                    ActionDispetcherClass.Action(ssd);

                    
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            Logger.Trace("Exit ActionsStartAsync()");

        }
    }
}