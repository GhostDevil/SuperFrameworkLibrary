/******************************************
 * 
 * ģ�����ƣ��ڲ��� ��־�ļ�������
 * ��ǰ�汾��1.0
 * ������Ա��������
 * ����ʱ�䣺2011-06-29
 * �汾��ʷ��
 * 
 * Copyright (C) ������ 2011-2015
 * 
 ******************************************/
using SuperFramework.SuperNLogger.Asynchronous;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace SuperFramework.SuperNLogger
{
    /// <summary>
    /// ��־�ļ�������
    /// </summary>
    internal class FileOperate
    {
        /// <summary>
        /// ��־�ļ���չ��
        /// </summary>
        internal const string log_extension = ".log";
        /// <summary>
        /// ��־�ļ�·��+�ļ����������ļ���չ����
        /// </summary>
        private string file_dir_name = "Logger";

        /// <summary>
        /// ���캯�� FileOperate
        /// </summary>
        /// <param name="strDirAndName">��־�ļ�·��+�ļ����������ļ���չ����</param>
        internal FileOperate(string strDirAndName)
        {
            if (!string.IsNullOrEmpty(strDirAndName))
            {
                file_dir_name = strDirAndName;
            }
        }

        /// <summary>
        /// ��һ���ļ���������׷��ָ�����ַ�����Ȼ��رո��ļ���
        /// ����ļ������ڣ��˷�������һ���ļ�����ָ�����ַ���д���ļ���Ȼ��رո��ļ���
        /// ����ļ���С����һ��(������)����ת�����ݲ��½�һͬ���ļ�����׷�ӣ�Ȼ��رո��ļ���
        /// </summary>
        /// <param name="logFileSuffix">��־�ļ���׺�����Էֿ���¼��־֮�ã������ֿ���¼��־��Ϊ�գ�</param>
        /// <param name="contents">Ҫ׷�ӵ��ļ��е��ַ���</param>
        /// <param name="isAsyn">isAsyn TRUE�첽������־��FALSE</param>
        internal void AppendFile(string logFileSuffix, string contents, bool isAsyn)
        {
            //��־�ļ�·��+�ļ���+��չ��
            string filename_full = file_dir_name + logFileSuffix + log_extension;

            if (isAsyn)
            {
                LogEntity entity = new()
                {
                    LOG_CONTENTS = contents,
                    FILE_DIR_NAME = file_dir_name,
                    LOG_FILE_SUFFIX = logFileSuffix,
                    LOG_EXTENSION = log_extension
                };
                LogQueue.LOG_QUEUE.Enqueue(entity);
            }
            else
            {
                FileInfo fi = new(filename_full);
                if (!fi.Directory.Exists)
                {
                    fi.Directory.Create();//fi.DirectoryName :: fi.Name
                }
                if (fi.Exists)
                {
                    //1M = 1*1024*1024=1048576
                    if (fi.Length > NLogger.GetInstance().LogFileMaxSize)
                    {
                        ////fi.MoveTo(fi.DirectoryName + "\\" + fi.Name.Replace(log_extension, "_") + DateTime.Now.Ticks.ToString() + log_extension);
                        //fi.MoveTo(file_dir_name + logFileSuffix + "_" + DateTime.Now.Ticks.ToString() + log_extension);
                        string newFileName = file_dir_name + logFileSuffix + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + log_extension;
                        //fi.MoveTo(newFileName);
                        LogUtil.FileRename(fi, newFileName);
                    }
                }
                try
                {
                    File.AppendAllText(filename_full, contents + "\r\n", Encoding.UTF8);
                }
                catch
                {
                    //System.Threading.Thread.Sleep(100);
                    //AppendFile(logFileSuffix, contents, isAsyn);
                    AppendFileImmediately(logFileSuffix, contents); //һ��д��ʧ�����������д�� ����Ҳʧ����д��ϵͳ��־
                }
            }
        }
        /// <summary>
        /// ��һ���ļ�������������׷��ָ�����ַ�����Ȼ��رո��ļ���
        /// ����ļ������ڣ��˷�������һ���ļ�����ָ�����ַ���д���ļ���Ȼ��رո��ļ���
        /// ����ļ���С����һ��(������)����ת�����ݲ��½�һͬ���ļ�����׷�ӣ�Ȼ��رո��ļ���
        /// </summary>
        /// <param name="logFileSuffix">��־�ļ���׺�����Էֿ���¼��־֮�ã������ֿ���¼��־��Ϊ�գ�</param>
        /// <param name="contents">Ҫ׷�ӵ��ļ��е��ַ���</param>
        internal void AppendFileImmediately(string logFileSuffix, string contents)
        {
            //��־�ļ�·��+�ļ���+��չ��
            string filename_full = file_dir_name + logFileSuffix + log_extension;
            FileInfo fi = new(filename_full);
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();//fi.DirectoryName :: fi.Name
            }
            if (fi.Exists)
            {
                //1M = 1*1024*1024=1048576
                if (fi.Length > NLogger.GetInstance().LogFileMaxSize)
                {
                    ////fi.MoveTo(fi.DirectoryName + "\\" + fi.Name.Replace(log_extension, "_") + DateTime.Now.Ticks.ToString() + log_extension);
                    //fi.MoveTo(file_dir_name + logFileSuffix + "_" + DateTime.Now.Ticks.ToString() + log_extension);
                    string newFileName = file_dir_name + logFileSuffix + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + log_extension;
                    //fi.MoveTo(newFileName);
                    LogUtil.FileRename(fi, newFileName);
                }
            }
            try
            {
                File.AppendAllText(filename_full, contents + "\r\n", Encoding.UTF8);
            }
            catch
            {
                //System.Threading.Thread.Sleep(100); //Ӧ�ü������ϵͳ �¼���־
                //AppendFile(logFileSuffix, contents, isAsyn);
                try
                {
                    EventLog.WriteEntry("Application", contents, EventLogEntryType.Error);
                }
                catch { }
            }
        }

    }
}
