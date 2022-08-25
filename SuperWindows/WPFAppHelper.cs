//using IWshRuntimeLibrary;
using SuperFramework.WindowsAPI;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using System.Windows;
using System.Windows.Interop;

namespace SuperFramework.SuperWindows
{
    /// <summary>
    /// 作者：不良帥
    /// 日期：2020-07-05
    /// 说明：wpf程序辅助
    /// </summary>
    public static class WPFAppHelper
    {
        /// <summary>
        /// 设置窗口不抢焦点
        /// </summary>
        /// <param name="win"></param>
        public static void SetWindowNoFocus(Window win)
        {

            win.Dispatcher.BeginInvoke(new Action(() =>
            {
                WindowInteropHelper helper = new WindowInteropHelper(win);
                //int pup = Convert.ToInt32(NativeConst.WS_POPUPWINDOW);
                //SetWindowLong(helper.Handle, GWL_EXSTYLE, GetWindowLong(helper.Handle, GWL_EXSTYLE) | WS_EX_NOACTIVATE | pup);
                var style = User32API.GetWindowLong(helper.Handle, NativeConst.GWL_EXSTYLE);
                long sty = style.ToInt64() | NativeConst.WS_EX_NOACTIVATE;
                User32API.SetWindowLong(helper.Handle, NativeConst.GWL_EXSTYLE, new IntPtr(sty));
            }));
            //Set the window style to noactivate.

        }
        /// <summary>
        /// 设置窗口不抢焦点
        /// </summary>
        /// <param name="handle"></param>
        public static void SetWindowNoFocus(IntPtr handle)
        {
            var style = User32API.GetWindowLong(handle, NativeConst.GWL_EXSTYLE);
            long sty = style.ToInt64() | NativeConst.WS_EX_NOACTIVATE;
            //Set the window style to noactivate.
            User32API.SetWindowLong(handle, NativeConst.GWL_EXSTYLE, new IntPtr(sty));
        }
        public static void SetWindowTopModel(IntPtr handle)
        {
            User32API.SetWindowPos(handle, NativeConst.HWND_TOP, 0, 0, 0, 0, NativeConst.SWP_NOMOVE | NativeConst.SWP_NOSIZE);

        }

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
            return new System.Drawing.Icon(System.Windows.Application.GetResourceStream(iconUri).Stream);
        }
        #endregion

