using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Controls;

namespace SuperFramework.SuperWindows
{
    /// <summary>
    /// 过lambda表达式树来获取属性信息
    /// </summary>
    public static class PropertyInfoHelper
    {
        /// <summary>
        /// 获取指定属性信息（非String类型存在装箱与拆箱）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="select"></param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyInfo<T>(Expression<Func<T, dynamic>> select)
        {
            var body = select.Body;
            if (body.NodeType == ExpressionType.Convert)
            {
                var o = (body as UnaryExpression).Operand;
                return (o as MemberExpression).Member as PropertyInfo;
            }
            else if (body.NodeType == ExpressionType.MemberAccess)
            {
                return (body as MemberExpression).Member as PropertyInfo;
            }
            return null;
        }

        /// <summary>
        /// 获取指定属性信息（需要明确指定属性类型，但不存在装箱与拆箱）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TR"></typeparam>
        /// <param name="select"></param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyInfo<T, TR>(Expression<Func<T, TR>> select)
        {
            var body = select.Body;
            if (body.NodeType == ExpressionType.Convert)
            {
                var o = (body as UnaryExpression).Operand;
                return (o as MemberExpression).Member as PropertyInfo;
            }
            else if (body.NodeType == ExpressionType.MemberAccess)
            {
                return (body as MemberExpression).Member as PropertyInfo;
            }
            return null;
        }

        /// <summary>
        /// 获取类型的所有属性信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="select"></param>
        /// <returns></returns>
        public static PropertyInfo[] GetPropertyInfos<T>(Expression<Func<T, dynamic>> select)
        {
            var body = select.Body;
            if (body.NodeType == ExpressionType.Parameter)
            {
                return (body as ParameterExpression).Type.GetProperties();
            }
            else if (body.NodeType == ExpressionType.New)
            {
                return (body as NewExpression).Members.Select(m => m as PropertyInfo).ToArray();
            }
            return null;
        }

        #region 内部委托
        private delegate object MethodInvoker(Control control, string methodName, params object[] args);
        private delegate object PropertyGetInvoker(Control control, object noncontrol, string propertyName);
        private delegate void PropertySetInvoker(Control control, object noncontrol, string propertyName, object value);
        #endregion

        #region 获取指定控件的公共属性私有方法
        private static PropertyInfo GetPropertyInfo(Control control, object noncontrol, string attributeName)
        {
            if (control != null && !string.IsNullOrEmpty(attributeName))
            {
                Type t;
                if (noncontrol != null)
                    t = noncontrol.GetType();
                else
                    t = control.GetType();

                PropertyInfo pi = t.GetProperty(attributeName);

                if (pi == null)
                    throw new InvalidOperationException(string.Format("Can't find property {0} in {1}.", attributeName, t.ToString()));
                return pi;
            }
            else
                throw new ArgumentNullException("Invalid argument.");
        }
        #endregion

        #region 调用主界面控件的某个方法，并返回方法执行结果。
        /// <summary>
        /// 调用主界面控件的某个方法，并返回方法执行结果。（调用InvokeHelper.Invoke(控件, "方法名称", 参数);）
        /// </summary>
        /// <param name="control">控件</param>
        /// <param name="methodName">方法名称</param>
        /// <param name="args">方法参数</param>
        /// <returns>返回方法执行结果</returns>
        public static object Invoke(Control control, string methodName, params object[] args)
        {
            if (control != null && !string.IsNullOrEmpty(methodName))
            {

                MethodInfo mi;
                if (args != null && args.Length > 0)
                    {
                        Type[] types = new Type[args.Length];
                        for (int i = 0; i < args.Length; i++)
                        {
                            if (args[i] != null)
                                types[i] = args[i].GetType();
                        }

                        mi = control.GetType().GetMethod(methodName, types);
                    }
                    else
                        mi = control.GetType().GetMethod(methodName);

                    // check method info you get
                    if (mi != null)
                        return mi.Invoke(control, args);
                    else
                        throw new InvalidOperationException("Invalid method.");
                
            }
            else
                throw new ArgumentNullException("Invalid argument.");
        }
        #endregion

        #region 获取主界面控件的某个属性
        /// <summary>
        /// 获取主界面控件的某个属性
        /// </summary>
        /// <param name="control">控件</param>
        /// <param name="propertyName">属性名称</param>
        /// <returns>返回控件属性值</returns>
        public static object GetAttribute(Control control, string attributeName)
        {
            return GetAttribute(control, null, attributeName);
        }
        /// <summary>
        /// 获取主界面控件的某个属性
        /// </summary>
        /// <param name="control">控件</param>
        /// <param name="nonControl"></param>
        /// <param name="attributeName">属性名称</param>
        /// <returns>返回控件属性值</returns>
        public static object GetAttribute(Control control, object nonControl, string attributeName)
        {
            if (control != null && !string.IsNullOrEmpty(attributeName))
            {
               
                    PropertyInfo pi = GetPropertyInfo(control, nonControl, attributeName);
                    object invokee = (nonControl == null) ? control : nonControl;

                    if (pi != null)
                        if (pi.CanRead)
                            return pi.GetValue(invokee, null);
                        else
                            throw new FieldAccessException(string.Format("{0}.{1} is a write-only property.", invokee.GetType().ToString(), attributeName));
                    return null;
                
            }
            else
                throw new ArgumentNullException("Invalid argument.");
        }
        #endregion

        #region 设置主界面控件的某个属性
        /// <summary>
        /// 设置主界面控件的某个属性
        /// </summary>
        /// <param name="control">控件</param>
        /// <param name="attributeName">属性名称</param>
        /// <param name="value">属性值</param>
        public static void SetAttribute(Control control, string attributeName, object value)
        {
            SetAttribute(control, null, attributeName, value);
        }
        /// <summary>
        /// 设置主界面控件的某个属性
        /// </summary>
        /// <param name="control">控件</param>
        /// <param name="nonControl"></param>
        /// <param name="attributeName">属性名称</param>
        /// <param name="value">属性值</param>
        public static void SetAttribute(Control control, object nonControl, string attributeName, object value)
        {
            if (control != null && !string.IsNullOrEmpty(attributeName))
            {
                
                    PropertyInfo pi = GetPropertyInfo(control, nonControl, attributeName);
                    object invokee = nonControl ?? control;

                    if (pi != null)
                        if (pi.CanWrite)
                            pi.SetValue(invokee, value, null);
                        else
                            throw new FieldAccessException(string.Format("{0}.{1} is a read-only property.", invokee.GetType().ToString(), attributeName));
                
            }
            else
                throw new ArgumentNullException("Invalid argument.");
        }
        #endregion
    }
}
