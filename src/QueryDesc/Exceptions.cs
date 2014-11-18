using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace me.fengyj.QueryDesc
{
    /// <summary>
    /// this exception occurs when the Val was invalid for specified type
    /// </summary>
    [Serializable]
    public class FilterCriteriaCastException : InvalidCastException
    {
        public FilterCriteriaCastException() : base() { }
        public FilterCriteriaCastException(string msg) : base(msg) { }
        public FilterCriteriaCastException(string msg, Exception innerExp) : base(msg, innerExp) { }

        protected FilterCriteriaCastException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.FieldName = info.GetString("FieldName");
            this.FuncName = info.GetString("FuncName");
            this.Operator = info.GetInt32("Operator");
            this.Val = info.GetString("Val");
            try
            {
                this.TargetType = Type.GetType(info.GetString("TargetType"));
            }
            catch
            {
            }
        }

        public string FieldName { get; set; }
        public string FuncName { get; set; }
        public int Operator { get; set; }
        public string Val { get; set; }
        public Type TargetType { get; set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("FieldName", this.FieldName);
            info.AddValue("FuncName", this.FuncName);
            info.AddValue("Operator", this.Operator);
            info.AddValue("Val", this.Val);
            info.AddValue("TargetType", this.TargetType.FullName);
            base.GetObjectData(info, context);
        }
    }
}
