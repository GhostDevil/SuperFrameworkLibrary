using System;
using System.Runtime.InteropServices;
using System.Windows;
namespace SuperFramework
{
    /// <summary>
    /// 屏幕Dpi缩放
    /// </summary>
    public class ScreenDpi
    {
        #region Win32 API
        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr ptr);
        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(
            IntPtr hdc, // handle to DC
            int nIndex // index of capability
        );
        [DllImport("user32.dll", EntryPoint = "ReleaseDC")]
        static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDc);
        #endregion

        #region DeviceCaps常量
        const int HORZRES = 8;
        const int VERTRES = 10;
        const int LOGPIXELSX = 88;
        const int LOGPIXELSY = 90;
        const int DESKTOPVERTRES = 117;
        const int DESKTOPHORZRES = 118;
        #endregion

        #region 属性

        /// <summary>
        /// 当前系统DPI_X 大小 一般为96
        /// </summary>
        public static int DpiX
        {
            get
            {
                var hdc = GetDC(IntPtr.Zero);
                var dpiX = GetDeviceCaps(hdc, LOGPIXELSX);
                ReleaseDC(IntPtr.Zero, hdc);
                return dpiX;
            }
        }

        /// <summary>
        /// 当前系统DPI_Y 大小 一般为96
        /// </summary>
        public static int DpiY
        {
            get
            {
                var hdc = GetDC(IntPtr.Zero);
                var dpiX = GetDeviceCaps(hdc, LOGPIXELSY);
                ReleaseDC(IntPtr.Zero, hdc);
                return dpiX;
            }
        }

        /// <summary>
        /// 获取真实设置的桌面分辨率大小
        /// </summary>
        public static Size Desktop
        {
            get
            {
                var hdc = GetDC(IntPtr.Zero);
                var size = new Size
                {
                    Width = GetDeviceCaps(hdc, DESKTOPHORZRES),
                    Height = GetDeviceCaps(hdc, DESKTOPVERTRES)
                };
                ReleaseDC(IntPtr.Zero, hdc);
                return size;
            }
        }

        /// <summary>
        /// 获取屏幕分辨率当前物理大小
        /// </summary>        
        public static Size WorkingArea
        {
            get
            {
                var hdc = GetDC(IntPtr.Zero);
                var size = new Size
                {
                    Width = GetDeviceCaps(hdc, HORZRES),
                    Height = GetDeviceCaps(hdc, VERTRES)
                };
                ReleaseDC(IntPtr.Zero, hdc);
                return size;
            }
        }
        /// <summary>
        /// 获得横轴缩放比
        /// 此参数为[控制面板-设置-显示-更改文本、应用等项目的大小]所显示的百分比
        /// </summary>
        public static float ScaleX => DpiX / 96f;

        /// <summary>
        ///  获得纵轴缩放比
        /// 此参数为[控制面板-设置-显示-更改文本、应用等项目的大小]所显示的百分比
        /// </summary>
        public static float ScaleY => DpiY / 96f;

        #endregion
        /// <summary>
        /// 缩放比
        /// </summary>
        /// <returns></returns>
        public static float GetPositionScale()
        {
            var hdc = GetDC(IntPtr.Zero);
            var dpiX = GetDeviceCaps(hdc, LOGPIXELSX);
            ReleaseDC(IntPtr.Zero, hdc);
            var scale = Convert.ToInt32(dpiX / 96f * 100);
            switch (scale)
            {
                case 125:
                    return .8f;
                case 150:
                    return .65f;
                case 175:
                    return .575f;
                default:
                    return 1f;
            }
        }
        public static void GetDPI(out int dpix, out int dpiy)
        {
            dpix = 0;
            dpiy = 0;
            using (System.Management.ManagementClass mc = new("Win32_DesktopMonitor"))
            {
                using (System.Management.ManagementObjectCollection moc = mc.GetInstances())
                {

                    foreach (System.Management.ManagementObject each in moc)
                    {
                        dpix = int.Parse(each.Properties["PixelsPerXLogicalInch"].Value.ToString());
                        dpiy = int.Parse(each.Properties["PixelsPerYLogicalInch"].Value.ToString());
                    }
                }
            }
        }

    }
}
