using System;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using System.Threading;

namespace SuperForm
{
    /// <summary>
    /// 日 期:2015-01-15
    /// 作 者:不良帥
    /// 描 述:主程序辅助类
    /// </summary>
    public static class FormAppHelper
    {


        #region  以管理员启动程序,限启动一个实例
        /// <summary>
        /// 以管理员启动程序，可限启动一个实例，替代Main函数。
        /// </summary>
        /// <param name="form">程序主窗体</param>
        /// <param name="Args">数组参数</param>
        /// <param name="isOne">是否启动一个实例</param>
        /// <param name="isShowMsg">是否显示提示</param>
        public static void RunByAdmin(Form form, string[] Args, bool isOne = false, bool isShowMsg = false)
        {
            /**
             * 当前用户是管理员的时候，直接启动应用程序
             * 如果不是管理员，则使用启动对象启动程序，以确保使用管理员身份运行
             */
            //获得当前登录的Windows用户标示
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            //创建Windows用户主题
            System.Windows.Forms.Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);

            //返回是否赋予了使用线程的互斥体初始所属权 
            /*
            在运行终端服务的服务器上，已命名的系统 mutex 可以具有两级可见性。如果名称以前缀“Global\”开头，则 mutex 在所有终端服务器会话中均为可见。如果名称以前缀“Local\”开头，则 mutex 仅在创建它的终端服务器会话中可见。在这种情况下，服务器上各个其他终端服务器会话中都可以拥有一个名称相同的独立 mutex。如果创建已命名 mutex 时不指定前缀，则它将采用前缀“Local\”。在终端服务器会话中，只是名称前缀不同的两个 mutex 是独立的 mutex，这两个 mutex 对于终端服务器会话中的所有进程均为可见。即：前缀名称“Global\”和“Local\”说明 mutex 名称相对于终端服务器会话（而并非相对于进程）的范围。
            */
            using (Mutex instance = new Mutex(true, string.Format("Global\\{0}{1}", System.Windows.Forms.Application.ProductName, form.Name), out bool createdNew))
            {//同步基元变量 
             //判断当前登录用户是否为管理员

                //    //如果是管理员，则直接运行
                //    if (isOne)
                //    {

                //        if (createdNew) //赋予了线程初始所属权，也就是首次使用互斥体 
                //            instance.ReleaseMutex();
                //        else
                //        {
                //            if(isShowMsg)
                //                MessageBox.Show("已经启动了一个程序，请先退出 !", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //            Environment.Exit(0);
                //            Process.GetCurrentProcess().Dispose();
                //            //return;
                //        }
                //    }
                //    //Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                //    //AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException2;
                //    //Application.ThreadException += Application_ThreadException2;
                //    Application.Run(form);
                //}
                //else
                //{

                //创建启动对象
                ProcessStartInfo startInfo = new ProcessStartInfo() { /*设置运行文件*/FileName = System.Windows.Forms.Application.ExecutablePath, /*设置启动参数*/Arguments = string.Join(" ", Args), /*设置启动动作,确保以管理员身份运行*/Verb = "runas" };
                if (isOne)
                {
                    if (createdNew) //赋予了线程初始所属权，也就是首次使用互斥体 
                        instance.ReleaseMutex();
                    else
                    {
                        if (isShowMsg)
                            System.Windows.Forms.MessageBox.Show("已经启动了一个程序，请先退出 !", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(0);
                        Process.GetCurrentProcess().Dispose();
                        //return;
                    }
                }
                if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
                {
                    try
                    {
                        //如果不是管理员，则启动UAC
                        Process.Start(startInfo);
                    }
                    catch (Exception) { return; }
                    //退出
                    // Application.Exit();
                    Environment.Exit(0);
                }
                System.Windows.Forms.Application.Run(form);
            }
        }

