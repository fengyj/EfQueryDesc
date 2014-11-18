using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace me.fengyj.QueryDesc.LinqProvider
{
    class ConstantOrFuncProvider : SearchCriteriaElementProvider
    {
        public override Expression GetExpression(
            ISearchCriteriaElement searchCriteriaElement,
            Type entityType,
            ref Type returnType)
        {
            Debug.Assert(searchCriteriaElement != null);
            Debug.Assert(searchCriteriaElement is SearchCriteriaElement.ConstantOrFunction);

            var field = searchCriteriaElement as SearchCriteriaElement.ConstantOrFunction;

            Debug.Assert(field.ElementType == ElementTypes.Function);

            return SearchCriteriaProvider.GetSearchCriteriaExpression(
                field.FunctionElement, entityType, ref returnType);
        }
    }
}
