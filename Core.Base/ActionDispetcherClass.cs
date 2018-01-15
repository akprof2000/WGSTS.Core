using System;
using WGSTS.Logger;

namespace Core.Base
{
    public class ActionDispetcherClass
    {
        public static ILogger Logger { get; internal set; }

        internal bool Start()
        {
            return true;
        }

        internal bool Stop()
        {
            return true;
        }
    }
}