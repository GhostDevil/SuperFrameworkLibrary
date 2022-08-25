using System.Drawing;
using System.Drawing.Imaging;

namespace SuperFramework.SuperImage
{
    /// <summary>
    /// 日 期:2016-07-25
    /// 作 者:不良帥
    /// 描 述:图片效果代理类
    /// </summary>
    public static class ImageHandleOperate
    {
        #region 灰化图片
        /// <summary>
        /// 灰化图片
        /// </summary>
        /// <param name="img">图片对象</param>
        /// <returns>返回处理后的Bitmap对象</returns>
        public static Bitmap ImageAshing(Image img)
        {
            if (img != null)
            {
                int Height = img.Height;
                int Width = img.Width;
                Bitmap bitmap = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
                Bitmap MyBitmap = (Bitmap)img;

                BitmapData oldData = MyBitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                BitmapData newData = bitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                unsafe
                {
                    int x = 0, y = 0;
                    byte* pin = (byte*)(oldData.Scan0.ToPointer());
                    byte* pout = (byte*)(newData.Scan0.ToPointer());
                    for (y = 0; y < oldData.Height; y++)
                    {
                        for (x = 0; x < oldData.Width; x++)
                        {
                            byte Result = (byte)(pin[0] * 0.1 + pin[1] * 0.2 + pin[2] * 0.7);//加权平均实现灰化
                            pout[0] = Result;
                            pout[1] = Result;
                            pout[2] = Result;
                            pin += 3;
                            pout += 3;
                        }
                        pin += oldData.Stride - oldData.Width * 3;
                        pout += newData.Stride - newData.Width * 3;
                    }
                    bitmap.UnlockBits(newData);
                    MyBitmap.UnlockBits(oldData);
                    return bitmap;
                }

            }

            return null;
        }
        #endregion

        #region 柔化图片
        /// <summary>
        /// 柔化图片
        /// </summary>
        /// <param name="img">图片对象</param>
        /// <returns>返回处理后的Bitmap对象</returns>
        public static Bitmap ImageSoften(Image img)
        {
            if (img != null)
            {
                int Height = img.Height;
                int Width = img.Width;
                Bitmap bitmap = new Bitmap(Width, Height, PixelFormat.Format32bppRgb);
                Bitmap MyBitmap = (Bitmap)img;

                BitmapData oldData = MyBitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
                BitmapData newData = bitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
                unsafe
                {
                    byte* pin = (byte*)(oldData.Scan0.ToPointer());
                    byte* pout = (byte*)(newData.Scan0.ToPointer());
                    //高斯模板
                    int[] Gauss = { 1, 2, 1, 2, 4, 2, 1, 2, 1 };
                    for (int i = 1; i < Width - 1; i++)
                    {
                        for (int j = 1; j < Height - 1; j++)
                        {
                            int r = 0, g = 0, b = 0;
                            int Index = 0;

                            for (int col = -1; col <= 1; col++)
                            {
                                for (int row = -1; row <= 1; row++)
                                {
                                    int off = ((j + row) * (Width) + (i + col)) * 4;
                                    r += pin[off + 0] * Gauss[Index];
                                    g += pin[off + 1] * Gauss[Index];
                                    b += pin[off + 2] * Gauss[Index];
                                    Index++;
                                }
                            }
                            r /= 16;
                            g /= 16;
                            b /= 16;
                            //处理颜色值溢出
                            if (r < 0) r = 0;
                            if (r > 255) r = 255;
                            if (g < 0) g = 0;
                            if (g > 255) g = 255;
                            if (b < 0) b = 0;
                            if (b > 255) b = 255;
                            int off2 = (j * Width + i) * 4;
                            pout[off2 + 0] = (byte)r;
                            pout[off2 + 1] = (byte)g;
                            pout[off2 + 2] = (byte)b;
                        }
                    }
                    bitmap.UnlockBits(newData);
                    MyBitmap.UnlockBits(oldData);
                    return bitmap;
                }




            }
            return null;

        }
        #endregion

        #region 锐化图片，显示数值最大像素点
        /// <summary>
        /// 锐化图片，显示数值最大像素点
        /// </summary>
        /// <param name="img">图片对象</param>
        /// <returns>返回处理后的Bitmap对象</returns>
        public static Bitmap ImageSharpen(Image img)
        {
            if (img != null)
            {
                int Height =img.Height;
                int Width =img.Width;
                Bitmap bitmap = new Bitmap(Width, Height, PixelFormat.Format32bppRgb);
                Bitmap MyBitmap = (Bitmap)img;

                BitmapData oldData = MyBitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
                BitmapData newData = bitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
                unsafe
                {
                    byte* pin = (byte*)(oldData.Scan0.ToPointer());
                    byte* pout = (byte*)(newData.Scan0.ToPointer());
                    //拉普拉斯模板
                    int[] Laplacian = { -1, -1, -1, -1, 9, -1, -1, -1, -1 };
                    for (int i = 1; i < Width - 1; i++)
                    {
                        for (int j = 1; j < Height - 1; j++)
                        {
                            int r = 0, g = 0, b = 0;
                            int Index = 0;

                            for (int col = -1; col <= 1; col++)
                            {
                                for (int row = -1; row <= 1; row++)
                                {
                                    int off = ((j + row) * (Width) + (i + col)) * 4;
                                    r += pin[off + 0] * Laplacian[Index];
                                    g += pin[off + 1] * Laplacian[Index];
                                    b += pin[off + 2] * Laplacian[Index];
                                    Index++;
                                }
                            }

                            if (r < 0) r = 0;
                            if (r > 255) r = 255;
                            if (g < 0) g = 0;
                            if (g > 255) g = 255;
                            if (b < 0) b = 0;
                            if (b > 255) b = 255;
                            int off2 = (j * Width + i) * 4;
                            pout[off2 + 0] = (byte)r;
                            pout[off2 + 1] = (byte)g;
                            pout[off2 + 2] = (byte)b;
                        }
                    }
                    bitmap.UnlockBits(newData);
                    MyBitmap.UnlockBits(oldData);
                   return bitmap;
                }

            }
            return null;
        }
        #endregion

