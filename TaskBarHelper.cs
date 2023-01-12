using System;
using System.Runtime.InteropServices;

namespace SuperFramework
{
    public class TaskBarHelper
    {
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
                System.Drawing.Rectangle r = new();
                GetWindowRect(foor, ref r);
                //从任务栏左上角从左到右 MOUSEMOVE一遍，所有图标状态会被更新
                for (int x = 0; x < r.Right - r.Left - r.X; x++)
                {
                    SendMessage(foor, WM_MOUSEMOVE, 0, (1 << 16) | x);
                }
            }
        }
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
    }
}