        private static void CurrentDomain_UnhandledException2(object sender, UnhandledExceptionEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("程序运行出错，请查看错误信息，联系管理员处理！\r\n" + (e.ExceptionObject as Exception).Message.ToString(), "异 常", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private static void Application_ThreadException2(object sender, ThreadExceptionEventArgs e)
        {
            //Application.Restart();
            // string s = e.Exception.Message;
            System.Windows.Forms.MessageBox.Show("程序运行出错，请查看错误信息，联系管理员处理！\r\n" + e.Exception.Message, "异 常", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //Application.Exit();
            System.Windows.Forms.Application.Restart();
        }

        /// <summary>
        /// 限制程序只能有一个启动实例，替代Main函数。
        /// </summary>
        /// <param name="form">启动窗体</param>
        /// <param name="isShowMsg"></param>
        public static void RestrictAppOne(Form form, bool isShowMsg = false)
        {
            bool createdNew; //返回是否赋予了使用线程的互斥体初始所属权 
            using (Mutex instance = new Mutex(true, string.Format("Global\\{0}{1}", System.Windows.Forms.Application.ProductName, form.Name), out createdNew))
            { //同步基元变量 
                if (createdNew) //赋予了线程初始所属权，也就是首次使用互斥体 
                {
                    System.Windows.Forms.Application.EnableVisualStyles();
                    //Application.SetCompatibleTextRenderingDefault(false);
                    System.Windows.Forms.Application.Run(form);
                    instance.ReleaseMutex();
                }
                else
                {
                    if (isShowMsg)
                        System.Windows.Forms.MessageBox.Show("已经启动了一个程序，请先退出 !", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    System.Windows.Forms.Application.Exit();
                }
            }
        }
        #endregion

        #region  获取全局异常Main方法 
        /// <summary>
        /// 获取全局异常Main方法
        /// </summary>
        /// <param name="frmMain">主窗体对象</param>
        public static void AppExceptionMain(Form frmMain)
        {
            try
            {
                //设置应用程序处理异常方式：ThreadException处理
                System.Windows.Forms.Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                //处理UI线程异常
                System.Windows.Forms.Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
                //处理非UI线程异常
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

                #region 应用程序的主入口点
                System.Windows.Forms.Application.EnableVisualStyles();
                System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
                System.Windows.Forms.Application.Run(frmMain);
                #endregion
            }
            catch (Exception ex)
            {
                string str = GetExceptionMsg(ex, string.Empty);
                System.Windows.Forms.MessageBox.Show(str, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            string str = GetExceptionMsg(e.Exception, e.ToString());
            System.Windows.Forms.MessageBox.Show(str, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string str = GetExceptionMsg(e.ExceptionObject as Exception, e.ToString());
            System.Windows.Forms.MessageBox.Show(str, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// 生成自定义异常消息
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <param name="backStr">备用异常消息：当ex为null时有效</param>
        /// <returns>异常字符串文本</returns>
        static string GetExceptionMsg(Exception ex, string backStr)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("****************************异常文本****************************");
            sb.AppendLine(" 出现时间 ：" + DateTime.Now.ToString());
            if (ex != null)
            {
                sb.AppendLine(" 异常类型 ：" + ex.GetType().Name);
                sb.AppendLine(" 异常信息 ：" + ex.Message);
                sb.AppendLine(" 堆栈调用 ：" + ex.StackTrace);
            }
            else
            {
                sb.AppendLine(" 未处理异常 ：" + backStr);
            }
            sb.AppendLine("***************************************************************");
            return sb.ToString();
        }
        #endregion


        //#region 启动程序
        ///// <summary>
        ///// 启动程序
        ///// </summary>
        ///// <param name="path">程序路径</param>
        //public static void StartApp(string path)
        //{
        //    //Process.GetCurrentProcess().MainModule.FileName;
        //    Process myNewProcess = new Process();
        //    myNewProcess.StartInfo.FileName = path;
        //    myNewProcess.StartInfo.WorkingDirectory = Application.ExecutablePath;
        //    myNewProcess.Start();
        //    //Application.Exit();
        //    Environment.Exit(0);
        //}
        //#endregion

    }
}
