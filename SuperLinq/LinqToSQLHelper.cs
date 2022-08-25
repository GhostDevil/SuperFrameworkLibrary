using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SuperFramework.SuperLinq
{

    /// <summary>
    /// Linq通用数据访问类
    /// 指定TDataBase来代替后面要使用的数据上下文(指代)
    /// where：说明指代的类型
    /// new：限定必须有一个不带参数的构造函数
    /// </summary>
    /// <typeparam name="TDataBase"></typeparam>
    public class LinqToSQLHelper<TDataBase> where TDataBase : DataContext, new()
    {
        //private readonly string connStr = "server=.;uid=sa;pwd=123456;database=StuDB";
        TDataBase db = default(TDataBase);
        /// <summary>
        /// 创建数据库连接 :"server=.;uid=sa;pwd=123456;database=StuDB"
        /// </summary>
        public LinqToSQLHelper(string connectionString)
        {
            db = new TDataBase();
            db.Connection.ConnectionString = connectionString;
        }

        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> GetList<T>() where T : class
        {
            return db.GetTable<T>().ToList();
        }

        /// <summary>
        /// 按条件查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">Lambda表达式</param>
        /// <returns></returns>
        public List<T> GetList<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return db.GetTable<T>().Where(predicate).ToList();
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public T GetEntity<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return db.GetTable<T>().Where(predicate).FirstOrDefault();
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        public void InsertEntity<T>(T entity) where T : class
        {
            try
            {
                //将对象保存到上下文当中
                db.GetTable<T>().InsertOnSubmit(entity);
                //提交更改
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 修改实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        public void UpdateEntity<T>(T entity) where T : class
        {
            try
            {
                //将新实体附加到上下文
                db.GetTable<T>().Attach(entity);
                //刷新数据库
                db.Refresh(RefreshMode.KeepCurrentValues, entity);
                //提交更改
                db.SubmitChanges(ConflictMode.ContinueOnConflict);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        public void DeleteEntity<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            try
            {
                //获取要删除的实体
                var entity = db.GetTable<T>().Where(predicate).FirstOrDefault();
                db.GetTable<T>().DeleteOnSubmit(entity);
                db.SubmitChanges(ConflictMode.ContinueOnConflict);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="sortExpression"></param>
        /// <param name="sortDirection"></param>
        /// <returns></returns>
        public static IQueryable<T> DataSorting<T>(IQueryable<T> source, string sortExpression, string sortDirection)
        {
            string sortingDir = string.Empty;
            if (sortDirection.ToUpper().Trim() == "ASC")
                sortingDir = "OrderBy";
            else if (sortDirection.ToUpper().Trim() == "DESC")
                sortingDir = "OrderByDescending";
            ParameterExpression param = Expression.Parameter(typeof(T), sortExpression);
            PropertyInfo pi = typeof(T).GetProperty(sortExpression);
            Type[] types = new Type[2];
            types[0] = typeof(T);
            types[1] = pi.PropertyType;
            Expression expr = Expression.Call(typeof(Queryable), sortingDir, types, source.Expression, Expression.Lambda(Expression.Property(param, sortExpression), param));
            IQueryable<T> query = source.AsQueryable().Provider.CreateQuery<T>(expr);
            return query;
        }
        /// <summary>
        /// 分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static IQueryable<T> DataPaging<T>(IQueryable<T> source, int pageNumber, int pageSize)
        {
            return source.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }
        /// <summary>
        /// 排序并分页 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="sortExpression"></param>
        /// <param name="sortDirection"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static IQueryable<T> SortingAndPaging<T>(IQueryable<T> source, string sortExpression, string sortDirection, int pageNumber, int pageSize)
        {
            if (!string.IsNullOrEmpty(sortDirection))
            {
                //  IQueryable<T> query = DataSorting<T>(source, sortExpression, sortDirection);
            }
            IQueryable<T> query = source.AsQueryable();
            return DataPaging(query, pageNumber, pageSize);
        }
    }
}
