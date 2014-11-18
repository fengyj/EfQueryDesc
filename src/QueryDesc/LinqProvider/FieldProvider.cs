
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using me.fengyj.QueryDesc.Utils;

namespace me.fengyj.QueryDesc.LinqProvider
{
    class FieldProvider : SearchCriteriaElementProvider
    {
        public override Expression GetExpression(
            ISearchCriteriaElement searchCriteriaElement,
            Type entityType,
            ref Type returnType)
        {
            Debug.Assert(searchCriteriaElement != null);
            Debug.Assert(searchCriteriaElement is SearchCriteriaElement.Field);

            var field = searchCriteriaElement as SearchCriteriaElement.Field;

            //var entityType = Type.GetType(field.Entity);

            var exp = ExpressionUtils.GetPropertyExp(entityType, field.FieldName);
            returnType = exp.ReturnType;
            return exp;
        }
    }
}
