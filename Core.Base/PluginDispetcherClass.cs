using Core.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using WGSTS.Logger;

namespace Core.Base
{
    internal static class PluginDispetcherClass
    {
        public static ILogger Logger { get; internal set; }
        static private string _path = AppDomain.CurrentDomain.BaseDirectory;
        private static object _pathconfig;
        private static string _pathplugin;

        internal static bool Start()
        {
            Logger.Trace("Start() Start");
            var ret = true;
            if (_plugins != null)
            {
                foreach (var item in _plugins.Values)
                {
                    if (!item.Start())
                    {
                        Logger.Trace("Item Start false", item.ToJson());
                        ret = false;
                    }
                    Thread.Sleep(300);
                }
            }
            Logger.Trace("End Start");
            return ret;
        }

        internal static bool Stop()
        {
            Logger.Trace("Start Stop");
            if (_plugins != null)
            {
                foreach (var item in _plugins.Values)
                {
                    item.Stop();
                }
            }
            Logger.Trace("End Stop");
            return true;
        }

        internal static bool Action(SandboxDataValue data)
        {
            var ret = false;
            Logger.Trace("Start  Action<T>(T action)");
            
            var val = data.Value;
            foreach (var item in data.To)
            {
                if (_plugins.TryGetValue(item.Key, out IConfiguration pl))
                {
                    if (!_methods.TryGetValue(pl, out ConcurrentDictionary<Type, MethodInfo> mt))
                    {
                        mt = new ConcurrentDictionary<Type, MethodInfo>();
                        _methods[pl] = mt;
                    }

                    if (!mt.TryGetValue(data.TheType, out MethodInfo method))
                    {
                        method = pl.GetType().GetMethod("Action").MakeGenericMethod(new Type[] { data.TheType });
                        mt[data.TheType] = method;

                    }
                    Logger.Debug("run action", data.TheType);
                    var exec = new object[] { val, item.Value };
                    try
                    {
                        ret = (bool)method.Invoke(pl, exec);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                        ret = false;
                    }
                    val = exec[0] as ISubstance;

                }
            }
            data.Value = val;
            if (!data.Controled)
            {
                if (_currentDeligate.TryGetValue(data, out EventDelegate onDelegate))
                    onDelegate?.Invoke(val);
                else if (_currentWait.TryGetValue(data, out ManualResetEvent onEvent))
                {
                    try
                    {
                        onEvent?.Set();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                }
            }

            
            Logger.Trace("End  Action<T>(T action)");
            return ret;
        }

        internal static bool IsRestart()
        {
            Logger.Trace("Start IsRestart");
            if (_plugins != null)
            {
                foreach (var item in _plugins.Values)
                {
                    if (!item.IsRestart())
                    {
                        Logger.Trace("End IsRestart false");
                        return false;
                    }
                }
            }
            Logger.Trace("End IsRestart true");
            return true;
        }


        private static ConcurrentDictionary<Guid, IConfiguration> _plugins = new ConcurrentDictionary<Guid, IConfiguration>();
        private static ConcurrentDictionary<IConfiguration, ConcurrentDictionary<Type, MethodInfo>> _methods = new ConcurrentDictionary<IConfiguration, ConcurrentDictionary<Type, MethodInfo>>();
        private static ConcurrentDictionary<object, ManualResetEvent> _currentWait = new ConcurrentDictionary<object, ManualResetEvent>();
        private static ConcurrentDictionary<object, EventDelegate> _currentDeligate = new ConcurrentDictionary<object, EventDelegate>();


        internal static void Init(Dictionary<string, BaseCoreConfiguration> plugins, string pluginsPath, string configPath)
        {
            Logger.Info("Path of execution", _path);
            _pathconfig = Path.Combine(_path, configPath);
            _pathplugin = Path.Combine(_path, pluginsPath);

            Logger.Debug(plugins.ToJson());
            foreach (var item in plugins.Values)
            {
                var ret = loadPlugin(item, out IConfiguration data);
                if (!ret)
                {
                    Logger.Error($"{item.FileName} not loaded");
                    break;
                }
                _plugins[data.Id] = data;
            }

            Logger.Debug("End Init ManageServiceStarter");
            
        }

        private static bool loadPlugin(BaseCoreConfiguration config, out IConfiguration plugin)
        {
            Logger.Trace("start loadPlugin with param", config.ToJson());
            plugin = null;
            var ret = false;
            if (config != null)
            {
                var path = Path.Combine(_pathplugin, config.FileName);
                Logger.Debug("Plugin path", path);
                if (File.Exists(path))
                {
                    var name = AssemblyName.GetAssemblyName(path);
                    var currentAssembly = Assembly.Load(name);

                    var type = currentAssembly.GetType($"{config.ClassName}", false, true);
                    if (type == null)
                        Logger.Error("Can not find type", config.ClassName, currentAssembly.ToJson());
                    else
                    {
                        try
                        {
                            plugin = Activator.CreateInstance(type) as IConfiguration;
                            plugin.OnNeedRestart += plugin_OnNeedRestart;
                            plugin.OnForceRestart += plugin_OnForceRestart;

                            plugin.Init(config);
                            
                        }
                        catch (Exception ex)
                        {
                            Logger.Fatal(ex);
                        }
                    }
                }
                else
                    Logger.Error("File not found", path);
            }
            Logger.Debug(plugin.ToJson());
            ret = plugin != null;
            Logger.Info("end loadPlugin with", ret);
            return ret;
        }

        internal static event Action OnNeedRestart;
        internal static event Action OnForceRestart;

        private static void plugin_OnForceRestart()
        {
            Logger.Trace("Start Plugin_onNeedRestart");
            OnForceRestart?.Invoke();
            Logger.Trace("End Plugin_onNeedRestart");
        }

        private static void plugin_OnNeedRestart()
        {
            Logger.Trace("Start Plugin_onNeedRestart");
            OnNeedRestart?.Invoke();
            Logger.Trace("End Plugin_onNeedRestart");
        }
    }
}