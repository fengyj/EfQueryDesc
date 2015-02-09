using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace me.fengyj.QueryDesc.LinqProvider
{
    /// <summary>
    /// !!! Performance is not good. !!!
    /// </summary>
    public static class EnumerableTranslator
    {
        public static IEnumerable<TEntity> ApplyTo<TEntity>(this IEnumerable<TEntity> query, FilterCriteria criteria)
        {
            return query.Where(BuildExpression<TEntity>(criteria).Compile());
        }

        private static Expression<Func<TEntity, bool>> BuildExpression<TEntity>(FilterCriteria criteria)
        {
            Type outputType = null;
            var result = SearchCriteriaProvider.GetSearchCriteriaExpression(criteria, typeof(TEntity), ref outputType)
                as Expression<Func<TEntity, bool>>;
            if (result == null) result = obj => true;
            return result;
        }

        public static IOrderedEnumerable<TEntity> ApplyTo<TEntity>(this IEnumerable<TEntity> query, OrderByCriteria criteria)
        {
            var newQuery = GetOrderByExpression<TEntity>(criteria)(query);
            var thenby = criteria.ThenBy;
            while (thenby != null)
            {
                newQuery = GetThenByExpression<TEntity>(thenby)(newQuery);
                thenby = thenby.ThenBy;
            }
            return newQuery;
        }

        public static IEnumerable<TEntity> ApplyTo<TEntity>(this IOrderedEnumerable<TEntity> query, PagingCriteria criteria)
        {
            return query.Skip((criteria.CurrentPage - 1) * criteria.PageSize).Take(criteria.PageSize);
        }

        private static Func<IEnumerable<TEntity>, IOrderedEnumerable<TEntity>> GetOrderByExpression<TEntity>(
            OrderByCriteria criteria)
        {
            Type outputType = null;
            var propExp = SearchCriteriaProvider.GetSearchCriteriaExpression(
                criteria.Field, typeof(TEntity), ref outputType) as LambdaExpression;
            var paramExp = Expression.Parameter(typeof(IEnumerable<TEntity>));

            var cmd = criteria.Order == OrderByCriteria.SortTypes.Asc ? "OrderBy" : "OrderByDescending";
            var body = Expression.Call(
            typeof(Enumerable),
            cmd,
            new Type[] { typeof(TEntity), propExp.Body.Type },
            paramExp,
            propExp);

            var lambdaExp = Expression.Lambda(body, paramExp);
            return lambdaExp.Compile() as Func<IEnumerable<TEntity>, IOrderedEnumerable<TEntity>>;
        }

        private static Func<IOrderedEnumerable<TEntity>, IOrderedEnumerable<TEntity>> GetThenByExpression<TEntity>(
            OrderByCriteria criteria)
        {
            Type outputType = null;
            var propExp = SearchCriteriaProvider.GetSearchCriteriaExpression(
                criteria.Field, typeof(TEntity), ref outputType) as LambdaExpression;
            var paramExp = Expression.Parameter(typeof(IOrderedEnumerable<TEntity>));

            var cmd = criteria.Order == OrderByCriteria.SortTypes.Asc ? "ThenBy" : "ThenByDescending";
            var body = Expression.Call(
            typeof(Enumerable),
            cmd,
            new Type[] { typeof(TEntity), propExp.Body.Type },
            paramExp,
            propExp);

            var lambdaExp = Expression.Lambda(body, paramExp);
            return lambdaExp.Compile() as Func<IOrderedEnumerable<TEntity>, IOrderedEnumerable<TEntity>>;
        }
    }

    public static partial class SearchCriteriaQueryableExtension
    {
        public static IEnumerable<T> Where<T>(this IEnumerable<T> query, FilterCriteria filterCriteria)
        {
            if (filterCriteria == null) return query;
            return EnumerableTranslator.ApplyTo(query, filterCriteria);
        }

        public static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> query, OrderByCriteria orderByCriteria)
        {
            return EnumerableTranslator.ApplyTo(query, orderByCriteria);
        }

        public static IEnumerable<T> Paging<T>(this IOrderedEnumerable<T> query, PagingCriteria pagingCriteria)
        {
            if (pagingCriteria == null) return query;
            return EnumerableTranslator.ApplyTo(query, pagingCriteria);
        }

        public static IEnumerable<T> SearchByCriteria<T>(this IEnumerable<T> query, QueryDescription queryDesc)
        {
            var result = query.Where(queryDesc.Filter);
            if (queryDesc.OrderBy != null)
            {
                var orderedResult = result.OrderBy(queryDesc.OrderBy);
                if (queryDesc.Paging != null)
                    result = orderedResult.Paging(queryDesc.Paging);
                else
                    result = orderedResult;
            }
            return result;
        }
    }
}
