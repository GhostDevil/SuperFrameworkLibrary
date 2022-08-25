using System;

namespace SuperFramework.Channels
{
    public class MessageResponse : SafeObject
    {
        public MessageResponse(object value, params object[] arguments)
        {
            this.Value = value;
            this.Args = arguments;
        }

        public MessageResponse(Exception exception) => Message = exception;

        public object Value { get; set; }
        public object[] Args { get; set; }
        public Exception Message { get; set; }
    }
}
