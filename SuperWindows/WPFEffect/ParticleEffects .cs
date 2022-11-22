using System;
using System.Collections.Generic;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Media;

namespace SuperFramework.SuperWindows.WPFEffect
{
    /// <summary>
    /// 日期：2019-01-11
    /// 作者：不良帥
    /// 说明：粒子特效
    /// </summary>
    public static class ParticleEffects
    {
        #region 变量
        /// <summary>
        /// 定时器
        /// </summary>
        private static DispatcherTimer updateTimer;
        /// <summary>
        /// 容器
        /// </summary>
        private static Canvas Cav = null;
        /// <summary>
        /// 粒子颜色
        /// </summary>
        static Color c1 = Colors.White;
        /// <summary>
        /// 连线颜色
        /// </summary>
        static Color c2 = Colors.Aqua;
        /// <summary>
        /// 运行速度
        /// </summary>
        static int runSpend = 5;
        /// <summary>
        /// 最大连线数
        /// </summary>
        static int maxLine = 8000;
        /// <summary>
        /// 粒子总数
        /// </summary>
        static int count = 100;
        /// <summary>
        /// 是否使用鼠标事件
        /// </summary>
        static bool useMouseEvent = false;
        /// <summary>
        /// 粒子数组
        /// </summary>
        static readonly List<GrainBase> grains = new();
        /// <summary>
        /// 鼠标粒子信息
        /// </summary>
        static readonly GrainBase mousePoint = new();
        /// <summary>
        /// 随机数
        /// </summary>
        static readonly Random rand = new();
        #endregion

        /// <summary>
        /// 开始绘制
        /// </summary>
        /// <param name="vas">Canvas控件对象</param>
        /// <param name="grainColor">粒子颜色</param>
        /// <param name="lineColor">连线颜色</param>
        /// <param name="spend">粒子运行速度</param>
        /// <param name="maxLineLength">粒子连线最大距离</param>
        /// <param name="grainCount">粒子数量</param>
        /// <param name="interval">粒子变换速度</param>
        /// <param name="isUseMouseEvent">是否使用鼠标事件：鼠标点击连线</param>
        public static void StartGrain(Canvas vas, Color grainColor, Color lineColor, int spend = 5, int maxLineLength = 10000, int grainCount = 100, double interval = 25, bool isUseMouseEvent = false)
        {

            Cav = vas;
            c1 = grainColor;
            c2 = lineColor;
            runSpend = spend;
            maxLine = maxLineLength;
            count = grainCount;
            useMouseEvent = isUseMouseEvent;
            updateTimer = new DispatcherTimer();
            updateTimer.Tick += (o, a) =>
            {
                // new Thread(AyKeyFrame) { IsBackground = true }.Start();
                AyKeyFrame();
            };
            updateTimer.Interval = TimeSpan.FromMilliseconds(interval);//1000 / 60
            Loaded();
            updateTimer.Start();
            if (isUseMouseEvent)
            {
                Cav.MouseDown += Cav_MouseDown;
                Cav.MouseLeave += Cav_MouseLeave;
                Cav.MouseMove += Cav_MouseMove;
            }

        }
        /// <summary>
        /// 停止绘制
        /// </summary>
        public static void StopGrain()
        {

            Cav = null;
            if (updateTimer != null)
            {
                updateTimer.Stop();
                //updateTimer.Tick = null;
                updateTimer = null;
            }
            if (useMouseEvent)
            {
                Cav.MouseDown -= Cav_MouseDown;
                Cav.MouseLeave -= Cav_MouseLeave;
                Cav.MouseMove -= Cav_MouseMove;
            }
        }

