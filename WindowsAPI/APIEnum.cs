using System;

namespace SuperFramework.WindowsAPI
{
    
    /// <summary>
    /// 版 本:Release
    /// 日 期:2015-09-08
    /// 作 者:不良帥
    /// 描 述:API操作枚举
    /// </summary>
    public class APIEnum
    {
        public enum ShowWindow
        {
            /// <summary>
            /// 隐藏, 并且任务栏也没有最小化图标
            /// </summary>
            SW_HIDE = 0,
            /// <summary>
            /// 用最近的大小和位置显示, 激活
            /// </summary>
            SW_SHOWNORMAL = 1,
            /// <summary>
            /// 同 SW_SHOWNORMAL
            /// </summary>
            SW_NORMAL = 1,
            /// <summary>
            /// 最小化, 激活
            /// </summary>
            SW_SHOWMINIMIZED = 2,
            /// <summary>
            ///  最大化, 激活
            /// </summary>
            SW_SHOWMAXIMIZED = 3,
            /// <summary>
            ///  同 SW_SHOWMAXIMIZED
            /// </summary>
            SW_MAXIMIZE = 3,
            /// <summary>
            /// 用最近的大小和位置显示, 不激活
            /// </summary>
            SW_SHOWNOACTIVATE = 4,
            /// <summary>
            /// 同 SW_SHOWNORMAL
            /// </summary>
            SW_SHOW = 5,
            /// <summary>
            /// 最小化, 不激活
            /// </summary>
            SW_MINIMIZE = 6,
            /// <summary>
            ///  同 SW_MINIMIZE
            /// </summary>
            SW_SHOWMINNOACTIVE = 7,
            /// <summary>
            /// 同 SW_SHOWNOACTIVATE
            /// </summary>
            SW_SHOWNA = 8,
            /// <summary>
            /// 同 SW_SHOWNORMAL
            /// </summary>
            SW_RESTORE = 9,
            /// <summary>
            /// 同 SW_SHOWNORMAL
            /// </summary>
            SW_SHOWDEFAULT = 10,
            /// <summary>
            /// 同 SW_SHOWNORMAL
            /// </summary>
            SW_MAX = 10,
        }
        public enum VirtualKeyStates : int
        {
            VK_LBUTTON = 0x01,
            VK_RBUTTON = 0x02,
            VK_CANCEL = 0x03,
            VK_MBUTTON = 0x04,
            //
            VK_XBUTTON1 = 0x05,
            VK_XBUTTON2 = 0x06,
            //
            VK_BACK = 0x08,
            VK_TAB = 0x09,
            //
            VK_CLEAR = 0x0C,
            VK_RETURN = 0x0D,
            //
            VK_SHIFT = 0x10,
            VK_CONTROL = 0x11,
            VK_MENU = 0x12,
            VK_PAUSE = 0x13,
            VK_CAPITAL = 0x14,
            //
            VK_KANA = 0x15,
            VK_HANGEUL = 0x15,  /* old name - should be here for compatibility */
            VK_HANGUL = 0x15,
            VK_JUNJA = 0x17,
            VK_FINAL = 0x18,
            VK_HANJA = 0x19,
            VK_KANJI = 0x19,
            //
            VK_ESCAPE = 0x1B,
            //
            VK_CONVERT = 0x1C,
            VK_NONCONVERT = 0x1D,
            VK_ACCEPT = 0x1E,
            VK_MODECHANGE = 0x1F,
            //
            VK_SPACE = 0x20,
            VK_PRIOR = 0x21,
            VK_NEXT = 0x22,
            VK_END = 0x23,
            VK_HOME = 0x24,
            VK_LEFT = 0x25,
            VK_UP = 0x26,
            VK_RIGHT = 0x27,
            VK_DOWN = 0x28,
            VK_SELECT = 0x29,
            VK_PRINT = 0x2A,
            VK_EXECUTE = 0x2B,
            VK_SNAPSHOT = 0x2C,
            VK_INSERT = 0x2D,
            VK_DELETE = 0x2E,
            VK_HELP = 0x2F,
            //
            VK_LWIN = 0x5B,
            VK_RWIN = 0x5C,
            VK_APPS = 0x5D,
            //
            VK_SLEEP = 0x5F,
            //
            VK_NUMPAD0 = 0x60,
            VK_NUMPAD1 = 0x61,
            VK_NUMPAD2 = 0x62,
            VK_NUMPAD3 = 0x63,
            VK_NUMPAD4 = 0x64,
            VK_NUMPAD5 = 0x65,
            VK_NUMPAD6 = 0x66,
            VK_NUMPAD7 = 0x67,
            VK_NUMPAD8 = 0x68,
            VK_NUMPAD9 = 0x69,
            VK_MULTIPLY = 0x6A,
            VK_ADD = 0x6B,
            VK_SEPARATOR = 0x6C,
            VK_SUBTRACT = 0x6D,
            VK_DECIMAL = 0x6E,
            VK_DIVIDE = 0x6F,
            VK_F1 = 0x70,
            VK_F2 = 0x71,
            VK_F3 = 0x72,
            VK_F4 = 0x73,
            VK_F5 = 0x74,
            VK_F6 = 0x75,
            VK_F7 = 0x76,
            VK_F8 = 0x77,
            VK_F9 = 0x78,
            VK_F10 = 0x79,
            VK_F11 = 0x7A,
            VK_F12 = 0x7B,
            VK_F13 = 0x7C,
            VK_F14 = 0x7D,
            VK_F15 = 0x7E,
            VK_F16 = 0x7F,
            VK_F17 = 0x80,
            VK_F18 = 0x81,
            VK_F19 = 0x82,
            VK_F20 = 0x83,
            VK_F21 = 0x84,
            VK_F22 = 0x85,
            VK_F23 = 0x86,
            VK_F24 = 0x87,
            //
            VK_NUMLOCK = 0x90,
            VK_SCROLL = 0x91,
            //
            VK_OEM_NEC_EQUAL = 0x92,   // '=' key on numpad
                                       //
            VK_OEM_FJ_JISHO = 0x92,   // 'Dictionary' key
            VK_OEM_FJ_MASSHOU = 0x93,   // 'Unregister word' key
            VK_OEM_FJ_TOUROKU = 0x94,   // 'Register word' key
            VK_OEM_FJ_LOYA = 0x95,   // 'Left OYAYUBI' key
            VK_OEM_FJ_ROYA = 0x96,   // 'Right OYAYUBI' key
                                     //
            VK_LSHIFT = 0xA0,
            VK_RSHIFT = 0xA1,
            VK_LCONTROL = 0xA2,
            VK_RCONTROL = 0xA3,
            VK_LMENU = 0xA4,
            VK_RMENU = 0xA5,
            //
            VK_BROWSER_BACK = 0xA6,
            VK_BROWSER_FORWARD = 0xA7,
            VK_BROWSER_REFRESH = 0xA8,
            VK_BROWSER_STOP = 0xA9,
            VK_BROWSER_SEARCH = 0xAA,
            VK_BROWSER_FAVORITES = 0xAB,
            VK_BROWSER_HOME = 0xAC,
            //
            VK_VOLUME_MUTE = 0xAD,
            VK_VOLUME_DOWN = 0xAE,
            VK_VOLUME_UP = 0xAF,
            VK_MEDIA_NEXT_TRACK = 0xB0,
            VK_MEDIA_PREV_TRACK = 0xB1,
            VK_MEDIA_STOP = 0xB2,
            VK_MEDIA_PLAY_PAUSE = 0xB3,
            VK_LAUNCH_MAIL = 0xB4,
            VK_LAUNCH_MEDIA_SELECT = 0xB5,
            VK_LAUNCH_APP1 = 0xB6,
            VK_LAUNCH_APP2 = 0xB7,
            //
            VK_OEM_1 = 0xBA,   // ';:' for US
            VK_OEM_PLUS = 0xBB,   // '+' any country
            VK_OEM_COMMA = 0xBC,   // ',' any country
            VK_OEM_MINUS = 0xBD,   // '-' any country
            VK_OEM_PERIOD = 0xBE,   // '.' any country
            VK_OEM_2 = 0xBF,   // '/?' for US
            VK_OEM_3 = 0xC0,   // '`~' for US
                               //
            VK_OEM_4 = 0xDB,  //  '[{' for US
            VK_OEM_5 = 0xDC,  //  '\|' for US
            VK_OEM_6 = 0xDD,  //  ']}' for US
            VK_OEM_7 = 0xDE,  //  ''"' for US
            VK_OEM_8 = 0xDF,
            //
            VK_OEM_AX = 0xE1,  //  'AX' key on Japanese AX kbd
            VK_OEM_102 = 0xE2,  //  "<>" or "\|" on RT 102-key kbd.
            VK_ICO_HELP = 0xE3,  //  Help key on ICO
            VK_ICO_00 = 0xE4,  //  00 key on ICO
                               //
            VK_PROCESSKEY = 0xE5,
            //
            VK_ICO_CLEAR = 0xE6,
            //
            VK_PACKET = 0xE7,
            //
            VK_OEM_RESET = 0xE9,
            VK_OEM_JUMP = 0xEA,
            VK_OEM_PA1 = 0xEB,
            VK_OEM_PA2 = 0xEC,
            VK_OEM_PA3 = 0xED,
            VK_OEM_WSCTRL = 0xEE,
            VK_OEM_CUSEL = 0xEF,
            VK_OEM_ATTN = 0xF0,
            VK_OEM_FINISH = 0xF1,
            VK_OEM_COPY = 0xF2,
            VK_OEM_AUTO = 0xF3,
            VK_OEM_ENLW = 0xF4,
            VK_OEM_BACKTAB = 0xF5,
            //
            VK_ATTN = 0xF6,
            VK_CRSEL = 0xF7,
            VK_EXSEL = 0xF8,
            VK_EREOF = 0xF9,
            VK_PLAY = 0xFA,
            VK_ZOOM = 0xFB,
            VK_NONAME = 0xFC,
            VK_PA1 = 0xFD,
            VK_OEM_CLEAR = 0xFE
        }
        [Flags]
        public enum MouseClickFlag : uint
        {
            /// <summary>
            /// 鼠标左键按下
            /// </summary>
            WM_LBUTTONDOWN = 513,
            /// <summary>
            /// 鼠标左键抬起
            /// </summary>
            WM_LBUTTONUP = 514,
            /// <summary>
            /// 鼠标左键双击
            /// </summary>
            WM_LBUTTONDBLCLK = 515,
            /// <summary>
            /// 鼠标右键按下
            /// </summary>
            WM_RBUTTONDOWN = 516,
            /// <summary>
            /// 鼠标右键双击
            /// </summary>
            WM_RBUTTONDBLCLK = 518,
            /// <summary>
            /// 鼠标右键抬起
            /// </summary>
            WM_RBUTTONUP = 517,
            /// <summary>
            /// 鼠标中键按下
            /// </summary>
            WM_MBUTTONDOWN = 519,
            /// <summary>
            /// 鼠标中键抬起
            /// </summary>
            WM_MBUTTONUP = 520,
            /// <summary>
            /// 鼠标移动
            /// </summary>
            WM_MOUSEMOVE = 512


