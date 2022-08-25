using System;

namespace SuperFramework
{
    [Serializable]
    public abstract class SafeObject : MarshalByRefObject, IDisposable
    {
        public sealed override object InitializeLifetimeService() => null;

        /// <summary>
        /// GC资源回收
        /// </summary>
        ~SafeObject() => Dispose(false);

        /// <summary>
        /// 资源回收
        /// </summary>
        public void Dispose() => Dispose(true);

        /// <summary>
        /// 资源回收
        /// </summary>
        /// <param name="disposing">true 表示释放托管资源和非托管资源；false 表示仅释放非托管资源。</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                GC.SuppressFinalize(this);
        }
    }
}

