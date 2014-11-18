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
    class ConstantProvider : SearchCriteriaElementProvider
    {
        public override Expression GetExpression(
            ISearchCriteriaElement searchCriteriaElement,
            Type entityType,
            ref Type returnType)
        {
            Debug.Assert(searchCriteriaElement != null);
            Debug.Assert(searchCriteriaElement is SearchCriteriaElement.Constant);
            Debug.Assert(returnType != null);

            var constant = searchCriteriaElement as SearchCriteriaElement.Constant;

            try
            {
                return ExpressionUtils.GetConstantExpFromString(constant.Val, returnType);
            }
            catch (ArgumentNullException)
            {
                return null;
            }
            catch (FormatException fmtExp)
            {
                throw new FilterCriteriaCastException(string.Empty, fmtExp)
                {
                    Val = constant.Val,
                    TargetType = returnType
                };
            }
            catch (KeyNotFoundException keyNotFoundExp)
            {
                throw new FilterCriteriaCastException("Don't know how to convert the value to target type.", keyNotFoundExp)
                {
                    Val = constant.Val,
                    TargetType = returnType
                };
            }
        }

        
    }
}
