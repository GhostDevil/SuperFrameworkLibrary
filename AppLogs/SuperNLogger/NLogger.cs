/******************************************
 * 
 * ģ�����ƣ���־��
 * ��ǰ�汾��1.0
 * ������Ա��������
 * ����ʱ�䣺2011-11-14
 * �汾��ʷ��
 * 
 * 2016-03-13 Fatal������־ִ������д�룬����д��ʧ�ܵ���־��������ϵͳ�¼���־��
 *            ����ͬ��д����־����һ��д��ʧ�ܵ���־��ִ�ж���д�룬����д��ִ������д�룻
 *            �����첽д����־����һ��д��ʧ�ܵ���־��ִ�ж���д�룬����д��ִ�м����β�Ŷ�д��
 * 
 * Copyright (C) ������ 2011-2015 
 * 
 ******************************************/
using SuperFramework.SuperNLogger.Asynchronous;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

namespace SuperFramework.SuperNLogger
{
    /// <summary>
    /// ��־��
    /// </summary>
    public class NLogger
    {
        /// <summary>
        /// Config��־�������ýڹؼ��� �������ݣ�DEBUG|INFO|WARN|ERROR|FATAL ��Ĭ��DEBUG��
        /// </summary>
        private const string _LOG_LEVEL_ = "_LOG_LEVEL_";
        /// <summary>
        /// �Ƿ�����־���������� ��Ĭ��True��
        /// </summary>
        private const string _LOG_LEVELGROUP_ = "_LOG_LEVELGROUP_";
        /// <summary>
        /// Config��־�ļ����ýڹؼ��� �������ݣ���־�ļ�·��+�ļ����������ļ���չ������Ĭ��Logger��
        /// </summary>
        private const string _LOG_FILE_ = "_LOG_FILE_";
        /// <summary>
        /// Config��־�ļ����ýڹؼ��֣���־�ļ�����󳤶ȣ���λ���ֽڣ�������С��1024�ֽڣ���1K��
        /// ��Ĭ��1M = 1*1024*1024=1048576��
        /// </summary>
        private const string _LOG_FILE_MAX_SIZE_ = "_LOG_FILE_MAX_SIZE_";
        /// <summary>
        /// Config��־�ļ��Ƿ������ռ�ֿ��洢��ʾ�����ýڹؼ��� : TRUE|FALSE ��Ĭ��FALSE��
        /// </summary>
        private const string _LOG_SPLIT_ = "_LOG_SPLIT_";
        /// <summary>
        /// ����־�ļ��������ռ�ֿ��洢ʱ����_LOG_SPLIT_=TRUE���������ռ��ȡ����󳤶�(��ֵ�������0������Ч)��(��ѡ)Ĭ�������ռ�ȫ��
        /// </summary>
        private const string _LOG_NS_MAXLENGTH_ = "_LOG_NS_MAXLENGTH_";
        /// <summary>
        /// ����־�ļ��������ռ�ֿ��洢ʱ����_LOG_SPLIT_=TRUE���������ռ䳤������ȡ���ڼ��ֶΣ��ԡ�.���ָ(��ֵ�������0������Ч)��(��ѡ)Ĭ�������ռ�ȫ��
        /// </summary>
        private const string _LOG_NS_DIV_COUNT_ = "_LOG_NS_DIV_COUNT_";
        /// <summary>
        /// ��־ �첽д���ʾ TRUE�������߳��첽д�룬FALSE��ֱ��д����־�ļ� ��Ĭ��FALSE��
        /// </summary>
        private const string _LOG_ASYN_ = "_LOG_ASYN_";
        /// <summary>
        /// ��־ �첽д��ʱ������־����Ϊ�յĵȴ���������������ڵ���0��Ϊ0��û�еȴ�ʱ��(�Ϻ���Դ)���ɲ����ã�Ĭ��100���룩
        /// </summary>
        private const string _LOG_ASYN_WAIT_ = "_LOG_ASYN_WAIT_";
        /// <summary>
        /// ��־ �첽д���߳���ǰ̨�̻߳��Ǻ�̨�߳� TRUE��̨�̣߳�FALSEǰ̨�̣߳�Ĭ��FALSE��
        /// </summary>
        private const string _LOG_ASYN_THREAD_BACKGROUND_ = "_LOG_ASYN_THREAD_BACKGROUND_";
        /// <summary>
        /// #��־�ļ��ı���ʱ�䣨��λ���죬����-Ĭ��30�죩��ֵ�����㷽��Ч��
        /// </summary>
        private const string _LOG_FILE_SAVE_TIME_ = "_LOG_FILE_SAVE_TIME_";

