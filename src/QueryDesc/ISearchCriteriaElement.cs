using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace me.fengyj.QueryDesc
{
    public interface ISearchCriteriaElement
    {
        ElementTypes ElementType { get; }

        XElement Serialize();

        JObject Jsonize();
    }

    public enum ElementTypes
    {
        Function = 0,
        Field,
        Constant
    }

    public class SearchCriteriaElement
    {
        /// <summary>
        /// the element is a function element (in fact, all the elements should be a kind of function element)
        /// </summary>
        public abstract class Function : ISearchCriteriaElement
        {
            protected Function()
            {
            }

            public ElementTypes ElementType { get { return ElementTypes.Function; } }

            public abstract XElement Serialize();

            public static Function Deserialize(XElement ele)
            {
                Type type = null;
                try
                {
                    type = Type.GetType(ele.Attribute(SceIdentifies.Function_Type).Value);
                }
                catch(TargetInvocationException)
                {
                    type = Type.GetType(ele.Attribute(SceIdentifies.Function_QualifiedName).Value);
                }

                var obj = type.GetMethod("Deserialize", BindingFlags.Static | BindingFlags.Public)
                    .Invoke(null, new object[] { ele });
                return obj as Function;
            }

            public abstract JObject Jsonize();

            public static Function Dejsonize(JObject jObj)
            {
                Type type = null;

                try
                {
                    type = Type.GetType(jObj.Value<string>(SceIdentifies.Function_Type));
                }
                catch (TargetInvocationException)
                {
                    type = Type.GetType(jObj.Value<string>(SceIdentifies.Function_QualifiedName));
                }

                var obj = type.GetMethod("Dejsonize", BindingFlags.Static | BindingFlags.Public)
                    .Invoke(null, new object[] { jObj });
                return obj as Function;
            }
        }

        /// <summary>
        /// the element is a property of function
        /// </summary>
        public class FieldOrFunction : ISearchCriteriaElement
        {
            protected FieldOrFunction()
            {
                this.ElementType = ElementTypes.Function;
            }

            public ElementTypes ElementType { get; protected set; }

            public Function FunctionElement { get; private set; }
            
            public virtual XElement Serialize()
            {
                return this.FunctionElement.Serialize();
            }

            public static FieldOrFunction Deserialize(XElement ele)
            {
                switch (ele.Name.LocalName)
                {
                    case SceIdentifies.Field: return Field.Deserialize(ele);
                    case SceIdentifies.Function: return Function.Deserialize(ele);
                    default: throw new ArgumentException("Unknown the element type");
                }
            }

            public virtual JObject Jsonize()
            {
                return this.FunctionElement.Jsonize();
            }

            public static FieldOrFunction Dejsonize(JObject jObj)
            {
                var t = jObj.Value<string>(SceIdentifies.JObjTypeProp);

                switch (t)
                {
                    case SceIdentifies.Field: return Field.Dejsonize(jObj);
                    case SceIdentifies.Function: return Function.Dejsonize(jObj);
                    default: throw new ArgumentException("Unknown the element type");
                }
            }

            #region implicit operator
            public static implicit operator FieldOrFunction(string str)
            {
                return new Field { FieldName = str };
            }

            public static implicit operator FieldOrFunction(Function ele)
            {
                return new FieldOrFunction { FunctionElement = ele };
            }
            #endregion
        }

        /// <summary>
        /// the element is a constant or function
        /// </summary>
        public class ConstantOrFunction : ISearchCriteriaElement
        {
            protected ConstantOrFunction()
            {
                this.ElementType = ElementTypes.Function;
            }

            public Function FunctionElement { get; private set; }
            
            public ElementTypes ElementType { get; protected set; }

            public virtual XElement Serialize()
            {
                return this.FunctionElement.Serialize();
            }

            public static ConstantOrFunction Deserialize(XElement ele)
            {
                switch (ele.Name.LocalName)
                {
                    case SceIdentifies.Function: return Function.Deserialize(ele);
                    case SceIdentifies.Constant: return Constant.Deserialize(ele);
                    case SceIdentifies.ArrayTypeConstant: return ArrayTypeConstant.Deserialize(ele);
                    case SceIdentifies.ComplexTypeConstant: return ComplexTypeConstant.Deserialize(ele);
                    default: throw new ArgumentException("Unknown element type");
                }
            }

            public virtual JObject Jsonize()
            {
                return this.FunctionElement.Jsonize();
            }

            public static ConstantOrFunction Dejsonize(JObject jObj)
            {
                var t = jObj.Value<string>(SceIdentifies.JObjTypeProp);

                switch (t)
                {
                    case SceIdentifies.Function: return Function.Dejsonize(jObj);
                    case SceIdentifies.Constant: return Constant.Dejsonize(jObj);
                    case SceIdentifies.ArrayTypeConstant: return ArrayTypeConstant.Dejsonize(jObj);
                    case SceIdentifies.ComplexTypeConstant: return ComplexTypeConstant.Dejsonize(jObj);
                    default: throw new ArgumentException("Unknown element type");
                }
            }

            #region implicit operator
            public static implicit operator ConstantOrFunction(string val)
            {
                return new Constant { Val = val };
            }

            public static implicit operator ConstantOrFunction(int val)
            {
                return new Constant { Val = val.ToString() };
            }

            public static implicit operator ConstantOrFunction(short val)
            {
                return new Constant { Val = val.ToString() };
            }

            public static implicit operator ConstantOrFunction(byte val)
            {
                return new Constant { Val = val.ToString() };
            }

            public static implicit operator ConstantOrFunction(sbyte val)
            {
                return new Constant { Val = val.ToString() };
            }

            public static implicit operator ConstantOrFunction(long val)
            {
                return new Constant { Val = val.ToString() };
            }

            public static implicit operator ConstantOrFunction(bool val)
            {
                return new Constant { Val = val.ToString() };
            }

            public static implicit operator ConstantOrFunction(double val)
            {
                return new Constant { Val = val.ToString() };
            }

            public static implicit operator ConstantOrFunction(float val)
            {
                return new Constant { Val = val.ToString() };
            }

            public static implicit operator ConstantOrFunction(decimal val)
            {
                return new Constant { Val = val.ToString() };
            }

            public static implicit operator ConstantOrFunction(Guid val)
            {
                return new Constant { Val = val.ToString() };
            }

            public static implicit operator ConstantOrFunction(DateTime val)
            {
                return new Constant { Val = val.ToString() };
            }

            public static implicit operator ConstantOrFunction(TimeSpan val)
            {
                return new Constant { Val = val.ToString() };
            }

            public static implicit operator ConstantOrFunction(DateTimeOffset val)
            {
                return new Constant { Val = val.ToString() };
            }

            public static implicit operator ConstantOrFunction(int? val)
            {
                return new Constant { Val = val.HasValue ? val.ToString() : (string)null };
            }

            public static implicit operator ConstantOrFunction(short? val)
            {
                return new Constant { Val = val.HasValue ? val.ToString() : (string)null };
            }

            public static implicit operator ConstantOrFunction(byte? val)
            {
                return new Constant { Val = val.HasValue ? val.ToString() : (string)null };
            }

            public static implicit operator ConstantOrFunction(sbyte? val)
            {
                return new Constant { Val = val.HasValue ? val.ToString() : (string)null };
            }

            public static implicit operator ConstantOrFunction(long? val)
            {
                return new Constant { Val = val.HasValue ? val.ToString() : (string)null };
            }

            public static implicit operator ConstantOrFunction(bool? val)
            {
                return new Constant { Val = val.HasValue ? val.ToString() : (string)null };
            }

            public static implicit operator ConstantOrFunction(double? val)
            {
                return new Constant { Val = val.HasValue ? val.ToString() : (string)null };
            }

            public static implicit operator ConstantOrFunction(float? val)
            {
                return new Constant { Val = val.HasValue ? val.ToString() : (string)null };
            }

            public static implicit operator ConstantOrFunction(decimal? val)
            {
                return new Constant { Val = val.HasValue ? val.ToString() : (string)null };
            }

            public static implicit operator ConstantOrFunction(Guid? val)
            {
                return new Constant { Val = val.HasValue ? val.ToString() : (string)null };
            }

            public static implicit operator ConstantOrFunction(DateTime? val)
            {
                return new Constant { Val = val.HasValue ? val.ToString() : (string)null };
            }

            public static implicit operator ConstantOrFunction(TimeSpan? val)
            {
                return new Constant { Val = val.HasValue ? val.ToString() : (string)null };
            }

            public static implicit operator ConstantOrFunction(DateTimeOffset? val)
            {
                return new Constant { Val = val.HasValue ? val.ToString() : (string)null };
            }

            public static implicit operator ConstantOrFunction(Function ele)
            {
                return new ConstantOrFunction { FunctionElement = ele };
            }
            #endregion
        }

        /// <summary>
        /// the element is a property
        /// </summary>
        public sealed class Field : FieldOrFunction
        {
            public Field()
            {
                this.ElementType = ElementTypes.Field;
            }

            public string FieldName { get; set; }

            //public string Entity { get; set; }

            public override XElement Serialize()
            {
                XElement ele = new XElement(
                    SceIdentifies.Field, 
                    new XElement(
                        SceIdentifies.Field_NameProp, 
                        this.FieldName));
                return ele;
            }

            public static new Field Deserialize(XElement ele)
            {
                return new Field {
                    FieldName = ele.Element(SceIdentifies.Field_NameProp).Value
                };
            }

            public override JObject Jsonize()
            {
                var jObj = new JObject();
                jObj.Add(SceIdentifies.JObjTypeProp, SceIdentifies.Field);
                jObj.Add(SceIdentifies.Field_NameProp, this.FieldName);
                return jObj;
            }

            public static new Field Dejsonize(JObject jObj)
            {
                return new Field { 
                    FieldName = jObj.Value<string>(SceIdentifies.Field_NameProp)
                };
            }

            public static implicit operator Field(string str)
            {
                return new Field { FieldName = str };
            }
        }

        /// <summary>
        /// the element is a constant (for the basic types, like int, dateime?, etc.)
        /// </summary>
        /// <exception cref="aZaaS.Framework.DataService.Common.SearchCriteria.InvalidCriteriaExpception">
        /// when pass a null and want to convert it to a non-nullable type, will get this exception
        /// </exception>
        /// <exception cref="aZaaS.Framework.DataService.Common.SearchCriteria.FilterCriteriaCastException">
        /// the input value's format is incorrect, can't parse to the target type
        /// or
        /// the target type is not supported. you can call the 
        /// aZaaS.Framework.DataService.Common.Utils.ExpressionUtils.ConstantExpHelper.AddParser<T> 
        /// function to add the parser for the target type.
        /// </exception>
        public sealed class Constant : ConstantOrFunction
        {
            public Constant()
            {
                this.ElementType = ElementTypes.Constant;
            }

            public string Val { get; set; }

            public override XElement Serialize()
            {
                XElement ele = new XElement(
                    SceIdentifies.Constant, 
                    new XElement(
                        SceIdentifies.Constant_ValProp,
                        this.Val));
                return ele;
            }

            public static new Constant Deserialize(XElement ele)
            {
                return new Constant
                {
                    Val = ele.Element(SceIdentifies.Constant_ValProp).Value
                };
            }

            public override JObject Jsonize()
            {
                var jObj = new JObject();
                jObj.Add(SceIdentifies.JObjTypeProp, SceIdentifies.Constant);
                jObj.Add(SceIdentifies.Constant_ValProp, this.Val);
                return jObj;
            }

            public static new Constant Dejsonize(JObject jObj)
            {
                return new Constant
                {
                    Val = jObj.Value<string>(SceIdentifies.Constant_ValProp)
                };
            }

            #region implicit operator
            public static implicit operator Constant(string val)
            {
                return new Constant { Val = val };
            }

            public static implicit operator Constant(int val)
            {
                return new Constant { Val = val.ToString() };
            }

            public static implicit operator Constant(short val)
            {
                return new Constant { Val = val.ToString() };
            }

            public static implicit operator Constant(byte val)
            {
                return new Constant { Val = val.ToString() };
            }

            public static implicit operator Constant(sbyte val)
            {
                return new Constant { Val = val.ToString() };
            }

            public static implicit operator Constant(long val)
            {
                return new Constant { Val = val.ToString() };
            }

            public static implicit operator Constant(bool val)
            {
                return new Constant { Val = val.ToString() };
            }

            public static implicit operator Constant(double val)
            {
                return new Constant { Val = val.ToString() };
            }

            public static implicit operator Constant(float val)
            {
                return new Constant { Val = val.ToString() };
            }

            public static implicit operator Constant(decimal val)
            {
                return new Constant { Val = val.ToString() };
            }

            public static implicit operator Constant(Guid val)
            {
                return new Constant { Val = val.ToString() };
            }

            public static implicit operator Constant(DateTime val)
            {
                return new Constant { Val = val.ToString() };
            }

            public static implicit operator Constant(TimeSpan val)
            {
                return new Constant { Val = val.ToString() };
            }

            public static implicit operator Constant(DateTimeOffset val)
            {
                return new Constant { Val = val.ToString() };
            }

            public static implicit operator Constant(int? val)
            {
                return new Constant { Val = val.HasValue ? val.ToString() : (string)null };
            }

            public static implicit operator Constant(short? val)
            {
                return new Constant { Val = val.HasValue ? val.ToString() : (string)null };
            }

            public static implicit operator Constant(byte? val)
            {
                return new Constant { Val = val.HasValue ? val.ToString() : (string)null };
            }

            public static implicit operator Constant(sbyte? val)
            {
                return new Constant { Val = val.HasValue ? val.ToString() : (string)null };
            }

            public static implicit operator Constant(long? val)
            {
                return new Constant { Val = val.HasValue ? val.ToString() : (string)null };
            }

            public static implicit operator Constant(bool? val)
            {
                return new Constant { Val = val.HasValue ? val.ToString() : (string)null };
            }

            public static implicit operator Constant(double? val)
            {
                return new Constant { Val = val.HasValue ? val.ToString() : (string)null };
            }

            public static implicit operator Constant(float? val)
            {
                return new Constant { Val = val.HasValue ? val.ToString() : (string)null };
            }

            public static implicit operator Constant(decimal? val)
            {
                return new Constant { Val = val.HasValue ? val.ToString() : (string)null };
            }

            public static implicit operator Constant(Guid? val)
            {
                return new Constant { Val = val.HasValue ? val.ToString() : (string)null };
            }

            public static implicit operator Constant(DateTime? val)
            {
                return new Constant { Val = val.HasValue ? val.ToString() : (string)null };
            }

            public static implicit operator Constant(TimeSpan? val)
            {
                return new Constant { Val = val.HasValue ? val.ToString() : (string)null };
            }

            public static implicit operator Constant(DateTimeOffset? val)
            {
                return new Constant { Val = val.HasValue ? val.ToString() : (string)null };
            }
            #endregion
        }

        /// <summary>
        /// the element is a array (the item's type is basic type)
        /// </summary>
        public sealed class ArrayTypeConstant : ConstantOrFunction
        {
            public ArrayTypeConstant()
            {
                this.ElementType = ElementTypes.Constant;
            }

            public string[] Val { get; set; }

            public override XElement Serialize()
            {
                return new XElement(SceIdentifies.ArrayTypeConstant,
                    new XElement(
                        SceIdentifies.ArrayTypeConstant_ValProp, 
                        this.Val != null
                        ? this.Val.Select(item => new XElement(
                            SceIdentifies.ArrayTypeConstant_Item,
                            item)).ToArray()
                        : null));
            }

            public new static ArrayTypeConstant Deserialize(XElement ele)
            {
                return new ArrayTypeConstant { 
                    Val = ele
                        .Element(SceIdentifies.ArrayTypeConstant_ValProp)
                        .Elements(SceIdentifies.ArrayTypeConstant_Item)
                        .Select(item => item.Value)
                        .ToArray()
                };
            }

            public override JObject Jsonize()
            {
                var jObj = new JObject();
                jObj.Add(SceIdentifies.JObjTypeProp, SceIdentifies.ArrayTypeConstant);
                jObj.Add(SceIdentifies.ArrayTypeConstant_ValProp, new JArray(this.Val));
                return jObj;
            }

            public static new ArrayTypeConstant Dejsonize(JObject jObj)
            {
                return new ArrayTypeConstant
                {
                    Val = (jObj.GetValue(SceIdentifies.ArrayTypeConstant_ValProp) as JArray).ToObject<string[]>()
                };
            }
        }

        /// <summary>
        /// the element is a constant (besides the basic types)
        /// </summary>
        public sealed class ComplexTypeConstant : ConstantOrFunction 
        {
            public ComplexTypeConstant()
            {
                this.ElementType = ElementTypes.Constant;
            }

            public ComplexTypeConstant(object val) : this()
            {
                Debug.Assert(val != null, "Cannot pass a null value to this constructor.");
                this.ObjType = val.GetType();
                this.Val = val;
            }

            public ComplexTypeConstant(object val, Type valType)
                : this()
            {
                this.ObjType = valType;
                this.Val = val;
            }

            public object Val { get; private set; }

            public Type ObjType { get; set; }

            public override XElement Serialize()
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(this.ObjType);
                XDocument valDoc = new XDocument();
                using (var writer = valDoc.CreateWriter())
                {
                    serializer.Serialize(writer, this.Val);
                    writer.Flush();
                }

                XElement ele = new XElement(SceIdentifies.ComplexTypeConstant, new XElement(
                    SceIdentifies.ComplexTypeConstant_ValProp,
                    new XAttribute(SceIdentifies.ComplexTypeConstant_Type, this.ObjType.FullName),
                    new XAttribute(SceIdentifies.ComplexTypeConstant_QualifiedName, this.ObjType.AssemblyQualifiedName),
                    new XCData(valDoc.ToString())
                    ));
                return ele;
            }

            public new static ComplexTypeConstant Deserialize(XElement ele)
            {
                var valEle = ele.Element(SceIdentifies.ComplexTypeConstant_ValProp);
                if (string.IsNullOrEmpty(valEle.Value))
                {
                    return new ComplexTypeConstant(null, typeof(object));
                }
                Type type = null;
                try
                {
                    type = Type.GetType(valEle.Attribute(SceIdentifies.ComplexTypeConstant_Type).Value);
                }
                catch(TargetInvocationException) {
                    type = Type.GetType(valEle.Attribute(SceIdentifies.ComplexTypeConstant_QualifiedName).Value);
                }
                
                var serializer = new System.Xml.Serialization.XmlSerializer(type);
                var val = serializer.Deserialize(XDocument.Parse(valEle.Value).CreateReader());

                return new ComplexTypeConstant(val, type);
            }

            public override JObject Jsonize()
            {
                var jObj = new JObject();
                jObj.Add(SceIdentifies.JObjTypeProp, SceIdentifies.ComplexTypeConstant);
                jObj.Add(SceIdentifies.ComplexTypeConstant_Type, this.ObjType.FullName);
                jObj.Add(SceIdentifies.ComplexTypeConstant_QualifiedName, this.ObjType.AssemblyQualifiedName);
                jObj.Add(SceIdentifies.ArrayTypeConstant_ValProp, JsonConvert.SerializeObject(this.Val));
                return jObj;
            }

            public static new ComplexTypeConstant Dejsonize(JObject jObj)
            {
                var val = jObj.GetValue(SceIdentifies.ComplexTypeConstant_ValProp);
                if (val == null)
                {
                    return new ComplexTypeConstant(null, typeof(object));
                }
                Type type = null;
                try
                {
                    type = Type.GetType(jObj.Value<string>(SceIdentifies.ComplexTypeConstant_Type));
                }
                catch (TargetInvocationException)
                {
                    type = Type.GetType(jObj.Value<string>(SceIdentifies.ComplexTypeConstant_QualifiedName));
                }

                var serializer = new System.Xml.Serialization.XmlSerializer(type);
                
                return new ComplexTypeConstant(val.ToObject(type), type);
            }
        }
    }
}
