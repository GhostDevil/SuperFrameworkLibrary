using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Data;
using System;

namespace SuperFramework
{
    /// <summary>
    /// <para>日期：2016-08-10</para>
    /// <para>作者：不良帥</para>
    /// <para>说明：ReportImage 主要用于GDI+绘制统计图
    /// 目前能绘制 平面柱状图、平面折线图、平面扇形图
    /// 立体柱状图、立体折线图、立体扇形图
    /// 柱状百分比图、巴士图、一维雷达图、多维雷达图</para>
    /// </summary>
    public static class ReportImage
    {
        #region 说明图
        /// <summary>
        /// 给统计图画说明图
        /// </summary>
        /// <param name="CaptionNames">名字</param>
        /// <param name="g">所属画布</param>
        /// <param name="x">x轴位置</param>
        /// <param name="TjType">统计图类型（折线图，柱状图，扇形图）</param>
        /// <param name="colors">颜色集合</param>
        private static void CreateCaption(List<string> CaptionNames, Graphics g, int x, int y, string TjType, List<Color> colors)
        {
            int tempx = x;
            int tempy = y;
            int temwidth = 0, temheight = 0;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int width = 100;
            int height = 20 * CaptionNames.Count;
            Font font = new Font("宋体", 10, FontStyle.Regular);
            Color color = new Color();
            int colorindex = 0;
            if (TjType == "折线图")
            {
                foreach (string captionname in CaptionNames)
                {
                    g.DrawLine(new Pen(colors[colorindex]), x + 10, y + 12, x + 30, y + 12);
                    g.DrawString(captionname, font, new SolidBrush(colors[colorindex]), x + 40, y + 5);
                    y += 20;
                    colorindex++;
                }
                temheight = height;
                temwidth = width;
                color = Color.Black;
            }
            else if (TjType == "柱状图")
            {

                foreach (string captionname in CaptionNames)
                {
                    g.FillRectangle(new SolidBrush(colors[colorindex]), x + 10, y + 7, 20, 10);
                    g.DrawString(captionname, font, new SolidBrush(colors[colorindex]), x + 40, y + 5);
                    y += 20;
                    colorindex++;
                }
                temheight = height;
                temwidth = width;
                color = Color.Black;
            }
            else if (TjType == "扇形图")
            {
                foreach (string captionname in CaptionNames)
                {
                    g.FillRectangle(new SolidBrush(colors[colorindex]), x + 10, y + 7, 10, 5);
                    g.DrawString(captionname, font, new SolidBrush(colors[colorindex]), x + 35, y + 5);
                    colorindex++;
                    y += 20;
                }
                temheight = height;
                temwidth = width;
                color = Color.Black;
            }
            else if (TjType == "巴士图")
            {
                width = 0;
                temheight = 20;
                int heindex = 1;
                foreach (string captionname in CaptionNames)
                {
                    if (width > 300)
                    {
                        heindex++;
                        temheight = 20 * heindex;
                        y += 20;
                        x = tempx;
                        temwidth = width;
                        width = 0;
                    }
                    else
                    {
                        width += (captionname.Length + 1) * 12 + 15;
                        if (temheight == 20)
                        {
                            temwidth = width;
                        }
                    }
                    g.FillRectangle(new SolidBrush(colors[colorindex]), x + 10, y + 7, 10, 5);
                    g.DrawString(captionname, font, new SolidBrush(colors[colorindex]), x + 25, y + 5);
                    colorindex++;
                    x = x + (captionname.Length + 1) * 12 + 15;
                }
                temwidth += 10;
                color = Color.Black;
            }
            else if (TjType == "雷达图")
            {
                width = 0;
                temheight = 20;
                int heindex = 1;
                foreach (string captionname in CaptionNames)
                {
                    if (width > 300)
                    {
                        heindex++;
                        temheight = 20 * heindex;
                        y += 20;
                        x = tempx;
                        temwidth = width;
                        width = 0;
                    }
                    else
                    {
                        width += (captionname.Length + 1) * 12 + 15;
                        if (temheight == 20)
                        {
                            temwidth = width;
                        }
                    }
                    g.FillRectangle(new SolidBrush(colors[colorindex]), x + 10, y + 7, 10, 5);
                    g.DrawString(captionname, font, new SolidBrush(colors[colorindex]), x + 25, y + 5);
                    colorindex++;
                    x = x + (captionname.Length + 1) * 12 + 15;
                }
                temwidth += 10;
                color = Color.FromArgb(82, 137, 247);
            }
            g.DrawRectangle(new Pen(color), tempx, tempy, temwidth, temheight);

        }
        #endregion

