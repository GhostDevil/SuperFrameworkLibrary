using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Drawing;
using static SuperFramework.WindowsAPI.APIStruct;
using static SuperFramework.WindowsAPI.APIDelegate;
using static SuperFramework.WindowsAPI.APIEnum;

namespace SuperFramework.WindowsAPI
{
    /// <summary>
    /// <para>日 期:2015-09-08</para>
    /// <para>作 者:不良帥</para>
    /// <para>描 述:Windows用户界面相关应用程序接口，用于包括Windows处理，基本用户界面等特性，如创建窗口和发送消息。</para>
    /// </summary>
    public static class User32API
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr SetActiveWindow(IntPtr hwnd);

        #region 清除图标
        /// <summary>
        /// 清除图标
        /// </summary>
        /// <param name="hIcon">图标句柄</param>
        /// <returns>非零表示成功，零表示失败</returns>
        [DllImport("User32.dll", EntryPoint = "DestroyIcon")]
        public static extern int DestroyIcon(IntPtr hIcon);
        #endregion

        #region 系统控制


        /// <summary>
        /// ExitWindowEx函数要么注销当前用户,关闭系统, 要么关闭系统然后重新启动. 它发送 WM_QUERYENDSESSION 给所有的应用程序，决定是否可以停止它们的操作.
        /// </summary>
        /// <param name="uFlags">指定关闭的类型.</param>
        /// <param name="dwReserved">该参数忽略.</param>
        /// <returns>如果执行成功，返回非0.,如果执行失败，返回0. 如果要获取更多的错误信息, 请调用 Marshal.GetLastWin32Error.,</returns>
        [DllImport("user32.dll", EntryPoint = "ExitWindowsEx", CharSet = CharSet.Ansi)]
        public static extern int ExitWindowsEx(int uFlags, int dwReserved);

        /// <summary>
        /// 关机类型枚举
        /// </summary>
        public enum ExitWindowsExEnum
        {
            /// <summary>
            /// 注销
            /// </summary>
            EWX_LOGOFF = 0x00000000,
            /// <summary>
            /// 关机
            /// </summary>
            EWX_SHUTDOWN = 0x00000001,
            /// <summary>
            /// 重启
            /// </summary>
            EWX_REBOOT = 0x00000002,
            /// <summary>
            /// 强制关机
            /// </summary>
            EWX_FORCE = 0x00000004, 
            /// <summary>
            /// 关闭电源
            /// </summary>
            EWX_POWEROFF = 0x00000008,
        }
        /// <summary>
        /// 函数用来退出、重启或注销系统。
        /// </summary>
        /// <param name="flg">EWX_FORCE 强制关机 EWX_LOGOFF注销 EWX_POWEROFF关闭电源 EWX_REBOOT 重启 EWX_SHUTDOWN关机</param>
        /// <param name="rea"></param>
        /// <returns></returns>
        [DllImport("user32.dll ", ExactSpelling = true, SetLastError = true)]
        public static extern bool ExitWindowsEx(ExitWindowsExEnum flg, int rea);

        #endregion

        #region 格式化消息字符串
        /// <summary>
        /// FormatMessage格式化消息字符串. 该函数需要一个已定义的消息参数作为输入. 消息的定义从一个缓冲区传入函数，它也可以从一个已载入模块的消息表资源中获取. 
        /// 调用者也可以要求函数搜索系统的消息表来查找消息定义. 函数根据消息ID号和语言ID号从消息表资源中查找消息定义.
        /// 函数最终将格式化的消息文本拷贝到输出缓冲区, 要求处理任何内嵌的顺序.
        /// </summary>
        /// <param name="dwFlags">指定格式化处理和如何翻译 lpSource 参数. dwFlags的低字节指定函数如何处理输出缓冲区的换行. 
        /// 低字节也可以指定格式化后的输出缓冲区的最大宽度.</param>
        /// <param name="lpSource">指定消息定义的位置. 此参数的类型依据 dwFlags 参数的设定.</param>
        /// <param name="dwMessageId">指定消息的消息标志ID. 如果 dwFlags 参数包含 FORMAT_MESSAGE_FROM_STRING ，则该参数被忽略.</param>
        /// <param name="dwLanguageId">指定消息的语言ID. 如果 dwFlags 参数包含 FORMAT_MESSAGE_FROM_STRING，则该参数被忽略.</param>
        /// <param name="lpBuffer">用来放置格式化消息(以null结束)的缓冲区.如果 dwFlags 参数包括 FORMAT_MESSAGE_ALLOCATE_BUFFER, 
        /// 本函数将使用LocalAlloc函数定位一个缓冲区，然后将缓冲区指针放到 lpBuffer 指向的地址.</param>
        /// <param name="nSize">如果没有设置 FORMAT_MESSAGE_ALLOCATE_BUFFER 标志, 此参数指定了输出缓冲区可以容纳的TCHARs最大个数. 
        /// 如果设置了 FORMAT_MESSAGE_ALLOCATE_BUFFER 标志，则此参数指定了输出缓冲区可以容纳的TCHARs 的最小个数. 对于ANSI文本, 容量为bytes的个数; 
        /// 对于Unicode 文本, 容量为字符的个数.</param>
        /// <param name="Arguments">数组指针,用于在格式化消息中插入信息. 格式字符串中的 A %1 指示参数数组中的第一值; a %2 表示第二个值; 以此类推.</param>
        /// <returns>如果执行成功, 返回值为存储在输出缓冲区的TCHARs个数, 包括了null结束符.
        /// 如果执行失败, 返回值为0. 如果要获取更多的错误信息, 请调用 Marshal.GetLastWin32Error.,</returns>
        [DllImport("user32.dll", EntryPoint = "FormatMessageA", CharSet = CharSet.Ansi)]
        public static extern int FormatMessage(int dwFlags, IntPtr lpSource, int dwMessageId, int dwLanguageId, StringBuilder lpBuffer, int nSize, int Arguments);
        #endregion

