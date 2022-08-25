namespace SuperFramework.SuperImage
{
    /// <summary>
    /// 版 本:Release
    /// 日 期:2014-09-23
    /// 作 者:不良帥
    /// 描 述:图片处理相关枚举
    /// </summary>
    public static class ImageEnum
    {
        #region 裁剪方式
        /// <summary>
        /// 裁剪方式
        /// </summary>
        public enum CutMode
        {
            /// <summary>
            /// 根据高宽剪切
            /// </summary>
            CutWH = 1,
            /// <summary>
            /// 根据宽剪切
            /// </summary>
            CutW = 2,
            /// <summary>
            /// 根据高剪切
            /// </summary>
            CutH = 3,
            /// <summary>
            /// 缩放不剪裁
            /// </summary>
            CutNo = 4
        }
        #endregion

        #region 水印位置
        /// <summary>
        /// 水印位置
        /// </summary>
        public enum WaterPosition
        {
            /// <summary>
            /// 左上
            /// </summary>
            LeftTop,        //左上
            /// <summary>
            /// 左下
            /// </summary>
            LeftBottom,    //左下
            /// <summary>
            /// 右上
            /// </summary>
            RightTop,       //右上
            /// <summary>
            /// 右下
            /// </summary>
            RigthBottom, //右下
            /// <summary>
            /// 顶部居中
            /// </summary>
            TopMiddle,     //顶部居中
            /// <summary>
            /// 底部居中
            /// </summary>
            BottomMiddle, //底部居中
            /// <summary>
            /// 中心
            /// </summary>
            Center           //中心
        }
        #endregion

        #region 缩略图模式
        /// <summary>
        /// 缩略图模式
        /// </summary>
        public enum ThumbnailMod : byte
        {
            /// <summary>
            /// //指定高宽缩放（可能变形）
            /// </summary>
            Cut,
            /// <summary>
            /// //指定宽，高按比例
            /// </summary>
            H,
            /// <summary>
            /// //指定高，宽按比例
            /// </summary>
            W,
            /// <summary>
            /// //指定高宽裁减（不变形）  
            /// </summary>
            HW
        }
        #endregion

        #region 文件类型
        /// <summary>
        /// 文件类型
        /// </summary>
        public enum FileExtension
        {
            JPG = 255216,
            GIF = 7173,
            BMP = 6677,
            PNG = 13780,
            RAR = 8297,
            jpg = 255216,
            exe = 7790,
            xml = 6063,
            html = 6033,
            aspx = 239187,
            cs = 117115,
            js = 119105,
            txt = 210187,
            sql = 255254
        }
        #endregion
    }
}
