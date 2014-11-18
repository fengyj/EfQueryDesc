using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace me.fengyj.QueryDesc
{
    interface ISearchCriteriaExpressionProvider<T>
    {
        T GetExpression(ISearchCriteriaElement searchCriteriaElement);
    }
}
