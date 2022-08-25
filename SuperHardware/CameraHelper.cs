using System;
using System.Runtime.InteropServices;

namespace SuperFramework.SuperHardware
{
    /// <summary>
    /// 类名:CameraHelper
    /// 版本:Release
    /// 日期:2015-06-24
    /// 作者:不良帥
    /// 描述:获取电脑摄像头操作辅助类
    /// </summary>
    public class CameraHelper
    {
        private IntPtr lwndC;
        private IntPtr mControlPtr;
        private int mWidth;
        private int mHeight;

        #region  构造函数 
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="handle">显示画面的句柄</param>
        /// <param name="width">画面宽度</param>
        /// <param name="height">画面高度</param>
        public CameraHelper(IntPtr handle, int width, int height)
        {
            mControlPtr = handle;
            mWidth = width;
            mHeight = height;
        }
        #endregion

        #region  帧回调的委托 
        /// <summary>
        /// 帧回调的委托
        /// </summary>
        /// <param name="data"></param>
        public delegate void RecievedFrameEventHandler(byte[] data);
        public event RecievedFrameEventHandler RecievedFrame;
        private AviCapture.FrameEventHandler mFrameEventHandler;
        #endregion

        #region 关闭摄像头 
        /// <summary>
        /// 关闭摄像头
        /// </summary>
        public void CloseWebcam()
        {
            capDriverDisconnect(lwndC);
        }
        #endregion

        #region 开启摄像头 
        /// <summary>
        /// 开启摄像头
        /// </summary>
        public void StartWebCam()
        {
            byte[] lpszName = new byte[100];
            byte[] lpszVer = new byte[100];

            SuperFramework.SuperHardware.AviCapture.capGetDriverDescriptionA(1, lpszName, 100, lpszVer, 100);
            lwndC = SuperFramework.SuperHardware.AviCapture.capCreateCaptureWindowA(lpszName, SuperFramework.SuperHardware.AviCapture.WS_VISIBLE + SuperFramework.SuperHardware.AviCapture.WS_CHILD, 0, 0, mWidth, mHeight, mControlPtr, 0);

            if (capDriverConnect(lwndC, 0))
            {
                capPreviewRate(lwndC, 66);

                capPreview(lwndC, true);
                capOverlay(lwndC, true);
                AviCapture.BITMAPINFO bitmapinfo = new AviCapture.BITMAPINFO();
                bitmapinfo.bmiHeader.biSize = SuperFramework.SuperHardware.AviCapture.SizeOf(bitmapinfo.bmiHeader);
                bitmapinfo.bmiHeader.biWidth = mWidth;
                bitmapinfo.bmiHeader.biHeight = mHeight;
                bitmapinfo.bmiHeader.biPlanes = 1;
                bitmapinfo.bmiHeader.biBitCount = 24;
                capSetVideoFormat(lwndC, ref bitmapinfo, SuperFramework.SuperHardware.AviCapture.SizeOf(bitmapinfo));

                mFrameEventHandler = new AviCapture.FrameEventHandler(FrameCallBack);
                capSetCallbackOnFrame(lwndC, mFrameEventHandler);
                SuperFramework.SuperHardware.AviCapture.SetWindowPos(lwndC, 0, 0, 0, mWidth, mHeight, 6);
            }
        }
        #endregion

        #region  抓图到文件 
        /// <summary>
        /// 抓图到文件
        /// </summary>
        /// <param name="path"></param>
        public void GrabImage(string path)
        {
            IntPtr hBmp = Marshal.StringToHGlobalAnsi(path);
            SuperFramework.SuperHardware.AviCapture.SendMessage(lwndC, SuperFramework.SuperHardware.AviCapture.WM_CAP_SAVEDIB, 0, hBmp.ToInt32());
        }
        #endregion

        #region  抓图到剪切板 
        /// <summary>
        /// 抓图到剪切板
        /// </summary>
        /// <returns></returns>
        public bool GrabImageToClipBoard()
        {
            return SuperFramework.SuperHardware.AviCapture.SendMessage(lwndC, SuperFramework.SuperHardware.AviCapture.WM_CAP_EDIT_COPY, 0, 0);
        }
        #endregion

        #region  弹出色彩设置对话框 
        /// <summary>
        /// 弹出色彩设置对话框
        /// </summary>
        public void SetCaptureSource()
        {
            AviCapture.CAPDRIVERCAPS caps = new AviCapture.CAPDRIVERCAPS();
            SuperFramework.SuperHardware.AviCapture.SendMessage(lwndC, SuperFramework.SuperHardware.AviCapture.WM_CAP_GET_CAPS, SuperFramework.SuperHardware.AviCapture.SizeOf(caps), ref caps);
            if (caps.fHasDlgVideoSource)
            {
                SuperFramework.SuperHardware.AviCapture.SendMessage(lwndC, SuperFramework.SuperHardware.AviCapture.WM_CAP_DLG_VIDEOSOURCE, 0, 0);
            }
        }
        #endregion

