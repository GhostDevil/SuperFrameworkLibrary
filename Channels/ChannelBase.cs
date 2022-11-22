using System;
using System.Linq;

namespace SuperFramework.Channels
{
    public abstract class ChannelBase<T> : SafeObject where T : RequestBase
    {
        public bool IsSecured { get; set; }
        public abstract string ChannelName { get; }
        public virtual int ChannelPriority => 100;
        public string[] Schemes => ChannelName.Split(';').Select(s => $"{s}://").ToArray();


        public Uri Url { get; private set; }
        public object State { get; private set; }

    }
}