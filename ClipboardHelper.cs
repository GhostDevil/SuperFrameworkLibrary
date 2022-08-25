using System.Drawing;
using System.Windows.Forms;

namespace SuperFramework
{
    /// <summary>
    /// <para>日 期:2015-08-18</para>
    /// <para>作 者:不良帥</para>
    /// <para>描 述:剪贴板辅助类</para>
    /// </summary>
    public static class ClipboardHelper
    {
        #region 粘贴
        /// <summary>
        /// 粘贴字符串
        /// </summary>
        ///<returns>字符串</returns>
        public static string PasteString()
        {

            IDataObject iData = Clipboard.GetDataObject();
            if (iData.GetDataPresent(DataFormats.Text))
                return (string)iData.GetData(DataFormats.Text);
            return "";
        }
        /// <summary>
        /// 粘贴图片
        /// </summary>
        ///<returns>Bitmap 对象</returns>
        public static Bitmap PasteBitmap()
        {

            IDataObject iData = Clipboard.GetDataObject();
            if (iData.GetDataPresent(DataFormats.Bitmap))
                return (Bitmap)iData.GetData(DataFormats.Bitmap);
            return null;
        }
        /// <summary>
        /// 粘贴
        /// </summary>
        /// <param name="type">从DataFormats类中获取类型 如：DataFormats.Bitmap</param>
        ///<returns>object 对象</returns>
        public static object PasteObject(string type)
        {

            IDataObject iData = Clipboard.GetDataObject();
            if (iData.GetDataPresent(type))
                return iData.GetData(type);
            return null;
        }
        #endregion

        #region 复制
        /// <summary>
        /// 复制字符串
        /// </summary>
        /// <param name="str">字符串</param>
        public static void CopyString(string str)
        {
            Clipboard.SetDataObject(str);
        }
        /// <summary>
        /// 复制图片
        /// </summary>
        /// <param name="img">图片对象</param>
        public static void CopyString(Image img)
        {
            Clipboard.SetDataObject(img);
        }
        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="obj">复制内容</param>
        public static void CopyString(object obj)
        {
            Clipboard.SetDataObject(obj);
        }
        #endregion
    }
}
