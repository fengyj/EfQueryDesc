using me.fengyj.QueryDesc.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace me.fengyj.QueryDesc.LinqProvider
{
    class ArrayTypeConstantProvider : SearchCriteriaElementProvider
    {
        public override Expression GetExpression(
            ISearchCriteriaElement searchCriteriaElement,
            Type entityType,
            ref Type returnType)
        {
            Debug.Assert(searchCriteriaElement != null);
            Debug.Assert(searchCriteriaElement is SearchCriteriaElement.ArrayTypeConstant);
            Debug.Assert(returnType != null);

            var constant = searchCriteriaElement as SearchCriteriaElement.ArrayTypeConstant;

            Debug.Assert(returnType.IsArray, "The type of this constant should be an array.");
            if (constant.Val == null)
                return Expression.Constant(null, returnType);
            var eleType = returnType.GetElementType();
            return ExpressionUtils.ConstantExpHelper.GetConstantExp(constant.Val, eleType);
        }
    }
}
