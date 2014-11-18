using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace me.fengyj.QueryDesc.LinqProvider
{
    public class MathFuncProvider
    {
        public class Abs : SearchCriteriaElementProvider
        {
            public override Expression GetExpression(
                ISearchCriteriaElement searchCriteriaElement,
                Type entityType, 
                ref Type outputType)
            {
                Debug.Assert(searchCriteriaElement != null);
                Debug.Assert(searchCriteriaElement is FilterCriteriaMathFuncElements.Abs);

                var criteria = searchCriteriaElement as FilterCriteriaMathFuncElements.Abs;
                
                Type typeofFieldOrFunc = null;
                var exp = SearchCriteriaProvider.GetSearchCriteriaExpression(
                    criteria.FieldOrFunc, entityType, ref typeofFieldOrFunc) as LambdaExpression;

                outputType = exp.ReturnType;

                return Expression.Lambda(
                    Expression.Call(typeof(Math), "Abs", Type.EmptyTypes, exp.Body),
                    exp.Parameters);
            }
        }
    }
}
