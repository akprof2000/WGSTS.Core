using System;

namespace Core.Interfaces
{
    public interface IConfiguration
    {
        string Name { get; }
        string Description { get; }
        Guid Id { get; }
        bool Init(BaseCoreConfiguration config);
        bool IsRestart();
        bool Start();
        bool Stop();
        event Action OnNeedRestart;
        event Action OnForceRestart;
    }

    public delegate bool EventDelegate(ISubstance value);
    public delegate ISubstance ActionDelegateWait(ISubstance value, Guid self, string eventName);
    public delegate ISubstance ActionDelegate(ISubstance value, Guid self, string eventName);
    public delegate void ActionCallBackDelegate(ISubstance value, Guid self, string eventName, EventDelegate method);
}
