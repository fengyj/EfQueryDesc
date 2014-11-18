using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace me.fengyj.QueryDesc
{
    public sealed class BinaryOpFilterCriteria : FilterCriteria
    {
        public SearchCriteriaElement.FieldOrFunction FieldOrFunc { get; set; }

        public SearchCriteriaElement.ConstantOrFunction Arg { get; set; }

        public override XElement Serialize()
        {
            XElement ele = new XElement(
                FcIdentifies.BinaryOpFilterCriteria,
                new XElement(FcIdentifies.FofProp, this.FieldOrFunc.Serialize()),
                new XElement(FcIdentifies.OpProp, this.OperationType),
                new XElement(FcIdentifies.ArgProp, this.Arg.Serialize()));
            return ele;
        }

        public static new BinaryOpFilterCriteria Deserialize(XElement ele)
        {
            return new BinaryOpFilterCriteria {
                FieldOrFunc = SearchCriteriaElement.FieldOrFunction.Deserialize(
                    ele.Element(FcIdentifies.FofProp).Elements().First()),
                OperationType = int.Parse(ele.Element(FcIdentifies.OpProp).Value),
                Arg = SearchCriteriaElement.ConstantOrFunction.Deserialize(
                    ele.Element(FcIdentifies.ArgProp).Elements().First())
            };
        }

        public override JObject Jsonize()
        {
            var jObj = new JObject();
            jObj.Add(FcIdentifies.JObjTypeProp, FcIdentifies.BinaryOpFilterCriteria);
            jObj.Add(FcIdentifies.FofProp, this.FieldOrFunc.Jsonize());
            jObj.Add(FcIdentifies.OpProp, this.OperationType);
            jObj.Add(FcIdentifies.ArgProp, this.Arg.Jsonize());
            return jObj;
        }

        public static new BinaryOpFilterCriteria Dejsonize(JObject jObj)
        {
            return new BinaryOpFilterCriteria
            {
                FieldOrFunc = SearchCriteriaElement.FieldOrFunction.Dejsonize(
                    jObj.GetValue(FcIdentifies.FofProp) as JObject),
                OperationType = jObj.Value<int>(FcIdentifies.OpProp),
                Arg = SearchCriteriaElement.ConstantOrFunction.Dejsonize(
                    jObj.GetValue(FcIdentifies.ArgProp) as JObject)
            };
        }
    }
}
