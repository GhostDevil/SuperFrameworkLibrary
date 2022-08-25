using System;
using System.IO;

namespace SuperFramework.SuperRegistry
{
    /// <summary>
    /// 注册表监视器帮助
    /// </summary>
    internal class RegistryMonitorHelper : IDisposable
    {
        private RegistryMonitor registryMonitor;//注册表监控
        internal delegate void dlgOnError(object sender, ErrorEventArgs e);
        internal delegate void dlgValueChanged(object sender, EventArgs e);
        /// <summary>
        /// 错误处理
        /// </summary>
        internal event dlgOnError OnError;
        /// <summary>
        /// 监控值改变
        /// </summary>
        internal event dlgValueChanged ValueChanged;
        /// <summary>
        /// 监控是否已经开始
        /// </summary>
        private bool started;
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (registryMonitor != null)
            {
                if (started) Stop();
                registryMonitor.Dispose();
                registryMonitor = null;
            }
        }
        /// <summary>
        /// 启动监视
        /// </summary>
        /// <param name="regField"></param>
        internal void Start(string regField)
        {
            registryMonitor = new RegistryMonitor(string.Format("{0}\\SOFTWARE\\{1}", Microsoft.Win32.Registry.LocalMachine.Name, regField));
            registryMonitor.RegChanged += OnRegChanged;
            registryMonitor.Error += RegError;
            registryMonitor.Start();
            started = true;
        }
        /// <summary>
        /// 停止监视
        /// </summary>
        internal void Stop()
        {
            if (registryMonitor != null)
            {
                registryMonitor.Stop();
                registryMonitor.RegChanged -= OnRegChanged;
                registryMonitor.Error -= RegError;
                registryMonitor = null;
            }
            started = false;
        }
        /// <summary>
        /// 错误处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RegError(object sender, ErrorEventArgs e)
        {
            //if (InvokeRequired)
            //{
            //    BeginInvoke(new ErrorEventHandler(RegError), new object[] { sender, e });
            //    return;
            //}
            if (OnError != null)
                OnError(sender, e);
            else
                Stop();
        }
        /// <summary>
        /// 注册表改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRegChanged(object sender, EventArgs e)
        {
            //if (InvokeRequired)
            //{
            //    BeginInvoke(new EventHandler(OnRegChanged), new object[] { sender, e });
            //    return;
            //}
            if (ValueChanged != null)
                ValueChanged(sender, e);
        }
    }
}
