using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Layout;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
//指定log4net使用的config文件来读取配置信息
//[assembly: XmlConfigurator(ConfigFile = @"ConfigInfo\Log4Net.config", Watch = true)]
namespace SuperFramework.AppLogs
{
    /// <summary>
    /// 作者：痞子少爷
    /// 说明：Log4Net日志
    /// 日期：2017-05-13
    /// </summary>
    public static class AppLog4Net
    {
        #region 日志类型

        /// <summary>
        /// 日志类型
        /// </summary>
        public enum LogType
        {
            /// <summary>
            /// 普通操作日志：记录系统运行中应该让用户知道的基本信息。例如，服务开始运行，功能已经开户等。
            /// </summary>
            InfoLog,
            /// <summary>
            /// 警告日志：记录系统中不影响系统继续运行，但不符合系统运行正常条件，有可能引起系统错误的信息。例如，记录内容为空，数据内容不正确等。
            /// </summary>
            WarnLog,
            /// <summary>
            /// 调试日志：记录系统用于调试的一切信息，内容或者是一些关键数据内容的输出。
            /// </summary>
            DebugLog,
            /// <summary>
            /// 致命的错误日志：记录系统中出现的能使用系统完全失去功能，服务停止，系统崩溃等使系统无法继续运行下去的错误。例如，数据库无法连接，系统出现死循环。
            /// </summary>
            FatalLog,
            /// <summary>
            /// 错误的错误日志：记录系统中出现的导致系统不稳定，部分功能出现混乱或部分功能失效一类的错误。例如，数据字段为空，数据操作不可完成，操作出现异常等。
            /// </summary>
            ErrorLog
        }
        #endregion

        //记录异常日志数据库连接字符串
        private const string _ConnectionString = @"data source=your server;initial catalog=your db;integrated security=false;persist security info=True;User ID=sa;Password=1111";

        /// <summary>
        /// 使用SQLSERVER记录异常日志
        /// </summary>
        /// <Author>Ryanding</Author>
        /// <date>2011-05-01</date>
        public static void LoadADONetAppender()
        {
            LoadFileAppender();

            log4net.Repository.Hierarchy.Hierarchy hier = LogManager.GetRepository() as log4net.Repository.Hierarchy.Hierarchy;

            if (hier != null)
            {
                log4net.Appender.AdoNetAppender adoAppender = new log4net.Appender.AdoNetAppender();
                adoAppender.Name = "AdoNetAppender";
                adoAppender.CommandType = CommandType.Text;
                adoAppender.BufferSize = 1;
                adoAppender.ConnectionType = "System.Data.SqlClient.SqlConnection, System.Data, Version=1.0.3300.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                adoAppender.ConnectionString = _ConnectionString;
                adoAppender.CommandText = @"INSERT INTO Log ([Date],[Thread],[Level],[Logger],[Message],[Exception]) VALUES (@log_date, @thread, @log_level, @logger, @message, @exception)";
                adoAppender.AddParameter(new AdoNetAppenderParameter { ParameterName = "@log_date", DbType = System.Data.DbType.DateTime, Layout = new log4net.Layout.RawTimeStampLayout() });
                adoAppender.AddParameter(new AdoNetAppenderParameter { ParameterName = "@thread", DbType = System.Data.DbType.String, Size = 255, Layout = new Layout2RawLayoutAdapter(new PatternLayout("%thread")) });
                adoAppender.AddParameter(new AdoNetAppenderParameter { ParameterName = "@log_level", DbType = System.Data.DbType.String, Size = 50, Layout = new Layout2RawLayoutAdapter(new PatternLayout("%level")) });
                adoAppender.AddParameter(new AdoNetAppenderParameter { ParameterName = "@logger", DbType = System.Data.DbType.String, Size = 255, Layout = new Layout2RawLayoutAdapter(new PatternLayout("%logger")) });
                adoAppender.AddParameter(new AdoNetAppenderParameter { ParameterName = "@message", DbType = System.Data.DbType.String, Size = 4000, Layout = new Layout2RawLayoutAdapter(new PatternLayout("%message")) });
                adoAppender.AddParameter(new AdoNetAppenderParameter { ParameterName = "@exception", DbType = System.Data.DbType.String, Size = 4000, Layout = new Layout2RawLayoutAdapter(new ExceptionLayout()) });
                adoAppender.ActivateOptions();
                BasicConfigurator.Configure(adoAppender);
            }


        }

