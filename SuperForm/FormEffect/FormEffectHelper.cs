using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
//using IWshRuntimeLibrary;
using System.Threading;
using System.Reflection;
using static SuperForm.FormEffect.FormEffectEnum;
using static SuperFramework.SuperWinAPI.User32API;
namespace SuperForm.FormEffect
{
    /// <summary>
    /// 日 期:2014-12-10
    /// 作 者:不良帥
    /// 描 述:Form窗体特效
    /// </summary>
    public static class FormEffectHelper
    {
        #region 获取窗体居中显示的起始坐标
        /// <summary>
        /// 获取窗体居中显示的起始坐标
        /// </summary>
        /// <param name="frm">当前窗体</param>
        /// <returns>返回坐标位置</returns>
        public static Point GetCenterPoint(Form frm)
        {
            return  new Point(Screen.PrimaryScreen.WorkingArea.Left + (Screen.PrimaryScreen.WorkingArea.Width - frm.Width) / 2, (Screen.PrimaryScreen.WorkingArea.Bottom - frm.Height) / 2);

        }
        #endregion

        #region  使用动画方式弹出窗体 
        /// <summary>
        /// 使用动画方式弹出窗体
        /// </summary>
        /// <param name="handle">指定产生动画的窗口的句柄。</param>
        /// <param name="showStyle">指定动画类型。</param>
        /// <param name="time">指明动画持续的时间（以微秒计），完成一个动画的标准时间为200微秒。 </param>
        public static void ShowAnimationWindow(IntPtr handle, ShowStyle showStyle, int time = 200)
        {
            AnimateWindow(handle, time, (int)showStyle);
        }
        /// <summary>
        /// 使用动画方式弹出窗体
        /// </summary>
        /// <param name="handle">指定产生动画的窗口的句柄。</param>
        /// <param name="showStyle">指定动画类型。</param>
        /// <param name="time">指明动画持续的时间（以微秒计），完成一个动画的标准时间为200微秒。 </param>
        public static void ShowAnimationWindow(IntPtr handle, int showStyle, int time = 200)
        {
            AnimateWindow(handle, time, showStyle);
        }
        #endregion

        #region  右下角弹窗带动画 
        /// <summary>
        /// 右下角弹窗带动画
        /// </summary>
        /// <param name="form">窗体对象</param>
        /// <param name="showStyle">动画方式</param>
        /// <param name="time">动画完成时间</param>
        /// <param name="isDialog">是否模式弹窗</param>
        public static void ShowRightWindows(Form form, ShowStyle showStyle = ShowStyle.AW_SLIDE, int time = 1000,bool isDialog=false)
        {
            try
            {
                form.StartPosition = FormStartPosition.Manual;
                form.Location = new Point(Screen.PrimaryScreen.WorkingArea.Right - form.Width, Screen.PrimaryScreen.WorkingArea.Bottom - form.Height);//设置窗体在屏幕右下角显示
                AnimateWindow(form.Handle, time, (int)showStyle);
                if (isDialog)
                    form.ShowDialog();
                else
                    form.Show();
            }
            catch (Exception) { }
        }
        ///// <summary>
        ///// 右下角弹窗带动画
        ///// </summary>
        ///// <param name="form">窗体对象</param>
        ///// <param name="showStyle">动画方式</param>
        ///// <param name="time">动画完成时间</param>
        ///// <param name="isDialog">是否模式弹窗</param>
        //public static void ShowRightWindows(System.Windows.Window window, ShowStyle showStyle = ShowStyle.AW_SLIDE, int time = 1000, bool isDialog = false)
        //{
        //    try
        //    {
        //        window.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
        //        // = new Point(Screen.PrimaryScreen.WorkingArea.Right - (int)window.Width, Screen.PrimaryScreen.WorkingArea.Bottom - (int)window.Height);//设置窗体在屏幕右下角显示
        //        //AnimateWindow(window., time, (int)showStyle);
        //        //if (isDialog)
        //        //    window.ShowDialog();
        //        //else
        //        //    window.Show();
        //    }
        //    catch (Exception) { }
        //}
        #endregion

        #region  右上角弹窗带动画 
        /// <summary>
        /// 右上角弹窗带动画
        /// </summary>
        /// <param name="form">窗体对象</param>
        /// <param name="showStyle">动画方式</param>
        /// <param name="time">动画完成时间</param>
        public static void ShowRightTopWindows(Form form, ShowStyle showStyle = ShowStyle.AW_SLIDE, int time = 1000)
        {
            form.StartPosition = FormStartPosition.Manual;
            form.Location = new Point(Screen.PrimaryScreen.WorkingArea.Right - form.Width, 0 + 2);//设置窗体在屏幕右上角显示
            AnimateWindow(form.Handle, time, (int)ShowStyle.AW_SLIDE);
            form.Show();
        }
        #endregion

