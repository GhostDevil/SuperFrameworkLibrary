/******************************************
 * 
 * 模块名称：内部公共类
 * 当前版本：1.0
 * 开发人员：不良帥
 * 开发时间：2011-11-16
 * 版本历史：
 * 
 * Copyright (C) 不良帥 2011-2015 
 * 
 ******************************************/
using System.IO;
using System.IO.Compression;

namespace SuperFramework.SuperNLogger
{
    /// <summary>
    /// 内部公共类
    /// </summary>
    internal class LogUtil
    {
        /// <summary>
        /// 配置文件路径
        /// </summary>
        internal static string filePath = "SuperFramework\\ConfigInfo\\LogsConfig.xml";
        /// <summary>
        /// 获取配置信息的值
        /// </summary>
        /// <param name="cfgKey">配置关键字key</param>
        /// <returns>配置信息对应值</returns>
        internal static string GetConfig(string cfgKey)
        {
            try
            {
               return SuperNetCore.SuperConfig.Xml.XmlHelper.GetNodeInfoByNodeName(filePath, cfgKey);
                //return ConfigurationManager.AppSettings[cfgKey].ToString();//appconfig
            }
            catch 
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 重命名文件
        /// </summary>
        /// <param name="fi">FileInfo文件对象</param>
        /// <param name="fileFullName">待重命名后的完全名（含路径）</param>
        internal static void FileRename(FileInfo fi, string fileFullName)
        {
            try
            {
                fi.MoveTo(fileFullName);
            }
            catch { }
        }

        /// <summary>
        /// 创建压缩文件并删除源文件
        /// </summary>
        /// <param name="fileFullName">源文件完全限定名称</param>
        /// <param name="zipFile">待创建的压缩文件全名(.zip)</param>
        internal static void CreateZipFile(string fileFullName, string zipFile)
        {
            try
            {
                CompressFile(fileFullName, zipFile);
                File.Delete(fileFullName);
            }
            catch { }
        }

        /// <summary>
        /// 将原文件sourceFile压缩targetFile文件
        /// </summary>
        /// <param name="sourceFile">源文件完全限定名称</param>
        /// <param name="targetFile">待创建的目标压缩文件全名</param>
        private static void CompressFile(string sourceFile, string targetFile)
        {
            byte[] buffer = new byte[8192];
            using (FileStream targetStream = new(targetFile, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (GZipStream compressedStream = new(targetStream, CompressionMode.Compress, true))
                {
                    using (FileStream sourceStream = File.OpenRead(sourceFile))
                    {
                        int sourceBytes;
                        do
                        {
                            sourceBytes = sourceStream.Read(buffer, 0, buffer.Length);
                            compressedStream.Write(buffer, 0, sourceBytes);
                            compressedStream.Flush();
                        } while (sourceBytes > 0);
                    }
                }
            }
        }

    }
}
