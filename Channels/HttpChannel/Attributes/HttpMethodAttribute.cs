using System;

namespace SuperFramework.Channels.Channel
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class HttpMethodAttribute : Attribute
    {
        public HttpMethodAttribute(HttpMethod method) => Method = method;

        public HttpMethod Method { get; }
    }

    public enum HttpMethod
    {
        GET,
        POST,
        HEAD,
        OPTIONS,
        PUT,
        DELETE,
        TRACE,
        CONNECT
    }
}