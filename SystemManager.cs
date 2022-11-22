
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static SuperFramework.WindowsAPI.Advapi32API;
using static SuperFramework.WindowsAPI.APIDelegate;
using static SuperFramework.WindowsAPI.APIStruct;
using static SuperFramework.WindowsAPI.Kernel32API;

namespace SuperFramework
{
    /// <summary>
    /// 日期:2022-01-20
    /// 作者:不良帥
    /// 描述:系统控制辅助类
    /// </summary>
    public class SystemManager
    {
        #region  枚举类型,指定可以允许的操作。 
        /// <summary>
        /// 枚举类型,指定可以允许的操作。
        /// </summary>
        public enum RestartOptions
        {
            /// <summary>
            /// Shuts down all processes running in the security context of the process that called the ExitWindowsEx function. Then it logs the user off.
            /// 注销，关闭调用ExitWindowsEx()功能的进程安全上下文中所有运行的程序，然后用户退出登录
            /// </summary>
            LogOff = 0,
            /// <summary>
            /// Shuts down the system and turns off the power. The system must support the power-off feature.
            /// 关闭操作系统和电源，计算机必须支持软件控制电源
            /// </summary>
            PowerOff = 8,
            /// <summary>
            /// Shuts down the system and then restarts the system.
            /// 关闭系统然后重启
            /// </summary>
            Reboot = 2,
            /// <summary>
            /// Shuts down the system to a point at which it is safe to turn off the power. All file buffers have been flushed to disk, and all running processes have stopped. If the system supports the power-off feature, the power is also turned off.
            /// 关闭系统，等待合适的时刻关闭电源：当所有文件的缓存区被写入磁盘，所有运行的进程停止，如果系统支持软件控制电源，就关闭电源
            /// </summary>
            ShutDown = 1,
            /// <summary>
            /// Suspends the system.
            /// 挂起
            /// </summary>
            Suspend = -1,
            /// <summary>
            /// Hibernates the system.
            /// 休眠
            /// </summary>
            Hibernate = -2,
        }
        #endregion

        #region  私有常量 
        /// <summary>用来在访问令牌中启用和禁用一个权限</summary>
        private const int TOKEN_ADJUST_PRIVILEGES = 0x20;
        /// <summary>用来查询一个访问令牌</summary>
        private const int TOKEN_QUERY = 0x8;
        /// <summary>权限启用标志</summary>
        private const int SE_PRIVILEGE_ENABLED = 0x2;
        /// <summary>指定了函数需要为请求消息查找系统消息表资源 </summary>
        private const int FORMAT_MESSAGE_FROM_SYSTEM = 0x1000;
        /// <summary>
        /// 强制停止进程。当设置了该标志后，系统不会发送WM_QUERYENDSESSION 和 WM_ENDSSESSION消息，这会使应用程序丢失数据。
        /// 因此，除非紧急，你要慎用该标志
        /// </summary>
        private const int EWX_FORCE = 4;
        /// <summary>
        /// 系统消息
        /// </summary>
        private const int WM_SYSCOMMAND = 0x112;
        /// <summary>
        /// 启动屏幕保护消息
        /// </summary>
        private const int SC_SCREENSAVE = 0xf140;
        /// <summary>
        /// 关闭显示器的系统命令
        /// </summary>
        private const int SC_MONITORPOWER = 0xF170;
        /// <summary>
        /// 2 为关闭, 1为省电状态，-1为开机
        /// </summary>
        private const int POWER_OFF = 2;
        /// <summary>
        /// 广播消息，所有顶级窗体都会接收
        /// </summary>
        private static readonly IntPtr HWND_BROADCAST = new(0xffff);
        #endregion

        #region  API函数 

        /// <summary>
        /// SetSuspendState函数关闭电源挂起系统，根据休眠参数设置，系统将挂起或者休眠，如果ForceFlag为真，
        /// 系统将立即停止所有操作，如果为假，系统将征求所有应用程序和驱动程序意见后才会这么做
        /// </summary>
        /// <param name="Hibernate">休眠参数，如果为true，系统进入修改，如果为false，系统挂起</param>
        /// <param name="ForceCritical">强制挂起. 如果为TRUE, 函数向每个应用程序和驱动广播一个 PBT_APMSUSPEND 事件, 然后立即挂起所有操作。
        /// 如果为 FALSE, 函数向每个应用程序广播一个 PBT_APMQUERYSUSPEND 事件，征求挂起</param>
        /// <param name="DisableWakeEvent">如果为 TRUE, 系统禁用所有唤醒事件，如果为 FALSE, 所有系统唤醒事件继续有效</param>
        /// <returns>如果执行成功，返回非0,如果执行失败, 返回0. 如果要获取更多错误信息，请调用 Marshal.GetLastWin32Error.</returns>
        [DllImport("powrprof.dll", EntryPoint = "SetSuspendState", CharSet = CharSet.Ansi)]
        private static extern int SetSuspendState(int Hibernate, int ForceCritical, int DisableWakeEvent);

        #endregion

        #region  退出Windows,如果需要,申请相应权限。 

        /// <summary>
        /// 退出Windows,如果需要,申请相应权限。
        /// </summary>
        /// <param name="how">操作选项,指示如何退出Windows</param>
        /// <param name="force">True表示强制退出</param>
        /// <exception cref="PrivilegeException">当申请权限时发生了一个错误</exception>
        /// <exception cref="PlatformNotSupportedException">系统不支持则引发异常</exception>
        public static void ExitWindows(RestartOptions how, bool force = false)
        {
            switch (how)
            {
                case RestartOptions.Suspend:
                    SuspendSystem(false, force);
                    break;
                case RestartOptions.Hibernate:
                    SuspendSystem(true, force);
                    break;
                default:
                    ExitWindows((int)how, force);
                    break;
            }
        }
        /// <summary>
        /// 退出Windows,如果需要,申请所有权限。
        /// </summary>
        /// <param name="how">操作选项,指示如何退出Windows</param>
        /// <param name="force">True表示强制退出</param>
        /// <remarks>本函数无法挂起或休眠系统</remarks>
        /// <exception cref="PrivilegeException">当申请一个权限时发生了错误</exception>
        private static void ExitWindows(int how, bool force = false)
        {
            EnableToken("SeShutdownPrivilege");
            if (force)
                how |= EWX_FORCE;
            if (WindowsAPI.User32API.ExitWindowsEx(how, 0) == 0)
                throw new PrivilegeException(FormatError(Marshal.GetLastWin32Error()));
        }

