using Core.Interfaces;
using System;
using System.Collections.Generic;

namespace Core.Substance
{
    public class StorageData : ISubstance
    {
        public StatusSubstance Status { get; set; } = StatusSubstance.Ok;
        private object _value;
        public bool Controled { get; set; }

        public string Description
        {
            get;
            set;
        } = "Action for storage operation";

        public Guid GUID
        {
            get;
            set;
        } = new Guid("7BB30CA4-261C-485D-BD7F-17962EBD67E7");


        public Guid Index
        {
            get;
            set;
        } = Guid.NewGuid();

        public string Name
        {
            get;
            set;
        } = "Storage Action";

        public Type TheType { get; set; }
        public void SetData<T>(T value) where T : ISubstance, new()
        {
            _value = value;
            TheType = typeof(T);
        }

        public T GetData<T>() where T : ISubstance, new()
        {
            if (_value is T)
            {
                return (T)_value;
            }
            else
            {
                return (T)Convert.ChangeType(_value, typeof(T));
            }
        }


        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public bool Result { get; set; }
        public StorageAction Action { get; set; } = StorageAction.Add;

        public DateTime Creation
        {
            get;
            set;
        } = DateTime.Now;

        public int Id
        {
            get;
            set;
        }

        public static StorageData Init<T>(T value, StorageAction action = StorageAction.Add) where T : ISubstance, new()
        {
            var sd = new StorageData() { Action = action };
            sd.SetData(value);
            return sd;
        }
    }


    public class FindData : ISubstance
    {
        public StatusSubstance Status { get; set; } = StatusSubstance.Ok;
        private object _value;
        public bool Controled { get; set; }
        public string Description
        {
            get;
            set;
        } = "Action for find data in db";
        public Guid GUID
        {
            get;
            set;
        } = new Guid("B62FA3D5-6F4D-4B45-A7BC-5D036187FEE8");

        public Guid Index
        {
            get;
            set;
        } = Guid.NewGuid();
        public string Name
        {
            get;
            set;
        } = "Find data";
        public Type TheType { get; set; }

        public T GetData<T>() where T : ISubstance, new()
        {
            if (_value == null)
                return default(T);

            if (_value is T)
            {
                return (T)_value;
            }
            else
            {
                return (T)Convert.ChangeType(_value, typeof(T));
            }
        }
        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public DateTime Creation
        {
            get;
            set;
        } = DateTime.Now;

        public Where Condition { get; set; }

        public int Id
        {
            get;
            set;
        }
    }

    public class FindListData : ISubstance
    {
        public StatusSubstance Status { get; set; } = StatusSubstance.Ok;
        private object[] _value;
        public bool Controled { get; set; }
        public string Description
        {
            get;
            set;
        } = "Action for find list data in db";
        public Guid GUID
        {
            get;
            set;
        } = new Guid("02A9A6E1-D3FA-41E7-8A32-E322C20AF27A");
        public Guid Index
        {
            get;
            set;
        } = Guid.NewGuid();
        public string Name
        {
            get;
            set;
        } = "Find list data";
        public Type TheType { get; set; }

        public T[] GetData<T>() where T : ISubstance, new()
        {
            List<T> list = new List<T>();
            foreach (var item in _value)
            {
                if (item is T)
                {
                    list.Add((T)item);


                }
                else
                {
                    list.Add((T)Convert.ChangeType(_value, typeof(T)));
                }
            }
            return list.ToArray();
        }
        public object[] Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public DateTime Creation
        {
            get;
            set;
        } = DateTime.Now;

        public Where Condition { get; set; }

        public int Id
        {
            get;
            set;
        }
    }


    public class DeleteData : ISubstance
    {
        public StatusSubstance Status { get; set; } = StatusSubstance.Ok;
        public bool Controled { get; set; }
        public string Description
        {
            get;
            set;
        } = "Action for delete in db";
        public Guid GUID
        {
            get;
            set;
        } = new Guid("733AB816-DC1F-43D5-8723-3AFE9DB1590A");
        public Guid Index
        {
            get;
            set;
        } = Guid.NewGuid();
        public string Name
        {
            get;
            set;
        } = "Find data";

        public bool Result { get; set; }

        public DateTime Creation
        {
            get;
            set;
        } = DateTime.Now;

        public Type TheType { get; set; }
        public Where Condition { get; set; }

        public int Id
        {
            get;
            set;
        }
    }


    public class IndexData : ISubstance
    {
        public StatusSubstance Status { get; set; } = StatusSubstance.Ok;
        public bool Controled { get; set; }
        public string Description
        {
            get;
            set;
        } = "Action for delete in db";
        public Guid GUID
        {
            get;
            set;
        } = new Guid("D15A5D10-854D-45FD-AC01-918D161421B0");
        public Guid Index
        {
            get;
            set;
        } = Guid.NewGuid();
        public string Name
        {
            get;
            set;
        } = "Find data";

        public bool Result { get; set; }

        public DateTime Creation
        {
            get;
            set;
        } = DateTime.Now;

        public Type TheType { get; set; }
        public string Condition { get; set; }

        public int Id
        {
            get;
            set;
        }
    }
}
