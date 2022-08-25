//using IWshRuntimeLibrary;
using SuperFramework.SuperRegistry;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using static SuperFramework.WindowsAPI.User32API;

namespace SuperFramework
{
    /// <summary>
    /// <para>说明：主程序辅助</para>
    /// <para>作者：不良帥</para>
    /// <para>日期：2016-01-30</para>
    /// </summary>
    public static class AppHelper
    {
        /// <summary> 
        /// 设置应用程序开机是否自动运行
        /// </summary>          
        /// <param name="filePath">应用程序的全路径</param>          
        /// <param name="isAutoRun">是否自动运行，为false时，取消自动运行</param>         
        /// <exception cref="Exception">设置不成功时抛出异常</exception>         

        public static void AppSetAutoRun(string filePath, bool isAutoRun)
        {
            RegistHelper.SetAutoRun(filePath, isAutoRun);
        }
        /// <summary>
        /// 程序是否自动运行
        /// </summary>
        /// <param name="filePath">应用程序的全路径</param>      
        /// <returns>true：自动运行，false：非自动运行</returns>
        /// <exception cref="Exception">获取不成功时抛出异常</exception> 
        public static bool AppIsAutoRun(string filePath)
        {
            return RegistHelper.IsAutoRun(filePath);
        }
        #region 程序倒计时退出
        /// <summary>
        /// 程序退出倒计时
        /// </summary>
        /// <param name="seconds">倒计时 （秒）</param>
        /// <param name="path"></param>
        public static void AppCountdown(long seconds, string path = "")
        {
            new Thread(async () =>
            {
                // 程序启动时间
                DateTime time = DateTime.Now;
                while (true)
                {
                    if (SuperDate.DateHelper.DateDiff2(DateTime.Now, time).TotalSeconds >= seconds)
                        break;
                    else
                        await Task.Delay(1000).ConfigureAwait(false);
                }
                if (string.IsNullOrWhiteSpace(path))
                    Environment.Exit(0);
                else
                    ProcessHelper.KillProcess(path);
            })
            { IsBackground = true }.Start();
        }
        #endregion