        #region 柱状图
        /// <summary>
        /// 画柱状图
        /// </summary>
        /// <param name="dt">数据源（名字、数据1、数据n）</param>
        /// <param name="title">统计图标题</param>
        /// <param name="linecolor">网格线颜色</param>
        /// <param name="x_name">x轴注释</param>
        /// <param name="y_name">y轴注释</param>
        /// <param name="T_width">柱状图的宽</param>
        /// <returns></returns>
        public static Bitmap CreateColumnarity(DataTable dt, string title, Color linecolor, string x_name, string y_name, int T_width)
        {
            float maxValue = 0;
            float b = 0;
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                foreach (DataRow row in dt.Rows)
                {
                    float a = Convert.ToSingle(row[i].ToString());
                    if (a > b)
                    {
                        maxValue = a; b = a;
                    }
                    else
                    {
                        maxValue = b;
                    }
                }
            }
            if ((maxValue % 10) != 0)
                maxValue = 10 * ((int)maxValue / 10) + 10;
            maxValue /= 5;
            List<float> y_value = new List<float>();
            for (int i = 1; i <= 5; i++)
            {
                y_value.Add(maxValue * i);
            }
            List<string> x_zhi = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                x_zhi.Add(row[0].ToString());
            }
            int x = 50;
            int y = 70;
            int x_pitch = 15 + T_width * (dt.Columns.Count - 1);
            int y_pitch = 35;
            int T_place = x_pitch - T_width * (dt.Columns.Count - 1) / 2;
            int height = y_value.Count * y_pitch + 300;
            int width = x_zhi.Count * x_pitch + 250;
            int tempheight = 100 + 20 * dt.Rows.Count;
            if (height < tempheight)
            {
                height = tempheight;
            }
            int x2 = width - 150;
            int y2 = height - 150;
            int datay2 = y2;
            int datax = x;
            List<Color> colors = new List<Color>();
            Random ran = new Random();
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                colors.Add(Color.FromArgb(200, ran.Next(0, 200), ran.Next(0, 200), ran.Next(0, 200)));
            }
            List<string> capname = new List<string>();
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                capname.Add(dt.Columns[i].ToString());
            }

            Bitmap image = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(image);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.White);
            Font font1 = new Font("Arial", 10, FontStyle.Regular);
            Font font2 = new Font("宋体", 10, FontStyle.Regular);
            Font font3 = new Font("宋体", 10, FontStyle.Regular);
            Brush brushe1 = new SolidBrush(Color.Black);
            g.DrawString(title, font2, brushe1, width / 5, 30);
            g.DrawRectangle(new Pen(Color.Black), 0, 0, image.Width - 1, image.Height - 1);

            Pen pen1 = new Pen(linecolor, 1);
            Pen pen2 = new Pen(linecolor, 2);
            foreach (string xtitle in x_zhi)
            {
                x += x_pitch;
                g.DrawLine(pen1, x, y, x, y2);
                StringFormat format = new StringFormat();
                format.FormatFlags = StringFormatFlags.DirectionVertical | StringFormatFlags.DirectionRightToLeft;
                g.DrawString(xtitle, font3, brushe1, x + 5, y2, format);
            }
            x = datax;
            g.DrawLine(pen2, x, y, x, y2);
            g.DrawString(x_name, font3, brushe1, x2, y2);
            g.DrawLine(pen2, x, y2, x2, y2);
            g.DrawString(y_name, font3, brushe1, x - 40, y);
            y2 -= y_pitch;
            foreach (float ytitle in y_value)
            {
                g.DrawLine(pen1, x, y2, x2, y2);
                g.DrawString(ytitle.ToString(), font3, brushe1, x - 30, y2 - 5);
                y2 -= y_pitch;
            }

            int colorindex = 0;
            int dataindex = 0;
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                x = datax + T_place;
                foreach (DataRow row in dt.Rows)
                {
                    string data = row[i].ToString();
                    float gao = Convert.ToSingle(data) / y_value[0] * y_pitch;
                    g.FillRectangle(new SolidBrush(colors[colorindex]), x + T_width * dataindex, datay2 - gao, T_width, gao);
                    g.DrawString(data, font3, new SolidBrush(colors[colorindex]), x + T_width * dataindex - 10, datay2 - gao - 20);
                    x += x_pitch;
                }
                colorindex++;
                dataindex++;
            }

            CreateCaption(capname, g, width - 105, y, "柱状图", colors);
            return image;
        }
        #endregion

        #region 立体柱状图
        /// <summary>
        /// 画立体柱状图
        /// </summary>
        /// <param name="dt">数据源（名字、数据1、数据n）</param>
        /// <param name="title">统计图标题</param>
        /// <param name="pathcolor">轴颜色</param>
        /// <param name="x_name">x轴注释</param>
        /// <param name="y_name">y轴注释</param>
        /// <param name="T_width">柱状图的宽</param>
        /// <returns></returns>
        public static Bitmap CreateColumnarity3D(DataTable dt, string title, Color pathcolor, string x_name, string y_name, int T_width)
        {
            float maxValue = 0;
            float b = 0;
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                foreach (DataRow row in dt.Rows)
                {
                    float a = Convert.ToSingle(row[i].ToString());
                    if (a > b)
                    {
                        maxValue = a; b = a;
                    }
                    else
                    {
                        maxValue = b;
                    }
                }
            }
            if ((maxValue % 10) != 0)
                maxValue = 10 * ((int)maxValue / 10) + 10;
            maxValue /= 5;
            List<float> y_value = new List<float>();
            for (int i = 1; i <= 5; i++)
            {
                y_value.Add(maxValue * i);
            }

            List<string> x_zhi = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                x_zhi.Add(row[0].ToString());
            }
            int x = 50;
            int y = 70;
            int x_pitch = 20 + T_width * (dt.Columns.Count - 1);
            int y_pitch = 35;
            int T_place = x_pitch - T_width * (dt.Columns.Count - 1) / 2;
            int height = y_value.Count * y_pitch + 300;
            int width = x_zhi.Count * x_pitch + 250;
            int tempheight = 100 + 20 * dt.Rows.Count;
            if (height < tempheight)
            {
                height = tempheight;
            }
            int x2 = width - 150;
            int y2 = height - 150;
            int datay2 = y2;
            int datax = x;
            List<Color> colors = new List<Color>();
            Random ran = new Random();
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                colors.Add(Color.FromArgb(ran.Next(0, 200), ran.Next(0, 200), ran.Next(0, 200)));
            }

            List<Color> colors1 = new List<Color>();
            List<Color> colors2 = new List<Color>();
            foreach (Color color in colors)
            {
                int r1 = (int)color.R - 30;
                int r2 = (int)color.R - 60;
                int g1 = (int)color.G - 30;
                int g2 = (int)color.G - 60;
                int b1 = (int)color.B - 30;
                int b2 = (int)color.B - 60;
                if (r1 < 0)
                    r1 = 0;
                if (g1 < 0)
                    g1 = 0;
                if (b1 < 0)
                    b1 = 0;
                if (r2 < 0)
                    r2 = 0;
                if (g2 < 0)
                    g2 = 0;
                if (b2 < 0)
                    b2 = 0;
                colors1.Add(Color.FromArgb(r1, g1, b1));
                colors2.Add(Color.FromArgb(r2, g2, b2));
            }
            List<string> capname = new List<string>();
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                capname.Add(dt.Columns[i].ToString());
            }

            Bitmap image = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(image);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.White);
            Font font1 = new Font("Arial", 10, FontStyle.Regular);
            Font font2 = new Font("宋体", 10, FontStyle.Regular);
            Font font3 = new Font("宋体", 10, FontStyle.Regular);
            Brush brushe1 = new SolidBrush(Color.Black);
            g.DrawString(title, font2, brushe1, width / 5, 30);
            g.DrawRectangle(new Pen(Color.Black), 0, 0, image.Width - 1, image.Height - 1);

            foreach (string xtitle in x_zhi)
            {
                x += x_pitch;
                StringFormat format = new StringFormat();
                format.FormatFlags = StringFormatFlags.DirectionVertical | StringFormatFlags.DirectionRightToLeft;
                g.DrawString(xtitle, font3, brushe1, x + 5, y2, format);
            }
            x = datax;
            GraphicsPath path = new GraphicsPath(new Point[] { new Point(x, y), new Point(x + 10, y - 15), new Point(x + 10, y2 - 15), new Point(x, y2), new Point(x, y) }, new byte[] { (byte)PathPointType.Start, (byte)PathPointType.Line, (byte)PathPointType.Line, (byte)PathPointType.Line, (byte)PathPointType.Line });
            g.FillPath(new SolidBrush(pathcolor), path);
            g.DrawPath(new Pen(pathcolor), path);
            g.DrawString(x_name, font3, brushe1, x2, y2);

            path = new GraphicsPath(new Point[] { new Point(x, y2), new Point(x + 10, y2 - 15), new Point(x2 + 10, y2 - 15), new Point(x2, y2), new Point(x, y2) }, new byte[] { (byte)PathPointType.Start, (byte)PathPointType.Line, (byte)PathPointType.Line, (byte)PathPointType.Line, (byte)PathPointType.Line });
            g.FillPath(new SolidBrush(pathcolor), path);
            g.DrawPath(new Pen(pathcolor), path);
            g.DrawString(y_name, font3, brushe1, x - 40, y);
            y2 -= y_pitch;
            foreach (float ytitle in y_value)
            {
                g.DrawString(ytitle.ToString(), font3, brushe1, x - 30, y2 - 10);
                y2 -= y_pitch;
            }

            x = datax + T_place;
            foreach (DataRow row in dt.Rows)
            {
                int colorindex = 0;
                int dataindex = 0;
                List<string> datas = new List<string>();
                List<float> gaos = new List<float>();
                for (int i = 1; i < dt.Columns.Count; i++)
                {
                    path.Dispose();

                    string data = row[i].ToString();
                    float gao = Convert.ToSingle(data) / y_value[0] * y_pitch;

                    //侧面
                    PointF point1_2 = new PointF(Convert.ToSingle(x + T_width * dataindex + T_width + 10), Convert.ToSingle(datay2 - gao + 0.1 - 15));
                    PointF point2_2 = new PointF(Convert.ToSingle(x + T_width * dataindex + T_width), Convert.ToSingle(datay2 - gao + 0.1));
                    PointF point3_2 = new PointF(Convert.ToSingle(x + T_width * dataindex + T_width), Convert.ToSingle(datay2));
                    PointF point4_2 = new PointF(Convert.ToSingle(x + T_width * dataindex + T_width + 10), Convert.ToSingle(datay2 + 0.1 - 15));
                    PointF point5_2 = new PointF(Convert.ToSingle(x + T_width * dataindex + T_width + 10), Convert.ToSingle(datay2 - gao + 0.1 - 15));

                    path = new GraphicsPath(new PointF[] { point1_2, point2_2, point3_2, point4_2, point5_2 }, new byte[] { (byte)PathPointType.Start, (byte)PathPointType.Line, (byte)PathPointType.Line, (byte)PathPointType.Line, (byte)PathPointType.Line });

                    g.FillPath(new SolidBrush(colors2[colorindex]), path);

                    //顶部
                    PointF point1_1 = new PointF(Convert.ToSingle(x + T_width * dataindex), Convert.ToSingle(datay2 - gao + 0.1));
                    PointF point2_1 = new PointF(Convert.ToSingle(x + T_width * dataindex + T_width), Convert.ToSingle(datay2 - gao + 0.1));
                    PointF point3_1 = new PointF(Convert.ToSingle(x + T_width * dataindex + T_width + 10), Convert.ToSingle(datay2 - gao + 0.1 - 15));
                    PointF point4_1 = new PointF(Convert.ToSingle(x + T_width * dataindex + 10), Convert.ToSingle(datay2 - gao + 0.1 - 15));
                    PointF point5_1 = new PointF(Convert.ToSingle(x + T_width * dataindex), Convert.ToSingle(datay2 - gao + 0.1));

                    path = new GraphicsPath(new PointF[] { point1_1, point2_1, point3_1, point4_1, point5_1 }, new byte[] { (byte)PathPointType.Start, (byte)PathPointType.Line, (byte)PathPointType.Line, (byte)PathPointType.Line, (byte)PathPointType.Line });

                    g.FillPath(new SolidBrush(colors1[colorindex]), path);

                    //正面
                    PointF point1 = new PointF(Convert.ToSingle(x + T_width * dataindex), Convert.ToSingle(datay2 - gao + 0.1));
                    PointF point2 = new PointF(Convert.ToSingle(x + T_width * dataindex + T_width), Convert.ToSingle(datay2 - gao + 0.1));
                    PointF point3 = new PointF(Convert.ToSingle(x + T_width * dataindex + T_width), Convert.ToSingle(datay2));
                    PointF point4 = new PointF(Convert.ToSingle(x + T_width * dataindex), Convert.ToSingle(datay2));
                    PointF point5 = new PointF(Convert.ToSingle(x + T_width * dataindex), Convert.ToSingle(datay2 - gao + 0.1));

                    path = new GraphicsPath(new PointF[] { point1, point2, point3, point4, point5 }, new byte[] { (byte)PathPointType.Start, (byte)PathPointType.Line, (byte)PathPointType.Line, (byte)PathPointType.Line, (byte)PathPointType.Line });

                    g.FillPath(new SolidBrush(colors[colorindex]), path);

                    datas.Add(data);
                    gaos.Add(gao);
                    colorindex++;
                    dataindex++;
                }
                int gaoindex = 0;
                colorindex = 0;
                dataindex = 0;
                foreach (string data1 in datas)
                {
                    g.DrawString(data1, font3, new SolidBrush(colors[colorindex]), x + T_width * dataindex, datay2 - gaos[gaoindex] - 30);
                    gaoindex++;
                    colorindex++;
                    dataindex++;
                }
                x += x_pitch;
            }

            CreateCaption(capname, g, width - 105, y, "柱状图", colors);
            return image;
        }
        #endregion

        #region 折线图
        /// <summary>
        /// 画折线图
        /// </summary>
        /// <param name="dt">数据源（名字、数据1、数据n）</param>
        /// <param name="title">统计图标题</param>
        /// <param name="linecolor">网格线颜色</param>
        /// <param name="x_name">x轴注释</param>
        /// <param name="y_name">y轴注释</param>
        /// <returns></returns>
        public static Bitmap CreateLineChart(DataTable dt, string title, Color linecolor, string x_name, string y_name)
        {
            float maxValue = 0;
            float b = 0;
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                foreach (DataRow row in dt.Rows)
                {
                    float a = Convert.ToSingle(row[i].ToString());
                    if (a > b)
                    {
                        maxValue = a; b = a;
                    }
                    else
                    {
                        maxValue = b;
                    }
                }
            }
            if ((maxValue % 10) != 0)
                maxValue = 10 * ((int)maxValue / 10) + 10;
            maxValue /= 5;
            List<float> y_value = new List<float>();
            for (int i = 1; i <= 5; i++)
            {
                y_value.Add(maxValue * i);
            }
            List<string> x_zhi = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                x_zhi.Add(row[0].ToString());
            }
            int x = 50;
            int y = 70;
            int x_pitch = 40;
            int y_pitch = 35;
            int height = y_value.Count * y_pitch + 300;
            int width = x_zhi.Count * x_pitch + 225;
            int tempheight = 100 + 20 * dt.Rows.Count;
            if (height < tempheight)
            {
                height = tempheight;
            }
            int x2 = width - 120;
            int y2 = height - 120;
            int datay2 = y2;
            int datax = x;
            List<Color> colors = new List<Color>();
            Random ran = new Random();
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                colors.Add(Color.FromArgb(200, ran.Next(0, 200), ran.Next(0, 200), ran.Next(0, 200)));
            }
            List<string> capname = new List<string>();
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                capname.Add(dt.Columns[i].ToString());
            }

            Bitmap image = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(image);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.White);
            Font font1 = new Font("Arial", 10, FontStyle.Regular);
            Font font2 = new Font("宋体", 10, FontStyle.Regular);
            Font font3 = new Font("宋体", 10, FontStyle.Regular);
            Brush brushe1 = new SolidBrush(Color.Black);
            g.DrawString(title, font2, brushe1, width / 5, 30);
            g.DrawRectangle(new Pen(Color.Black), 0, 0, image.Width - 1, image.Height - 1);

            Pen pen1 = new Pen(linecolor, 1);
            Pen pen2 = new Pen(linecolor, 2);
            foreach (string xtitle in x_zhi)
            {
                x += x_pitch;
                g.DrawLine(pen1, x, y, x, y2);
                StringFormat format = new StringFormat();
                format.FormatFlags = StringFormatFlags.DirectionVertical;
                g.DrawString(xtitle, font3, brushe1, x - 10, y2, format);
            }
            x = datax;
            g.DrawLine(pen2, x, y, x, y2);
            g.DrawString(x_name, font3, brushe1, x2, y2);
            g.DrawLine(pen2, x, y2, x2, y2);
            g.DrawString(y_name, font3, brushe1, x - 40, y);
            y2 -= y_pitch;
            foreach (float ytitle in y_value)
            {
                g.DrawLine(pen1, x, y2, x2, y2);
                g.DrawString(ytitle.ToString(), font3, brushe1, x - 30, y2 - 10);
                y2 -= y_pitch;
            }

            int colorindex = 0;
            int dataindex = 0;
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                Point p1 = new Point();
                Point p2 = new Point();
                Point p3 = new Point();
                int rowindex = 0;
                x = datax + x_pitch;
                foreach (DataRow row in dt.Rows)
                {
                    string data = row[i].ToString();
                    float gao = Convert.ToSingle(data) / y_value[0] * y_pitch;
                    p1 = new Point(x, datay2 - (int)gao);
                    p2 = p3;
                    p3 = p1;
                    if (p2.X == 0 || p2.Y == 0)
                    {
                        g.DrawLine(new Pen(colors[colorindex], 3), p3, p1);
                    }
                    else
                    {
                        g.DrawLine(new Pen(colors[colorindex], 3), p2, p1);
                    }
                    g.DrawString(data, font3, new SolidBrush(colors[colorindex]), x, datay2 - gao - 20);
                    x += x_pitch;
                    rowindex++;
                }
                colorindex++;
                dataindex++;
            }

            CreateCaption(capname, g, width - 105, y, "折线图", colors);
            return image;
        }
        #endregion

        #region 立体折线图
        /// <summary>
        /// 画立体折线图
        /// </summary>
        /// <param name="dt">数据集（名字、数据1、数据n）</param>
        /// <param name="title">统计图标题</param>
        /// <param name="linecolor">轴颜色</param>
        /// <param name="x_name">x轴注释</param>
        /// <param name="y_name">y轴注释</param>
        /// <returns></returns>
        public static Bitmap CreateLineChart3D(DataTable dt, string title, Color linecolor, string x_name, string y_name)
        {
            float maxValue = 0;
            float b = 0;
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                foreach (DataRow row in dt.Rows)
                {
                    float a = Convert.ToSingle(row[i].ToString());
                    if (a > b)
                    {
                        maxValue = a; b = a;
                    }
                    else
                    {
                        maxValue = b;
                    }
                }
            }
            if ((maxValue % 10) != 0)
                maxValue = 10 * ((int)maxValue / 10) + 10;
            maxValue /= 5;
            List<float> y_value = new List<float>();
            for (int i = 1; i <= 5; i++)
            {
                y_value.Add(maxValue * i);
            }
            List<string> x_zhi = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                x_zhi.Add(row[0].ToString());
            }
            int x = 50;
            int y = 70;
            int x_pitch = 40;
            int y_pitch = 35;
            int height = y_value.Count * y_pitch + 300;
            int width = x_zhi.Count * x_pitch + 225;
            int tempheight = 100 + 20 * dt.Rows.Count;
            if (height < tempheight)
            {
                height = tempheight;
            }
            int x2 = width - 120;
            int y2 = height - 120;
            int datay2 = y2;
            int datax = x;
            List<Color> colors = new List<Color>();
            Random ran = new Random();
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                colors.Add(Color.FromArgb(200, ran.Next(0, 200), ran.Next(0, 200), ran.Next(0, 200)));
            }
            List<string> capname = new List<string>();
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                capname.Add(dt.Columns[i].ToString());
            }

            Bitmap image = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(image);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.White);
            Font font1 = new Font("Arial", 10, FontStyle.Regular);
            Font font2 = new Font("宋体", 10, FontStyle.Regular);
            Font font3 = new Font("宋体", 10, FontStyle.Regular);
            Brush brushe1 = new SolidBrush(Color.Black);
            g.DrawString(title, font2, brushe1, width / 5, 30);
            g.DrawRectangle(new Pen(Color.Black), 0, 0, image.Width - 1, image.Height - 1);

            Pen pen1 = new Pen(linecolor, 1);
            Pen pen2 = new Pen(linecolor, 2);
            foreach (string xtitle in x_zhi)
            {
                x += x_pitch;
                StringFormat format = new StringFormat();
                format.FormatFlags = StringFormatFlags.DirectionVertical;
                g.DrawString(xtitle, font3, brushe1, x, y2, format);
            }
            x = datax;
            g.DrawLine(pen2, x, y, x, y2);
            g.DrawString(x_name, font3, brushe1, x2, y2);
            g.DrawLine(pen2, x, y2, x2, y2);
            g.DrawString(y_name, font3, brushe1, x - 40, y);
            y2 -= y_pitch;
            foreach (float ytitle in y_value)
            {
                g.DrawString(ytitle.ToString(), font3, brushe1, x - 30, y2);
                y2 -= y_pitch;
            }


            int colorindex = 0;
            int dataindex = 0;
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                Point p1 = new Point();
                Point p2 = new Point();
                Point p3 = new Point();
                int rowindex = 0;
                x = datax + x_pitch;
                foreach (DataRow row in dt.Rows)
                {
                    string data = row[i].ToString();
                    float gao = Convert.ToSingle(data) / y_value[0] * y_pitch;
                    p1 = new Point(x, datay2 - (int)gao);
                    p2 = p3;
                    p3 = p1;
                    if (p2.X == 0 || p2.Y == 0)
                    {
                        g.DrawLine(new Pen(Color.FromArgb(121, 121, 121), 3), new Point(p3.X + 2, p3.Y + 2), new Point(p1.X + 2, p1.Y + 2));
                        g.DrawLine(new Pen(colors[colorindex], 3), p3, p1);
                    }
                    else
                    {
                        g.DrawLine(new Pen(Color.FromArgb(121, 121, 121), 3), new Point(p2.X + 2, p2.Y + 2), new Point(p1.X + 2, p1.Y + 2));
                        g.DrawLine(new Pen(colors[colorindex], 3), p2, p1);
                    }
                    g.DrawString(data, font3, new SolidBrush(colors[colorindex]), x, datay2 - gao - 20);
                    x += x_pitch;
                    rowindex++;
                }
                colorindex++;
                dataindex++;
            }

            CreateCaption(capname, g, width - 105, y, "折线图", colors);
            return image;
        }
        #endregion

        #region 扇形图
        /// <summary>
        /// 画扇形图
        /// </summary>
        /// <param name="title">统计图标题</param>
        /// <param name="dt">数据集（名字、数据1、数据n）</param>
        /// <param name="capname">说明图元素</param>
        /// <returns></returns>
        public static Bitmap CreatePieSlice(string title, DataTable dt)
        {
            int x = 100;
            int y = 100;
            int t_empx = x;
            int t_empy = y;
            int piewidth = 200;
            int pieheight = 100;
            int piepitch = 50;
            int width = piewidth * 2 + 300;
            int height = 200 + pieheight * dt.Columns.Count / 2 + piepitch * dt.Columns.Count / 2;
            int tempheight = 100 + 20 * dt.Rows.Count;
            if (height < tempheight)
            {
                height = tempheight;
            }
            Random ran = new Random();
            List<Color> colors = new List<Color>();
            List<string> capname = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                colors.Add(Color.FromArgb(200, ran.Next(0, 200), ran.Next(0, 200), ran.Next(0, 200)));
                capname.Add(row[0].ToString());
            }
            List<float> count = new List<float>();
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                float sum = 0;
                foreach (DataRow row in dt.Rows)
                {
                    sum += Convert.ToSingle(row[i].ToString());
                }
                count.Add(sum);
            }
            List<string> titlestr = new List<string>();
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                titlestr.Add(dt.Columns[i].ToString());
            }
            Bitmap image = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(image);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.White);
            Font font1 = new Font("Arial", 10, FontStyle.Regular);
            Font font2 = new Font("宋体", 10, FontStyle.Regular);
            Font font3 = new Font("宋体", 10, FontStyle.Regular);
            Brush brushe1 = new SolidBrush(Color.Black);
            g.DrawString(title, font2, brushe1, width / 5, 30);
            g.DrawRectangle(new Pen(Color.Black), 0, 0, image.Width - 1, image.Height - 1);

            int dataindex = 0;
            int capindex = 0;
            int countindex = 0;
            int xindex = 0;
            int yindex = 0;
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                if ((piewidth * xindex + x + 150) < width)
                {
                    x = piepitch * xindex + t_empx + piewidth * xindex;
                    xindex++;
                }
                else
                {
                    yindex++;
                    x = t_empx;
                    xindex = 1;
                    y = piepitch * yindex + t_empy + pieheight * yindex;
                }
                g.DrawString(titlestr[capindex], new Font("黑体", 10), new SolidBrush(Color.Black), x, y - 30);
                float r = 0.0f;
                float JiaoDu = 0.0f;
                int colorindex = 0;
                foreach (DataRow row in dt.Rows)
                {
                    r = JiaoDu + r;
                    float data = Convert.ToSingle(row[i].ToString());
                    JiaoDu = (data / count[countindex]) * 360;

                    g.FillPie(new SolidBrush(colors[colorindex]), x, y, piewidth, pieheight, r, JiaoDu);

                    colorindex++;
                }
                countindex++;
                dataindex++;
                capindex++;
            }
            CreateCaption(capname, g, width - 105, t_empy - 50, "扇形图", colors);
            return image;
        }

        #endregion

        #region 立体扇形图
        /// <summary>
        /// 画立体扇形图
        /// </summary>
        /// <param name="title">统计图标题</param>
        /// <param name="dt">数据集（名字、数据1、数据n）</param>
        /// <param name="capname">说明图元素</param>
        /// <returns></returns>
        public static Bitmap CreatePieSlice3D(string title, DataTable dt)
        {
            int x = 100;
            int y = 100;
            int t_empx = x;
            int t_empy = y;
            int piewidth = 200;
            int pieheight = 100;
            int piepitch = 50;
            int width = piewidth * 2 + 300;
            int height = 200 + (pieheight * dt.Columns.Count - 1) / 2 + (piepitch * dt.Columns.Count - 1) / 2;
            int tempheight = 100 + 20 * dt.Rows.Count;
            if (height < tempheight)
            {
                height = tempheight;
            }
            Random ran = new Random();
            List<Color> colors = new List<Color>();
            List<string> capname = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                colors.Add(Color.FromArgb(200, ran.Next(0, 200), ran.Next(0, 200), ran.Next(0, 200)));
                capname.Add(row[0].ToString());
            }
            List<Color> colors1 = new List<Color>();
            foreach (Color color in colors)
            {
                int r = (int)color.R - 30;
                int g1 = (int)color.G - 30;
                int b = (int)color.B - 30;
                if (r < 0)
                    r = 0;
                if (g1 < 0)
                    g1 = 0;
                if (b < 0)
                    b = 0;
                colors1.Add(Color.FromArgb(r, g1, b));
            }
            List<float> count = new List<float>();
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                float sum = 0;
                foreach (DataRow row in dt.Rows)
                {
                    sum += Convert.ToSingle(row[i].ToString());
                }
                count.Add(sum);
            }
            List<string> titlestr = new List<string>();
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                titlestr.Add(dt.Columns[i].ToString());
            }
            Bitmap image = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(image);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.White);
            Font font1 = new Font("Arial", 10, FontStyle.Regular);
            Font font2 = new Font("宋体", 10, FontStyle.Regular);
            Font font3 = new Font("宋体", 10, FontStyle.Regular);
            Brush brushe1 = new SolidBrush(Color.Black);
            g.DrawString(title, font2, brushe1, width / 5, 30);
            g.DrawRectangle(new Pen(Color.Black), 0, 0, image.Width - 1, image.Height - 1);

            int dataindex = 0;
            int capindex = 0;
            int countindex = 0;
            int xindex = 0;
            int yindex = 0;
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                if ((piewidth * xindex + x + 150) < width)
                {
                    x = piepitch * xindex + t_empx + piewidth * xindex;
                    xindex++;
                }
                else
                {
                    yindex++;
                    x = t_empx;
                    xindex = 1;
                    y = piepitch * yindex + t_empy + pieheight * yindex;
                }
                g.DrawString(titlestr[capindex], new Font("黑体", 10), new SolidBrush(Color.Black), x, y - 30);
                float r = 0.0f;
                float JiaoDu = 0.0f;
                List<float> L_r = new List<float>();
                List<float> L_JiaoDu = new List<float>();
                float data;
                foreach (DataRow row in dt.Rows)
                {
                    r = JiaoDu + r;
                    data = Convert.ToSingle(row[i].ToString());
                    JiaoDu = (data / count[countindex]) * 360;

                    L_r.Add(r);
                    L_JiaoDu.Add(JiaoDu);
                }
                int jiaoduindex = L_r.Count - 1;
                int tempindex = 0;
                foreach (float lr in L_r)
                {
                    if ((L_r[jiaoduindex] >= 90f && L_r[jiaoduindex] <= 180f))
                    {
                        for (int j = 0; j <= 15; j++)
                        {
                            g.FillPie(new SolidBrush(colors1[jiaoduindex]), x, y + j, piewidth, pieheight, L_r[jiaoduindex], L_JiaoDu[jiaoduindex]);
                        }
                        jiaoduindex--;
                    }
                    if (L_r[jiaoduindex] < 90f)
                    {
                        if (tempindex <= jiaoduindex)
                        {
                            for (int j = 0; j <= 15; j++)
                            {
                                g.FillPie(new SolidBrush(colors1[tempindex]), x, y + j, piewidth, pieheight, L_r[tempindex], L_JiaoDu[tempindex]);
                            }
                        }
                        tempindex++;
                    }
                    else if (L_r[jiaoduindex] > 180f && L_r[jiaoduindex] <= 360f)
                    {
                        jiaoduindex--;
                    }
                }

                int colorindex = 0;
                foreach (DataRow row in dt.Rows)
                {
                    r = JiaoDu + r;
                    data = Convert.ToSingle(row[i].ToString());
                    JiaoDu = (data / count[countindex]) * 360;
                    float baifen = (data / count[countindex]) * 100f;

                    g.FillPie(new SolidBrush(colors[colorindex]), x, y, piewidth, pieheight, r, JiaoDu);

                    colorindex++;
                }
                countindex++;
                dataindex++;
                capindex++;
            }
            CreateCaption(capname, g, width - 105, t_empy - 50, "扇形图", colors);
            return image;
        }

        #endregion

        #region 柱状百分比图
        /// <summary>
        /// 画柱状百分比图
        /// </summary>
        /// <param name="dt">数据源（名字、数据1、数据n）</param>
        /// <param name="title">统计图标题</param>
        /// <param name="linecolor">网格线颜色</param>
        /// <param name="x_name">x轴注释</param>
        /// <param name="y_name">y轴注释</param>
        /// <param name="T_width">柱状图的宽</param>
        /// <returns></returns>
        public static Bitmap CreateColPerCent(DataTable dt, string title, Color linecolor, string x_name, string y_name, int T_width)
        {
            StringFormat format = new StringFormat();
            format.FormatFlags = StringFormatFlags.DirectionVertical | StringFormatFlags.DirectionRightToLeft;
            List<float> count = new List<float>();
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                float sum = 0;
                foreach (DataRow row in dt.Rows)
                {
                    sum += Convert.ToSingle(row[i].ToString());
                }
                count.Add(sum);
            }
            float maxValue = 0;
            float b = 0;
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                foreach (DataRow row in dt.Rows)
                {
                    float a = Convert.ToSingle(row[i].ToString());
                    if (a > b)
                    {
                        maxValue = a; b = a;
                    }
                    else
                    {
                        maxValue = b;
                    }
                }
            }
            if ((maxValue % 10) != 0)
                maxValue = 10 * ((int)maxValue / 10) + 10;
            maxValue /= 5;
            List<float> y_value = new List<float>();
            for (int i = 1; i <= 5; i++)
            {
                y_value.Add(maxValue * i);
            }
            List<string> x_zhi = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                x_zhi.Add(row[0].ToString());
            }
            int x = 50;
            int y = 70;
            int x_pitch = 15 + T_width * (dt.Columns.Count - 1);
            int y_pitch = 35;
            int T_place = x_pitch - T_width * (dt.Columns.Count - 1) / 2;
            int height = y_value.Count * y_pitch + 300;
            int width = x_zhi.Count * x_pitch + 250;
            int tempheight = 100 + 20 * dt.Rows.Count;
            if (height < tempheight)
            {
                height = tempheight;
            }
            int x2 = width - 150;
            int y2 = height - 150;
            int datay2 = y2;
            int datax = x;
            List<Color> colors = new List<Color>();
            Random ran = new Random();
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                colors.Add(Color.FromArgb(200, ran.Next(0, 200), ran.Next(0, 200), ran.Next(0, 200)));
            }
            List<string> capname = new List<string>();
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                capname.Add(dt.Columns[i].ToString());
            }

            Bitmap image = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(image);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.White);
            Font font1 = new Font("Arial", 10, FontStyle.Regular);
            Font font2 = new Font("宋体", 10, FontStyle.Regular);
            Font font3 = new Font("宋体", 10, FontStyle.Regular);
            Brush brushe1 = new SolidBrush(Color.Black);
            g.DrawString(title, font2, brushe1, width / 5, 30);
            g.DrawRectangle(new Pen(Color.Black), 0, 0, image.Width - 1, image.Height - 1);

            Pen pen1 = new Pen(linecolor, 1);
            Pen pen2 = new Pen(linecolor, 2);
            foreach (string xtitle in x_zhi)
            {
                x += x_pitch;
                g.DrawLine(pen1, x, y, x, y2);

                g.DrawString(xtitle, font3, brushe1, x + 5, y2, format);
            }
            x = datax;
            g.DrawLine(pen2, x, y, x, y2);
            g.DrawString(x_name, font3, brushe1, x2, y2);
            g.DrawLine(pen2, x, y2, x2, y2);
            g.DrawString(y_name, font3, brushe1, x - 40, y);
            y2 -= y_pitch;
            foreach (float ytitle in y_value)
            {
                g.DrawLine(pen1, x, y2, x2, y2);
                g.DrawString(ytitle.ToString(), font3, brushe1, x - 30, y2 - 5);
                y2 -= y_pitch;
            }

            int colorindex = 0;
            int dataindex = 0;
            int contindex = 0;
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                x = datax + T_place;
                foreach (DataRow row in dt.Rows)
                {
                    string data = row[i].ToString();
                    string percent = Convert.ToString(((Convert.ToSingle(data) / count[contindex]) * 100));
                    float gao = Convert.ToSingle(data) / y_value[0] * y_pitch;
                    g.FillRectangle(new SolidBrush(colors[colorindex]), x + T_width * dataindex, datay2 - gao, T_width, gao);
                    g.DrawString(Convert.ToSingle(percent).ToString("f2") + "%", font3, new SolidBrush(colors[colorindex]), x + T_width * dataindex - 10, datay2 - gao - 20);
                    x += x_pitch;
                }
                colorindex++;
                dataindex++;
                contindex++;
            }

            CreateCaption(capname, g, width - 105, y, "柱状图", colors);
            return image;
        }
        #endregion

        #region 雷达图
        /// <summary>
        /// 一维雷达图
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="values">数据表（格式：名字、数据1），只能有一行数据</param>
        /// <returns></returns>
        public static Bitmap CreateRadarChart(string title, DataTable values)
        {
            float maxValue = 0;
            float b = 0;
            for (int i = 1; i < values.Columns.Count; i++)
            {
                foreach (DataRow row in values.Rows)
                {
                    float a = Convert.ToSingle(row[i].ToString());
                    if (a > b)
                    {
                        maxValue = a; b = a;
                    }
                    else
                    {
                        maxValue = b;
                    }
                }
            }
            if ((maxValue % 10) != 0)
                maxValue = 10 * ((int)maxValue / 10) + 10;

            int maxlen = 0;
            int c = 0;
            foreach (DataRow row in values.Rows)
            {
                int len = row[0].ToString().Length;
                if (len > c)
                {
                    maxlen = len;
                    c = len;
                }
                else
                {
                    maxlen = c;
                }
            }
            int left = (maxlen + 1) * 14;

            List<float> r_value = new List<float>();
            float maxstep = maxValue / 5;
            for (int i = 0; i < 5; i++)
            {
                r_value.Add(maxValue - maxstep * i);
            }

            List<string> attr = new List<string>();
            foreach (DataRow row in values.Rows)
            {
                attr.Add(row[0].ToString());
            }

            Rectangle rect = new Rectangle(left, 100, 300, 300);
            Bitmap image = new Bitmap(rect.Width + left * 2, rect.Height + 150);
            Graphics g = Graphics.FromImage(image);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.FromArgb(0, 255, 255, 255));
            Font font1 = new Font("Arial", 10, FontStyle.Regular);
            Font font2 = new Font("宋体", 10, FontStyle.Regular);
            Brush brushe1 = new SolidBrush(Color.FromArgb(82, 137, 247));
            g.DrawString(title, font2, brushe1, image.Width / 3, 30);
            //g.DrawRectangle(new Pen(Color.Black), 0, 0, image.Width - 1, image.Height - 1);
            //圆的直径
            float diameter = Math.Min(rect.Width, rect.Height);
            //圆的半径
            float radius = diameter / 2;
            //圆心
            PointF center = new PointF(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);

            //画5个同心圆
            int count = 5;
            float diameterstep = diameter / count;
            float radiustep = radius / count;

            RectangleF rectf = new RectangleF();
            rectf.X = center.X - radius;
            rectf.Y = center.Y - radius;
            rectf.Width = rectf.Height = diameter;

            //float r0 = radius;

            for (int i = 0; i < count; i++)
            {
                g.DrawEllipse(new Pen(Color.FromArgb(82, 137, 247)), rectf);

                rectf.X += radiustep;
                rectf.Y += radiustep;
                rectf.Width -= diameterstep;
                rectf.Height -= diameterstep;



                //PointF StrPt = new PointF((float)(r0 * Math.Cos(270 * Math.PI / 180) + center.X), (float)(r0 * Math.Sin(270 * Math.PI / 180) + center.Y) - 5);
                //g.DrawString(r_value[i].ToString(), font2, new SolidBrush(Color.FromArgb(0, 255, 0)), StrPt);
                //r0 -= radiustep;
            }
            //射线数
            int linecount = values.Rows.Count;
            if (linecount > 0)
            {
                //计算角度
                float angle = 0;
                float anglestep = 360f / linecount;

                for (int i = 0; i < linecount; i++)
                {
                    //终点
                    PointF endPoint = new PointF((float)(radius * Math.Cos(angle * Math.PI / 180) + center.X), (float)(radius * Math.Sin(angle * Math.PI / 180) + center.Y));
                    g.DrawLine(new Pen(Color.FromArgb(82, 137, 247)), center, endPoint);
                    // 画名字，属性
                    string anglestr = attr[i];
                    PointF textpoint = endPoint;
                    if (angle == 270)
                    {
                        textpoint.Y -= 20;
                        textpoint.X -= anglestr.Length * 12 / 2;
                    }
                    else if (angle < 270 && angle > 180)
                    {
                        textpoint.X -= (anglestr.Length + 1) * 12;
                        textpoint.Y -= 10;
                    }
                    else if (angle == 180)
                    {
                        textpoint.X -= (anglestr.Length + 1) * 12;
                        textpoint.Y -= 5;
                    }
                    else if (angle < 180 && angle > 90)
                    {
                        textpoint.X -= (anglestr.Length + 1) * 12;
                        //textpoint.Y += 5;
                    }
                    else if (angle == 90)
                    {
                        textpoint.X -= anglestr.Length * 12 / 2;
                        textpoint.Y += 10;
                    }
                    //else if (angle > 0 && angle < 90)
                    //{
                    //    textpoint.X -= 5;
                    //    textpoint.Y += 5;
                    //}
                    else if (angle > 270 && angle < 360)
                    {
                        textpoint.Y -= 10;
                    }
                    else if (angle == 0)
                    {
                        textpoint.X += 5;
                        textpoint.Y -= 5;
                    }
                    g.DrawString(anglestr, font2, new SolidBrush(Color.FromArgb(82, 137, 247)), textpoint);
                    angle += anglestep;
                    angle %= 360;
                }
            }

            PointF[] points = new PointF[values.Rows.Count + 1];
            byte[] bytes = new byte[values.Rows.Count + 1];
            PointF NextPoint;
            //计算角度
            float anglex = 0;
            float anglestepx = 360f / values.Rows.Count;
            float nextang = 0;
            for (int i = 0; i < values.Rows.Count; i++)
            {
                float r = radius * (Convert.ToSingle(values.Rows[i][1])) / maxValue;
                nextang += anglestepx;
                if ((i + 1) < values.Rows.Count)
                {
                    float r1 = radius * (Convert.ToSingle(values.Rows[i + 1][1])) / maxValue;
                    NextPoint = new PointF((float)(r1 * Math.Cos(nextang * Math.PI / 180) + center.X), (float)(r1 * Math.Sin(nextang * Math.PI / 180) + center.Y));
                    points[i + 1] = NextPoint;
                    bytes[i + 1] = (byte)PathPointType.Line;
                }
                else
                {
                    float r2 = radius * (Convert.ToSingle(values.Rows[0][1])) / maxValue;
                    NextPoint = new PointF((float)(r2 * Math.Cos(0 * Math.PI / 180) + center.X), (float)(r2 * Math.Sin(0 * Math.PI / 180) + center.Y));
                    points[i + 1] = NextPoint;
                    bytes[i + 1] = (byte)PathPointType.Line;
                }
                points[i] = new PointF((float)(r * Math.Cos(anglex * Math.PI / 180) + center.X), (float)(r * Math.Sin(anglex * Math.PI / 180) + center.Y));
                bytes[0] = (byte)PathPointType.Start;
                // 连接当前点和下一点
                //g.DrawLine(Pens.Black, NextPoint, new PointF((float)(r * Math.Cos(anglex * Math.PI / 180) + center.X), (float)(r * Math.Sin(anglex * Math.PI / 180) + center.Y)));

                anglex += anglestepx;
                anglex %= 360;
            }
            GraphicsPath path = new GraphicsPath(points, bytes);
            g.DrawPath(new Pen(Color.Green), path);
            g.FillPath(new SolidBrush(Color.FromArgb(150, 18, 144, 23)), path);
            for (int pti = 0; pti < points.Length - 1; pti++)
            {
                g.DrawString(values.Rows[pti][1].ToString(), new Font("Arial", 8), new SolidBrush(Color.Green), points[pti]);
            }
            return image;
        }
        #endregion

        #region 巴士图
        /// <summary>
        /// 巴士图
        /// </summary>
        /// <param name="dt">数据源（名字，数据1..数据n）</param>
        /// <param name="title">标题</param>
        /// <param name="linecolor">线颜色</param>
        /// <param name="x_name">x轴注释</param>
        /// <param name="y_name">y轴注释</param>
        /// <param name="T_width">一个柱图的宽</param>
        /// <returns></returns>
        public static Bitmap CreateBarChart(DataTable dt, string title, Color linecolor, string x_name, string y_name, int T_width)
        {
            int maxlen = 0;
            int temlen = 0;
            foreach (DataRow row in dt.Rows)
            {
                int a = row[0].ToString().Length;
                if (a > temlen)
                {
                    maxlen = a; temlen = a;
                }
                else
                {
                    maxlen = temlen;
                }
            }
            maxlen *= 12;
            float maxValue = 0;
            float b = 0;
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                foreach (DataRow row in dt.Rows)
                {
                    float a = Convert.ToSingle(row[i].ToString());
                    if (a > b)
                    {
                        maxValue = a; b = a;
                    }
                    else
                    {
                        maxValue = b;
                    }
                }
            }
            if ((maxValue % 10) != 0)
                maxValue = 10 * ((int)maxValue / 10) + 10;
            maxValue /= 5;
            List<float> x_value = new List<float>();
            for (int i = 1; i <= 5; i++)
            {
                x_value.Add(maxValue * i);
            }
            List<string> y_zhi = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                y_zhi.Add(row[0].ToString());
            }
            int x = maxlen + 20;
            int y = 70;
            int y_pitch = 15 + T_width * (dt.Columns.Count - 1);
            int x_pitch = 50;
            int T_place = y_pitch - T_width * (dt.Columns.Count - 1) / 2;
            int width = x_value.Count * x_pitch + 250;
            int height = y_zhi.Count * y_pitch + 300;

            int x2 = width - 80;
            int y2 = T_place * 2 + y + y_pitch * dt.Rows.Count;
            int datay = y;
            int datax = x;
            List<Color> colors = new List<Color>();
            Random ran = new Random();
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                colors.Add(Color.FromArgb(200, ran.Next(0, 200), ran.Next(0, 200), ran.Next(0, 200)));
            }
            List<string> capname = new List<string>();
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                capname.Add(dt.Columns[i].ToString());
            }

            Bitmap image = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(image);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.White);
            Font font1 = new Font("Arial", 10, FontStyle.Regular);
            Font font2 = new Font("宋体", 10, FontStyle.Regular);
            Font font3 = new Font("宋体", 10, FontStyle.Regular);
            Brush brushe1 = new SolidBrush(Color.Black);
            g.DrawString(title, font2, brushe1, width / 5, 30);
            g.DrawRectangle(new Pen(Color.Black), 0, 0, image.Width - 1, image.Height - 1);

            Pen pen1 = new Pen(linecolor, 1);
            Pen pen2 = new Pen(linecolor, 2);

            g.DrawLine(pen2, x, y, x2, y);
            g.DrawString(y_name, font3, brushe1, x - 40, y);
            foreach (string ytitle in y_zhi)
            {
                y += y_pitch;
                g.DrawLine(pen1, x, y, x2, y);
                //StringFormat format = new StringFormat();
                //format.FormatFlags = StringFormatFlags.DirectionVertical | StringFormatFlags.DirectionRightToLeft;
                g.DrawString(ytitle, font3, brushe1, x - maxlen - 10, y);
            }

            g.DrawString(x_name, font3, brushe1, x2, y2);
            g.DrawLine(pen2, x, y2, x2, y2);

            x = datax;
            g.DrawLine(pen2, x, datay, x, y2);
            foreach (float xtitle in x_value)
            {
                x += x_pitch;
                g.DrawString(xtitle.ToString(), font3, brushe1, x - 10, y2 + 5);

            }

            int colorindex = 0;
            int dataindex = 0;
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                y = datay + T_place;
                foreach (DataRow row in dt.Rows)
                {
                    string data = row[i].ToString();
                    float kuan = Convert.ToSingle(data) / x_value[0] * x_pitch;
                    g.FillRectangle(new SolidBrush(colors[colorindex]), datax, y + T_width * dataindex, kuan, T_width);
                    //g.DrawString(data, font3, new SolidBrush(Color.White), datax + kuan - (data.Length + 1) * 6, y + T_width * dataindex + 3);
                    g.DrawString(data, font3, new SolidBrush(colors[colorindex]), datax + kuan, y + T_width * dataindex + 3);
                    y += y_pitch;
                }
                colorindex++;
                dataindex++;
            }

            CreateCaption(capname, g, datax, y2 + 30, "巴士图", colors);
            return image;
        }
        #endregion

        #region 多维雷达图
        /// <summary>
        /// 多维雷达图
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="values">数据表（格式：名字、数据1,数据2,数据3），统计维度小于或等于3</param>
        /// <returns></returns>
        public static Bitmap CreateRadarCharts(string title, DataTable values)
        {
            float maxValue = 0;
            float b = 0;
            for (int i = 1; i < values.Columns.Count; i++)
            {
                foreach (DataRow row in values.Rows)
                {
                    float a = Convert.ToSingle(row[i].ToString());
                    if (a > b)
                    {
                        maxValue = a; b = a;
                    }
                    else
                    {
                        maxValue = b;
                    }
                }
            }
            if ((maxValue % 10) != 0)
                maxValue = 10 * ((int)maxValue / 10) + 10;

            int maxlen = 0;
            int c = 0;
            foreach (DataRow row in values.Rows)
            {
                int len = row[0].ToString().Length;
                if (len > c)
                {
                    maxlen = len;
                    c = len;
                }
                else
                {
                    maxlen = c;
                }
            }
            int left = (maxlen + 1) * 14;
            List<float> r_value = new List<float>();
            float maxstep = maxValue / 5;
            for (int i = 0; i < 5; i++)
            {
                r_value.Add(maxValue - maxstep * i);
            }
            List<string> capname = new List<string>();
            for (int i = 1; i < values.Columns.Count; i++)
            {
                capname.Add(values.Columns[i].ToString());
            }
            List<string> attr = new List<string>();
            foreach (DataRow row in values.Rows)
            {
                attr.Add(row[0].ToString());
            }
            List<Color> colors = new List<Color>();
            colors.Add(Color.Green);
            colors.Add(Color.Red);
            colors.Add(Color.Blue);
            //Random ran = new Random();
            //for (int i = 1; i < values.Columns.Count; i++)
            //{
            //    colors.Add(Color.FromArgb(ran.Next(0, 200), ran.Next(0, 200), ran.Next(0, 200)));
            //}
            List<Color> colors1 = new List<Color>();
            foreach (Color color in colors)
            {
                int r1 = (int)color.R + 30;
                int g1 = (int)color.G + 30;
                int b1 = (int)color.B + 30;
                if (r1 > 255)
                    r1 = 200;
                if (g1 > 255)
                    g1 = 200;
                if (b1 > 255)
                    b1 = 200;
                colors1.Add(Color.FromArgb(150, r1, g1, b1));
            }
            Rectangle rect = new Rectangle(left, 100, 300, 300);
            Bitmap image = new Bitmap(rect.Width + left * 2, rect.Height + 200);
            Graphics g = Graphics.FromImage(image);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.FromArgb(0, 255, 255, 255));
            Font font1 = new Font("Arial", 10, FontStyle.Regular);
            Font font2 = new Font("宋体", 10, FontStyle.Regular);
            Brush brushe1 = new SolidBrush(Color.FromArgb(82, 137, 247));
            g.DrawString(title, font2, brushe1, image.Width / 3, 30);
            //g.DrawRectangle(new Pen(Color.Black), 0, 0, image.Width - 1, image.Height - 1);
            //圆的直径
            float diameter = Math.Min(rect.Width, rect.Height);
            //圆的半径
            float radius = diameter / 2;
            //圆心
            PointF center = new PointF(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);

            //画5个同心圆
            int count = 5;
            float diameterstep = diameter / count;
            float radiustep = radius / count;

            RectangleF rectf = new RectangleF();
            rectf.X = center.X - radius;
            rectf.Y = center.Y - radius;
            rectf.Width = rectf.Height = diameter;

            //float r0 = radius;

            for (int i = 0; i < count; i++)
            {
                g.DrawEllipse(new Pen(Color.FromArgb(82, 137, 247)), rectf);

                rectf.X += radiustep;
                rectf.Y += radiustep;
                rectf.Width -= diameterstep;
                rectf.Height -= diameterstep;

            }
            //射线数
            int linecount = values.Rows.Count;
            if (linecount > 0)
            {
                //计算角度
                float angle = 0;
                float anglestep = 360f / linecount;

                for (int i = 0; i < linecount; i++)
                {
                    //终点
                    PointF endPoint = new PointF((float)(radius * Math.Cos(angle * Math.PI / 180) + center.X), (float)(radius * Math.Sin(angle * Math.PI / 180) + center.Y));
                    g.DrawLine(new Pen(Color.FromArgb(82, 137, 247)), center, endPoint);
                    // 画名字，属性
                    string anglestr = attr[i];
                    PointF textpoint = endPoint;
                    if (angle == 270)
                    {
                        textpoint.Y -= 20;
                        textpoint.X -= anglestr.Length * 12 / 2;
                    }
                    else if (angle < 270 && angle > 180)
                    {
                        textpoint.X -= (anglestr.Length + 1) * 12;
                        textpoint.Y -= 10;
                    }
                    else if (angle == 180)
                    {
                        textpoint.X -= (anglestr.Length + 1) * 12;
                        textpoint.Y -= 5;
                    }
                    else if (angle < 180 && angle > 90)
                    {
                        textpoint.X -= (anglestr.Length + 1) * 12;
                        //textpoint.Y += 5;
                    }
                    else if (angle == 90)
                    {
                        textpoint.X -= anglestr.Length * 12 / 2;
                        textpoint.Y += 10;
                    }
                    //else if (angle > 0 && angle < 90)
                    //{
                    //    textpoint.X -= 5;
                    //    textpoint.Y += 5;
                    //}
                    else if (angle > 270 && angle < 360)
                    {
                        textpoint.Y -= 10;
                    }
                    else if (angle == 0)
                    {
                        textpoint.X += 5;
                        textpoint.Y -= 5;
                    }
                    g.DrawString(anglestr, font2, new SolidBrush(Color.FromArgb(82, 137, 247)), textpoint);
                    angle += anglestep;
                    angle %= 360;
                }
            }

            List<PointF[]> pointfs = new List<PointF[]>();
            for (int j = 1; j < values.Columns.Count; j++)
            {
                PointF[] points = new PointF[values.Rows.Count + 1];
                PointF NextPoint;
                //计算角度
                float anglex = 0;
                float anglestepx = 360f / values.Rows.Count;
                float nextang = 0;
                for (int i = 0; i < values.Rows.Count; i++)
                {
                    float r = radius * (Convert.ToSingle(values.Rows[i][j])) / maxValue;
                    nextang += anglestepx;
                    if ((i + 1) < values.Rows.Count)
                    {
                        float r1 = radius * (Convert.ToSingle(values.Rows[i + 1][j])) / maxValue;
                        NextPoint = new PointF((float)(r1 * Math.Cos(nextang * Math.PI / 180) + center.X), (float)(r1 * Math.Sin(nextang * Math.PI / 180) + center.Y));
                        points[i + 1] = NextPoint;

                    }
                    else
                    {
                        float r2 = radius * (Convert.ToSingle(values.Rows[0][j])) / maxValue;
                        NextPoint = new PointF((float)(r2 * Math.Cos(0 * Math.PI / 180) + center.X), (float)(r2 * Math.Sin(0 * Math.PI / 180) + center.Y));
                        points[i + 1] = NextPoint;

                    }
                    points[i] = new PointF((float)(r * Math.Cos(anglex * Math.PI / 180) + center.X), (float)(r * Math.Sin(anglex * Math.PI / 180) + center.Y));

                    anglex += anglestepx;
                    anglex %= 360;
                }
                pointfs.Add(points);
            }
            byte[] bytes = new byte[values.Rows.Count + 1];
            for (int bt = 0; bt <= values.Rows.Count; bt++)
            {
                if (bt == 0)
                {
                    bytes[bt] = (byte)PathPointType.Start;
                }
                else
                {
                    bytes[bt] = (byte)PathPointType.Line;
                }
            }
            int colsindex = 0;
            int dataindex = 1;
            foreach (PointF[] ps in pointfs)
            {
                GraphicsPath path = new GraphicsPath(ps, bytes);
                g.DrawPath(new Pen(colors[colsindex]), path);
                g.FillPath(new SolidBrush(colors1[colsindex]), path);
                for (int pti = 0; pti < ps.Length - 1; pti++)
                {
                    g.DrawString(values.Rows[pti][dataindex].ToString(), new Font("Arial", 8), new SolidBrush(colors[colsindex]), ps[pti]);
                }
                colsindex++;
                dataindex++;
            }
            CreateCaption(capname, g, 90, 450, "雷达图", colors);
            return image;


        }
        #endregion
    }
}
