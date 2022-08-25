using System;
using System.Reflection;

namespace SuperFramework.Channels
{
    public class MessageRequest : SafeObject
    {
        public MessageRequest(Type type, MethodInfo method, params object[] arguments)
        {
            Type = type;
            Method = method;
            Args = arguments;
        }

        public Type Type { get; }
        public MethodInfo Method { get; }
        public object[] Args { get; }
    }
}
