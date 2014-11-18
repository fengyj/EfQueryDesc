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
    public sealed class RefOpFilterCriteria : FilterCriteria
    {
        public SearchCriteriaElement.FieldOrFunction FieldOrFunc { get; set; }

        /// <summary>
        /// Field 2's type must be equals to the Field's type or it can be converted to the Field's type
        /// </summary>
        public SearchCriteriaElement.Field FieldOrFunc2 { get; set; }

        public override XElement Serialize()
        {
            XElement ele = new XElement(
                FcIdentifies.RefOpFilterCriteria,
                new XElement(FcIdentifies.OpProp, this.OperationType),
                new XElement(FcIdentifies.Fof1Prop, this.FieldOrFunc.Serialize()),
                new XElement(FcIdentifies.Fof2Prop, this.FieldOrFunc2.Serialize()));
            return ele;
        }

        public static new RefOpFilterCriteria Deserialize(XElement ele)
        {
            return new RefOpFilterCriteria
            {
                OperationType = int.Parse(ele.Element(FcIdentifies.OpProp).Value),
                FieldOrFunc = SearchCriteriaElement.FieldOrFunction.Deserialize(
                    ele.Element(FcIdentifies.Fof1Prop).Elements().First()),
                FieldOrFunc2 = SearchCriteriaElement.Field.Deserialize(
                    ele.Element(FcIdentifies.Fof2Prop).Elements().First())
            };
        }

        public override JObject Jsonize()
        {
            var jObj = new JObject();
            jObj.Add(FcIdentifies.JObjTypeProp, FcIdentifies.RefOpFilterCriteria);
            jObj.Add(FcIdentifies.Fof1Prop, this.FieldOrFunc.Jsonize());
            jObj.Add(FcIdentifies.Fof2Prop, this.FieldOrFunc2.Jsonize());
            jObj.Add(FcIdentifies.OpProp, this.OperationType);
            return jObj;
        }

        public static new RefOpFilterCriteria Dejsonize(JObject jObj)
        {
            return new RefOpFilterCriteria
            {
                FieldOrFunc = SearchCriteriaElement.FieldOrFunction.Dejsonize(
                    jObj.GetValue(FcIdentifies.Fof1Prop) as JObject),
                FieldOrFunc2 = SearchCriteriaElement.Field.Dejsonize(
                    jObj.GetValue(FcIdentifies.Fof2Prop) as JObject),
                OperationType = jObj.Value<int>(FcIdentifies.OpProp)
            };
        }
    }
}
