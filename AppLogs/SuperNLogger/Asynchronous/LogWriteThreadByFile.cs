/******************************************
 * 
 * ģ�����ƣ��ڲ��� LogWriteThreadByFile
 * ��ǰ�汾��1.0
 * ������Ա��������
 * ����ʱ�䣺2011-06-29
 * �汾��ʷ��
 * 
 * Copyright (C) ������ 2011-2015
 * 
 ******************************************/
using System;
using System.Text;
using System.Collections;
using System.Threading;
using System.IO;

namespace SuperFramework.SuperNLogger.Asynchronous
{
    internal class LogWriteThreadByFile
    {
        private string _filename_full;
        private Queue _queue_contents;
        private Thread _thread;
        private int _thread_wait = 100;

        internal LogWriteThreadByFile(string filename_full)
        {
            _filename_full = filename_full;
            _queue_contents = Queue.Synchronized(new Queue(512));
            _thread = new Thread(WriteLogContents);
            _thread.IsBackground = NLogger.GetInstance().THREAD_IS_BACKGROUD;
            _thread.Start();
            if (NLogger.GetInstance().LOG_THREAD_WAIT > 0)
            {
                _thread_wait = NLogger.GetInstance().LOG_THREAD_WAIT;
            }
        }

        /// <summary>
        /// ��־�ļ�ȫ�� ·��+�ļ���+�ļ���չ��
        /// </summary>
        internal string FILENAME_FULL
        {
            get { return _filename_full; }
        }
        /// <summary>
        /// ��־����
        /// </summary>
        internal Queue QUEUE_CONTENTS
        {
            get { return _queue_contents; }
        }

        /// <summary>
        /// ��ͬһ����־�ļ�ѭ��׷����־���ݣ�����־����Ϊ��ʱ�߳�Sleepһ��ʱ��
        /// </summary>
        private void WriteLogContents()
        {
            while (true)
            {
                while (QUEUE_CONTENTS.Count > 0)
                {
                    LogEntity entity = QUEUE_CONTENTS.Dequeue() as LogEntity;
                    if (entity != null)
                    {
                        FileInfo fi = new FileInfo(FILENAME_FULL);
                        if (fi.Exists)
                        {
                            //1M = 1*1024*1024=1048576
                            if (fi.Length > NLogger.GetInstance().LogFileMaxSize)
                            {
                                //fi.MoveTo(entity.FILE_DIR_NAME + entity.LOG_FILE_SUFFIX + "_" + DateTime.Now.Ticks.ToString() + entity.LOG_EXTENSION);
                                string newFileName = entity.FILE_DIR_NAME + entity.LOG_FILE_SUFFIX + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + entity.LOG_EXTENSION;
                                //fi.MoveTo(newFileName);
                                LogUtil.FileRename(fi, newFileName);
                            }
                        }
                        try
                        {
                            File.AppendAllText(FILENAME_FULL, entity.LOG_CONTENTS + "\r\n", Encoding.UTF8);
                        }
                        catch 
                        {
                            System.Threading.Thread.Sleep(100);
                            QUEUE_CONTENTS.Enqueue(entity);
                        }
                    }
                }
                if (QUEUE_CONTENTS.Count <= 0)
                {
                    if (_thread_wait > 0)
                    {
                        Thread.Sleep(_thread_wait);
                    }
                }
            }
        }
    }
}
