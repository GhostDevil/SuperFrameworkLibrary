/******************************************
 * 
 * ģ�����ƣ���־����
 * ��ǰ�汾��1.0
 * ������Ա��������
 * ����ʱ�䣺2011-06-30
 * �汾��ʷ��
 * 
 * Copyright (C) ������ 2011-2015  
 * 
 ******************************************/

namespace SuperFramework.SuperNLogger
{
    /// <summary>
    /// ��־����  �ӵ͵�������˳��DEBUG|INFO|WARN|ERROR|FATAL
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// All
        /// </summary>
        All=0,
        /// <summary>
        /// DEBUG
        /// </summary>
        DEBUG = 1,
        /// <summary>
        /// INFO
        /// </summary>
        INFO = 2,
        /// <summary>
        /// WARN
        /// </summary>
        WARN = 3,
        /// <summary>
        /// ERROR
        /// </summary>
        ERROR = 4,
        /// <summary>
        /// FATAL
        /// </summary>
        FATAL = 5
    }

    /// <summary>
    /// ��־�������ַ������໥ת��
    /// </summary>
    internal class LogLevelHelper
    {
        /// <summary>
        /// ��־�����ַ���ת��Ϊ��־����
        /// </summary>
        /// <param name="strLogLevel">��־�����ַ���</param>
        /// <returns>��־����Ĭ��ΪDEBUG</returns>
        internal static LogLevel StrToLogLevel(string strLogLevel)
        {
            LogLevel _loglevel = LogLevel.DEBUG;
            if (string.IsNullOrEmpty(strLogLevel))
            {
                return _loglevel;
            }
            string level = strLogLevel.ToUpper();
            
            switch (level)
            {
                case "All":
                    _loglevel = LogLevel.All;
                    break;
                case "DEBUG":
                    _loglevel = LogLevel.DEBUG;
                    break;
                case "INFO":
                    _loglevel = LogLevel.INFO;
                    break;
                case "WARN":
                    _loglevel = LogLevel.WARN;
                    break;
                case "ERROR":
                    _loglevel = LogLevel.ERROR;
                    break;
                case "FATAL":
                    _loglevel = LogLevel.FATAL;
                    break;
            }
            return _loglevel;
        }
    }
}