            //MOUSEEVENTF_MOVE = 0x0001, // 移动鼠标         
            //MOUSEEVENTF_LEFTDOWN = 0x0002, // 鼠标左键按下        
            //MOUSEEVENTF_LEFTUP = 0x0004, // 鼠标左键抬起        
            //MOUSEEVENTF_RIGHTDOWN = 0x0008; // 鼠标右键按下       
            // MOUSEEVENTF_RIGHTUP = 0x0010; // 鼠标右键抬起          
            //MOUSEEVENTF_MIDDLEDOWN = 0x0020; // 鼠标中键按下    
            //MOUSEEVENTF_MIDDLEUP = 0x0040; // 鼠标中键抬起           
            //MOUSEEVENTF_ABSOLUTE = 0x8000; // 绝对坐标   
        }
        [Flags]
        public enum SetWindowPosFlags : uint
        {
            // ReSharper disable InconsistentNaming

            /// <summary>
            ///     If the calling thread and the thread that owns the window are attached to different input queues, the system posts the request to the thread that owns the window. This prevents the calling thread from blocking its execution while other threads process the request.
            /// </summary>
            SWP_ASYNCWINDOWPOS = 0x4000,

            /// <summary>
            ///     Prevents generation of the WM_SYNCPAINT message.
            /// </summary>
            SWP_DEFERERASE = 0x2000,

