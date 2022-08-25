using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using SuperFramework.WindowsAPI;
using static SuperFramework.WindowsAPI.APIDelegate;

namespace SuperFramework.SuperHook
{

    /*
    注意：
        如果运行中出现SetWindowsHookEx的返回值为0，这是因为.net 调试模式的问题，具体的做法是禁用宿主进程，在 Visual Studio 中打开项目。
        在“项目”菜单上单击“属性”。
        单击“调试”选项卡。
        清除“启用 Visual Studio 宿主进程(启用windows承载进程)”复选框 或 勾选启用非托管代码调试
    */

    /// <summary>
    /// 
    /// </summary>
    public class MouseHook
    {
        /// <summary>
        /// 要安装的挂钩过程的类型
        /// </summary>
        public enum HookTypeConst
        {
            /// <summary>
            /// 在系统将消息发送到目标窗口过程之前，安装用于监视消息的挂钩过程。有关更多信息请参见CallWndRetProc挂接过程。
            /// </summary>
            WH_CALLWNDPROC = 4,
            /// <summary>
            /// 安装挂钩过程，以监视目标窗口过程处理完的消息。有关更多信息，请参见CallWndRetProc挂接过程。
            /// </summary>
            WH_CALLWNDPROCRET = 12,
            /// <summary>
            /// 安装一个挂钩程序，该程序接收对CBT应用程序有用的通知。有关更多信息，请参见CBTProc挂钩过程。
            /// </summary>
            WH_CBT = 5,
            /// <summary>
            /// 安装一个挂钩程序，对于调试其他挂钩程序很有用。有关更多信息，请参见DebugProc挂钩过程。
            /// </summary>
            WH_DEBUG = 9,
            /// <summary>
            /// 安装一个挂钩程序，当应用程序的前台线程即将变为空闲时将调用该挂钩程序。该挂钩对于在空闲时间执行低优先级任务很有用。有关更多信息，请参见ForegroundIdleProc挂钩过程。
            /// </summary>
            WH_FOREGROUNDIDLE = 11,
            /// <summary>
            /// 安装挂钩过程，以监视发布到消息队列的消息。有关更多信息，请参见GetMsgProc挂钩过程。
            /// </summary>
            WH_GETMESSAGE = 3,
            /// <summary>
            /// 安装一个挂钩程序，该程序发布以前由WH_JOURNALRECORD挂钩程序记录的消息。有关更多信息，请参见JournalPlaybackProc挂钩过程。
            /// </summary>
            WH_JOURNALPLAYBACK = 1,
            /// <summary>
            /// 安装挂钩过程，该过程记录输入到系统消息队列中的输入消息。该钩子对于记录宏很有用。有关更多信息，请参见JournalRecordProc挂钩过程。
            /// </summary>
            WH_JOURNALRECORD = 0,
            /// <summary>
            /// 安装钩子过程，以监视击键消息。有关更多信息，请参见KeyboardProc挂钩过程。
            /// </summary>
            WH_KEYBOARD = 2,
            /// <summary>
            /// 安装钩子程序，以监视低级键盘输入事件。有关更多信息，请参见LowLevelKeyboardProc挂钩过程。
            /// </summary>
            WH_KEYBOARD_LL = 13,
            /// <summary>
            /// 安装监视鼠标消息的挂钩过程。有关更多信息，请参见MouseProc挂钩过程。
            /// </summary>
            WH_MOUSE = 7,
            /// <summary>
            /// 安装钩子过程，以监视低级鼠标输入事件。有关更多信息，请参见LowLevelMouseProc挂钩过程。
            /// </summary>
            WH_MOUSE_LL = 14,
            /// <summary>
            /// 安装挂钩过程，以监视由于输入事件而在对话框，消息框，菜单或滚动条中生成的消息。有关更多信息，请参见MessageProc挂钩过程。
            /// </summary>
            WH_MSGFILTER = -1,
            /// <summary>
            /// 安装一个挂钩程序，该程序接收对外壳应用程序有用的通知。有关更多信息，请参见ShellProc挂钩过程。
            /// </summary>
            WH_SHELL = 10,
            /// <summary>
            /// 安装挂钩过程，以监视由于输入事件而在对话框，消息框，菜单或滚动条中生成的消息。挂钩过程会在与调用线程相同的桌面中监视所有应用程序的这些消息。有关更多信息，请参见SysMsgProc挂接过程。
            /// </summary>
            WH_SYSMSGFILTER = 6

        }
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct MouseHookStruct
        {
            public POINT pt;
            public int hwnd;
            public int wHitTestCode;
            public int dwExtraInfo;
        }