        /// <summary>
        /// ��־�ļ�Ĭ��·���������� "_LOG_ENV_PATH_FILE_"
        /// ������ý�_LOG_FILE_δ���ã���ȡ��ֵ������û�������ֵҲδ������ȡ%TEMP%��Ӧ��ֵ�������ֵҲû����ȡ��ǰ���г�������Ŀ¼
        /// </summary>
        private const string _LOG_DEFAULT_ENVIRONMENT_ROOT = "_LOG_ENV_PATH_FILE_";
        /// <summary>
        /// NLogger��ʼ�������ļ�����·����ͨ��ϵͳ������������ָ���ķ�ʽ�� "_LOG_ENV_CONFIG_FILE_"
        /// </summary>
        private const string _LOG_DEFAULT_ENVIRONMENT_LOG_CONFIG = "_LOG_ENV_CONFIG_FILE_";
        /// <summary>
        /// ��־�ļ�Ĭ���ļ���Logger�������û���������Ը���Ϊ��־�ļ�����ǰ׺��
        /// </summary>
        private const string _LOG_DEFAULT_FILE_NAME_ = "NLogger";

        /// <summary>
        /// ��ǰ��־��������
        /// </summary>
        private static LogLevel _cur_log_level = LogLevel.ERROR; //Ĭ����־���� //ԭdebug
        /// <summary>
        /// �Ƿ�����־�ȼ����з������
        /// </summary>
        private static bool _log_levelgroup_ = false;
        /// <summary>
        /// ��־�ļ�·��+�ļ����������ļ���չ����
        /// </summary>
        private static string _cur_log_file_ = _LOG_DEFAULT_FILE_NAME_;
        /// <summary>
        /// ��־�ļ��Ƿ������ռ�ֿ���ʾ��Ĭ��false��
        /// false:���ֿ���ȫ����¼��һ���ļ���true:�ֿ����������ռ�ֿ���¼�������ļ���
        /// </summary>
        private static bool _cur_log_split_ = false;
        /// <summary>
        /// ����־�ļ��������ռ�ֿ��洢ʱ�������ռ��ȡ����󳤶�(��ֵ�������0������Ч)
        /// </summary>
        private static int _log_ns_maxlength_ = -1;
        /// <summary>
        /// ����־�ļ��������ռ�ֿ��洢ʱ�������ռ䳤������ȡ���ڼ��ֶΣ��ԡ�.���ָ(ֵ�������0������Ч)
        /// </summary>
        private static int _log_ns_div_count_ = -1;
        /// <summary>
        /// ��־ �첽д���ʾ��Ĭ��false�� trueʹ���첽����д��־��false��ʹ���첽����д��־
        /// </summary>
        private static bool _log_asyn_ = false;
        /// <summary>
        /// �첽д��ʱ������־����Ϊ�յĵȴ�������
        /// ��������ڵ���0��Ϊ0��û�еȴ�ʱ��(�Ϻ���Դ)���ɲ����ã�Ĭ��100���룩��_LOG_ASYN_��������TRUE������������ã�
        /// </summary>
        private static int _log_thread_wait_ = 100;
        /// <summary>
        /// �첽д���߳���ǰ̨�̻߳��Ǻ�̨�̣߳���ӦThread.IsBackground���ԣ�
        /// TRUE��̨�̣߳�FALSEǰ̨�̣߳�Ĭ��FALSE����_LOG_ASYN_��������TRUE������������ã�
        /// </summary>
        private static bool _thread_is_backgroud_ = false;
        /// <summary>
        /// #��־�ļ��ı���ʱ�䣨��λ���죬����-Ĭ��30�죩��ֵ�����㷽��Ч��
        /// </summary>
        private static int _log_file_save_time = 30;

