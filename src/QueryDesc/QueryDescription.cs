using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace me.fengyj.QueryDesc
{
    [Serializable]
    public class QueryDescription : ISerializable
    {
        [NonSerialized]
        private FilterCriteria filter;
        [NonSerialized]
        private OrderByCriteria orderBy;
        [NonSerialized]
        private PagingCriteria paging;

        public QueryDescription()
        {
        }

        protected QueryDescription(SerializationInfo info, StreamingContext context)
        {
            var filterStr = info.GetString("Filter");
            var orderStr = info.GetString("OrderBy");
            var pagingStr = info.GetString("Paging");

            if (filterStr != null)
            {
                this.filter = FilterCriteria.Deserialize(XElement.Parse(filterStr));
            }
            if (orderStr != null)
            {
                this.orderBy = OrderByCriteria.Deserialize(XElement.Parse(orderStr));
            }
            if (pagingStr != null)
            {
                this.paging = PagingCriteria.Deserialze(XElement.Parse(pagingStr));
            }
        }

        public FilterCriteria Filter
        {
            get
            {
                return this.filter;
            }
            set
            {
                this.filter = value;
            }
        }

        public OrderByCriteria OrderBy
        {
            get
            {
                return this.orderBy;
            }
            set
            {
                this.orderBy = value;
            }
        }

        public PagingCriteria Paging
        {
            get
            {
                return this.paging;
            }
            set
            {
                this.paging = value;
            }
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Filter", this.filter == null ? null : this.filter.Serialize().ToString());
            info.AddValue("OrderBy", this.orderBy == null ? null : this.orderBy.Serialize().ToString());
            info.AddValue("Paging", this.paging == null ? null : this.paging.Serialize().ToString());
        }

        public string ToJson()
        {
            var jObj = new JObject();
            if(this.Filter != null) jObj.Add("Filter", this.Filter.Jsonize());
            if(this.OrderBy !=null) jObj.Add("OrderBy", this.OrderBy.Jsonize());
            if(this.Paging != null) jObj.Add("Paging", this.Paging.Jsonize());
            return jObj.ToString();
        }

        public static QueryDescription FromJson(string json)
        {
            var jObj = JObject.Parse(json);
            var result  = new QueryDescription();
            JToken tmp = null;
            if(jObj.TryGetValue("Filter", out tmp) && tmp is JObject)
            {
                result.Filter = FilterCriteria.Dejsonize(tmp as JObject);
            }
            if (jObj.TryGetValue("OrderBy", out tmp) && tmp is JObject)
            {
                result.OrderBy = OrderByCriteria.Dejsonize(tmp as JObject);
            }
            if (jObj.TryGetValue("Paging", out tmp) && tmp is JObject)
            {
                result.Paging = PagingCriteria.Dejsonize(tmp as JObject);
            }
            return result;
        }
    }
}
