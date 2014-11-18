using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace me.fengyj.QueryDesc.LinqProvider
{
    class CompositeFilterCriteriaProvider
    {
        public class And : SearchCriteriaElementProvider
        {
            public override Expression GetExpression(
                ISearchCriteriaElement searchCriteriaElement,
                Type entityType,
                ref Type returnType)
            {
                Debug.Assert(searchCriteriaElement != null);
                Debug.Assert(searchCriteriaElement is CompositeFilterCriteria.And);

                var criteria = searchCriteriaElement as CompositeFilterCriteria.And;
                returnType = typeof(bool);

                Type argType = null;

                var exp1 = SearchCriteriaProvider.GetSearchCriteriaExpression(
                    criteria.Arg1, entityType, ref argType) as LambdaExpression;

                if (exp1 != null)
                    Debug.Assert(argType == typeof(bool));

                var exp2 = SearchCriteriaProvider.GetSearchCriteriaExpression(
                    criteria.Arg2, entityType, ref argType) as LambdaExpression;

                if (exp2 != null)
                    Debug.Assert(argType == typeof(bool));

                if (exp1 == null) return exp2;
                if (exp2 == null) return exp1;
                return FilterExpressionHelper.And(exp1, exp2);
            }
        }

        public class Or : SearchCriteriaElementProvider
        {
            public override Expression GetExpression(
                ISearchCriteriaElement searchCriteriaElement,
                Type entityType,
                ref Type returnType)
            {
                Debug.Assert(searchCriteriaElement != null);
                Debug.Assert(searchCriteriaElement is CompositeFilterCriteria.Or);

                var criteria = searchCriteriaElement as CompositeFilterCriteria.Or;
                returnType = typeof(bool);

                Type argType = null;

                var exp1 = SearchCriteriaProvider.GetSearchCriteriaExpression(
                    criteria.Arg1, entityType, ref argType) as LambdaExpression;

                if (exp1 != null)
                    Debug.Assert(argType == typeof(bool));

                var exp2 = SearchCriteriaProvider.GetSearchCriteriaExpression(
                    criteria.Arg2, entityType, ref argType) as LambdaExpression;

                if (exp2 != null)
                    Debug.Assert(argType == typeof(bool));

                if (exp1 == null) return exp2;
                if (exp2 == null) return exp1;
                return FilterExpressionHelper.Or(exp1, exp2);
            }
        }

        public class Not : SearchCriteriaElementProvider
        {
            public override Expression GetExpression(
                ISearchCriteriaElement searchCriteriaElement,
                Type entityType,
                ref Type returnType)
            {
                Debug.Assert(searchCriteriaElement != null);
                Debug.Assert(searchCriteriaElement is CompositeFilterCriteria.Not);

                var criteria = searchCriteriaElement as CompositeFilterCriteria.Not;
                returnType = typeof(bool);

                Type argType = null;

                var exp = SearchCriteriaProvider.GetSearchCriteriaExpression(
                    criteria.Arg, entityType, ref argType) as LambdaExpression;

                if (exp == null) return null;

                Debug.Assert(argType == typeof(bool));

                return FilterExpressionHelper.Not(exp);
            }
        }
    }
}
