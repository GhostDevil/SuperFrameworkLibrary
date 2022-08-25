using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;

namespace SuperFramework.SuperMouseHelper
{
    /// <summary>
    /// 日 期:2015-07-21
    /// 作 者:不良帥
    /// 描 述:鼠标事件辅助类
    /// </summary>
    public sealed class MouseHelper
    {
        #region WinAPI

        /// <summary>
        /// 鼠标事件类型
        /// </summary>
        private enum MouseEventFlags       //鼠标按键的ASCLL码
        {
            Move = 0x0001,
            LeftDown = 0x0002,
            LeftUp = 0x0004,
            RightDown = 0x0008,
            RightUp = 0x0010,
            MiddleDown = 0x0020,
            MiddleUp = 0x0040,
            Wheel = 0x0800,
            Absolute = 0x8000
        }
        /// <summary>
        /// 坐标点
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            /// <summary>
            /// 横坐标
            /// </summary>
            public int X;
            /// <summary>
            /// 纵坐标
            /// </summary>
            public int Y;
        }

        ///// <summary>
        ///// 鼠标事件
        ///// </summary>
        ///// <param name="dwFlags">事件类型</param>
        ///// <param name="dx">X坐标</param>
        ///// <param name="dy">Y坐标</param>
        ///// <param name="dwData"></param>
        ///// <param name="dwExtraInfo"></param>
        //[DllImport("User32")]
        //private extern static void mouse_event(int dwFlags, int dx, int dy, int dwData, IntPtr dwExtraInfo);
        ///// <summary>
        ///// 设置鼠标的坐标点
        ///// </summary>
        ///// <param name="x">横坐标</param>
        ///// <param name="y">纵坐标</param>
        //[DllImport("User32")]
        //public extern static void SetCursorPos(int x, int y);
        ///// <summary>
        ///// 获取鼠标的坐标点
        ///// </summary>
        ///// <param name="p">输出坐标点</param>
        ///// <returns>成功返回true，否则失败</returns>
        //[DllImport("User32")]
        //public extern static bool GetCursorPos(out Point p);

        #endregion

        /// <summary>
        /// 鼠标左键单击
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        public static void LeftButtonClick(int x = 0, int y = 0)
        {
            WindowsAPI.User32API.SetCursorPos(x, y);
            WindowsAPI.User32API.Mouse_event((int)(MouseEventFlags.LeftDown | MouseEventFlags.Absolute), 0, 0, 0,0);
            Thread.Sleep(100);
           WindowsAPI.User32API.Mouse_event((int)(MouseEventFlags.LeftUp | MouseEventFlags.Absolute), 0, 0, 0, 0);
        }

        /// <summary>
        /// 鼠标右键单击
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        public static void RightButtonClick(int x = 0, int y = 0)
        {
            WindowsAPI.User32API.SetCursorPos(x, y);
            WindowsAPI.User32API.Mouse_event((int)(MouseEventFlags.RightDown | MouseEventFlags.Absolute), 0, 0, 0, 0);
            Thread.Sleep(100);
            WindowsAPI.User32API.Mouse_event((int)(MouseEventFlags.RightUp | MouseEventFlags.Absolute), 0, 0, 0, 0);
        }

        /// <summary>
        /// 鼠标左键双击
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        public static void LeftButtonDoubleClick(int x = 0, int y = 0)
        {
            LeftButtonClick(x, y);
            Thread.Sleep(200);
            LeftButtonClick(x, y);
        }

        /// <summary>
        /// 鼠标右键双击
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        public static void RightButtonDoubleClick(int x = 0, int y = 0)
        {
            RightButtonClick(x, y);
            Thread.Sleep(200);
            RightButtonClick(x, y);
        }
        /// <summary>
        /// 设置鼠标的移动范围
        /// </summary>
        public static void SetMouseRectangle(Rectangle rectangle)
        {
            System.Windows.Forms.Cursor.Clip = rectangle;

        }
        /// <summary>
        /// 设置鼠标位于屏幕中心
        /// </summary>
        public static void SetMouseAtCenterScreen()
        {
            int winHeight = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height;
            int winWidth = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width;           
            WindowsAPI.User32API.SetCursorPos(winWidth / 2, winHeight / 2);
        }
        public static void MouseWheel(bool isDown, uint count = 1)
        {
            if (isDown)
                count = 0 - count * 120;
            else
                count *= 120;
            //控制鼠标滑轮滚动，count代表滚动的值，负数代表向下，正数代表向上，如-100代表向下滚动100的y坐标，一轮为120
            WindowsAPI.User32API.Mouse_event((int)MouseEventFlags.Wheel, 0, 0, count, 0);
        }
    }
}
