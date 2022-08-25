using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using static SuperFramework.SuperImage.ImageClasses;
using static SuperFramework.SuperImage.ImageEnum;

//调用
//UploadImage ui = new UploadImage();
///***可选参数***/
//ui.SetWordWater = "哈哈";//文字水印
//// ui.SetPicWater = Server.MapPath("2.png");//图片水印(图片和文字都赋值图片有效)
//ui.SetPositionWater = 4;//水印图片的位置 0居中、1左上角、2右上角、3左下角、4右下角
//ui.SetSmallImgHeight = "110,40,20";//设置缩略图可以多个
//ui.SetSmallImgWidth = "100,40,20";
////保存图片生成缩略图
//var reponseMessage = ui.FileSaveAs(Request.Files0], Server.MapPath("~/file/temp"));
////裁剪图片
//var reponseMessage2 = ui.FileCutSaveAs(Request.Files0], Server.MapPath("~/file/temp2"), 400, 300, UploadImage.CutMode.CutNo);
///***返回信息***/
//var isError = reponseMessage.IsError;//是否异常
//var webPath = reponseMessage.WebPath;//web路径
//var filePath = reponseMessage.filePath;//物理路径
//var message = reponseMessage.Message;//错误信息
//var directory = reponseMessage.Directory;//目录
//var smallPath1 = reponseMessage.SmallPath(0);//缩略图路径1
//var smallPath2 = reponseMessage.SmallPath(1);//缩略图路径2
//var smallPath3 = reponseMessage.SmallPath(2);//缩略图路径3
/// ** 使用说明：http://www.cnblogs.com/sunkaixuan/p/4536626.html

namespace SuperFramework.SuperImage
{
    /// <summary>
    /// 版 本:Release
    /// 日 期:2015-5-28
    /// 作 者:不良帥
    /// 描 述:图片上传类、支持水印、缩略图
    /// </summary>
    public class ImageUpload
    {
        #region 属性
        /// <summary>
        /// 允许图片格式
        /// </summary>
        public string SetAllowFormat { get; set; }
        /// <summary>
        /// 允许上传图片大小
        /// </summary>
        public double SetAllowSize { get; set; }
        /// <summary>
        /// 文字水印字符
        /// </summary>
        public string SetWordWater { get; set; }
        /// <summary>
        /// 图片水印
        /// </summary>
        public string SetPicWater { get; set; }
        /// <summary>
        /// 水印图片的位置 0居中、1左上角、2右上角、3左下角、4右下角
        /// </summary>
        public int SetPositionWater { get; set; }
        /// <summary>
        /// 缩略图宽度多个逗号格开（例如:200,100）
        /// </summary>
        public string SetSmallImgWidth { get; set; }
        /// <summary>
        /// 缩略图高度多个逗号格开（例如:200,100）
        /// </summary>
        public string SetSmallImgHeight { get; set; }
        /// <summary>
        /// 是否限制最大宽度，默认为true
        /// </summary>
        public bool SetLimitWidth { get; set; }
        /// <summary>
        /// 最大宽度尺寸，默认为600
        /// </summary>
        public int SetMaxWidth { get; set; }
        /// <summary>
        /// 是否剪裁图片，默认true
        /// </summary>
        public bool SetCutImage { get; set; }
        /// <summary>
        /// 限制图片最小宽度，0表示不限制
        /// </summary>
        public int SetMinWidth { get; set; }
        #endregion
        /// <summary>
        /// 构造函数
        /// </summary>
        public ImageUpload()
        {
            SetAllowFormat = ".jpeg|.jpg|.bmp|.gif|.png";   //允许图片格式
            SetAllowSize = 1;       //允许上传图片大小,默认为1MB
            SetPositionWater = 4;
            SetCutImage = true;
        }
        #region main method
        
