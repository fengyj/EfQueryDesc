using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace me.fengyj.QueryDesc.LinqProvider
{
    class ComplexTypeConstantProvider : SearchCriteriaElementProvider
    {
        public override Expression GetExpression(
            ISearchCriteriaElement searchCriteriaElement,
            Type entityType,
            ref Type returnType)
        {
            Debug.Assert(searchCriteriaElement != null);
            Debug.Assert(searchCriteriaElement is SearchCriteriaElement.ComplexTypeConstant);

            var constant = searchCriteriaElement as SearchCriteriaElement.ComplexTypeConstant;
            
            returnType = constant.ObjType;

            var exp = Expression.Constant(constant.Val, returnType);
            return exp;
        }
    }
}
