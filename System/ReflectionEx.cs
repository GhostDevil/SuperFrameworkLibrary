using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace System.Reflection
{
    public static class ReflectionEx
    {
        /// <summary>
        /// 获取类型和它的描述
        /// </summary>
        /// <param name="type">需要读的类型</param>
        /// <returns>键值对</returns>
        public static NameValueCollection GetNameAndDesc(this Type type)
        {
            NameValueCollection nvc = new();
            Type typeDescription = typeof(DescriptionAttribute);
            System.Reflection.FieldInfo[] fields = type.GetFields();
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.IsEnum)
                {
                    string strValue = ((int)type.InvokeMember(field.Name, BindingFlags.GetField, null, null, null)).ToString();
                    object[] arr = field.GetCustomAttributes(typeDescription, true);
                    string strText;
                    if (arr.Length > 0)
                    {
                        DescriptionAttribute aa = (DescriptionAttribute)arr[0];
                        strText = aa.Description;
                    }
                    else
                    {
                        strText = field.Name;
                    }
                    nvc.Add(strText, strValue);
                }
            }
            return nvc;
        }
        /// <summary>
        /// 获取自定义标记
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="inherit">是否继承</param>
        /// <returns></returns>
        public static T GetCustomAttribute<T>(this Type source, bool inherit = false) where T : Attribute
        {
            return (T)source.GetCustomAttributes(inherit).FirstOrDefault(q => q.GetType().Equals(typeof(T)));
        }
        /// <summary>
        /// 获取自定义标记
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static T GetCustomAttribute<T>(this MethodBase source, bool inherit = false) where T : Attribute
        {
            return (T)source.GetCustomAttributes(inherit).FirstOrDefault(q => q.GetType().Equals(typeof(T)));
        }

        public static string GetTypeName(this Type source)
        {
            var typeName = source.AssemblyQualifiedName;
            var match = Regex.Match(typeName, "([^,]+,[^,]+)", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
            if (match.Success && match.Groups.Count >= 2)
                typeName = match.Groups[1].Value;
            return typeName;
        }
        
    }
}