            /// <summary>
            ///     Draws a frame (defined in the window's class description) around the window.
            /// </summary>
            SWP_DRAWFRAME = 0x0020,

            /// <summary>
            ///     Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE is sent only when the window's size is being changed.
            /// </summary>
            SWP_FRAMECHANGED = 0x0020,

            /// <summary>
            ///     Hides the window.
            /// </summary>
            SWP_HIDEWINDOW = 0x0080,

            /// <summary>
            ///     Does not activate the window. If this flag is not set, the window is activated and moved to the top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter parameter).
            /// </summary>
            SWP_NOACTIVATE = 0x0010,

            /// <summary>
            ///     Discards the entire contents of the client area. If this flag is not specified, the valid contents of the client area are saved and copied back into the client area after the window is sized or repositioned.
            /// </summary>
            SWP_NOCOPYBITS = 0x0100,

            /// <summary>
            ///     Retains the current position (ignores X and Y parameters).
            /// </summary>
            SWP_NOMOVE = 0x0002,

            /// <summary>
            ///     Does not change the owner window's position in the Z order.
            /// </summary>
            SWP_NOOWNERZORDER = 0x0200,

            /// <summary>
            ///     Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent window uncovered as a result of the window being moved. When this flag is set, the application must explicitly invalidate or redraw any parts of the window and parent window that need redrawing.
            /// </summary>
            SWP_NOREDRAW = 0x0008,