        /// <summary>
        /// ��־�ļ���������
        /// </summary>
        private FileOperate filOperate = null;

        /// <summary>
        /// ˽����־�����״ι���ʱ��ʼ��
        /// </summary>
        private static NLogger _logger = null;
        /// <summary>
        /// ���߳� ������
        /// </summary>
        private static readonly object _syncRoot = new();

        /// <summary>
        /// ��־�ļ�����󳤶ȣ���λ���ֽڣ�������С��1024�ֽڣ���1K��
        /// </summary>
        internal long LogFileMaxSize { get; private set; } = 1048576;
        /// <summary>
        /// �첽д��ʱ������־����Ϊ�յĵȴ�������
        /// ��������ڵ���0��Ϊ0��û�еȴ�ʱ��(�Ϻ���Դ)���ɲ����ã�Ĭ��100���룩��_LOG_ASYN_��������TRUE������������ã�
        /// </summary>
        internal int LOG_THREAD_WAIT
        {
            get { return _log_thread_wait_; }
        }
        /// <summary>
        /// �첽д���߳���ǰ̨�̻߳��Ǻ�̨�̣߳���ӦThread.IsBackground���ԣ�
        /// TRUE��̨�̣߳�FALSEǰ̨�̣߳�Ĭ��FALSE����_LOG_ASYN_��������TRUE������������ã�
        /// </summary>
        internal bool THREAD_IS_BACKGROUD
        {
            get { return _thread_is_backgroud_; }
        }
        /// <summary>
        /// Logger˽�й��캯���������ʵ��
        /// </summary>
        private NLogger()
        {
            // Environment.GetEnvironmentVariable(_LOG_DEFAULT_ENVIRONMENT_LOG_CONFIG, EnvironmentVariableTarget.Machine);
            string strEnvConfig = Environment.GetEnvironmentVariable(_LOG_DEFAULT_ENVIRONMENT_LOG_CONFIG);
            if (!string.IsNullOrEmpty(strEnvConfig) && System.IO.File.Exists(strEnvConfig))
            {
                InitByPropertiesCfgFile(strEnvConfig);
            }
            else
            {
                InitByConfiguration();
            }
            if (_log_file_save_time > 0)
            {
                Thread logFileStorageThread = new(MonitorLogFileStorage)
                {
                    IsBackground = THREAD_IS_BACKGROUD
                };
                logFileStorageThread.Start();
            }
        }
        /// <summary>
        /// ͨ��ϵͳ��������ָ��Properties�����ļ���ʼ��Loggerʵ��
        /// </summary>
        /// <param name="strEnvConfig">ָ��Properties�����ļ�</param>
        private void InitByPropertiesCfgFile(string strEnvConfig)
        {
            LogProperties hashPro = new();
            hashPro.Load(strEnvConfig);

            _cur_log_level = LogLevelHelper.StrToLogLevel(hashPro.GetProperty(_LOG_LEVEL_));

            string logFile = hashPro.GetProperty(_LOG_FILE_);
            if (!string.IsNullOrEmpty(logFile))
            {
                _cur_log_file_ = logFile;
                if (_cur_log_file_.EndsWith("\\") || _cur_log_file_.EndsWith("/"))
                {
                    _cur_log_file_ += _LOG_DEFAULT_FILE_NAME_;
                }
            }
            else
            {
                string strLogPath = Environment.GetEnvironmentVariable(_LOG_DEFAULT_ENVIRONMENT_ROOT);
                if (string.IsNullOrEmpty(strLogPath))
                {
                    string strTemp = Environment.GetEnvironmentVariable("TEMP");
                    if (string.IsNullOrEmpty(strTemp))
                    {
                        if (!strTemp.EndsWith("\\") && !strTemp.EndsWith("/"))
                        {
                            strTemp += "\\";
                        }
                        strLogPath = strTemp;
                    }
                }
                if (_cur_log_file_.EndsWith("\\") || _cur_log_file_.EndsWith("/"))
                {
                    strLogPath += _LOG_DEFAULT_FILE_NAME_;
                }
                if (string.IsNullOrEmpty(strLogPath)) strLogPath = _LOG_DEFAULT_FILE_NAME_;
                _cur_log_file_ = strLogPath;
            }

            string logFileMaxSize = hashPro.GetProperty(_LOG_FILE_MAX_SIZE_);
            if (!string.IsNullOrEmpty(logFileMaxSize))
            {
                long maxSize;
                long.TryParse(logFileMaxSize, out maxSize);
                if (maxSize >= 1024)
                {
                    LogFileMaxSize = maxSize;
                }
            }

            string logSplit = hashPro.GetProperty(_LOG_SPLIT_);
            if (!string.IsNullOrEmpty(logSplit) && logSplit.ToUpper().Equals("TRUE"))
            {
                _cur_log_split_ = true;
            }

            if (_cur_log_split_)
            {
                string strLength = hashPro.GetProperty(_LOG_NS_MAXLENGTH_);
                if (!string.IsNullOrEmpty(strLength))
                {
                    int.TryParse(strLength, out _log_ns_maxlength_);
                }

                string strCount = hashPro.GetProperty(_LOG_NS_DIV_COUNT_);
                if (!string.IsNullOrEmpty(strCount))
                {
                    int.TryParse(strCount, out _log_ns_div_count_);
                }
            }

            string logAsyn = hashPro.GetProperty(_LOG_ASYN_);
            if (!string.IsNullOrEmpty(logAsyn) && logAsyn.ToUpper().Equals("TRUE"))
            {
                _log_asyn_ = true;

                string strWait = hashPro.GetProperty(_LOG_ASYN_WAIT_);
                int.TryParse(strWait, out _log_thread_wait_);

                string strThreadBackgroud = hashPro.GetProperty(_LOG_ASYN_THREAD_BACKGROUND_);
                if (!string.IsNullOrEmpty(strThreadBackgroud) && strThreadBackgroud.ToUpper().Equals("TRUE"))
                {
                    _thread_is_backgroud_ = true;
                }
                LogThread.RunAsynchronous(_log_thread_wait_, _thread_is_backgroud_);
            }

            string logTime = hashPro.GetProperty(_LOG_FILE_SAVE_TIME_);
            if (!string.IsNullOrEmpty(logTime))
            {
                if (!int.TryParse(logTime, out _log_file_save_time))
                {
                    _log_file_save_time = 30;
                }
            }

            filOperate = new FileOperate(_cur_log_file_);
        }
        /// <summary>
        /// ͨ��web.config��App.config���ýڳ�ʼ��Loggerʵ��
        /// </summary>
        private void InitByConfiguration()
        {
            string logLevel = LogUtil.GetConfig(_LOG_LEVEL_);
            _cur_log_level = LogLevelHelper.StrToLogLevel(logLevel);

            string logFile = LogUtil.GetConfig(_LOG_FILE_);
            if (!string.IsNullOrEmpty(logFile))
            {
                _cur_log_file_ = logFile;
                if (_cur_log_file_.EndsWith("\\") || _cur_log_file_.EndsWith("/"))
                {
                    _cur_log_file_ += _LOG_DEFAULT_FILE_NAME_;
                }
            }
            else
            {
                string strLogPath;
                strLogPath = Environment.GetEnvironmentVariable(_LOG_DEFAULT_ENVIRONMENT_ROOT);
                if (string.IsNullOrEmpty(strLogPath))
                {
                    string strTemp = Environment.GetEnvironmentVariable("TEMP");
                    if (string.IsNullOrEmpty(strTemp))
                    {
                        if (!strTemp.EndsWith("\\") && !strTemp.EndsWith("/"))
                        {
                            strTemp += "\\";
                        }
                        strLogPath = strTemp;
                    }
                }
                if (_cur_log_file_.EndsWith("\\") || _cur_log_file_.EndsWith("/"))
                {
                    strLogPath += _LOG_DEFAULT_FILE_NAME_;
                }
                if (string.IsNullOrEmpty(strLogPath)) strLogPath = _LOG_DEFAULT_FILE_NAME_;
                _cur_log_file_ = strLogPath;
            }

            string logFileMaxSize = LogUtil.GetConfig(_LOG_FILE_MAX_SIZE_);
            if (!string.IsNullOrEmpty(logFileMaxSize))
            {
                long maxSize;
                long.TryParse(logFileMaxSize, out maxSize);
                if (maxSize >= 1024)
                {
                    LogFileMaxSize = maxSize;
                }
            }

            string logSplie = LogUtil.GetConfig(_LOG_SPLIT_);
            if (!string.IsNullOrEmpty(logSplie) && logSplie.ToUpper().Equals("TRUE"))
            {
                _cur_log_split_ = true;
            }

            if (_cur_log_split_)
            {
                string strLength = LogUtil.GetConfig(_LOG_NS_MAXLENGTH_);
                if (!string.IsNullOrEmpty(strLength))
                {
                    int.TryParse(strLength, out _log_ns_maxlength_);
                }

                string strCount = LogUtil.GetConfig(_LOG_NS_DIV_COUNT_);
                if (!string.IsNullOrEmpty(strCount))
                {
                    int.TryParse(strCount, out _log_ns_div_count_);
                }
            }

            string logAsyn = LogUtil.GetConfig(_LOG_ASYN_);
            if (!string.IsNullOrEmpty(logAsyn) && logAsyn.ToUpper().Equals("TRUE"))
            {
                _log_asyn_ = true;

                string strWait = LogUtil.GetConfig(_LOG_ASYN_WAIT_);
                int.TryParse(strWait, out _log_thread_wait_);

                string strThreadBackgroud = LogUtil.GetConfig(_LOG_ASYN_THREAD_BACKGROUND_);
                if (!string.IsNullOrEmpty(strThreadBackgroud) && strThreadBackgroud.ToUpper().Equals("TRUE"))
                {
                    _thread_is_backgroud_ = true;
                }
                LogThread.RunAsynchronous(_log_thread_wait_, _thread_is_backgroud_);
            }

            string logTime = LogUtil.GetConfig(_LOG_FILE_SAVE_TIME_);
            if (!string.IsNullOrEmpty(logTime))
            {
                if (!int.TryParse(logTime, out _log_file_save_time))
                {
                    _log_file_save_time = 30;
                }
            }
            _log_levelgroup_ = bool.Parse(LogUtil.GetConfig(_LOG_LEVELGROUP_));
            filOperate = new FileOperate(_cur_log_file_);
        }

