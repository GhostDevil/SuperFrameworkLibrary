/******************************************
 * 
 * ģ�����ƣ��ڲ�������
 * ��ǰ�汾��1.0
 * ������Ա��������
 * ����ʱ�䣺2011-11-16
 * �汾��ʷ��
 * 
 * Copyright (C) ������ 2011-2015 
 * 
 ******************************************/
using System.IO;
using System.IO.Compression;

namespace SuperFramework.SuperNLogger
{
    /// <summary>
    /// �ڲ�������
    /// </summary>
    internal class LogUtil
    {
        /// <summary>
        /// �����ļ�·��
        /// </summary>
        internal static string filePath = "SuperFramework\\ConfigInfo\\LogsConfig.xml";
        /// <summary>
        /// ��ȡ������Ϣ��ֵ
        /// </summary>
        /// <param name="cfgKey">���ùؼ���key</param>
        /// <returns>������Ϣ��Ӧֵ</returns>
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
        /// �������ļ�
        /// </summary>
        /// <param name="fi">FileInfo�ļ�����</param>
        /// <param name="fileFullName">�������������ȫ������·����</param>
        internal static void FileRename(FileInfo fi, string fileFullName)
        {
            try
            {
                fi.MoveTo(fileFullName);
            }
            catch { }
        }

        /// <summary>
        /// ����ѹ���ļ���ɾ��Դ�ļ�
        /// </summary>
        /// <param name="fileFullName">Դ�ļ���ȫ�޶�����</param>
        /// <param name="zipFile">��������ѹ���ļ�ȫ��(.zip)</param>
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
        /// ��ԭ�ļ�sourceFileѹ��targetFile�ļ�
        /// </summary>
        /// <param name="sourceFile">Դ�ļ���ȫ�޶�����</param>
        /// <param name="targetFile">��������Ŀ��ѹ���ļ�ȫ��</param>
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
