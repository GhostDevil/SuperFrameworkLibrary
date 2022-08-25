using System.Collections.Generic;
using static SuperFramework.SuperFTP.FTPWinAPI;

namespace SuperFramework.SuperFTP
{
    /// <summary>
    /// 比较对象
    /// </summary>
    public class FileComparer : IComparer<FileStruct>
    {
        /// <summary>
        /// 比较文件信息
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(FileStruct x, FileStruct y)
        {
            return x.CreateTime.CompareTo(y.CreateTime);
        }
    }
}