        #region 隐藏系统滚动条
        /// <summary>
        /// 隐藏系统滚动条
        /// </summary>
        /// <param name="hWnd">this.Handle</param>
        /// <param name="wBar">0:horizontal,1:vertical,3:both</param>
        /// <param name="bShow">是否显示</param>
        /// <returns>成功返回true，失败返回false</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);
        #endregion

        #region 得到光标在屏幕上的位置
        /// <summary>
        /// 得到光标在屏幕上的位置
        /// </summary>
        /// <param name="lpPoint">坐标位置</param>
        /// <returns>true：成功，false：失败</returns>
        [DllImport("user32")]
        public static extern bool GetCaretPos(out Point lpPoint);
        #endregion

        #region 窗口句柄
        /// <summary>
        /// 获取窗口句柄
        /// </summary>
        /// <returns>窗口句柄</returns>
        [DllImport("user32.dll", EntryPoint = "GetForegroundWindow", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();
        #endregion

        #region  锁定系统 
        /// <summary>
        /// 锁定系统
        /// </summary>
        /// <returns></returns>
        [DllImport("user32 ")]
        public static extern bool LockWorkStation();//这个是调用windows的系统锁定
        #endregion

        #region  开启、锁定键盘鼠标 
        /// <summary>
        /// 开启、锁定键盘鼠标
        /// </summary>
        /// <param name="Block">true为锁定,false为开启</param>
        [DllImport("user32.dll")]
        public static extern void BlockInput(bool Block);
        #endregion

        #region 检查键盘大小写锁定状态
        /// <summary>
        /// 检查键盘大小写锁定状态
        /// </summary>
        /// <param name="pbKeyState">byte数组，长度256.</param>
        /// <returns>返回true为大写锁定状态，false为小写锁定状态</returns>
        [DllImport("user32.dll", EntryPoint = "GetKeyboardState")]
        public static extern int GetKeyboardState(byte[] pbKeyState);

        [DllImport("user32.dll")]
        public static extern short GetKeyStates(int vKey);
        #endregion

        #region 窗体动画

        /// <summary>
        /// 该函数能在显示与隐藏窗口时能产生特殊的效果。有两种类型的动画效果：滚动动画和滑动动画。 
        /// </summary>
        /// <param name="hwnd">指定产生动画的窗口的句柄。 </param>
        /// <param name="dwTime">指明动画持续的时间（以微秒计），完成一个动画的标准时间为200微秒。 </param>
        /// <param name="dwFlags">指定动画类型。可用加号连接多个</param>
        /// <returns>返回true成功，失败返回false</returns>
        [DllImport("user32")]

        public static extern bool AnimateWindow(IntPtr hwnd, int dwTime, int dwFlags);
        #endregion

        #region 窗口闪动
        /// <summary>
        /// 窗口闪动
        /// </summary>
        /// <param name="hWnd">窗体句柄</param>
        /// <param name="bInvert">是否闪动</param>
        /// <returns>true：成功，false：失败</returns>
        [DllImport("User32")]
        public static extern bool FlashWindow(IntPtr hWnd, bool bInvert);
        #endregion

        #region 从当前线程中的窗口释放鼠标捕获，并恢复通常的鼠标输入处理。
        /// <summary>
        /// 从当前线程中的窗口释放鼠标捕获，并恢复通常的鼠标输入处理。
        /// </summary>
        /// <returns>true：成功，false：失败</returns>

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        #endregion

        #region 光标
        /// <summary>
        /// 隐藏光标
        /// </summary>
        /// <param name="hWnd">控件句柄</param>
        /// <returns>true：成功，false：失败</returns>
        [DllImport("user32.dll", EntryPoint = "HideCaret")]
        public static extern bool HideCaret(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern bool ShowCaret(IntPtr hWnd);
        /// <summary>
        /// 是否显示鼠标
        /// </summary>
        /// <param name="show"></param>
        /// <returns></returns>
        [DllImport("user32", EntryPoint = "ShowCursor")]
        public extern static bool ShowCursor(bool show);

        #endregion


        /// <summary>
        /// 注册热键
        /// 如果函数执行成功，返回值不为0。
        /// 如果函数执行失败，返回值为0。要得到扩展错误信息，调用GetLastError。.NET方法:Marshal.GetLastWin32Error()
        /// </summary>
        /// <param name="hWnd">要定义热键的窗口的句柄</param>
        /// <param name="id">定义热键ID（不能与其它ID重复）  </param>
        /// <param name="fsModifiers">标识热键是否在按Alt、Ctrl、Shift、Windows等键时才会生效</param>
        /// <param name="vk">定义热键的内容,WinForm中可以使用Keys枚举转换，
        /// WPF中Key枚举是不正确的,应该使用System.Windows.Forms.Keys枚举，或者自定义正确的枚举或int常量</param>
        /// <returns>true:成功，false:失败</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, KeyModifiers fsModifiers, int vk);

        /// <summary>
        /// 取消热键
        /// </summary>
        /// <param name="hWnd">要取消热键的窗口的句柄</param>
        /// <param name="id">要取消热键的ID</param>
        /// <returns>true:成功，false:失败</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        /// <summary>
        /// 该函数检索一指定窗口的客户区域或整个屏幕的显示设备上下文环境的句柄，以后可以在GDI函数中使用该句柄来在设备上下文环境中绘图。
        /// </summary>
        /// <param name="hWnd">设备上下文环境被检索的窗口的句柄</param>
        /// <returns>如果成功，返回指定窗口客户区的设备上下文环境句柄；如果失败，返回值为Null。</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetDC(IntPtr hWnd);
        /// <summary>
        /// 函数释放设备上下文环境（DC）供其他应用程序使用。函数的效果与设备上下文环境类型有关。它只释放公用的和设备上下文环境，对于类或私有的则无效。
        /// </summary>
        /// <param name="hWnd">指向要释放的设备上下文环境所在的窗口的句柄</param>
        /// <param name="hDC">指向要释放的设备上下文环境的句柄</param>
        /// <returns>返回值说明了设备上下文环境是否释放；如果释放成功，则返回值为1；如果没有释放成功，则返回值为0。</returns>
        /// <remarks>
        /// 每次调用GetWindowDC和GetDC函数检索公用设备上下文环境之后，应用程序必须调用ReleaseDC函数来释放设备上下文环境。
        /// 应用程序不能调用ReleaseDC函数来释放由CreateDC函数创建的设备上下文环境，只能使用DeleteDC函数。
        /// </remarks>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
        /// <summary>
        /// 该函数返回桌面窗口的句柄。桌面窗口覆盖整个屏幕。桌面窗口是一个要在其上绘制所有的图标和其他窗口的区域。
        /// </summary>
        /// <returns>函数返回桌面窗口的句柄</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetDesktopWindow();
        /// <summary>
        /// 该函数设置指定窗口的显示状态。
        /// </summary>
        /// <param name="hWnd">指窗口句柄</param>
        /// <param name="nCmdShow">指定窗口如何显示。 1 表示显示， 0 表示隐藏。
        /// 如果发送应用程序的程序提供了STARTUPINFO结构，则应用程序第一次调用ShowWindow时该参数被忽略。否则，在第一次调用ShowWindow函数时，该值应为在函数WinMain中nCmdShow参数。在随后的调用中，该参数可以为WindowShow枚举值之一。</param>
        /// <returns>true:成功，false:失败</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        ///<summary>
        /// 该函数设置由不同线程产生的窗口的显示状态
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="cmdShow">指定窗口如何显示。查看允许值列表，请查阅ShowWlndow函数的说明部分</param>
        /// <returns>如果函数原来可见，返回值为非零；如果函数原来被隐藏，返回值为零</returns>
        [DllImport("User32.dll")]
        public static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);
        /// <summary>
        /// 如果窗口更新的区域不为空，UpdateWindow函数通过发送一个WM_PAINT消息来更新指定窗口的客户区。函数绕过应用程序的消息队列，直接发送WM_PAINT消息给指定窗口的窗口过程，如果更新区域为空，则不发送消息。
        /// </summary>
        /// <param name="hWnd">要更新的窗口的句柄</param>
        /// <returns>true:成功，false:失败</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool UpdateWindow(IntPtr hWnd);
        /// <summary>
        /// 该函数将创建指定窗口的线程设置到前台，并且激活该窗口。键盘输入转向该窗口，并为用户改各种可视的记号。系统给创建前台窗口的线程分配的权限稍高于其他线程。
        /// </summary>
        /// <returns>如果窗口设入了前台，返回true；如果窗口未被设入前台，返回false</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);


        /// <summary>
        /// 该函数改变一个子窗口，弹出式窗口式顶层窗口的尺寸，位置和Z序。
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="hWndInsertAfter">标识了在Z轴次序上位于这个CWnd对象之前的CWnd对象。这个参数可以是指向CWnd对象的指针，也可以是指向枚举InsertAfter值的指针。</param>
        /// <param name="x">指定窗口左边的新位置</param>
        /// <param name="y">指定窗口顶部的新位置</param>
        /// <param name="Width">指定窗口的新宽度</param>
        /// <param name="Height">指定窗口的新高度</param>
        /// <param name="flags">指定了大小和位置选项。这个参数可以是枚举WindowSiezeLocation值的组合</param>
        /// <returns>true:成功，false:失败</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int Width, int Height, uint flags);
        [DllImport("user32")]
        public static extern int SetWindowLong(IntPtr hwnd, long index, long newLong);
        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern IntPtr GetWindowLong32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        private static extern IntPtr GetWindowLong64(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern IntPtr SetWindowLong32(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLong64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
        public static IntPtr GetWindowLong(IntPtr hWnd, int nIndex)
        {
            return Environment.Is64BitProcess
                ? GetWindowLong64(hWnd, nIndex)
                : GetWindowLong32(hWnd, nIndex);
        }

        public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            return Environment.Is64BitProcess
                ? SetWindowLong64(hWnd, nIndex, dwNewLong)
                : SetWindowLong32(hWnd, nIndex, dwNewLong);
        }

        /// <summary>
        /// 设置owner属性
        /// </summary>
        /// <param name="hWndChild"></param>
        /// <param name="hWndNewParent"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        #region 剪切板
        /// <summary>
        /// 指定关联到打开的剪切板的窗口句柄，传入NULL表示关联到当前任务。每次只允许一个进程打开并访问。每打开一次就要关闭，否则其他进程无法访问剪切板。
        /// </summary>
        /// <param name="hWndNewOwner">窗口句柄</param>
        /// <returns>true:成功，false:失败</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool OpenClipboard(IntPtr hWndNewOwner);
        /// <summary>
        /// 关闭剪切板
        /// </summary>
        /// <returns>true:成功，false:失败</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool CloseClipboard();
        /// <summary>
        /// 打开清空，写入前必须先清空，得到剪切板占有权。
        /// </summary>
        /// <returns>true:成功，false:失败</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool EmptyClipboard();

        /// <summary>
        /// 将存放有数据的内存块放入剪切板的资源管理中
        /// </summary>
        /// <param name="Format">用来指定要放到剪切板中的数据的格式</param>
        /// <param name="hData">指定具有指定格式的数据的句柄,该参数可以是空.</param>
        /// <returns> 返回存放数据的指针 </returns>
        /// <remarks>
        /// 如果hMem为空则表明直到有其他程序对剪切板中的数据进行请求时,该程序才会将指定格式的数据写入到剪切板中.
        /// 延迟提交所说的就是第二个参数留空.
        /// 调用该函数以后，hMem所指定的内存对象被系统拥有，程序不应当将他释放，或者锁定。
        /// </remarks>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetClipboardData(uint Format, IntPtr hData);
        #endregion

        /// <summary>
        /// 在一个矩形中装载指定菜单条目的屏幕坐标信息 
        /// </summary>
        /// <param name="hWnd">包含指定菜单或弹出式菜单的一个窗口的句柄</param>
        /// <param name="hMenu">菜单的句柄</param>
        /// <param name="item">欲检查的菜单条目的位置或菜单ID</param>
        /// <param name="rc">在这个结构中装载菜单条目的位置及大小（采用屏幕坐标表示）</param>
        /// <returns>true:成功，false:失败</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetMenuItemRect(IntPtr hWnd, IntPtr hMenu, uint item, ref RECT rc);
        /// <summary>
        /// 该函数获得一个指定子窗口的父窗口句柄。
        /// </summary>
        /// <param name="hWnd">欲测试的窗口的句柄</param>
        /// <returns>父窗口的句柄。如窗口没有父，或遇到错误，则返回零。</returns>
        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        /// <summary>
        /// 该函数将指定的消息发送到一个或多个窗口。此函数为指定的窗口调用窗口程序，直到窗口程序处理完消息再返回。　
        /// </summary>
        /// <param name="hWnd">其窗口程序将接收消息的窗口的句柄</param>
        /// <param name="msg">指定被发送的消息</param>
        /// <param name="wParam">指定附加的消息指定信息</param>
        /// <param name="lParam">指定附加的消息指定信息</param>
        /// <returns>由具体的消息决定</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, int lParam);
        /// <summary>
        /// 该函数将指定的消息发送到一个或多个窗口。此函数为指定的窗口调用窗口程序，直到窗口程序处理完消息再返回。　
        /// </summary>
        /// <param name="hwnd">其窗口程序将接收消息的窗口的句柄</param>
        /// <param name="msg">指定被发送的消息</param>
        /// <param name="wParam">指定附加的消息指定信息</param>
        /// <param name="lParam">指定附加的消息指定信息</param>
        /// <returns>由具体的消息决定</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hwnd, int msg, IntPtr wParam, ref IntPtr lParam);
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, ref COPYDATASTRUCT lParam);
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, COPYDATASTRUCT lParam);
        /// <summary>
        /// 该函数将指定的消息发送到一个或多个窗口。此函数为指定的窗口调用窗口程序，直到窗口程序处理完消息再返回。　
        /// </summary>
        /// <param name="hWnd">窗口句柄。窗口可以是任何类型的屏幕对象。</param>
        /// <param name="msg">用于区别其他消息的常量值</param>
        /// <param name="wParam">通常是一个与消息有关的常量值，也可能是窗口或控件的句柄。</param>
        /// <param name="lParam">通常是一个指向内存中数据的指针</param>
        /// <returns>由具体的消息决定</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// 该函数将一个消息放入（寄送）到与指定窗口创建的线程相联系消息队列里
        /// </summary>
        /// <param name="hWnd">信息发往的窗口的句</param>
        /// <param name="msg">消息ID</param>
        /// <param name="wParam">参数1</param>
        /// <param name="lParam">参数2</param>
        /// <returns>由具体的消息决定</returns>
        [DllImport("user32.dll", EntryPoint = "PostMessage", CharSet = CharSet.Auto)]
        public static extern IntPtr PostMessage(IntPtr hWnd, int msg, IntPtr wParam, ref IntPtr lParam);



        /// <summary>
        /// 取得指定窗口的系统菜单的句柄。
        /// </summary>
        /// <param name="hwnd">指向要获取系统菜单窗口的 <see cref="IntPtr"/> 句柄。</param>
        /// <param name="bRevert">获取系统菜单的方式。设置为 <b>true</b>，表示接收原始的系统菜单，否则设置为 <b>false</b> 。</param>
        /// <returns>指向要获取的系统菜单的 <see cref="IntPtr"/> 句柄。</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetSystemMenu(IntPtr hwnd, bool bRevert);

        /// <summary>
        /// 获取指定的菜单中条目（菜单项）的数量。
        /// </summary>
        /// <param name="hMenu">指向要获取菜单项数量的系统菜单的 <see cref="IntPtr"/> 句柄。</param>
        /// <returns>菜单中的条目数量</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetMenuItemCount(IntPtr hMenu);

        /// <summary>
        /// 删除指定的菜单条目。
        /// </summary>
        /// <param name="hMenu">指向要移除的菜单的 <see cref="IntPtr"/> 。</param>
        /// <param name="uPosition">欲改变的菜单条目的标识符。</param>
        /// <param name="uFlags"></param>
        /// <returns>非零表示成功，零表示失败。</returns>
        /// <remarks>
        /// 如果在 <paramref name="uFlags"/> 中使用了<see cref="MF_BYPOSITION"/> ，则在 <paramref name="uPosition"/> 参数表示菜单项的索引；
        /// 如果在 <paramref name="uFlags"/> 中使用了 <b>MF_BYCOMMAND</b>，则在 <paramref name="uPosition"/> 中使用菜单项的ID。
        /// </remarks>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int RemoveMenu(IntPtr hMenu, int uPosition, int uFlags);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);

        /// <summary>
        /// 根据窗口句柄获得进程PID和线程PID
        /// </summary>
        /// <param name="hwnd">句柄</param>
        /// <param name="pid">返回 进程PID</param>
        /// <returns>方法的返回值，线程PID，进程PID和线程PID这两个概念不同</returns>
        [DllImport("user32.dll", EntryPoint = "GetWindowThreadProcessId")]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int pid);