        #region  弹出视频格式设置对话框 
        /// <summary>
        /// 弹出视频格式设置对话框
        /// </summary>
        public void SetCaptureFormat()
        {
            AviCapture.CAPDRIVERCAPS caps = new AviCapture.CAPDRIVERCAPS();
            SuperFramework.SuperHardware.AviCapture.SendMessage(lwndC, SuperFramework.SuperHardware.AviCapture.WM_CAP_GET_CAPS, SuperFramework.SuperHardware.AviCapture.SizeOf(caps), ref caps);
            if (caps.fHasDlgVideoSource)
            {
                SuperFramework.SuperHardware.AviCapture.SendMessage(lwndC, SuperFramework.SuperHardware.AviCapture.WM_CAP_DLG_VIDEOFORMAT, 0, 0);
            }
        }
        #endregion

        #region  以下为私有函数 
        private bool capDriverConnect(IntPtr lwnd, short i)
        {
            return SuperFramework.SuperHardware.AviCapture.SendMessage(lwnd, SuperFramework.SuperHardware.AviCapture.WM_CAP_DRIVER_CONNECT, i, 0);
        }

        private bool capDriverDisconnect(IntPtr lwnd)
        {
            return SuperFramework.SuperHardware.AviCapture.SendMessage(lwnd, SuperFramework.SuperHardware.AviCapture.WM_CAP_DRIVER_DISCONNECT, 0, 0);
        }

        private bool capPreview(IntPtr lwnd, bool f)
        {
            return SuperFramework.SuperHardware.AviCapture.SendMessage(lwnd, SuperFramework.SuperHardware.AviCapture.WM_CAP_SET_PREVIEW, f, 0);
        }

        private bool capPreviewRate(IntPtr lwnd, short wMS)
        {
            return SuperFramework.SuperHardware.AviCapture.SendMessage(lwnd, SuperFramework.SuperHardware.AviCapture.WM_CAP_SET_PREVIEWRATE, wMS, 0);
        }

        private bool capSetCallbackOnFrame(IntPtr lwnd, AviCapture.FrameEventHandler lpProc)
        {
            return SuperFramework.SuperHardware.AviCapture.SendMessage(lwnd, SuperFramework.SuperHardware.AviCapture.WM_CAP_SET_CALLBACK_FRAME, 0, lpProc);
        }

        private bool capSetVideoFormat(IntPtr hCapWnd, ref AviCapture.BITMAPINFO BmpFormat, int CapFormatSize)
        {
            return SuperFramework.SuperHardware.AviCapture.SendMessage(hCapWnd, SuperFramework.SuperHardware.AviCapture.WM_CAP_SET_VIDEOFORMAT, CapFormatSize, ref BmpFormat);
        }

        private void FrameCallBack(IntPtr lwnd, IntPtr lpVHdr)
        {
            AviCapture.VIDEOHDR videoHeader = new AviCapture.VIDEOHDR();
            byte[] VideoData;
            videoHeader = (AviCapture.VIDEOHDR)SuperFramework.SuperHardware.AviCapture.GetStructure(lpVHdr, videoHeader);
            VideoData = new byte[videoHeader.dwBytesUsed];
            SuperFramework.SuperHardware.AviCapture.Copy(videoHeader.lpData, VideoData);
            if (RecievedFrame != null)
                RecievedFrame(VideoData);
        }
        private bool capOverlay(IntPtr lwnd, bool f)
        {
            return SuperFramework.SuperHardware.AviCapture.SendMessage(lwnd, SuperFramework.SuperHardware.AviCapture.WM_CAP_SET_OVERLAY, f, 0);
        }
        #endregion

    }
    #region  视频辅助类 
    /// <summary>
    /// 视频辅助类
    /// </summary>
    internal class AviCapture
    {
        //通过调用acicap32.dll进行读取摄像头数据
        [DllImport("avicap32.dll")]
        public static extern IntPtr capCreateCaptureWindowA(byte[] lpszWindowName, int dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, int nID);
        [DllImport("avicap32.dll")]
        public static extern bool capGetDriverDescriptionA(short wDriver, byte[] lpszName, int cbName, byte[] lpszVer, int cbVer);
        [DllImport("User32.dll")]
        public static extern bool SendMessage(IntPtr hWnd, int wMsg, bool wParam, int lParam);
        [DllImport("User32.dll")]
        public static extern bool SendMessage(IntPtr hWnd, int wMsg, short wParam, int lParam);
        [DllImport("User32.dll")]
        public static extern bool SendMessage(IntPtr hWnd, int wMsg, short wParam, FrameEventHandler lParam);
        [DllImport("User32.dll")]
        public static extern bool SendMessage(IntPtr hWnd, int wMsg, int wParam, ref BITMAPINFO lParam);
        [DllImport("User32.dll")]
        public static extern bool SendMessage(IntPtr hWnd, int wMsg, int wParam, ref CAPDRIVERCAPS lParam);
        [DllImport("User32.dll")]
        public static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);
        [DllImport("avicap32.dll")]
        public static extern int capGetVideoFormat(IntPtr hWnd, IntPtr psVideoFormat, int wSize);