        /// <summary>
        /// ��ȡ��ǰ��־ʵ������(��ʵ��)
        /// </summary>
        /// <returns></returns>
        public static NLogger GetInstance(string configPath= "ConfigInfo\\LogsConfig.xml")
        {
            if (_logger == null)
            {
                lock (_syncRoot)
                {
                    if (_logger == null)
                    {
                        LogUtil.filePath = configPath;
                        _logger = new NLogger();//���̵߳�����˫������������ʽ������
                    }
                }
            }
            return _logger;
        }

        /// <summary>
        /// �ı䵱ǰ��־�����������й����иı䣬���Ƽ�ʹ�ã���������ʱ�̲��ã���ֵ��ı����ýڵ�Ĭ����Ϊ��
        /// </summary>
        /// <param name="value">��־����DEBUG|INFO|WARN|ERROR|FATAL��</param>
        [Obsolete("���Ƽ�ʹ�ã���������ʱ�̲��ã���ֵ��ı����ýڵ�Ĭ����Ϊ")]
        public static void SetLogLevel(LogLevel value)
        {
            _cur_log_level = value;
        }

        /// <summary>
        /// ��ȡ��ǰ��־����
        /// </summary>
        public static LogLevel GetLogLevel()
        {
            return _cur_log_level;
        }

