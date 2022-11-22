using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SuperFramework.SuperLinq
{
    /// <summary>
    /// 主要用于数据集，排序、分页等功能
    /// </summary>
    public static class LinqPage<TSource> where TSource : class
    {
        /// <summary>
        /// 根据指定属性名称对序列进行排序
        /// </summary>
        /// <typeparam name="TSource">数据实体类</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="property">属性名称</param>
        /// <param name="descending">是否降序</param>
        /// <returns></returns>
        public static IQueryable<TSource> OrderBy(IQueryable<TSource> source, string property, bool descending) 
        {
            ParameterExpression param = Expression.Parameter(typeof(TSource), property);
            PropertyInfo pi = typeof(TSource).GetProperty(property);
            MemberExpression selector = Expression.MakeMemberAccess(param, pi);
            LambdaExpression le = Expression.Lambda(selector, param);
            string methodName = descending ? "OrderByDescending" : "OrderBy";
            MethodCallExpression resultExp = Expression.Call(typeof(Queryable), methodName, new Type[] { typeof(TSource), pi.PropertyType }, source.Expression, le);
            return source.Provider.CreateQuery<TSource>(resultExp);
        }
        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="source">数据源</param>
        /// <param name="pageNumber">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        public static IQueryable<TSource> DataPaging(IQueryable<TSource> source, int pageNumber,int pageSize)
        {
            return source.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }
        /// <summary>
        /// 排序并分页 
        /// </summary>
        /// <param name="source">数据源</param>
        /// <param name="gridPager">操作参数</param>
        /// <returns></returns>
        public static IQueryable<TSource> SortingAndPaging(IQueryable<TSource> source, GridPager gridPager)
        {
            if (gridPager != null)
            {
                IQueryable<TSource> query = OrderBy(source, gridPager.Property, gridPager.IsDesc);
                query = source.AsQueryable();
                return DataPaging(query, gridPager.Page,gridPager.Rows);
            }
            return source;
        }

        #region 分页排序参数类
        /// <summary>
        /// 分页辅助，用于分页排序，限定分页参数
        /// </summary>
        public class GridPager
        {
            /// <summary>
            /// 每页行数
            /// </summary>
            public int Rows { get; set; }
            /// <summary>
            /// 当前页码
            /// </summary>
            public int Page { get; set; }
            /// <summary>
            /// 排序列
            /// </summary>
            public string Property { get; set; }
            /// <summary>
            /// 排序方式 是否降序
            /// </summary>
            public bool IsDesc { get; set; }
            /// <summary>
            /// 总行数
            /// </summary>
            public int TotalRows { get; set; }
          
        }
        #endregion

    }
}
