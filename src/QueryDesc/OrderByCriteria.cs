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
    public class OrderByCriteria
    {
        public OrderByCriteria()
        {
            Order = SortTypes.Asc;
        }

        public SearchCriteriaElement.Field Field { get; set; }
        //public string Entity { get; set; }
        public SortTypes Order { get; set; }
        public OrderByCriteria ThenBy { get; set; }
        
        public XElement Serialize()
        {
            if (this.ThenBy != null)
            {
                return new XElement(
                    OcIdentifies.OrderByCriteria,
                    new XElement(
                        OcIdentifies.Field, 
                        this.Field.FieldName, 
                        new XAttribute(OcIdentifies.Order, this.Order.ToString())),
                    new XElement(OcIdentifies.ThenBy, this.ThenBy.Serialize()));
            }
            else
            {
                return new XElement(
                    OcIdentifies.OrderByCriteria,
                    new XElement(
                        OcIdentifies.Field, 
                        this.Field.FieldName,
                        new XAttribute(OcIdentifies.Order, this.Order.ToString())));
            }
        }

        public static OrderByCriteria Deserialize(XElement ele)
        {
            return new OrderByCriteria
            {
                Field = ele.Element(OcIdentifies.Field).Value,
                Order = (SortTypes)Enum.Parse(
                    typeof(SortTypes), 
                    ele.Element(OcIdentifies.Field).Attribute(OcIdentifies.Order).Value),
                ThenBy = ele.Element(OcIdentifies.ThenBy) == null
                    ? null
                    : Deserialize(ele.Element(OcIdentifies.ThenBy).Elements().First())
            };
        }

        public JObject Jsonize()
        {
            var jObj = new JObject();
            jObj.Add(OcIdentifies.JObjTypeProp, OcIdentifies.OrderByCriteria);
            jObj.Add(OcIdentifies.Field, this.Field.FieldName);
            jObj.Add(OcIdentifies.Order, (int)this.Order);
            if(this.ThenBy != null)
                jObj.Add(OcIdentifies.ThenBy, this.ThenBy.Jsonize());
            return jObj;
        }

        public static OrderByCriteria Dejsonize(JObject jObj)
        {
            var result = new OrderByCriteria
            {
                Field = jObj.Value<string>(OcIdentifies.Field),
                Order = (SortTypes)jObj.Value<int>(OcIdentifies.Order)
            };
            JToken thenby = null;
            if(jObj.TryGetValue(OcIdentifies.ThenBy, out thenby) && thenby is JObject)
            {
                result.ThenBy = OrderByCriteria.Dejsonize(thenby as JObject);
            }
            return result;
        }

        public enum SortTypes
        {
            Asc = 1,
            Desc = 2
        }
    }

}