        #region 浮雕图片
        /// <summary>
        /// 浮雕图片
        /// </summary>
        /// <param name="img">图片对象</param>
        /// <returns>返回处理后的Bitmap对象</returns>
        public static Bitmap ImageRelief(Image img)
        {
            if (img != null)
            {

                int Height =img.Height;
                int Width =img.Width;
                Bitmap bitmap = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
                Bitmap MyBitmap = (Bitmap)img;
                BitmapData oldData = MyBitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                BitmapData newData = bitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                unsafe
                {
                    byte* pin_1 = (byte*)(oldData.Scan0.ToPointer());
                    byte* pin_2 = pin_1 + (oldData.Stride);
                    byte* pout = (byte*)(newData.Scan0.ToPointer());
                    for (int y = 0; y < oldData.Height - 1; y++)
                    {
                        for (int x = 0; x < oldData.Width; x++)
                        {
                            int b = pin_1[0] - pin_2[0] + 128;
                            int g = pin_1[1] - pin_2[1] + 128;
                            int r = pin_1[2] - pin_2[2] + 128;

                            if (r < 0) r = 0;
                            if (r > 255) r = 255;
                            if (g < 0) g = 0;
                            if (g > 255) g = 255;
                            if (b < 0) b = 0;
                            if (b > 255) b = 255;
                            pout[0] = (byte)(b);
                            pout[1] = (byte)(g);
                            pout[2] = (byte)(r);
                            pin_1 += 3;
                            pin_2 += 3;
                            pout += 3;
                        }
                        pin_1 += oldData.Stride - oldData.Width * 3;
                        pin_2 += oldData.Stride - oldData.Width * 3;
                        pout += newData.Stride - newData.Width * 3;
                    }
                    bitmap.UnlockBits(newData);
                    MyBitmap.UnlockBits(oldData);
                  return bitmap;
                }

            }
            return null;
        }
        #endregion

        #region 底片图片
        /// <summary>
        /// 底片图片
        /// </summary>
        /// <param name="img">图片对象</param>
        /// <returns>返回处理后的Bitmap对象</returns>
        public static Bitmap ImageNegative(Image img)
        {
            if (img != null)
            {
                int Height =img.Height;
                int Width =img.Width;
                Bitmap bitmap = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
                Bitmap MyBitmap = (Bitmap)img;
                BitmapData oldData = MyBitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                BitmapData newData = bitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                unsafe
                {
                    byte* pin = (byte*)(oldData.Scan0.ToPointer());
                    byte* pout = (byte*)(newData.Scan0.ToPointer());
                    for (int y = 0; y < oldData.Height; y++)
                    {
                        for (int x = 0; x < oldData.Width; x++)
                        {
                            pout[0] = (byte)(255 - pin[0]);
                            pout[1] = (byte)(255 - pin[1]);
                            pout[2] = (byte)(255 - pin[2]);
                            pin += 3;
                            pout += 3;
                        }
                        pin += oldData.Stride - oldData.Width * 3;
                        pout += newData.Stride - newData.Width * 3;
                    }
                    bitmap.UnlockBits(newData);
                    MyBitmap.UnlockBits(oldData);
                  return bitmap;
                }
            }
            return null;
        }
        #endregion

        #region 积木图片
        /// <summary>
        /// 积木图片
        /// </summary>
        /// <param name="img">图片对象</param>
        /// <returns>返回处理后的Bitmap对象</returns>
        public static Bitmap ImageBlock(Image img)
        {
            if (img != null)
            {
                int Height =img.Height;
                int Width =img.Width;
                Bitmap bitmap = new Bitmap(Width, Height);
                Bitmap Mybitmap = (Bitmap)img;
                BitmapData oldData = Mybitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                BitmapData newData = bitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                unsafe
                {
                    byte* pin = (byte*)(oldData.Scan0.ToPointer());
                    byte* pout = (byte*)(newData.Scan0.ToPointer());
                    for (int y = 0; y < oldData.Height; y++)
                    {
                        for (int x = 0; x < oldData.Width; x++)
                        {
                            int avg = (pin[0] + pin[1] + pin[2]) / 3;
                            if (avg > 128)
                            {
                                pout[0] = 255;
                                pout[1] = 255;
                                pout[2] = 255;
                            }
                            else
                            {
                                pout[0] = 0;
                                pout[1] = 0;
                                pout[2] = 0;
                            }
                            pin += 3;
                            pout += 3;
                        }
                        pin = pin + oldData.Stride - oldData.Width * 3;
                        pout = pout + newData.Stride - newData.Width * 3;
                    }
                    bitmap.UnlockBits(newData);
                    Mybitmap.UnlockBits(oldData);
                  return bitmap;

                }
            }
            return null;
        }
        #endregion

