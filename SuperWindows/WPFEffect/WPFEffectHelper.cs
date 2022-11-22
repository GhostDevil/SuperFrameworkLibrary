using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace SuperFramework.SuperWindows.WPFEffect
{
    /// <summary>
    /// 日 期:2014-12-10
    /// 作 者:不良帥
    /// 描 述:Windows 窗体特效
    /// </summary>
    public static class WPFEffectHelper
    {
        ///// <summary>
        ///// 设置控件的【大小、背景】动画效果，
        ///// 高度、宽度默认当前大小，背景默认White~LightGreen
        ///// </summary>
        ///// <param name="control">要设置动画的控件</param>
        ///// <Author> frd 2011-9-8</Author>
        //public static void SetDoubleAnimation(object control)
        //{
        //    Type type = control.GetType();
        //    switch ("Border")
        //    {
        //        case "Image":
        //            {
        //                //其他控件
        //            }
        //            break;
        //        case "Border":
        //            {
        //                Border newBorder = (Border)control;
        //                #region 高宽变化动画

        //                DoubleAnimation widthAnimation = new DoubleAnimation(0, newBorder.Width, new Duration(TimeSpan.FromSeconds(0.5)));
        //                newBorder.BeginAnimation(Border.WidthProperty, widthAnimation, HandoffBehavior.Compose);

        //                DoubleAnimation heightAnimation = new DoubleAnimation(0, newBorder.Height, new Duration(TimeSpan.FromSeconds(0.5)));
        //                newBorder.BeginAnimation(Border.HeightProperty, heightAnimation, HandoffBehavior.Compose);
        //                #endregion

        //                #region 背景颜色变化动画
        //                SolidColorBrush myBrush = new SolidColorBrush();

        //                ColorAnimation myColorAnimation = new ColorAnimation();
        //                myColorAnimation.From = Colors.White;
        //                myColorAnimation.To = Colors.LightGreen;
        //                myColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
        //                myColorAnimation.AutoReverse = true;
        //                myColorAnimation.RepeatBehavior = RepeatBehavior.Forever;

        //                myBrush.BeginAnimation(SolidColorBrush.ColorProperty, myColorAnimation, HandoffBehavior.Compose);
        //                newBorder.Background = myBrush;

        //                #endregion
        //            }
        //            break;

        //        default:
        //            break;
        //    }
        //}
        /// <summary>
        /// 获取窗口在右下角显示的位置
        /// </summary>
        /// <param name="win">窗口对象</param>
        /// <returns>返回开始坐标点</returns>
        public static Point GetRightPoint(Window win)
        {
            return new Point(Screen.PrimaryScreen.WorkingArea.Right - win.Width, Screen.PrimaryScreen.WorkingArea.Bottom - win.Height);//设置窗体在屏幕右下角显示
        }
        /// <summary>
        /// 右下角窗口
        /// </summary>
        /// <param name="win">窗口对象</param>
        public static void SetRightWindow(Window win)
        {
            win.WindowStartupLocation = WindowStartupLocation.Manual;
            Point p = GetRightPoint(win);
            win.Left = p.X;
            win.Top = p.Y;
        }

        /// <summary>
        /// 设置控件Clip属性
        /// </summary>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="radiusX">椭圆的X半径</param>
        /// <param name="radiusY">椭圆的Y半径</param>
        /// <param name="x">起始坐标X</param>
        /// <param name="y">起始坐标Y</param>
        /// <returns></returns>
        public static PathGeometry GetControlClip(double width, double height, double radiusX, double radiusY, double x = 0, double y = 0)
        {
            RectangleGeometry rg = new()
            {
                //设置矩形区域大小
                Rect = new Rect(x, y, width, height),
                RadiusX = radiusX,
                RadiusY = radiusY
            };
            //合并几何图形
            PathGeometry pg = new();
            pg = Geometry.Combine(pg, rg, GeometryCombineMode.Union, null);
            return pg;
        }
    }
}