            /// <summary>
            ///     Same as the SWP_NOOWNERZORDER flag.
            /// </summary>
            SWP_NOREPOSITION = 0x0200,

            /// <summary>
            ///     Prevents the window from receiving the WM_WINDOWPOSCHANGING message.
            /// </summary>
            SWP_NOSENDCHANGING = 0x0400,

            /// <summary>
            ///     Retains the current size (ignores the cx and cy parameters).
            /// </summary>
            SWP_NOSIZE = 0x0001,

            /// <summary>
            ///     Retains the current Z order (ignores the hWndInsertAfter parameter).
            /// </summary>
            SWP_NOZORDER = 0x0004,

            /// <summary>
            ///     Displays the window.
            /// </summary>
            SWP_SHOWWINDOW = 0x0040,

            // ReSharper restore InconsistentNaming
        }
        /// <summary>
        /// 设置鼠标动作的键值
        /// </summary>
        [Flags]
        public enum MouseEventFlag : uint
        {
            /// <summary>
            /// 发生移动
            /// </summary>
            Move = 0x0001,
            /// <summary>
            /// 鼠标按下左键
            /// </summary>
            LeftDown = 0x0002,
            /// <summary>
            /// 鼠标松开左键
            /// </summary>
            LeftUp = 0x0004,
            /// <summary>
            /// 鼠标按下右键
            /// </summary>
            RightDown = 0x0008,
            /// <summary>
            /// 鼠标松开右键
            /// </summary>
            RightUp = 0x0010,
            /// <summary>
            /// 鼠标按下中键
            /// </summary>
            MiddleDown = 0x0020,
            /// <summary>
            /// 鼠标松开中键
            /// </summary>
            MiddleUp = 0x0040,
            /// <summary>
            /// 中键按下
            /// </summary>
            XDown = 0x0080,
            /// <summary>
            /// 中键弹起
            /// </summary>
            XUp = 0x0100,
            /// <summary>
            /// 鼠标轮被移动
            /// </summary>
            Wheel = 0x0800,
            /// <summary>
            /// 绝对坐标
            /// </summary>
            Absolute = 0x8000
        }
        public static int MAKEPARAM(int l, int h)
        {
            return (l & 0xffff) | (h << 0x10);
        }