        #region  右下角弹窗无动画 
        /// <summary>
        /// 右下角弹窗无动画
        /// </summary>
        /// <param name="form">窗体对象</param>
        public static void ShowRightWindow(Form form)
        {
            form.StartPosition = FormStartPosition.Manual;
            Point p = new Point(Screen.PrimaryScreen.WorkingArea.Width - form.Width, Screen.PrimaryScreen.WorkingArea.Height - form.Height);
            form.PointToScreen(p);
            form.Location = p;
            form.Show();
        }
        #endregion

        #region  连续右侧消息通知 

        static int x = 1, y = 1;
        static int count = 0;
        static List<Point> points = new List<Point>();
        static System.Timers.Timer timer = new System.Timers.Timer() { Enabled = true, Interval = 3000 };
        static DateTime tr = new DateTime();
        static Point location = new Point();
        static Form frm = null;
        static int hover;
        static int poison = 0;
        /// <summary>
        /// 连续右侧消息通知
        /// </summary>
        /// <param name="form">窗体对象</param>
        /// <param name="isCover">是否覆盖</param>
        /// <param name="isAutoClose">是否自动关闭</param>
        /// <param name="showTime">动画时间</param>
        /// <param name="hoverTime">停留时间</param>
        /// <param name="showStyle">动画样式</param>
        /// <param name="startPoison">开始位置（0为从右上角开始，1为右下角开始）</param>
        public static void FrmRightLoad(Form form, bool isCover = false, bool isAutoClose = true, int showTime = 200, int hoverTime = 10, ShowStyle showStyle = ShowStyle.AW_VER_NEGATIVE, int startPoison = 0)
        {
            timer.Interval = hoverTime;
            if (points.Count > 0)
            {
                if (poison == 1)
                {
                    form.Location = points[0];//设置坐标为空缺位置
                    points.RemoveAt(0);
                }
                else
                {
                    form.Location = points[points.Count - 1];//设置坐标为空缺位置
                    points.RemoveAt(points.Count - 1);
                }
            }
            else
            {
                int height = 2;
                // int indexX = Screen.PrimaryScreen.WorkingArea.Right / form.Width;
                int indexX = 2;
                int indexY = Screen.PrimaryScreen.WorkingArea.Bottom / (form.Height + 2);
                int k = Screen.PrimaryScreen.WorkingArea.Bottom % (form.Height + height);
                if (k != 0)
                    height += k / (indexY + 1);
                if (poison == 1)
                    form.Location = new Point(Screen.PrimaryScreen.WorkingArea.Right - form.Width * x, Screen.PrimaryScreen.WorkingArea.Bottom - (form.Height + height) * y);//设置窗体在屏幕右下角显示
                else if (poison == 0)
                    form.Location = new Point(Screen.PrimaryScreen.WorkingArea.Right - form.Width * x, (form.Height + height) * (y - 1) + height);//设置窗体在屏幕右上角显示
                else
                    form.Location = new Point(Screen.PrimaryScreen.WorkingArea.Right - form.Width * x, (form.Height + height) * (y - 1) + height);//设置窗体在屏幕右上角显示
                if (!isCover)
                {
                    if (y < indexY)
                        y++;
                    else if (y == indexY)
                    {
                        y = 1;
                        if (x == indexX)
                            x = 1;
                        else
                            x++;
                    }
                }
            }

            ShowAnimationWindow(form.Handle, showStyle, showTime);
            if (isAutoClose)
            {
                timer.Elapsed += timer_Elapsed;
                timer.Start();
                tr = DateTime.Now;
            }
            count++;
            location = new Point();
            location = form.Location;
            frm = form;
            hover = hoverTime;
        }
        static void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (DateTime.Compare(DateTime.Now, tr.AddSeconds(hover)) >= 0)
                frm.Close();
        }
        /// <summary>
        /// 消息窗口关闭
        /// </summary>
        public static void FrmRightClosed()
        {
            count--;

            if (count == 0)
            {
                x = y = 1;
                points.Clear();
            }
            else
                points.Add(location);
            points.Distinct();
            for (int i = 0; i < points.Count - 1; i++)
            {
                for (int j = 0; j < points.Count - i - 1; j++)
                {
                    if (points[j].X + points[j].Y < points[j + 1].X + points[j + 1].Y)
                    {
                        Point n = points[j];
                        points[j] = points[j + 1];
                        points[j + 1] = n;
                    }
                }
            }
            timer.Stop();
        }
        #endregion

