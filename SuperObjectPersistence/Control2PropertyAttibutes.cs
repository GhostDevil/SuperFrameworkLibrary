using System;

namespace SuperFramework.SuperObjectPersistence
{
    /// <summary> 控件转为属性的转换类
    /// （凡是打了此标记的控件才能够被转换)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public class Control2PropertyAttibutes : Attribute
    {
        /// <summary> 控件类型
        /// </summary>
        private Type controlType;

        /// <summary> 控件名
        /// </summary>
        private string controlName;

        /// <summary> 控件值字段名
        /// </summary>
        private string propertyName;

        /// <summary> 控件类型
        /// </summary>
        public Type ControlType
        {
            get { return controlType; }
            set { controlType = value; }
        }

        /// <summary> 控件名
        /// </summary>
        public string ControlName
        {
            get { return controlName; }
            set { controlName = value; }
        }

        /// <summary> 控件值字段名
        /// </summary>
        public string PropertyName
        {
            get { return propertyName; }
            set { propertyName = value; }
        }
    }
}
