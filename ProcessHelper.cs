using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using static SuperFramework.WindowsAPI.User32API;

namespace SuperFramework
{
    /// <summary>
    /// <para>日期：2021-06-05</para>
    /// <para>作者：不良帥</para>
    /// <para>描述：操作进程方法</para>
    /// </summary>
    public static class ProcessHelper
    {
        #region  关闭进程 
        /// <summary>
        /// 根据进程名关闭进程
        /// </summary>
        /// <param name="processName">进程名称，非路径不包含后缀</param>
        /// <param name="killAll">关闭同一路径所有进程</param>
        /// <returns>成功返回true，失败返回false</returns>
        public static bool CloseProcessByName(string processName, bool killAll = true)
        {
            try
            {
                //根据进程名称,获取该进程信息
                Process[] MyProcess = Process.GetProcessesByName(processName);
                foreach (Process myProcess in MyProcess)
                {
                    myProcess.Kill();
                    myProcess.WaitForExit();
                    myProcess.Close();
                    if (!killAll)
                        break;
                    else
                        Thread.Sleep(50);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 关闭进程
        /// </summary>
        /// <param name="pName">进程的名称，非路径不包含后缀</param>
        /// <param name="killAll">关闭同一路径所有进程</param>
        /// <returns>成功返回true，失败返回false</returns>
        public static bool CloseProcessByPName(string pName, bool killAll = false)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(pName))
            {
                try
                {
                    Process[] myProcesses = Process.GetProcesses();
                    foreach (Process myProcess in myProcesses)
                    {
                        try
                        {
                            if (myProcess.ProcessName.ToUpper() == pName.Substring(0, pName.LastIndexOf(".")).ToUpper())
                            {
                                myProcess.Kill();
                                result = true;
                                if (!killAll)
                                    break;
                                else
                                    Thread.Sleep(50);
                            }

                        }
                        catch (Exception)
                        {

                            if (myProcess.ProcessName.ToUpper() == pName.ToUpper())
                            {
                                myProcess.Kill();
                                if (!killAll)
                                    break;
                                else
                                    Thread.Sleep(50);
                            }
                        }
                    }

                }
                catch (Exception ex) { }
            }
            return result;

        }

        /// <summary>
        /// 根据进程名关闭一组进程
        /// </summary>
        /// <param name="process">进程集合</param>
        /// <returns>返回集合包含操作状态</returns>
        public static List<ProcessBaseInfo> CloseProcessByNames(List<ProcessBaseInfo> process)
        {
            if (process.Count > 0)
            {
                foreach (var item in process)
                {
                    //根据进程名称,获取该进程信息
                    Process[] MyProcessS = Process.GetProcessesByName(item.ProcessName);
                    bool b = true;
                    foreach (Process MyProcess in MyProcessS)
                    {
                        try
                        {
                            MyProcess.Kill();
                            MyProcess.WaitForExit();
                            MyProcess.Close();
                            b = false;
                            Thread.Sleep(50);
                        }
                        catch (Exception) { }
                    }
                    item.HasExited = b;
                }
            }
            return process;
        }
        /// <summary>
        /// 根据进程路径关闭进程
        /// </summary>
        /// <param name="processPath">进程完全路径</param>
        /// <param name="killAll">关闭同一路径所有进程</param>
        /// <returns>成功返回true，失败返回false</returns>
        public static bool CloseProcessByPath(string processPath, bool killAll = true)
        {
            try
            {
                foreach (Process pro in Process.GetProcesses())
                {
                    try
                    {
                        if (pro.MainModule.FileName == processPath)
                        {
                            pro.Kill();
                            pro.WaitForExit();
                            pro.Close();
                            if (!killAll)
                                break;
                            else
                                Thread.Sleep(10);
                        }
                    }
                    catch (Exception) { Thread.Sleep(10); }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 根据进程路径关闭一组进程
        /// </summary>
        /// <param name="process">进程完全路径</param>
        /// <param name="killAll">关闭同一路径所有进程</param>
        /// <returns>成功返回true，失败返回false</returns>
        public static List<ProcessBaseInfo> CloseProcessByPath(List<ProcessBaseInfo> process, bool killAll = true)
        {
            foreach (ProcessBaseInfo item in process)
            {
                foreach (Process pro in Process.GetProcesses())
                {
                    if (pro.MainModule.FileName == item.ProcessPath)
                    {
                        try
                        {
                            pro.Kill();
                            pro.WaitForExit();
                            pro.Close();
                            item.HasExited = false;
                        }
                        catch
                        {
                            item.HasExited = false;
                        }
                        if (!killAll)
                            break;
                    }

                }

            }
            return process;
        }
        static uint WM_CLOSE = 0x10;
        /// <summary>
        /// 关闭本地程序
        /// </summary>
        /// <param name="strClassName">类名</param>
        /// <param name="strTitleName">标题</param>
        /// <param name="strAppName">名称(不含后缀)</param>
        /// <param name="timeOut">超时时间</param>
        public async static Task<int> CloseApp(string strTitleName, string strAppName, string strClassName = null,int timeOut=100)
        {
            int x = 0;
            if (strClassName != "" || strTitleName != "")
            {
                IntPtr appHandle = FindWindow(strClassName, strTitleName);
                if (appHandle != IntPtr.Zero)
                {
                    SendMessage(appHandle, (int)WM_CLOSE, 0, 0);
                }
                if (strAppName != "")
                    await Task.Delay(timeOut).ConfigureAwait(false);
            }
            if (strAppName != "")
            {
                //提供对本地和远程访问并停止和启动本地进程
                Process[] myProcesses = Process.GetProcessesByName(strAppName);

                foreach (Process myProcess in myProcesses)
                {
                    if (strAppName.ToLower() == myProcess.ProcessName.Trim().ToLower() || strAppName.ToLower() + ".exe" == myProcess.ProcessName.Trim().ToLower())
                    {
                        try
                        {
                            try
                            {
                                myProcess.CloseMainWindow();
                                //myProcess.Close();
                            }
                            catch { }
                            
                            myProcess.Kill();
                        }
                        catch
                        {
                            try 
                            {
                                //SendMessage(myProcess.Handle, (int)WM_CLOSE, 0, 0);
                                myProcess.Dispose();
                            } catch { }
                        }
                        x++;
                    }
                }
            }
            return x;
        }
        /// <summary>
        /// 结束任务
        /// </summary>
        /// <param name="appName">程序名称</param>
        /// <returns>操作结果</returns>
        public static string  TaskKill(string appName)
        {
            return  RunCmd("taskkill /f /im " + appName);
        }
        /// <summary>
        /// 结束任务
        /// </summary>
        /// <param name="appName">程序名称</param>
        /// <returns>操作结果</returns>
        public static string TaskKills(List<string> appName)
        {
            string arg = "";
            appName?.ForEach(x => arg+=$" /im {x}");
            return RunCmd($"taskkill /f{arg}");
        }
        /// <summary>
        /// 关闭指定进程
        /// </summary>
        /// <param name="pid">进程id</param>
        /// <returns>操作结果</returns>
        public static string TaskKill(int pid)
        {
            return RunCmd($"taskkill /f /t /fi \"pid eq { pid}\"");
        }
        ///// <summary>
        ///// 结束任务
        ///// </summary>
        ///// <param name="taskPid">进程id</param>
        ///// <returns>操作结果</returns>
        //public static string TaskKill(int taskPid)
        //{
        //    return RunCmd("taskkill /f /pid " + taskPid);
        //}
        /// <summary>
        /// 运行cmd命令
        /// </summary>
        /// <param name="cmd">命令</param>
        /// <returns>执行结果</returns>
        public static string RunCmd(string cmd)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments = "/c " + cmd;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                string str = process.StandardOutput.ReadToEnd();
                process.Dispose();
                return str;
            }
            catch { return ""; }
        }
        /// <summary>
        /// 结束服务进程
        /// </summary>
        /// <param name="imagename"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static string TaskKillService(string imagename, string user, string password, string ip)
        {
            var process = new Process();
            process.StartInfo.FileName = "taskkill.exe";
            process.StartInfo.Arguments = string.Format(" /s {0} /f /t /im \"{1}\" /U {2} /P {3}", ip, imagename, user, password);
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            //process.StartInfo.StandardOutputEncoding = Encoding.UTF8;

            //process.OutputDataReceived += (s, e) =>
            //{
            //    ret += e.Data;
            //};
            //process.ErrorDataReceived += (s, e) =>
            //{
            //    ret += e.Data;
            //};
            //process.BeginOutputReadLine();
            //process.BeginErrorReadLine();
            process.Start();

            string ret = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            process.Close();
            return ret;
        }
        #endregion

        #region  创建进程 
        /// <summary>
        /// 创建进程
        /// </summary>
        /// <param name="process">进程对象</param>
        /// <param name="isKillOld">是否关闭已有进程</param>
        /// <returns>成功返回true，失败返回false</returns>
        public static bool StartProcess(ProcessBaseInfo process, bool isKillOld = false)
        {
            Process TheStartProcess;
            try
            {
                if (isKillOld)
                    CloseProcessByName(process.ProcessName.Replace(Path.GetExtension(process.ProcessName), ""));
                if (string.IsNullOrEmpty(process.StartMode))
                    TheStartProcess = Process.Start(process.ProcessPath);
                else
                {
                    if (File.Exists(process.StartMode))
                        TheStartProcess = Process.Start(process.StartMode, process.ProcessPath);
                    else
                        TheStartProcess = Process.Start(process.ProcessPath);
                }
            }
            catch { return false; }
            return true;
        }

        /// <summary>
        /// 创建多个进程
        /// </summary>
        /// <param name="process">进程集合</param>
        /// <param name="isKillOld">是否关闭已有进程</param>
        /// <returns>进程集合包含操作状态</returns>
        public static List<ProcessBaseInfo> StartProcess(List<ProcessBaseInfo> process, bool isKillOld = false)
        {
            if (isKillOld)
            {
                if (process.Count > 0) foreach (var item in process)
                        CloseProcessByName(item.ProcessName.Replace(Path.GetExtension(item.ProcessName), ""));
            }
            Process TheStartProcess;
            if (process.Count > 0)
            {
                foreach (var item in process)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(item.StartMode))
                            TheStartProcess = Process.Start(item.ProcessPath);
                        else
                        {
                            if (File.Exists(item.StartMode))
                                TheStartProcess = Process.Start(item.StartMode, item.ProcessPath);
                            else
                                TheStartProcess = Process.Start(item.ProcessPath);
                        }
                        item.HasExited = false;
                        Thread.Sleep(100);
                    }
                    catch { item.HasExited = true; }
                }
            }
            return process;
        }
        #endregion