        #region 验证格式
        /// <summary>
        /// 验证格式
        /// </summary>
        /// <param name="allType">所有格式</param>
        /// <param name="chkType">被检查的格式</param>
        /// <returns>bool</returns>
        public bool CheckValidExt(string allType, string chkType)
        {
            bool flag = false;
            string[] sArray = allType.Split('|');
            foreach (string temp in sArray)
            {
                if (temp.ToLower() == chkType.ToLower())
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }
        #endregion
        #region 根据需要的图片尺寸，按比例剪裁原始图片
        /// <summary>
        /// 根据需要的图片尺寸，按比例剪裁原始图片
        /// </summary>
        /// <param name="nWidth">缩略图宽度</param>
        /// <param name="nHeight">缩略图高度</param>
        /// <param name="img">原始图片</param>
        /// <returns>剪裁区域尺寸</returns>
        public Size CutRegion(int nWidth, int nHeight, Image img)
        {
            double nw = (double)nWidth;
            double nh = (double)nHeight;
            double pw = (double)img.Width;
            double ph = (double)img.Height;
            double width;
            double height;
            if (nw / nh > pw / ph)
            {
                width = pw;
                height = pw * nh / nw;
            }
            else if (nw / nh < pw / ph)
            {
                width = ph * nw / nh;
                height = ph;
            }
            else
            {
                width = pw;
                height = ph;
            }
            return new Size(Convert.ToInt32(width), Convert.ToInt32(height));
        }
        #endregion
        #region 等比例缩小图片
        /// <summary>
        /// 等比例缩小图片
        /// </summary>
        /// <param name="nWidth">缩略图宽度</param>
        /// <param name="nHeight">缩略图高度</param>
        /// <param name="img">原始图片</param>
        /// <returns>剪裁区域尺寸</returns>
        public Size NewSize(int nWidth, int nHeight, Image img)
        {
            double sw = Convert.ToDouble(img.Width);
            double sh = Convert.ToDouble(img.Height);
            double mw = Convert.ToDouble(nWidth);
            double mh = Convert.ToDouble(nHeight);
            double w;
            double h;
            if (sw < mw && sh < mh)
            {
                w = sw;
                h = sh;
            }
            else if ((sw / sh) > (mw / mh))
            {
                w = nWidth;
                h = (w * sh) / sw;
            }
            else
            {
                h = nHeight;
                w = (h * sw) / sh;
            }
            return new Size(Convert.ToInt32(w), Convert.ToInt32(h));
        }
        #endregion
        #region 生成缩略图
        #region 生成缩略图，不加水印
        /// <summary>
        /// 生成缩略图，不加水印
        /// </summary>
        /// <param name="filename">源文件</param>
        /// <param name="nWidth">缩略图宽度</param>
        /// <param name="nHeight">缩略图高度</param>
        /// <param name="destfile">缩略图保存位置</param>
        public void CreateSmallPhoto(string filename, int nWidth, int nHeight, string destfile)
        {
            Image img = Image.FromFile(filename);
            ImageFormat thisFormat = img.RawFormat;
            Size CutSize = CutRegion(nWidth, nHeight, img);
            Bitmap outBmp = new Bitmap(nWidth, nHeight);
            Graphics g = Graphics.FromImage(outBmp);
            // 设置画布的描绘质量
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            int nStartX = (img.Width - CutSize.Width) / 2;
            int nStartY = (img.Height - CutSize.Height) / 2;
            g.DrawImage(img, new Rectangle(0, 0, nWidth, nHeight),
                nStartX, nStartY, CutSize.Width, CutSize.Height, GraphicsUnit.Pixel);
            g.Dispose();
            //if (thisFormat.Equals(ImageFormat.Gif))
            //{
            //    Response.ContentType = "image/gif";
            //}
            //else
            //{
            //    Response.ContentType = "image/jpeg";
            //}
            // 以下代码为保存图片时，设置压缩质量
            EncoderParameters encoderParams = new EncoderParameters();
            long[] quality = new long[1];
            quality[0] = 100;
            EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            encoderParams.Param[0] = encoderParam;
            //获得包含有关内置图像编码解码器的信息的ImageCodecInfo 对象。
            ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo jpegICI = null;
            for (int x = 0; x < arrayICI.Length; x++)
            {
                if (arrayICI[x].FormatDescription.Equals("JPEG"))
                {
                    jpegICI = arrayICI[x];//设置JPEG编码
                    break;
                }
            }
            if (jpegICI != null)
            {
                //outBmp.Save(Response.OutputStream, jpegICI, encoderParams);
                outBmp.Save(destfile, jpegICI, encoderParams);
            }
            else
            {
                //outBmp.Save(Response.OutputStream, thisFormat);
                outBmp.Save(destfile, thisFormat);
            }
            img.Dispose();
            outBmp.Dispose();
        }
        #endregion
        #region 生成缩略图，加水印
        /// <summary>
        /// 生成缩略图，加水印
        /// </summary>
        /// <param name="filename">源文件</param>
        /// <param name="nWidth">缩略图宽度</param>
        /// <param name="nHeight">缩略图高度</param>
        /// <param name="destfile">缩略图保存位置</param>
        /// <param name="sy">水印文字或者水印图片路径</param>
        /// <param name="nType">水印类型，0-图片水印，1-文字水印</param>
        public void CreateSmallPhoto(string filename, int nWidth, int nHeight, string destfile, string sy, int nType)
        {
            if (nType == 0)
                CreateSmallPhoto(filename, nWidth, nHeight, destfile, sy, "");
            else
                CreateSmallPhoto(filename, nWidth, nHeight, destfile, "", sy);
        }
        #endregion
        #region 生成缩略图
        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="filename">源文件</param>
        /// <param name="nWidth">缩略图宽度</param>
        /// <param name="nHeight">缩略图高度</param>
        /// <param name="destfile">缩略图保存位置</param>
        /// <param name="png">图片水印</param>
        /// <param name="text">文本水印</param>
        public void CreateSmallPhoto(string filename, int nWidth, int nHeight, string destfile, string png, string text)
        {
            Image img = Image.FromFile(filename);
            ImageFormat thisFormat = img.RawFormat;
            Size CutSize = CutRegion(nWidth, nHeight, img);
            Bitmap outBmp = new Bitmap(nWidth, nHeight);
            Graphics g = Graphics.FromImage(outBmp);
            g.Clear(Color.White);
            // 设置画布的描绘质量
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            int nStartX = (img.Width - CutSize.Width) / 2;
            int nStartY = (img.Height - CutSize.Height) / 2;
            g.DrawImage(img, new Rectangle(0, 0, nWidth, nHeight),
                nStartX, nStartY, CutSize.Width, CutSize.Height, GraphicsUnit.Pixel);
            g.Dispose();
            // 以下代码为保存图片时，设置压缩质量
            EncoderParameters encoderParams = new EncoderParameters();
            long[] quality = new long[1];
            quality[0] = 100;
            EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            encoderParams.Param[0] = encoderParam;
            //获得包含有关内置图像编码解码器的信息的ImageCodecInfo 对象。
            ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo jpegICI = null;
            for (int x = 0; x < arrayICI.Length; x++)
            {
                if (arrayICI[x].FormatDescription.Equals("JPEG"))
                {
                    jpegICI = arrayICI[x];//设置JPEG编码
                    break;
                }
            }
            if (jpegICI != null)
            {
                outBmp.Save(destfile, jpegICI, encoderParams);
            }
            else
            {
                outBmp.Save(destfile, thisFormat);
            }
            img.Dispose();
            outBmp.Dispose();
            if (!string.IsNullOrEmpty(png))
                AttachPng(png, destfile);
            if (!string.IsNullOrEmpty(text))
                AttachText(text, destfile);
        }
        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="filename">源文件</param>
        /// <param name="nWidth">缩略图宽度</param>
        /// <param name="nHeight">缩略图高度</param>
        /// <param name="destfile">缩略图保存位置</param>
        /// <param name="png">图片水印</param>
        /// <param name="text">文本水印</param>
        /// <param name="cMode">裁剪方式</param>
        public void CreateSmallPhoto(string filename, int nWidth, int nHeight, string destfile, string png, string text, CutMode cMode)
        {
            Image img = Image.FromFile(filename);
            if (nWidth <= 0)
                nWidth = img.Width;
            if (nHeight <= 0)
                nHeight = img.Height;
            int towidth = nWidth;
            int toheight = nHeight;
            switch (cMode)
            {
                case CutMode.CutWH://指定高宽缩放（可能变形）               
                    break;
                case CutMode.CutW://指定宽，高按比例                   
                    toheight = img.Height * nWidth / img.Width;
                    break;
                case CutMode.CutH://指定高，宽按比例
                    towidth = img.Width * nHeight / img.Height;
                    break;
                case CutMode.CutNo: //缩放不剪裁
                    int maxSize = (nWidth >= nHeight ? nWidth : nHeight);
                    if (img.Width >= img.Height)
                    {
                        towidth = maxSize;
                        toheight = img.Height * maxSize / img.Width;
                    }
                    else
                    {
                        toheight = maxSize;
                        towidth = img.Width * maxSize / img.Height;
                    }
                    break;
                default:
                    break;
            }
            nWidth = towidth;
            nHeight = toheight;
            ImageFormat thisFormat = img.RawFormat;
            Size CutSize = new Size(nWidth, nHeight);
            if (cMode != CutMode.CutNo)
                CutSize = CutRegion(nWidth, nHeight, img);
            Bitmap outBmp = new Bitmap(CutSize.Width, CutSize.Height);
            Graphics g = Graphics.FromImage(outBmp);
            g.Clear(Color.White);
            // 设置画布的描绘质量
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            int nStartX = (img.Width - CutSize.Width) / 2;
            int nStartY = (img.Height - CutSize.Height) / 2;
            //int x1 = (outBmp.Width - nWidth) / 2;
            //int y1 = (outBmp.Height - nHeight) / 2;
            if (cMode != CutMode.CutNo)
                g.DrawImage(img, new Rectangle(0, 0, nWidth, nHeight),
                    nStartX, nStartY, CutSize.Width, CutSize.Height, GraphicsUnit.Pixel);
            else
                g.DrawImage(img, new Rectangle(0, 0, nWidth, nHeight),
                0, 0, img.Width, img.Height, GraphicsUnit.Pixel);
            g.Dispose();
            // 以下代码为保存图片时，设置压缩质量
            EncoderParameters encoderParams = new EncoderParameters();
            long[] quality = new long[1];
            quality[0] = 100;
            EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            encoderParams.Param[0] = encoderParam;
            //获得包含有关内置图像编码解码器的信息的ImageCodecInfo 对象。
            ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo jpegICI = null;
            for (int x = 0; x < arrayICI.Length; x++)
            {
                if (arrayICI[x].FormatDescription.Equals("JPEG"))
                {
                    jpegICI = arrayICI[x];//设置JPEG编码
                    break;
                }
            }
            if (jpegICI != null)
            {
                outBmp.Save(destfile, jpegICI, encoderParams);
            }
            else
            {
                outBmp.Save(destfile, thisFormat);
            }
            img.Dispose();
            outBmp.Dispose();
            if (!string.IsNullOrEmpty(png))
                AttachPng(png, destfile);
            if (!string.IsNullOrEmpty(text))
                AttachText(text, destfile);
        }
        #endregion
        #endregion
        #region 添加文字水印
        /// <summary>
        /// 添加文字水印
        /// </summary>
        /// <param name="text">水印文字</param>
        /// <param name="file">原文件</param>

        public void AttachText(string text, string file)
        {
            if (string.IsNullOrEmpty(text))
                return;
            if (!System.IO.File.Exists(file))
                return;
            System.IO.FileInfo oFile = new System.IO.FileInfo(file);
            string strTempFile = System.IO.Path.Combine(oFile.DirectoryName, Guid.NewGuid().ToString() + oFile.Extension);
            oFile.CopyTo(strTempFile);
            Image img = Image.FromFile(strTempFile);
            ImageFormat thisFormat = img.RawFormat;
            int nHeight = img.Height;
            int nWidth = img.Width;
            Bitmap outBmp = new Bitmap(nWidth, nHeight);
            Graphics g = Graphics.FromImage(outBmp);
            g.Clear(Color.White);
            // 设置画布的描绘质量
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(img, new Rectangle(0, 0, nWidth, nHeight),
                0, 0, nWidth, nHeight, GraphicsUnit.Pixel);
            int[] sizes = new int[] { 16, 14, 12, 10, 8, 6, 4 };
            Font crFont = null;
            SizeF crSize = new SizeF();
            //通过循环这个数组，来选用不同的字体大小
            //如果它的大小小于图像的宽度，就选用这个大小的字体
            for (int i = 0; i < 7; i++)
            {
                //设置字体，这里是用arial，黑体
                crFont = new Font("arial", sizes[i], FontStyle.Bold);
                //Measure the Copyright string in this Font
                crSize = g.MeasureString(text, crFont);
                if ((ushort)crSize.Width < (ushort)nWidth)
                    break;
            }
            //因为图片的高度可能不尽相同, 所以定义了
            //从图片底部算起预留了5%的空间
            int yPixlesFromBottom = (int)(nHeight * .08);
            //现在使用版权信息字符串的高度来确定要绘制的图像的字符串的y坐标
            float yPosFromBottom = ((nHeight - yPixlesFromBottom) - (crSize.Height / 2));
            //计算x坐标
            float xCenterOfImg = (nWidth / 2);
            //把文本布局设置为居中
            StringFormat StrFormat = new StringFormat();
            StrFormat.Alignment = StringAlignment.Center;
            //通过Brush来设置黑色半透明
            SolidBrush semiTransBrush2 = new SolidBrush(Color.FromArgb(153, 0, 0, 0));
            //绘制版权字符串
            g.DrawString(text,                 //版权字符串文本
                crFont,                                   //字体
                semiTransBrush2,                           //Brush
                new PointF(xCenterOfImg + 1, yPosFromBottom + 1),  //位置
                StrFormat);
            //设置成白色半透明
            SolidBrush semiTransBrush = new SolidBrush(Color.FromArgb(153, 255, 255, 255));
            //第二次绘制版权字符串来创建阴影效果
            //记住移动文本的位置1像素
            g.DrawString(text,                 //版权文本
                crFont,                                   //字体
                semiTransBrush,                           //Brush
                new PointF(xCenterOfImg, yPosFromBottom),  //位置
                StrFormat);
            g.Dispose();
            // 以下代码为保存图片时，设置压缩质量
            EncoderParameters encoderParams = new EncoderParameters();
            long[] quality = new long[1];
            quality[0] = 100;
            EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            encoderParams.Param[0] = encoderParam;
            //获得包含有关内置图像编码解码器的信息的ImageCodecInfo 对象。
            ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo jpegICI = null;
            for (int x = 0; x < arrayICI.Length; x++)
            {
                if (arrayICI[x].FormatDescription.Equals("JPEG"))
                {
                    jpegICI = arrayICI[x];//设置JPEG编码
                    break;
                }
            }
            if (jpegICI != null)
            {
                outBmp.Save(file, jpegICI, encoderParams);
            }
            else
            {
                outBmp.Save(file, thisFormat);
            }
            img.Dispose();
            outBmp.Dispose();
            System.IO.File.Delete(strTempFile);
        }
        #endregion
        #region 添加图片水印
        ///<summary>
        /// 添加图片水印
        /// </summary>
        /// <param name="png">水印图片</param>
        /// <param name="file">原文件</param>

        public void AttachPng(string png, string file)
        {
            if (string.IsNullOrEmpty(png))
                return;
            if (!System.IO.File.Exists(png))
                return;
            if (!System.IO.File.Exists(file))
                return;
            System.IO.FileInfo oFile = new System.IO.FileInfo(file);
            string strTempFile = System.IO.Path.Combine(oFile.DirectoryName, Guid.NewGuid().ToString() + oFile.Extension);
            oFile.CopyTo(strTempFile);
            Image img = Image.FromFile(strTempFile);
            ImageFormat thisFormat = img.RawFormat;
            int nHeight = img.Height;
            int nWidth = img.Width;
            Bitmap outBmp = new Bitmap(nWidth, nHeight);
            Graphics g = Graphics.FromImage(outBmp);
            // 设置画布的描绘质量
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(img, new Rectangle(0, 0, nWidth, nHeight),
                0, 0, nWidth, nHeight, GraphicsUnit.Pixel);
            img.Dispose();
            img = Image.FromFile(png);
            //Bitmap bmpPng = new Bitmap(img);
            //ImageAttributes imageAttr = new ImageAttributes();
            //Color bg = Color.Green;
            //imageAttr.SetColorKey(bg, bg);
            Size pngSize = NewSize(nWidth, nHeight, img);
            int padding = 10;
            int nx;
            int ny;
            if (SetPositionWater == 0)
            {
                nx = (nWidth - pngSize.Width) / 2;
                ny = (nHeight - pngSize.Height) / 2;
            }
            else if (SetPositionWater == 1)
            {
                nx = padding;
                ny = padding;
            }
            else if (SetPositionWater == 2)
            {
                nx = (nWidth - pngSize.Width) - padding;
                ny = padding;
            }
            else if (SetPositionWater == 3)
            {
                nx = padding;
                ny = (nHeight - pngSize.Height) - padding;
            }
            else
            {
                nx = (nWidth - pngSize.Width) - padding;
                ny = (nHeight - pngSize.Height) - padding;
            }
            g.DrawImage(img, new Rectangle(nx, ny, pngSize.Width, pngSize.Height),
                0, 0, img.Width, img.Height, GraphicsUnit.Pixel);
            g.Dispose();
            // 以下代码为保存图片时，设置压缩质量
            EncoderParameters encoderParams = new EncoderParameters();
            long[] quality = new long[1];
            quality[0] = 100;
            EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            encoderParams.Param[0] = encoderParam;
            //获得包含有关内置图像编码解码器的信息的ImageCodecInfo 对象。
            ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo jpegICI = null;
            for (int x = 0; x < arrayICI.Length; x++)
            {
                if (arrayICI[x].FormatDescription.Equals("JPEG"))
                {
                    jpegICI = arrayICI[x];//设置JPEG编码
                    break;
                }
            }
            if (jpegICI != null)
            {
                outBmp.Save(file, jpegICI, encoderParams);
            }
            else
            {
                outBmp.Save(file, thisFormat);
            }
            img.Dispose();
            outBmp.Dispose();
            System.IO.File.Delete(strTempFile);
        }
        #endregion
        #region 得到指定mimeType的ImageCodecInfo
        /// <summary>
        /// 保存JPG时用
        /// </summary>
        /// <param name="mimeType"> </param>
        /// <returns>得到指定mimeType的ImageCodecInfo </returns>
        private ImageCodecInfo GetCodecInfo(string mimeType)
        {
            ImageCodecInfo[] CodecInfo = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo ici in CodecInfo)
            {
                if (ici.MimeType == mimeType) return ici;
            }
            return null;
        }
        #endregion
        #region 保存为JPEG格式，支持压缩质量选项
        /// <summary>
        /// 保存为JPEG格式，支持压缩质量选项
        /// </summary>
        /// <param name="SourceFile"></param>
        /// <param name="FileName"></param>
        /// <param name="Qty"></param>
        /// <returns></returns>
        public bool KiSaveAsJPEG(string SourceFile, string FileName, int Qty)
        {
            Bitmap bmp = new Bitmap(SourceFile);
            try
            {
                EncoderParameter p;
                EncoderParameters ps;
                ps = new EncoderParameters(1);
                p = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, Qty);
                ps.Param[0] = p;
                bmp.Save(FileName, GetCodecInfo("image/jpeg"), ps);
                bmp.Dispose();
                return true;
            }
            catch
            {
                bmp.Dispose();
                return false;
            }
        }
        #endregion
        #region 将图片压缩到指定大小
        /// <summary>
        /// 将图片压缩到指定大小
        /// </summary>
        /// <param name="FileName">待压缩图片</param>
        /// <param name="size">期望压缩后的尺寸</param>
        public void CompressPhoto(string FileName, int size)
        {
            if (!System.IO.File.Exists(FileName))
                return;
            int nCount = 0;
            System.IO.FileInfo oFile = new System.IO.FileInfo(FileName);
            long nLen = oFile.Length;
            while (nLen > size * 1024 && nCount < 10)
            {
                string dir = oFile.Directory.FullName;
                string TempFile = System.IO.Path.Combine(dir, Guid.NewGuid().ToString() + "." + oFile.Extension);
                oFile.CopyTo(TempFile, true);
                KiSaveAsJPEG(TempFile, FileName, 70);
                try
                {
                    System.IO.File.Delete(TempFile);
                }
                catch { }
                nCount++;
                oFile = new System.IO.FileInfo(FileName);
                nLen = oFile.Length;
            }
        }
        #endregion
        #endregion
        #region common method
        /// <summary>
        /// 图片上传错误编码
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private string GetCodeMessage(int code)
        {
            var dic = new Dictionary<int, string>(){
                {0,"系统配置错误"},
                {1,"上传图片成功"},
                {2,string.Format( "对不起，上传格式错误！请上传{0}格式图片",SetAllowFormat)},
                {3,string.Format("超过文件上传大小,不得超过{0}M",SetAllowSize)},
                {4,"未上传文件"},
                {5,""},
                {6,"缩略图长度和宽度配置错误"},
                {7,"检测图片宽度限制"}
                 };
            return dic[code];
        }
        private void TryError(ResponseMessage rm, int code)
        {
            rm.IsError = true;
            rm.Message = GetCodeMessage(code);
        }
        private void TryError(ResponseMessage rm, string message)
        {
            rm.IsError = true;
            rm.Message = message;
        }
        #endregion
    }
}
