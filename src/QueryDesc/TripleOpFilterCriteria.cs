using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace me.fengyj.QueryDesc
{
    public sealed class TripleOpFilterCriteria : FilterCriteria
    {
        public SearchCriteriaElement.FieldOrFunction FieldOrFunc { get; set; }

        public SearchCriteriaElement.ConstantOrFunction Arg1 { get; set; }
        public SearchCriteriaElement.ConstantOrFunction Arg2 { get; set; }

        public override int OperationType
        {
            get
            {
                return (int)FilterOperations.ThreeOperations.Between;
            }
            set
            {
                // empty
            }
        }

        public override XElement Serialize()
        {
            XElement ele = new XElement(
                FcIdentifies.TripleOpFilterCriteria,
                new XElement(FcIdentifies.FofProp, this.FieldOrFunc.Serialize()),
                new XElement(FcIdentifies.OpProp, this.OperationType),
                new XElement(FcIdentifies.Arg1Prop, this.Arg1.Serialize()),
                new XElement(FcIdentifies.Arg2Prop, this.Arg2.Serialize()));
            return ele;
        }

        public static new TripleOpFilterCriteria Deserialize(XElement ele)
        {
            return new TripleOpFilterCriteria {
                FieldOrFunc = SearchCriteriaElement.FieldOrFunction.Deserialize(
                    ele.Element(FcIdentifies.FofProp).Elements().First()),
                OperationType = int.Parse(ele.Element(FcIdentifies.OpProp).Value),
                Arg1 = SearchCriteriaElement.ConstantOrFunction.Deserialize(
                    ele.Element(FcIdentifies.Arg1Prop).Elements().First()),
                Arg2 = SearchCriteriaElement.ConstantOrFunction.Deserialize(
                    ele.Element(FcIdentifies.Arg2Prop).Elements().First())
            };
        }

        public override JObject Jsonize()
        {
            var jObj = new JObject();
            jObj.Add(FcIdentifies.JObjTypeProp, FcIdentifies.TripleOpFilterCriteria);
            jObj.Add(FcIdentifies.FofProp, this.FieldOrFunc.Jsonize());
            jObj.Add(FcIdentifies.Arg1Prop, this.Arg1.Jsonize());
            jObj.Add(FcIdentifies.Arg2Prop, this.Arg2.Jsonize());
            return jObj;
        }

        public static new TripleOpFilterCriteria Dejsonize(JObject jObj)
        {
            return new TripleOpFilterCriteria
            {
                FieldOrFunc = SearchCriteriaElement.FieldOrFunction.Dejsonize(
                    jObj.GetValue(FcIdentifies.FofProp) as JObject),
                OperationType = jObj.Value<int>(FcIdentifies.OpProp),
                Arg1 = SearchCriteriaElement.ConstantOrFunction.Dejsonize(
                    jObj.GetValue(FcIdentifies.Arg1Prop) as JObject),
                Arg2 = SearchCriteriaElement.ConstantOrFunction.Dejsonize(
                    jObj.GetValue(FcIdentifies.Arg2Prop) as JObject)
            };
        }
    }
}