        /// <summary>
        /// 使用文本记录异常日志
        /// </summary>
        /// <Author>Ryanding</Author>
        /// <date>2011-05-01</date>
        public static void LoadFileAppender()
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            string iisBinPath = AppDomain.CurrentDomain.RelativeSearchPath;
            string txtLogPath;
            if (!string.IsNullOrEmpty(iisBinPath))
                txtLogPath = Path.Combine(iisBinPath, "ErrorLog.txt");
            else
                txtLogPath = Path.Combine(currentPath, "ErrorLog.txt");

            // log4net.Repository.Hierarchy.Hierarchy hier = LogManager.GetLoggerRepository() as log4net.Repository.Hierarchy.Hierarchy;

            FileAppender fileAppender = new FileAppender
            {
                Name = "LogFileAppender",
                File = txtLogPath,
                AppendToFile = true
            };

            PatternLayout patternLayout = new PatternLayout();
            patternLayout.ConversionPattern = "记录时间：%date 线程ID:[%thread] 日志级别：%-5level 出错类：%logger property:[%property{NDC}] - 错误描述：%message%newline";
            patternLayout.ActivateOptions();
            fileAppender.Layout = patternLayout;

            //选择UTF8编码，确保中文不乱码。
            fileAppender.Encoding = Encoding.UTF8;

            fileAppender.ActivateOptions();
            BasicConfigurator.Configure(fileAppender);

        }

        /// <summary>
        ///  异常处理
        /// </summary>
        /// <param name="methedType">出现异常方法的类型。例如：MethodBase.GetCurrentMethod().DeclaringType</param>
        /// <param name="errorMsg">错误信息。例如：SaveBuildingPhoto方法出错。Author:开发者名称</param>
        /// <param name="ex"></param>
        public static void InvokeErrorLog(Type methedType, string errorMsg, Exception ex)
        {
            LoadADONetAppender();
            ILog log = log4net.LogManager.GetLogger(methedType);
            log.Info(errorMsg, ex);
        }



        /// <summary>
        /// LoggerName
        /// </summary>
        public static LogName LoggerName = LogName.LogText;
        /// <summary>
        /// 用户ID
        /// </summary>
        public static string UserID = string.Empty;
        /// <summary>
        /// 用户名称
        /// </summary>
        public static string UserName = string.Empty;
        private static ILog iLog;
        private static LogEntity logEntity;
        /// <summary>
        /// 接口
        /// </summary>
        private static ILog log
        {
            get
            {
                string path = @"ConfigInfo\Log4Net.config";
                XmlConfigurator.Configure(new FileInfo(path));
                if (iLog == null)
                {
                    iLog = log4net.LogManager.GetLogger(LoggerName.ToString());
                }
                else
                {
                    if (iLog.Logger.Name != LoggerName.ToString())
                    {
                        iLog = log4net.LogManager.GetLogger(LoggerName.ToString());
                    }
                }
                return iLog;
            }
        }

