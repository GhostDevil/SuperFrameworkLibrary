using System;
using System.IO;
using System.Runtime.Versioning;

namespace SuperFramework.SuperRegistry
{
    /// <summary>
    /// 注册表监视器帮助
    /// </summary>
    [SupportedOSPlatform("windows")]
    public class RegistryMonitorHelper
    {
        private RegistryMonitor registryMonitor;//注册表监控
        private bool disposedValue;

        public delegate void OnError(object sender, ErrorEventArgs e);
        public delegate void OnValueChanged(object sender, EventArgs e);
        /// <summary>
        /// 错误处理
        /// </summary>
        public event OnError Error;
        /// <summary>
        /// 监控值改变
        /// </summary>
        public event OnValueChanged ValueChanged;
        /// <summary>
        /// 监控是否已经开始
        /// </summary>
        public bool Started { get; private set; }

        /// <summary>
        /// 启动监视
        /// </summary>
        /// <param name="regField"></param>
        public void Start(string regField)
        {
            registryMonitor = new RegistryMonitor(string.Format("{0}\\SOFTWARE\\{1}", Microsoft.Win32.Registry.LocalMachine.Name, regField));
            registryMonitor.RegChanged += OnRegChanged;
            registryMonitor.Error += RegError;
            registryMonitor.Start();
            Started = true;
        }
        /// <summary>
        /// 停止监视
        /// </summary>
        public void Stop()
        {
            if (registryMonitor != null)
            {
                registryMonitor.Stop();
                registryMonitor.RegChanged -= OnRegChanged;
                registryMonitor.Error -= RegError;
                registryMonitor.Dispose();
                registryMonitor = null;
            }
            Started = false;
        }
        /// <summary>
        /// 错误处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RegError(object sender, ErrorEventArgs e)
        {

            if (Error != null)
                Error.Invoke(sender, e);
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
            ValueChanged?.Invoke(sender, e);
        }
    }
}
