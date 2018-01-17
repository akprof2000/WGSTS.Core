using System;

namespace Core.Interfaces
{

    public interface IConfiguration
    {
        string Name { get; }
        string Description { get; }
        Guid Id { get; }
        bool Init(string config);
        bool ChangeConfig(string config);
        bool IsRestart();
        bool Start();
        bool Stop();

        event Action OnNeedRestart;
        event Action OnForceRestart;
        event ActionDelegate OnActionInvock;
        event ActionDelegateWait OnAction;
        event ActionCallBackDelegate OnActionCallBackInvock;
    }

    public delegate void EventDelegate(ISubstance value);

    public delegate ISubstance ActionDelegateWait(ISubstance value, Guid self, string eventName);
    public delegate void ActionDelegate(ISubstance value, Guid self, string eventName);
    public delegate void ActionCallBackDelegate(ISubstance value, Guid self, string eventName, EventDelegate method);
}
