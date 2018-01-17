using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WGSTS.Logger;

namespace Core.Base
{
    internal static class ActionDispetcherClass
    {

        internal static event Action<SandboxDataValue> OnAction;
        internal static ILogger Logger { get; set; } = new DummyLogger();

        internal static bool Start()
        {
            Logger.Trace("Start() Start");

            Logger.Trace("End Start");
            return true;
        }

        internal static bool Stop()
        {
            Logger.Trace("Start Stop");

            Logger.Trace("End Stop");
            return true;
        }

        internal static Dictionary<string, ActionConfig> ActionConnections { get; set; } = new Dictionary<string, ActionConfig>();

        

        internal static void Action(SandboxDataValue value)
        {
            Logger.Trace("Start  Action(SandboxDataValue ", value, ")");
            var data = value;
            if (ActionConnections.TryGetValue($"{data.From}.{data.EventName}", out ActionConfig info))
            {
                data.To = info.Events;
                data.Controled = info.Controled;

            }
            try
            {
                Logger.Trace("Start  Action invoke", data.ToJson());

                Task.Run(() => OnAction?.Invoke(data));
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }

            Logger.Trace("End  Action(SandboxDataValue value, string eventName)");

        }
    }
}