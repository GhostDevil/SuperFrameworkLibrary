using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace SuperFramework.WindowsAPI
{
    /// <summary>
    /// win api辅助
    /// </summary>
    public static class WinApiHelper
    {
        ///// <summary>
        ///// 窗体置顶
        ///// </summary>
        // static IntPtr HWND_TOPMOST = new IntPtr(-1);
        ///// <summary>
        ///// 取消窗体置顶
        ///// </summary>
        // static IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        ///// <summary>
        ///// 不调整窗体位置
        ///// </summary>
        // const uint SWP_NOMOVE = 0x0002;
        ///// <summary>
        ///// 不调整窗体大小
        ///// </summary>
        // const uint SWP_NOSIZE = 0x0001;
        /// <summary>
        /// 置顶窗口
        /// </summary>
        /// <param name="handle">窗口句柄</param>
        /// <param name="isTop">是否置顶</param>
        public static void SetWindowTop(IntPtr handle,bool isTop)
        {
            if(isTop)
                User32API.SetWindowPos(handle,NativeConst.HWND_TOPMOST, 0, 0, 0, 0, NativeConst.SWP_NOMOVE | NativeConst.SWP_NOSIZE);
            else
                User32API.SetWindowPos(handle, NativeConst.HWND_NOTOPMOST, 0, 0, 0, 0, NativeConst.SWP_NOMOVE | NativeConst.SWP_NOSIZE);

        }
       
        /// <summary>
        /// 设置光标位置
        /// </summary>
        /// <param name="isLock">是否锁定</param>
        public static void ClipCursor(bool isLock)
        {

            if (isLock) 
            {
               APIStruct.TagRECT rect = new APIStruct.TagRECT() { left = 0, top = 0, right = 1, bottom = 1 };
                var p = Marshal.AllocHGlobal(Marshal.SizeOf(rect));
                Marshal.StructureToPtr(rect, p, false);
                User32API.ClipCursor(p);//锁定            
            }
            else
                User32API.ClipCursor(IntPtr.Zero);//解锁
        }
        private const int OF_READWRITE = 2;
        private const int OF_SHARE_DENY_NONE = 0x40;
        private static readonly IntPtr HFILE_ERROR = new IntPtr(-1);
        /// <summary>
        /// 以二进制模式打开指定的文件,查看文件是否被占用
        /// </summary>
        /// <param name="fileFullName">文件路径</param>
        /// <returns></returns>
        public static bool IsFileOpen(string fileFullName)
        {
            IntPtr handle=IntPtr.Zero;
            if (File.Exists(fileFullName))
            {
                handle = Kernel32API._lopen(fileFullName, OF_READWRITE | OF_SHARE_DENY_NONE);
                if (handle == HFILE_ERROR)
                {
                    return true;
                }
            }
            Kernel32API.CloseHandle(handle);
            return false;
        }

        public static List<IntPtr> FindWindows(string lpszClass, string lpszWindow)
        {
            List<IntPtr> lip = new List<IntPtr>();
            IntPtr ip = IntPtr.Zero;
            ip = SuperFramework.WindowsAPI.User32API.FindWindow("CabinetWClass", null);
            while (ip != IntPtr.Zero)
            {
                lip.Add(ip);
                ip = SuperFramework.WindowsAPI.User32API.FindWindowEx(IntPtr.Zero, ip, "CabinetWClass", null);
            }
            return lip;
        }

       

        /// <summary>
        /// 鼠标点击窗口位置
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        public static void LeftMouseClickOffice(int X, int Y, int width, int height)
        {
            System.Drawing.Point pos = new System.Drawing.Point(X, Y);
            uint x = (uint)(pos.X * 65535 / width);
            uint y = (uint)(pos.Y * 65535 / height);
            User32API.Mouse_event(NativeConst.MOUSEEVENTF_ABSOLUTE | NativeConst.MOUSEEVENTF_MOVE, x, y, 0, 0);
            User32API.Mouse_event(NativeConst.MOUSEEVENTF_ABSOLUTE | NativeConst.MOUSEEVENTF_LEFTDOWN | NativeConst.MOUSEEVENTF_LEFTUP, x, y, 0, 0);
        }


        
        public static void CloseWindows(string strSent, IntPtr WINDOW_HANDLE)
        {
            if (WINDOW_HANDLE != IntPtr.Zero)
            {
                byte[] arr = Encoding.Default.GetBytes(strSent);
                int len = arr.Length;
                APIStruct.COPYDATASTRUCT cdata;
                cdata.dwData = (IntPtr)100;
                cdata.lpData = strSent;
                cdata.cData = len + 1;
                User32API.SendMessage(WINDOW_HANDLE, NativeConst.WM_CLOSE, 0, ref cdata);
            }
        }

        /// <summary>
        /// 获取句柄的最外层窗口句柄
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        public static IntPtr GetOutWindow(IntPtr hWnd)
        {
            IntPtr parent = hWnd;
            int whileIndex = 0;
            while (true)
            {

                whileIndex++;
                IntPtr tempOut = User32API.GetParent(parent);
                if (tempOut == IntPtr.Zero || whileIndex > 10)
                {
                    break;
                }
                parent = tempOut;
            }
            return parent;
        }

        public static void SendMessage(string strSent, IntPtr WINDOW_HANDLE)
        {
            if (WINDOW_HANDLE != IntPtr.Zero)
            {
                byte[] arr = Encoding.Default.GetBytes(strSent);
                int len = arr.Length;
                APIStruct.COPYDATASTRUCT cdata;
                cdata.dwData = (IntPtr)100;
                cdata.lpData = strSent;
                cdata.cData = len + 1;

                User32API.SendMessage(WINDOW_HANDLE, NativeConst.WM_COPYDATA, 0, ref cdata);
            }
        }
        /// <summary>
        /// 函数用来打开与进程相关联的访问令牌
        /// </summary>
        /// <param name="h"></param>
        /// <param name="acc">第二个参数指定你要进行的操作类型</param>
        /// <param name="phtok">第三个参数就是返回的访问令牌指针</param>
        /// <returns></returns>
        public static bool OpenProcessTokenEX(IntPtr h, int acc, ref IntPtr phtok)
        {
            if (SuperFramework.WindowsAPI.Advapi32API.OpenProcessToken(h, acc, ref phtok) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }

        }



    }
}