        /// <summary>
        /// 设置光标的位置，以屏幕坐标表示
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern int SetCursorPos(int x, int y);
        /// <summary>
        /// 设置系统参数
        /// </summary>
        /// <param name="uAction"></param>
        /// <param name="uParam"></param>
        /// <param name="lpvParam"></param>
        /// <param name="fuWinIni"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
        public static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="hWnd"></param>
        ///// <param name="nXAmount"></param>
        ///// <param name="nYAmount"></param>
        ///// <param name="rectScrollRegion"></param>
        ///// <param name="rectClip"></param>
        ///// <returns></returns>
        //[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        //public static extern bool ScrollWindow(HandleRef hWnd, int nXAmount, int nYAmount, ref RECT rectScrollRegion, ref RECT rectClip);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="nBar"></param>
        /// <param name="nPos"></param>
        /// <param name="bRedraw"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int SetScrollPos(HandleRef hWnd, int nBar, int nPos, bool bRedraw);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="fnBar"></param>
        /// <param name="si"></param>
        /// <param name="redraw"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int SetScrollInfo(HandleRef hWnd, int fnBar, SCROLLINFO si, bool redraw);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SetClassLong(IntPtr hwnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetClassLong(IntPtr hwnd, int nIndex);

        [DllImport("user32", EntryPoint = "SetWindowLong")]
        public static extern uint SetWindowLong(IntPtr hwnd, int nIndex, uint dwNewLong);

