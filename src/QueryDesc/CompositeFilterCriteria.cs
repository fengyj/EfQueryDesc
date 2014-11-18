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
    public class CompositeFilterCriteria
    {
        public class Not : FilterCriteria
        {
            public FilterCriteria Arg { get; set; }

            public override int OperationType
            {
                get { return (int)FilterOperations.ComposeFilterOperations.Not; }
                set { /* empty */ }
            }

            public override XElement Serialize()
            {
                XElement ele = new XElement(
                    FcIdentifies.CompositeFilterCriteria_Not,
                    new XElement(FcIdentifies.ArgProp, this.Arg.Serialize()));
                return ele;
            }

            public new static Not Deserialize(XElement ele)
            {
                return new Not {
                    Arg = FilterCriteria.Deserialize(ele.Element(FcIdentifies.ArgProp).Elements().First())
                };
            }

            public override JObject Jsonize()
            {
                var jObj = new JObject();
                jObj.Add(FcIdentifies.JObjTypeProp, FcIdentifies.CompositeFilterCriteria_Not);
                jObj.Add(FcIdentifies.ArgProp, this.Arg.Jsonize());
                return jObj;
            }

            public static new Not Dejsonize(JObject jObj)
            {
                return new Not
                {
                    Arg = FilterCriteria.Dejsonize(
                        jObj.GetValue(FcIdentifies.ArgProp) as JObject)
                };
            }
        }
        
        public class And : FilterCriteria
        {
            public FilterCriteria Arg1 { get; set; }
            public FilterCriteria Arg2 { get; set; }
            
            public override int OperationType { 
                get { return (int)FilterOperations.ComposeFilterOperations.And; } 
                set { /* empty */ }
            }

            public override XElement Serialize()
            {
                XElement ele = new XElement(
                    FcIdentifies.CompositeFilterCriteria_And,
                    new XElement(FcIdentifies.Arg1Prop, this.Arg1.Serialize()),
                    new XElement(FcIdentifies.Arg2Prop, this.Arg2.Serialize()));
                return ele;
            }

            public new static And Deserialize(XElement ele)
            {
                return new And {
                    Arg1 = FilterCriteria.Deserialize(ele.Element(FcIdentifies.Arg1Prop).Elements().First()),
                    Arg2 = FilterCriteria.Deserialize(ele.Element(FcIdentifies.Arg2Prop).Elements().First())
                };
            }

            public override JObject Jsonize()
            {
                var jObj = new JObject();
                jObj.Add(FcIdentifies.JObjTypeProp, FcIdentifies.CompositeFilterCriteria_And);
                jObj.Add(FcIdentifies.Arg1Prop, this.Arg1.Jsonize());
                jObj.Add(FcIdentifies.Arg2Prop, this.Arg2.Jsonize());
                return jObj;
            }

            public static new And Dejsonize(JObject jObj)
            {
                return new And
                {
                    Arg1 = FilterCriteria.Dejsonize(
                        jObj.GetValue(FcIdentifies.Arg1Prop) as JObject),
                    Arg2 =FilterCriteria.Dejsonize(
                        jObj.GetValue(FcIdentifies.Arg2Prop) as JObject)
                };
            }
        }

        public class Or : FilterCriteria
        {
            public FilterCriteria Arg1 { get; set; }
            public FilterCriteria Arg2 { get; set; }

            public override int OperationType { 
                get { return (int)FilterOperations.ComposeFilterOperations.Or; } 
                set { /* empty */ }
            }

            public override XElement Serialize()
            {
                XElement ele = new XElement(
                    FcIdentifies.CompositeFilterCriteria_Or,
                    new XElement(FcIdentifies.Arg1Prop, this.Arg1.Serialize()),
                    new XElement(FcIdentifies.Arg2Prop, this.Arg2.Serialize()));
                return ele;
            }

            public new static Or Deserialize(XElement ele)
            {
                return new Or
                {
                    Arg1 = FilterCriteria.Deserialize(ele.Element(FcIdentifies.Arg1Prop).Elements().First()),
                    Arg2 = FilterCriteria.Deserialize(ele.Element(FcIdentifies.Arg2Prop).Elements().First())
                };
            }

            public override JObject Jsonize()
            {
                var jObj = new JObject();
                jObj.Add(FcIdentifies.JObjTypeProp, FcIdentifies.CompositeFilterCriteria_Or);
                jObj.Add(FcIdentifies.Arg1Prop, this.Arg1.Jsonize());
                jObj.Add(FcIdentifies.Arg2Prop, this.Arg2.Jsonize());
                return jObj;
            }

            public static new Or Dejsonize(JObject jObj)
            {
                return new Or
                {
                    Arg1 = FilterCriteria.Dejsonize(
                        jObj.GetValue(FcIdentifies.Arg1Prop) as JObject),
                    Arg2 = FilterCriteria.Dejsonize(
                        jObj.GetValue(FcIdentifies.Arg2Prop) as JObject)
                };
            }
        }
    }
}
