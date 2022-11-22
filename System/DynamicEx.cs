
namespace System
{
    public static class DynamicEx
    {
        /// <summary>
        /// 获取对象的属性或字段值
        /// </summary>
        /// <param name="d"></param>
        /// <param name="property">属性名称</param>
        /// <returns></returns>
        public static dynamic GetValueEx<T>(this T d, string property) where T : class
        {
            var type = d?.GetType() ?? null;
            if (type == null) return null;
            var data = type.GetProperty(property);
            if (data == null)
            {
                var field = d.GetType().GetField(property);
                if (field == null)
                    return default;
                else return field.GetValue(d);
            }
            else
                return data.GetValue(d);
        }
        /// <summary>
        /// 设置对象的属性或字段值
        /// </summary>
        /// <param name="d"></param>
        /// <param name="property">属性名称</param>
        /// <param name="value">值</param>
        public static void SetValueEx<T>(this T d, string property, object value) where T : class
        {
            if (d == null || value == null) return;
            var pi = d.GetType().GetProperty(property);
            if (pi == null)
            {
                var field = d.GetType().GetField(property);
                if (field == null)
                    return;
                else field.SetValue(d, Convert.ChangeType(value, field.FieldType));
            }
            else
            {
                var resutl = Convert.ChangeType(value, pi.PropertyType);
                pi.SetValue(d, resutl, null);
            }
        }
        
    }
}
