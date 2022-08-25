/******************************************
 * 
 * 模块名称：内部类 LogWrite
 * 当前版本：1.0
 * 开发人员：不良帥
 * 开发时间：2011-06-28
 * 版本历史：
 * 
 * Copyright (C) 不良帥 2011-2015
 * 
 ******************************************/
using System.Collections.Generic;
using System.Threading;
using System.IO;

namespace SuperFramework.SuperNLogger.Asynchronous
{
    internal class LogWrite
    {
        /// <summary>
        /// 标识线程是否运行，这里 true 代表运行，false代表停止
        /// </summary>
        internal static volatile bool isRun = false;
        /// <summary>
        /// 当日志队列为空时等待的毫秒数
        /// </summary>
        internal static int threadWait = 100;
        /// <summary>
        /// 文件写入 线程列表 一个文件一个线程写入
        /// </summary>
        private static List<LogWriteThreadByFile> lstLogFileThread = new List<LogWriteThreadByFile>();

        /// <summary>
        /// 循环追加日志
        /// </summary>
        public void LogAppend()
        {
            while (isRun)
            {
                while (LogQueue.LOG_QUEUE.Count > 0)
                {
                    LogEntity entity = LogQueue.LOG_QUEUE.Dequeue() as LogEntity;
                    //Console.WriteLine(LogQueue.LOG_QUEUE.Count + " _ " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff"));//调试测试打印//耗时
                    //Console.WriteLine(LogQueue.LOG_QUEUE.Count);//调试测试打印
                    if (entity != null)
                    {
                        //string contents = entity.LOG_CONTENTS;
                        string file_dir_name = entity.FILE_DIR_NAME;
                        string logFileSuffix = entity.LOG_FILE_SUFFIX;
                        string log_extension = entity.LOG_EXTENSION;

                        //日志文件路径+文件名+扩展名
                        string filename_full = file_dir_name + logFileSuffix + log_extension;

                        FileInfo fi = new FileInfo(filename_full);
                        if (!fi.Directory.Exists)
                        {
                            fi.Directory.Create();//fi.DirectoryName :: fi.Name
                        }
                        //if (fi.Exists)
                        //{
                        //    //1M = 1*1024*1024=1048576
                        //    if (fi.Length > SysLogger.GetInstance().LogFileMaxSize)
                        //    {
                        //        //fi.MoveTo(fi.DirectoryName + "\\" + fi.Name.Replace(log_extension, "_") + DateTime.Now.Ticks.ToString() + log_extension);
                        //        fi.MoveTo(file_dir_name + logFileSuffix + "_" + DateTime.Now.Ticks.ToString() + log_extension);
                        //    }
                        //}
                        bool isFind = false;
                        foreach (LogWriteThreadByFile item in lstLogFileThread)
                        { 
                            if(item.FILENAME_FULL == filename_full)
                            {
                                item.QUEUE_CONTENTS.Enqueue(entity);
                                isFind = true;
                            }
                        }
                        if (!isFind)
                        {
                            LogWriteThreadByFile lwt = new LogWriteThreadByFile(filename_full);
                            lwt.QUEUE_CONTENTS.Enqueue(entity);
                            lstLogFileThread.Add(lwt);
                        }

                        //File.AppendAllText(filename_full, contents + "\n", Encoding.UTF8);
                    }
                }
                if (LogQueue.LOG_QUEUE.Count <= 0)
                {
                    if (threadWait > 0)
                    {
                        Thread.Sleep(threadWait);
                    }
                }
            }
        }
    }
}
