/******************************************
 * 
 * ģ�����ƣ��ڲ��� LogThread
 * ��ǰ�汾��1.0
 * ������Ա��������
 * ����ʱ�䣺2011-06-28
 * �汾��ʷ��
 * 
 * Copyright (C) ������ 2011-2015 
 * 
 ******************************************/
using System.Threading;

namespace SuperFramework.SuperNLogger.Asynchronous
{
    internal class LogThread
    {
        /// <summary>
        /// ������־�첽д���߳�
        /// </summary>
        /// <param name="wait">����־����Ϊ��ʱ�ȴ��ĺ�����</param>
        /// <param name="threadIsBackgroud">true��̨�̣߳�falseǰ̨�߳�</param>
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
                //logThread.IsBackground = true;//true ����Ϊ��̨�̣߳���ǰ̨�߳�ȫ�������󣬺�̨Ҳ���Ž�����
                IsBackground = threadIsBackgroud//false ǰ̨�߳�
            };
            logThread.Start();
        }
    }
}
