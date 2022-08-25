using System;
using System.ComponentModel;

namespace SuperFramework.Channels.Channel
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ContentTypeAttribute : Attribute
    {
        public ContentTypeAttribute(ContentType type) => Value = type;

        public ContentType Value { get; }
    }

    public enum ContentType
    {
        [Description("multipart/form-data;")]
        FormData,
        [Description("application/json; charset=utf-8")]
        Json,
        [Description("")]
        XML,
        [Description("")]
        HTML
    }
}
