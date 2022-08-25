namespace SuperFramework.Channels.Channel
{
    public class HttpChannel : ChannelBase<HttpMessage>
    {
        public override string ChannelName => "http;https";
    }
}