        /// <summary>
        /// 构造消息实体
        /// </summary>
        /// <param name="message"></param>
        /// <param name="className"></param>
        /// <param name="methodName"></param>
        /// <param name="codeIndex"></param>
        /// <returns></returns>
        private static LogEntity BuildMessageMode(string message,string className,string methodName, string codeIndex)
        {
            if (logEntity == null)
            {
                logEntity = new LogEntity
                {
                    UserID = UserID,
                    UserName = UserName
                };
            }
            logEntity.Message = message;
            logEntity.CodeIndex = codeIndex;
            if (className != "")
                logEntity.ClassName = className;
            else
                className = (new StackTrace(true).GetFrame(2).GetMethod()).ReflectedType.Namespace;
            if (methodName != "")
                logEntity.MethodName = methodName;
            else
                logEntity.MethodName = (new StackTrace(true).GetFrame(2).GetMethod()).Name;
            return logEntity;
        }
        /// <summary>
        /// 调试
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="className">类名</param>
        /// <param name="methodName">方法名</param>
        /// <param name="codeIndex">代码行号</param>
        public static void Debug(string message, string className = "", string methodName = "", string codeIndex = "")
        {
            if (log.IsDebugEnabled)
                log.Debug(BuildMessageMode(message,className,methodName,codeIndex));
        }
        /// <summary>
        /// 调试
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="ex">异常</param>
        /// <param name="className">类名</param>
        /// <param name="methodName">方法名</param>
        /// <param name="codeIndex">代码行号</param>
        public static void Debug(string message, Exception ex, string className = "", string methodName = "", string codeIndex = "")
        {
            if (log.IsDebugEnabled)
                log.Debug(BuildMessageMode(message, className, methodName, codeIndex), ex);
        }
        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="className">类名</param>
        /// <param name="methodName">方法名</param>
        /// <param name="codeIndex">代码行号</param>
        public static void Info(string message,string className="",string methodName="",string codeIndex="")
        {
            //if (log.IsInfoEnabled)
                log.Info(BuildMessageMode(message, className, methodName, codeIndex));
        }
        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="className">类名</param>
        /// <param name="methodName">方法名</param>
        /// <param name="codeIndex">代码行号</param>
        public static void Info(string message, Exception ex, string className = "", string methodName = "", string codeIndex = "")
        {
            //if (log.IsInfoEnabled)
                log.Info(BuildMessageMode(message, className, methodName, codeIndex), ex);
        }
        /// <summary>
        /// 一般错误
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="className">类名</param>
        /// <param name="methodName">方法名</param>
        /// <param name="codeIndex">代码行号</param>
        public static void Error(string message, string className = "", string methodName = "", string codeIndex = "")
        {
            if (log.IsErrorEnabled)
                log.Error(BuildMessageMode(message, className, methodName, codeIndex));
        }
        /// <summary>
        /// 一般错误
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="exception">异常</param>
        /// <param name="className">类名</param>
        /// <param name="methodName">方法名</param>
        /// <param name="codeIndex">代码行号</param>
        public static void Error(string message, Exception exception, string className = "", string methodName = "", string codeIndex = "")
        {
            if (log.IsErrorEnabled)
                log.Error(BuildMessageMode(message, className, methodName, codeIndex), exception);
        }
        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="className">类名</param>
        /// <param name="methodName">方法名</param>
        /// <param name="codeIndex">代码行号</param>
        public static void Warn(string message, string className = "", string methodName = "", string codeIndex = "")
        {
            if (log.IsWarnEnabled)
                log.Warn(BuildMessageMode(message, className, methodName, codeIndex));
        }
        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="ex">异常</param>
        /// <param name="className">类名</param>
        /// <param name="methodName">方法名</param>
        /// <param name="codeIndex">代码行号</param>
        public static void Warn(string message, Exception ex, string className = "", string methodName = "", string codeIndex = "")
        {
            if (log.IsWarnEnabled)
                log.Warn(BuildMessageMode(message, className, methodName, codeIndex), ex);
        }
        /// <summary>
        /// 严重
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="className">类名</param>
        /// <param name="methodName">方法名</param>
        /// <param name="codeIndex">代码行号</param>
        public static void Fatal(string message, string className = "", string methodName = "", string codeIndex = "")
        {
            if (log.IsFatalEnabled)
                log.Fatal(BuildMessageMode(message, className, methodName, codeIndex));
        }
        /// <summary>
        /// 严重
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="ex">异常</param>
        /// <param name="className">类名</param>
        /// <param name="methodName">方法名</param>
        /// <param name="codeIndex">代码行号</param>
        public static void Fatal(string message, Exception ex, string className = "", string methodName = "", string codeIndex = "")
        {
            if (log.IsFatalEnabled)
                log.Fatal(BuildMessageMode(message, className, methodName, codeIndex), ex);
        }

        /// <summary>
        /// 日志对象
        /// </summary>
        [Serializable]
        public class LogEntity
        {
            /// <summary>
            /// 用户ID
            /// </summary>
            public string UserID { get; set; }
            /// <summary>
            /// 用户名称
            /// </summary>
            public string UserName { get; set; }
            /// <summary>
            /// 日志信息
            /// </summary>
            public string Message { get; set; }
            /// <summary>
            /// 方法类名
            /// </summary>
            public string ClassName { get; set; }
            /// <summary>
            /// 方法名称
            /// </summary>
            public string MethodName { get; set; }
            /// <summary>
            /// 代码行号
            /// </summary>
            public string CodeIndex { get; set; }
            /// <summary>
            /// 异常信息
            /// </summary>
            public Exception Exception { get; set; }

        }
        /// <summary>
        /// Log书写类型名
        /// </summary>
        public enum LogName
        {
            /// <summary>
            /// 控制台
            /// </summary>
            LogConsole,
            /// <summary>
            /// windows日志
            /// </summary>
            LogWindows,
            /// <summary>
            /// 文本
            /// </summary>
            LogText,
            /// <summary>
            /// 数据库
            /// </summary>
            LogDataBase
        }
    }
}
