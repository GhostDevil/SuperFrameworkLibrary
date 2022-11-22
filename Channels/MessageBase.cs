using SuperFramework.Channels.Channel;
using System;
using System.Collections.Generic;

namespace SuperFramework.Channels
{
    public abstract class RequestBase
    {
        public Uri Url { get; protected set; }

        protected abstract void Init(string host);

        protected abstract T RequestExecute<T>(object[] @params, Dictionary<string, string> headers) where T : DataResultBase;
    }
}