        /// <summary>
        /// 启用指定的权限
        /// </summary>
        /// <param name="privilege">要启用的权限</param>
        /// <exception cref="PrivilegeException">表明在申请相应权限时发生了错误</exception>
        private static void EnableToken(string privilege)
        {
            if (!CheckEntryPoint("advapi32.dll", "AdjustTokenPrivileges"))
                return;
            IntPtr tokenHandle = IntPtr.Zero;
            LUID privilegeLUID = new();
            TOKEN_PRIVILEGES newPrivileges = new();
            TOKEN_PRIVILEGES tokenPrivileges;
            if (OpenProcessToken(Process.GetCurrentProcess().Handle, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref tokenHandle) == 0)
                throw new PrivilegeException(FormatError(Marshal.GetLastWin32Error()));
            if (LookupPrivilegeValue("", privilege, ref privilegeLUID) == 0)
                throw new PrivilegeException(FormatError(Marshal.GetLastWin32Error()));
            tokenPrivileges.PrivilegeCount = 1;
            tokenPrivileges.Privileges.Attributes = SE_PRIVILEGE_ENABLED;
            tokenPrivileges.Privileges.pLuid = privilegeLUID;
            int size = 4;
            if (AdjustTokenPrivileges(tokenHandle, 0, ref tokenPrivileges, 4 + (12 * tokenPrivileges.PrivilegeCount), ref newPrivileges, ref size) == 0)
                throw new PrivilegeException(FormatError(Marshal.GetLastWin32Error()));
        }

        /// <summary>
        /// 挂起或休眠系统
        /// </summary>
        /// <param name="hibernate">True表示休眠，否则表示挂起系统</param>
        /// <param name="force">True表示强制退出</param>
        /// <exception cref="PlatformNotSupportedException">如果系统不支持，将抛出PlatformNotSupportedException.</exception>
        private static void SuspendSystem(bool hibernate, bool force)
        {
            if (!CheckEntryPoint("powrprof.dll", "SetSuspendState"))
                throw new PlatformNotSupportedException("The SetSuspendState method is not supported on this system!");
            SetSuspendState((int)(hibernate ? 1 : 0), (int)(force ? 1 : 0), 0);
        }

        /// <summary>
        /// 检测本地系统上是否存在一个指定的方法入口
        /// </summary>
        /// <param name="library">包含指定方法的库文件</param>
        /// <param name="method">指定方法的入口</param>
        /// <returns>如果存在指定方法，返回True，否则返回False</returns>
        private static bool CheckEntryPoint(string library, string method)
        {
            IntPtr libPtr = LoadLibrary(library);
            if (!libPtr.Equals(IntPtr.Zero))
            {
                if (!GetProcAddress(libPtr, method).Equals(IntPtr.Zero))
                {
                    FreeLibrary(libPtr);
                    return true;
                }
                FreeLibrary(libPtr);
            }
            return false;
        }

        /// <summary>
        /// 将错误号转换为错误消息
        /// </summary>
        /// <param name="number">需要转换的错误号</param>
        /// <returns>代表指定错误号的字符串.</returns>
        private static string FormatError(int number)
        {
            StringBuilder buffer = new(255);
            WindowsAPI.User32API.FormatMessage(FORMAT_MESSAGE_FROM_SYSTEM, IntPtr.Zero, number, 0, buffer, buffer.Capacity, 0);
            return buffer.ToString();
        }
        #endregion

        #region  关闭或打开显示器 
        /// <summary>
        /// 关闭或者打开显示器
        /// </summary>
        /// <param name="hWnd">其窗口程序将接收消息的窗口的句柄 可为0</param>
        /// <param name="isOpen">是否打开</param>
        public static void MonitorDisplay(bool isOpen, IntPtr hWnd)
        {
            int HWND_BROADCAST = 0xffff;
            if (hWnd != IntPtr.Zero)
                HWND_BROADCAST = (int)hWnd;
            if (isOpen)
                WindowsAPI.User32API.SendMessage((IntPtr)HWND_BROADCAST, WM_SYSCOMMAND, SC_MONITORPOWER, 1);  //打开显示器;
            else
                WindowsAPI.User32API.SendMessage((IntPtr)HWND_BROADCAST, WM_SYSCOMMAND, SC_MONITORPOWER, 2);  //关闭显示器;
        }
        #endregion

        #region  启动屏保 
        /// <summary>
        /// 启动屏保
        /// </summary>
        /// <param name="isStart"></param>
        public static void StartSaver()
        {
            WindowsAPI.User32API.SendMessage(HWND_BROADCAST, WM_SYSCOMMAND, SC_SCREENSAVE, 0);           // 启动屏保
        }
        #endregion

        #region  锁定系统 
        /// <summary>
        /// 锁定系统
        /// </summary>
        /// <returns></returns>
        public static bool LockWorkStation()
        {
            return WindowsAPI.User32API.LockWorkStation();
        }
        #endregion

        #region  开启锁定键盘鼠标 
        /// <summary>
        /// 开启锁定键盘鼠标
        /// </summary>
        /// <param name="block">true为锁定,false为开启</param>
        public static void BlockInput(bool block)
        {
            WindowsAPI.User32API.BlockInput(block);
        }
        #endregion

        #region  锁定任务管理器 
        static FileStream fs = null;
        /// <summary>
        /// 锁定任务管理器
        /// </summary>
        public static void LockTaskmgr()//锁定任务管理器
        {
            fs = new FileStream(Environment.ExpandEnvironmentVariables("C:\\Windows\\System32\\taskmgr.exe"), FileMode.Open);
            //byte[] Mybyte = new byte[(int)MyFs.Length];
            //MyFs.Write(Mybyte, 0, (int)MyFs.Length);
            //MyFs.Close(); //用文件流打开任务管理器应用程序而不关闭文件流就会阻止打开任务管理器
        }
        /// <summary>
        /// 解除锁定任务管理器
        /// </summary>
        public static void DeLockTaskmgr()//锁定任务管理器
        {
            if (fs != null)
            {
                fs.Close();
                fs = null;
            }

        }
        #endregion

        #region  隐藏系统滚动条 
        //private override void WndProc(ref System.Windows.Forms.Message m)
        //{
        //    ShowScrollBar(this.Handle, 3, false);//0:horizontal,1:vertical,3:both
        //    base.WndProc(ref m);
        //}

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetCurrentThreadId();

        #endregion

