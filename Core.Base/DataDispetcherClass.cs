using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Core.Interfaces;
using WGSTS.Logger;

namespace Core.Base
{
    internal static class DataDispetcherClass
    {
        static private string _path = AppDomain.CurrentDomain.BaseDirectory;
        internal static ILogger Logger { get; set; } = new DummyLogger();

        static IStorageSyncro _plugin;
        private static string _pathconfig;
        private static string _pathplugin;
        private static BaseCoreConfiguration _current;

        static DataDispetcherClass()
        {
            ConfigDispetcherClass.OnChangeSettings += configDispetcherClass_OnChangeSettings;
        }

        private static void configDispetcherClass_OnChangeSettings(HashSet<string> config, string from, string to)
        {
            Logger.Trace("Start configDispetcherClass_OnChangeSettings(", config, ")");
            if (_current == null)
            {
                Logger.Warning("End not configure configDispetcherClass_OnChangeSettings(HashSet<string> list)");
                return;
            }
            if (config.Contains(_current.ConfigName))
            {
                var item = _current.ConfigName;
                Logger.Info("Change find in  ", item, "need info plugin");
                if (File.Exists(Path.Combine(from, item)))
                    File.Copy(Path.Combine(from, item), Path.Combine(to, item), true);

                Logger.Info("Send info in plugin  ");

                var fn = Path.Combine(_pathconfig, item);
                string confdata = null;
                if (File.Exists(fn))
                    confdata = File.ReadAllText(fn);

                _plugin.ChangeConfig(confdata);
            }

            Logger.Trace("End configDispetcherClass_OnChangeSettings(HashSet<string> list)");
        }

        internal static bool Execution(SandboxDataValue value)
        {
            Logger.Trace("Start Execution(SandboxDataValue ",value,")");
            var ret =  _plugin.Execution(value.Value);
            Logger.Trace("End IsRestart()");
            return ret;
        }


        internal static bool Start()
        {
            Logger.Info("Start() Start");
            var ret = true;
            _plugin?.Start();

            Logger.Debug("End Start");
            return ret;
        }

        internal static bool Stop()
        {
            Logger.Info("Start Stop");
            _plugin?.Stop();

            Logger.Debug("End Stop");
            return true;
        }

        internal static void Init(BaseCoreConfiguration storeSyncroPlugin, string pluginsPath, string pathconfig)
        {
            Logger.Info("Path of execution", _path);
            _pathconfig = Path.Combine(_path, pathconfig);
            _pathplugin = Path.Combine(_path, pluginsPath);
            _current = storeSyncroPlugin;

            Logger.Debug(storeSyncroPlugin);
            var ret = loadPlugin(storeSyncroPlugin, out IStorageSyncro data);
            if (!ret)
            {
                Logger.Error($"{storeSyncroPlugin?.FileName} not loaded");

            }
            else
            {
                _plugin = data;
                Logger.Debug("plugin load ", data);
            }

            Logger.Debug("End Init DataDispetcherClass");
        }


        private static bool loadPlugin(BaseCoreConfiguration config, out IStorageSyncro plugin)
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
                            plugin = Activator.CreateInstance(type) as IStorageSyncro;
                            plugin.OnForceRestart += plugin_OnForceRestart;
                            plugin.OnNeedRestart += plugin_OnNeedRestart;

                            var fn = Path.Combine(_pathconfig, config.FileName);
                            string confdata = null;
                            if (File.Exists(fn))
                                confdata = File.ReadAllText(fn);

                            plugin.Init(confdata);

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

        internal static bool IsRestart()
        {
            Logger.Trace("Start IsRestart()");
            if (_plugin == null)
                return false;
            var ret = _plugin.IsRestart();
            Logger.Trace("Stop IsRestart()");
            return ret;
        }
    }
}