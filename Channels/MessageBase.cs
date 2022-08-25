using SuperFramework.Channels.Channel;
using System;

namespace SuperFramework.Channels
{
    public abstract class MessageBase 
    {
        public Uri Url { get; protected set; }

        protected abstract void Init(string host);

        protected abstract T Execute<T>(object[] @params) where T : DataResultBase;
    }
}
