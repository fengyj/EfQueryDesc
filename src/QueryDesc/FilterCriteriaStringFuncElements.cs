using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace me.fengyj.QueryDesc
{
    public class FilterCriteriaStringFuncElements
    {
        #region Substring()
        public class Substring : SearchCriteriaElement.Function
        {
            private SearchCriteriaElement.Constant start;
            private SearchCriteriaElement.Constant length;
            
            public Substring()
            {
                this.start = new SearchCriteriaElement.Constant
                { 
                    //ReturnType = typeof(int)
                };
                this.length = new SearchCriteriaElement.Constant
                {
                    //ReturnType = typeof(int)
                };
            }

            public SearchCriteriaElement.FieldOrFunction FieldOrFunc { get; set; }

            public SearchCriteriaElement.Constant Start
            {
                get { return this.start; }
                set { this.start.Val = value.Val; }
            }

            public SearchCriteriaElement.Constant Length
            {
                get { return this.length; }
                set { this.length.Val = (value == null ? (string)null : value.Val); }
            }

            //public override Expression GetExpression()
            //{
            //    var exp = FieldOrFunc.GetExpression() as LambdaExpression;
            //    if (this.Length.Val == null)
            //    {
            //        return Expression.Lambda(
            //            Expression.Call(
            //                exp.Body,
            //                methodWithOneParameter,
            //                this.Start.GetExpression()),
            //            exp.Parameters);
            //    }
            //    else
            //    {
            //        return Expression.Lambda(
            //            Expression.Call(
            //                exp.Body,
            //                methodWithTwoParameter,
            //                this.Start.GetExpression(),
            //                this.Length.GetExpression()),
            //            exp.Parameters);
            //    }
            //}

            //public override Type ReturnType
            //{
            //    get { return typeof(string); }
            //    set { }
            //}

            public override XElement Serialize()
            {
                XElement ele = new XElement(
                    SceIdentifies.Function,
                    new XAttribute(SceIdentifies.Function_Type, this.GetType().FullName),
                    new XAttribute(SceIdentifies.Function_QualifiedName, this.GetType().AssemblyQualifiedName),
                    new XElement("FOF", this.FieldOrFunc.Serialize()),
                    new XElement("Start", Start.Serialize()),
                    new XElement("Length", Length.Serialize()));
                return ele;
            }

            public static new Substring Deserialize(XElement ele)
            {
                return new Substring
                {
                    FieldOrFunc = SearchCriteriaElement.FieldOrFunction.Deserialize(ele.Element("FOF").Elements().First()),
                    Start = SearchCriteriaElement.Constant.Deserialize(ele.Element("Start").Elements().First()),
                    Length = SearchCriteriaElement.Constant.Deserialize(ele.Element("Length").Elements().First())
                };
            }

            public override JObject Jsonize()
            {
                var jObj = new JObject();
                jObj.Add(SceIdentifies.JObjTypeProp, SceIdentifies.Function);
                jObj.Add(SceIdentifies.Function_Type, this.GetType().FullName);
                jObj.Add(SceIdentifies.Function_QualifiedName, this.GetType().FullName);
                jObj.Add("FOF", this.FieldOrFunc.Jsonize());
                jObj.Add("Start", start.Jsonize());
                jObj.Add("Length", length.Jsonize());
                return jObj;
            }

            public static new Substring Dejsonize(JObject jObj)
            {
                return new Substring
                {
                    FieldOrFunc = SearchCriteriaElement.FieldOrFunction.Dejsonize(
                        jObj.GetValue("FOF") as JObject),
                    Start = SearchCriteriaElement.Constant.Dejsonize(
                        jObj.GetValue("Start") as JObject),
                    length = SearchCriteriaElement.Constant.Dejsonize(
                        jObj.GetValue("Length") as JObject)
                };
            }
        }
        #endregion

        #region Trim()
        public class Trim : SearchCriteriaElement.Function
        {
            private static MethodInfo methodWithEmptyParameter = typeof(string).GetMethod("Trim", Type.EmptyTypes);
            private static MethodInfo methodWithOneParameter = typeof(string).GetMethod("Trim", new Type[] { typeof(char[]) });

            public Trim()
            {
            }

            public SearchCriteriaElement.FieldOrFunction FieldOrFunc { get; set; }

            
            //public override Expression GetExpression()
            //{
            //    var exp = FieldOrFunc.GetExpression() as LambdaExpression;
            //    return Expression.Lambda(
            //           Expression.Call(
            //               exp.Body,
            //               methodWithEmptyParameter),
            //           exp.Parameters);
            //}

            public override XElement Serialize()
            {
                XElement ele = new XElement(
                    SceIdentifies.Function,
                    new XAttribute(SceIdentifies.Function_Type, this.GetType().FullName),
                    new XAttribute(SceIdentifies.Function_QualifiedName, this.GetType().AssemblyQualifiedName),
                    new XElement("FOF", this.FieldOrFunc.Serialize()));
                return ele;
            }

            public static new Trim Deserialize(XElement ele)
            {
                return new Trim {
                    FieldOrFunc = SearchCriteriaElement.FieldOrFunction.Deserialize(ele.Element("FOF").Elements().First())
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

            public static new Trim Dejsonize(JObject jObj)
            {
                return new Trim
                {
                    FieldOrFunc = SearchCriteriaElement.FieldOrFunction.Dejsonize(
                        jObj.GetValue("FOF") as JObject)
                };
            }
        }
        #endregion
    }
}