        public enum WindowStyles : uint
        {
            WS_OVERLAPPED = 0x00000000,
            WS_POPUP = 0x80000000,
            WS_CHILD = 0x40000000,
            WS_MINIMIZE = 0x20000000,
            WS_VISIBLE = 0x10000000,
            WS_DISABLED = 0x08000000,
            WS_CLIPSIBLINGS = 0x04000000,
            WS_CLIPCHILDREN = 0x02000000,
            WS_MAXIMIZE = 0x01000000,
            WS_BORDER = 0x00800000,
            WS_DLGFRAME = 0x00400000,
            WS_VSCROLL = 0x00200000,
            WS_HSCROLL = 0x00100000,
            WS_SYSMENU = 0x00080000,
            WS_THICKFRAME = 0x00040000,
            WS_GROUP = 0x00020000,
            WS_TABSTOP = 0x00010000,

            WS_MINIMIZEBOX = 0x00020000,
            WS_MAXIMIZEBOX = 0x00010000,

            WS_CAPTION = WS_BORDER | WS_DLGFRAME,
            WS_TILED = WS_OVERLAPPED,
            WS_ICONIC = WS_MINIMIZE,
            WS_SIZEBOX = WS_THICKFRAME,
            WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW,

            WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
            WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,
            WS_CHILDWINDOW = WS_CHILD,

            //Extended Window Styles

            WS_EX_DLGMODALFRAME = 0x00000001,
            WS_EX_NOPARENTNOTIFY = 0x00000004,
            WS_EX_TOPMOST = 0x00000008,
            WS_EX_ACCEPTFILES = 0x00000010,
            WS_EX_TRANSPARENT = 0x00000020,

            //#if(WINVER >= 0x0400)

            WS_EX_MDICHILD = 0x00000040,
            WS_EX_TOOLWINDOW = 0x00000080,
            WS_EX_WINDOWEDGE = 0x00000100,
            WS_EX_CLIENTEDGE = 0x00000200,
            WS_EX_CONTEXTHELP = 0x00000400,

            WS_EX_RIGHT = 0x00001000,
            WS_EX_LEFT = 0x00000000,
            WS_EX_RTLREADING = 0x00002000,
            WS_EX_LTRREADING = 0x00000000,
            WS_EX_LEFTSCROLLBAR = 0x00004000,
            WS_EX_RIGHTSCROLLBAR = 0x00000000,

            WS_EX_CONTROLPARENT = 0x00010000,
            WS_EX_STATICEDGE = 0x00020000,
            WS_EX_APPWINDOW = 0x00040000,

            WS_EX_OVERLAPPEDWINDOW = WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE,
            WS_EX_PALETTEWINDOW = WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST,

            //#endif /* WINVER >= 0x0400 */

            //#if(WIN32WINNT >= 0x0500)

            WS_EX_LAYERED = 0x00080000,

            //#endif /* WIN32WINNT >= 0x0500 */

            //#if(WINVER >= 0x0500)

            WS_EX_NOINHERITLAYOUT = 0x00100000, // Disable inheritence of mirroring by children
            WS_EX_LAYOUTRTL = 0x00400000, // Right to left mirroring

            //#endif /* WINVER >= 0x0500 */

            //#if(WIN32WINNT >= 0x0500)

            WS_EX_COMPOSITED = 0x02000000,
            WS_EX_NOACTIVATE = 0x08000000

            //#endif /* WIN32WINNT >= 0x0500 */

        }


