using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace me.fengyj.QueryDesc
{
    public class JoinCriteria
    {
        public string Entity { get; set; }
        /// <summary>
        /// The fields on the left
        /// </summary>
        public SearchCriteriaElement.Field[] Left { get; set; }
        /// <summary>
        /// The fields on the right (the Entity)
        /// </summary>
        public SearchCriteriaElement.Field[] Right { get; set; }
        public FilterCriteria Filter { get; set; }

        public XElement Serialize()
        {
            return new XElement(
                this.GetType().Name,
                new XElement("Entity", this.Entity),
                new XElement("Left", this.Left.Select(item => item.Serialize())),
                new XElement("Right", this.Right.Select(item => item.Serialize())),
                new XElement("Filter", Filter == null ? null : Filter.Serialize()));
        }

        public static JoinCriteria Deserialize(XElement ele)
        {
            var filter = ele.Element("Filter").Elements().FirstOrDefault();

            return new JoinCriteria()
            {
                Entity = ele.Element("Entity").Value,
                Left = ele.Element("Left").Elements().Select(item => SearchCriteriaElement.Field.Deserialize(item)).ToArray(),
                Right = ele.Element("Right").Elements().Select(item => SearchCriteriaElement.Field.Deserialize(item)).ToArray(),
                Filter = filter == null ? null : FilterCriteria.Deserialize(filter)
            };
        }
    }
}