        /// <summary>
        /// ��¼Debug������־
        /// </summary>
        /// <param name="msg">��־����</param>
        public void Debug(string msg)
        {
            if (LogLevel.DEBUG >= _cur_log_level)
            {
                Log(LogLevel.DEBUG, msg, _log_levelgroup_);
            }
        }
        /// <summary>
        /// ��¼Debug������־
        /// </summary>
        /// <param name="obj">��־����</param>
        public void Debug(object obj)
        {
            if (LogLevel.DEBUG >= _cur_log_level)
            {
                Log(LogLevel.DEBUG, obj + "", _log_levelgroup_);
            }
        }
        /// <summary>
        /// ��¼Debug������־
        /// </summary>
        /// <param name="msg">��־����</param>
        /// <param name="obj">��־����</param>
        public void Debug(string msg, object obj)
        {
            if (LogLevel.DEBUG >= _cur_log_level)
            {
                Log(LogLevel.DEBUG, msg + " " + obj, _log_levelgroup_);
            }
        }

        /// <summary>
        /// ��¼Info������־
        /// </summary>
        /// <param name="msg">��־����</param>
        public void Info(string msg)
        {
            if (LogLevel.INFO >= _cur_log_level)
            {
                Log(LogLevel.INFO, msg, _log_levelgroup_);
            }
        }
        /// <summary>
        /// ��¼Info������־
        /// </summary>
        /// <param name="obj">��־����</param>
        public void Info(object obj)
        {
            if (LogLevel.INFO >= _cur_log_level)
            {
                Log(LogLevel.INFO, obj + "", _log_levelgroup_);
            }
        }
        /// <summary>
        /// ��¼Info������־
        /// </summary>
        /// <param name="msg">��־����</param>
        /// <param name="obj">��־����</param>
        public void Info(string msg, object obj)
        {
            if (LogLevel.INFO >= _cur_log_level)
            {
                Log(LogLevel.INFO, msg + " " + obj, _log_levelgroup_);
            }
        }