        #region 得到光标在屏幕上的位置
        /// <summary>
        /// 得到光标在屏幕上的位置
        /// </summary>
        /// <returns>返回光标在屏幕上的位置点</returns>
        public static Point GetCaretPos()
        {
            IntPtr ptr = WindowsAPI.User32API.GetForegroundWindow();
            Point p = new();

            //得到Caret在屏幕上的位置   
            if (ptr.ToInt32() != 0)
            {
                IntPtr targetThreadID = WindowsAPI.User32API.GetWindowThreadProcessId(ptr, IntPtr.Zero);
                IntPtr localThreadID = GetCurrentThreadId();

                if (localThreadID != targetThreadID)
                {
                    WindowsAPI.User32API.AttachThreadInput(localThreadID, targetThreadID, 1);
                    ptr = WindowsAPI.User32API.GetFocus();
                    if (ptr.ToInt32() != 0)
                    {
                        WindowsAPI.User32API.GetCaretPos(out p);
                        WindowsAPI.User32API.ClientToScreen(ptr, ref p);
                    }
                    WindowsAPI.User32API.AttachThreadInput(localThreadID, targetThreadID, 0);
                }
            }
            return p;
        }
        #endregion

        #region  设置活动窗体 
        /// <summary>
        /// 设置活动窗体
        /// </summary>
        /// <param name="frm">要设置的窗体对象</param>
        /// <returns>true：成功，false：失败</returns>
        public static bool SetForegroundWindow(Form frm)
        {
            return WindowsAPI.User32API.SetForegroundWindow(frm.Handle);
        }
        #endregion

        #region  控制鼠标在窗口内 
        /// <summary>
        /// 控制鼠标在窗口内
        /// </summary>
        /// <param name="form">窗体对象</param>
        public static void MouseInWindows(Form form)
        {
            Cursor.Clip = new Rectangle(form.Location, form.Size); //控制鼠标在窗口范围内 
        }
        #endregion

        #region  屏蔽键鼠及热键 


        static int hKeyboardHook = 0;
        static HookProc KeyboardHookProcedure;
        public const int WH_KEYBOARD = 13;
        public const int WH_MOUSE_LL = 14;



        /// <summary>
        /// 基于HOOK钩子屏蔽键盘代码，包括了安装钩子和 卸载钩子，设置的是线程钩子，
        /// 或截获键盘上的Ctrl+Esc 、Alt+Esc、alt+f4 、alt+tab、Ctrl+Shift+Esc、截获alt+空格 、截获Ctrl+Alt+Delete 、截获Ctrl+Alt+空格，
        /// 钩子函数,需要引用空间(using System.Reflection;)，
        /// 线程钩子监听键盘消息设为2,全局钩子监听键盘消息设为13，线程钩子监听鼠标消息设为7,全局钩子监听鼠标消息设为14。
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        private static int KeyboardHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            KeyboardMSG m = (KeyboardMSG)Marshal.PtrToStructure(lParam, typeof(KeyboardMSG));
            if ((int)m.vkCode == 91 || (int)m.vkCode == 92 || (int)m.vkCode == 10)
            {
                return 1;
            }
            if (((int)m.vkCode == 46) && ((int)m.vkCode == 17) && ((int)m.vkCode == 18))
            {
                return 2;
            }
            if (m.vkCode == (int)Keys.Escape && (int)Control.ModifierKeys == (int)Keys.Control) //截获Ctrl+Esc 
            {
                return 1;
            }

