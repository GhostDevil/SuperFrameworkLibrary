namespace System
{
    public static class DynamicEx
    {
        public static dynamic GetValueExt(this object d, string field)
        {
            if (d == null) return default;
            var data = d.GetType().GetProperty(field);
            if (data == null)
                return default;
            else
                return data.GetValue(d);
        }
        public static void SetValueExt(this object d, string field, object val)
        {
            if (d == null || val == null) return;
            var pi = d.GetType().GetProperty(field);
            if (pi == null)
                return;
            else
            {
                var value = Convert.ChangeType(val, pi.PropertyType);
                pi.SetValue(d, value, null);
            }

        }
    }
}
