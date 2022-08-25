using System.IO;
using static SuperFramework.SuperImage.ImageEnum;
namespace SuperFramework.SuperImage
{
    /// <summary>
    /// 版 本:Release
    /// 日 期:2014-09-23
    /// 作 者:不良帥
    /// 描 述:图片处理相关类
    /// </summary>
    public static class ImageClasses
    {
        #region 请求返回消息
        /// <summary>
        /// 请求返回消息
        /// </summary>
        public class ResponseMessage
        {
            /// <summary>
            /// 是否遇到错误
            /// </summary>
            public bool IsError { get; set; }
            /// <summary>
            /// web路径
            /// </summary>
            public string WebPath { get; set; }
            /// <summary>
            /// 文件物理路径
            /// </summary>
            public string filePath { get; set; }
            /// <summary>
            /// 反回消息
            /// </summary>
            public string Message { get; set; }
            /// <summary>
            /// 文件大小
            /// </summary>
            public double Size { get; set; }
            /// <summary>
            /// 图片名
            /// </summary>
            public string FileName { get; set; }
            /// <summary>
            /// 图片目录
            /// </summary>
            public string Directory
            {
                get
                {
                    if (WebPath == null) return null;
                    return WebPath.Replace(FileName, "");
                }
            }
            /// <summary>
            /// 缩略图路径
            /// </summary>
            public string SmallPath(int index)
            {
                return string.Format("{0}{1}_{2}{3}", Directory, Path.GetFileNameWithoutExtension(FileName), index, Path.GetExtension(FileName));
            }
        }
        #endregion

        #region 装载水印图片的相关信息
        /// <summary>
        /// 装载水印图片的相关信息
        /// </summary>
        public struct WaterInfo
        {
            private string m_sourcePicture;
            /// <summary>
            /// 源图片地址名字(带后缀)
            /// </summary>
            public string SourcePicture
            {
                get { return m_sourcePicture; }
                set { m_sourcePicture = value; }
            }



            private string m_waterImager;
            /// <summary>
            /// 水印图片名字(带后缀)
            /// </summary>
            public string WaterPicture
            {
                get { return m_waterImager; }
                set { m_waterImager = value; }
            }

            private float m_alpha;
            /// <summary>
            /// 水印图片文字的透明度
            /// </summary>
            public float Alpha
            {
                get { return m_alpha; }
                set { m_alpha = value; }
            }

            private WaterPosition m_postition;
            /// <summary>
            /// 水印图片或文字在图片中的位置
            /// </summary>
            public WaterPosition Position
            {
                get { return m_postition; }
                set { m_postition = value; }
            }

            private string m_words;
            /// <summary>
            /// 水印文字的内容
            /// </summary>
            public string Words
            {
                get { return m_words; }
                set { m_words = value; }
            }

        }
        #endregion
    }
}
