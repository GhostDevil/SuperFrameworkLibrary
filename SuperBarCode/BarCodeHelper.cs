using System;
using System.Drawing;

namespace SuperFramework.SuperBarCode
{
    /// <summary>
    /// 日 期:2014-09-23
    /// 作 者:不良帥
    /// 描 述:条形码类
    /// </summary>
    public class BarCodeHelper
    {
        private static double[] AddVector(double[] a, double[] b) => new double[] { a[0] + b[0], a[1] + b[1], a[2] + b[2] };

        private static double[] ScalarProduct(double[] vector, double scalar) => new double[] { vector[0] * scalar, vector[1] * scalar, vector[2] * scalar };

        private static double DotProduct(double[] a, double[] b) => a[0] * b[0] + a[1] * b[1] + a[2] * b[2];

        private static double Norm(double[] vector) => Math.Sqrt(DotProduct(vector, vector));

        private static double[] Normalize(double[] vector) => ScalarProduct(vector, 1.0 / Norm(vector));

        private static double[] CrossProduct(double[] a, double[] b)
        => new double[] { (a[1] * b[2] - a[2] * b[1]), (a[2] * b[0] - a[0] * b[2]), (a[0] * b[1] - a[1] * b[0]) };

        private static double[] VectorProductIndexed(double[] v, double[] m, int i)
        => new double[] { v[i + 0] * m[0] + v[i + 1] * m[4] + v[i + 2] * m[8] + v[i + 3] * m[12], v[i + 0] * m[1] + v[i + 1] * m[5] + v[i + 2] * m[9] + v[i + 3] * m[13], v[i + 0] * m[2] + v[i + 1] * m[6] + v[i + 2] * m[10] + v[i + 3] * m[14], v[i + 0] * m[3] + v[i + 1] * m[7] + v[i + 2] * m[11] + v[i + 3] * m[15] };

        private static double[] VectorProduct(double[] v, double[] m)
        => VectorProductIndexed(v, m, 0);

        private static double[] MatrixProduct(double[] a, double[] b)
        {
            double[] o1 = VectorProductIndexed(a, b, 0);
            double[] o2 = VectorProductIndexed(a, b, 4);
            double[] o3 = VectorProductIndexed(a, b, 8);
            double[] o4 = VectorProductIndexed(a, b, 12);

            return new double[]
                {
                    o1[0], o1[1], o1[2], o1[3],
                    o2[0], o2[1], o2[2], o2[3],
                    o3[0], o3[1], o3[2], o3[3],
                    o4[0], o4[1], o4[2], o4[3]
                };
        }

        private static double[] CameraTransform(double[] C, double[] A)
        {
            double[] w = Normalize(AddVector(C, ScalarProduct(A, -1)));
            double[] y = new double[] { 0, 1, 0 };
            double[] u = Normalize(CrossProduct(y, w));
            double[] v = CrossProduct(w, u);
            double[] t = ScalarProduct(C, -1);

            return new double[]
                {
                    u[0], v[0], w[0], 0,
                    u[1], v[1], w[1], 0,
                    u[2], v[2], w[2], 0,
                    DotProduct(u, t), DotProduct(v, t), DotProduct(w, t), 1
                };
        }

        private static double[] ViewingTransform(double fov, double n, double f)
        {
            fov *= (Math.PI / 180);
            double cot = 1.0 / Math.Tan(fov / 2);
            return new double[] { cot, 0, 0, 0, 0, cot, 0, 0, 0, 0, (f + n) / (f - n), -1, 0, 0, 2 * f * n / (f - n), 0 };
        }
        /// <summary>
        /// 生成条码
        /// </summary>
        /// <param name="captchaText">内容</param>
        /// <returns>返回条码图片</returns>
        public static Image Generate(string captchaText)
        {
            int fontsize = 24;
            Font font = new Font("Arial", fontsize);

            SizeF sizeF;
            using (Graphics g = Graphics.FromImage(new Bitmap(1, 1)))
            {
                sizeF = g.MeasureString(captchaText, font, 0, StringFormat.GenericDefault);
            }

            int image2d_x = (int)sizeF.Width;
            int image2d_y = (int)(fontsize * 1.3);

            Bitmap image2d = new Bitmap(image2d_x, image2d_y);
            Color black = Color.Black;
            Color white = Color.White;

            using (Graphics g = Graphics.FromImage(image2d))
            {
                g.Clear(black);
                g.DrawString(captchaText, font, Brushes.White, 0, 0);
            }

            Random rnd = new Random();
            double[] T = CameraTransform(new double[] { rnd.Next(-90, 90), -200, rnd.Next(150, 250) }, new double[] { 0, 0, 0 });
            T = MatrixProduct(T, ViewingTransform(60, 300, 3000));

            double[][] coord = new double[image2d_x * image2d_y][];

            int count = 0;
            for (int y = 0; y < image2d_y; y += 2)
            {
                for (int x = 0; x < image2d_x; x++)
                {
                    int xc = x - image2d_x / 2;
                    int zc = y - image2d_y / 2;
                    double yc = -(double)(image2d.GetPixel(x, y).ToArgb() & 0xff) / 256 * 4;
                    double[] xyz = new double[] { xc, yc, zc, 1 };
                    xyz = VectorProduct(xyz, T);
                    coord[count] = xyz;
                    count++;
                }
            }

            int image3d_x = 256;
            int image3d_y = image3d_x * 9 / 16;
            Bitmap image3d = new Bitmap(image3d_x, image3d_y);
            Color fgcolor = Color.White;
            Color bgcolor = Color.Black;
            using (Graphics g = Graphics.FromImage(image3d))
            {
                g.Clear(bgcolor);
                count = 0;
                double scale = 1.75 - (double)image2d_x / 400;
                for (int y = 0; y < image2d_y; y += 2)
                {
                    for (int x = 0; x < image2d_x; x++)
                    {
                        if (x > 0)
                        {
                            double x0 = coord[count - 1][0] * scale + image3d_x / 2;
                            double y0 = coord[count - 1][1] * scale + image3d_y / 2;
                            double x1 = coord[count][0] * scale + image3d_x / 2;
                            double y1 = coord[count][1] * scale + image3d_y / 2;
                            g.DrawLine(new Pen(fgcolor), (float)x0, (float)y0, (float)x1, (float)y1);
                        }
                        count++;
                    }
                }
            }
            return image3d;
        }
    }
}
