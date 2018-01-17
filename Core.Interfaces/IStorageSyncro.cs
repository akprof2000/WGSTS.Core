using System;

namespace Core.Interfaces
{
    public interface IStorageSyncro
    {
        bool Init(string config);
        bool ChangeConfig(string config);
        bool IsRestart();
        bool Start();
        bool Stop();
        bool Execution(ISubstance value);

        event Action OnNeedRestart;
        event Action OnForceRestart;
    }
}
