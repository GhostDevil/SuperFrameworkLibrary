#if NET462 || NET47
using AxMSTSCLib;
using MSTSCLib;
using System;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace SuperFramework
{
    public class RDPView : AxMsRdpClient9NotSafeForScripting
    {
        /// <summary>
        /// 打开子会话功能
        /// </summary>
        /// <param name="enable"></param>
        /// <returns></returns>
        [DllImport("Wtsapi32.dll")]
        public static extern bool WTSEnableChildSessions(bool enable);
        /// <summary>
        /// 判断用户是否打开了子会话功能
        /// </summary>
        /// <param name="isEnabled"></param>
        /// <returns></returns>
        [DllImport("Wtsapi32.dll")]
        public static extern bool WTSIsChildSessionsEnabled(out bool isEnabled);
        /// <summary>
        /// 获取子会话的SessionID
        /// </summary>
        /// <param name="SessionId"></param>
        /// <returns></returns>
        [DllImport("Wtsapi32.dll")]
        public static extern bool WTSGetChildSessionId(out uint SessionId);
        /// <summary>
        /// 注销子会话
        /// </summary>
        /// <param name="SessionId"></param>
        /// <returns></returns>
        [DllImport("Wtsapi32.dll", SetLastError = true)]
        public static extern bool WTSLogoffSession(IntPtr hServer, uint SessionId, bool bWait);
        /// <summary>
        /// rdp客户端对象
        /// </summary>
        //public  AxMsTscAxNotSafeForScripting rdpClient;
        AxMsRdpClient9NotSafeForScripting rdpClient;
        /// <summary>
        /// 容器
        /// </summary>
        WindowsFormsHost wfhost;
        /// <summary>
        /// 连接信息
        /// </summary>
        public RdpInfo rdpInfo;
        /// <summary>
        /// 是否用户退出
        /// </summary>
        public bool userExit = false;
        /// <summary>
        /// 是否获得焦点
        /// </summary>
        public bool isGetFocus = true;
        /// <summary>
        /// 指示控件的连接状态。这可以是以下值之一
        /// <para>0 控件未连接;1 控件已连接;2 控件正在建立连接</para>
        /// </summary>
        public int IsConnected { get { return (int)rdpClient?.Connected; } }
        /// <summary>
        /// 连接状态委托
        /// </summary>
        /// <param name="rdpState">状态</param>
        /// <param name="code">用户’设备地址、或者相关代码</param>
        /// <param name="msg">事件消息</param>
        /// <param name="ex">异常，正常时为null</param>
        public delegate void RdpClientStateChangeHandle(RdpState rdpState, string code, string msg, Exception ex = null);
        /// <summary>
        /// 连接状态发生改变
        /// </summary>
        public event RdpClientStateChangeHandle RdpClientStateChange;
        /// <summary>
        /// 连接状态
        /// </summary>
        public enum RdpState
        {
            ControlAdded,
            /// <summary>
            /// 当客户端控件正在与RD会话主机服务器建立连接的过程中调用。
            /// </summary>
            OnConnected,
            /// <summary>
            /// 当客户端控件与RD会话主机服务器断开连接时调用。
            /// </summary>
            OnDisconnected,
            /// <summary>
            /// 当客户端控件开始连接到服务器以响应对IMsTscAx :: Connect的调用时调用。
            /// </summary>
            OnConnecting,
            OnFatalError,
            /// <summary>
            /// 客户端控件成功登录到RD会话主机服务器后，在Windows登录对话框显示之后调用。
            /// </summary>
            OnLoginComplete,
            /// <summary>
            /// 在发生登录错误或其他登录事件时调用。
            /// </summary>
            OnLogonError,
            /// <summary>
            /// 当客户端控件遇到非致命错误条件时调用。
            /// </summary>
            OnWarning,
            /// <summary>
            /// 显示RemoteApp窗口时调用。
            /// </summary>
            OnRemoteWindowDisplayed,
            /// <summary>
            /// 网络状态更改时调用。, 当网络状态发生变化时调用。
            /// </summary>
            OnNetworkStatusChanged,
            /// <summary>
            /// 获取焦点
            /// </summary>
            GotFocus,
            /// <summary>
            /// 失去焦点
            /// </summary>
            LostFocus,
            /// <summary>
            /// 当按下释放焦点组合键时调用。例如，当用户按下CTRL + ALT +向左键或CTRL + ALT +向右键组合时，将调用此事件。
            /// </summary>
            OnFocusReleased,
            /// <summary>
            /// 异常
            /// </summary>
            Exception,
            /// <summary>
            /// 当客户端控件已自动重新连接到远程会话时调用。
            /// </summary>
            OnAutoReconnected,
            /// <summary>
            /// 当客户端正在自动将会话与RD会话主机服务器重新连接的过程中调用。
            /// </summary>
            OnAutoReconnecting2,
            /// <summary>
            /// 当客户端正在自动将会话与RD会话主机服务器重新连接的过程中调用。
            /// </summary>
            OnAutoReconnecting,
            /// <summary>
            /// 创建连接网络不通时调用
            /// </summary>
            OnCheckNetOff,
            /// <summary>
            /// 创建连接网络通时调用
            /// </summary>
            OnCheckNetOn,
            /// <summary>
            /// 创建连接时网络恢复时调用
            /// </summary>
            OnCheckNetRecall,
        }
        /// <summary>
        /// 远程参数
        /// </summary>
        public class RdpInfo
        {
            public Grid grid;
            public string rdpIP;
            public string userName;
            public string password;
            public int rdpPort = 3389;
            public int width = -1;
            public int height = -1;
            /// <summary>
            /// 是否全屏
            /// </summary>
            public bool fullScreen = false;
            /// <summary>
            /// 是否采用压缩
            /// </summary>
            public bool compress = false;
            /// <summary>
            /// 是否采用缓存
            /// </summary>
            public bool bitmapPersistence = true;
            /// <summary>
            /// 指定鼠标是否应使用相对模式。如果鼠标处于相对模式，则包含VARIANT_TRUE；如果鼠标处于绝对模式，则包含VARIANT_FALSE。
            /// <para>鼠标模式指示ActiveX控件如何计算发送到远程桌面会话主机（RD会话主机）服务器的鼠标坐标。当鼠标处于相对模式时，ActiveX控件将计算相对于鼠标最后位置的鼠标坐标。当鼠标处于绝对模式时，ActiveX控件将计算相对于RD会话主机服务器桌面的鼠标坐标。</para>
            /// </summary>
            public bool relativeMouseMode = true;
            /// <summary>
            /// 指定是否缩放显示以适合控件的客户区域。
            /// <para>请注意，启用SmartSizing属性时，滚动条不会出现。</para>
            /// </summary>
            public bool smartSizing = true;
            /// <summary>
            /// 指定在网络断开的情况下是否启用客户端控件以自动重新连接到会话。
            /// </summary>
            public bool enableAutoReconnect = true;
            /// <summary>
            /// 指定在自动重新连接期间尝试重新连接的次数。此属性的有效值为0到200（含）。
            /// </summary>
            public int maxReconnectAttempts = 20;
        }
        //RDPView() : base(){ }

        //protected override void WndProc(ref Message m)
        //{
        //    // Fix for the missing focus issue on the rdp client component
        //    if (m.Msg == 0x0021) // WM_MOUSEACTIVATE
        //    {
        //        if (!ContainsFocus)
        //        {
        //            Focus();
        //        }
        //    }

        //    base.WndProc(ref m);
        //}
        public RDPView(RdpInfo info) : base()
        {
            if (info != null)
                rdpInfo = info;

        }
        #region Ping设备，Ping通了则连接
        public bool IsPingState(string ipAddress)
        {
            bool flag;
            try
            {
                // bool b=SuperNetwork.PingHelper.Ping(ipAddress);
                Ping ping = new Ping();
                PingReply pingReply = ping.Send(ipAddress, 5000);
                if (pingReply.Status == IPStatus.Success)
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                }
            }
            catch
            {
                flag = false;
            }
            return flag;
        }
        #endregion

        #region 连接RDP
        /// <summary>
        /// 连接RDP
        /// </summary>
        public bool ConnectRDPView()
        {
            if (rdpInfo == null)
            {
                RdpClientStateChange?.Invoke(RdpState.Exception, null, $"连接参数RdpInfo为 null，请检查。", new Exception("rdp连接参数RdpInfo为null"));
                return false;
            }
            if (IsPingState(rdpInfo.rdpIP))
            {
                RdpClientStateChange?.Invoke(RdpState.OnCheckNetOn, rdpInfo?.rdpIP, $"IP地址为: {rdpInfo.rdpIP} 的设备网络正常，开始创建连接···");
                if (rdpClient != null)
                    CloseConnection();

            }
            else
            {
                //Logger.log.Error($"IP地址为:{rdpIP}的设备 Ping不通，请检查。");
                //System.Windows.MessageBox.Show($"IP地址为: {rdpInfo.rdpIP} 的设备 Ping不通，请检查。");
                new Thread(async () =>
                {
                    while (!IsPingState(rdpInfo.rdpIP))
                    {
                        await Task.Delay(1000).ConfigureAwait(false);
                    }
                    RdpClientStateChange?.Invoke(RdpState.OnCheckNetRecall, rdpInfo?.rdpIP, $"IP地址为: {rdpInfo.rdpIP} 的设备网络恢复正常！！！");
                    //if (InvokeRequired)
                    //{
                    //    Invoke(new MethodInvoker(delegate { ConnectRDPView(); }));
                    //}
                    //else
                    //{ ConnectRDPView(); }

                })
                { IsBackground = true }.Start();
                RdpClientStateChange?.Invoke(RdpState.OnCheckNetOff, rdpInfo?.rdpIP, $"IP地址为: {rdpInfo.rdpIP} 的设备 Ping不通，请检查。");
            }
            InitRdpClient();
            SetOtherParam();
            try
            {
                rdpClient.Connect();
                return true;
            }
            catch (Exception ex)
            {
                //Logger.log.Error(ex.Message);
                RdpClientStateChange?.Invoke(RdpState.Exception, rdpInfo?.rdpIP, $"IP地址为: {rdpInfo.rdpIP} 用户为：{rdpInfo.userName} 连接异常。", ex);
            }
            return false;
        }
        /// <summary>
        /// 初始化rdp
        /// </summary>
        private void InitRdpClient()
        {
            rdpClient = null;
            rdpClient = new AxMsRdpClient9NotSafeForScripting();
            wfhost = new WindowsFormsHost();
            wfhost.MouseEnter += Wfhost_MouseEnter;
            RdpEvent();
            wfhost.Width = rdpInfo.width == -1 ? (int)SystemParameters.PrimaryScreenWidth : rdpInfo.width;
            wfhost.Height = rdpInfo.height == -1 ? (int)SystemParameters.PrimaryScreenHeight : rdpInfo.height;

            ((System.ComponentModel.ISupportInitialize)rdpClient).BeginInit();
            rdpClient.Top = 0;
            rdpClient.Left = 0;
            rdpClient.UseWaitCursor = true;
            rdpClient.Width = rdpInfo.width == -1 ? (int)SystemParameters.PrimaryScreenWidth : rdpInfo.width;
            rdpClient.Height = rdpInfo.height == -1 ? (int)SystemParameters.PrimaryScreenHeight : rdpInfo.height;
            wfhost.Child = rdpClient;
            ((System.ComponentModel.ISupportInitialize)rdpClient).EndInit();

            rdpInfo.grid.Children.Add(wfhost);
            wfhost.Visibility = Visibility.Hidden;
            rdpClient.Visible = false;


            rdpClient.Server = rdpInfo?.rdpIP;
            rdpClient.UserName = rdpInfo?.userName;

            // 颜色深度。值包括每个像素8、15、16、24和32位。
            rdpClient.ColorDepth = 32;
            // 开启全屏 true|flase
            rdpClient.FullScreen = rdpInfo.fullScreen;
            // 设置远程桌面宽度为显示器宽度
            rdpClient.DesktopWidth = rdpInfo.width == -1 ? (int)SystemParameters.PrimaryScreenWidth : rdpInfo.width;
            // 设置远程桌面宽度为显示器高度
            //axMsRdpc.DesktopHeight = ScreenArea.Height;
            rdpClient.DesktopHeight = rdpInfo.height == -1 ? (int)SystemParameters.PrimaryScreenHeight : rdpInfo.height;

            // RDP名字
            //rdpClient.Name = "live10";
            IMsTscNonScriptable secured = (IMsTscNonScriptable)rdpClient.GetOcx();
            secured.ClearTextPassword = rdpInfo.password;
        }
        /// <summary>
        /// 设置其他参数
        /// </summary>
        private void SetOtherParam()
        {
            rdpClient.AdvancedSettings9.RDPPort = rdpInfo.rdpPort;
            //用于每个像素8位的位图的位图缓存文件的大小（以千字节为单位）。此属性的有效数字值为1到32（含）
            rdpClient.AdvancedSettings9.BitmapCacheSize = 32;
            //指定连接时客户端控件是否应具有焦点
            rdpClient.AdvancedSettings9.GrabFocusOnConnect = true;
            // 启用CredSSP身份验证（有些服务器连接没有反应，需要开启这个）
            rdpClient.AdvancedSettings9.EnableCredSspSupport = true;
            //指定是否缩放显示以适合控件的客户区域。请注意，启用SmartSizing属性时，滚动条不会出现。
            rdpClient.AdvancedSettings9.SmartSizing = true;
            //AuthenticationLevel身份验证级别
            //指定鼠标是否应使用相对模式。如果鼠标处于相对模式，则包含VARIANT_TRUE；如果鼠标处于绝对模式，则包含VARIANT_FALSE。
            rdpClient.AdvancedSettings9.RelativeMouseMode = rdpInfo.relativeMouseMode;

            //将此参数设置为0以禁用缓存，或者将其设置为非零值以启用缓存。指定是否应使用持久性位图缓存。持久缓存可以提高性能，但需要更多的磁盘空间。
            rdpClient.AdvancedSettings9.BitmapPeristence = rdpInfo.bitmapPersistence ? 1 : 0;
            //指定是否启用压缩.将此参数设置为0以禁用压缩，或者将其设置为非零值以启用压缩。
            rdpClient.AdvancedSettings9.Compress = rdpInfo.compress ? 1 : 0;
            //指定在网络断开的情况下是否启用客户端控件以自动重新连接到会话。
            rdpClient.AdvancedSettings9.EnableAutoReconnect = rdpInfo.enableAutoReconnect;
            //指定在自动重新连接期间尝试重新连接的次数。此属性的有效值为0到200（含）。
            rdpClient.AdvancedSettings9.MaxReconnectAttempts = rdpInfo.maxReconnectAttempts;
            //指定是否自动检测带宽变化。
            rdpClient.AdvancedSettings9.BandwidthDetection = true;
        }
        /// <summary>
        /// 设置事件
        /// </summary>
        private void RdpEvent()
        {
            rdpClient.LostFocus += Amtansfs_LostFocus;
            rdpClient.OnConnected += Amtansfs_OnConnected;
            rdpClient.OnDisconnected += Amtansfs_OnDisconnected;
            rdpClient.OnFatalError += Amtansfs_OnFatalError;
            rdpClient.OnLoginComplete += Amtansfs_OnLoginComplete;
            rdpClient.OnLogonError += Amtansfs_OnLogonError;
            rdpClient.OnWarning += Amtansfs_OnWarning;
            rdpClient.OnRemoteWindowDisplayed += Amtansfs_OnRemoteWindowDisplayed;
            rdpClient.OnNetworkStatusChanged += Amtansfs_OnNetworkStatusChanged;
            rdpClient.GotFocus += RdpClient_GotFocus;
            rdpClient.OnFocusReleased += RdpClient_OnFocusReleased;
            rdpClient.OnConnecting += RdpClient_OnConnecting;
            rdpClient.ControlAdded += RdpClient_ControlAdded;
            rdpClient.Leave += RdpClient_Leave;

            rdpClient.OnAutoReconnected += RdpClient_OnAutoReconnected;
            rdpClient.OnAutoReconnecting2 += RdpClient_OnAutoReconnecting2;
        }

        #endregion

        #region  事件
        private void RdpClient_OnAutoReconnecting2(object sender, IMsTscAxEvents_OnAutoReconnecting2Event e)
        {
            RdpClientStateChange?.Invoke(RdpState.OnAutoReconnecting2, rdpInfo?.userName, $"自动开始重新连接···");

        }

        private void RdpClient_OnAutoReconnected(object sender, EventArgs e)
        {
            RdpClientStateChange?.Invoke(RdpState.OnAutoReconnected, rdpInfo?.userName, $"自动重新连接完成！");
        }

        private void RdpClient_ControlAdded(object sender, ControlEventArgs e)
        {
            RdpClientStateChange?.Invoke(RdpState.ControlAdded, rdpInfo?.userName, $"插件完成初始化！");

        }

        private void RdpClient_OnConnecting(object sender, EventArgs e)
        {
            RdpClientStateChange?.Invoke(RdpState.OnConnecting, rdpInfo?.userName, $"用户 {rdpInfo?.userName} 连接中···");

        }

        private void RdpClient_Leave(object sender, EventArgs e)
        {
            //if (isGetFocus)
            //{
            //    wfhost.Focus();
            //    rdpClient.Focus();
            //}
        }

        private void Wfhost_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //AxMsRdpClient9NotSafeForScripting rdp = (sender as System.Windows.Forms.Integration.WindowsFormsHost).Child as AxMsRdpClient9NotSafeForScripting;
            //rdp.Focus();
        }

        //void host_MouseMove(object sender, MouseEventArgs e)
        //{
        //    RDP rdp = (sender as System.Windows.Forms.Integration.WindowsFormsHost).Child as RDP;
        //    rdp.Focus();
        //}
        private void RdpClient_OnFocusReleased(object sender, IMsTscAxEvents_OnFocusReleasedEvent e)
        {
            RdpClientStateChange?.Invoke(RdpState.OnFocusReleased, rdpInfo?.userName, $"用户 {rdpInfo?.userName} 控制焦点已释放");

        }

        private void RdpClient_GotFocus(object sender, EventArgs e)
        {
            RdpClientStateChange?.Invoke(RdpState.GotFocus, rdpInfo?.userName, $"用户 {rdpInfo?.userName} 取得控制焦点");

        }

        private void Amtansfs_OnNetworkStatusChanged(object sender, IMsTscAxEvents_OnNetworkStatusChangedEvent e)
        {
            RdpClientStateChange?.Invoke(RdpState.OnNetworkStatusChanged, e.rtt.ToString(), "");
        }

        private void Amtansfs_OnRemoteWindowDisplayed(object sender, IMsTscAxEvents_OnRemoteWindowDisplayedEvent e)
        {
            RdpClientStateChange?.Invoke(RdpState.OnRemoteWindowDisplayed, e.vbDisplayed.ToString(), $"用户 {rdpInfo?.userName} RDP显示状态({e.vbDisplayed.ToString()})");
        }

        private void Amtansfs_OnWarning(object sender, IMsTscAxEvents_OnWarningEvent e)
        {
            RdpClientStateChange?.Invoke(RdpState.OnWarning, e.warningCode.ToString(), $"用户 {rdpInfo?.userName} 出现警告({e.warningCode})");

        }

        private void Amtansfs_OnLogonError(object sender, IMsTscAxEvents_OnLogonErrorEvent e)
        {
            RdpClientStateChange?.Invoke(RdpState.OnLogonError, e.lError.ToString(), $"用户 {rdpInfo?.userName} 登录异常({e.lError})");

        }

        private void Amtansfs_OnLoginComplete(object sender, EventArgs e)
        {
            RdpClientStateChange?.Invoke(RdpState.OnLoginComplete, rdpInfo?.userName, $"用户 {rdpInfo?.userName} 登录完成");

        }

        private void Amtansfs_OnFatalError(object sender, IMsTscAxEvents_OnFatalErrorEvent e)
        {
            RdpClientStateChange?.Invoke(RdpState.OnFatalError, e.errorCode.ToString(), $"用户 {rdpInfo?.userName} 连接出现异常({e.errorCode})");

        }

        private async void Amtansfs_OnDisconnected(object sender, IMsTscAxEvents_OnDisconnectedEvent e)
        {

            if (!userExit && rdpClient != null)
            {
                await Task.Delay(2000).ConfigureAwait(false);
                string str = rdpClient.GetErrorDescription((uint)e.discReason, (uint)rdpClient.ExtendedDisconnectReason);
                RdpClientStateChange?.Invoke(RdpState.OnDisconnected, rdpInfo?.userName, $"用户 {rdpInfo?.userName} 连接异常关闭 {e.discReason}:{str},正在重新连接···");

                if (rdpClient != null)
                    rdpClient.Connect();
            }
            else
            {
                RdpClientStateChange?.Invoke(RdpState.OnDisconnected, rdpInfo?.userName, $"用户 {rdpInfo?.userName} 连接关闭成功");

            }
        }

        private void Amtansfs_OnConnected(object sender, EventArgs e)
        {
            RdpClientStateChange?.Invoke(RdpState.OnConnected, rdpInfo?.userName, $"用户 {rdpInfo?.userName} 连接成功");
            rdpClient.Visible = true;
            wfhost.Visibility = Visibility.Visible;
        }

        private void Amtansfs_LostFocus(object sender, EventArgs e)
        {
            RdpClientStateChange?.Invoke(RdpState.LostFocus, rdpInfo?.userName, $"用户 {rdpInfo?.userName} 失去控制焦点");
            if (isGetFocus)
            {
                wfhost.Focus();
                rdpClient.Focus();
            }
        }
        #endregion

        #region 关闭RDP
        public void CloseConnection()
        {
            try
            {
                //等于1：说明连接成功
                //等于0：说明未连接或断开连接
                if (rdpClient != null)
                {
                    if (rdpClient.Connected == 1)
                    {
                        userExit = true;
                        rdpClient.Disconnect();
                        rdpClient = null;
                    }
                    else
                    {
                        userExit = true;
                        rdpClient = null;
                    }
                }
            }
            catch (Exception ex)
            {
                RdpClientStateChange?.Invoke(RdpState.Exception, rdpInfo?.userName, $"用户 {rdpInfo?.userName} 连接异常。", ex);

            }
        }

        #endregion

        /// <summary>
        /// 重新连接
        /// </summary>
        /// <returns></returns>
        public bool ReConnection()
        {
            try
            {
                if (rdpClient != null)
                {
                    ControlReconnectStatus controlReconnectStatus = rdpClient.Reconnect((uint)rdpClient.DesktopWidth, (uint)rdpClient.DesktopHeight);
                    if (controlReconnectStatus == ControlReconnectStatus.controlReconnectStarted)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                RdpClientStateChange?.Invoke(RdpState.Exception, rdpInfo?.userName, $"用户 {rdpInfo?.userName} 重新连接异常。", ex);
            }
            return false;
        }
        /// <summary>
        /// 指定要在远程会话中启动的RemoteApp程序。必须在连接的会话上调用此功能（在客户端收到会话连接通知之后）。一个会话中可以启动任何数量的RemoteApp程序。如果在超时限制（Windows Server 2008为2分钟）内没有在会话中启动RemoteApp程序，则RemoteApp会话将超时。
        /// </summary>
        /// <param name="bstrExecutablePath">服务器上RemoteApp程序可执行文件的路径。</param>
        /// <param name="bstrFilePath">通过文件关联在服务器上打开的文件的路径，例如“ C：\\ Documents \\ MyReport.docx”。如果指定bstrFilePath，则不应指定bstrExecutablePath参数，反之亦然。您仅应指定参数之一。</param>
        /// <param name="bstrWorkingDirectory">服务器上RemoteApp程序的工作目录。</param>
        /// <param name="bstrArgument">在bstrExecutablePath中指定的RemoteApp程序的命令行参数。如果未指定bstrExecutablePath，请将其设置为NULL。</param>
        /// <param name="vbExpandEnvVarInWorkingDirectoryOnServer">指示服务器是否应在工作目录路径中扩展环境变量。如果工作目录路径包含环境变量，则将此参数设置为VARIANT_TRUE；如果工作目录路径不包含环境变量，则将此参数设置为VARIANT_FALSE。</param>
        /// <param name="vbExpandEnvVarInArgumentsOnServer">指示服务器是否应该在命令行参数中扩展环境变量。如果参数包含环境变量，则将此参数设置为VARIANT_TRUE；如果参数不包含环境变量，则将此参数设置为VARIANT_FALSE。</param>
        public void OpenProgram(string bstrExecutablePath, string bstrWorkingDirectory, string bstrFilePath = "", string bstrArgument = "", bool vbExpandEnvVarInWorkingDirectoryOnServer = false, bool vbExpandEnvVarInArgumentsOnServer = false)
        {
            if (rdpClient.Connected == 1)
            {
                rdpClient.RemoteProgram2.ServerStartProgram(bstrExecutablePath, bstrFilePath, bstrWorkingDirectory, vbExpandEnvVarInWorkingDirectoryOnServer, bstrArgument, vbExpandEnvVarInArgumentsOnServer);
            }
        }
        /// <summary>
        /// 设置焦点
        /// </summary>
        public new void Focus()
        {
            rdpClient?.Focus();
        }
    }
}
#endif