            if (m.vkCode == (int)Keys.Escape && (int)Control.ModifierKeys == (int)Keys.Alt) //截获Alt+Esc 
            {
                return 1;
            }
            if (m.vkCode == (int)Keys.F4 && (int)Control.ModifierKeys == (int)Keys.Alt) //截获alt+f4 
            {
                return 1;
            }
            if (m.vkCode == (int)Keys.Tab && (int)Control.ModifierKeys == (int)Keys.Alt) //截获alt+tab
            {
                return 1;
            }
            if (m.vkCode == (int)Keys.Escape && (int)Control.ModifierKeys == (int)Keys.Control + (int)Keys.Shift) //截获Ctrl+Shift+Esc
            {
                return 1;
            }
            if (m.vkCode == (int)Keys.Space && (int)Control.ModifierKeys == (int)Keys.Alt) //截获alt+空格 
            {
                return 1;
            }
            if (m.vkCode == (int)Keys.Delete && (int)Control.ModifierKeys == (int)Keys.Control + (int)Keys.Alt)      //截获Ctrl+Alt+Delete 
            {
                return 1;
            }
            if (m.vkCode == (int)Keys.Space && (int)Control.ModifierKeys == (int)Keys.Control + (int)Keys.Alt) //截获Ctrl+Alt+空格 
            {
                return 1;
            }
            return WindowsAPI.User32API.CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);
        }
        /// <summary>
        /// 安装钩子屏蔽键盘，热键，鼠标
        /// </summary>
        public static void KeyMaskStart()
        {
            if (hKeyboardHook == 0)
            {
                // 创建HookProc实例
                KeyboardHookProcedure = new HookProc(KeyboardHookProc);
                // 设置线程钩子
                hKeyboardHook = WindowsAPI.User32API.SetWindowsHookEx(WH_KEYBOARD, KeyboardHookProcedure, Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]), 0);
                // 如果设置钩子失败
                if (hKeyboardHook == 0)
                {
                    KeyMaskStop();
                    throw new Exception("SetWindowsHookEx failed.");
                }
            }
        }
        /// <summary>
        ///  卸载钩子取消屏蔽键盘，热键，鼠标
        /// </summary>
        public static void KeyMaskStop()
        {
            bool retKeyboard = true;
            if (hKeyboardHook != 0)
            {
                retKeyboard = WindowsAPI.User32API.UnhookWindowsHookEx(hKeyboardHook);
                hKeyboardHook = 0;
            }
            if (!(retKeyboard))
            {
                throw new Exception("UnhookWindowsHookEx  failed.");
            }
        }
        #endregion

        #region  检查键盘大小写锁定状态 

        /// <summary>
        /// 检查键盘大小写锁定状态
        /// </summary>
        /// <returns>返回true为大写锁定状态，false为小写锁定状态</returns>
        public static bool CapsLockStatus()
        {
            byte[] bs = new byte[256];
            WindowsAPI.User32API.GetKeyboardState(bs);
            return (bs[0x14] == 1);
        }
        #endregion

        #region  获取系统鼠标双击时间间隔 
        /// <summary>
        /// 获取系统鼠标双击时间间隔
        /// </summary>
        /// <returns>返回间隔时间（毫秒）</returns>

        [DllImport("user32.dll", EntryPoint = "GetDoubleClickTime")]

        public extern static int GetDoubleClickTime();
        #endregion

        #region  设置系统鼠标双击时间间隔 
        /// <summary>
        /// 设置系统鼠标双击时间间隔
        /// </summary>
        /// <param name="time">毫秒间隔</param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "SetDoubleClickTime")]
        public extern static int SetDoubleClickTime(int time);
        #endregion

        #region  异常信息类 
        /// <summary>
        /// 如果在申请一个权限时发生错，将引发本异常。
        /// </summary>
        public class PrivilegeException : Exception
        {
            /// <summary>
            /// Initializes a new instance of the PrivilegeException class.
            /// </summary>
            public PrivilegeException() : base() { }
            /// <summary>
            /// Initializes a new instance of the PrivilegeException class with a specified error message.
            /// </summary>
            /// <param name="message">The message that describes the error.</param>
            public PrivilegeException(string message) : base(message) { }
        }
        #endregion

        #region  windows账户操作 

        /// <summary>
        /// 创建Windows帐户
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">用户密码</param>
        /// <param name="description">用户描述</param>
        public static void CreateLocalUser(string username, string password, string description)
        {
            DirectoryEntry localMachine = new(string.Format("WinNT://{0},computer", Environment.MachineName));
            var newUser = localMachine.Children.Add(username, "user");
            newUser.Invoke("SetPassword", new object[] { password });
            newUser.Invoke("Put", new object[] { "Description", description });
            newUser.CommitChanges();
            localMachine.Close();
            newUser.Close();
        }
        /// <summary>
        /// 重置指定用户的密码
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">新密码</param>
        public static void ResetUserPassword(string userName, string password)
        {
            string _Path = "WinNT://" + Environment.MachineName;

            DirectoryEntry machine = new(_Path); //获得计算机实例
            DirectoryEntry user = machine.Children.Find(userName, "User"); //找得用户
            if (user != null)
            {
                user.Invoke("SetPassword", password); //用户密码
                user.CommitChanges();
            }
        }
        /// <summary>
        /// 更改Windows帐户密码
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="oldPwd">旧密码</param>
        /// <param name="newPwd">新密码</param>
        public static void ChangeWinUserPasswd(string username, string oldPwd, string newPwd)
        {
            DirectoryEntry localMachine = new(string.Format("WinNT://{0},computer", Environment.MachineName));
            DirectoryEntry user = localMachine.Children.Find(username, "user");
            object[] password = new object[] { oldPwd, newPwd };
            object ret = user.Invoke("ChangePassword", password);
            user.CommitChanges();
            localMachine.Close();
            user.Close();
        }
        /// <summary>
        /// 判断Windows用户是否存在
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns>存在返回true，不存在返回false</returns>
        public static bool ExistWinUser(string username)
        {
            try
            {
                using (DirectoryEntry localMachine = new(string.Format("WinNT://{0},computer", Environment.MachineName)))
                {
                    var user = localMachine.Children.Find(username, "user");
                    return user != null;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 删除Windows用户
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns>成功返回true，失败返回false</returns>
        public static bool DeleteWinUser(string username)
        {
            try
            {
                using (DirectoryEntry localMachine = new(string.Format("WinNT://{0},computer", Environment.MachineName)))
                {
                    //删除存在用户
                    var delUser = localMachine.Children.Find(username, "user");
                    if (delUser != null)
                    {
                        localMachine.Children.Remove(delUser);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 启用/禁用windows帐户
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="isDisable">是否启用</param>
        public static void Disable(string username, bool isDisable)
        {
            var userDn = string.Format("WinNT://{0}/{1},user", Environment.MachineName, username);
            DirectoryEntry user = new(userDn);
            user.InvokeSet("AccountDisabled", isDisable);
            user.CommitChanges();
            user.Close();
        }
        #endregion

        #region 任务栏
        /// <summary>
        /// 显示任务栏
        /// </summary>
        public static void ShowTaskbar(bool isShow)
        {
            IntPtr trayHwnd = WindowsAPI.User32API.FindWindow("Shell_TrayWnd", null);

            if (trayHwnd != IntPtr.Zero)
            {
                WindowsAPI.User32API.ShowWindow(trayHwnd, isShow ? 1 : 0);
            }

            IntPtr hStar = WindowsAPI.User32API.FindWindowEx(IntPtr.Zero, IntPtr.Zero, "Button", null);
            if (hStar != IntPtr.Zero)
            {
                WindowsAPI.User32API.ShowWindow(hStar, isShow ? 1 : 0);
            }
        }
        /// <summary>
        /// 隐藏任务栏
        /// </summary>
        public static void HideTaskbar()
        {
            IntPtr trayHwnd = WindowsAPI.User32API.FindWindow("Shell_TrayWnd", null);//获得窗口的句柄
            if (trayHwnd != IntPtr.Zero)  //初始化不为0的句柄或指针
            {
                WindowsAPI.User32API.ShowWindow(trayHwnd, 0);//该函数设置指定窗口的显示状态
            }

            IntPtr hStar = WindowsAPI.User32API.FindWindowEx(IntPtr.Zero, IntPtr.Zero, "Button", null);
            if (hStar != IntPtr.Zero)
            {
                WindowsAPI.User32API.ShowWindow(hStar, 0);
            }
        }
        #endregion


        /// <summary>
        /// 得到系统中显示设备的信息
        /// </summary>
        /// <param name="lpDevice">该参数当前不用，应设为Null。</param>
        /// <param name="iDevNum">指定感兴趣的显示设备的索引值，操作系统通过索引值确定每一个显示设备。索引值是连续的整数。从0开始，例如：如果一个系统有三个显示设备，那么它们的索引值为0、1、2。</param>
        /// <param name="lpDisplayDevice">DISPLAY_DEVICE结构的指针，该结构检索由iDevNum指定的显示设备的信息，在调用EnumDisplayDevices之前，必须以字节为单位把DISPLAY_DEVICE结构中cb元素初始化为DISPLAY_DEVICE结构的大小。</param>
        /// <param name="dwFlags">约束函数行为的一组标志，当前没有定义标志。</param>
        /// <remarks>为了查询系统的所有显示设备，在一个循环中调用该函数开始时iDevNum设为0，并增加iDevNum，直到函数失败。</remarks>
        /// <returns>如果成功，则返回值非零；如果失败，则返回值为零；如果iDevNum大于最大的设备索引，则函数失败。</returns>
        [DllImport("user32.dll")]
        public static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);
        /// <summary>
        /// 该函数得到显示设备的一个图形模式设备，通过对该函数一系列的调用可以得到显示设备所有的图形模式信息。
        /// </summary>
        /// <param name="deviceName">指向一个以null的结尾的字符串，该字符串指定了显示设备。此函数将获得该显示设备的图形模式信息。该参数可以为NULL。NULL值表明调用线程正运行在计算机的当前显示设备上。如果lpszDeviceName为NULL，该字符串的形式为\.displayx，其中x的值可以为1、2或3。对于Windows 95和Windows 98，lpszDeviceName必须为NULL。</param>
        /// <param name="modeNum">表明要检索的信息类型，该值可以是一个图形模式索引</param>
        /// <param name="devMode"></param>
        /// <returns>如果成功，返回非零值；如果失败，返回零。</returns>
        [DllImport("user32.dll")]
        public static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);
        /// <summary>
        /// 该函数把显示设备在lpszDeviceName参数中定义的设置，改变为在lpDevMode参数中定义的图形模式。
        /// </summary>
        /// <param name="lpszDeviceName">指向一个以null结尾的字符串的指针，该字符串指定了一个显示设备，该函数从该显示设备中得到它的图形模式的信息。</param>
        /// <param name="lpDevMode">指向DEVMOD结构的指针，该结构描述了要经向的图形模式dmSize元素，必须以字节为单位，初始化为DEVMODE结构的尺寸</param>
        /// <param name="hwnd">必须为NULL</param>
        /// <param name="dwflags">表明图形模式如何改变</param>
        /// <param name="lParam">必须为空</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern DISP_CHANGE ChangeDisplaySettingsEx(string lpszDeviceName, ref DEVMODE lpDevMode, IntPtr hwnd, uint dwflags, IntPtr lParam);
        DEVMODE ddActive = new();
        DEVMODE ddInactive = new();
        string szActiveDeviceName;
        string szInactiveDeviceName;
        const int ENUM_REGISTRY_SETTINGS = -2;
        void GetDisplayInfo()
        {
            ddActive.dmSize = (short)Marshal.SizeOf(ddActive);
            ddInactive.dmSize = (short)Marshal.SizeOf(ddInactive);
            uint iDeviceCntr = 0;
            DISPLAY_DEVICE dd = new();
            dd.cb = Marshal.SizeOf(dd);
            while (EnumDisplayDevices(null, iDeviceCntr, ref dd, 0))
            {
                DEVMODE dMode = new();
                dMode.dmSize = (short)Marshal.SizeOf(dMode);
                if (EnumDisplaySettings(dd.DeviceName, ENUM_REGISTRY_SETTINGS, ref dMode))
                {
                    if (dMode.dmPelsHeight > 0 && dMode.dmPelsWidth > 0)
                    {
                        if ((dd.StateFlags & (int)DisplayDeviceStateFlags.PrimaryDevice) ==
                             (int)DisplayDeviceStateFlags.PrimaryDevice)
                        {
                            ddActive = dMode;
                            szActiveDeviceName = dd.DeviceName;
                        }
                        else
                        {
                            ddInactive = dMode;
                            szInactiveDeviceName = dd.DeviceName;
                        }
                    }
                }
                iDeviceCntr++;
            }
        }
        /// <summary>
        /// 将主显示设备设置为活动并禁用第二个显示设备。这使用 API。
        /// </summary>
        public void ChangeToSingleDisplay()
        {
            GetDisplayInfo();

            ddInactive.dmPosition.x = 0;
            ddInactive.dmPosition.y = 0;
            ddInactive.dmPelsHeight = 0;
            ddInactive.dmPelsWidth = 0;
            ddInactive.dmFields = DM.PelsHeight | DM.PelsWidth | DM.Position;

            ChangeDisplaySettingsEx(szInactiveDeviceName, ref ddInactive,
              IntPtr.Zero, (int)(CDSFlags.CDS_RESET | CDSFlags.CDS_UPDATEREGISTRY), IntPtr.Zero);
        }
        /// <summary>
        /// 我们将获取刷新显示信息，并尝试切换显示器（使活动设备处于非活动状态，反之亦然）。
        /// </summary>
        public void ChangeToSingleDisplay2()
        {
            GetDisplayInfo();
            ddInactive.dmPosition.x = 0;
            ddInactive.dmPosition.y = 0;
            ddInactive.dmFields = DM.Position;
            if (DISP_CHANGE.Successful == ChangeDisplaySettingsEx(szInactiveDeviceName,
                ref ddInactive, IntPtr.Zero,
                (int)(CDSFlags.CDS_RESET | CDSFlags.CDS_UPDATEREGISTRY |
                 CDSFlags.CDS_SET_PRIMARY), IntPtr.Zero))
            {
                ddActive.dmPosition.x = 0;
                ddActive.dmPosition.y = 0;
                ddActive.dmPelsHeight = 0;
                ddActive.dmPelsWidth = 0;
                ddActive.dmFields = DM.PelsHeight | DM.PelsWidth | DM.Position;

                ChangeDisplaySettingsEx(szActiveDeviceName, ref ddActive,
                  IntPtr.Zero, (int)(CDSFlags.CDS_RESET |
                  CDSFlags.CDS_UPDATEREGISTRY), IntPtr.Zero);
            }
        }
        /// <summary>
        /// 获取设备信息。这将设置"活动"和"非活动设备"信息，供您的代码使用。
        /// </summary>
        /// <param name="ddActive"></param>
        /// <param name="ddInactive"></param>
        /// <returns></returns>
        public static uint GetDisplayInfo(ref Dictionary<string, DEVMODE> ddActive, ref Dictionary<string, DEVMODE> ddInactive)
        {
            //Member variables
            ddActive = new Dictionary<string, DEVMODE>();
            ddInactive = new Dictionary<string, DEVMODE>();
            string szActiveDeviceName;
            string szInactiveDeviceName;
            const int ENUM_REGISTRY_SETTINGS = -2;
            //ddActive.dmSize = (short)Marshal.SizeOf(ddActive);
            //ddInactive.dmSize = (short)Marshal.SizeOf(ddInactive);
            uint iDeviceCntr = 0;
            DISPLAY_DEVICE dd = new();
            dd.cb = Marshal.SizeOf(dd);
            while (EnumDisplayDevices(null, iDeviceCntr, ref dd, 0))
            {
                DEVMODE dMode = new();
                dMode.dmSize = (short)Marshal.SizeOf(dMode);
                if (EnumDisplaySettings(dd.DeviceName, ENUM_REGISTRY_SETTINGS, ref dMode))
                {
                    if (dMode.dmPelsHeight > 0 && dMode.dmPelsWidth > 0)
                    {
                        if ((dd.StateFlags & (int)DisplayDeviceStateFlags.PrimaryDevice) ==
                             (int)DisplayDeviceStateFlags.PrimaryDevice)
                        {
                            ddActive.Add(dd.DeviceName, dMode);
                            szActiveDeviceName = dd.DeviceName;
                        }
                        else
                        {
                            ddInactive.Add(dd.DeviceName, dMode);
                            szInactiveDeviceName = dd.DeviceName;
                        }
                    }
                }
                iDeviceCntr++;
            }
            return iDeviceCntr;
        }
        /// <summary>
        /// 包含了有关设备初始化和打印机环境的信息
        /// </summary>
        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi)]
        public struct DEVMODE
        {
            public const int CCHDEVICENAME = 32;
            public const int CCHFORMNAME = 32;
            /// <summary>
            /// 指定了驱动程序支持的设备名称；例如，对于PCL/HP LaserJet系列，会是CL/HP激光打印机。这个字符串在设备驱动程序之间是相互不同的。
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
            [FieldOffset(0)]
            public string dmDeviceName;
            /// <summary>
            /// 指定了初始化数据的版本数字，这个结构就基于这些数据。
            /// </summary>
            [FieldOffset(32)]
            public short dmSpecVersion;
            /// <summary>
            /// 指定了打印机驱动程序开发商分配的打印机驱动程序版本号。
            /// </summary>
            [FieldOffset(34)]
            public short dmDriverVersion;
            [FieldOffset(36)]
            public short dmSize;
            /// <summary>
            /// 包含了这个结构后面的私有驱动程序数据的数目，以字节为单位。如果设备驱动程序不使用该设备独有的信息，就把这个成员设为零。
            /// </summary>
            [FieldOffset(38)]
            public short dmDriverExtra;
            /// <summary>
            /// 指定了DEVMODE结构的其余成员中哪些已被初始化。第0位（定义为DM）ORIENTATION）代表dmOrientation，第1位（定义为DM_PAPERSIZE）代表dmPaperSize等等。打印机驱动出现仅支持那些适合打印技术的成员。
            /// </summary>
            [FieldOffset(40)]
            public DM dmFields;
            [FieldOffset(44)]
            short dmOrientation;
            [FieldOffset(46)]
            short dmPaperSize;
            [FieldOffset(48)]
            short dmPaperLength;
            [FieldOffset(50)]
            short dmPaperWidth;
            [FieldOffset(52)]
            short dmScale;
            [FieldOffset(54)]
            short dmCopies;
            /// <summary>
            /// 保留，必须为0。
            /// </summary>
            [FieldOffset(56)]
            short dmDefaultSource;
            [FieldOffset(58)]
            short dmPrintQuality;

            [FieldOffset(44)]
            public POINTL dmPosition;
            [FieldOffset(52)]
            public int dmDisplayOrientation;
            [FieldOffset(56)]
            public int dmDisplayFixedOutput;

            [FieldOffset(60)]
            public short dmColor;
            [FieldOffset(62)]
            public short dmDuplex;
            /// <summary>
            /// 指定Y轴DPI，若初始化设定这个值，PrintQuality 设置值为X轴DPI
            /// </summary>
            [FieldOffset(64)]
            public short dmYResolution;
            [FieldOffset(66)]
            public short dmTTOption;
            [FieldOffset(68)]
            public short dmCollate;
            [FieldOffset(72)]
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHFORMNAME)]
            public string dmFormName;
            [FieldOffset(102)]
            public short dmLogPixels;
            /// <summary>
            /// 指定了显示设备的颜色分辨率，以像素的位数为单位。例如，16色使用4位，256色使用8位，而65536色使用16位。
            /// </summary>
            [FieldOffset(104)]
            public int dmBitsPerPel;
            /// <summary>
            /// 指定了可见设备表面的以像素为单位的宽度。dmPelsHeight指定了可见设备表面的以像素为单位的高度。
            /// </summary>
            [FieldOffset(108)]
            public int dmPelsWidth;
            [FieldOffset(112)]
            public int dmPelsHeight;
            [FieldOffset(116)]
            public int dmDisplayFlags;
            [FieldOffset(116)]
            public int dmNup;
            /// <summary>
            /// 指定了显示设备的特定模式所使用的以赫兹为单位的频率（每秒的周期数）。
            /// </summary>
            [FieldOffset(120)]
            public int dmDisplayFrequency;

        }

        [Flags()]
        public enum DisplayDeviceStateFlags : int
        {
            AttachedToDesktop = 0x1,
            MultiDriver = 0x2,
            PrimaryDevice = 0x4,
            MirroringDriver = 0x8,
            VGACompatible = 0x16,
            Removable = 0x20,
            ModesPruned = 0x8000000,
            Remote = 0x4000000,
            Disconnect = 0x2000000
        }

        [Flags()]
        public enum DM : int
        {
            Orientation = 0x1,
            PaperSize = 0x2,
            PaperLength = 0x4,
            PaperWidth = 0x8,
            Scale = 0x10,
            Position = 0x20,
            NUP = 0x40,
            DisplayOrientation = 0x80,
            Copies = 0x100,
            DefaultSource = 0x200,
            PrintQuality = 0x400,
            Color = 0x800,
            Duplex = 0x1000,
            YResolution = 0x2000,
            TTOption = 0x4000,
            Collate = 0x8000,
            FormName = 0x10000,
            LogPixels = 0x20000,
            BitsPerPixel = 0x40000,
            PelsWidth = 0x80000,
            PelsHeight = 0x100000,
            DisplayFlags = 0x200000,
            DisplayFrequency = 0x400000,
            ICMMethod = 0x800000,
            ICMIntent = 0x1000000,
            MediaType = 0x2000000,
            DitherType = 0x4000000,
            PanningWidth = 0x8000000,
            PanningHeight = 0x10000000,
            DisplayFixedOutput = 0x20000000
        }

        public enum DISP_CHANGE : int
        {
            Successful = 0,
            Restart = 1,
            Failed = -1,
            BadMode = -2,
            NotUpdated = -3,
            BadFlags = -4,
            BadParam = -5,
            BadDualView = -1
        }

        public struct POINTL
        {
            public long x;
            public long y;
        }

        enum CDSFlags
        {
            CDS_RESET = 0x40000000,
            CDS_UPDATEREGISTRY = 0x00000001,
            CDS_SET_PRIMARY = 0x00000010
        }
        /// <summary>
        /// 显示设备的信息
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct DISPLAY_DEVICE
        {
            public int cb;
            /// <summary>
            /// 识别显示设备名称
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;
            /// <summary>
            /// 包含显示设备上下文字符串的字符阵列。这要么是显示适配器的描述，要么是显示监视器的描述。
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceString;
            /// <summary>
            /// 一个或多个设备状态标志
            /// </summary>
            public int StateFlags;
            /// <summary>
            /// Windows 98/Me：唯一标识硬件适配器或显示器的字符串。这是"即插即用"标识符。
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceID;
            /// <summary>
            /// 已保留
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceKey;
            /// <summary>
            /// 显示设备的信息
            /// </summary>
            /// <param name="flags"></param>
            public DISPLAY_DEVICE(int flags)
            {
                cb = 0;
                StateFlags = flags;
                DeviceName = new string((char)32, 32);
                DeviceString = new string((char)32, 128);
                DeviceID = new string((char)32, 128);
                DeviceKey = new string((char)32, 128);
                cb = Marshal.SizeOf(this);
            }
        }
        /// <summary>
        /// 显示器显示模式
        /// </summary>
        public enum DisplaySwitchEnum
        {
            /// <summary>
            /// 默认
            /// </summary>
            Default,
            /// <summary>
            /// 仅电脑屏幕
            /// </summary>
            Internal,
            /// <summary>
            /// 复制屏
            /// </summary>
            Clone,
            /// <summary>
            /// 扩展屏
            /// </summary>
            Extend,
            /// <summary>
            /// 仅第二屏幕
            /// </summary>
            External
        }
        /// <summary>
        /// 切换显示模式
        /// </summary>
        /// <param name="displaySwitch"></param>
        public static void DisplaySwitch(DisplaySwitchEnum displaySwitch)
        {
            try
            {
                Process process = new();
                string str = Environment.GetEnvironmentVariable("windir");//获取系统目录
                string dir = "System32";
                if (!Environment.Is64BitProcess)
                {
                    dir = "SysNative";//非64位进程的使用这个目录
                }
                process.StartInfo.WorkingDirectory = System.IO.Path.Combine(str, dir);

                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                string cmd = string.Empty;
                switch (displaySwitch)
                {
                    case DisplaySwitchEnum.Clone:
                        cmd = "displayswitch.exe /clone";
                        break;
                    case DisplaySwitchEnum.Extend:
                        cmd = "displayswitch.exe /extend";
                        break;
                    case DisplaySwitchEnum.External:
                        cmd = "displayswitch.exe /external";
                        break;
                    case DisplaySwitchEnum.Internal:
                        cmd = "displayswitch.exe /internal";
                        break;
                }
                process.StandardInput.WriteLine(cmd);
                process.Close();
            }
            catch { }
        }

        /// <summary>
        /// 获取所有已经安装的程序
        /// </summary>
        /// <param name="registryKey">注册表跟</param>
        /// <param name="count">数量</param>
        /// <returns>程序名称,安装路径</returns> 
        public static List<ProgramInfo> GetProgramAndPath(RegistryKey registryKey, out int count)
        {
            var reg = new string[] {
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
                @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
            };
            var tags = new RegistryKey[] {
            RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32),
            RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
        };
            string tempType = null;
            int softNum = 0;//所有已经安装的程序数量
            List<ProgramInfo> ls = new();

            foreach (var item222 in tags)
            {
                using (RegistryKey pregkey = item222.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"))//Registry.LocalMachine.OpenSubKey(item222);//获取指定路径下的键 
                {
                    if (pregkey == null)
                        continue;
                    foreach (string item in pregkey?.GetSubKeyNames())               //循环所有子键
                    {
                        ProgramInfo programInfo = new();
                        using (RegistryKey currentKey = pregkey.OpenSubKey(item, false))
                        {
                            try
                            {
                                string[] vs = currentKey.GetValueNames();
                                if (vs.Length == 0)
                                    continue;
                                programInfo.DisplayName = currentKey.GetValue("DisplayName", "Nothing");           //获取显示名称
                                programInfo.InstallLocation = currentKey.GetValue("InstallLocation", "Nothing");   //获取安装路径
                                programInfo.UninstallString = currentKey.GetValue("UninstallString", "Nothing");   //获取卸载字符串路径
                                programInfo.ReleaseType = currentKey.GetValue("ReleaseType", "Nothing");           //发行类型,值是Security Update为安全更新,Update为更新
                                programInfo.DisplayIco = currentKey.GetValue("DisplayIco", "Nothing");
                                programInfo.DisplayVersion = currentKey.GetValue("DisplayVersion", "Nothing");
                                programInfo.InstallDate = currentKey.GetValue("InstallDate", "Nothing");
                                programInfo.NoModify = currentKey.GetValue("NoModify", "Nothing");
                                programInfo.NoRepair = currentKey.GetValue("NoRepair", "Nothing");
                                programInfo.Publisher = currentKey.GetValue("Publisher", "Nothing");
                                programInfo.Version = currentKey.GetValue("Version", "Nothing");
                                programInfo.VersionMajor = currentKey.GetValue("VersionMajor", "Nothing");
                                programInfo.VersionMinor = currentKey.GetValue("VersionMinor", "Nothing");
                                programInfo.Language = currentKey.GetValue("Language", "Nothing");
                                programInfo.ModifyPath = currentKey.GetValue("ModifyPath", "Nothing");
                                programInfo.Size = currentKey.GetValue("Size", "Nothing");
                                bool isSecurityUpdate = false;

                                if (programInfo.ReleaseType.ToString() != "Nothing")
                                {
                                    tempType = programInfo.ReleaseType.ToString();
                                    if (tempType == "Security Update" || tempType == "Update")
                                    {
                                        isSecurityUpdate = true;
                                    }
                                }
                                if (!isSecurityUpdate && programInfo.DisplayName.ToString() != "Nothing" && programInfo.UninstallString.ToString() != "Nothing")
                                {
                                    softNum++;
                                    //if (programInfo.InstallLocation == null)
                                    //{
                                    ls.Add(programInfo);
                                    //}
                                    //else
                                    //{
                                    //    ls.Add(programInfo);
                                    //}
                                }
                            }
                            catch { }
                        }
                    }
                }
            }
            count = softNum;
            return ls;
        }
        /// <summary>
        /// 程序信息
        /// </summary>
        public struct ProgramInfo
        {
            /// <summary>
            /// 显示名称
            /// </summary>
            public object DisplayName;
            /// <summary>
            /// 卸载字符串路径
            /// </summary>
            public object UninstallString;
            /// <summary>
            /// 安装路径
            /// </summary>
            public object InstallLocation;
            /// <summary>
            /// 发行类型,值是Security Update为安全更新,Update为更新
            /// </summary>
            public object ReleaseType;
            /// <summary>
            /// 显示ICO
            /// </summary>
            public object DisplayIco;
            /// <summary>
            /// 禁止修改
            /// </summary>
            public object NoModify;
            /// <summary>
            /// 禁止修复
            /// </summary>
            public object NoRepair;
            /// <summary>
            /// 发行
            /// </summary>
            public object Publisher;
            /// <summary>
            /// 版本号
            /// </summary>
            public object Version;
            /// <summary>
            /// 显示版本
            /// </summary>
            public object DisplayVersion;
            /// <summary>
            /// 安装日期
            /// </summary>
            public object InstallDate;
            /// <summary>
            /// 主要版本
            /// </summary>
            public object VersionMajor;
            /// <summary>
            /// 次要版本
            /// </summary>
            public object VersionMinor;
            /// <summary>
            /// 命令
            /// </summary>
            public object Language;
            /// <summary>
            /// 修改命令
            /// </summary>
            public object ModifyPath;
            /// <summary>
            /// 
            /// </summary>
            public object Size;

        }
        /// <summary>
        /// 获取所有已经安装的程序
        /// </summary>
        /// <returns>程序名称,安装路径···</returns> 
        public async static Task<List<ProgramInfo>> GetSoftWares()
        {
            List<RegistryKey> registryKeys = new()
            {
                //Registry.ClassesRoot,
                //Registry.CurrentConfig,
                //Registry.CurrentUser,
                Registry.LocalMachine,
                //Registry.PerformanceData,
                //Registry.Users
            };
            //string subKeyName = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            var ls = new List<ProgramInfo>();
            int sum = 0;
            int x = 0;
            foreach (var item in registryKeys)
            {
                _ = Task.Factory.StartNew(new Action(() =>
                 {
                     ls.AddRange(GetProgramAndPath(item, out int count));
                     sum += count;
                     x++;
                 }));
                //using (RegistryKey reg = item.OpenSubKey(subKeyName, false))
                //{
                //    if (reg == null)
                //        continue;
                //    foreach (var keyName in reg.GetSubKeyNames())
                //    {
                //        using (RegistryKey registry = reg.OpenSubKey(keyName))
                //        {
                //            if (registry == null)
                //                continue;
                //            string name = registry.GetValue("DisplayName").ToString();
                //            string installLocation = registry.GetValue("InstallLocation").ToString();
                //            if (!string.IsNullOrWhiteSpace(installLocation))
                //                keyValuePairs.Add(name, installLocation);
                //        }
                //    }
                //}
            }
            while (x != registryKeys.Count)
            {
                await Task.Delay(25).ConfigureAwait(false);
            }
            return ls;
        }

        #region 音量
        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        [DllImport("user32.dll")]
        static extern byte MapVirtualKey(uint uCode, uint uMapType);

        private const byte VK_VOLUME_MUTE = 0xAD;
        private const byte VK_VOLUME_DOWN = 0xAE;
        private const byte VK_VOLUME_UP = 0xAF;
        private const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        private const uint KEYEVENTF_KEYUP = 0x0002;

        /// <summary>
        /// 改变系统音量大小，增加
        /// </summary>
        public static void VolumeUp()
        {
            keybd_event(VK_VOLUME_UP, MapVirtualKey(VK_VOLUME_UP, 0), KEYEVENTF_EXTENDEDKEY, 0);
            keybd_event(VK_VOLUME_UP, MapVirtualKey(VK_VOLUME_UP, 0), KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
        }

        /// <summary>
        /// 改变系统音量大小，减小
        /// </summary>
        public static void VolumeDown()
        {
            keybd_event(VK_VOLUME_DOWN, MapVirtualKey(VK_VOLUME_DOWN, 0), KEYEVENTF_EXTENDEDKEY, 0);
            keybd_event(VK_VOLUME_DOWN, MapVirtualKey(VK_VOLUME_DOWN, 0), KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
        }

        /// <summary>
        /// 改变系统音量大小，静音
        /// </summary>
        public static void Mute()
        {
            keybd_event(VK_VOLUME_MUTE, MapVirtualKey(VK_VOLUME_MUTE, 0), KEYEVENTF_EXTENDEDKEY, 0);
            keybd_event(VK_VOLUME_MUTE, MapVirtualKey(VK_VOLUME_MUTE, 0), KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
        }
        [DllImport("Winmm.dll")]
        private static extern int waveOutSetVolume(int hwo, uint pdwVolume);

        [DllImport("Winmm.dll")]
        private static extern uint waveOutGetVolume(int hwo, out uint pdwVolume);

        private static int volumeMinScope = 0;
        private static int volumeMaxScope = 100;
        private static int volumeSize = 100;

        /// <summary>
        /// 音量控制，但不改变系统音量设置
        /// </summary>
        public static int VolumeSize
        {
            get { return volumeSize; }
            set { volumeSize = value; }
        }

        public static void SetCurrentVolume()
        {
            if (volumeSize < 0)
            {
                volumeSize = 0;
            }

            if (volumeSize > 100)
            {
                volumeSize = 100;
            }
            uint Value = (uint)((double)0xffff * (double)volumeSize / (double)(volumeMaxScope - volumeMinScope));//先把trackbar的value值映射到0x0000～0xFFFF范围


            //限制value的取值范围
            if (Value < 0)
            {
                Value = 0;
            }

            if (Value > 0xffff)
            {
                Value = 0xffff;
            }

            uint left = (uint)Value;//左声道音量
            uint right = (uint)Value;//右
            waveOutSetVolume(0, left << 16 | right); //"<<"左移，“|”逻辑或运算
        }
        #endregion

        /// <summary>
        /// 刷新任务栏通知区域
        /// </summary>
        public static void RefreshTaskbarNotificationArea()
        {
            IntPtr systemTrayContainerHandle = SuperFramework.WindowsAPI.User32API.FindWindow("Shell_TrayWnd", null);
            IntPtr systemTrayHandle = SuperFramework.WindowsAPI.User32API.FindWindowEx(systemTrayContainerHandle, IntPtr.Zero, "TrayNotifyWnd", null);
            IntPtr sysPagerHandle = SuperFramework.WindowsAPI.User32API.FindWindowEx(systemTrayHandle, IntPtr.Zero, "SysPager", null);
            IntPtr notificationAreaHandle = SuperFramework.WindowsAPI.User32API.FindWindowEx(sysPagerHandle, IntPtr.Zero, "ToolbarWindow32", "Notification Area");
            if (notificationAreaHandle == IntPtr.Zero)
            {
                notificationAreaHandle = SuperFramework.WindowsAPI.User32API.FindWindowEx(sysPagerHandle, IntPtr.Zero, "ToolbarWindow32", "User Promoted Notification Area");
                IntPtr notifyIconOverflowWindowHandle = SuperFramework.WindowsAPI.User32API.FindWindow("NotifyIconOverflowWindow", null);
                IntPtr overflowNotificationAreaHandle = SuperFramework.WindowsAPI.User32API.FindWindowEx(notifyIconOverflowWindowHandle, IntPtr.Zero, "ToolbarWindow32", "Overflow Notification Area");
                RefreshTaskbarNotificationArea(overflowNotificationAreaHandle);
            }
            RefreshTaskbarNotificationArea(notificationAreaHandle);
        }

        /// <summary>
        /// 刷新任务栏通知区域
        /// </summary>
        /// <param name="windowHandle"></param>
        private static void RefreshTaskbarNotificationArea(IntPtr windowHandle)
        {
            const uint wmMousemove = 0x0200;
            RECT rect;
            SuperFramework.WindowsAPI.User32API.GetClientRect(windowHandle, out rect);
            for (var x = 0; x < rect.right; x += 5)
                for (var y = 0; y < rect.bottom; y += 5)
                    SuperFramework.WindowsAPI.User32API.SendMessage(windowHandle, (int)wmMousemove, 0, (y << 16) + x);
        }
    }

}
