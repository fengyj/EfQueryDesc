using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace me.fengyj.QueryDesc
{
    public class FilterCriteriaMathFuncElements
    {
        #region abs()
        public class Abs : SearchCriteriaElement.Function
        {    
            public SearchCriteriaElement.FieldOrFunction FieldOrFunc { get; set; }

            public override XElement Serialize()
            {
                XElement ele = new XElement(
                    SceIdentifies.Function,
                    new XAttribute(SceIdentifies.Function_Type, this.GetType().FullName),
                    new XAttribute(SceIdentifies.Function_QualifiedName, this.GetType().AssemblyQualifiedName),
                    new XElement("FOF", this.FieldOrFunc.Serialize()));
                return ele;
            }

            public static new Abs Deserialize(XElement ele)
            {
                return new Abs { 
                    FieldOrFunc = SearchCriteriaElement.FieldOrFunction.Deserialize(
                        ele.Element("FOF").Elements().First())
                };
            }

            public override JObject Jsonize()
            {
                var jObj = new JObject();
                jObj.Add(SceIdentifies.JObjTypeProp, SceIdentifies.Function);
                jObj.Add(SceIdentifies.Function_Type, this.GetType().FullName);
                jObj.Add(SceIdentifies.Function_QualifiedName, this.GetType().FullName);
                jObj.Add("FOF", this.FieldOrFunc.Jsonize());
                return jObj;
            }

            public static new Abs Dejsonize(JObject jObj)
            {
                return new Abs
                {
                    FieldOrFunc = SearchCriteriaElement.FieldOrFunction.Dejsonize(
                        jObj.GetValue("FOF") as JObject)
                };
            }
        }
        #endregion
    }
}
