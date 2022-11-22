/******************************************
 * 
 * 模块名称：内部类 LogThread
 * 当前版本：1.0
 * 开发人员：不良帥
 * 开发时间：2011-06-28
 * 版本历史：
 * 
 * Copyright (C) 不良帥 2011-2015 
 * 
 ******************************************/
using System.Threading;

namespace SuperFramework.SuperNLogger.Asynchronous
{
    internal class LogThread
    {
        /// <summary>
        /// 启动日志异步写入线程
        /// </summary>
        /// <param name="wait">当日志队列为空时等待的毫秒数</param>
        /// <param name="threadIsBackgroud">true后台线程，false前台线程</param>
        internal static void RunAsynchronous(int wait, bool threadIsBackgroud)
        {
            LogWrite log = new();
            LogWrite.isRun = true;
            if (wait >= 0)
            {
                LogWrite.threadWait = wait;
            }
            Thread logThread = new(log.LogAppend)
            {
                //logThread.IsBackground = true;//true 设置为后台线程，既前台线程全部结束后，后台也跟着结束。
                IsBackground = threadIsBackgroud//false 前台线程
            };
            logThread.Start();
        }
    }
}