        #region 设置指定窗口的显示状态
        /// <summary>
        /// 设置指定窗口的显示状态
        /// </summary>
        public enum WindowShow
        {
            /// <summary>
            /// 在WindowNT5.0中最小化窗口，即使拥有窗口的线程被挂起也会最小化。在从其他线程最小化窗口时才使用这个参数。
            /// </summary>
            SW_FORCEMINIMIZE,
            /// <summary>
            /// 隐藏窗口并激活其他窗口
            /// </summary>
            SW_HIDE,
            /// <summary>
            /// 最大化指定的窗口
            /// </summary>
            SW_MAXIMIZE,
            /// <summary>
            /// 最小化指定的窗口并且激活在Z序中的下一个顶层窗口
            /// </summary>
            SW_MINIMIZE,
            /// <summary>
            /// 最小化指定的窗口并且激活在Z序中的下一个顶层窗口
            /// </summary>
            SW_RESTORE,
            /// <summary>
            /// 在窗口原来的位置以原来的尺寸激活和显示窗口
            /// </summary>
            SW_SHOW,
            /// <summary>
            /// 依据在STARTUPINFO结构中指定的SW_FLAG标志设定显示状态，STARTUPINFO 结构是由启动应用程序的程序传递给CreateProcess函数的
            /// </summary>
            SW_SHOWDEFAULT,
            /// <summary>
            /// 激活窗口并将其最大化
            /// </summary>
            SW_SHOWMAXIMIZED,
            /// <summary>
            /// 激活窗口并将其最小化
            /// </summary>
            SW_SHOWMINIMIZED,
            /// <summary>
            /// 窗口最小化，激活窗口仍然维持激活状态。
            /// </summary>
            SW_SHOWMINNOACTIVE,
            /// <summary>
            /// 以窗口原来的状态显示窗口。激活窗口仍然维持激活状态
            /// </summary>
            SW_SHOWNA,
            /// <summary>
            /// 以窗口最近一次的大小和状态显示窗口。激活窗口仍然维持激活状态
            /// </summary>
            SW_SHOWNOACTIVATE,
            /// <summary>
            /// 激活并显示一个窗口。如果窗口被最小化或最大化，系统将其恢复到原来的尺寸和大小。应用程序在第一次显示窗口的时候应该指定此标志。
            /// </summary>
            SW_SHOWNORMAL
        }
        #endregion

        #region 标识了在Z轴次序上位于这个句柄对象之前的句柄对象
        /// <summary>
        /// 标识了在Z轴次序上位于这个句柄对象之前的句柄对象
        /// </summary>
        public enum InsertAfter
        {
            /// <summary>
            ///  将窗口放在Z轴次序的底部。如果这个CWnd是一个顶层窗口，则窗口将失去它的顶层状态；系统将这个窗口放在其它所有窗口的底部。
            /// </summary>
            wndBottom,
            /// <summary>
            /// 将窗口放在Z轴次序的顶部
            /// </summary>
            wndTop,
            /// <summary>
            ///  将窗口放在所有非顶层窗口的上面。这个窗口将保持它的顶层位置，即使它失去了活动状态。
            /// </summary>
            wndTopMost,
            /// <summary>
            /// 将窗口重新定位到所有非顶层窗口的顶部（这意味着在所有的顶层窗口之下）。这个标志对那些已经是非顶层窗口的窗口没有作用。有关这个函数以及这些参数的使用规则参见说明部分。
            /// </summary>
            wndNoTopMost
        }
        #endregion

        #region 改变一个子窗口，弹出式窗口式顶层窗口的尺寸，位置和Z序。

        /// <summary>
        /// 改变一个子窗口，弹出式窗口式顶层窗口的尺寸，位置和Z序。
        /// </summary>
        public enum WindowSiezeLocation
        {
            /// <summary>
            /// 把窗口放在Z轴的最后，即所有窗口的后面。
            /// </summary>
            HWND_BOTTOM = 1,
            /// <summary>
            ///  将窗口放在Z轴的前面，即所有窗口的前面。
            /// </summary>
            HWND_TOP = 0,
            /// <summary>
            /// 使窗口成为“TopMost”类型的窗口，这种类型的窗口总是在其它窗口的前面，直到它被关闭。
            /// </summary>
            HWND_TOPMOST = -1,
            /// <summary>
            ///  将窗口放在所有“TopMost”类型
            /// </summary>
            HWND_NOTOPMOST = -2,
            /// <summary>
            /// 保持当前的大小（忽略cx和cy参数）
            /// </summary>
            SWP_NOSIZE = 0x0001,
            /// <summary>
            /// 保持当前的位置（忽略x和y参数）
            /// </summary>
            SWP_NOMOVE = 0x0002,
            /// <summary>
            /// 保持当前的次序（忽略pWndInsertAfter）
            /// </summary>
            SWP_NOZORDER = 0x0004,
            /// <summary>
            /// 不重画变化。如果设置了这个标志，则不发生任何种类的变化。这适用于客户区、非客户区（包括标题和滚动条）以及被移动窗口覆盖的父窗口的任何部分。当这个标志被设置的时候，应用程序必须明确地无效或重画要重画的窗口和父窗口的任何部分。
            /// </summary>
            SWP_NOREDRAW = 0x0008,
            /// <summary>
            ///  不激活窗口。如果没有设置这个标志，则窗口将被激活并移动到顶层或非顶层窗口组（依赖于pWndInsertAfter参数的设置）的顶部。
            /// </summary>
            SWP_NOACTIVATE = 0x0010,
            /// <summary>
            ///  向窗口发送一条WM_NCCALCSIZE消息，即使窗口的大小不会改变。如果没有指定这个标志，则仅当窗口的大小发生变化时才发送WM_NCCALCSIZE消息。
            /// </summary>
            SWP_FRAMECHANGED = 0x0020,
            /// <summary>
            /// 显示窗口（之前必须使用过SWP_HIDEWINDOW 隐藏窗口）
            /// </summary>
            SWP_SHOWWINDOW = 0x0040,
            /// <summary>
            /// 隐藏窗口
            /// </summary>
            SWP_HIDEWINDOW = 0x0080,
            /// <summary>
            /// 废弃这个客户区的内容。如果没有指定这个参数，则客户区的有效内容将被保存，并在窗口的大小或位置改变以后被拷贝回客户区。
            /// </summary>
            SWP_NOCOPYBITS = 0x0100,
            /// <summary>
            /// 不改变拥有者窗口在Z轴次序上的位置
            /// </summary>
            SWP_NOOWNERZORDER = 0x0200,
            /// <summary>
            /// 防止窗口接收WM_WINDOWPOSCHANGING消息
            /// </summary>
            SWP_NOSENDCHANGING = 0x0400
        }
        #endregion

