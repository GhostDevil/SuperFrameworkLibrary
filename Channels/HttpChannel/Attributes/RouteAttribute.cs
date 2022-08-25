using System;

namespace SuperFramework.Channels.Channel
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RouteAttribute : Attribute
    {
        public RouteAttribute(string path) => Value = path;

        public string Value { get; }
    }
}