        #region  窗体抖动 
        /// <summary>
        /// 窗体抖动
        /// </summary>
        /// <param name="form">窗体</param>
        /// <param name="number">抖动次数</param>
        /// <param name="speed">抖动速度</param>
        public static void WindowShake(Form form, int number = 30, int speed = 50)
        {
            Random ran = new Random((int)DateTime.Now.Ticks);
            Point point = form.Location;
            for (int i = 0; i < number; i++)
            {
                form.Location = new Point(point.X + ran.Next(8) - 4, point.Y + ran.Next(8) - 4);
                System.Threading.Thread.Sleep(speed);
                form.Location = point;
                System.Threading.Thread.Sleep(speed);
            }

        }
        #endregion

        #region  闪动窗口 

        /// <summary>
        /// 闪动窗口
        /// </summary>
        /// <param name="form">要闪动的窗口</param>
        public static void FlashChatWindow(Form form)
        {
            if (form.WindowState == FormWindowState.Minimized || !form.Focused)
            {
                FlashWindow(form.Handle, true);
            }
        }
        #endregion  

        #region  帖边收缩窗体 
        /// <summary>
        /// 帖边收缩窗体
        /// 用法：在对应窗体timer控件的Tick事件中写代码 int height = this.Height; hide_show(this, ref height, timer1);
        /// </summary>
        /// <param name="frm">要吸附边缘的窗体</param>
        /// <param name="frmHeight">窗体的高度</param>
        /// <param name="timer">定时器控件</param>
        public static void HideShow(Form frm, ref int frmHeight, System.Timers.Timer timer)
        {
            frm.TopMost = true;
            if (frm.WindowState != FormWindowState.Minimized)
            {
                timer.Interval = 100;
                if (Cursor.Position.X > frm.Left - 1 && Cursor.Position.X < frm.Right && Cursor.Position.Y > frm.Top - 1 && Cursor.Position.Y < frm.Bottom)
                {
                    if (frm.Top <= 0 && frm.Left > 5 && frm.Left < Screen.PrimaryScreen.WorkingArea.Width - frm.Width)
                    {
                        frm.Top = 0;
                    }
                    else if (frm.Left <= 0)
                    {
                        frm.Left = 0;
                    }
                    else if (frm.Left + frm.Width > Screen.PrimaryScreen.WorkingArea.Width)
                    {
                        frm.Left = Screen.PrimaryScreen.WorkingArea.Width - frm.Width;
                    }
                    else
                    {
                        if (frmHeight > 0)
                        {
                            frm.Height = frmHeight;
                            frmHeight = 0;
                        }
                    }
                }
                else
                {
                    if (frmHeight < 1)
                    {
                        frmHeight = frm.Height;
                    }
                    if (frm.Top <= 4 && frm.Left > 5 && frm.Left < Screen.PrimaryScreen.WorkingArea.Width - frm.Width)
                    {
                        frm.Top = 3 - frm.Height;
                        if (frm.Left <= 4)
                        {
                            frm.Left = -5;
                        }
                        else if (frm.Left + frm.Width >= Screen.PrimaryScreen.WorkingArea.Width - 4)
                        {
                            frm.Left = Screen.PrimaryScreen.WorkingArea.Width - frm.Width + 5;
                        }
                    }
                    else if (frm.Left <= 4)
                    {
                        frm.Left = 3 - frm.Width;
                    }
                    else if (frm.Left + frm.Width >= Screen.PrimaryScreen.WorkingArea.Width - 4)
                    {
                        frm.Left = Screen.PrimaryScreen.WorkingArea.Width - 3;
                    }
                }
            }
        }
        #endregion

        #region  无标题栏窗体移动（调用Api方式） 

