/******************************************
 * 
 * ģ�����ƣ��ڲ��� LogQueue
 * ��ǰ�汾��1.0
 * ������Ա��������
 * ����ʱ�䣺2011-06-28
 * �汾��ʷ��
 * 
 * Copyright (C) ������ 2011-2015
 * 
 ******************************************/
using System.Collections;

namespace SuperFramework.SuperNLogger.Asynchronous
{
    internal class LogQueue
    {
        /// <summary>
        /// ��־ ����
        /// </summary>
        //public static Queue<LogEntity> LOG_QUEUE = new Queue<LogEntity>();
        internal static Queue LOG_QUEUE = Queue.Synchronized(new Queue(512));
    }
}