        #region  检查进程是否在运行 
        /// <summary>
        /// 检查进程是否在运行
        /// </summary>
        /// <param name="processName">进程名称，非路径不包含后缀</param>
        /// <returns>运行返回true，为运行返回false</returns>
        public static bool ProcessExists(string processName)
        {
           //Process[] sss= Process.GetProcesses();
            Process[] MyProcess = Process.GetProcessesByName(processName);
            if (MyProcess.Length > 0) return true;
            else return false;
        }
        /// <summary>
        /// 检查一组进程是否在运行
        /// </summary>
        /// <param name="process">进程集合</param>
        /// <returns>进程集合包含进程状态</returns>
        public static List<ProcessBaseInfo> ProcessExists(List<ProcessBaseInfo> process)
        {
            if (process.Count > 0) foreach (var item in process)
                {
                    Process[] MyProcessS = Process.GetProcessesByName(item.ProcessName);
                    if (MyProcessS.Length > 0)
                        item.HasExited = false;
                    else
                        item.HasExited = true;
                }

            return process;
        }
        #endregion

        #region  进程的cpu使用率 
        /// <summary>
        /// 进程的cpu使用率
        /// </summary>
        /// <param name="processName">进程的名称</param>
        /// <returns>string</returns>
        public static string GetProcessRate(string processName)
        {
            string result = string.Empty;
            try
            {
                PerformanceCounter pfc = new PerformanceCounter() { CategoryName = "Process" /* 指定获取计算机进程信息*/, CounterName = "% Processor Time" /* 占有率*/, InstanceName = processName /* 指定进程   */, MachineName = "." };      // 性能计数器
                result = Math.Round(pfc.NextValue(), 2) + "%";
            }
            catch (Exception ex) { }
            return result;
        }
        #endregion

