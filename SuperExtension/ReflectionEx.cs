using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace System.Reflection
{
    public static class ReflectionEx
    {
        public static T GetCustomAttribute<T>(this Type source, bool inherit = false) where T : Attribute
        {
            return (T)source.GetCustomAttributes(inherit).FirstOrDefault(q => q.GetType().Equals(typeof(T)));
        }

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

        /// <summary>
        /// 根据描述获取对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="description"></param>
        /// <returns></returns>
        public static T FindFieldByDesc<T>(this string description)
        {
            System.Reflection.FieldInfo[] fields = typeof(T).GetFields();
            foreach (System.Reflection.FieldInfo field in fields)
            {
                object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);    //获取描述属性
                if (objs.Length > 0 && (objs[0] as DescriptionAttribute).Description == description)
                {
                    return (T)field.GetValue(null);
                }
            }
            return default;
        }
        /// <summary>
        /// 获取特性值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static dynamic GetAttribute<T>(this object obj) where T : Attribute
        {
            string value = obj.ToString();
            FieldInfo field = obj.GetType().GetField(value);
            object[] objs = field.GetCustomAttributes(typeof(T), false);    //获取描述属性
            if (objs == null || objs.Length == 0)    //当描述属性没有时，直接返回名称
                return value;
            T descriptionAttribute = (T)objs[0];
            string fieldName = (descriptionAttribute.TypeId as dynamic).Name;
            fieldName = fieldName.Replace("Attribute", "");
            return descriptionAttribute.GetValueExt(fieldName);
        }
        /// <summary>
        /// 获取描述值
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetDescription(this object enumValue)
        {
            string value = enumValue.ToString();
            FieldInfo field = enumValue.GetType().GetField(value);
            object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);    //获取描述属性
            if (objs == null || objs.Length == 0)    //当描述属性没有时，直接返回名称
                return value;
            DescriptionAttribute descriptionAttribute = (DescriptionAttribute)objs[0];
            return descriptionAttribute.Description;
        }
        
    }
}