        #region 边缘提取
        /// <summary>
        /// 边缘提取
        /// </summary>
        /// <param name="img">图片对象</param>
        /// <returns>返回处理后的Bitmap对象</returns>
        public static Bitmap ImageEdge(Image img)
        {
            if (img != null)
            {

                int Height =img.Height;
                int Width =img.Width;
                Bitmap bitmap = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
                Bitmap MyBitmap = (Bitmap)img;
                BitmapData oldData = MyBitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb); //原图
                BitmapData newData = bitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);  //新图即边缘图
                unsafe
                {
                    //首先第一段代码是提取边缘，边缘置为黑色，其他部分置为白色
                    byte* pin_1 = (byte*)(oldData.Scan0.ToPointer());
                    byte* pin_2 = pin_1 + (oldData.Stride);
                    byte* pout = (byte*)(newData.Scan0.ToPointer());
                    for (int y = 0; y < oldData.Height - 1; y++)
                    {
                        for (int x = 0; x < oldData.Width; x++)
                        {
                            //使用robert算子
                            double b = System.Math.Sqrt((pin_1[0] - (double)(pin_2[0] + 3)) * (pin_1[0] - (double)(pin_2[0] + 3)) + (pin_1[0] + 3 - (double)pin_2[0]) * (pin_1[0] + 3 - (double)pin_2[0]));
                            double g = System.Math.Sqrt((pin_1[1] - (double)(pin_2[1] + 3)) * (pin_1[1] - (double)(pin_2[1] + 3)) + (pin_1[1] + 3 - (double)pin_2[1]) * (pin_1[1] + 3 - (double)pin_2[1]));
                            double r = System.Math.Sqrt((pin_1[2] - (double)(pin_2[2] + 3)) * (pin_1[2] - (double)(pin_2[2] + 3)) + (pin_1[2] + 3 - (double)pin_2[2]) * (pin_1[2] + 3 - (double)pin_2[2]));
                            double bgr = b + g + r;//博主一直在纠结要不要除以3，感觉没差，选阈值的时候调整一下就好了- -

                            if (bgr > 80) //阈值，超过阈值判定为边缘（选取适当的阈值）
                            {
                                b = 0;
                                g = 0;
                                r = 0;
                            }
                            else
                            {
                                b = 255;
                                g = 255;
                                r = 255;
                            }
                            pout[0] = (byte)(b);
                            pout[1] = (byte)(g);
                            pout[2] = (byte)(r);
                            pin_1 += 3;
                            pin_2 += 3;
                            pout += 3;

                        }
                        pin_1 += oldData.Stride - oldData.Width * 3;
                        pin_2 += oldData.Stride - oldData.Width * 3;
                        pout += newData.Stride - newData.Width * 3;
                    }

                    //这里博主加粗了一下线条- -，不喜欢的同学可以删了这段代码
                    byte* pin_5 = (byte*)(newData.Scan0.ToPointer());
                    for (int y = 0; y < oldData.Height - 1; y++)
                    {
                        for (int x = 3; x < oldData.Width; x++)
                        {
                            if (pin_5[0] == 0 && pin_5[1] == 0 && pin_5[2] == 0)
                            {
                                pin_5[-3] = 0;
                                pin_5[-2] = 0;
                                pin_5[-1] = 0;      //边缘点的前一个像素点置为黑色（注意一定要是遍历过的像素点）                                                    
                            }
                            pin_5 += 3;

                        }
                        pin_5 += newData.Stride - newData.Width * 3;
                    }

                    //这段代码是把原图和边缘图重合
                    byte* pin_3 = (byte*)(oldData.Scan0.ToPointer());
                    byte* pin_4 = (byte*)(newData.Scan0.ToPointer());
                    for (int y = 0; y < oldData.Height - 1; y++)
                    {
                        for (int x = 0; x < oldData.Width; x++)
                        {
                            if (pin_4[0] == 255 && pin_4[1] == 255 && pin_4[2] == 255)
                            {
                                pin_4[0] = pin_3[0];
                                pin_4[1] = pin_3[1];
                                pin_4[2] = pin_3[2];
                            }
                            pin_3 += 3;
                            pin_4 += 3;
                        }
                        pin_3 += oldData.Stride - oldData.Width * 3;
                        pin_4 += newData.Stride - newData.Width * 3;
                    }
                    //......
                    bitmap.UnlockBits(newData);
                    MyBitmap.UnlockBits(oldData);
                  return bitmap;
                }

            }
            return null;
        }
        #endregion
    }
}
