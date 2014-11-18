using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace me.fengyj.QueryDesc.LinqProvider
{
    class TripleOpFilterCriteriaProvider : SearchCriteriaElementProvider
    {
        public override Expression GetExpression(
            ISearchCriteriaElement searchCriteriaElement,
            Type entityType,
            ref Type returnType)
        {
            Debug.Assert(searchCriteriaElement != null);
            Debug.Assert(searchCriteriaElement is TripleOpFilterCriteria);

            var criteria = searchCriteriaElement as TripleOpFilterCriteria;
            returnType = typeof(bool);

            switch ((FilterOperations.FullFilterOperations)criteria.OperationType)
            {
                case FilterOperations.FullFilterOperations.Between:
                    var and = new CompositeFilterCriteria.And
                    {
                        Arg1 = new BinaryOpFilterCriteria
                        {
                            Arg = criteria.Arg1,
                            FieldOrFunc = criteria.FieldOrFunc,
                            OperationType = (int)FilterOperations.BinaryFilterOperations.GreaterThanOrEqual
                        },
                        Arg2 = new BinaryOpFilterCriteria
                        {
                            Arg = criteria.Arg2,
                            FieldOrFunc = criteria.FieldOrFunc,
                            OperationType = (int)FilterOperations.BinaryFilterOperations.LessThanOrEqual
                        }
                    };
                    return SearchCriteriaProvider.GetSearchCriteriaExpression(and, entityType, ref returnType);
                default: throw new NotSupportedException("Not supports the operation");
            }
        }
    }
}
