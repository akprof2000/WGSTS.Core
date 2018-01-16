using Core.Interfaces;
using Core.Substance;
using System;
using WGSTS.Logger;

namespace Core.Base
{
    internal static class DataDispetcherClass
    {
        internal static ILogger Logger { get; set; } = new DummyLogger();

        internal static bool Start()
        {
            return true;
        }

        internal static bool Stop()
        {
            return true;
        }

        internal static bool Execution(StorageData value)
        {

            return true;

        }

    }
}