using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace me.fengyj.QueryDesc.LinqProvider
{
    public class StringFuncProvider
    {
        public class Substring : SearchCriteriaElementProvider
        {
            private static MethodInfo methodWithOneParameter;
            private static MethodInfo methodWithTwoParameter;

            static Substring()
            {
                methodWithOneParameter = typeof(string).GetMethod("Substring", new Type[] { typeof(int) });
                methodWithTwoParameter = typeof(string).GetMethod("Substring", new Type[] { typeof(int), typeof(int) });
            }

            public override Expression GetExpression(
                ISearchCriteriaElement searchCriteriaElement,
                Type entityType,
                ref Type outputType)
            {
                Debug.Assert(searchCriteriaElement != null);
                Debug.Assert(searchCriteriaElement is FilterCriteriaStringFuncElements.Substring);

                outputType = typeof(string);

                var criteria = searchCriteriaElement as FilterCriteriaStringFuncElements.Substring;

                Type typeofFieldOrFunc = null;
                var exp = SearchCriteriaProvider.GetSearchCriteriaExpression(
                    criteria.FieldOrFunc, entityType, ref typeofFieldOrFunc) as LambdaExpression;

                Type intType = typeof(int);

                if (criteria.Length.Val == null)
                {
                    return Expression.Lambda(
                        Expression.Call(
                            exp.Body,
                            methodWithOneParameter,
                            SearchCriteriaProvider.GetSearchCriteriaExpression(criteria.Start, entityType, ref intType)),
                        exp.Parameters);
                }
                else
                {
                    return Expression.Lambda(
                        Expression.Call(
                            exp.Body,
                            methodWithTwoParameter,
                            SearchCriteriaProvider.GetSearchCriteriaExpression(criteria.Start, entityType, ref intType),
                            SearchCriteriaProvider.GetSearchCriteriaExpression(criteria.Length, entityType, ref intType)),
                        exp.Parameters);
                }
            }
        }

        public class Trim : SearchCriteriaElementProvider
        {

            private static MethodInfo methodWithEmptyParameter = typeof(string).GetMethod("Trim", Type.EmptyTypes);
            private static MethodInfo methodWithOneParameter = typeof(string).GetMethod("Trim", new Type[] { typeof(char[]) });


            public override Expression GetExpression(
                ISearchCriteriaElement searchCriteriaElement,
                Type entityType,
                ref Type outputType)
            {
                Debug.Assert(searchCriteriaElement != null);
                Debug.Assert(searchCriteriaElement is FilterCriteriaStringFuncElements.Trim);

                outputType = typeof(string);

                var criteria = searchCriteriaElement as FilterCriteriaStringFuncElements.Trim;

                Type typeofFieldOrFunc = null;
                var exp = SearchCriteriaProvider.GetSearchCriteriaExpression(
                    criteria.FieldOrFunc, entityType, ref typeofFieldOrFunc) as LambdaExpression;

                Type intType = typeof(int);

                return Expression.Lambda(
                       Expression.Call(
                           exp.Body,
                           methodWithEmptyParameter),
                       exp.Parameters);
            }
        }
    }
}
