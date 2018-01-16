using System.Collections.Generic;

namespace Core.Interfaces
{
    public enum StorageAction
    {
        Add,
        Update,
        Delete,
        Set
    }
    public interface IStorageInterface: ISubstance
    {
        bool SetIndex<T>(string index) where T : new();
        bool Open(string filename);
        bool Add<T>(T value) where T : new();
        T Read<T>(Where where) where T : new();
        T Read<T, TParam>(string field, TParam value) where T : new();
        IEnumerable<T> ReadList<T>(Where where) where T : new();
        IEnumerable<T> ReadAll<T>() where T : new();
        bool Update<T>(T value) where T : new();
        bool Delete<T>(Where where) where T : new();
        bool Delete<T, TParam>(string field, TParam value) where T : new();
        bool Close();
    }
}
