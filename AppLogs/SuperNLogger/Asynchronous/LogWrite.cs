/******************************************
 * 
 * ģ�����ƣ��ڲ��� LogWrite
 * ��ǰ�汾��1.0
 * ������Ա��������
 * ����ʱ�䣺2011-06-28
 * �汾��ʷ��
 * 
 * Copyright (C) ������ 2011-2015
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
        /// ��ʶ�߳��Ƿ����У����� true �������У�false����ֹͣ
        /// </summary>
        internal static volatile bool isRun = false;
        /// <summary>
        /// ����־����Ϊ��ʱ�ȴ��ĺ�����
        /// </summary>
        internal static int threadWait = 100;
        /// <summary>
        /// �ļ�д�� �߳��б� һ���ļ�һ���߳�д��
        /// </summary>
        private static List<LogWriteThreadByFile> lstLogFileThread = new List<LogWriteThreadByFile>();

        /// <summary>
        /// ѭ��׷����־
        /// </summary>
        public void LogAppend()
        {
            while (isRun)
            {
                while (LogQueue.LOG_QUEUE.Count > 0)
                {
                    LogEntity entity = LogQueue.LOG_QUEUE.Dequeue() as LogEntity;
                    //Console.WriteLine(LogQueue.LOG_QUEUE.Count + " _ " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff"));//���Բ��Դ�ӡ//��ʱ
                    //Console.WriteLine(LogQueue.LOG_QUEUE.Count);//���Բ��Դ�ӡ
                    if (entity != null)
                    {
                        //string contents = entity.LOG_CONTENTS;
                        string file_dir_name = entity.FILE_DIR_NAME;
                        string logFileSuffix = entity.LOG_FILE_SUFFIX;
                        string log_extension = entity.LOG_EXTENSION;

                        //��־�ļ�·��+�ļ���+��չ��
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
