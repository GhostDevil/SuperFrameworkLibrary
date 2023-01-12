using SuperFramework.SuperRegistry;
using SuperNetCore;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Versioning;
using System.Security.Principal;
using System.Threading.Tasks;

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
            RegistryHelper.SetAutoRun(filePath, isAutoRun);
        }
        /// <summary>
        /// 程序是否自动运行
        /// </summary>
        /// <param name="filePath">应用程序的全路径</param>      
        /// <returns>true：自动运行，false：非自动运行</returns>
        /// <exception cref="Exception">获取不成功时抛出异常</exception> 
        public static bool AppIsAutoRun(string filePath)
        {
            return RegistryHelper.IsAutoRun(filePath);
        }
        #region 程序倒计时退出
        /// <summary>
        /// 程序退出倒计时
        /// </summary>
        /// <param name="seconds">倒计时 （秒）</param>
        /// <param name="path"></param>
        public async static void AppCountdown(long seconds, string path = "")
        {
            await Task.Factory.StartNew(async () =>
            {
                // 程序启动时间
                DateTime time = DateTime.Now;
                while (true)
                {
                    if (SuperNetCore.SuperDate.DateHelper.DateDiff2(DateTime.Now, time).TotalSeconds >= seconds)
                        break;
                    else
                        await Task.Delay(1000).ConfigureAwait(false);
                }
                if (string.IsNullOrWhiteSpace(path))
                    Environment.Exit(0);
                else
                    await ProcessHelper.KillProcessAsync(path);
            });
        }
        #endregion

      
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

       
        /// <summary>
        /// 检查系统主体是否以管理员运行的
        /// </summary>
        /// <returns>管理员运行返回true，非管理员运行返回false</returns>
        [SupportedOSPlatform("windows")]
        public static bool AppIsAdmin()
        {
            WindowsIdentity current = WindowsIdentity.GetCurrent();
            WindowsPrincipal windowsPrincipal = new(current);
            //WindowsBuiltInRole可以枚举出很多权限，例如系统用户、User、Guest等等
            return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
        }
     
        /// <summary>
        /// 使用cmd启动程序
        /// </summary>
        /// <param name="sExePath">程序路径 Application.ExecutablePath</param>
        /// <param name="sArguments">启动参数</param>
        /// <param name="sleep">必须等待，否则重启的程序还未启动完成时将推出cmd；根据情况调整等待时间</param>
        public async static void AppStartByCmd(string sExePath, string sArguments = "", int sleep = 5000)
        {
            using (Process p = new())
            {
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
            }

            await Task.Delay(sleep);//必须等待，否则重启的程序还未启动完成；根据情况调整等待时间
        }

        [SupportedOSPlatform("windows")]
        public static void ShowDesktop()
        {

            Type shellType = Type.GetTypeFromProgID("Shell.Application");
            object shellObject = System.Activator.CreateInstance(shellType);
            shellType.InvokeMember("ToggleDesktop", System.Reflection.BindingFlags.InvokeMethod, null, shellObject, null);
        }
        #region 禁止启动任务管理器
        /// <summary>
        /// 禁止启动任务管理器
        /// </summary>
        public static void KillTaskbar()
        {
            Task.Factory.StartNew(async () =>
            {
                Process[] find = null;
                while (true)
                {
                    find = Process.GetProcessesByName("Taskmgr");
                    if (find.Length <= 0) continue;
                    else
                    {
                        for (int i = 0; i < find.Length; i++)
                        {
                            find[i].Kill();
                        }
                    }
                    await Task.Delay(50);
                }
            });
        }
        #endregion

        #region 获取路径
        /// <summary>
        /// 模块的完整路径
        /// </summary>
        /// <returns>程序根路径</returns>
        public static string AppStartPath => System.IO.Path.GetDirectoryName(Environment.ProcessPath);
        /// <summary>
        /// 程序的根目录
        /// </summary>
        /// <returns>程序根路径</returns>
        public static string AppPathRoot => System.IO.Path.GetPathRoot(Environment.ProcessPath);
        /// <summary>
        /// exe的完整路径
        /// </summary>
        /// <returns>程序路径</returns>
        public static string AppFullPath => System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;

        /// <summary>
        /// exe的名称
        /// </summary>
        /// <returns>程序路径</returns>
        public static string AppExeName => System.IO.Path.GetFileName(Assembly.GetEntryAssembly().GetName().Name + ".exe");
        #endregion
    }
}