        /// <summary>
        /// ��¼Warn������־
        /// </summary>
        /// <param name="msg">��־����</param>
        public void Warn(string msg)
        {
            if (LogLevel.WARN >= _cur_log_level)
            {
                Log(LogLevel.WARN, msg, _log_levelgroup_);
            }
        }
        /// <summary>
        /// ��¼Warn������־
        /// </summary>
        /// <param name="obj">��־����</param>
        public void Warn(object obj)
        {
            if (LogLevel.WARN >= _cur_log_level)
            {
                Log(LogLevel.WARN, obj + "", _log_levelgroup_);
            }
        }
        /// <summary>
        /// ��¼Warn������־
        /// </summary>
        /// <param name="msg">��־����</param>
        /// <param name="obj">��־����</param>
        public void Warn(string msg, object obj)
        {
            if (LogLevel.WARN >= _cur_log_level)
            {
                Log(LogLevel.WARN, msg + " " + obj, _log_levelgroup_);
            }
        }

        /// <summary>
        /// ��¼Error������־
        /// </summary>
        /// <param name="msg">��־����</param>
        public void Error(string msg)
        {
            if (LogLevel.ERROR >= _cur_log_level)
            {
                Log(LogLevel.ERROR, msg, _log_levelgroup_);
            }
        }
        /// <summary>
        /// ��¼Error������־
        /// </summary>
        /// <param name="obj">��־����</param>
        public void Error(object obj)
        {
            if (LogLevel.ERROR >= _cur_log_level)
            {
                Log(LogLevel.ERROR, obj + "", _log_levelgroup_);
            }
        }
        /// <summary>
        /// ��¼Error������־
        /// </summary>
        /// <param name="msg">��־����</param>
        /// <param name="obj">��־����</param>
        public void Error(string msg, object obj)
        {
            if (LogLevel.ERROR >= _cur_log_level)
            {
                Log(LogLevel.ERROR, msg + " " + obj, _log_levelgroup_);
            }
        }

