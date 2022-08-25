using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperFramework.SuperGraphics
{
    /// <summary>
    /// 日 期:2015-11-26
    /// 作 者:不良帥
    /// 描 述:画图辅助类
    /// </summary>
    public class DrawHelper
    {
        /// <summary>
        /// 画时钟（在定时器中调用此方法）
        /// </summary>
        /// <param name="panel1">容器</param>
        /// <param name="backColor">背景色</param>
        private static void DrawClock(System.Windows.Forms.Panel panel1, Color backColor)
        {
            //讲解
            /*
             *一个圆是360度，刻度盘共60个，每个刻度为6度
             * 一小时=60分，一分钟=60秒，所以每秒和每分钟转动的角度为(分/秒)*6
             * 没小时的刻度为360/12=30，小时的角度=时*30*（分钟角度/360*30）
            */
            DrawBottonBg(Math.Abs(DateTime.Now.Hour - 12) * 30 + DateTime.Now.Minute * 6 * 30 / 360, DateTime.Now.Minute * 6, DateTime.Now.Second * 6, panel1, backColor);
        }
        /// <summary>
        /// 更新画布
        /// </summary>
        /// <param name="Hour">小时转动的角度</param>
        /// <param name="Minute">分钟转动的角度</param>
        /// <param name="Second">秒钟转动的角度</param>
        /// <param name="panel1">容器</param>
        /// <param name="backColor">背景色</param>
        private static void DrawBottonBg(int Hour, int Minute, int Second, System.Windows.Forms.Panel panel1,Color backColor)
        {
            try
            {
                PointF Center = new PointF(panel1.Width / 2, 185);
                #region 初始化画布
                //读取背景图
                //添加一块画布
                Bitmap bmp = new Bitmap(panel1.Width, panel1.Height);
                Graphics g = Graphics.FromImage(bmp);
                /*-----清除Graphics-----*/
                g.Clear(backColor);
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                #endregion

                /*-----时钟表盘-----*/
                #region 时钟表盘
                g.DrawImage(Resources.时钟表盘, new Rectangle(0, 0, panel1.Width, panel1.Height), new Rectangle(0, 0, panel1.Width, panel1.Height), GraphicsUnit.Pixel);
                #endregion

                /*-----画时针-----*/
                #region  画时针
                RectangleF re = new RectangleF(panel1.Width / 2 - Resources.时针.Width / 2, panel1.Height / 2 - Resources.时针.Height + 14, Resources.时针.Width, Resources.时针.Height);
                //移动到矩形中心点坐标
                g.TranslateTransform(Center.X, Center.Y);
                //旋转指定的角度
                g.RotateTransform(Hour);
                //准备画图  旋转之后，坐标变负
                g.TranslateTransform(-Center.X, -Center.Y);
                //在矩形中绘制图形
                g.DrawImage(Resources.时针, re);
                //重置坐标原点
                g.ResetTransform();
                #endregion

                /*-----画分针-----*/
                #region  画分针
                re = new RectangleF(panel1.Width / 2 - Resources.分针.Width / 2, panel1.Height / 2 - Resources.分针.Height + 14, Resources.分针.Width, Resources.分针.Height);
                //移动到矩形中心点坐标
                g.TranslateTransform(Center.X, Center.Y);
                //旋转指定的角度
                g.RotateTransform(Minute);
                //准备画图  旋转之后，坐标变负
                g.TranslateTransform(-Center.X, -Center.Y);
                //在矩形中绘制图形
                g.DrawImage(Resources.分针, re);
                //重置坐标原点
                g.ResetTransform();
                #endregion

                /*-----画秒针-----*/
                #region  画分针
                re = new RectangleF(panel1.Width / 2 - Resources.秒针.Width / 2, panel1.Height / 2 - Resources.秒针.Height + 14 + 33, Resources.秒针.Width, Resources.秒针.Height);
                //移动到矩形中心点坐标
                g.TranslateTransform(Center.X, Center.Y);
                //旋转指定的角度
                g.RotateTransform(Second);
                //准备画图  旋转之后，坐标变负
                g.TranslateTransform(-Center.X, -Center.Y);
                //在矩形中绘制图形
                g.DrawImage(Resources.秒针, re);
                //重置坐标原点
                g.ResetTransform();
                #endregion
                g.Dispose();
                panel1.CreateGraphics().DrawImage(bmp, 0, 0);
                bmp.Dispose();
            }
            catch (Exception ex)
            {
            }
        }

    }
}