        [DllImport("user32", EntryPoint = "SetLayeredWindowAttributes")]
        public static extern int SetLayeredWindowAttributes(IntPtr hwnd, int crKey, int bAlpha, int dwFlags);


        /// <summary>
        /// 查找窗口。来根据类名和窗口名来得到窗口句柄。但是这个函数不能查找子窗口，也不区分大小写。如果要从一个窗口的子窗口中查找需要使用的FindWindowEX
        /// </summary>
        /// <param name="classname">要查找子窗口的父窗口句柄。如果hwnjParent为NULL，则函数桌面窗口为父窗口，查找桌面窗口的所有子窗口。</param>
        /// <param name="title">窗体的标题名</param>
        /// <returns>假设函数成功，返回值为具有指定类名和窗体名的窗体句柄；假设函数失败，返回值为NULL</returns>
        [DllImport("user32.dll", EntryPoint = "FindWindow", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr FindWindow(string classname, string title); // extern method: FindWindow
        /// <summary>
        /// 查找窗口。来根据类名和窗口名来得到窗口句柄。但是这个函数不能查找子窗口，也不区分大小写。如果要从一个窗口的子窗口中查找需要使用的FindWindowEX
        /// </summary>
        /// <param name="classname">要查找子窗口的父窗口句柄。如果hwnjParent为NULL，则函数桌面窗口为父窗口，查找桌面窗口的所有子窗口。</param>
        /// <param name="title">窗体的标题名</param>
        /// <returns>假设函数成功，返回值为具有指定类名和窗体名的窗体句柄；假设函数失败，返回值为NULL</returns>
        [DllImport("user32.dll", EntryPoint = "FindWindow", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr FindWindow(IntPtr classname, string title); // extern method: FindWindow
        /// <summary>
        /// 查找窗口
        /// 如果 Parent 是 0, 则函数以桌面窗口为父窗口, 查找桌面窗口的所有子窗口;
        /// 如果 是 HWND_MESSAGE, 函数仅查找所有消息窗口;
        /// 子窗口必须是 Parent 窗口的直接子窗口;
        /// 如果 Child 是 0, 查找从 Parent 的第一个子窗口开始;
        /// 如果 Parent 和 Child 同时是 0, 则函数查找所有的顶层窗口及消息窗口。
        /// </summary>
        /// <param name="hwndParent">要查找子窗口的父窗口句柄。</param>
        /// <param name="hwndChildAfter">
        /// 子窗口句柄。查找从在Z序中的下一个子窗口开始。子窗口必须为hwndPareRt窗口的直接子窗口而非后代窗口。
        /// 如果HwndChildAfter为NULL，查找从hwndParent的第一个子窗口开始。如果hwndParent 和 hwndChildAfter同时为NULL，则函数查找所有的顶层窗口及消息窗口。</param>
        /// <param name="lpszClass">
        /// 指向一个指定了类名的空结束字符串，或一个标识类名字符串的成员的指针。如果该参数为一个成员，
        /// 则它必须为前次调用theGlobaIAddAtom函数产生的全局成员。该成员为16位，必须位于lpClassName的低16位，高位必须为0。</param>
        /// <param name="lpszWindow">指向一个指定了窗口名（窗口标题）的空结束字符串。如果该参数为 NULL，则为所有窗口全匹配。</param>
        /// <returns>返回值：如果函数成功，返回值为具有指定类名和窗口名的窗口句柄。如果函数失败，返回值为NULL。</returns>
        [DllImport("user32.dll", EntryPoint = "FindWindowEx", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll")]
        public static extern void MoveWindow(IntPtr hwnd, int X, int Y, int nWidth, int nHeight, bool rePaint); // extern method: MoveWindow

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, out Rectangle rect); // extern method: GetWindowRect


        /// <summary>
        /// 确定当前焦点位于哪个控件上。
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern IntPtr GetFocus();
        [DllImport("user32.dll")]
        public static extern IntPtr AttachThreadInput(IntPtr idAttach, IntPtr idAttachTo, int fAttach);
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);
        [DllImport("user32.dll")]
        public static extern void ClientToScreen(IntPtr hWnd, ref Point p);
        [DllImport("user32.dll")]
        public static extern bool GetComboBoxInfo(IntPtr hwnd, ref COMBOBOXINFO pcbi);
        /// <summary>
        /// 将应用程序定义的挂钩过程安装到挂钩链中。您将安装一个挂钩过程来监视系统中的某些类型的事件。这些事件与特定线程或与调用线程在同一桌面上的所有线程相关联。
        /// </summary>
        /// <param name="idHook">要安装的挂钩过程的类型</param>
        /// <param name="lpfn">指向钩子过程的指针。如果dwThreadId参数为零或指定由其他进程创建的线程的标识符，则lpfn参数必须指向DLL中的挂钩过程。否则，lpfn可以指向与当前进程关联的代码中的挂钩过程。</param>
        /// <param name="hInstance">DLL的句柄，其中包含由lpfn参数指向的挂钩过程。如果dwThreadId参数指定了由当前进程创建的线程，并且挂钩过程在与当前进程关联的代码中，则hMod参数必须设置为NULL。</param>
        /// <param name="threadId">挂钩过程将与之关联的线程的标识符。对于桌面应用程序，如果此参数为零，则挂钩过程与与调用线程在同一桌面上运行的所有现有线程相关联。</param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        public enum WH_Codes : int
        {
            /// <summary>
            /// 底层键盘钩子
            /// </summary>
            WH_KEYBOARD_LL = 13,

            /// <summary>
            /// 底层鼠标钩子
            /// </summary>
            WH_MOUSE_LL = 14
        }
        /// <summary>
        /// 安装钩子
        /// </summary>
        /// <param name="idHook"></param>
        /// <param name="lpfn"></param>
        /// <param name="hInstance"></param>
        /// <param name="threadId"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr SetWindowsHookEx(WH_Codes idHook, HookProc lpfn, IntPtr pInstance, int threadId);

        //[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        //public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        //卸载钩子
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);

        /// <summary>
        /// 卸载钩子
        /// </summary>
        /// <param name="idHook"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(IntPtr pHookHandle);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        //调用下一个钩子
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, IntPtr wParam, IntPtr lParam);


        ///// <summary>
        ///// 传递钩子
        ///// </summary>
        ///// <param name="idHook"></param>
        ///// <param name="nCode"></param>
        ///// <param name="wParam"></param>
        ///// <param name="lParam"></param>
        ///// <returns></returns>
        //[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall)]
        //public static extern int CallNextHookEx(IntPtr pHookHandle, int nCode,  Int32 wParam, IntPtr lParam);

        /// <summary>
        /// 转换当前按键信息
        /// </summary>
        /// <param name="uVirtKey"></param>
        /// <param name="uScanCode"></param>
        /// <param name="lpbKeyState"></param>
        /// <param name="lpwTransKey"></param>
        /// <param name="fuState"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern int ToAscii(uint uVirtKey, uint uScanCode,
            byte[] lpbKeyState, byte[] lpwTransKey, uint fuState);

        [DllImport("user32.dll")]
        public static extern bool EnableWindow(IntPtr hWnd, bool bEnable);


       


        public static uint mouseSpeedLow = 0;
        public static uint mouseSpeedNormal = 10;
        [DllImport("User32.dll")]
        public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, uint pvParam, uint fWinIni);

