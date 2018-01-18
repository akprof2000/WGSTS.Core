using Core.Interfaces;
using System;
using System.Collections.Generic;

namespace Core.Base
{
    public class SandboxDataValue 
    {
        public Guid From { get; set; }
        public Dictionary<Guid, string> To { get; set; } = new Dictionary<Guid, string>();
        ISubstance _value;
        public Type TheType { get; set; }
        public void AddData<T>(T value) where T:ISubstance, new()
        {
            _value = value;
            TheType = typeof(T);
        }

        public ISubstance Value
        {
            get { return _value; }
            set { _value = value; }
        }
        public string EventName { get; set; }
        public bool Controled { get; internal set; }
    }
}