using log4net;
using log4net.Config;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LogManager = log4net.LogManager;
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "ConfigInfo\\log4net.config", Watch = true)]
namespace System
{
    /// <summary>
    /// 超高速高性能写日志
    /// 1 FlashLogger.Instance().Register();
    /// 2 FlashLogger.Debug("Debug", new Exception("testexception"));
    /// </summary>
    public sealed class FlashLogger
    {
        /// <summary>
        /// 记录消息Queue
        /// </summary>
        private readonly ConcurrentQueue<FlashLogMessage> que;

        /// <summary>
        /// 信号
        /// </summary>
        private readonly ManualResetEvent mre;

        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILog log;
        CancellationTokenSource cancellationTokenSource;
        CancellationToken cancellationToken;
        Task taskLog;
        /// <summary>
        /// 日志
        /// </summary>
        private static FlashLogger flashLog; //= new FlashLogger();

        private FlashLogger(string logConfigFile = "ConfigInfo\\log4net.config", string logName = "LogText")
        {

            var configFile = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, logConfigFile));
            if (!configFile.Exists)
            {
                throw new ArgumentNullException(nameof(logConfigFile));
            }

            // 设置日志配置文件路径
            XmlConfigurator.Configure(configFile);

            que = new ConcurrentQueue<FlashLogMessage>();
            mre = new ManualResetEvent(false);
            log = LogManager.GetLogger(logName);//(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        /// <summary>
        /// 单例
        /// </summary>
        /// <returns></returns>
        public static FlashLogger Instance(string logConfigFile = "ConfigInfo\\log4net.config", string logName = "LogText")
        {
            if (flashLog == null)
                flashLog = new FlashLogger(logConfigFile, logName);
            return flashLog;
        }

        /// <summary>
        /// 程序初始化时注册调用一次
        /// </summary>
        public void Register()
        {
            cancellationTokenSource = new CancellationTokenSource();
            cancellationToken = cancellationTokenSource.Token;
            taskLog = Task.Factory.StartNew(WriteLog, cancellationToken);
        }
        public void UnRegister()
        {
            if(cancellationToken.CanBeCanceled)
                cancellationTokenSource.Cancel();
            try
            {
                taskLog.Dispose();
            }
            catch { }
        }
        /// <summary>
        /// 从队列中写日志至磁盘
        /// </summary>
        private async void WriteLog()
        {
            while (true)
            {
                // 等待信号通知
                mre.WaitOne();

                // 判断是否有内容需要如磁盘 从列队中获取内容，并删除列队中的内容
                while (!que.IsEmpty && que.TryDequeue(out FlashLogMessage msg))
                {
                    // 判断日志等级，然后写日志
                    switch (msg.Level)
                    {
                        case FlashLogLevel.Debug:
                            log?.Debug(msg.Message, msg?.Exception);
                            break;
                        case FlashLogLevel.Info:
                            log?.Info(msg.Message, msg?.Exception);
                            break;
                        case FlashLogLevel.Error:
                            log?.Error(msg.Message, msg?.Exception);
                            break;
                        case FlashLogLevel.Warn:
                            log?.Warn(msg.Message, msg?.Exception);
                            break;
                        case FlashLogLevel.Fatal:
                            log?.Fatal(msg.Message, msg?.Exception);
                            break;
                    }
                }

                // 重新设置信号
                mre.Reset();
                await Task.Delay(1);
            }
        }


        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="message">日志文本</param>
        /// <param name="level">等级</param>
        /// <param name="ex">Exception</param>
        void EnqueueMessage(string message, FlashLogLevel level, Exception ex = null)
        {
            if ((level == FlashLogLevel.Debug && log.IsDebugEnabled)
             || (level == FlashLogLevel.Error && log.IsErrorEnabled)
             || (level == FlashLogLevel.Fatal && log.IsFatalEnabled)
             || (level == FlashLogLevel.Info && log.IsInfoEnabled)
             || (level == FlashLogLevel.Warn && log.IsWarnEnabled))
            {
                que.Enqueue(new FlashLogMessage
                {
                    // Message = "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff") + "]\r\n" + message,
                    Message = message,
                    Level = level,
                    Exception = ex
                });

                // 通知线程往磁盘中写日志
                mre.Set();
            }
        }

        public static void Debug(string msg, Exception ex = null)
        {

            string file = SuperFramework.CodeHelper.GetCurrentMethodFullName(2);
            string line = SuperFramework.CodeHelper.GetLineNum(2);
            Instance().EnqueueMessage($"[{file}:{line}]" + msg, FlashLogLevel.Debug, ex);
        }

        public static void Error(string msg, Exception ex)
        {
            string file = SuperFramework.CodeHelper.GetCurrentMethodFullName(2);
            string line = SuperFramework.CodeHelper.GetLineNum(2);
            Instance().EnqueueMessage($"[{file}:{line}]" + msg, FlashLogLevel.Error, ex);
        }

        public static void Fatal(string msg, Exception ex)
        {
            string file = SuperFramework.CodeHelper.GetCurrentMethodFullName(2);
            string line = SuperFramework.CodeHelper.GetLineNum(2);
            Instance().EnqueueMessage($"[{file}:{line}]" + msg, FlashLogLevel.Fatal, ex);
        }

        public static void Info(string msg)
        {
            string file = SuperFramework.CodeHelper.GetCurrentMethodFullName(2);
            string line = SuperFramework.CodeHelper.GetLineNum(2);
            Instance().EnqueueMessage($"[{file}:{line}]" + msg, FlashLogLevel.Info);
        }

        public static void Warn(string msg, Exception ex = null)
        {
            string file = SuperFramework.CodeHelper.GetCurrentMethodFullName(2);
            string line = SuperFramework.CodeHelper.GetLineNum(2);
            Instance().EnqueueMessage($"[{file}:{line}]" + msg, FlashLogLevel.Warn, ex);
        }
        /// <summary>
        /// 日志等级
        /// </summary>
        internal enum FlashLogLevel
        {
            Debug,
            Info,
            Error,
            Warn,
            Fatal
        }

        /// <summary>
        /// 日志内容
        /// </summary>
        internal class FlashLogMessage
        {
            public string Message { get; set; }
            public FlashLogLevel Level { get; set; }
            public Exception Exception { get; set; }

        }
    }
}