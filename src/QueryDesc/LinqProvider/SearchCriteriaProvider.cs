using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace me.fengyj.QueryDesc.LinqProvider
{
    public static class SearchCriteriaProvider
    {
        private static Dictionary<Type, SearchCriteriaElementProvider> implements
            = new Dictionary<Type, SearchCriteriaElementProvider>();

        static SearchCriteriaProvider()
        {
            Register(typeof(SearchCriteriaElement.FieldOrFunction), new FieldOrFuncProvider());
            Register(typeof(SearchCriteriaElement.ConstantOrFunction), new ConstantOrFuncProvider());
            Register(typeof(SearchCriteriaElement.Field), new FieldProvider());
            Register(typeof(SearchCriteriaElement.Constant), new ConstantProvider());
            Register(typeof(SearchCriteriaElement.ArrayTypeConstant), new ArrayTypeConstantProvider());
            Register(typeof(SearchCriteriaElement.ComplexTypeConstant), new ComplexTypeConstantProvider());
            Register(typeof(BinaryOpFilterCriteria), new BinaryOpFilterCriteriaProvider());
            Register(typeof(CompositeFilterCriteria.And), new CompositeFilterCriteriaProvider.And());
            Register(typeof(CompositeFilterCriteria.Or), new CompositeFilterCriteriaProvider.Or());
            Register(typeof(CompositeFilterCriteria.Not), new CompositeFilterCriteriaProvider.Not());
            Register(typeof(InOpFilterCriteria), new InOpFilterCriteriaProvider());
            Register(typeof(RefOpFilterCriteria), new RefOpFilterCriteriaProvider());
            Register(typeof(TripleOpFilterCriteria), new TripleOpFilterCriteriaProvider());

            Register(typeof(FilterCriteriaMathFuncElements.Abs), new MathFuncProvider.Abs());
            Register(typeof(FilterCriteriaStringFuncElements.Substring), new StringFuncProvider.Substring());
            Register(typeof(FilterCriteriaStringFuncElements.Trim), new StringFuncProvider.Trim());
        }

        public static void Register(
            Type searchCriteriaElementType,
            SearchCriteriaElementProvider provider)
        {
            Debug.Assert(searchCriteriaElementType.GetInterface(typeof(ISearchCriteriaElement).FullName) != null);
            Debug.Assert(provider != null);

            if (!implements.ContainsKey(searchCriteriaElementType))
                implements.Add(searchCriteriaElementType, provider);
        }

        public static Expression GetSearchCriteriaExpression(
            ISearchCriteriaElement searchCriteriaElement, 
            Type entityType,
            ref Type outputType)
        {
            Debug.Assert(searchCriteriaElement != null);

            Type type = searchCriteriaElement.GetType();

            Debug.Assert(implements.ContainsKey(type));

            var exp = implements[type].GetExpression(searchCriteriaElement, entityType, ref outputType);

            return exp;
        }
    }

    public abstract class SearchCriteriaElementProvider
    {
        public abstract Expression GetExpression(
            ISearchCriteriaElement searchCriteriaElement,
            Type entityType,
            ref Type outputType);
    }
}
