using SuperFramework.WindowsAPI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using static SuperFramework.SuperImage.ImageEnum;
using static SuperFramework.WindowsAPI.User32API;

namespace SuperFramework.SuperImage
{
    /// <summary>
    /// <para>日 期:2016-09-05</para>
    /// <para>作 者:不良帥</para>
    /// <para>描 述:图片操作类</para>
    /// </summary>
    public static class ImageHelper
    {

        //private static Hashtable htmimes = new Hashtable();
        /// <summary>
        /// 图片格式过滤
        /// </summary>
        internal static readonly string AllowExt = ".jpe|.jpeg|.jpg|.png|.tif|.tiff|.bmp";

        #region 生成缩略图
        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="originalImagePath">源图路径（物理路径）</param>
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="mode">生成缩略图的方式</param>    
        /// <returns>成功返回true，失败返回false</returns>
        public static bool MakeThumbnail(string originalImagePath, int width, int height, ThumbnailMod mode)
        {
            //System.Drawing.Image originalImage = System.Drawing.Image.FromFile(originalImagePath);

            //int towidth = width;
            //int toheight = height;

            //int x = 0;
            //int y = 0;
            //int ow = originalImage.Width;
            //int oh = originalImage.Height;

            //switch (mode)
            //{
            //    case ImageMode.HW:  //指定高宽缩放（可能变形）                
            //        break;
            //    case ImageMode.W:   //指定宽，高按比例                    
            //        toheight = originalImage.Height * width / originalImage.Width;
            //        break;
            //    case ImageMode.H:   //指定高，宽按比例
            //        towidth = originalImage.Width * height / originalImage.Height;
            //        break;
            //    case ImageMode.Cut: //指定高宽裁减（不变形）                
            //        if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
            //        {
            //            oh = originalImage.Height;
            //            ow = originalImage.Height * towidth / toheight;
            //            y = 0;
            //            x = (originalImage.Width - ow) / 2;
            //        }
            //        else
            //        {
            //            ow = originalImage.Width;
            //            oh = originalImage.Width * height / towidth;
            //            x = 0;
            //            y = (originalImage.Height - oh) / 2;
            //        }
            //        break;
            //    default:
            //        break;
            //}

            ////新建一个bmp图片
            //System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight);

            ////新建一个画板
            //System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            ////设置高质量插值法
            //g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

            ////设置高质量,低速度呈现平滑程度
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            ////清空画布并以透明背景色填充
            //g.Clear(System.Drawing.Color.Transparent);

            ////在指定位置并且按指定大小绘制原图片的指定部分
            //g.DrawImage(originalImage, new System.Drawing.Rectangle(0, 0, towidth, toheight), new System.Drawing.Rectangle(x, y, ow, oh), System.Drawing.GraphicsUnit.Pixel);

            //try
            //{
            //    //以jpg格式保存缩略图
            //    bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg);
            //}
            //catch (System.Exception e)
            //{
            //    throw e;
            //}
            //finally
            //{
            //    originalImage.Dispose();
            //    bitmap.Dispose();
            //    g.Dispose();
            //}

            string thumbnailPath = originalImagePath.Substring(0, originalImagePath.LastIndexOf('.')) + "s.jpg";
            Image originalImage = Image.FromFile(originalImagePath);

            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;

            switch (mode)
            {
                case ThumbnailMod.HW://指定高宽缩放（可能变形）                
                    break;
                case ThumbnailMod.W://指定宽，高按比例                    
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case ThumbnailMod.H://指定高，宽按比例
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case ThumbnailMod.Cut://指定高宽裁减（不变形）                
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }

            //新建一个bmp图片
            Image bitmap = new Bitmap(towidth, toheight);

            //新建一个画板
            Graphics g = Graphics.FromImage(bitmap);

            //设置高质量插值法
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //清空画布并以透明背景色填充
            g.Clear(System.Drawing.Color.Transparent);

            //在指定位置并且按指定大小绘制原图片的指定部分
            g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight),
                new Rectangle(x, y, ow, oh),
                GraphicsUnit.Pixel);
            bool isok = false;
            try
            {
                //以jpg格式保存缩略图
                bitmap.Save(thumbnailPath, ImageFormat.Jpeg);
                isok = true;
            }
            catch (Exception)
            {
                thumbnailPath = originalImagePath;
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
            return isok;
        }
        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="originalImagePath">源图路径（物理路径）</param>
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="mode">生成缩略图的方式</param>    
        public static void MakeThumbnail(string originalImagePath, string thumbnailPath, int width, int height, string mode)
        {
            Image originalImage = System.Drawing.Image.FromFile(originalImagePath);

            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;

            switch (mode)
            {
                case "HW":  //指定高宽缩放（可能变形）                
                    break;
                case "W":   //指定宽，高按比例                    
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case "H":   //指定高，宽按比例
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case "Cut": //指定高宽裁减（不变形）                
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }

            //新建一个bmp图片
            Image bitmap = new Bitmap(towidth, toheight);

            //新建一个画板
            Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            //设置高质量插值法
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //清空画布并以透明背景色填充
            g.Clear(System.Drawing.Color.Transparent);

            //在指定位置并且按指定大小绘制原图片的指定部分
            g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight), new Rectangle(x, y, ow, oh), System.Drawing.GraphicsUnit.Pixel);

            try
            {
                //以jpg格式保存缩略图
                bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
        }
        #endregion

        #region 获取图片中的各帧
        /// <summary>
        /// 获取图片中的各帧
        /// </summary>
        /// <param name="pPath">图片路径</param>
        /// <param name="pSavedPath">保存路径</param>
        public static void GetFrames(string pPath, string pSavedPath)
        {
            Image gif = Image.FromFile(pPath);
            FrameDimension fd = new FrameDimension(gif.FrameDimensionsList[0]);
            int count = gif.GetFrameCount(fd); //获取帧数(gif图片可能包含多帧，其它格式图片一般仅一帧)
            for (int i = 0; i < count; i++)    //以Jpeg格式保存各帧
            {
                gif.SelectActiveFrame(fd, i);
                gif.Save(string.Format("{0}\\frame_{1}.jpg", pSavedPath, i), ImageFormat.Jpeg);
            }
        }
        #endregion

        #region Bytes2Int函数
        /// <summary>
        /// Bytes2Int函数（重载）
        /// </summary>
        /// <param name="H">一个字节高位在前</param>
        /// <param name="L">一个字节低位在前</param>
        /// <param name="IsLowHigh"></param>
        /// <returns></returns>
        private static int Bytes2Int(byte H, byte L, bool IsLowHigh)
        {
            if (IsLowHigh)
            {
                return Convert.ToInt32(L.ToString("X2") + H.ToString("X2"), 16);
            }
            else
            {
                return Convert.ToInt32(H.ToString("X2") + L.ToString("X2"), 16);
            }
        }
        /// <summary>
        /// Bytes2Int函数（重载）
        /// </summary>
        /// <param name="HH"></param>
        /// <param name="HL"></param>
        /// <param name="LH"></param>
        /// <param name="LL"></param>
        /// <param name="IsLowHigh"></param>
        /// <returns></returns>
        private static int Bytes2Int(byte HH, byte HL, byte LH, byte LL, bool IsLowHigh)
        {
            if (IsLowHigh)
            {
                return Convert.ToInt32(LL.ToString("X2") + LH.ToString("X2") + HL.ToString("X2") + HH.ToString("X2"), 16);
            }
            else
            {
                return Convert.ToInt32(HH.ToString("X2") + HL.ToString("X2") + LH.ToString("X2") + LL.ToString("X2"), 16);
            }
        }
        #endregion

        #region  获得JPEG格式文件尺寸
        /// <summary>
        /// 获得JPEG格式文件尺寸
        /// </summary>
        /// <param name="h">文件头数组</param>
        /// <returns>文件尺寸，Point.X存储图像宽度，Point.Y存储图像高度</returns>
        public static Point GetJpegSize(byte[] h)
        {
            Point size = new Point(0, 0);
            for (byte j = 0xC0; j <= 0xCF; j++)
            {
                for (int i = 0; i < h.Length - 8; i++)
                {
                    if (h[i] == 0xFF && h[i + 1] == j)
                    {
                        size.Y = Bytes2Int(h[i + 5], h[i + 6], false);    //height 
                        size.X = Bytes2Int(h[i + 7], h[i + 8], false);    //width
                        return size;
                    }
                }
            }
            return size;
        }
        #endregion

        #region  获得BMP格式文件尺寸
        /// <summary>
        /// 获得BMP格式文件尺寸
        /// </summary>
        /// <param name="h">文件头数组</param>
        /// <returns>文件尺寸，Point.X存储图像宽度，Point.Y存储图像高度</returns>
        public static Point GetBmpSize(byte[] h)
        {
            Point size = new Point(0, 0); if (h.Length > 25)
                if (h.Length > 25)
                {
                    size.X = Bytes2Int(h[18], h[19], h[20], h[21], true); size.Y = Bytes2Int(h[22], h[23], h[24], h[25], true);
                }
            return size;
        }
        #endregion

        #region  获得PNG格式文件尺寸
        /// <summary>
        /// 获得PNG格式文件尺寸
        /// </summary>
        /// <param name="h">文件头数组</param>
        /// <returns>文件尺寸，Point.X存储图像宽度，Point.Y存储图像高度</returns>
        public static Point GetPngSize(byte[] h)
        {
            Point size = new Point(0, 0);
            for (int i = 0; i < h.Length - 11; i++)
            {
                if (h[i] == 0x49 && h[i + 1] == 0x48 && h[i + 2] == 0x44 && h[i + 3] == 0x52)
                {
                    size.X = Bytes2Int(h[i + 4], h[i + 5], h[i + 6], h[i + 7], false);    //width
                    size.Y = Bytes2Int(h[i + 8], h[i + 9], h[i + 10], h[i + 11], false);    //height
                    break;
                }
            }
            return size;
        }
        #endregion

        #region  获得GIF格式文件尺寸
        /// <summary>
        /// 获得GIF格式文件尺寸
        /// </summary>
        /// <param name="h">文件头数组</param>
        /// <returns>文件尺寸，Point.X存储图像宽度，Point.Y存储图像高度</returns>
        public static Point GetGifSize(byte[] h)
        {
            Point size = new Point(0, 0);
            if (h.Length > 9)
            {
                size.X = Bytes2Int(h[6], h[7], true);   //width 
                size.Y = Bytes2Int(h[8], h[9], true);   //height
            }
            return size;
        }
        #endregion

        #region  根据图片路径返回图片的字节流byte[]
        /// <summary> 
        /// 根据图片路径返回图片的字节流byte[] 
        /// </summary> 
        /// <param name="imagePath">图片路径</param> 
        /// <returns>返回的字节流</returns> 
        public static byte[] GetImageByte(string imagePath)
        {
            FileStream files = new FileStream(imagePath, FileMode.Open);
            byte[] imgByte = new byte[files.Length];
            files.Read(imgByte, 0, imgByte.Length);
            files.Close();
            return imgByte;
        }
        #endregion

        #region  图片转换成字节流 
        /// <summary> 
        /// 图片转换成字节流 
        /// </summary> 
        /// <param name="img">要转换的Image对象</param> 
        /// <returns>转换后返回的字节流</returns> 
        public static byte[] ImgToByte(Image img, ImageFormat type)
        {
            MemoryStream ms = new MemoryStream();
            img.Save(ms, type);
            byte[] imagedata = ms.GetBuffer();
            return imagedata;
        }
        #endregion

        #region  字节流转换成图片
        /// <summary> 
        /// 字节流转换成图片 
        /// </summary> 
        /// <param name="byt">要转换的字节流</param> 
        /// <returns>转换得到的Image对象</returns> 
        public static Image ByteToImg(byte[] byt)
        {
            MemoryStream ms = new MemoryStream(byt);
            Image img = Image.FromStream(ms);
            return img;
        }
        #endregion

        #region  根据坐标点获取屏幕图像
        /// <summary>
        /// 根据坐标点获取屏幕图像
        /// </summary>
        /// <param name="x1">左上角横坐标</param>
        /// <param name="y1">左上角纵坐标</param>
        /// <param name="x2">右下角横坐标</param>
        /// <param name="y2">右下角纵坐标</param>
        /// <returns>返回屏幕像</returns>
        public static Image GetScreenImg(int x1, int y1, int x2, int y2)
        {
            int w = (x2 - x1);
            int h = (y2 - y1);
            Image myImage = new Bitmap(w, h);
            Graphics g = Graphics.FromImage(myImage);
            g.CopyFromScreen(new Point(x1, y1), new Point(0, 0), new Size(w, h));
            IntPtr dc1 = g.GetHdc();
            g.ReleaseHdc(dc1);
            return myImage;
        }
        #endregion

        #region 获取图片一部分

        /// <summary> 
        /// 获取均分图片中的某一个 
        /// </summary> 
        /// <param name="orignal"></param>
        /// <param name="count"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Image GetImageByAverageIndex(Image orignal, int count, int index)
        {
            int width = orignal.Width / count;
            return CutImage(orignal, width * (index - 1), width, orignal.Height);
        }
        /// <summary> 
        /// 获取图片一部分 
        /// </summary> 
        /// <param name="orignal"></param>
        /// <param name="start"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private static Image CutImage(Image orignal, int start, int width, int height)
        {
            Bitmap partImage = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(partImage);//获取画板 
            Rectangle srcRect = new Rectangle(start, 0, width, height);//源位置开始 
            Rectangle destRect = new Rectangle(0, 0, width, height);//目标位置 
            //复制图片 
            g.DrawImage(orignal, destRect, srcRect, GraphicsUnit.Pixel);
            partImage.MakeTransparent(Color.FromArgb(255, 0, 255));
            g.Dispose();
            return partImage;
        }
        #endregion

        #region 相似找图

        private static unsafe bool ScanColor(byte b1, byte g1, byte r1, byte b2, byte g2, byte r2, int similar)
        {

            if ((Math.Abs(b1 - b2)) > similar)
            {
                return false;    //B
            }
            if ((Math.Abs(g1 - g2)) > similar)
            {
                return false;    //G
            }
            if ((Math.Abs(r1 - r2)) > similar)
            {
                return false;    //R
            }
            return true;
        }

        /// <summary>
        /// 相似找图 - 返回第一张图片的左上角X和Y
        /// </summary>
        /// <param name="left">搜寻范围左上角X</param>
        /// <param name="top">搜寻范围左上角Y</param>
        /// <param name="width">搜寻范围右下角X</param>
        /// <param name="height">搜寻范围右下角Y</param>
        /// <param name="P_DataPath">需要查找图片的路径</param>
        /// <param name="similar">误差值(相似度) 0 - 100</param>
        /// <param name="IntX">返回 第一张图片的 X 坐标</param>
        /// <param name="IntY">返回 第一张图片的 Y 坐标</param>
        /// <returns>找到返回true</returns>
        public unsafe static bool FindPicEx(int left, int top, int width, int height, string P_DataPath, int similar, out int IntX, out int IntY)
        {
            IntX = -1;
            IntY = -1;
            Bitmap P_bmp = new Bitmap(P_DataPath);
            Bitmap S_bmp = CopyScreen(left, top, width, height);
            int S_Width = S_bmp.Width;
            int S_Height = S_bmp.Height;
            int P_Width = P_bmp.Width;
            int P_Height = P_bmp.Height;
            BitmapData S_Data = S_bmp.LockBits(new Rectangle(0, 0, S_Width, S_Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData P_Data = P_bmp.LockBits(new Rectangle(0, 0, P_Width, P_Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            List<Point> List = new List<Point>();
            int S_stride = S_Data.Stride;
            int P_stride = P_Data.Stride;
            IntPtr S_Iptr = S_Data.Scan0;
            IntPtr P_Iptr = P_Data.Scan0;
            byte* S_ptr;
            byte* P_ptr;
            bool IsOk = false;
            int _BreakW = width - P_Data.Width + 1;
            int _BreakH = height - P_Data.Height + 1;
            for (int h = top; h < _BreakH; h++)
            {
                for (int w = left; w < _BreakW; w++)
                {
                    P_ptr = (byte*)(P_Iptr);
                    for (int y = 0; y < P_Data.Height; y++)
                    {
                        for (int x = 0; x < P_Data.Width; x++)
                        {
                            S_ptr = (byte*)((int)S_Iptr + S_stride * (h + y) + (w + x) * 3);
                            P_ptr = (byte*)((int)P_Iptr + P_stride * y + x * 3);
                            if (ScanColor(S_ptr[0], S_ptr[1], S_ptr[2], P_ptr[0], P_ptr[1], P_ptr[2], similar))  //比较颜色
                            {
                                IsOk = true;
                            }
                            else
                            {
                                IsOk = false;
                                break;
                            }
                        }
                        if (IsOk == false)
                        {
                            break;
                        }
                    }
                    if (IsOk)
                    {
                        IntX = w;
                        IntY = h;
                        return true;
                    }
                    IsOk = false;
                }
            }
            return false;
        }

        /// <summary>
        /// 相似找图 - 返回所有图片坐标
        /// </summary>
        /// <param name="left">搜寻范围左上角X</param>
        /// <param name="top">搜寻范围左上角Y</param>
        /// <param name="width">搜寻范围右下角X</param>
        /// <param name="height">搜寻范围右下角Y</param>
        /// <param name="P_DataPath">需要查找图片的路径</param>
        /// <param name="similar">误差值(相似度)0 - 100</param>
        /// <returns>找到 返回所有坐标集合</returns>
        public unsafe static List<Point> FindPicExAll(int left, int top, int width, int height, string P_DataPath, int similar)
        {
            Bitmap P_bmp = new Bitmap(P_DataPath);
            Bitmap S_bmp = CopyScreen(left, top, width, height);
            int S_Width = S_bmp.Width;
            int S_Height = S_bmp.Height;
            int P_Width = P_bmp.Width;
            int P_Height = P_bmp.Height;
            BitmapData S_Data = S_bmp.LockBits(new Rectangle(0, 0, S_Width, S_Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData P_Data = P_bmp.LockBits(new Rectangle(0, 0, P_Width, P_Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            List<Point> List = new List<Point>();
            int S_stride = S_Data.Stride;
            int P_stride = P_Data.Stride;
            IntPtr S_Iptr = S_Data.Scan0;
            IntPtr P_Iptr = P_Data.Scan0;
            byte* S_ptr;
            byte* P_ptr;
            bool IsOk = false;
            int _BreakW = width - P_Data.Width + 1;
            int _BreakH = height - P_Data.Height + 1;
            for (int h = top; h < _BreakH; h++)
            {
                for (int w = left; w < _BreakW; w++)
                {
                    P_ptr = (byte*)(P_Iptr);
                    for (int y = 0; y < P_Data.Height; y++)
                    {
                        for (int x = 0; x < P_Data.Width; x++)
                        {
                            S_ptr = (byte*)((int)S_Iptr + S_stride * (h + y) + (w + x) * 3);
                            P_ptr = (byte*)((int)P_Iptr + P_stride * y + x * 3);
                            if (ScanColor(S_ptr[0], S_ptr[1], S_ptr[2], P_ptr[0], P_ptr[1], P_ptr[2], similar))  //比较颜色
                            {
                                IsOk = true;
                            }
                            else
                            {
                                IsOk = false;
                                break;
                            }
                        }
                        if (IsOk == false)
                        {
                            break;
                        }
                    }
                    if (IsOk)
                    {
                        List.Add(new Point(w, h));
                    }
                    IsOk = false;
                }
            }
            return List;
        }
        #endregion

        #region 无容错找图
        /// <summary>
        /// 查找指定图片，并返回找到的第一个图片的坐标（无容错，相似度必须为100%）
        /// </summary>
        /// <param name="P_bmp">待搜索的图片</param>
        /// <param name="Rtx">搜寻范围左上角X</param>
        /// <param name="Rty">搜寻范围左上角Y</param>
        /// <param name="Rtz">搜寻范围右下角X</param>
        /// <param name="Rtw">搜寻范围右下角Y</param>
        /// <param name="Rtz">返回 第一张图片的 X 坐标</param>
        /// <param name="Rtw">返回 第一张图片的 Y 坐标</param>
        /// <returns>找图成功返回 true</returns>
        public unsafe static bool FindPic(string SP_bmp, int Rtx, int Rty, int Rtz, int Rtw, out int OtX, out int OtY)
        {
            OtX = -1;
            OtY = -1;
            Bitmap P_bmp = new Bitmap(SP_bmp);
            //
            Rectangle rect = new Rectangle(Rtx, Rty, Rtz, Rtw);
            //S_bmp为待对比的图片，这里是先将屏幕指定位置截图，再进行判断
            Bitmap S_bmp = CopyScreen(Rtx, Rty, Rtz, Rtw);
            //
            if (S_bmp.PixelFormat != PixelFormat.Format24bppRgb)
            {
                throw new Exception("颜色格式只支持24位bmp");
            }
            if (P_bmp.PixelFormat != PixelFormat.Format24bppRgb)
            {
                throw new Exception("颜色格式只支持24位bmp");
            }
            int S_Width = S_bmp.Width;
            int S_Height = S_bmp.Height;
            int P_Width = P_bmp.Width;
            int P_Height = P_bmp.Height;
            if (rect == Rectangle.Empty)
            {
                rect = new Rectangle(0, 0, S_Width, S_Height);
            }
            BitmapData S_Data = S_bmp.LockBits(new Rectangle(0, 0, S_Width, S_Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData P_Data = P_bmp.LockBits(new Rectangle(0, 0, P_Width, P_Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            List<Point> List = new List<Point>();
            int S_stride = S_Data.Stride;
            int P_stride = P_Data.Stride;
            int P_offset = P_stride - P_Data.Width * 3;
            IntPtr S_Iptr = S_Data.Scan0;
            IntPtr P_Iptr = P_Data.Scan0;
            byte* S_ptr;
            byte* P_ptr;
            bool IsOk = false;
            int _BreakW = S_Width - P_Width + 1;
            int _BreakH = S_Height - P_Height + 1;
            for (int h = rect.Y; h < _BreakH; h++)
            {
                for (int w = rect.X; w < _BreakW; w++)
                {
                    P_ptr = (byte*)(P_Iptr);
                    for (int y = 0; y < P_Height; y++)
                    {
                        for (int x = 0; x < P_Width; x++)
                        {
                            S_ptr = (byte*)(S_Iptr + S_stride * (h + y) + (w + x) * 3);
                            if (S_ptr[0] == P_ptr[0] && S_ptr[1] == P_ptr[1] && S_ptr[2] == P_ptr[2])
                            {
                                IsOk = true;
                            }
                            else
                            {
                                IsOk = false;
                                break;
                            }
                            P_ptr += 3;
                        }
                        if (IsOk == false)
                        {
                            break;
                        }
                        P_ptr += P_offset;
                    }
                    if (IsOk)
                    {
                        OtX = w;
                        OtY = h;
                        //
                        S_bmp.UnlockBits(S_Data);
                        P_bmp.UnlockBits(P_Data);
                        //释放资源
                        S_bmp.Dispose();
                        P_bmp.Dispose();
                        return true;
                    }
                    IsOk = false;
                }
            }
            //
            S_bmp.UnlockBits(S_Data);
            P_bmp.UnlockBits(P_Data);
            //释放资源
            S_bmp.Dispose();
            P_bmp.Dispose();
            return false;
        }

        /// <summary>
        /// 查找指定图片，并返回所有坐标（无容错，相似度必须为100%）
        /// </summary>
        /// <param name="P_bmp">待搜索的图片</param>
        /// <param name="Rtx">搜寻范围左上角X</param>
        /// <param name="Rty">搜寻范围左上角Y</param>
        /// <param name="Rtz">搜寻范围右下角X</param>
        /// <param name="Rtw">搜寻范围右下角Y</param>
        /// <returns></returns>
        public unsafe static List<Point> FindPic_All(string SP_bmp, int Rtx, int Rty, int Rtz, int Rtw)
        {
            Bitmap P_bmp = new Bitmap(SP_bmp);
            //
            Rectangle rect = new Rectangle(Rtx, Rty, Rtz, Rtw);
            //S_bmp为待对比的图片，这里是先将屏幕指定位置截图，再进行判断
            Bitmap S_bmp = CopyScreen(Rtx, Rty, Rtz, Rtw);
            //
            if (S_bmp.PixelFormat != PixelFormat.Format24bppRgb)
            {
                throw new Exception("颜色格式只支持24位bmp");
            }
            if (P_bmp.PixelFormat != PixelFormat.Format24bppRgb)
            {
                throw new Exception("颜色格式只支持24位bmp");
            }
            int S_Width = S_bmp.Width;
            int S_Height = S_bmp.Height;
            int P_Width = P_bmp.Width;
            int P_Height = P_bmp.Height;
            if (rect == Rectangle.Empty)
            {
                rect = new Rectangle(0, 0, S_Width, S_Height);
            }
            BitmapData S_Data = S_bmp.LockBits(new Rectangle(0, 0, S_Width, S_Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData P_Data = P_bmp.LockBits(new Rectangle(0, 0, P_Width, P_Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            List<Point> List = new List<Point>();
            int S_stride = S_Data.Stride;
            int P_stride = P_Data.Stride;
            int P_offset = P_stride - P_Data.Width * 3;
            IntPtr S_Iptr = S_Data.Scan0;
            IntPtr P_Iptr = P_Data.Scan0;
            byte* S_ptr;
            byte* P_ptr;
            bool IsOk = false;
            int _BreakW = S_Width - P_Width + 1;
            int _BreakH = S_Height - P_Height + 1;
            for (int h = rect.Y; h < _BreakH; h++)
            {
                for (int w = rect.X; w < _BreakW; w++)
                {
                    P_ptr = (byte*)(P_Iptr);
                    for (int y = 0; y < P_Height; y++)
                    {
                        for (int x = 0; x < P_Width; x++)
                        {
                            S_ptr = (byte*)(S_Iptr + S_stride * (h + y) + (w + x) * 3);
                            if (S_ptr[0] == P_ptr[0] && S_ptr[1] == P_ptr[1] && S_ptr[2] == P_ptr[2])
                            {
                                IsOk = true;
                            }
                            else
                            {
                                IsOk = false;
                                break;
                            }
                            P_ptr += 3;
                        }
                        if (IsOk == false)
                        {
                            break;
                        }
                        P_ptr += P_offset;
                    }
                    if (IsOk)
                    {
                        List.Add(new Point(w, h));
                    }
                    IsOk = false;
                }
            }
            S_bmp.UnlockBits(S_Data);
            P_bmp.UnlockBits(P_Data);
            //释放资源
            S_bmp.Dispose();
            P_bmp.Dispose();
            return List;
        }
        #endregion

        #region 屏幕截图
        /// <summary>
        /// 屏幕截图
        /// </summary>
        /// <param name="x">x 坐标</param>
        /// <param name="y">y 坐标</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <returns>返回 Bitmap 对象</returns>
        public static Bitmap CopyScreen(int x, int y, int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(x, y, 0, 0, new Size(width, height));
            }
            return bitmap;
        }
        /// <summary>
        /// 全屏截图
        /// </summary>
        /// <returns></returns>
        public static Image CaptureScreen()
        {
            return CaptureWindow(User32API.GetDesktopWindow());
        }

        /// <summary>
        /// 指定窗口截图
        /// </summary>
        /// <param name="handle">窗口句柄. (在windows应用程序中, 从Handle属性获得)</param>
        /// <returns></returns>
        public static Image CaptureWindow(IntPtr handle)
        {
            IntPtr hdcSrc = User32API.GetWindowDC(handle);
            APIStruct.RECT windowRect = new APIStruct.RECT();
            User32API.GetWindowRect(handle, ref windowRect);
            int width = windowRect.right - windowRect.left;
            int height = windowRect.bottom - windowRect.top;
            IntPtr hdcDest = GDI32API.CreateCompatibleDC(hdcSrc);
            IntPtr hBitmap = GDI32API.CreateCompatibleBitmap(hdcSrc, width, height);
            IntPtr hOld = GDI32API.SelectObject(hdcDest, hBitmap);
            GDI32API.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, GDI32API.SRCCOPY);
            GDI32API.SelectObject(hdcDest, hOld);
            GDI32API.DeleteDC(hdcDest);
            User32API.ReleaseDC(handle, hdcSrc);
            Image img = Image.FromHbitmap(hBitmap);
            GDI32API.DeleteObject(hBitmap);
            return img;
        }

        /// <summary>
        /// 指定窗口截图 保存为图片文件
        /// </summary>
        /// <param name="handle">指定窗口句柄</param>
        /// <param name="filename">保存路径</param>
        /// <param name="format">图片格式</param>
        public static void CaptureWindowToFile(IntPtr handle, string filename, ImageFormat format)
        {
            Image img = CaptureWindow(handle);
            img.Save(filename, format);
        }

        /// <summary>
        /// 全屏截图 保存为文件
        /// </summary>
        /// <param name="filename">保存路径</param>
        /// <param name="format">图片格式</param>
        public static void CaptureScreenToFile(string filename, ImageFormat format)
        {
            Image img = CaptureScreen();
            img.Save(filename, format);
        }
        #endregion

        #region 把bmp格式图片转换成24位位图格式的bmp
        /// <summary>
        /// 把bmp格式图片转换成24位位图格式的bmp
        /// </summary>
        /// <param name="bmOld">待转换的图片</param>
        /// <returns>返回 转换后的图片</returns>
        public static Bitmap To24pic(Bitmap bmOld)
        {
            int iwidth = bmOld.Width;
            int iHeight = bmOld.Height;
            Bitmap bmNew = new Bitmap(iwidth, iHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(bmNew);
            g.DrawImage(bmOld, new Point(0, 0));
            g.Dispose();
            GC.Collect();
            return bmNew;
        }
        #endregion

        #region 获取 bing 图片

        private static string GetHtml(string URL, Encoding en) // 获取HTML 源代码      设定编码
        {
            Uri uri = new Uri(URL);
           
            WebRequest request = WebRequest.Create(uri);
            string html;
            try
            {
                // err = false;
                WebResponse response = request.GetResponse();
                Stream W_stream = response.GetResponseStream();
                StreamReader read = new StreamReader(W_stream, en);
                html = read.ReadToEnd();
                read.Close();
                W_stream.Close();
                response.Close();
            }
            catch (Exception)
            {
                return "ERR";
            }
            return html;
        }
        /// <summary>
        /// 获取 bing 图片
        /// </summary>
        /// <returns>返回图片对象</returns>
        public static Image GetBingImg()
        {

            string html = GetHtml("http://cn.bing.com", Encoding.UTF8);
            if (html != "ERR")
            {
                html = html.Substring(html.IndexOf("http://s.cn.bing.net/az/hprichbg"));
                html = html.Substring(0, html.IndexOf(".jpg") + 4);

                PictureBox p1 = new PictureBox();
                p1.Load(html);
                return p1.Image;
            }
            return null;
        }
        #endregion

        #region 设置桌面背景
        /// <summary>
        /// 设置桌面背景
        /// </summary>
        /// <param name="path">图片地址</param>
        /// <returns>true：成功 false：失败</returns>
        public static bool SetBankground(string path) 
        {
            if (File.Exists(path))
            {
                try
                {
                    Image image = Image.FromFile(path);
                    image.Save(Application.StartupPath + "\\Bing\\TEMP.BMP", System.Drawing.Imaging.ImageFormat.Bmp);
                    SystemParametersInfo(20, 0, Application.StartupPath + "\\Bing\\TEMP.BMP", 0x2);
                    return true;
                }
                catch (Exception ex)
                { 
                }
            }
            return false;
        }
        #endregion

        [DllImport("gdi32")]
        static extern int DeleteObject(IntPtr o);
        /// <summary>
        /// Bitmap->BitmapSource
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static System.Windows.Media.Imaging.BitmapSource GetBitMapSourceFromBitmap(Bitmap bitmap)
        {
            IntPtr intPtrl = bitmap.GetHbitmap();
            System.Windows.Media.Imaging.BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(intPtrl,
                IntPtr.Zero,
                System.Windows.Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            DeleteObject(intPtrl);
            return bitmapSource;
        }


        /// <summary>
        ///  Bitmap --> BitmapImage
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static System.Windows.Media.Imaging.BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Bmp);
                stream.Position = 0;
                System.Windows.Media.Imaging.BitmapImage result = new System.Windows.Media.Imaging.BitmapImage();
                result.BeginInit();
                result.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();
                return result;
            }
        }
        /// <summary>
        /// ImageSource转Bitmap
        /// </summary>
        /// <param name="imageSource"></param>
        /// <returns></returns>
        public static Bitmap ImageSourceToBitmap(System.Windows.Media.ImageSource imageSource)
        {
            System.Windows.Media.Imaging.BitmapSource m = (System.Windows.Media.Imaging.BitmapSource)imageSource;

            Bitmap bmp = new Bitmap(m.PixelWidth, m.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            BitmapData data = bmp.LockBits(
            new Rectangle(System.Drawing.Point.Empty, bmp.Size), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            m.CopyPixels(System.Windows.Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride); bmp.UnlockBits(data);

            return bmp;
        }
        /// <summary>
        /// 把本地图片读到内存流中，防止出现"一般性错误"
        /// </summary>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        public static System.Windows.Media.Imaging.BitmapImage GetImage(string imagePath)
        {
            try
            {
                System.Windows.Media.Imaging.BitmapImage bitmap = new System.Windows.Media.Imaging.BitmapImage();
                if (File.Exists(imagePath))
                {
                    bitmap.BeginInit();
                    bitmap.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                    using (Stream ms = new MemoryStream(File.ReadAllBytes(imagePath)))
                    {
                        bitmap.StreamSource = ms;
                        bitmap.EndInit();
                        bitmap.Freeze();
                    }
                }
                return bitmap;
            }
            catch { return null; }
        }
        /// <summary>
        /// 截图
        /// </summary>
        /// <param name="bitmapSource"></param>
        /// <param name="cut"></param>
        /// <returns></returns>
        public static System.Windows.Media.Imaging.BitmapSource CutImage(System.Windows.Media.Imaging.BitmapSource bitmapSource, System.Windows.Int32Rect cut)
        {
            //计算Stride
            var stride = bitmapSource.Format.BitsPerPixel * cut.Width / 8;
            //声明字节数组
            byte[] data = new byte[cut.Height * stride];
            //调用CopyPixels
            bitmapSource.CopyPixels(cut, data, stride, 0);

            return System.Windows.Media.Imaging.BitmapSource.Create(cut.Width, cut.Height, 0, 0, System.Windows.Media.PixelFormats.Bgr32, null, data, stride);
        }
    }
}