        private System.Drawing.Point point;
        private System.Drawing.Point Point
        {
            get { return point; }
            set
            {
                if (point != value)
                {
                    point = value;
                    if (MouseMoveEvent != null)
                    {
                        var e = new MouseEventArgs(MouseButtons.Left, 0, point.X, point.Y, 0);
                        MouseMoveEvent(this, e);
                    }
                }
            }
        }
        private int hHook;
        /// <summary>
        /// 
        /// </summary>
        public HookProc hProc;

        public MouseHook()
        {
            Point = new System.Drawing.Point();
        }
        /// <summary>
        /// 安装鼠标钩子 
        /// </summary>
        /// <param name="hInstance">DLL的句柄，其中包含由lpfn参数指向的挂钩过程。如果dwThreadId参数指定了由当前进程创建的线程，并且挂钩过程在与当前进程关联的代码中，则hMod参数必须设置为NULL。</param>
        /// <param name="threadId">挂钩过程将与之关联的线程的标识符。对于桌面应用程序，如果此参数为零，则挂钩过程与与调用线程在同一桌面上运行的所有现有线程相关联。</param>
        /// <returns></returns>
        public int SetMouseHook(IntPtr hInstance, int threadId)
        {
            hProc = new HookProc(MouseHookProc);
            hHook = User32API.SetWindowsHookEx((int)HookTypeConst.WH_MOUSE, hProc, hInstance, threadId);
            if (hHook == 0)
                UnMouseHook();
            return hHook;
        }
        /// <summary>
        /// 卸载鼠标钩子
        /// </summary>
        public void UnMouseHook()
        {

            User32API.UnhookWindowsHookEx(hHook);
        }
        private int MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            MouseHookStruct MyMouseHookStruct = (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct));
            if (nCode < 0)
            {
                return User32API.CallNextHookEx(hHook, nCode, wParam, lParam);
            }
            else
            {
                int clickCount;
                MouseButtons button;
                switch ((int)wParam)
                {
                    case NativeConst.WM_LBUTTONDOWN:
                        button = MouseButtons.Left;
                        clickCount = 1;
                        MouseDownEvent?.BeginInvoke(this, new MouseEventArgs(button, clickCount, point.X, point.Y, 0), null, null);
                        break;
                    case NativeConst.WM_RBUTTONDOWN:
                        button = MouseButtons.Right;
                        clickCount = 1;
                        MouseDownEvent?.BeginInvoke(this, new MouseEventArgs(button, clickCount, point.X, point.Y, 0), null, null);
                        break;
                    case NativeConst.WM_MBUTTONDOWN:
                        button = MouseButtons.Middle;
                        clickCount = 1;
                        MouseDownEvent?.BeginInvoke(this, new MouseEventArgs(button, clickCount, point.X, point.Y, 0), null, null);
                        break;
                    case NativeConst.WM_LBUTTONUP:
                        button = MouseButtons.Left;
                        clickCount = 1;
                        MouseUpEvent?.BeginInvoke(this, new MouseEventArgs(button, clickCount, point.X, point.Y, 0), null, null);
                        break;
                    case NativeConst.WM_RBUTTONUP:
                        button = MouseButtons.Right;
                        clickCount = 1;
                        MouseUpEvent?.BeginInvoke(this, new MouseEventArgs(button, clickCount, point.X, point.Y, 0), null, null);
                        break;
                    case NativeConst.WM_MBUTTONUP:
                        button = MouseButtons.Middle;
                        clickCount = 1;
                        MouseUpEvent?.BeginInvoke(this, new MouseEventArgs(button, clickCount, point.X, point.Y, 0), null, null);
                        break;
                }

                Point = new System.Drawing.Point(MyMouseHookStruct.pt.x, MyMouseHookStruct.pt.y);
                return User32API.CallNextHookEx(hHook, nCode, wParam, lParam);
            }
        }
        public delegate void MouseMoveHandler(object sender, MouseEventArgs e);
        public event MouseMoveHandler MouseMoveEvent;

        public delegate void MouseClickHandler(object sender, MouseEventArgs e);
        public event MouseClickHandler MouseClickEvent;

        public delegate void MouseDownHandler(object sender, MouseEventArgs e);
        public event MouseDownHandler MouseDownEvent;

        public delegate void MouseUpHandler(object sender, MouseEventArgs e);
        public event MouseUpHandler MouseUpEvent;
    }
}