        /// <summary>
        /// 按键类型
        /// </summary>
        public enum KeyType
        {
            /// <summary>
            /// 键盘消息按下(描述键盘虚拟键码的，它对应的是键盘物理按键。)如果键盘键入的是“Ctrl+D”，则应该选择WM_KEYDOWN，因为WM_KEYDOWN既包含字母也包含特殊字符。
            /// </summary>
            WM_KEYDOWN = 0X100,
            /// <summary>
            /// 键盘消息弹起(描述键盘虚拟键码的，它对应的是键盘物理按键。)
            /// </summary>
            WM_KEYUP = 0X101,
            /// <summary>
            /// 系统按键键盘消息，字符串码
            /// </summary>
            WM_SYSCHAR = 0X106,
            /// <summary>
            /// 系统按键弹起。接受快捷键或系统命令按键的，由与Alt相组合的按键产生，Ctrl和shift不属于WM_SYSKEYUP。
            /// 这些按键启动程序菜单或者系统菜单上的选项，或者用于切换活动窗口等系统功能（Alt-Tab或者Alt-Esc），也可以用作加速键（Alt键与一个功能键相结合，例如Alt-F4用于关闭应用程序)。(描述键盘虚拟键码的，它对应的是键盘物理按键。)
            /// </summary>
            WM_SYSKEYUP = 0X105,
            /// <summary>
            /// 系统按键弹起。接受快捷键或系统命令按键的，由与Alt相组合的按键产生，Ctrl和shift不属于WM_SYSKEYDOWN。
            /// 这些按键启动程序菜单或者系统菜单上的选项，或者用于切换活动窗口等系统功能（Alt-Tab或者Alt-Esc），也可以用作加速键（Alt键与一个功能键相结合，例如Alt-F4用于关闭应用程序)。(描述键盘虚拟键码的，它对应的是键盘物理按键。)
            /// </summary>
            WM_SYSKEYDOWN = 0X104,
            /// <summary>
            /// 键盘消息，字符串码，对应的键盘操作所按下的字符。如：键入“D”键，就应该选择WM_CHAR，因为WM_CHAR 只是字母，不包含特殊字符如Ctrl等。
            /// 是由WM_KEYDOWN消息Translate()之后产生的，然后再发送给窗口过程。例如按下“D”键，产生WM_KEYDOWN消息，此消息经过Translate()处理后变成了WM_KEYDOW、WM_CHAR两个消息传递给窗口过程。
            /// (只响应字符和部分字符相关的控制符（如空格、回车），其它控制键码则不响应，如： Tab、Caps Lock、ESC、F1~F12、SHIFT、CTRL、ALT、方向键、方向键上方键盘区和Num Lock键等。)
            /// </summary>
            WM_CHAR = 0X102
        }
    }
}
