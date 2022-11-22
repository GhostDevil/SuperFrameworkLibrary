/******************************************
 * 
 * 模块名称：日志类
 * 当前版本：1.0
 * 开发人员：不良帥
 * 开发时间：2011-11-14
 * 版本历史：
 * 
 * 2016-03-13 Fatal级别日志执行立即写入，立即写入失败的日志则记入操作系统事件日志；
 *            对于同步写入日志，若一次写入失败的日志则执行二次写入，二次写入执行立即写入；
 *            对于异步写入日志，若一次写入失败的日志则执行二次写入，二次写入执行加入队尾排队写入
 * 
 * Copyright (C) 不良帥 2011-2015 
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
    /// 日志类
    /// </summary>
    public class NLogger
    {
        /// <summary>
        /// Config日志级别配置节关键字 配置内容：DEBUG|INFO|WARN|ERROR|FATAL （默认DEBUG）
        /// </summary>
        private const string _LOG_LEVEL_ = "_LOG_LEVEL_";
        /// <summary>
        /// 是否按照日志级别分类输出 （默认True）
        /// </summary>
        private const string _LOG_LEVELGROUP_ = "_LOG_LEVELGROUP_";
        /// <summary>
        /// Config日志文件配置节关键字 配置内容：日志文件路径+文件名（不含文件扩展名）（默认Logger）
        /// </summary>
        private const string _LOG_FILE_ = "_LOG_FILE_";
        /// <summary>
        /// Config日志文件配置节关键字，日志文件的最大长度（单位：字节）（不得小于1024字节，即1K）
        /// （默认1M = 1*1024*1024=1048576）
        /// </summary>
        private const string _LOG_FILE_MAX_SIZE_ = "_LOG_FILE_MAX_SIZE_";
        /// <summary>
        /// Config日志文件是否按命名空间分开存储标示的配置节关键字 : TRUE|FALSE （默认FALSE）
        /// </summary>
        private const string _LOG_SPLIT_ = "_LOG_SPLIT_";
        /// <summary>
        /// 当日志文件按命名空间分开存储时（即_LOG_SPLIT_=TRUE），命名空间截取的最大长度(该值必须大于0，方有效)，(可选)默认命名空间全称
        /// </summary>
        private const string _LOG_NS_MAXLENGTH_ = "_LOG_NS_MAXLENGTH_";
        /// <summary>
        /// 当日志文件按命名空间分开存储时（即_LOG_SPLIT_=TRUE），命名空间长度最大截取到第几分段（以“.”分割）(该值必须大于0，方有效)，(可选)默认命名空间全称
        /// </summary>
        private const string _LOG_NS_DIV_COUNT_ = "_LOG_NS_DIV_COUNT_";
        /// <summary>
        /// 日志 异步写入标示 TRUE：新启线程异步写入，FALSE：直接写入日志文件 （默认FALSE）
        /// </summary>
        private const string _LOG_ASYN_ = "_LOG_ASYN_";
        /// <summary>
        /// 日志 异步写入时，当日志队列为空的等待毫秒数（必须大于等于0，为0则没有等待时间(较耗资源)；可不配置，默认100毫秒）
        /// </summary>
        private const string _LOG_ASYN_WAIT_ = "_LOG_ASYN_WAIT_";
        /// <summary>
        /// 日志 异步写入线程是前台线程还是后台线程 TRUE后台线程，FALSE前台线程（默认FALSE）
        /// </summary>
        private const string _LOG_ASYN_THREAD_BACKGROUND_ = "_LOG_ASYN_THREAD_BACKGROUND_";
        /// <summary>
        /// #日志文件的保存时间（单位：天，整数-默认30天）（值大于零方有效）
        /// </summary>
        private const string _LOG_FILE_SAVE_TIME_ = "_LOG_FILE_SAVE_TIME_";

        /// <summary>
        /// 日志文件默认路径环境变量 "_LOG_ENV_PATH_FILE_"
        /// 如果配置节_LOG_FILE_未配置，则取该值；如果该环境变量值也未配置则取%TEMP%对应的值；如果该值也没有则取当前运行程序所在目录
        /// </summary>
        private const string _LOG_DEFAULT_ENVIRONMENT_ROOT = "_LOG_ENV_PATH_FILE_";
        /// <summary>
        /// NLogger初始化配置文件所在路径（通过系统环境环境变量指定的方式） "_LOG_ENV_CONFIG_FILE_"
        /// </summary>
        private const string _LOG_DEFAULT_ENVIRONMENT_LOG_CONFIG = "_LOG_ENV_CONFIG_FILE_";
        /// <summary>
        /// 日志文件默认文件（Logger）（如果没有配置则以该名为日志文件名的前缀）
        /// </summary>
        private const string _LOG_DEFAULT_FILE_NAME_ = "NLogger";

        /// <summary>
        /// 当前日志级别设置
        /// </summary>
        private static LogLevel _cur_log_level = LogLevel.ERROR; //默认日志级别 //原debug
        /// <summary>
        /// 是否按照日志等级进行分类输出
        /// </summary>
        private static bool _log_levelgroup_ = false;
        /// <summary>
        /// 日志文件路径+文件名（不含文件扩展名）
        /// </summary>
        private static string _cur_log_file_ = _LOG_DEFAULT_FILE_NAME_;
        /// <summary>
        /// 日志文件是否按命名空间分开标示，默认false。
        /// false:不分开，全部记录到一个文件；true:分开，按命名空间分开记录到各个文件。
        /// </summary>
        private static bool _cur_log_split_ = false;
        /// <summary>
        /// 当日志文件按命名空间分开存储时，命名空间截取的最大长度(该值必须大于0，方有效)
        /// </summary>
        private static int _log_ns_maxlength_ = -1;
        /// <summary>
        /// 当日志文件按命名空间分开存储时，命名空间长度最大截取到第几分段（以“.”分割）(值必须大于0，方有效)
        /// </summary>
        private static int _log_ns_div_count_ = -1;
        /// <summary>
        /// 日志 异步写入标示（默认false） true使用异步方法写日志；false不使用异步方法写日志
        /// </summary>
        private static bool _log_asyn_ = false;
        /// <summary>
        /// 异步写入时，当日志队列为空的等待毫秒数
        /// （必须大于等于0，为0则没有等待时间(较耗资源)；可不配置，默认100毫秒）（_LOG_ASYN_必须配置TRUE，该项才起作用）
        /// </summary>
        private static int _log_thread_wait_ = 100;
        /// <summary>
        /// 异步写入线程是前台线程还是后台线程（对应Thread.IsBackground属性）
        /// TRUE后台线程，FALSE前台线程（默认FALSE）（_LOG_ASYN_必须配置TRUE，该项才起作用）
        /// </summary>
        private static bool _thread_is_backgroud_ = false;
        /// <summary>
        /// #日志文件的保存时间（单位：天，整数-默认30天）（值大于零方有效）
        /// </summary>
        private static int _log_file_save_time = 30;

        /// <summary>
        /// 日志文件操作对象
        /// </summary>
        private FileOperate filOperate = null;

        /// <summary>
        /// 私有日志对象，首次构造时初始化
        /// </summary>
        private static NLogger _logger = null;
        /// <summary>
        /// 多线程 锁对象
        /// </summary>
        private static readonly object _syncRoot = new();

        /// <summary>
        /// 日志文件的最大长度（单位：字节）（不得小于1024字节，即1K）
        /// </summary>
        internal long LogFileMaxSize { get; private set; } = 1048576;
        /// <summary>
        /// 异步写入时，当日志队列为空的等待毫秒数
        /// （必须大于等于0，为0则没有等待时间(较耗资源)；可不配置，默认100毫秒）（_LOG_ASYN_必须配置TRUE，该项才起作用）
        /// </summary>
        internal int LOG_THREAD_WAIT
        {
            get { return _log_thread_wait_; }
        }
        /// <summary>
        /// 异步写入线程是前台线程还是后台线程（对应Thread.IsBackground属性）
        /// TRUE后台线程，FALSE前台线程（默认FALSE）（_LOG_ASYN_必须配置TRUE，该项才起作用）
        /// </summary>
        internal bool THREAD_IS_BACKGROUD
        {
            get { return _thread_is_backgroud_; }
        }
        /// <summary>
        /// Logger私有构造函数，避免多实例
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
        /// 通过系统环境变量指定Properties配置文件初始化Logger实例
        /// </summary>
        /// <param name="strEnvConfig">指定Properties配置文件</param>
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
        /// 通过web.config或App.config配置节初始化Logger实例
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
        /// 获取当前日志实例对象(单实例)
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
                        _logger = new NLogger();//多线程单例：双重锁定；懒汉式单例类
                    }
                }
            }
            return _logger;
        }

        /// <summary>
        /// 改变当前日志级别（用于运行过程中改变，不推荐使用，仅供特殊时刻采用，该值会改变配置节的默认行为）
        /// </summary>
        /// <param name="value">日志级别（DEBUG|INFO|WARN|ERROR|FATAL）</param>
        [Obsolete("不推荐使用，仅供特殊时刻采用，该值会改变配置节的默认行为")]
        public static void SetLogLevel(LogLevel value)
        {
            _cur_log_level = value;
        }

        /// <summary>
        /// 获取当前日志级别
        /// </summary>
        public static LogLevel GetLogLevel()
        {
            return _cur_log_level;
        }

        /// <summary>
        /// 记录Debug级别日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        public void Debug(string msg)
        {
            if (LogLevel.DEBUG >= _cur_log_level)
            {
                Log(LogLevel.DEBUG, msg, _log_levelgroup_);
            }
        }
        /// <summary>
        /// 记录Debug级别日志
        /// </summary>
        /// <param name="obj">日志对象</param>
        public void Debug(object obj)
        {
            if (LogLevel.DEBUG >= _cur_log_level)
            {
                Log(LogLevel.DEBUG, obj + "", _log_levelgroup_);
            }
        }
        /// <summary>
        /// 记录Debug级别日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        /// <param name="obj">日志对象</param>
        public void Debug(string msg, object obj)
        {
            if (LogLevel.DEBUG >= _cur_log_level)
            {
                Log(LogLevel.DEBUG, msg + " " + obj, _log_levelgroup_);
            }
        }

        /// <summary>
        /// 记录Info级别日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        public void Info(string msg)
        {
            if (LogLevel.INFO >= _cur_log_level)
            {
                Log(LogLevel.INFO, msg, _log_levelgroup_);
            }
        }
        /// <summary>
        /// 记录Info级别日志
        /// </summary>
        /// <param name="obj">日志对象</param>
        public void Info(object obj)
        {
            if (LogLevel.INFO >= _cur_log_level)
            {
                Log(LogLevel.INFO, obj + "", _log_levelgroup_);
            }
        }
        /// <summary>
        /// 记录Info级别日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        /// <param name="obj">日志对象</param>
        public void Info(string msg, object obj)
        {
            if (LogLevel.INFO >= _cur_log_level)
            {
                Log(LogLevel.INFO, msg + " " + obj, _log_levelgroup_);
            }
        }

        /// <summary>
        /// 记录Warn级别日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        public void Warn(string msg)
        {
            if (LogLevel.WARN >= _cur_log_level)
            {
                Log(LogLevel.WARN, msg, _log_levelgroup_);
            }
        }
        /// <summary>
        /// 记录Warn级别日志
        /// </summary>
        /// <param name="obj">日志对象</param>
        public void Warn(object obj)
        {
            if (LogLevel.WARN >= _cur_log_level)
            {
                Log(LogLevel.WARN, obj + "", _log_levelgroup_);
            }
        }
        /// <summary>
        /// 记录Warn级别日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        /// <param name="obj">日志对象</param>
        public void Warn(string msg, object obj)
        {
            if (LogLevel.WARN >= _cur_log_level)
            {
                Log(LogLevel.WARN, msg + " " + obj, _log_levelgroup_);
            }
        }

        /// <summary>
        /// 记录Error级别日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        public void Error(string msg)
        {
            if (LogLevel.ERROR >= _cur_log_level)
            {
                Log(LogLevel.ERROR, msg, _log_levelgroup_);
            }
        }
        /// <summary>
        /// 记录Error级别日志
        /// </summary>
        /// <param name="obj">日志对象</param>
        public void Error(object obj)
        {
            if (LogLevel.ERROR >= _cur_log_level)
            {
                Log(LogLevel.ERROR, obj + "", _log_levelgroup_);
            }
        }
        /// <summary>
        /// 记录Error级别日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        /// <param name="obj">日志对象</param>
        public void Error(string msg, object obj)
        {
            if (LogLevel.ERROR >= _cur_log_level)
            {
                Log(LogLevel.ERROR, msg + " " + obj, _log_levelgroup_);
            }
        }

        /// <summary>
        /// 记录Fatal级别日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        public void Fatal(string msg)
        {
            Log(LogLevel.FATAL, msg, _log_levelgroup_);//致命错误级别日志，始终打印
        }
        /// <summary>
        /// 记录Fatal级别日志
        /// </summary>
        /// <param name="obj">日志对象</param>
        public void Fatal(object obj)
        {
            Log(LogLevel.FATAL, obj + "", _log_levelgroup_);//致命错误级别日志，始终打印
        }
        /// <summary>
        /// 记录Fatal级别日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        /// <param name="obj">日志对象</param>
        public void Fatal(string msg, object obj)
        {
            Log(LogLevel.FATAL, msg + " " + obj, _log_levelgroup_);//致命错误级别日志，始终打印
        }


        /// <summary>
        /// 日志前缀
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <returns>日志前缀</returns>
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
            //时间 日志级别[DEBUG|INFO|ERROR|FATAL] 命名空间:方法名::文件名:行号:列号
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
        /// 日志文件分开记录时的文件名后缀（非日志文件扩展名）
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
                        {//index=-1，说明没有“.”号或者_log_ns_div_count_值已经超过了最大分隔数，此时已不需要截取了
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
        /// 记录内容到日志文件
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="contents">待记录的日志内容</param>
        /// <param name="isLevelGroup">是否按等级分类</param>
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
        /// 监控日志文件的存储（先压缩再删除超过指定时间的日志文件）（默认30天）
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
                    //先压缩
                    if (file.Name.EndsWith(FileOperate.log_extension) && file.Length > LogFileMaxSize)
                    {
                        //string zipFile = file.FullName.Remove(file.FullName.LastIndexOf(file.Extension)) + ".zip";
                        string zipFile = file.FullName + ".zip";
                        LogUtil.CreateZipFile(file.FullName, zipFile);
                    }
                    //再将过期的文件删除
                    if (file.LastWriteTime < DateTime.Now.AddDays(_log_file_save_time * -1))
                    {
                        if (file.Name.EndsWith(".zip") || (file.Name.EndsWith(FileOperate.log_extension) && file.Length > LogFileMaxSize)) // >=
                        {
                            try
                            {
                                file.Delete();//删除超过指定时间的并超过指定存储容量的日志文件
                            }
                            catch { }
                        }
                    }
                }
                Thread.Sleep(1800000);//1000*60*60=3600000（一个小时)（轮询周期）
            }
        }

    }

}
