using Core.Interfaces;
using System;
using System.Collections.Generic;
using WGSTS.Logger;

namespace Core.Base
{
    public class CoreConfig
    {
        public string SubstancePath { get; set; } = "substance";
        public LogLevel LogLevel { get; set; } = LogLevel.Debug;
        public int LogFileCount { get; set; } = 10;
        public int LogFileSize { get; set; } = 1024 * 1024 * 10;
        public string PluginsPath { get; set; } = "plugins";
        public Dictionary<string, BaseCoreConfiguration> Plugins { get; set; } = new Dictionary<string, BaseCoreConfiguration>();
        public Dictionary<string, ActionConfig> ActionConnections { get; set; } = new Dictionary<string, ActionConfig>();
    }
    
    public class ActionConfig
    {
        public bool Controled { get; set; }
        public Dictionary<Guid, string> Events { get; set; }
    }
    
}