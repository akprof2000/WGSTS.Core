using System;

namespace Core.Interfaces
{
    public enum StatusSubstance
    {
        Ok,
        Error,
        Waiting,
        None
    }
    public interface ISubstance
    {
        #region ISubstance
        StatusSubstance Status { get; set; }
        bool Controled { get; set; }
        int Id { get; set; }
        Guid Index { get; set; }
        Guid GUID { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        DateTime Creation { get; set; }
        #endregion ISubstance
    }

}
