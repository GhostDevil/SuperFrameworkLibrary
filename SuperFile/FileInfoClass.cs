using System;

namespace SuperFramework.SuperFile
{
    /// <summary>
    /// 日 期:2015-06-24
    /// 作 者:不良帥
    /// 描 述:文件或目录操作信息对象
    /// </summary>
    public static class FileInfoClass
    {

        #region 文件大小单位
        /// <summary>
        /// 文件大小单位
        /// </summary>
        public enum SizeUnit
        {
            /// <summary>
            /// 字节
            /// </summary>
            Bytes,
            /// <summary>
            /// 千字节（1024Byte）
            /// </summary>
            KB,
            /// <summary>
            /// 兆字节（1024KB）
            /// </summary>
            MB,
            /// <summary>
            /// 吉字节（1024MB）
            /// </summary>
            GB,
            /// <summary>
            /// 太字节（1024GB）
            /// </summary>
            TB,
            /// <summary>
            /// 拍字节（1024TB）
            /// </summary>
            PB,
            /// <summary>
            /// 艾字节（1024PB）
            /// </summary>
            EB,
            /// <summary>
            /// 泽字节（1024EB）
            /// </summary>
            ZB,
            /// <summary>
            /// 尧字节（1024ZB）
            /// </summary>
            YB,
            /// <summary>
            /// BB（1024YB）
            /// </summary>
            BB,
            /// <summary>
            /// NB（1024BB）
            /// </summary>
            NB,
            /// <summary>
            /// DB（1024NB）
            /// </summary>
            DB

        }

        #endregion

        #region 文件或目录属性结构
        /// <summary>
        /// 文件属性结构
        /// </summary>
        public struct FileAttribute
        {
            /// <summary>
            /// 文件或目录的完整路径
            /// </summary>
            public string FullName;
            /// <summary>
            /// 文件或目录的名称
            /// </summary>
            public string Name;
            /// <summary>
            /// 文件或目录的大小（字节）
            /// </summary>
            public long Length;
            /// <summary>
            /// 文件或目录的创建时间
            /// </summary>
            public DateTime CreationTime;
            /// <summary>
            /// 上次访问目录或文件的时间
            /// </summary>
            public DateTime LastAccessTime;
            /// <summary>
            /// 上次写入目录或文件的时间
            /// </summary>
            public DateTime LastWriteTime;
            /// <summary>
            /// 获取表示目录的完整路径字符串
            /// </summary>
            public string DirectoryName;
            /// <summary>
            /// 文件扩展名部分的字符串
            /// </summary>
            public string Extension;
        }
        #endregion
    }
}
