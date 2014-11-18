//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace me.fengyj.QueryDesc
//{
//    public interface ISearchCriteria
//    {
//        IQueryable<T> ApplyTo<T>(IQueryable<T> query);
//    }

//    public static class SearchCriteriaQueryableExtension
//    {
//        public static IQueryable<T> Where<T>(this IQueryable<T> query, FilterCriteria filterCriteria)
//        {
//            if (filterCriteria == null) return query;
//            return filterCriteria.ApplyTo<T>(query);
//        }

//        public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, OrderByCriteria orderByCriteria)
//        {
//            if (orderByCriteria == null) return query;
//            return orderByCriteria.ApplyTo(query);
//        }

//        public static IQueryable<T> Paging<T>(this IQueryable<T> query, PagingCriteria pagingCriteria)
//        {
//            if (pagingCriteria == null) return query;
//            return pagingCriteria.ApplyTo(query);
//        }
//    }
//}
