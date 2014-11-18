using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace me.fengyj.QueryDesc
{
    public class PagingCriteria
    {
        /// <summary>
        /// CurrentPage starts from 1
        /// </summary>
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int? TotalSize { get; set; }

        public int? TotalPages
        {
            get
            {
                if (TotalSize.HasValue)
                    return (int)Math.Ceiling(TotalSize.Value * 1.0 / PageSize);
                else
                    return null;
            }
        }

        public XElement Serialize()
        {
            return new XElement(
                PcIdentifies.PagingCriteria,
                new XElement(PcIdentifies.CurrentPage, this.CurrentPage),
                new XElement(PcIdentifies.PageSize, this.PageSize),
                new XElement(PcIdentifies.TotalSize, this.TotalSize),
                new XElement(PcIdentifies.TotalPages, this.TotalPages));
        }

        public static PagingCriteria Deserialze(XElement ele)
        {
            return new PagingCriteria
            {
                CurrentPage = int.Parse(ele.Element(PcIdentifies.CurrentPage).Value),
                PageSize = int.Parse(ele.Element(PcIdentifies.PageSize).Value),
                TotalSize = string.IsNullOrEmpty(ele.Element(PcIdentifies.TotalSize).Value)
                    ? (int?)null
                    : (int?)(int.Parse(ele.Element(PcIdentifies.TotalSize).Value))
            };
        }

        public JObject Jsonize()
        {
            var jObj = new JObject();
            jObj.Add(PcIdentifies.JObjTypeProp, PcIdentifies.PagingCriteria);
            jObj.Add(PcIdentifies.CurrentPage, this.CurrentPage);
            jObj.Add(PcIdentifies.PageSize, this.PageSize);
            jObj.Add(PcIdentifies.TotalSize, this.TotalSize);
            jObj.Add(PcIdentifies.TotalPages, this.TotalPages);
            return jObj;
        }

        public static PagingCriteria Dejsonize(JObject jObj)
        {
            return new PagingCriteria
            {
                CurrentPage = jObj.Value<int>(PcIdentifies.CurrentPage),
                PageSize = jObj.Value<int>(PcIdentifies.PageSize),
                TotalSize = jObj.Value<int?>(PcIdentifies.TotalSize)
            };
        }
    }
}
