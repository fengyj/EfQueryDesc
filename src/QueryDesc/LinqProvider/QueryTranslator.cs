using me.fengyj.QueryDesc.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace me.fengyj.QueryDesc.LinqProvider
{
    public static class QueryTranslator
    {
        public static IQueryable<TEntity> ApplyTo<TEntity>(this IQueryable<TEntity> query, FilterCriteria criteria)
        {
            return query.Where(BuildExpression<TEntity>(criteria));
        }

        private static Expression<Func<TEntity, bool>> BuildExpression<TEntity>(FilterCriteria criteria)
        {
            Type outputType = null;
            var result = SearchCriteriaProvider.GetSearchCriteriaExpression(criteria, typeof(TEntity), ref outputType)
                as Expression<Func<TEntity, bool>>;
            if (result == null) result = obj => true;
            return result;
        }

        public static IOrderedQueryable<TEntity> ApplyTo<TEntity>(this IQueryable<TEntity> query, OrderByCriteria criteria)
        {
            Func<LambdaExpression, IQueryable<TEntity>, IOrderedQueryable<TEntity>> func = (exp, q) =>
            {
                var queryExp = query.Expression;
                var expVisitor = new ExpressionUtils.ExpressionReplaceVisitor(exp.Parameters[0], q.Expression);
                var callExp = expVisitor.Visit(exp.Body);
                return (IOrderedQueryable<TEntity>)q.Provider.CreateQuery<TEntity>(callExp);
            };

            var entityType = typeof(TEntity);
            var lambdaExp = GetExpression<TEntity>(criteria, true);

            var newQuery = func(lambdaExp, query);

            var thenby = criteria.ThenBy;
            while (thenby != null)
            {
                lambdaExp = GetExpression<TEntity>(thenby, false);
                newQuery = func(lambdaExp, newQuery);
                thenby = thenby.ThenBy;
            }
            return newQuery;
        }

        public static IQueryable<TEntity> ApplyTo<TEntity>(this IOrderedQueryable<TEntity> query, PagingCriteria criteria)
        {
            return query.Skip((criteria.CurrentPage - 1) * criteria.PageSize).Take(criteria.PageSize);
        }

        private static LambdaExpression GetExpression<TEntity>(OrderByCriteria criteria, bool isFirstOrderCriteria)
        {
            Type outputType = null;
            var propExp = SearchCriteriaProvider.GetSearchCriteriaExpression(
                criteria.Field, typeof(TEntity), ref outputType) as LambdaExpression;
            var paramExp = isFirstOrderCriteria
                ? Expression.Parameter(typeof(IQueryable<TEntity>))
                : Expression.Parameter(typeof(IOrderedQueryable<TEntity>));

            var cmd = string.Empty;
            if (isFirstOrderCriteria)
            {
                cmd = criteria.Order == OrderByCriteria.SortTypes.Asc ? "OrderBy" : "OrderByDescending";
            }
            else
            {
                cmd = criteria.Order == OrderByCriteria.SortTypes.Asc ? "ThenBy" : "ThenByDescending";
            }

            var body = Expression.Call(
                typeof(Queryable),
                cmd,
                new Type[] { typeof(TEntity), propExp.Body.Type },
                paramExp,
                Expression.Constant(propExp));

            var lambdaExp = Expression.Lambda(body, paramExp);
            return lambdaExp;
        }
    }

    public static partial class SearchCriteriaQueryableExtension
    {
        public static IQueryable<T> Where<T>(this IQueryable<T> query, FilterCriteria filterCriteria)
        {
            if (filterCriteria == null) return query;
            return QueryTranslator.ApplyTo(query, filterCriteria);
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> query, OrderByCriteria orderByCriteria)
        {
            return QueryTranslator.ApplyTo(query, orderByCriteria);
        }

        public static IQueryable<T> Paging<T>(this IOrderedQueryable<T> query, PagingCriteria pagingCriteria)
        {
            if (pagingCriteria == null) return query;
            return QueryTranslator.ApplyTo(query, pagingCriteria);
        }

        public static IQueryable<T> SearchByCriteria<T>(this IQueryable<T> query, QueryDescription queryDesc)
        {
            var result = query.Where(queryDesc.Filter);
            if(queryDesc.OrderBy !=null)
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
