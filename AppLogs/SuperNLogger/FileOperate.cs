/******************************************
 * 
 * 模块名称：内部类 日志文件操作类
 * 当前版本：1.0
 * 开发人员：不良帥
 * 开发时间：2011-06-29
 * 版本历史：
 * 
 * Copyright (C) 不良帥 2011-2015
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
    /// 日志文件操作类
    /// </summary>
    internal class FileOperate
    {
        /// <summary>
        /// 日志文件扩展名
        /// </summary>
        internal const string log_extension = ".log";
        /// <summary>
        /// 日志文件路径+文件名（不含文件扩展名）
        /// </summary>
        private string file_dir_name = "Logger";

        /// <summary>
        /// 构造函数 FileOperate
        /// </summary>
        /// <param name="strDirAndName">日志文件路径+文件名（不含文件扩展名）</param>
        internal FileOperate(string strDirAndName)
        {
            if (!string.IsNullOrEmpty(strDirAndName))
            {
                file_dir_name = strDirAndName;
            }
        }

        /// <summary>
        /// 打开一个文件，向其中追加指定的字符串，然后关闭该文件。
        /// 如果文件不存在，此方法创建一个文件，将指定的字符串写入文件，然后关闭该文件。
        /// 如果文件大小超过一兆(可配置)，则转储备份并新建一同名文件，再追加，然后关闭该文件。
        /// </summary>
        /// <param name="logFileSuffix">日志文件后缀（用以分开记录日志之用，若不分开记录日志可为空）</param>
        /// <param name="contents">要追加到文件中的字符串</param>
        /// <param name="isAsyn">isAsyn TRUE异步记入日志，FALSE</param>
        internal void AppendFile(string logFileSuffix, string contents, bool isAsyn)
        {
            //日志文件路径+文件名+扩展名
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
                    AppendFileImmediately(logFileSuffix, contents); //一次写入失败则二次立即写入 二次也失败则写入系统日志
                }
            }
        }
        /// <summary>
        /// 打开一个文件，立即向其中追加指定的字符串，然后关闭该文件。
        /// 如果文件不存在，此方法创建一个文件，将指定的字符串写入文件，然后关闭该文件。
        /// 如果文件大小超过一兆(可配置)，则转储备份并新建一同名文件，再追加，然后关闭该文件。
        /// </summary>
        /// <param name="logFileSuffix">日志文件后缀（用以分开记录日志之用，若不分开记录日志可为空）</param>
        /// <param name="contents">要追加到文件中的字符串</param>
        internal void AppendFileImmediately(string logFileSuffix, string contents)
        {
            //日志文件路径+文件名+扩展名
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
                //System.Threading.Thread.Sleep(100); //应该计入操作系统 事件日志
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
