using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace me.fengyj.QueryDesc.LinqProvider
{
    class RefOpFilterCriteriaProvider : SearchCriteriaElementProvider
    {
        public override Expression GetExpression(
            ISearchCriteriaElement searchCriteriaElement,
            Type entityType,
            ref Type returnType)
        {
            Debug.Assert(searchCriteriaElement != null);
            Debug.Assert(searchCriteriaElement is RefOpFilterCriteria);

            var criteria = searchCriteriaElement as RefOpFilterCriteria;
            returnType = typeof(bool);

            Type typeofFieldOrFunc = null;

            var exp1 = SearchCriteriaProvider.GetSearchCriteriaExpression(
                criteria.FieldOrFunc, entityType, ref typeofFieldOrFunc) as LambdaExpression;

            var exp2 = SearchCriteriaProvider.GetSearchCriteriaExpression(
                criteria.FieldOrFunc2, entityType, ref typeofFieldOrFunc) as LambdaExpression;

            if (exp1 == null) return exp2;
            if (exp2 == null) return exp1;

            switch ((FilterOperations.FullFilterOperations)criteria.OperationType)
            {
                case FilterOperations.FullFilterOperations.Equal:
                    return FilterExpressionHelper.Equal(exp1, exp2);
                case FilterOperations.FullFilterOperations.GreaterThan:
                    return FilterExpressionHelper.GreaterThan(exp1, exp2);
                case FilterOperations.FullFilterOperations.GreaterThanOrEqual:
                    return FilterExpressionHelper.GreaterThanOrEqual(exp1, exp2);
                case FilterOperations.FullFilterOperations.LessThan:
                    return FilterExpressionHelper.LessThan(exp1, exp2);
                case FilterOperations.FullFilterOperations.LessThanOrEqual:
                    return FilterExpressionHelper.LessThanOrEqual(exp1, exp2);
                case FilterOperations.FullFilterOperations.NotEqual:
                    return FilterExpressionHelper.NotEqual(exp1, exp2);
                default: throw new NotSupportedException("Not supports the operation");
            }
        }
    }
}