        #region  重新启动进程 
        /// <summary>
        /// 重新启动进程
        /// </summary>
        /// <param name="pName">进程名称</param>
        /// <returns>成功返回true，失败返回false</returns>
        public static bool ResetProcessByPName(ProcessBaseInfo process)
        {
            bool result = false;
            if (null!=process)
            {
                try
                {
                    if (CloseProcessByPName(process.ProcessName))
                        result= StartProcess(process);
                    result = true;
                }
                catch (Exception ex) { }
            }
            return result;
        }
        #endregion

        #region  执行command命令（cmd） 

        /// <summary>
        /// 杀死进程
        /// </summary>
        /// <param name="name">进程名称</param>
        /// <param name="path">保存结果路径</param>
        /// <param name="showResult">是否显示结果</param>
        /// <param name="showCmdWin">执行时显示窗口</param>
        /// <returns>返回true为成功，false为失败</returns>
        public static bool KillProcess(string name, string resultPath = "c:\\result.txt", bool showResult = false, bool showCmdWin = false)
        {
            return RunCmd("taskkill /f /im " + name, resultPath, showResult, showCmdWin);
        }
        /// <summary>
        /// 执行command命令（cmd）
        /// </summary>
        /// <param name="cmd">cmd命令</param>
        /// <param name="path">保存结果路径</param>
        /// <param name="showResult">是否显示结果</param>
        /// <param name="showCmdWin">执行时是不是显示窗口</param>
        /// <param name="arguments">参数</param>
        /// <returns>返回true为成功，false为失败</returns>
        public static bool RunCmd(string cmd, string resultPath = "c:\\result.txt",bool showResult=false,bool showCmdWin=false,string arguments="", DataReceivedEventHandler CmdOutputDataReceived=null)
        {
            try
            {
                if (!string.IsNullOrEmpty(cmd) && !string.IsNullOrEmpty(resultPath))
                {
                    //string pa = ">" + path;
                    Process p = new Process();
                    p.StartInfo.FileName = "cmd.exe";
                    p.StartInfo.Arguments = arguments;
                    p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
                    p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
                    p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
                    p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
                    p.StartInfo.CreateNoWindow = !showCmdWin;//不显示程序窗口  
                    p.StartInfo.Verb = "runas";
                    p.OutputDataReceived += (e,o) => { CmdOutputDataReceived?.BeginInvoke(e, o, null, null); };
                    p.Start();//启动程序
                    StreamWriter cmdWriter = p.StandardInput;
                    p.BeginOutputReadLine();
                    if (!string.IsNullOrEmpty(cmd))
                    {
                        cmdWriter.WriteLine(cmd);
                    }
                    Thread.Sleep(2000);
                    cmdWriter.WriteLine("exit");
                    cmdWriter.Close();
                    //p.StandardInput.WriteLine(cmd + pa);
                    //Thread.Sleep(2000);
                    //p.StandardInput.WriteLine("exit");
                    //p.StandardInput.AutoFlush = true;

                    p.WaitForExit();
                    p.Dispose();
                    if(showResult)
                        Process.Start(resultPath);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 运行cmd命令
        /// 会显示命令窗口
        /// </summary>
        /// <param name="cmdExe">指定应用程序的完整路径</param>
        /// <param name="cmdStr">执行命令行参数</param>
        public static bool RunCmd(string cmdExe, string cmdStr)
        {
            bool result = false;
            try
            {
                using (Process myPro = new Process())
                {
                    //指定启动进程是调用的应用程序和命令行参数
                    ProcessStartInfo psi = new ProcessStartInfo(cmdExe, cmdStr);
                    myPro.StartInfo = psi;
                    myPro.Start();
                    myPro.WaitForExit();
                    result = true;
                }
            }
            catch
            {

            }
            return result;
        }

        #endregion

        #region  程序启动检查
        /// <summary>
        /// 程序启动检查
        /// </summary>
        /// <returns>true:已启动 false:未启动</returns>
        public static bool CheckAppIsStart()
        {
            Process[] tProcess = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);
            if (tProcess.Length > 1)
            {
                return true;
                //new Forms.FrmMsg("程序已经启动过一个实例,\n请关闭后再次重启程序！", Color.Blue, Color.SkyBlue).Show();
                //Application.Exit();
            }
            return false;
        }
        #endregion

        #region  获取任务栏显示的进程 
        /// <summary>
        /// 获取任务栏显示的进程
        /// </summary>
        private static Process[] GetWindowsInfo()
        {

            return Process.GetProcesses();

        }
        #endregion

        #region  获取指定进程的父进程名称 
        /// <summary>
        /// 获取指定进程的父进程名称
        /// </summary>
        /// <param name="p">要获取的进程</param>
        /// <returns>返回父进程名称</returns>
        public static string GetPrentProcessName(Process p)
        {
            PerformanceCounter performanceCounter = new PerformanceCounter("Process", "Creating Process ID", p.ProcessName);

            //得到父进程
            Process patent = Process.GetProcessById((int)performanceCounter.NextValue());
            return patent.ProcessName;
        }
        #endregion

        #region  主程序退出
        /// <summary>
        /// 主程序自毁
        /// </summary>
        public static void MainDelMyself()
        {
            //string bat = @"@echo off
            //            :tryagain
            //            del %1
            //            if exist %1 goto tryagain
            //            del %0";
            //File.WriteAllText("killme.bat", bat);//写bat文件
            //ProcessStartInfo psi = new ProcessStartInfo();
            //psi.FileName = "killme.bat";
            //psi.Arguments = "/" + Environment.GetCommandLineArgs()[0] + " / ";
            //psi.WindowStyle = ProcessWindowStyle.Hidden;
            //Process.Start(psi);
            string s = Process.GetCurrentProcess().MainModule.FileName;
            Process.Start("Cmd.exe", "/c del /f /s /q " + s);
            Process.GetCurrentProcess().Kill();

        }
        /// <summary>
        /// 删除程序自身
        /// </summary>
        public static void DeleteItselfByCMD()
        {
            ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", "/C ping 1.1.1.1 -n 1 -w 5000 > Nul & Del " + Process.GetCurrentProcess().MainModule.FileName);
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.CreateNoWindow = true;
            Process.Start(psi);
            Process.GetCurrentProcess().Kill();
        }
        ///// <summary>
        ///// 删除程序自身
        ///// </summary>
        //public static void DeleteItself()
        //{
        //    string vBatFile = Path.GetDirectoryName(Application.ExecutablePath) + "\\DeleteItself.bat";
        //    using (StreamWriter vStreamWriter = new StreamWriter(vBatFile, false, Encoding.Default))
        //    {
        //        vStreamWriter.Write(string.Format(
        //            ":del\r\n" +
        //            " del \"{0}\"\r\n" +
        //            "if exist \"{0}\" goto del\r\n" +
        //            "del %0\r\n", Application.ExecutablePath));
        //    }

        //    //************ 执行批处理
        //    WinExec(vBatFile, 0);
        //    //************ 结束退出
        //    Application.Exit();
        //}


        //[System.Runtime.InteropServices.DllImport("kernel32.dll")]
        //public static extern uint WinExec(string lpCmdLine, uint uCmdShow);
        /// <summary>
        /// 主程序退出
        /// </summary>
        public static void MainKillMyself()
        {
            string s = Process.GetCurrentProcess().MainModule.FileName;
            Process.Start("Cmd.exe", "taskkill /f /im " + s);
            Process.GetCurrentProcess().Kill();
        }
        /// <summary>
        /// 程序退出
        /// </summary>
        /// <param name="exeName">进程名称</param>
        public static void MainKillMyself(string exeName)
        {
            Process.Start("Cmd.exe", "taskkill /f /im   " + exeName);
            Process.GetCurrentProcess().Kill();
        }
        /// <summary>
        /// 主程序退出
        /// </summary>
        public static void MainExit()
        {
            Environment.Exit(0);
        }
        #endregion

        #region 正在运行的本实例
        /// <summary>
        /// 正在运行的本实例
        /// </summary>
        /// <returns>返回当前已运行的本实例，无则返回null。</returns>
        public static Process RunningInstance()
        {

            Process current = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(current.ProcessName);
            //循环通过以相同的名称运行进程
            foreach (Process process in processes)
            {
                //忽略当前进程   
                if (process.Id != current.Id)
                {
                    //确保过程是从EXE文件运行。  
                    if (Assembly.GetExecutingAssembly().Location.Replace("/", "\\") == current.MainModule.FileName)
                    {
                        //返回其他进程实例。
                        return process;
                    }
                }
            }
            //没有其他实例被发现，返回空。
            return null;
        }
        #endregion

        #region 给指定进程分配执行的cpu核心

        private static int cpuCount = 0;
        private static IntPtr halfLeft;
        private static IntPtr halfRight;
        private static IntPtr allCPU;
        private static void GetAllCpu(IntPtr processorAffinity)
        {
            cpuCount = 0;
            allCPU = processorAffinity;
            int affinity = processorAffinity.ToInt32();
            while (affinity > 0)
            {
                cpuCount++;
                affinity >>= 1;
            }
            halfLeft = (IntPtr)(allCPU.ToInt32() >> (cpuCount / 2));
            halfRight = (IntPtr)(allCPU.ToInt32() ^ halfLeft.ToInt32());
        }
        /// <summary>
        /// 将目标进程分配给前半部分的CPU核心
        /// 例如某机器有4个核心，则分配0，1号核心
        /// </summary>
        /// <param name="process">目标进程</param>
        public static void SetHalfLeft(Process process)
        {
            if (cpuCount == 0)
                GetAllCpu(process.ProcessorAffinity);
            process.ProcessorAffinity = halfLeft;
        }
        /// <summary>
        /// 将目标进程分配给后半部分的CPU核心
        /// 例如某机器有4个核心，则分配2，3号核心
        /// </summary>
        /// <param name="process"></param>
        public static void SetHalfRight(Process process)
        {
            if (cpuCount == 0)
                GetAllCpu(process.ProcessorAffinity);
            process.ProcessorAffinity = halfRight;
        }
        /// <summary>
        /// 将目标进程分配给所有的CPU核心
        /// 例如某机器有4个核心，则分配0，1，2，3号核心
        /// </summary>
        /// <param name="process"></param>
        public static void SetAll(Process process)
        {
            if (cpuCount == 0)
                throw new Exception("在使用另外两个方法之前就使用分配所有CPU的方法");
            GetAllCpu(process.ProcessorAffinity);
            process.ProcessorAffinity = allCPU;
        }
        #endregion

        #region 判断进程是否有响应


        /// <summary>
        /// 根据窗口句柄获得进程PID和线程PID
        /// </summary>
        /// <param name="hwnd">句柄</param>
        /// <param name="pid">返回 进程PID</param>
        /// <returns>方法的返回值，线程PID，进程PID和线程PID这两个概念不同</returns>
        public static int GetPidByHwnd(int hwnd, out int pid)
        {
            return GetWindowThreadProcessId((IntPtr)hwnd, out pid);
        }
        /// <summary>
        /// 根据窗口句柄获得进程PID和线程PID
        /// </summary>
        /// <param name="hwnd">句柄</param>
        /// <param name="pid">返回 进程PID</param>
        /// <returns>方法的返回值，线程PID，进程PID和线程PID这两个概念不同</returns>
        public static int GetPidByHwnd(IntPtr hwnd, out int pid)
        {
            return GetWindowThreadProcessId(hwnd, out pid);
        }
        /// <summary>
        /// 检查进程是否有响应
        /// </summary>
        /// <returns>true:有响应 false:无响应</returns>
        public static bool CheckAlive()
        {
            //定义一个int类型变量存放句柄
            IntPtr Hwnd = Process.GetCurrentProcess().Handle;

            //定义一个process对象
            Process LoginPos = null;

            //定义一个int类型变量存放取得的进程PID
            int LoginPid;

            //GetPidByHwnd方法也在宝典内，通过句柄获得进程PID
            int XcPid = GetPidByHwnd(Hwnd, out LoginPid);

            //下面方法是获得对应PID的Process对象（方法比较简陋，有更好的方法请提供）
            Process[] pp = Process.GetProcesses();
            for (int pd = 0; pd < pp.Length; pd++)
            {
                if (pp[pd].Id == LoginPid)
                {
                    LoginPos = pp[pd];
                }
            }

            //关键代码在这
            if (!LoginPos.Responding)//利用Responding属性判断进程是否有响应
            {
                //进程无响应
                return false;
            }
            else
            {
                //进程有响应
                return true;
            }
        }
        /// <summary>
        /// 检查进程是否有响应
        /// </summary>
        /// <param name="name">进程名</param>
        /// <returns>true:有响应 false:无响应</returns>
        public static bool CheckAlive(string name)
        {
            //定义一个int类型变量存放句柄
            //IntPtr Hwnd = Process.GetCurrentProcess().Handle;
            Process[] ps=Process.GetProcessesByName(name);
            if (ps.Length == 0) return false;
            return ps[0].Responding;
        }
        #endregion

        #region  进程信息对象 
        /// <summary>
        /// 进程基本属性对象
        /// </summary>
        public class ProcessBaseInfo
        {
            /// <summary>
            /// 进程的名称
            /// </summary>
            public string ProcessName { get; set; }
            /// <summary>
            /// 进程路径
            /// </summary>
            public string ProcessPath { get; set; }
            /// <summary>
            /// 进程是否已终止的值（值为 true 指示关联的进程已经正常或异常终止）
            /// </summary>
            public bool HasExited { get; set; }
            /// <summary>
            /// 打开方式
            /// </summary>
            public string StartMode { get; set; }
        }
        ///// <summary>
        ///// 进程常用信息对象
        ///// </summary>
        //public class ProcessInfo
        //{
        //    /// <summary>
        //    /// 进程的唯一标识符
        //    /// </summary>
        //    public int id { get; set; }
        //    /// <summary>
        //    /// 进程的名称
        //    /// </summary>
        //    public string ProcessName { get; set; }
        //    /// <summary>
        //    /// 进程路径
        //    /// </summary>
        //    public string ProcessPath { get; set; }
        //    /// <summary>
        //    /// 进程是否已终止的值（值为 true 指示关联的进程已经正常或异常终止）
        //    /// </summary>
        //    public bool HasExited { get; set; }
        //    /// <summary>
        //    /// 进程启动的时间
        //    /// </summary>
        //    public DateTime StartTime { get; set; }
        //    /// <summary>
        //    /// 进程退出的时间
        //    /// </summary>
        //    public DateTime ExitTime { get; set; }
        //    /// <summary>
        //    /// 进程的终端服务会话标识符
        //    /// </summary>
        //    public string SessionId { get; set; }
        //    /// <summary>
        //    /// 进程的总的处理器时间
        //    /// </summary>
        //    public TimeSpan TotalProcessorTime { get; set; }
        //    /// <summary>
        //    /// 打开方式
        //    /// </summary>
        //    public string StartMode { get; set; }
        //}

        #endregion

        #region 通过句柄获得进程路径
        /// <summary>
        /// 通过句柄获得进程路径
        /// </summary>
        /// <param name="hwnd">句柄</param>
        /// <returns>返回 进程路径 找不到返回空</returns>
        public static string GetAppRunPathByHandle(int hwnd)
        {
            Process[] ps = Process.GetProcesses();
            foreach (Process p in ps)
            {
                if ((int)p.MainWindowHandle == hwnd)
                {
                    string path = p.MainModule.FileName;
                    return path;
                }
            }
            return null;
        }
        #endregion

        /// <summary>
        /// 根据窗口标题查找窗口进程PID-返回多个
        /// </summary>
        /// <param name="title">窗口标题</param>
        /// <returns>返回 进程PID 多个以"|"隔开 找不到返回 ""</returns>
        public static string FindPidByTitleEx(string title)
        {
            string Pid = "";
            Process[] arrayProcess = Process.GetProcesses();
            foreach (Process p in arrayProcess)
            {
                if (p.MainWindowTitle.IndexOf(title) != -1)
                {
                    if (Pid == "")
                    {
                        Pid = p.Id.ToString();
                    }
                    else
                    {
                        Pid = string.Format("{0}|{1}", Pid, p.Id);
                    }
                }
            }
            return Pid;
        }
        /// <summary>
        /// 根据窗口标题获得进程Process对象的集合
        /// </summary>
        /// <param name="Title">窗口标题</param>
        /// <param name="Pro">进程Process 对象集合</param>
        /// <returns>找不到返回 false</returns>
        public static bool GetProcessExByTitle(string Title, ref List<Process> Pro)
        {
            bool finded = false;
            Process[] arrayProcess = Process.GetProcesses();
            foreach (Process p in arrayProcess)
            {
                if (p.MainWindowTitle.IndexOf(Title) != -1)
                {
                    finded = true;
                    Pro.Add(p);
                }
            }
            if (finded)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 打开一个exe程序
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="par">参数</param>
        /// <param name="closeEvent">关闭事件</param>
        /// <returns>进程对象，用于关闭释放</returns>
        public static Process OpenExe(string path, string par,EventHandler closeEvent,bool createNoWindow=true)
        {
            //指定启动进程时使用的诸如应用程序或文档的文件名
            ProcessStartInfo start = new ProcessStartInfo(path);
            //获取或设置要启动的进程的初始目录
            start.WorkingDirectory = Path.GetDirectoryName(path);
            //获取或设置启动应用程序时要使用的一组命令行参数
            start.Arguments = par;
            //获取或设置指示是否在新窗口中启动该进程的值
            start.CreateNoWindow = createNoWindow;
            //获取或设置一个值，该值指示是否将应用程序的输出写入System.Diagnostics.Process.StandardOutput流中
            start.RedirectStandardOutput = false;
            //获取或设置一个值，该值指示应用程序的输入是否从 System.Diagnostics.Process.StandardInput流中读取
            start.RedirectStandardInput = false;
            //获取或设置一个值，该值指示是否使用操作系统 shell 启动进程
            start.UseShellExecute = false;
            //启动由包含进程启动信息（例如，要启动的进程的文件名）的参数指定的进程资源，并将该资源与新的 System.Diagnostics.Process组件关联。
            Process p1 = Process.Start(start);
            p1.EnableRaisingEvents = true;
            p1.Exited += (e,o)=> { closeEvent?.Invoke(e,o); };
            //指示 System.Diagnostics.Process 组件在20秒内等待关联进程退出
            //p1.WaitForExit(20000);
            //p1.Close();
            //p1.Dispose();
            return p1;
        }
        /// <summary>
        /// 是否存在进程
        /// </summary>
        /// <param name="pName">进程名（不含后缀）</param>
        /// <param name="kill">是否关闭</param>
        /// <returns></returns>
        public static bool ProcessExitis(string pName, bool kill=false)
        {
            bool bo = false;
            Process[] processList = Process.GetProcesses();
            for (int i = 0; i < processList.Length; i++)
            {
                if (processList[i].ProcessName.ToLower() == pName.ToLower())
                {
                    if (kill)
                    {
                        bo = false;

                        processList[i].Kill(); //结束进程 
                    }
                    else
                    {
                        bo = true;
                    }
                }
            }
            return bo;
        }

        //// [StructLayout(LayoutKind.Sequential)]
        //private struct ProcessBasicInformation
        //{
        //    public int ExitStatus;
        //    public int PebBaseAddress;
        //    public int AffinityMask;
        //    public int BasePriority;
        //    public uint UniqueProcessId;
        //    public uint InheritedFromUniqueProcessId;
        //}

        //[DllImport("ntdll.dll")]
        //static extern int NtQueryInformationProcess(
        //   IntPtr hProcess,
        //   int processInformationClass /* 0 */,
        //   ref ProcessBasicInformation processBasicInformation,
        //   uint processInformationLength,
        //   out uint returnLength
        //);
        ///// <summary>
        ///// 查杀进程以及子进程
        ///// </summary>
        ///// <param name="parent"></param>
        //public static void KillProcessTree(this Process parent)
        //{
        //    var processes = Process.GetProcesses();
        //    foreach (var p in processes)
        //    {
        //        var pbi = new ProcessBasicInformation();
        //        try
        //        {
        //            uint bytesWritten;
        //            if (NtQueryInformationProcess(p.Handle, 0, ref pbi, (uint)Marshal.SizeOf(pbi), out bytesWritten) == 0) // == 0 is OK
        //                if (pbi.InheritedFromUniqueProcessId == parent.Id)
        //                    using (var newParent = Process.GetProcessById((int)pbi.UniqueProcessId))
        //                        newParent.KillProcessTree();
        //        }
        //        catch { }
        //    }
        //    parent.Kill();
        //}
    }
}