        /// <summary>
        /// 创建粒子对象
        /// </summary>
        private static void Loaded()
        {
            mousePoint.max = maxLine;
            grains.Add(mousePoint);
            //// 添加粒子
            //// x，y为粒子坐标，xa, ya为粒子xy轴加速度，max为连线的最大距离     
            for (int i = 0; i < count; i++)
                grains.Add(new GrainBase() { x = rand.NextDouble() * Cav.Width, y = rand.NextDouble() * Cav.Height, xa = rand.NextDouble() * runSpend - 1, ya = rand.NextDouble() * runSpend - 1, max = maxLine });
        }
        /// <summary>
        /// 粒子运动
        /// </summary>
        private static void AyKeyFrame()
        {
            Cav.Children.Clear();
            for (int i = 0; i < grains.Count; i++)
            {
                GrainBase dot = grains[i];
                if (dot.x == null || dot.y == null) continue;

                #region 创建碰撞粒子
                // 粒子位移
                dot.x += dot.xa;
                dot.y += dot.ya;
                // 遇到边界将加速度反向
                dot.xa *= (dot.x.Value > Cav.ActualWidth || dot.x.Value < 0) ? -1 : 1;
                dot.ya *= (dot.y.Value > Cav.ActualHeight || dot.y.Value < 0) ? -1 : 1;
                // 绘制点

                Ellipse elip = new() { Width = 2, Height = 2 };
                Canvas.SetLeft(elip, dot.x.Value - 0.5);
                Canvas.SetTop(elip, dot.y.Value - 0.5);
                elip.Fill = new SolidColorBrush(c1);
                Cav.Children.Add(elip);
                #endregion

                //判断是不是最后一个，就不用两两比较了
                int endIndex = i + 1;
                if (endIndex == grains.Count) continue;
                for (int j = endIndex; j < grains.Count; j++)
                {
                    GrainBase d2 = grains[j];
                    double xc = dot.x.Value - d2.x.Value;
                    double yc = dot.y.Value - d2.y.Value;
                    // 两个粒子之间的距离
                    double dis = xc * xc + yc * yc;
                    // 距离比
                    double ratio;
                    // 如果两个粒子之间的距离小于粒子对象的max值，则在两个粒子间画线
                    if (dis < d2.max)
                    {
                        // 计算距离比
                        ratio = (d2.max - dis) / d2.max;
                        Line line = new();
                        double opacity = ratio + 0.2;
                        if (opacity > 1) { opacity = 1; }
                        byte ar = (byte)(opacity * 255);
                        line.Stroke = new SolidColorBrush(c2);//Color.FromArgb(ar, 255, 255, 255)
                        line.StrokeThickness = ratio / 2;
                        line.X1 = dot.x.Value;
                        line.Y1 = dot.y.Value;
                        line.X2 = d2.x.Value;
                        line.Y2 = d2.y.Value;
                        Cav.Children.Add(line);
                    }
                }
            }
        }

        #region 鼠标事件
        /// <summary>
        /// 鼠标在粒子中按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void Cav_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
            {
                GrainBase gb = new();
                var ui = e.GetPosition(Cav);
                gb.x = ui.X;
                gb.y = ui.Y;
                gb.xa = rand.NextDouble() * 2 - 1;
                gb.ya = rand.NextDouble() * 2 - 1;
                gb.max = 8000;
                grains.Add(gb);
            }
        }
        /// <summary>
        /// 鼠标进入粒子事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Cav_MouseMove(object sender, MouseEventArgs e)
        {
            var ui = e.GetPosition(Cav);
            mousePoint.x = ui.X;
            mousePoint.y = ui.Y;
        }
        /// <summary>
        /// 鼠标离开粒子事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Cav_MouseLeave(object sender, MouseEventArgs e)
        {
            mousePoint.x = null;
            mousePoint.y = null;
        }
        #endregion

        #region 粒子信息

        /// <summary>
        /// 粒子信息
        /// </summary>
        internal class GrainBase
        {
            /// <summary>
            /// 粒子x坐标
            /// </summary>
            public double? x { get; set; }
            /// <summary>
            /// 粒子y坐标
            /// </summary>
            public double? y { get; set; }
            /// <summary>
            /// 粒子x轴加速度
            /// </summary>
            public double xa { get; set; }
            /// <summary>
            /// 粒子y轴加速度
            /// </summary>
            public double ya { get; set; }
            /// <summary>
            /// 连线的最大距离
            /// </summary>
            public double max { get; set; }
        }
        #endregion
    }

}