        /// <summary>
        /// ��¼Fatal������־
        /// </summary>
        /// <param name="msg">��־����</param>
        public void Fatal(string msg)
        {
            Log(LogLevel.FATAL, msg, _log_levelgroup_);//�������󼶱���־��ʼ�մ�ӡ
        }
        /// <summary>
        /// ��¼Fatal������־
        /// </summary>
        /// <param name="obj">��־����</param>
        public void Fatal(object obj)
        {
            Log(LogLevel.FATAL, obj + "", _log_levelgroup_);//�������󼶱���־��ʼ�մ�ӡ
        }
        /// <summary>
        /// ��¼Fatal������־
        /// </summary>
        /// <param name="msg">��־����</param>
        /// <param name="obj">��־����</param>
        public void Fatal(string msg, object obj)
        {
            Log(LogLevel.FATAL, msg + " " + obj, _log_levelgroup_);//�������󼶱���־��ʼ�մ�ӡ
        }


        /// <summary>
        /// ��־ǰ׺
        /// </summary>
        /// <param name="level">��־����</param>
        /// <returns>��־ǰ׺</returns>
        private string LogInfoPrefix(LogLevel level)
        {
            StackTrace st = new(true);
            StackFrame sf = null; // st.GetFrame(2);
            //st.FrameCount
            for (int i = 1; i <= 3; i++)
            {
                sf = st.GetFrame(i);
                string ns = ((MemberInfo)(sf.GetMethod())).DeclaringType.Namespace;
                //if (ns != null && ns != "AHCT.Log")
                if (ns != "SuperFramework.SuperNLogger")
                {
                    break;
                }
            }
            //ʱ�� ��־����[DEBUG|INFO|ERROR|FATAL] �����ռ�:������::�ļ���:�к�:�к�
            //string str = LogLevel.DEBUG.ToString();//=DEBUG
            //Console.WriteLine(LogLevel.DEBUG < LogLevel.FATAL);//True
            //String prefix = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff") + " [" + level.ToString() + "] "
            //    + ((MemberInfo)(sf.GetMethod())).DeclaringType.FullName + ":" + sf.GetMethod().Name + "::" //+ sf.GetFileName() + ":" 
            //    + sf.GetFileLineNumber() + ":" + sf.GetFileColumnNumber();

            string prefix = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff") + " [" + level.ToString() + "] " 
                + sf.GetMethod().DeclaringType.FullName + ":" + sf.GetMethod().ToString() + "::"/* + sf.GetFileName() + ":" */
                + CodeHelper.GetLineNum(2, 2) + "." + CodeHelper.GetColumnNumber(2, 2);
            return prefix;

            //return CommonHelp.GetLogPrefix(3, level.ToString());
        }

