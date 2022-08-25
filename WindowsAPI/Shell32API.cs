using System;
using System.Runtime.InteropServices;
using static SuperFramework.WindowsAPI.APIStruct;

namespace SuperFramework.WindowsAPI
{
    public static class Shell32API
    {
        /// <summary>
        /// 获取操作错误码
        /// </summary>
        /// <param name="lpFileOp"></param>
        /// <returns>返回错误码</returns>
        [DllImport("shell32.dll")]
        public static extern int SHFileOperation(ref SHFILEOPSTRUCT lpFileOp);


        #region 获得文件图标句柄
        /// <summary>
        /// 获得文件图标句柄
        /// </summary>
        /// <param name="pszPath">指定的文件名</param>
        /// <param name="dwFileAttribute">文件属性</param>
        /// <param name="psfi">记录类型，返回获得的文件信息</param>
        /// <param name="cbSizeFileInfo">psfi的比特值</param>
        /// <param name="Flags">指明需要返回的文件信息标识符</param>
        /// <returns>文件的图标句柄</returns>
        [DllImport("shell32.dll", EntryPoint = "SHGetFileInfo")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttribute, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint Flags);
        #endregion
    }
}
