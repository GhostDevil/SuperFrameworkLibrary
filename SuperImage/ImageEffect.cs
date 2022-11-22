using System.Drawing;
using System.Drawing.Imaging;

namespace SuperFramework.SuperImage
{
    /// <summary>
    /// 版 本:Release
    /// 日 期:2014-09-23
    /// 作 者:不良帥
    /// 描 述:图片特效辅助类
    /// </summary>
    public class ImageEffect
    {
        #region 矩形图片变换成梯形图片，用于QQ的3D翻转特效--含不安全代码
        /// <summary>
        /// 矩形图片变换成梯形图片，用于QQ的3D翻转特效--含不安全代码
        /// </summary>
        /// <param name="src">原图</param>
        /// <param name="compressH">左侧面或者右侧面纵向缩放的比例</param>
        /// <param name="compressW">横向缩放的比例</param>
        /// <param name="isLeft">是否是左侧面缩放</param>
        /// <param name="isCenter">是否水平居中</param>
        /// <returns>返回位图对象</returns>
        public static Bitmap TrapezoidTransformation(Bitmap src, double compressH, double compressW, bool isLeft, bool isCenter)
        {
            Rectangle rect = new(0, 0, src.Width, src.Height);
            using (Bitmap resultH = new(rect.Width, rect.Height))
            {
                Bitmap resultW = new(rect.Width, rect.Height);

                #region 指针算法，高速
                //LockBits将Bitmap锁定到内存中
                BitmapData srcData = src.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                BitmapData resultHData = resultH.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                BitmapData resultWData = resultW.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                unsafe
                {
                    //指向地址（分量顺序是BGRA）
                    byte* srcP = (byte*)srcData.Scan0;//原图地址
                    byte* resultHP = (byte*)resultHData.Scan0;//侧面压缩结果图像地址
                    byte* resultWP = (byte*)resultWData.Scan0;//纵向压缩
                    int dataW = srcData.Stride;//每行的数据量

                    double changeY = (1.0 - compressH) * rect.Height / 2 / rect.Width;//Y变化率

                    for (int y = 0; y < rect.Height; y++)
                    {
                        for (int x = 0; x < rect.Width; x++)
                        {
                            double h = 0;
                            double nH = 0;
                            if (isLeft && (y >= changeY * (rect.Width - x)))
                            {
                                h = rect.Height - 2.0 * changeY * (rect.Width - x);//变化之后的每竖像素高度
                                nH = y - (changeY * (rect.Width - x));//当前像素在变化之后的高度
                            }
                            else if (!isLeft && (y >= changeY * x))
                            {
                                h = rect.Height - 2.0 * changeY * x;//变化之后的每竖像素高度
                                nH = y - (changeY * x);//当前像素在变化之后的高度
                            }

                            double p = 1.0 * nH / h;//当前像素在变化之后的位置高度百分比
                            int nY = (int)(rect.Height * p);//变化之后像素在原图片中的Y轴位置

                            if (nY < rect.Height && nY > -1)
                            {
                                //result.SetPixel(x + offsetX, y, src.GetPixel(nX, nY));
                                byte* sp = srcP + nY * dataW + x * 4;//原图的像素偏移的数据位置

                                resultHP[0] = sp[0];
                                resultHP[1] = sp[1];
                                resultHP[2] = sp[2];
                                resultHP[3] = sp[3];
                            }

                            resultHP += 4;
                        } // x
                    } // y

                    resultHP = (byte*)resultHData.Scan0;//重置地址

                    //纵向压缩
                    int offsetX = 0;//居中偏移
                    if (isCenter)
                    {
                        offsetX = (int)((rect.Width - compressW * rect.Width) / 2);
                    }

                    for (int y = 0; y < rect.Height; y++)
                    {
                        for (int x = 0; x < rect.Width; x++)
                        {
                            int nX = (int)(1.0 * x / compressW);//纵向压缩后像素在原图片中的X轴位置
                            if (nX > -1 && nX < rect.Width)
                            {
                                //resultW.SetPixel(x, y, resultH.GetPixel(nX, y));
                                byte* hp = resultHP + nX * 4 + dataW * y;
                                byte* wp = resultWP + offsetX * 4;

                                wp[0] = hp[0];
                                wp[1] = hp[1];
                                wp[2] = hp[2];
                                wp[3] = hp[3];
                            }

                            resultWP += 4;
                        }
                    }


                    src.UnlockBits(srcData);//从内存中解除锁定
                    resultH.UnlockBits(resultHData);
                    resultW.UnlockBits(resultWData);

                }

                #endregion

                #region 位图像素设置，低速

                ////侧面压缩
                //double changeY = (1.0 - compressH) * rect.Height / 2 / rect.Width;//Y变化率

                //for (int y = 0; y < rect.Height; y++)
                //{
                //    for (int x = 0; x < rect.Width; x++)
                //    {
                //        double h = 0;
                //        double nH = 0;
                //        if (isLeft && (y >= changeY * (rect.Width - x)))
                //        {
                //            h = rect.Height - 2.0 * changeY * (rect.Width - x);//变化之后的每竖像素高度
                //            nH = y - (changeY * (rect.Width - x));//当前像素在变化之后的高度

                //        }
                //        else if (!isLeft && (y >= changeY * x))
                //        {
                //            h = rect.Height - 2.0 * changeY * x;//变化之后的每竖像素高度
                //            nH = y - (changeY * x);//当前像素在变化之后的高度
                //        }

                //        double p = 1.0 * nH / h;//当前像素在变化之后的位置高度百分比
                //        int nY = (int)(rect.Height * p);//变化之后像素在原图片中的Y轴位置

                //        if (nY < rect.Height && nY > -1)
                //        {
                //            resultH.SetPixel(x, y, src.GetPixel(x, nY));
                //        }
                //    } // x
                //} // y

                //// 纵向压缩
                //int offsetX = 0;
                //if (isCenter)
                //{
                //    offsetX = (int)((rect.Width - compressW * rect.Width) / 2);
                //}
                //for (int y = 0; y < rect.Height; y++)
                //{
                //    for (int x = 0; x < rect.Width; x++)
                //    {
                //        int nX = (int)(1.0 * x / compressW);//纵向压缩后像素在原图片中的X轴位置
                //        if (nX > -1 && nX < rect.Width && x + offsetX < rect.Width)
                //        {
                //            resultW.SetPixel(x + offsetX, y, resultH.GetPixel(nX, y));
                //        }
                //    }
                //}

                #endregion

                return resultW;
            }
        }
        #endregion

    }
}
