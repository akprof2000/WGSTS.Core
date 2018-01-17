using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using WGSTS.Logger;

namespace Core.Base
{
    internal static class ConfigDispetcherClass
    {
        internal static event Action<HashSet<string>, string, string> OnChangeSettings;

        static private string _path = AppDomain.CurrentDomain.BaseDirectory;
        private static bool _break;
        private static Thread _thread;
        const string config = @"Config";
        const string loadconfig = @"ServerConfig";
        const string checkconfig = @"CheckConfig";
        const string checkconfigfile = @"HashFileWithConfig.json";

        public static ILogger Logger { get; set; } = new DummyLogger();

        private static Dictionary<string, byte[]> _listConfig = new Dictionary<string, byte[]>();
        private static FileSystemWatcher _watcher;
        private static FileSystemEventArgs _duplicateEvent = new FileSystemEventArgs(WatcherChangeTypes.All, "", "");
        private static bool _inCheck = false;

        static ConfigDispetcherClass()
        {
            Directory.CreateDirectory(Path.Combine(_path, config));
            Directory.CreateDirectory(Path.Combine(_path, loadconfig));
            Directory.CreateDirectory(Path.Combine(_path, checkconfig));
        }

        internal static bool Start()
        {
            Logger.Info("ConfigDispetcherClass start");
            _break = false;
            _thread = new Thread(processing);
            _thread.Start();
            var fn = Path.Combine(Path.Combine(_path, checkconfig), "HashFileWithConfig.json");
            if (File.Exists(fn))
            {
                Logger.Info("Hash config file Exists", fn);
                _listConfig = File.ReadAllText(fn).FromJson<Dictionary<string, byte[]>>();
                checkConfigData();
            }
            else
            {
                Logger.Info("Hash config file not Exists", fn);
                fillList();
            }

            WatchDir();
            Logger.Debug("ConfigDispetcherClass start true");
            return true;
        }


        private static void fillList()
        {
            Logger.Debug("fillList start");
            _listConfig.Clear();
            var path = Path.Combine(_path, loadconfig);
            var files = Directory.GetFiles(path);
            foreach (var item in files)
            {
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(item))
                    {
                        _listConfig[Path.GetFileName(item).ToLower()] = md5.ComputeHash(stream);
                    }
                }
            }
            var fn = Path.Combine(Path.Combine(_path, checkconfig), "HashFileWithConfig.json");
            File.WriteAllText(fn, _listConfig.ToJson());

            Logger.Trace("fillList start");
        }

        public static void WatchDir()
        {
            Logger.Debug("WatchDir start");
            _watcher = new FileSystemWatcher
            {
                Path = Path.Combine(_path, loadconfig),
                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName
            };

            // Add event handlers.
            _watcher.Changed += new FileSystemEventHandler(onChanged);
            _watcher.Created += new FileSystemEventHandler(onChanged);
            _watcher.Deleted += new FileSystemEventHandler(onChanged);

            // Begin watching.
            _watcher.EnableRaisingEvents = true;
            Logger.Trace("fillList start");
        }



        private static void onChanged(object sender, FileSystemEventArgs e)
        {
            Logger.Debug("onChanged(object, ", sender, ", FileSystemEventArgs ", e, ")");
            if (string.Compare(e.Name, _duplicateEvent.Name) == 0 && (_duplicateEvent.ChangeType == e.ChangeType || e.ChangeType == WatcherChangeTypes.Created))
            {
                Logger.Info("skip duplicate event");
                return;
            }

            _duplicateEvent = new FileSystemEventArgs(WatcherChangeTypes.Changed, e.FullPath, e.Name);



            checkConfigData();
            Logger.Trace("onChanged(object sender, FileSystemEventArgs e) exit");
        }


        private static void checkConfigData()
        {
            if (_inCheck)
            {
                Logger.Debug("Skip by change");
                return;
            }

            _inCheck = true;
            Logger.Debug("checkConfigData start");
            var path = Path.Combine(_path, loadconfig);
            var upath = Path.Combine(_path, config);
            var files = Directory.GetFiles(path);
            var ok = true;
            var changes = new HashSet<string>();

            

            foreach (var item in files)
            {
                if (!_listConfig.ContainsKey(Path.GetFileName(item).ToLower()))
                {
                    Logger.Info("file add", Path.GetFileName(item));
                    changes.Add(Path.GetFileName(item).ToLower());
                    ok = false;
                    continue;
                }
                

                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(item))
                    {
                        Logger.Debug("Check change file", Path.GetFileName(item));
                        if (!_listConfig[Path.GetFileName(item).ToLower()].SequenceEqual(md5.ComputeHash(stream)))
                        {
                            ok = false;
                            Logger.Info("file change", Path.GetFileName(item));
                            changes.Add(Path.GetFileName(item).ToLower());
                        }
                    }
                }
            }

            foreach (var item in _listConfig.Keys)
            {
                var data = files.Where(a => Path.GetFileName(a).ToLower() == item).FirstOrDefault();
                if (data == null)
                {
                    ok = false;
                    Logger.Info("file delete", item);
                    changes.Add(item);
                }
            }


            if (!ok)
            {
                Logger.Warning("Some changed refill settings");
                Thread.Sleep(3000);
                fillList();            
                OnChangeSettings?.Invoke(changes, path, upath);
            }
            Logger.Debug("checkConfigData start");
            _inCheck = false;

        }

        private static void processing()
        {
            Logger.Info("processing() start");
            int ind = 0;
            for (; ; )
            {
                if (_break)
                {
                    Logger.Info("processing() break start");
                    break;
                }
                if (ind > 999)
                {
                    Logger.Debug("Timer set on check config");
                    checkConfigData();
                    ind = 0;
                }

                Thread.Sleep(1000);
            }
            Logger.Trace("processing() exit");
        }

        internal static bool Stop()
        {
            Logger.Info("Stop() start");
            _break = true;
            if (_watcher != null)
                _watcher.EnableRaisingEvents = false;
            _watcher = null;

            if (_thread != null && _thread.IsAlive)
            {
                Logger.Debug("Stop() wait thread ");
                _thread.Join();
            }

            Logger.Debug("Stop() stop");
            return true;
        }
    }
}