        /// <summary>
        /// 设置鼠标速度
        /// </summary>
        /// <param name="speed">鼠标速度，最大值20，最小值1</param>
        public static void SetMouseSpeed(uint speed)
        {
            uint mouseSpeed = speed;
            if (mouseSpeed < 1)
            {
                mouseSpeed = 1;
            }
            else if (mouseSpeed > 20)
            {
                mouseSpeed = 20;
            }
            SystemParametersInfo(0x0071, 0, mouseSpeed, 0);
        }
        [DllImport("user32.dll", EntryPoint = "keybd_event")]
        public static extern void Keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
        [DllImport("user32.dll")]
        public static extern byte MapVirtualKey(byte wCode, int wMap);
        [DllImport("User32",EntryPoint = "mouse_evnet" )]
        public extern static void Mouse_evnet(int dwFlags, int dx, int dy, int dwData, IntPtr dwExtraInfo);

        /// <summary>
        ///EntryPoint 入口点名称
        ///SetLastError FindLastError方法的返回值保存在这里
        /// </summary>
        /// <param name="dwFlags"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="cButtons"></param>
        /// <param name="dwExtraInfo"></param>
        [DllImport("user32.dll", EntryPoint = "mouse_event", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void Mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        /// <summary>
        /// 该函数综合鼠标击键和鼠标动作
        /// </summary>
        /// <param name="flags"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="data"></param>
        /// <param name="extraInfo"></param>
        [DllImport("user32.dll", EntryPoint = "mouse_event")]
        public static extern void Mouse_event(MouseEventFlag flags, int dx, int dy, int data, UIntPtr extraInfo);


        [DllImport("USER32.dll")]
        public static extern short GetKeyState(VirtualKeyStates nVirtKey);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern short VkKeyScanEx(char ch, IntPtr dwhkl);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns>
        /// Long，低字包含了虚拟键码。高字则包含了下述标志：
        /// 位0指出 Shift 键已经按下；位1指出 Ctrl 键已经按下；位2指出 Alt 键已经按下
        /// </returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern short VkKeyScan(char key);

        /// Return Type: BOOL->int
        ///lpRect: RECT*
        [DllImport("user32.dll", EntryPoint = "ClipCursor")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ClipCursor([In] IntPtr lpRect);


        /// <summary>
        /// 获取窗口是否是最小化的
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool IsIconic(IntPtr hWnd);

        /// <summary>
        /// 注册消息
        /// </summary>
        /// <param name="lpString"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int RegisterWindowMessage(string lpString);
        ///// <summary>
        ///// WM_COPYDATA消息的主要目的是允许在进程间传递只读数据
        ///// </summary>
        //public const int WM_COPYDATA = 0x004A;
        public static void SendHandleCmd(string strSent, IntPtr WINDOW_HANDLE)
        {
            if (WINDOW_HANDLE != IntPtr.Zero)
            {
                byte[] arr = Encoding.Default.GetBytes(strSent);
                int len = arr.Length;
                COPYDATASTRUCT cdata;
                cdata.dwData = (IntPtr)100;
                cdata.lpData = strSent;
                cdata.cData = len + 1;
                SendMessage(WINDOW_HANDLE, NativeConst.WM_COPYDATA,    (IntPtr)0, ref cdata);
            }
        }

        /// <summary>
        /// 获取窗口客户区的大小。注意一下：窗口的客户区为窗口中除标题栏、菜单栏之外的地方
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="lpRect"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);


        [StructLayout(LayoutKind.Sequential)]
        public struct PONITAPI
        {
            public int x, y;
        }
        /// <summary>
        /// 检取光标的位置，以屏幕坐标表示。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern int GetCursorPos(ref PONITAPI p);

        /// <summary>
        /// 查找当前鼠标位置的句柄
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(Point point);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
        private static extern IntPtr IntSetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
        private static extern int IntSetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("HardwareID.dll")]
        public static extern string GetHardwareIDWithAppID(string AppID, bool HDD, bool NIC, bool CPU, bool BIOS, string sRegistrationCode);

        [DllImport("user32")]
        static extern int GetSystemMetrics(int n);
    }
}
