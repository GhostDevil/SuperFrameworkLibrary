/******************************************
 * 
 * 模块名称：内部类 LogQueue
 * 当前版本：1.0
 * 开发人员：不良帥
 * 开发时间：2011-06-28
 * 版本历史：
 * 
 * Copyright (C) 不良帥 2011-2015
 * 
 ******************************************/
using System.Collections;

namespace SuperFramework.SuperNLogger.Asynchronous
{
    internal class LogQueue
    {
        /// <summary>
        /// 日志 队列
        /// </summary>
        //public static Queue<LogEntity> LOG_QUEUE = new Queue<LogEntity>();
        internal static Queue LOG_QUEUE = Queue.Synchronized(new Queue(512));
    }
}
