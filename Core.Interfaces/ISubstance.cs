using System;

namespace Core.Interfaces
{

    public enum StorageActionType
    {
        None = 0,
        Add = 1,
        Update = 2,
        Delete = 3,
        Set = 4,
        SingleSet = 5,
        Query = 6
    }
    public enum StatusSubstance
    {
        Ok = 0,
        Error = 1,
        Waiting = 2,
        None = 3
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
        StorageActionType StorageAction { get; set; }
        #endregion ISubstance
    }

}
