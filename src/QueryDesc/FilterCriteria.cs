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
    /// <summary>
    /// the base class for the filter criteria classes
    /// </summary>
    [Serializable]
    public abstract class FilterCriteria : SearchCriteriaElement.Function
    {
        public virtual int OperationType { get; set; }

        public new static FilterCriteria Deserialize(XElement ele)
        {
            switch (ele.Name.LocalName)
            {
                case FcIdentifies.BinaryOpFilterCriteria: 
                    return BinaryOpFilterCriteria.Deserialize(ele);
                case FcIdentifies.CompositeFilterCriteria_Not:
                    return CompositeFilterCriteria.Not.Deserialize(ele);
                case FcIdentifies.CompositeFilterCriteria_And:
                    return CompositeFilterCriteria.And.Deserialize(ele);
                case FcIdentifies.CompositeFilterCriteria_Or:
                    return CompositeFilterCriteria.Or.Deserialize(ele);
                case FcIdentifies.InOpFilterCriteria:
                    return InOpFilterCriteria.Deserialize(ele);
                case FcIdentifies.RefOpFilterCriteria:
                    return RefOpFilterCriteria.Deserialize(ele);
                case FcIdentifies.TripleOpFilterCriteria: 
                    return TripleOpFilterCriteria.Deserialize(ele);
                default: throw new ArgumentException("Unknown element type");
            }
        }

        public static FilterCriteria Dejsonize(JObject jObj)
        {
            switch (jObj.Value<string>(FcIdentifies.JObjTypeProp))
            {
                case FcIdentifies.BinaryOpFilterCriteria:
                    return BinaryOpFilterCriteria.Dejsonize(jObj);
                case FcIdentifies.CompositeFilterCriteria_Not:
                    return CompositeFilterCriteria.Not.Dejsonize(jObj);
                case FcIdentifies.CompositeFilterCriteria_And:
                    return CompositeFilterCriteria.And.Dejsonize(jObj);
                case FcIdentifies.CompositeFilterCriteria_Or:
                    return CompositeFilterCriteria.Or.Dejsonize(jObj);
                case FcIdentifies.InOpFilterCriteria:
                    return InOpFilterCriteria.Dejsonize(jObj);
                case FcIdentifies.RefOpFilterCriteria:
                    return RefOpFilterCriteria.Dejsonize(jObj);
                case FcIdentifies.TripleOpFilterCriteria:
                    return TripleOpFilterCriteria.Dejsonize(jObj);
                default: throw new ArgumentException("Unknown element type");
            }
        }
    }
}
  