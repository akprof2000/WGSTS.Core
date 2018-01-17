using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using WGSTS.Logger;

namespace Core.Base
{
    internal class CoreLoader
    {
        internal event Action OnNeedRestart;
        static private string _path = AppDomain.CurrentDomain.BaseDirectory;
        static private string _fileconfig;
        private CoreConfig _coreConfig = null;
        const string defaultpathconfig = @"Default";
        const string baseconfigname = @"configmain.json";

        const string pathconfig = @"Config";
        public static ILogger Logger { get; set; } = new DummyLogger();

        internal static string ConfigPath()
        {
            return _fileconfig;
        }

        public CoreLoader(ILogger logger)
        {
            Logger = logger;
            Logger.Trace("Start constructor CoreLoader");
            Logger.Info("Path of execution", _path);
            _fileconfig = Path.Combine(_path, pathconfig, baseconfigname);

            if (File.Exists(_fileconfig))
            {
                _coreConfig = (File.ReadAllText(_fileconfig)).FromJson<CoreConfig>();
                Logger.Info("File with config", _fileconfig);
            }

            if (_coreConfig == null)
            {
                Logger.Warning("File not load with base path", _fileconfig);
                _fileconfig = Path.Combine(_path, defaultpathconfig, baseconfigname);


                if (File.Exists(_fileconfig))
                {
                    _coreConfig = (File.ReadAllText(_fileconfig)).FromJson<CoreConfig>();
                    Logger.Info("File with config", _fileconfig);
                }
            }

            if (_coreConfig == null)
            {
                Logger.Fatal("File not load with base path", _fileconfig);
            }
            else
            {
                Logger.Debug(_coreConfig.ToJson());
                PluginDispetcherClass.Init(_coreConfig.Plugins, _coreConfig.PluginsPath, pathconfig);
                ActionDispetcherClass.ActionConnections = _coreConfig.ActionConnections;
            }
            ConfigDispetcherClass.OnChangeSettings += configDispetcherClass_OnChangeSettings;


            Logger.Debug("End constructor CoreLoader");
        }

        private void configDispetcherClass_OnChangeSettings(HashSet<string> list, string from, string to)
        {
            Logger.Trace("Start configDispetcherClass_OnChangeSettings(", list, ")");
            if (list.Contains(baseconfigname))
            {
                Logger.Info("Change find in  ", baseconfigname, "need restart");
                if (File.Exists(Path.Combine(from, baseconfigname)))
                    File.Copy(Path.Combine(from, baseconfigname), Path.Combine(to, baseconfigname), true);

                OnNeedRestart?.Invoke();
            }
            Logger.Trace("End configDispetcherClass_OnChangeSettings(HashSet<string> list)");
        }

        internal static void RemoveConfig()
        {
            Logger.Trace("start RemoveConfig");
            var path = new DirectoryInfo(Path.Combine(_path, pathconfig));
            Logger.Debug("start Remove", path.ToJson());
            if (path.Exists)
            {
                foreach (var item in path.GetFiles())
                {
                    Logger.Info("Remove", item.ToJson());
                    item.Delete();
                }
            }
            Logger.Trace("end RemoveConfig");
        }

        internal void EmptySettings()
        {
            Logger.Trace("start RemoveConfig");
            RemoveConfig();
            Logger.Trace("end RemoveConfig");
        }

        internal bool Init()
        {
            Logger.Trace("start Init");
            var info = false;


            info = loadPluginSubstance(_coreConfig.SubstancePath);


            Logger.Trace("end Init");
            return info;
        }



        private bool loadPluginSubstance(string substancePath)
        {
            Logger.Trace("start loadPluginSubstance by path", substancePath);
            if (string.IsNullOrEmpty(substancePath))
                return false;
            var path = Path.Combine(_path, substancePath);

            Directory.CreateDirectory(path);


            var ret = true;
            var files = Directory.GetFiles(path);
            foreach (var item in files)
            {
                if (File.Exists(item))
                {
                    try
                    {
                        Logger.Debug("Plugin path", item);

                        Assembly currentAssembly = null;
                        try
                        {
                            var name = AssemblyName.GetAssemblyName(item);
                            currentAssembly = Assembly.Load(name);
                            Logger.Debug("load assembly ", name, currentAssembly.ToJson());
                        }
                        catch (Exception ex)
                        {
                            Logger.Warning(ex);
                            continue;
                        }

                    }
                    catch (Exception ex)
                    {
                        Logger.Fatal(ex);
                    }
                }
            }
            Logger.Trace("End loadPluginSubstance by path ", substancePath);
            return ret;
        }
    }
}