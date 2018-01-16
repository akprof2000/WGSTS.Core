using System;
using System.Linq;

namespace Core.Interfaces
{
    public class Where
    {
        public const int Ascending = 1;
        public const int Descending = -1;
        public enum Operation
        {
            All,
            AllField,
            And,
            Between,
            Contains,
            EQ,
            GT,
            GTE,
            In,
            LT,
            LTE,
            Not,
            Or,
            StartsWith
        }

        int order = 1;
        Operation curent;
        object start;
        object end;
        string strvalue;
        object value;
        object Value
        {
            get
            {
                return value;
            }
            set
            {
                if (value is Enum)
                    this.value = value.ToString();
                else
                    this.value = value;
            }
        }
        object[] args;
        string field;
        Where left;
        Where right;

        public static Where All(int order = 1)
        {
            return new Where() { curent = Operation.All, order = order };
        }
        public static Where All(string field, int order = 1)
        {
            return new Where() { curent = Operation.AllField, field = field };
        }
        public static Where And(Where left, Where right)
        {
            return new Where() { curent = Operation.And, left = left, right = right };
        }
        public static Where Between<TParam>(string field, TParam start, TParam end)
        {
            return new Where() { curent = Operation.Between, field = field, start = start, end = end };
        }
        public static Where Contains(string field, string value)
        {
            return new Where() { curent = Operation.Contains, field = field, strvalue = value };
        }
        public static Where EQ<TParam>(string field, TParam value)
        {
            return new Where() { curent = Operation.EQ, field = field, Value = value };
        }
        public static Where GT<TParam>(string field, TParam value)
        {
            return new Where() { curent = Operation.GT, field = field, Value = value };
        }
        public static Where GTE<TParam>(string field, TParam value)
        {
            return new Where() { curent = Operation.GTE, field = field, Value = value };
        }
        public static Where In<TParam>(string field, TParam value)
        {
            return new Where() { curent = Operation.In, field = field, args = new object[] { value } };
        }
        public static Where In<TParam>(string field, params TParam[] values)
        {
            return new Where() { curent = Operation.In, field = field, args = values.Cast<object>().ToArray() };
        }
        public static Where LT<TParam>(string field, TParam value)
        {
            return new Where() { curent = Operation.LT, field = field, Value = value };
        }
        public static Where LTE<TParam>(string field, TParam value)
        {
            return new Where() { curent = Operation.LTE, field = field, Value = value };
        }
        public static Where Not<TParam>(string field, TParam value)
        {
            return new Where() { curent = Operation.Not, field = field, Value = value };
        }
        public static Where Or(Where left, Where right)
        {
            return new Where() { curent = Operation.Or, left = left, right = right };
        }
        public static Where StartsWith(string field, string value)
        {
            return new Where() { curent = Operation.StartsWith, field = field, strvalue = value };
        }




        public override string ToString()
        {
            var str = "";
            switch (curent)
            {
                case Operation.All:
                    str = $"read all with {order}";
                    break;
                case Operation.AllField:
                    str = $"read all with {order} by field {field}";
                    break;
                case Operation.And:
                    str = $"{left} and {right}";
                    break;
                case Operation.Between:
                    str = $"{field} Between {start} and {end}";
                    break;
                case Operation.Contains:
                    str = $"{field} Contains {strvalue}";
                    break;
                case Operation.EQ:
                    str = $"{field} eq {value}";
                    break;
                case Operation.GT:
                    str = $"{field} gt {value}";
                    break;
                case Operation.GTE:
                    str = $"{field} gte {value}";
                    break;
                case Operation.In:
                    str = $"{field} in {args}";
                    break;
                case Operation.LT:
                    str = $"{field} lt {value}";
                    break;
                case Operation.LTE:
                    str = $"{field} lte {value}";
                    break;
                case Operation.Not:
                    str = $"{field} not {value}";
                    break;
                case Operation.Or:
                    str = $"{left} or {right}";
                    break;
                case Operation.StartsWith:
                    str = $"{field} Starts With {strvalue}";
                    break;
                default:
                    break;
            }


            return str;
        }

    }

}