        //部分常量
        public const int WM_USER = 0x400;
        public const int WS_CHILD = 0x40000000;
        public const int WS_VISIBLE = 0x10000000;
        public const int SWP_NOMOVE = 0x2;
        public const int SWP_NOZORDER = 0x4;
        public const int WM_CAP_DRIVER_CONNECT = WM_USER + 10;
        public const int WM_CAP_DRIVER_DISCONNECT = WM_USER + 11;
        public const int WM_CAP_SET_CALLBACK_FRAME = WM_USER + 5;
        public const int WM_CAP_SET_PREVIEW = WM_USER + 50;
        public const int WM_CAP_SET_PREVIEWRATE = WM_USER + 52;
        public const int WM_CAP_SET_VIDEOFORMAT = WM_USER + 45;
        public const int WM_CAP_SAVEDIB = WM_USER + 25;
        public const int WM_CAP_SET_OVERLAY = WM_USER + 51;
        public const int WM_CAP_GET_CAPS = WM_USER + 14;
        public const int WM_CAP_DLG_VIDEOFORMAT = WM_USER + 41;
        public const int WM_CAP_DLG_VIDEOSOURCE = WM_USER + 42;
        public const int WM_CAP_DLG_VIDEODISPLAY = WM_USER + 43;
        public const int WM_CAP_EDIT_COPY = WM_USER + 30;
        public const int WM_CAP_SET_SEQUENCE_SETUP = WM_USER + 64;
        public const int WM_CAP_GET_SEQUENCE_SETUP = WM_USER + 65;


        // 结构
        [StructLayout(LayoutKind.Sequential)]
        //VideoHdr
        public struct VIDEOHDR
        {
            [MarshalAs(UnmanagedType.I4)]
            public int lpData;
            [MarshalAs(UnmanagedType.I4)]
            public int dwBufferLength;
            [MarshalAs(UnmanagedType.I4)]
            public int dwBytesUsed;
            [MarshalAs(UnmanagedType.I4)]
            public int dwTimeCaptured;
            [MarshalAs(UnmanagedType.I4)]
            public int dwUser;
            [MarshalAs(UnmanagedType.I4)]
            public int dwFlags;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public int[] dwReserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        //BitmapInfoHeader
        public struct BITMAPINFOHEADER
        {
            [MarshalAs(UnmanagedType.I4)]
            public int biSize;
            [MarshalAs(UnmanagedType.I4)]
            public int biWidth;
            [MarshalAs(UnmanagedType.I4)]
            public int biHeight;
            [MarshalAs(UnmanagedType.I2)]
            public short biPlanes;
            [MarshalAs(UnmanagedType.I2)]
            public short biBitCount;
            [MarshalAs(UnmanagedType.I4)]
            public int biCompression;
            [MarshalAs(UnmanagedType.I4)]
            public int biSizeImage;
            [MarshalAs(UnmanagedType.I4)]
            public int biXPelsPerMeter;
            [MarshalAs(UnmanagedType.I4)]
            public int biYPelsPerMeter;
            [MarshalAs(UnmanagedType.I4)]
            public int biClrUsed;
            [MarshalAs(UnmanagedType.I4)]
            public int biClrImportant;
        }

        [StructLayout(LayoutKind.Sequential)]
        //BitmapInfo
        public struct BITMAPINFO
        {
            [MarshalAs(UnmanagedType.Struct, SizeConst = 40)]
            public BITMAPINFOHEADER bmiHeader;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
            public int[] bmiColors;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CAPDRIVERCAPS
        {
            [MarshalAs(UnmanagedType.U2)]
            public ushort wDeviceIndex;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fHasOverlay;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fHasDlgVideoSource;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fHasDlgVideoFormat;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fHasDlgVideoDisplay;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fCaptureInitialized;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fDriverSuppliesPalettes;
            [MarshalAs(UnmanagedType.I4)]
            public int hVideoIn;
            [MarshalAs(UnmanagedType.I4)]
            public int hVideoOut;
            [MarshalAs(UnmanagedType.I4)]
            public int hVideoExtIn;
            [MarshalAs(UnmanagedType.I4)]
            public int hVideoExtOut;
        }


        public delegate void FrameEventHandler(IntPtr lwnd, IntPtr lpVHdr);

        // 公共函数
        public static object GetStructure(IntPtr ptr, ValueType structure)
        {
            return Marshal.PtrToStructure(ptr, structure.GetType());
        }

        public static object GetStructure(int ptr, ValueType structure)
        {
            return GetStructure(new IntPtr(ptr), structure);
        }

        public static void Copy(IntPtr ptr, byte[] data)
        {
            Marshal.Copy(ptr, data, 0, data.Length);
        }

        public static void Copy(int ptr, byte[] data)
        {
            Copy(new IntPtr(ptr), data);
        }

        public static int SizeOf(object structure)
        {
            return Marshal.SizeOf(structure);
        }
    }
    #endregion

}