        private const int WM_SYSCOMMAND = 0x0112;
        private const int SC_MOVE = 0xF010;
        private const int HTCAPTION = 0x0002;
        /// <summary>
        /// 无标题栏窗体移动，在窗体的_MouseDown中调用此方法。（调用Api方式）
        /// </summary>
        /// <param name="form">窗体</param>
        public static void FrmNonMove(Form form)
        {
            ReleaseCapture();
            SendMessage(form.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }
        #endregion

        #region  无标题栏可以移动窗体（事件结合方式） 
        private static bool isMouseDown = false;
        private static Point FormLocation;     //form的location
        private static Point mouseOffset;      //鼠标的按下位置
        /// <summary>
        /// 无标题栏可以移动窗体，配合Control_MouseMove方法和Control_MouseUp方法一同使用
        /// </summary>
        /// <param name="form">当前窗体</param>
        /// <param name="e">MouseEventArgs</param>
        public static void Control_MouseDown(Form form, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMouseDown = true;
                FormLocation = form.Location;
                mouseOffset = Control.MousePosition;
            }
        }
        /// <summary>
        /// 无标题栏可以移动窗体，配合Control_MouseMove方法和Control_MouseDown方法一同使用
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">MouseEventArgs</param>
        public static void Control_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
        }
        /// <summary>
        /// 无标题栏可以移动窗体，配合Control_MouseUp方法和Control_MouseDown方法一同使用
        /// </summary>
        /// <param name="form">当前窗体</param>
        /// <param name="e">MouseEventArgs</param>
        public static void Control_MouseMove(Form form, MouseEventArgs e)
        {
            int _x = 0;
            int _y = 0;
            if (isMouseDown)
            {
                Point pt = Control.MousePosition;
                _x = mouseOffset.X - pt.X;
                _y = mouseOffset.Y - pt.Y;

                form.Location = new Point(FormLocation.X - _x, FormLocation.Y - _y);
            }
        }
        #endregion

        #region  鼠标穿透窗体 
        private const uint WS_EX_LAYERED = 0x80000;
        private const int WS_EX_TRANSPARENT = 0x20;
        private const int GWL_STYLE = (-16);
        private const int GWL_EXSTYLE = (-20);
        private const int LWA_ALPHA = 0;




        /// <summary>
        /// 声明委托类
        /// </summary>
        /// <param name="MsgStr"></param>
        public delegate void FormCt();
        /// <summary>
        /// 定义委托
        /// </summary>
        public static FormCt Ct;
        /// <summary> 
        /// 设置窗体具有鼠标穿透效果 
        /// </summary> 
        public static void SetPenetrate(Form form)
        {
            if (form.InvokeRequired)
            {
                form.Invoke(Ct);
            }
            else
            {
                form.TopMost = true;
                GetWindowLong(form.Handle, GWL_EXSTYLE);
                SetWindowLong(form.Handle, GWL_EXSTYLE, WS_EX_TRANSPARENT | WS_EX_LAYERED);
                SetLayeredWindowAttributes(form.Handle, 0, 100, LWA_ALPHA);
            }
        }
        #endregion

        //#region  为指定程序创建桌面快捷方式 
        ///// <summary>       
        ///// 为指定程序创建桌面快捷方式(当前用户有效,传参数时按参数注释传入)
        ///// </summary>
        ///// <param name="filePath">程序路径</param>
        ///// <param name="name">快捷名称</param>
        ///// <param name="windowStyle">应用程序窗口状态(只能是1,3,7中一个):普通1,最大化3,最小化7</param>
        ///// <param name="iconLocation">快捷方式图标地址(传以ico后缀名结束的图标的路径,空为程序当前图标不变)</param>
        ///// <param name="hotKey">快捷键</param>
        ///// <param name="description">快捷方式描述消息</param>
        ///// <param name="startArguments">启动参数(无则为空,多个参数则用空格隔开)</param>
        ///// <returns>成功返回true,失败返回false</returns>
        //public static bool CreateDeskTopLink(string filePath, string name = "", int windowStyle = 1, string iconLocation = "", string hotKey = "", string description = "", string startArguments = "")
        //{
        //    if (!System.IO.File.Exists(filePath))
        //        return false;
        //    if (windowStyle != 1 && windowStyle != 3 && windowStyle != 7)
        //        return false;
        //    if (iconLocation != string.Empty && !System.IO.File.Exists(iconLocation))
        //        return false;
        //    string deskTop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        //    string fileNameWithoutExtension = "";
        //    if (name == "")
        //        fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(filePath);
        //    else
        //        fileNameWithoutExtension = name;
        //    string fileDirectory = System.IO.Path.GetDirectoryName(filePath);
        //    string shortCutFullName = deskTop + "\\" + fileNameWithoutExtension + ".lnk";

        //    WshShell shell = new WshShell();
        //    //快捷键方式创建的位置、名称
        //    IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortCutFullName);
        //    //目标文件
        //    shortcut.TargetPath = filePath;
        //    //启动参数
        //    shortcut.Arguments = startArguments;
        //    //工作目录
        //    shortcut.WorkingDirectory = fileDirectory;
        //    //快捷图标描述消息,若为空则使用自定义信息
        //    shortcut.Description = description == string.Empty ?
        //        (fileNameWithoutExtension + "\r\n" + fileDirectory) : description;
        //    //快捷键
        //    shortcut.Hotkey = hotKey;
        //    //目标应用程序的窗口状态分为普通(1)、最大化(3)、最小化(7)
        //    shortcut.WindowStyle = windowStyle;
        //    //如果不指定图标,则默认以程序当前图标为快捷方式的图标
        //    if (iconLocation != string.Empty)
        //        shortcut.IconLocation = iconLocation;
        //    //必须调用保存快捷才成创建成功
        //    shortcut.Save();
        //    return true;
        //}
        //#endregion

        #region  重新定位MessageBox位置 


        /// <summary>
        /// 重新定位MessageBox位置（在 MessageBox.Show 之前调用这个方法，并确保 caption 参数不能为空，因为 title 参数必须等于 caption 参数）
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <param name="title">标题</param>
        /// <param name="repaint">是否重画</param>
        public static void MoveMsgBox(int x, int y, string title, bool repaint = true)
        {
            Thread thr = new Thread(() => // create a new thread
            {
                IntPtr msgBox = IntPtr.Zero;
                //虽然没有消息，通过findwindow返回零IntPtr。
                while ((msgBox = FindWindow(IntPtr.Zero, title)) == IntPtr.Zero) ;
                // while循环后，本是你的消息处理
                Rectangle r = new Rectangle();
                GetWindowRect(msgBox, out r); // 获取消息框的矩形
                MoveWindow(msgBox /* 消息框的处理 */, x, y,
                r.Width - r.X /* 最初的消息框的宽度 */,
                r.Height - r.Y /* 最初的消息框的高度 */,
                repaint /* 如果TRUE，消息框重画 */);
            });
            thr.Start(); // 启动线程
        }
        #endregion

        #region  隐藏光标 


        /// <summary>
        /// 隐藏光标,将tabstop属性设为false。
        /// </summary>
        /// <param name="con">控件</param>
        public static void HideControlCaret(Control con)
        {
            HideCaret(con.Handle);
        }
        #endregion

        #region  窗体边框阴影效果 
        //变量申明
        const int CS_DropSHADOW = 0x20000;
        const int GCL_STYLE = (-26);
        /// <summary>
        /// 设置窗体边框阴影效果
        /// </summary>
        /// <param name="form">窗体对象</param>
        public static void SetShadow(Form form)
        {
            SetClassLong(form.Handle, GCL_STYLE, GetClassLong(form.Handle, GCL_STYLE) | CS_DropSHADOW); //API函数加载，实现窗体边框阴影效果
        }
        #endregion

        #region 全屏时notifyIcon依然能够弹出ShowBalloonTip 
        /// <summary>
        /// 全屏时notifyIcon依然能够弹出ShowBalloonTip 
        /// </summary>
        /// <param name="handle">句柄</param>
        /// <param name="notifyIcon">NotifyIcon对象</param>
        /// <param name="title">提示标题</param>
        /// <param name="str">提示内容</param>
        /// <param name="ico">提示图标</param>
        public static void ShowBalloonTip(IntPtr handle, NotifyIcon notifyIcon, string title, string str, ToolTipIcon ico)
        {
            if (handle != GetForegroundWindow())
            {
                SetForegroundWindow(handle);
                notifyIcon.ShowBalloonTip(1500, title, str, ico);
            }
        }
        #endregion

        #region  最小化所有活动窗口显示桌面 
        /// <summary>
        /// 最小化所有活动窗口显示桌面
        /// </summary>
        public static void AllWindowsMin()
        {

            //所有窗体最小化，显示桌面
            Type oleType = Type.GetTypeFromProgID("Shell.Application");
            object oleObject = System.Activator.CreateInstance(oleType);
            oleType.InvokeMember("ToggleDesktop", BindingFlags.InvokeMethod, null, oleObject, null);
        }
        #endregion

        #region  程序休眠 
        /// <summary>
        /// 程序休眠
        /// </summary>
        /// <param name="delaySeconds">休眠秒数</param>
        /// <returns>休眠完成返回true</returns>
        public static bool AppDelay(int delaySeconds)
        {
            DateTime now = DateTime.Now;
            int s;
            do
            {
                TimeSpan spand = DateTime.Now - now;
                s = spand.Seconds;
                Application.DoEvents();
            }
            while (s < delaySeconds);
            return true;
        }
        #endregion
    }
}
