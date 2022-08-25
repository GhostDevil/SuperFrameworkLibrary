using System;
using System.Diagnostics;
using System.Management;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace SuperFramework
{
    /// <summary>
    ///     虚拟键盘管理类
    /// </summary>
    public class VirtualKeyboardHelper
    {
        private const uint WS_VISIBLE = 0x10000000;
        private const int GWL_STYLE = -16;
        private const int WM_SYSCOMMAND = 0x0112;
        private const uint SC_CLOSE = 0xF060;
        private const int WS_DISABLED = 0x08000000;
        private const int DWMWA_CLOAKED = 14;

        private const string ApplicationFrameHostClassName = "ApplicationFrameWindow";
        private const string CoreWindowClassName = "Windows.UI.Core.CoreWindow";

        private const string TextInputApplicationCaption = "Microsoft Text Input Application";


        /// <summary>
        ///     win10 虚拟键盘路径
        /// </summary>
        private const string Win10TabTipPath = @"C:\Program Files\Common Files\microsoft shared\ink\TabTip.exe";

        /// <summary>
        ///     win7 虚拟键盘路径
        /// </summary>
        private const string Win7OskPath = @"C:\WINDOWS\system32\osk.exe";

        /// <summary>
        ///     虚拟键盘 窗口名称
        /// </summary>
        private const string TabTipWindowClassName = "IPTIP_Main_Window";

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass,
            string lpszWindow);


        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool PostMessage(IntPtr hWnd, int msg, uint wParam, uint lParam);


        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool Wow64DisableWow64FsRedirection(ref IntPtr ptr);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool Wow64RevertWow64FsRedirection(IntPtr ptr);


        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow", SetLastError = false)]
        private static extern IntPtr GetDesktopWindow();

        [DllImport("dwmapi.dll", EntryPoint = "DwmGetWindowAttribute")]
        private static extern int DwmGetWindowAttribute(IntPtr intPtr, int dwAttribute, out int pvAttribute,
            uint cbAttribute);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool PostMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int RegisterWindowMessage(string lpString);

        ///// <summary>
        ///// 显示屏幕键盘
        ///// </summary>
        ///// <returns></returns>
        //public static int ShowInputPanel()
        //{
        //    try
        //    {
        //        dynamic file = "C:\\Program Files\\Common Files\\microsoft shared\\ink\\TabTip.exe";
        //        if (!System.IO.File.Exists(file))
        //            return -1;
        //        Process.Start(file);
        //        //return SetUnDock(); //不知SetUnDock()是什么，所以直接注释返回1
        //        return 1;
        //    }
        //    catch (Exception)
        //    {
        //        return 255;
        //    }
        //}

        ///// <summary>
        ///// 隐藏屏幕键盘
        ///// </summary>
        //public static void HideInputPanel()
        //{
        //    IntPtr TouchhWnd = new IntPtr(0);
        //    TouchhWnd = FindWindow("IPTip_Main_Window", null);
        //    if (TouchhWnd == IntPtr.Zero)
        //        return;
        //    PostMessage(TouchhWnd, WM_SYSCOMMAND, SC_CLOSE, 0);
        //}

        /// <summary>
        /// 判断键盘是否连接
        /// </summary>
        /// <returns></returns>
        public static bool IsKeyboardAttached()
        {
            try
            {
                ManagementObjectSearcher searcher =
                    new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Keyboard");

                int devCount = 0;

                foreach (ManagementObject obj in searcher.Get())
                {
                    if (obj["Status"].ToString().Contains("OK")) // if device is ready
                    {
                        //surface测试时发现，HID设备，不是键盘，比较特殊
                        if (!obj["Description"].ToString().Contains("HID Keyboard Device"))
                        {
                            devCount++;
                        }
                    }
                }

                return devCount > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     打开虚拟键盘，目前支持win7 64位，win10 64位，exe编译为x86。
        /// </summary>
        public static void ShowVirtualKeyboard()
        {
            //+------------------------------------------------------------------------------+
            //|                    |   PlatformID    |   Major version   |   Minor version   |
            //+------------------------------------------------------------------------------+
            //| Windows 95         |  Win32Windows   |         4         |          0        |
            //| Windows 98         |  Win32Windows   |         4         |         10        |
            //| Windows Me         |  Win32Windows   |         4         |         90        |
            //| Windows NT 4.0     |  Win32NT        |         4         |          0        |
            //| Windows 2000       |  Win32NT        |         5         |          0        |
            //| Windows XP         |  Win32NT        |         5         |          1        |
            //| Windows 2003       |  Win32NT        |         5         |          2        |
            //| Windows Vista      |  Win32NT        |         6         |          0        |
            //| Windows 2008       |  Win32NT        |         6         |          0        |
            //| Windows 7          |  Win32NT        |         6         |          1        |
            //| Windows 2008 R2    |  Win32NT        |         6         |          1        |
            //| Windows 8          |  Win32NT        |         6         |          2        |
            //| Windows 8.1        |  Win32NT        |         6         |          3        |
            //+------------------------------------------------------------------------------+
            //| Windows 10         |  Win32NT        |        10         |          0        |

            try
            {
                var isWin7 = Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 1;
                var isWin8OrWin10 =
                    Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 2;
                var isWin10 = Environment.OSVersion.Version.Major == 10 && Environment.OSVersion.Version.Minor == 0;
                if (isWin7)
                {
                    //win7
                    ShowWin7VirtualKeyboard();
                }
                else if (isWin8OrWin10 || isWin10)
                {
                    //win10 
                    ShowWin10VirtualKeyboard();
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }
        /// <summary>
        /// 关闭虚拟键盘
        /// </summary>
        public void CloseVirtualKeyboard()
        {
            var touchhWnd = FindWindow("IPTip_Main_Window", null);
            if (touchhWnd == IntPtr.Zero)
            {
                return;
            }

            PostMessage(touchhWnd, WM_SYSCOMMAND, SC_CLOSE, 0);
        }
        private static void ShowWin7VirtualKeyboard()
        {
            if (!Environment.Is64BitProcess && Environment.Is64BitOperatingSystem)
            {
                //32位程序 运行在64位系统上，打开32位程序，需要禁用文件重定向
                var ptr = new IntPtr();
                var isWow64FsRedirectionDisabled = Wow64DisableWow64FsRedirection(ref ptr);

                Process.Start(Win7OskPath);

                if (isWow64FsRedirectionDisabled)
                {
                    Wow64RevertWow64FsRedirection(ptr);
                }
            }
            else if (Environment.Is64BitProcess && Environment.Is64BitOperatingSystem)
            {
                Process.Start(Win7OskPath);
            }
        }

        private static void ShowWin10VirtualKeyboard()
        {
            if (!IsTabTipProcessPresent())
            {
                Process.Start(Win10TabTipPath);
                while (!IsValidHandle(FindWindow("IPTIP_Main_Window", "")))
                {
                    Thread.Sleep(100);
                }
            }

            //判断可见性
            if (!IsWin10OnScreenKeyboardVisible())
            {
                ShowByCom();
            }
        }

        private static bool IsWin10OnScreenKeyboardVisible()
        {
            var handle = FindWindow(TabTipWindowClassName, "");
            if (!IsValidHandle(handle))
            {
                return false;
            }

            var isVisible = IsWindowVisibleByHandle(handle);
            if (isVisible.HasValue)
            {
                return isVisible.Value;
            }

            // hard way
            var textInputHandle = FindTextInputWindow();
            return IsValidHandle(textInputHandle);
        }

        private static IntPtr FindTextInputWindow()
        {
            var lastProbed = IntPtr.Zero;
            do
            {
                lastProbed = FindWindowEx(IntPtr.Zero, lastProbed, ApplicationFrameHostClassName, null);
                if (IsValidHandle(lastProbed))
                {
                    var textInput = FindWindowEx(lastProbed, IntPtr.Zero, CoreWindowClassName,
                        TextInputApplicationCaption);
                    return textInput;
                }
            } while (IsValidHandle(lastProbed));

            return IntPtr.Zero;
        }



        private static bool? IsWindowVisibleByHandle(IntPtr handle)
        {
            var style = GetWindowLong(handle, GWL_STYLE);
            //Console.WriteLine( "Style {0:X8}", style );

            // if is disabled - not visible
            if ((style & WS_DISABLED) != 0)
            {
                return false;
            }

            // if has visible style - visible :)
            if ((style & WS_VISIBLE) != 0)
            {
                return true;
            }

            // DWM Window can be cloaked
            // see https://social.msdn.microsoft.com/Forums/vstudio/en-US/f8341376-6015-4796-8273-31e0be91da62/difference-between-actually-visible-and-not-visiblewhich-are-there-but-we-cant-see-windows-of?forum=vcgeneral
            if (DwmGetWindowAttribute(handle, DWMWA_CLOAKED, out var cloaked, 4) == 0)
            {
                if (cloaked != 0)
                {
                    return false;
                }
            }

            // undefined
            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsValidHandle(IntPtr handle)
        {
            // if (?:) will be eliminated by jit
            return IntPtr.Size == 4
                ? handle.ToInt32() > 0
                : handle.ToInt64() > 0;
        }

        private static bool IsTabTipProcessPresent()
        {
            var handle = FindWindow(TabTipWindowClassName, "");
            return IntPtr.Size == 4
                ? handle.ToInt32() > 0
                : handle.ToInt64() > 0;
        }

        private static void ShowByCom()
        {
            ITipInvocation instance = null;
            try
            {
                instance = (ITipInvocation)Activator.CreateInstance(ComTypes.TipInvocationType);
                instance.Toggle(GetDesktopWindow());
            }
            finally
            {
                if (!ReferenceEquals(instance, null))
                {
                    Marshal.ReleaseComObject(instance);
                }
            }
        }
    }

    [ComImport]
    [Guid("37c994e7-432b-4834-a2f7-dce1f13b834b")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ITipInvocation
    {
        void Toggle(IntPtr hwnd);
    }

    internal static class ComTypes
    {
        internal static readonly Guid ImmersiveShellBrokerGuid;
        internal static readonly Type ImmersiveShellBrokerType;

        internal static readonly Guid TipInvocationGuid;
        internal static readonly Type TipInvocationType;

        static ComTypes()
        {
            TipInvocationGuid = Guid.Parse("4ce576fa-83dc-4F88-951c-9d0782b4e376");
            TipInvocationType = Type.GetTypeFromCLSID(TipInvocationGuid);

            ImmersiveShellBrokerGuid = new Guid("228826af-02e1-4226-a9e0-99a855e455a6");
            ImmersiveShellBrokerType = Type.GetTypeFromCLSID(ImmersiveShellBrokerGuid);
        }
    }
}