        #region 检查程序是否已经运行
        /// <summary>
        /// 检查程序是否已经运行
        /// </summary>
        /// <returns>true：已打开；false：未运行</returns>
        public static bool AppCheckIsRun()
        {
            //获取当前活动进程的模块名称
            string moduleName = Process.GetCurrentProcess().MainModule.ModuleName;
            //返回指定路径字符串的文件名
            string processName = System.IO.Path.GetFileNameWithoutExtension(moduleName);
            //根据文件名创建进程资源数组
            Process[] processes = Process.GetProcessesByName(processName);
            //如果该数组长度大于1，说明多次运行
            if (processes.Length > 1)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region  检查系统主体是否以管理员运行的 
        /// <summary>
        /// 检查系统主体是否以管理员运行的
        /// </summary>
        /// <returns>管理员运行返回true，非管理员运行返回false</returns>
        public static bool AppIsAdmin()
        {
            WindowsIdentity current = WindowsIdentity.GetCurrent();
            WindowsPrincipal windowsPrincipal = new WindowsPrincipal(current);
            //WindowsBuiltInRole可以枚举出很多权限，例如系统用户、User、Guest等等
            return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        #endregion

        #region  打开程序
        /// <summary>
        /// 打开本程序
        /// </summary>
        /// <param name="path">程序路径</param>
        public static void AppStartThis(string path = "")
        {
            if (path == "")
                Process.Start(System.Reflection.Assembly.GetExecutingAssembly().Location);
            else
                Process.Start(path);
            //Application.Exit();
        }

        /// <summary>
        /// 使用cmd启动程序
        /// </summary>
        /// <param name="sExePath">程序路径 Application.ExecutablePath</param>
        /// <param name="sArguments">启动参数</param>
        /// <param name="sleep">必须等待，否则重启的程序还未启动完成时将推出cmd；根据情况调整等待时间</param>
        public static void AppStartByCmd(string sExePath, string sArguments = "", int sleep = 5000)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = false;
            p.StartInfo.RedirectStandardError = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.Start();
            p.StandardInput.WriteLine(string.Format("{0} {1}", sExePath, sArguments));
            p.StandardInput.WriteLine("exit");
            p.Close();
            Thread.Sleep(sleep);//必须等待，否则重启的程序还未启动完成；根据情况调整等待时间
        }
        #endregion

        public static void ShowDesktop()
        {

            Type shellType = Type.GetTypeFromProgID("Shell.Application");
            object shellObject = System.Activator.CreateInstance(shellType);
            shellType.InvokeMember("ToggleDesktop", System.Reflection.BindingFlags.InvokeMethod, null, shellObject, null);
        }
        ////#region 快捷方式
        /////// <summary>
        /////// 创建快捷方式
        /////// </summary>
        /////// <param name="lnkName">快捷名称</param>
        /////// <param name="executablePath">exe全路径</param>
        /////// <param name="folder">快捷方式创建路径</param>
        ////public static void Createlnk(string lnkName, string executablePath, string icoPath, Environment.SpecialFolder folder = Environment.SpecialFolder.Desktop)                                 //创建快捷方式 设置开机启动
        ////{
        ////    WshShell shell = new WshShell();
        ////    IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(string.Format("{0}\\{1}.lnk", folder, lnkName));
        ////    shortcut.TargetPath = executablePath;
        ////    shortcut.Arguments = "-s";         //快捷启动  参数  用于静默设置
        ////    shortcut.WorkingDirectory = Environment.CurrentDirectory;
        ////    shortcut.IconLocation = icoPath;
        ////    shortcut.Save();

        ////}
        /////// <summary>
        /////// 删除快捷方式
        /////// </summary>
        /////// <param name="lnkName">快捷名称</param>
        /////// <param name="folder">快捷方式路径</param>
        ////public static void Deletelnk(string lnkName, Environment.SpecialFolder folder = Environment.SpecialFolder.Desktop)       //删除快捷方式
        ////{
        ////    System.IO.File.Delete(string.Format("{0}\\{1}.lnk", folder, lnkName));
        ////}
        ////#endregion

        #region 显示已运行的程序实例
        private const int WS_SHOWNORMAL = 1;
        /// <summary>
        /// 显示已运行的程序实例
        /// </summary>
        /// <param name="instance">进程对象</param>
        public static void ShowRunningInstance(Process instance)
        {
            //确保窗口没有最小化或最大化   
            ShowWindowAsync(instance.MainWindowHandle, WS_SHOWNORMAL);
            //设置为前台窗口
            SetForegroundWindow(instance.MainWindowHandle);
        }
        #endregion

        #region 禁止启动任务管理器
        /// <summary>
        /// 禁止启动任务管理器
        /// </summary>
        public static void FindAndKillTaskmgr()
        {
            var listenTaskChangThread = new Thread(new ParameterizedThreadStart((t) =>
            {
                Process[] find = null;
                while (true)
                {
                    find = Process.GetProcessesByName("Taskmgr");
                    Thread.Sleep(50);
                    if (find.Length <= 0) continue;
                    else
                    {
                        for (int i = 0; i < find.Length; i++)
                        {
                            find[i].Kill();
                        }
                    }
                }
            }));
            listenTaskChangThread.Start();
        }
        #endregion

        #region 获取路径
        /// <summary>
        /// 模块的完整路径
        /// </summary>
        /// <returns>程序根路径</returns>
        public static string AppStartPath => System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
        /// <summary>
        /// 程序的根目录
        /// </summary>
        /// <returns>程序根路径</returns>
        public static string AppPathRoot => System.IO.Path.GetPathRoot(Process.GetCurrentProcess().MainModule.FileName);
        /// <summary>
        /// exe的完整路径
        /// </summary>
        /// <returns>程序路径</returns>
        public static string AppFullPath => System.Windows.Forms.Application.ExecutablePath;
        //AppDomain.CurrentDomain.SetupInformation.ApplicationBase;                                                                  
        //  return Process.GetCurrentProcess().ProcessName;
        
        /// <summary>
        /// exe的名称
        /// </summary>
        /// <returns>程序路径</returns>
        public static string AppExeName => System.IO.Path.GetFileName(Assembly.GetEntryAssembly().GetName().Name + ".exe");
        #endregion
    }
}
