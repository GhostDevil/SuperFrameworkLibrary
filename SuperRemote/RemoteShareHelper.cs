
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;

namespace SuperFramework.SuperRemote
{
    /// <summary>
    /// 日 期:2015-04-24
    /// 作 者:不良帥
    /// 描 述:远程访问辅助类
    /// </summary>
    public static class RemoteShareHelper
    {

        #region  登陆服务器 
        /// <summary>
        /// 登陆服务器
        /// </summary>
        /// <param name="serverIp">服务器IP地址</param>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>是否成功登陆</returns>
        public static bool LoginServer(string serverIp, string userName, string password)
        {
            Process pProcess = null;
            try
            {
                pProcess = new Process();
                pProcess.StartInfo.FileName = "cmd.exe";
                pProcess.StartInfo.Arguments = string.Format(@"/cnet use \\{0} {1} /user:{2}", serverIp, password, userName);
                pProcess.StartInfo.UseShellExecute = false;
                pProcess.StartInfo.RedirectStandardInput = true;
                pProcess.StartInfo.RedirectStandardOutput = true;
                pProcess.StartInfo.RedirectStandardError = true;
                pProcess.StartInfo.CreateNoWindow = true;
                pProcess.Start();
                if (pProcess.StandardError.ReadToEnd() != "")
                    return false;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (pProcess != null)
                    pProcess.Dispose(); pProcess.Close();
            }
            return true;
        }
        #endregion

        #region  连接网络远程共享文件夹 
        /// <summary>
        /// 连接网络远程共享文件夹
        /// </summary>
        /// <param name="remoteHost">远程服务器的IP或域名</param>
        /// <param name="shareName">共享名</param>
        /// <param name="userName">远程共享访问帐户的用户名</param>
        /// <param name="passWord">远程共享访问帐户的密码</param>
        /// <returns>连接的结果</returns>    
        /// <exception cref="Exception">未知错误，详见错误参数</exception>    
        public static bool ConnectRemote(string remoteHost, string shareName, string userName, string passWord)
        {
            bool Flag = false;
            Process proc = new();
            try
            {
                proc.StartInfo.FileName = "cmd.exe";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardInput = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
                string dosLine = string.Format(@"net use \\{0}\{1} /User:{2} {3} /PERSISTENT:YES", remoteHost, shareName, userName, passWord);
                proc.StandardInput.WriteLine(dosLine);
                proc.StandardInput.WriteLine("exit");
                while (!proc.HasExited)
                {
                    proc.WaitForExit(1000);
                }

                string errormsg = proc.StandardError.ReadToEnd();
                proc.StandardError.Close();
                if (string.IsNullOrEmpty(errormsg))
                {
                    Flag = true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                proc.Close();
                proc.Dispose();
            }
            return Flag;
        }
        #endregion

        #region  连接共享文件夹 
        /// <summary>
        /// 连接共享文件夹
        /// </summary>
        /// <param name="ipPath">共享地址</param>
        /// <param name="userName">用户名</param>
        /// <param name="passWord">密码</param>
        /// <returns>连接成功返回true，失败返回false</returns>
        public static bool ConnectShare(string ipPath, string userName, string passWord)
        {
            bool Flag = false;
            Process proc = new();
            try
            {
                proc.StartInfo.FileName = "cmd.exe";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardInput = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
                string dosLine = string.Format(@"net use {0} /User:{1} {2} /PERSISTENT:YES", ipPath, userName, passWord);
                proc.StandardInput.WriteLine(dosLine);
                proc.StandardInput.WriteLine("exit");
                while (!proc.HasExited)
                {
                    proc.WaitForExit(1000);
                }
                string errormsg = proc.StandardError.ReadToEnd();
                proc.StandardError.Close();
                if (string.IsNullOrEmpty(errormsg))
                {
                    Flag = true;
                }
                else
                {
                    throw new Exception(errormsg);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                proc.Close();
                proc.Dispose();
            }
            return Flag;
        }
        #endregion

        #region  获取本地所有共享路径 
        /// <summary>
        /// 获取本地所有共享路径
        /// </summary>
        /// <returns>返回共享路径集合</returns>
        public static List<string> GetShareAllPath()
        {
            // 需要手动添加引用 System.Management
            ManagementObjectSearcher searcher = new("select  *  from  win32_share");
            List<string> ps = new();
            foreach (ManagementObject share in searcher.Get())
            {
                try
                {
                    string name = share["Name"].ToString();
                    string path = share["Path"].ToString();
                    ps.Add(path);
                }
                catch
                {
                    continue;
                }
            }
            return ps;
        }
        #endregion

    }
}