        /// <summary>
        /// ��־�ļ��ֿ���¼ʱ���ļ�����׺������־�ļ���չ����
        /// </summary>
        /// <returns></returns>
        private string LogFileSuffix()
        {
            string ns = "";// CommonHelp.GetLogNamespace(3);

            StackTrace st = new();
            //st.FrameCount
            for (int i = 1; i <= 3; i++)
            {
                StackFrame sf = st.GetFrame(i);
                ns = ((MemberInfo)(sf.GetMethod())).DeclaringType.Namespace;
                //if (ns != null && !ns.StartsWith("AHCT.Log"))
                if (ns != "SuperFramework.SuperNLogger")
                {
                    break;
                }
            }

            if (!string.IsNullOrEmpty(ns))
            {
                if (_log_ns_maxlength_ > 0 && _log_ns_maxlength_ < ns.Length)
                {
                    ns = ns.Substring(0, _log_ns_maxlength_);
                }

                if (_log_ns_div_count_ > 0)
                {
                    int index = 0;
                    for (int i = 1; i <= _log_ns_div_count_; i++)
                    {
                        index = ns.IndexOf(".", index + 1);
                        if (index == -1)
                        {//index=-1��˵��û�С�.���Ż���_log_ns_div_count_ֵ�Ѿ����������ָ�������ʱ�Ѳ���Ҫ��ȡ��
                            break;
                        }
                    }
                    //index != 0 && index != -1
                    if (index > 0) ns = ns.Substring(0, index);
                }
            }

            return "_" + ns;
        }

        /// <summary>
        /// ��¼���ݵ���־�ļ�
        /// </summary>
        /// <param name="level">��־����</param>
        /// <param name="contents">����¼����־����</param>
        /// <param name="isLevelGroup">�Ƿ񰴵ȼ�����</param>
        private void Log(LogLevel level, string contents, bool isLevelGroup = true)
        {
            try
            {
                string strLogContents = LogInfoPrefix(level) + " " + contents;
                string logFileSuffix = string.Empty;
                if (_cur_log_split_)
                    logFileSuffix = LogFileSuffix()+ DateTime.Now.ToString("_yyyyMMdd");
                else
                    logFileSuffix = DateTime.Now.ToString("_yyyyMMdd");
                if (isLevelGroup)
                    logFileSuffix = "_" + level.ToString() + logFileSuffix;
                if (level == LogLevel.FATAL)
                    filOperate.AppendFileImmediately(logFileSuffix, strLogContents);
                else
                    filOperate.AppendFile(logFileSuffix, strLogContents, _log_asyn_);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// �����־�ļ��Ĵ洢����ѹ����ɾ������ָ��ʱ�����־�ļ�����Ĭ��30�죩
        /// </summary>
        private void MonitorLogFileStorage()
        {
            while (true)
            {
                FileInfo fi = new(_cur_log_file_);
                if (!fi.Directory.Exists) fi.Directory.Create();
                FileInfo[] arrFiles = fi.Directory.GetFiles();
                foreach (FileInfo file in arrFiles)
                {
                    //��ѹ��
                    if (file.Name.EndsWith(FileOperate.log_extension) && file.Length > LogFileMaxSize)
                    {
                        //string zipFile = file.FullName.Remove(file.FullName.LastIndexOf(file.Extension)) + ".zip";
                        string zipFile = file.FullName + ".zip";
                        LogUtil.CreateZipFile(file.FullName, zipFile);
                    }
                    //�ٽ����ڵ��ļ�ɾ��
                    if (file.LastWriteTime < DateTime.Now.AddDays(_log_file_save_time * -1))
                    {
                        if (file.Name.EndsWith(".zip") || (file.Name.EndsWith(FileOperate.log_extension) && file.Length > LogFileMaxSize)) // >=
                        {
                            try
                            {
                                file.Delete();//ɾ������ָ��ʱ��Ĳ�����ָ���洢��������־�ļ�
                            }
                            catch { }
                        }
                    }
                }
                Thread.Sleep(1800000);//1000*60*60=3600000��һ��Сʱ)����ѯ���ڣ�
            }
        }

    }

}
