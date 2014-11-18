using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace me.fengyj.QueryDesc.LinqProvider
{
    class InOpFilterCriteriaProvider : SearchCriteriaElementProvider
    {
        public override Expression GetExpression(
            ISearchCriteriaElement searchCriteriaElement,
            Type entityType,
            ref Type returnType)
        {
            Debug.Assert(searchCriteriaElement != null);
            Debug.Assert(searchCriteriaElement is InOpFilterCriteria);

            var criteria = searchCriteriaElement as InOpFilterCriteria;
            returnType = typeof(bool);

            Type typeofFieldOrFunc = null;

            var exp1 = SearchCriteriaProvider.GetSearchCriteriaExpression(
                criteria.FieldOrFunc, entityType, ref typeofFieldOrFunc) as LambdaExpression;
            if (exp1 == null) return null;

            typeofFieldOrFunc = typeofFieldOrFunc.MakeArrayType();
            var exp2 = SearchCriteriaProvider.GetSearchCriteriaExpression(
                criteria.Arg, entityType, ref typeofFieldOrFunc) as ConstantExpression;
            if (exp2 == null) return null;

            return FilterExpressionHelper.In(exp1, exp2);
        }
    }
}
