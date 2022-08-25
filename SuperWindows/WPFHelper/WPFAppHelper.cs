//using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace SuperWindow.SuperWPF.WPFHelper
{
    /// <summary>
    /// 作者：不良帥
    /// 日期：2016-07-05
    /// 说明：wpf程序辅助
    /// </summary>
    public static class WinAppHelper
    {
        #region Application.Doevent函数，使得线程不阻塞UI
        ///// <summary>
        ///// Application.Doevent函数，使得线程不阻塞UI
        ///// </summary>
        //[SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        //public static void DoEvent()
        //{
        //    try
        //    {

        //        //DispatcherFrame frame = new DispatcherFrame();
        //        //Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrame), frame);
        //        //Dispatcher.PushFrame(frame);
        //        var frame = new DispatcherFrame();
        //        Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
        //        new DispatcherOperationCallback(f =>
        //        {
        //            ((DispatcherFrame)f).Continue = false;
        //            return null;
        //        }), frame);
        //        Dispatcher.PushFrame(frame);
        //    }
        //    catch (Exception) { }
        //}
        /// <summary>
        /// Application.Doevent函数，使得线程不阻塞UI
        /// </summary>
        private static void DoEvents()
        {
            try
            {
                Action action = delegate { };
                Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Input, action);
            }
            catch (Exception) { }
        }
        static object ExitFrame(object f)
        {
            ((DispatcherFrame)f).Continue = false;
            return null;
        }

        private static readonly DispatcherOperationCallback _exit_frame_callback = (state) => {
            var frame = state as DispatcherFrame;
            if (null == frame)
                return null;
            frame.Continue = false;
            return null;
        };
        /// <summary>
        /// Application.Doevent函数，使得线程不阻塞UI
        /// </summary>
        public static void DoEvent()
        {
            var frame = new DispatcherFrame();
            var exOperation = Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, _exit_frame_callback, frame);
            Dispatcher.PushFrame(frame);
            if (exOperation.Status != DispatcherOperationStatus.Completed)
            {
                exOperation.Abort();
            }
        }
        #endregion

        #region 获取路径
        /// <summary>
        /// 获取模块的完整路径
        /// </summary>
        /// <returns>程序根路径</returns>
        public static string GetAppStartPath()
        {
            return System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
        }
        /// <summary>
        /// 获取程序的根目录
        /// </summary>
        /// <returns>程序根路径</returns>
        public static string GetAppPathRoot()
        {
            return System.IO.Path.GetPathRoot(Process.GetCurrentProcess().MainModule.FileName);
        }
        /// <summary>
        /// 获取exe的完整路径
        /// </summary>
        /// <returns>程序路径</returns>
        public static string GetAppFullPath()
        {
            return System.Windows.Forms.Application.ExecutablePath;//AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                                                                   //  return Process.GetCurrentProcess().ProcessName;
        }
        /// <summary>
        /// 获取exe的名称
        /// </summary>
        /// <returns>程序路径</returns>
        public static string GetAppExeName()
        {
            return System.IO.Path.GetFileName(Assembly.GetEntryAssembly().GetName().Name+".exe");
        }
        #endregion

        #region  获取系统 ico 图标对象
        /// <summary>
        /// 获取系统 ico 图标对象
        /// </summary>
        /// <param name="icon">windows ico</param>
        /// <returns>返回ico对象</returns>
        public static System.Drawing.Icon GetImageSource(System.Windows.Media.ImageSource icon)
        {
            if (icon == null)
            {
                return null;
            }
            Uri iconUri = new Uri(icon.ToString());
            return new System.Drawing.Icon(Application.GetResourceStream(iconUri).Stream);
        }
        #endregion

        #region 以管理员启动程序，可限启动一个实例。（在 app 页面构造函数的 Startup 中调用）
        static Mutex instance;
        /// <summary>
        /// 以管理员启动程序，可限启动一个实例。（在 app 页面构造函数的 Startup 中调用）
        /// </summary>
        /// <param name="window">程序主窗体</param>
        /// <param name="Args">数组参数</param>
        /// <param name="isOne">是否启动一个实例</param>
        /// <param name="isShowMsg">是否显示提示框</param>
        public static void RunByAdministrator(string[] Args, bool isOne = false, bool isShowMsg = false)
        {
            /**
             * 当前用户是管理员的时候，直接启动应用程序
             * 如果不是管理员，则使用启动对象启动程序，以确保使用管理员身份运行
             */
            //获得当前登录的Windows用户标示
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            bool createdNew; //返回是否赋予了使用线程的互斥体初始所属权 
            /*
            在运行终端服务的服务器上，已命名的系统 mutex 可以具有两级可见性。如果名称以前缀“Global\”开头，则 mutex 在所有终端服务器会话中均为可见。如果名称以前缀“Local\”开头，则 mutex 仅在创建它的终端服务器会话中可见。在这种情况下，服务器上各个其他终端服务器会话中都可以拥有一个名称相同的独立 mutex。如果创建已命名 mutex 时不指定前缀，则它将采用前缀“Local\”。在终端服务器会话中，只是名称前缀不同的两个 mutex 是独立的 mutex，这两个 mutex 对于终端服务器会话中的所有进程均为可见。即：前缀名称“Global\”和“Local\”说明 mutex 名称相对于终端服务器会话（而并非相对于进程）的范围。
            */
            instance = new Mutex(true, string.Format("Global\\{0}{1}", System.Windows.Forms.Application.ProductName, GetAppExeName()), out createdNew);            //同步基元变量 
            if (isOne)
            {
                if (!createdNew)
                {
                    if (isShowMsg)
                        MessageBox.Show("已经启动了一个程序实例，请先退出后再次启动 !", "系统提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    Environment.Exit(0);
                    Process.GetCurrentProcess().Dispose();
                }
                //else
                //    instance.ReleaseMutex(); //赋予了线程初始所属权，也就是首次使用互斥体
            }
            if (!principal.IsInRole(WindowsBuiltInRole.Administrator)) //判断当前登录用户是否为管理员
            {
                try
                {
                    //创建启动对象
                    ProcessStartInfo startInfo = new ProcessStartInfo() { /*设置运行文件*/FileName = GetAppFullPath(), /*设置启动参数*/Arguments = string.Join(" ", Args), /*设置启动动作,确保以管理员身份运行*/Verb = "runas" };
                    //如果不是管理员，则启动UAC
                    Process.Start(startInfo);
                }
                catch (Exception) { return; }
                Environment.Exit(0);
            }

        }
        #endregion

        //#region 避免管理员权限问题，使得程序开机自动运行
        ///// <summary>
        /////  避免管理员权限问题，使得程序开机自动运行
        ///// </summary>
        //public static void AddShellRegistry()
        //{
        //    WshShell shell = new WshShell();
        //    IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(Environment.GetFolderPath(
        //                                        Environment.SpecialFolder.Startup) + "" + "\\NotifyIconWpf.lnk");
        //    shortcut.TargetPath = System.Windows.Forms.Application.ExecutablePath;
        //    shortcut.WorkingDirectory = Environment.CurrentDirectory;
        //    shortcut.WindowStyle = 1;
        //    shortcut.Description = "NotifyIconWpf_lnk";
        //    shortcut.Save();

        //    //}

        //}
        //#endregion

        #region 使用托盘程序
        /// <summary>
        /// NotifyIcon 托盘程序对象
        /// </summary>
        public static System.Windows.Forms.NotifyIcon notify = null;
        /// <summary>
        /// 程序窗口
        /// </summary>
        static Window win = null;
        static bool showInTaskbar = true;
        /// <summary>
        ///  NotifyIcon 鼠标单击委托
        /// </summary>
        public delegate void NotifyIconMouseClick();
        /// <summary>
        /// NotifyIcon 鼠标左键单击事件
        /// </summary>
        public static event NotifyIconMouseClick MouseLeftClick;
        /// <summary>
        /// NotifyIcon 鼠标左键双击事件
        /// </summary>
        public static event NotifyIconMouseClick MouseLeftDoubleClick;
        ///// <summary>
        /////  NotifyIcon 退出程序委托
        ///// </summary>
        //public delegate void NotifyIconAppCloseing();
        ///// <summary>
        ///// NotifyIcon 退出程序事件
        ///// </summary>
        //public static event NotifyIconAppCloseing AppCloseing;
        /// <summary>
        /// 使用托盘程序
        /// </summary>
        /// <param name="window">Window窗口</param>
        /// <param name="tipTitle">气球标题文本</param>
        /// <param name="tipText">气球提示文本</param>
        /// <param name="text">工具提示文本</param>
        /// <param name="icoPath">托盘图标</param>
        /// <param name="timeOut">提示超时时间 m</param>
        /// <param name="tipIco">气球提示ico</param>
        /// <param name="useExitMenu">是否使用退出菜单</param>
        public static void UseNotifyIcon(Window window, string tipTitle, string tipText, string text, string icoPath, int timeOut = 3000, System.Windows.Forms.ToolTipIcon tipIco = System.Windows.Forms.ToolTipIcon.Info, bool useExitMenu = true,bool isShow=true)
        {
            win = window;
            if (notify == null)
            {
                notify = new System.Windows.Forms.NotifyIcon();
                notify.BalloonTipText = tipText;
                notify.Text = text;
                if(icoPath!="")
                    notify.Icon = new System.Drawing.Icon(icoPath);
 
                notify.Visible = true;
                //打开菜单项
                System.Windows.Forms.MenuItem open = new System.Windows.Forms.MenuItem("打开程序");
                open.Click += new EventHandler(Show);

                //退出菜单项
                System.Windows.Forms.MenuItem exit = new System.Windows.Forms.MenuItem("退出程序");
                exit.Click += new EventHandler(Close);
                exit.Enabled = useExitMenu;

                //关联托盘控件
                System.Windows.Forms.MenuItem[] childen = new System.Windows.Forms.MenuItem[] { open, exit };
                notify.ContextMenu = new System.Windows.Forms.ContextMenu(childen);

                notify.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler((o, e) =>
                {
                    if (MouseLeftDoubleClick == null)
                    {
                        if (e.Button == System.Windows.Forms.MouseButtons.Left)
                        {
                            if (win.Visibility == Visibility.Hidden || win.Visibility == Visibility.Collapsed)
                                Show(o, e);
                            else
                                Hide(o, e);
                        }
                    }
                    else
                        MouseLeftDoubleClick();


                });
                notify.MouseClick += new System.Windows.Forms.MouseEventHandler((o, e) =>
                {
                    if (e.Button == System.Windows.Forms.MouseButtons.Right)
                    {
                        MouseLeftClick?.BeginInvoke(new AsyncCallback(ShowText), null);
                    }
                    //else
                    //{
                    //    if (notify != null)
                    //    {
                    //        notify.ShowBalloonTip(3000);
                    //    }
                    //}
                });
            }
            showInTaskbar = win.ShowInTaskbar;
            notify.ShowBalloonTip(timeOut, tipTitle, tipText, tipIco);
            //win.Visibility = Visibility.Hidden;
            if(!isShow)
                Hide(null, null);
        }

        private static void ShowText(IAsyncResult ar)
        {
            MouseLeftClick();
        }
        /// <summary>
        /// 显示程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void Show(object sender, EventArgs e)
        {
            win.Visibility = Visibility.Visible;
            if(showInTaskbar)
                win.ShowInTaskbar = true;
            win.Activate();
            //notify.Visible = false;
        }
        /// <summary>
        /// 托盘程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void Hide(object sender, EventArgs e)
        {
            notify.Visible = true;
            win.ShowInTaskbar = false;
            win.Visibility = Visibility.Hidden;
        }
        /// <summary>
        /// 退出程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void Close(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
        #endregion

        #region 使用全屏
        /// <summary>
        /// 使用全屏显示
        /// </summary>
        /// <param name="win">窗口对象</param>
        public static void UseAllScreen(Window win)
        {
            // 设置全屏  
            win.WindowState = WindowState.Normal;
            win.WindowStyle = WindowStyle.None;
            win.ResizeMode = ResizeMode.NoResize;
            win.Topmost = true;
            win.Left = 0.0;
            win.Top = 0.0;
            win.Width = SystemParameters.PrimaryScreenWidth;
            win.Height = SystemParameters.PrimaryScreenHeight;
        }
        #endregion
    }
}
