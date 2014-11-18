using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace me.fengyj.QueryDesc.LinqProvider
{
    class BinaryOpFilterCriteriaProvider : SearchCriteriaElementProvider
    {
        public override Expression GetExpression(
            ISearchCriteriaElement searchCriteriaElement,
            Type entityType,
            ref Type returnType)
        {
            Debug.Assert(searchCriteriaElement != null);
            Debug.Assert(searchCriteriaElement is BinaryOpFilterCriteria);

            var criteria = searchCriteriaElement as BinaryOpFilterCriteria;
            returnType = typeof(bool);

            Type typeofFieldOrFunc = null;

            var exp1 = SearchCriteriaProvider.GetSearchCriteriaExpression(
                criteria.FieldOrFunc, entityType, ref typeofFieldOrFunc) as LambdaExpression;
            if (exp1 == null) return null;

            Expression exp2 = null;
            try
            {
                if (criteria.Arg == null)
                {
                    // if the ReturnType is not class type or nullable type, will throw ArgumentException
                    exp2 = Expression.Constant(null, typeofFieldOrFunc);
                }
                else
                {
                    exp2 = SearchCriteriaProvider.GetSearchCriteriaExpression(
                        criteria.Arg, entityType, ref typeofFieldOrFunc);
                }
                if (exp2 == null) return null;
            }
            catch (ArgumentException)
            {
                return null;
            }
            catch (FilterCriteriaCastException ex)
            {
                if (criteria.FieldOrFunc is SearchCriteriaElement.Field)
                    ex.FieldName = (criteria.FieldOrFunc as SearchCriteriaElement.Field).FieldName;
                else
                    ex.FuncName = criteria.FieldOrFunc.FunctionElement.GetType().Name;
                ex.Operator = criteria.OperationType;
                throw;
            }

            switch ((FilterOperations.FullFilterOperations)criteria.OperationType)
            {
                case FilterOperations.FullFilterOperations.Contains:
                    Debug.Assert(exp1.ReturnType == typeof(string), "Contains operator only works with string type");
                    return FilterExpressionHelper.Contains(exp1, exp2 as ConstantExpression);
                case FilterOperations.FullFilterOperations.EndsWith:
                    Debug.Assert(exp1.ReturnType == typeof(string), "EndsWith operator only works with string type");
                    return FilterExpressionHelper.EndsWith(exp1, exp2 as ConstantExpression);
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
                case FilterOperations.FullFilterOperations.StartsWith:
                    Debug.Assert(exp1.ReturnType == typeof(string), "StartsWith operator only works with string type");
                    return FilterExpressionHelper.StartsWith(exp1, exp2 as ConstantExpression);
                default: throw new NotSupportedException("Not supports the operation");
            }
        }
    }
}
