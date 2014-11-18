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
    public class InOpFilterCriteria : FilterCriteria
    {
        public SearchCriteriaElement.FieldOrFunction FieldOrFunc { get; set; }

        public SearchCriteriaElement.ArrayTypeConstant Arg { get; set; }

        public override int OperationType
        {
            get
            {
                return (int)FilterOperations.FullFilterOperations.In;
            }
            set
            {
                // empty
            }
        }

        public override XElement Serialize()
        {
            XElement ele = new XElement(
                FcIdentifies.InOpFilterCriteria,
                new XElement(FcIdentifies.FofProp, this.FieldOrFunc.Serialize()),
                new XElement(FcIdentifies.ArgProp, this.Arg.Serialize()));
            return ele;
        }

        public static new InOpFilterCriteria Deserialize(XElement ele)
        {
            return new InOpFilterCriteria{
                FieldOrFunc = SearchCriteriaElement.FieldOrFunction.Deserialize(
                    ele.Element(FcIdentifies.FofProp).Elements().First()),
                Arg = SearchCriteriaElement.ArrayTypeConstant.Deserialize(
                    ele.Element(FcIdentifies.ArgProp).Elements().First())
            };
        }

        public override JObject Jsonize()
        {
            var jObj = new JObject();
            jObj.Add(FcIdentifies.JObjTypeProp, FcIdentifies.InOpFilterCriteria);
            jObj.Add(FcIdentifies.FofProp, this.FieldOrFunc.Jsonize());
            jObj.Add(FcIdentifies.ArgProp, this.Arg.Jsonize());
            return jObj;
        }

        public static new InOpFilterCriteria Dejsonize(JObject jObj)
        {
            return new InOpFilterCriteria
            {
                FieldOrFunc = SearchCriteriaElement.FieldOrFunction.Dejsonize(
                    jObj.GetValue(FcIdentifies.FofProp) as JObject),
                Arg = SearchCriteriaElement.ArrayTypeConstant.Dejsonize(
                    jObj.GetValue(FcIdentifies.ArgProp) as JObject)
            };
        }
    }
}
