using WGSTS.Logger;

namespace Core.Base
{
    public class CoreConfig
    {
        public string SubstancePath { get; set; } = "substance";
        public LogLevel LogLevel { get; set; } = LogLevel.Debug;
        public int LogFileCount { get; set; } = 10;
        public int LogFileSize { get; set; } = 1024 * 1024 * 10;
    }
}