        #region 以管理员启动程序，可限启动一个实例。（在 app 页面构造函数的 Startup 中调用）
        private static Mutex instance;
        /// <summary>
        /// 以管理员启动程序，可限启动一个实例。
        /// </summary>
        /// <param name="Args">数组参数</param>
        /// <param name="isOne">是否启动一个实例</param>
        /// <param name="isShowMsg">是否显示提示框</param>
        /// <param name="isAdmin">是否使用管理员模式</param>
        /// <param name="name"> 如果名称以前缀“Local\”命名，在所有终端服务器会话中均为可见;如果名称以前缀“Local\”命名，则 mutex 仅在创建它的终端服务器会话中可见</param>
        /// <param name="param">用与限制程序的参数，如exe路径时限制相同路径的程序启动，空则全局限制</param>
        public static void RunByAdmin(string[] Args, bool isOne = false, bool isShowMsg = false,bool isAdmin=true,string name= "Global",string param="")
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
            在运行终端服务的服务器上，已命名的系统 mutex 可以具有两级可见性。
            如果名称以前缀“Global\”开头，则 mutex 在所有终端服务器会话中均为可见。
            如果名称以前缀“Local\”开头，则 mutex 仅在创建它的终端服务器会话中可见。在这种情况下，服务器上各个其他终端服务器会话中都可以拥有一个名称相同的独立 mutex。
            如果创建已命名 mutex 时不指定前缀，则它将采用前缀“Local\”。在终端服务器会话中，只是名称前缀不同的两个 mutex 是独立的 mutex，这两个 mutex 对于终端服务器会话中的所有进程均为可见。即：前缀名称“Global\”和“Local\”说明 mutex 名称相对于终端服务器会话（而并非相对于进程）的范围。
            */
            //instance = new Mutex(true, string.Format("Global\\{0}{1}", System.Windows.Forms.Application.ProductName,AppHelper.AppExeName), out createdNew);//同步基元变量 
            instance = new Mutex(true, $"{name}\\{param}{AppDomain.CurrentDomain.FriendlyName}", out createdNew);//同步基元变量 
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
            if (isAdmin)
            {
                if (!principal.IsInRole(WindowsBuiltInRole.Administrator)) //判断当前登录用户是否为管理员
                {
                    try
                    {
                        //创建启动对象
                        ProcessStartInfo startInfo = new ProcessStartInfo() { /*设置运行文件*/FileName = AppHelper.AppFullPath, /*设置启动参数*/Arguments = string.Join(" ", Args), /*设置启动动作,确保以管理员身份运行*/Verb = "runas" };
                        //如果不是管理员，则启动UAC
                        Process.Start(startInfo);
                    }
                    catch (Exception) { return; }
                    Environment.Exit(0);
                }
            }
        }
        ///// <summary>
        ///// 启动一个实例。
        ///// </summary>
        ///// <param name="Args">数组参数</param>
        ///// <param name="isOne">是否启动一个实例</param>
        ///// <param name="isShowMsg">是否显示提示框</param>
        //public static void RunByOne(string[] Args, bool isOne = false, bool isShowMsg = false,bool )
        //{
        //    /**
        //     * 当前用户是管理员的时候，直接启动应用程序
        //     * 如果不是管理员，则使用启动对象启动程序，以确保使用管理员身份运行
        //     */
        //    //获得当前登录的Windows用户标示
        //    WindowsIdentity identity = WindowsIdentity.GetCurrent();
        //    WindowsPrincipal principal = new WindowsPrincipal(identity);
        //    bool createdNew; //返回是否赋予了使用线程的互斥体初始所属权 
        //    /*
        //    在运行终端服务的服务器上，已命名的系统 mutex 可以具有两级可见性。如果名称以前缀“Global\”开头，则 mutex 在所有终端服务器会话中均为可见。如果名称以前缀“Local\”开头，则 mutex 仅在创建它的终端服务器会话中可见。在这种情况下，服务器上各个其他终端服务器会话中都可以拥有一个名称相同的独立 mutex。如果创建已命名 mutex 时不指定前缀，则它将采用前缀“Local\”。在终端服务器会话中，只是名称前缀不同的两个 mutex 是独立的 mutex，这两个 mutex 对于终端服务器会话中的所有进程均为可见。即：前缀名称“Global\”和“Local\”说明 mutex 名称相对于终端服务器会话（而并非相对于进程）的范围。
        //    */
        //    instance = new Mutex(true, string.Format("Global\\{0}{1}", System.Windows.Forms.Application.ProductName, AppHelper.AppExeName), out createdNew);            //同步基元变量 
        //    if (isOne)
        //    {
        //        if (!createdNew)
        //        {
        //            if (isShowMsg)
        //                MessageBox.Show("已经启动了一个程序实例，请先退出后再次启动 !", "系统提示", MessageBoxButton.OK, MessageBoxImage.Warning);
        //            Environment.Exit(0);
        //            Process.GetCurrentProcess().Dispose();
        //        }
        //        //else
        //        //    instance.ReleaseMutex(); //赋予了线程初始所属权，也就是首次使用互斥体
        //    }
        //    if (!principal.IsInRole(WindowsBuiltInRole.Administrator)) //判断当前登录用户是否为管理员
        //    {
        //        try
        //        {
        //            //创建启动对象
        //            ProcessStartInfo startInfo = new ProcessStartInfo() { /*设置运行文件*/FileName = AppHelper.AppFullPath, /*设置启动参数*/Arguments = string.Join(" ", Args), /*设置启动动作,确保以管理员身份运行*/Verb = "runas" };
        //            //如果不是管理员，则启动UAC
        //            Process.Start(startInfo);
        //        }
        //        catch (Exception) { return; }
        //        Environment.Exit(0);
        //    }

        //}
        #endregion


        #region 刷新任务栏
        struct RECT
        {
            public int left, top, right, bottom;
        }
        private static readonly int WM_MOUSEMOVE = 512;
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        private static extern int FindWindow(string lpszClass, string lpszWindow);
        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        private static extern int FindWindowEx(int hwndParent, int hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll", EntryPoint = "GetWindowRect")]
        private static extern int GetWindowRect(int hwnd, ref System.Drawing.Rectangle lpRect);
        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(int hwnd, int wMsg, int wParam, int lParam);
        /// <summary>
        /// 刷新任务栏图标
        /// </summary>
        public static void RefreshTaskbarIcon()
        {
            //任务栏窗口
            int one = FindWindow("Shell_TrayWnd", null);
            //任务栏右边托盘图标+时间区
            int two = FindWindowEx(one, 0, "TrayNotifyWnd", null);
            //不同系统可能有可能没有这层
            int three = FindWindowEx(two, 0, "SysPager", null);
            //托盘图标窗口
            int foor;
            if (three > 0)
            {
                foor = FindWindowEx(three, 0, "ToolbarWindow32", null);
            }
            else
            {
                foor = FindWindowEx(two, 0, "ToolbarWindow32", null);
            }
            if (foor > 0)
            {
                System.Drawing.Rectangle r = new System.Drawing.Rectangle();
                GetWindowRect(foor, ref r);
                //从任务栏左上角从左到右 MOUSEMOVE一遍，所有图标状态会被更新
                for (int x = 0; x < (r.Right - r.Left) - r.X; x++)
                {
                    SendMessage(foor, WM_MOUSEMOVE, 0, (1 << 16) | x);
                }
            }
        }
        // <summary>
        /// 刷新任务栏图标
        /// </summary>
        public static void RefreshNotification()
        {
            var NotifyAreaHandle = GetNotifyAreaHandle();
            if (NotifyAreaHandle != IntPtr.Zero)
                RefreshWindow(NotifyAreaHandle);

            var NotifyOverHandle = GetNotifyOverHandle();
            if (NotifyOverHandle != IntPtr.Zero)
                RefreshWindow(NotifyOverHandle);
        }

        private static void RefreshWindow(IntPtr windowHandle)
        {
            const uint WM_MOUSEMOVE = 0x0200;
            RECT rect;
            GetClientRect(windowHandle, out rect);
            for (var x = 0; x < rect.right; x += 5)
                for (var y = 0; y < rect.bottom; y += 5)
                    SendMessage(windowHandle, WM_MOUSEMOVE, 0, (y << 16) + x);
        }

        private static IntPtr GetNotifyAreaHandle()
        {
            var TrayWndHandle = FindWindowEx(IntPtr.Zero, IntPtr.Zero, "Shell_TrayWnd", string.Empty);
            var TrayNotifyWndHandle = FindWindowEx(TrayWndHandle, IntPtr.Zero, "TrayNotifyWnd", string.Empty);
            var SysPagerHandle = FindWindowEx(TrayNotifyWndHandle, IntPtr.Zero, "SysPager", string.Empty);
            var NotifyAreaHandle = FindWindowEx(SysPagerHandle, IntPtr.Zero, "ToolbarWindow32", string.Empty);

            return NotifyAreaHandle;
        }

        private static IntPtr GetNotifyOverHandle()
        {
            var OverHandle = FindWindowEx(IntPtr.Zero, IntPtr.Zero, "NotifyIconOverflowWindow", string.Empty);
            var NotifyOverHandle = FindWindowEx(OverHandle, IntPtr.Zero, "ToolbarWindow32", string.Empty);

            return NotifyOverHandle;
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);
        [DllImport("user32.dll")]
        static extern bool GetClientRect(IntPtr handle, out RECT rect);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr handle, uint message, int wParam, int lParam);
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

        //#region 使用托盘程序
        ///// <summary>
        ///// NotifyIcon 托盘程序对象
        ///// </summary>
        //public static System.Windows.Forms.NotifyIcon notify = null;
        ///// <summary>
        ///// 程序窗口
        ///// </summary>
        //static Window notifyWin = null;
        //static bool showInTaskbar = true;
        ///// <summary>
        /////  NotifyIcon 鼠标单击委托
        ///// </summary>
        //public delegate void NotifyIconMouseClick();
        ///// <summary>
        ///// NotifyIcon 鼠标左键单击事件
        ///// </summary>
        //public static event NotifyIconMouseClick MouseLeftClick;
        ///// <summary>
        ///// NotifyIcon 鼠标左键双击事件
        ///// </summary>
        //public static event NotifyIconMouseClick MouseLeftDoubleClick;
        /////// <summary>
        ///////  NotifyIcon 退出程序委托
        /////// </summary>
        ////public delegate void NotifyIconAppCloseing();
        /////// <summary>
        /////// NotifyIcon 退出程序事件
        /////// </summary>
        ////public static event NotifyIconAppCloseing AppCloseing;
        ///// <summary>
        ///// 使用托盘程序
        ///// </summary>
        ///// <param name="window">Window窗口</param>
        ///// <param name="tipTitle">气球标题文本</param>
        ///// <param name="tipText">气球提示文本</param>
        ///// <param name="text">工具提示文本</param>
        ///// <param name="icoPath">托盘图标</param>
        ///// <param name="timeOut">提示超时时间 m(-1不提示)</param>
        ///// <param name="tipIco">气球提示ico</param>
        ///// <param name="useExitMenu">是否使用退出菜单</param>
        ///// <param name="isShow">是否显示程序</param>
        //public static void UseNotifyIcon(Window window, string tipTitle, string tipText, string text, string icoPath, int timeOut = 3000, System.Windows.Forms.ToolTipIcon tipIco = System.Windows.Forms.ToolTipIcon.Info, bool useExitMenu = true,bool isShow=true)
        //{
        //    notifyWin = window;
        //    if (notify == null)
        //    {
        //        notify = new System.Windows.Forms.NotifyIcon();
        //        notify.BalloonTipText = tipText;
        //        notify.Text = text;
        //        if(icoPath!="")
        //            notify.Icon = new System.Drawing.Icon(icoPath);

        //        notify.Visible = true;
        //        //打开菜单项
        //        System.Windows.Forms.MenuItem open = new System.Windows.Forms.MenuItem("打开程序");
        //        open.Click += new EventHandler(Show);

        //        //退出菜单项
        //        System.Windows.Forms.MenuItem exit = new System.Windows.Forms.MenuItem("退出程序");
        //        exit.Click += new EventHandler(Close);
        //        exit.Enabled = useExitMenu;

        //        //关联托盘控件
        //        System.Windows.Forms.MenuItem[] childen = new System.Windows.Forms.MenuItem[] { open, exit };
        //        notify.ContextMenu = new System.Windows.Forms.ContextMenu(childen);

        //        notify.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler((o, e) =>
        //        {
        //            if (MouseLeftDoubleClick == null)
        //            {
        //                if (e.Button == System.Windows.Forms.MouseButtons.Left)
        //                {
        //                    if (notifyWin.Visibility == Visibility.Hidden || notifyWin.Visibility == Visibility.Collapsed)
        //                        Show(o, e);
        //                    else
        //                        Hide(o, e);
        //                }
        //            }
        //            else
        //                MouseLeftDoubleClick();


        //        });
        //        notify.MouseClick += new System.Windows.Forms.MouseEventHandler((o, e) =>
        //        {
        //            if (e.Button == System.Windows.Forms.MouseButtons.Right)
        //            {
        //                MouseLeftClick?.BeginInvoke(new AsyncCallback(ShowText), null);
        //            }
        //            //else
        //            //{
        //            //    if (notify != null)
        //            //    {
        //            //        notify.ShowBalloonTip(3000);
        //            //    }
        //            //}
        //        });
        //    }
        //    showInTaskbar = notifyWin.ShowInTaskbar;
        //    if(timeOut>0)
        //        notify.ShowBalloonTip(timeOut, tipTitle, tipText, tipIco);
        //    //win.Visibility = Visibility.Hidden;
        //    if(!isShow)
        //        Hide(null, null);
        //}

        //private static void ShowText(IAsyncResult ar)
        //{
        //    MouseLeftClick();
        //}
        ///// <summary>
        ///// 显示程序
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //public static void Show(object sender, EventArgs e)
        //{
        //    notifyWin?.Dispatcher.BeginInvoke(new Action(() =>
        //    {
        //        notifyWin.Visibility = Visibility.Visible;
        //        if (showInTaskbar)
        //            notifyWin.ShowInTaskbar = true;
        //        if (notifyWin.WindowState == WindowState.Minimized)
        //            notifyWin.WindowState = WindowState.Normal;
        //        notifyWin.Activate();
        //        //notify.Visible = false;
        //    }));
        //}
        ///// <summary>
        ///// 托盘程序
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //public static void Hide(object sender, EventArgs e)
        //{
        //    notifyWin?.Dispatcher.BeginInvoke(new Action(() =>
        //    {
        //        notify.Visible = true;
        //        notifyWin.ShowInTaskbar = false;
        //        notifyWin.Visibility = Visibility.Collapsed;
        //    }));
        //}
        ///// <summary>
        ///// 退出程序
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //public static void Close(object sender, EventArgs e)
        //{
        //    notify.Dispose();
        //    notify = null;
        //    notifyWin.Close();
        //    //notifyWin = null;
        //    //Environment.Exit(0);
        //}
        //#endregion

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
