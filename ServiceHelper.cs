
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Text.RegularExpressions;
namespace SuperFramework
{
    /// <summary>
    /// 服务操作方法集合
    /// </summary>
    public class ServiceHelper
    {
//        #region  安装服务，非InstallUtil.exe方式。 
//        /// <summary>
//        /// 安装服务，非InstallUtil.exe方式。
//        /// </summary>
//        /// <param name="serviceName">服务名称</param>
//        /// <returns>成功返回true，失败返回false</returns>
//        public static bool InstallService(string serviceName)
//        {
//            string[] args = { "HP.Travel_Mail_Service.exe" };//要安装的服务文件（就是用 InstallUtil.exe 工具安装时的参数）
//            ServiceController svcCtrl = new ServiceController(serviceName);
//            if (!ServiceExist(serviceName))
//            {
//                try
//                {
//                    System.Configuration.Install.ManagedInstallerClass.InstallHelper(args);
//                    return true;
//                }
//                catch
//                {
//                    throw;
//                }
//            }
//            else
//            {
//                return false;
//            }
//        }
//        #endregion

//        #region  卸载服务，非InstallUtil.exe方式。 
//        /// <summary>
//        /// 卸载服务，非InstallUtil.exe方式。
//        /// </summary>
//        /// <param name="serviceName">服务名称</param>
//        /// <returns>成功返回true，失败返回false</returns>
//        public static bool UnInstallService(string serviceName)
//        {
//            try
//            {
//                if (ServiceExist(serviceName))
//                {
//                    //UnInstall Service  
//                    System.Configuration.Install.AssemblyInstaller myAssemblyInstaller = new System.Configuration.Install.AssemblyInstaller();
//                    myAssemblyInstaller.UseNewContext = true;
//                    myAssemblyInstaller.Path = "HP.Travel_Mail_Service.exe";
//                    myAssemblyInstaller.Uninstall(null);
//                    myAssemblyInstaller.Dispose();
//                    return true;
//                }
//                else
//                {
//                    return false;
//                }
//            }
//            catch
//            {
//                throw;
//            }

//        }
//        #endregion

//        #region  安装服务 
//        /// <summary>
//        /// 安装服务
//        /// </summary>
//        /// <param name="filepath">指定服务文件路径</param>
//        /// <param name="serviceName">服务名</param>
//        public static void InstallService(string filepath, string serviceName)
//        {
//            try
//            {
//                IDictionary stateSaver = new Hashtable();
//                ServiceController service = new ServiceController(serviceName);
//                if (!ServiceExist(serviceName))
//                {
//                    //Install Service
//                    AssemblyInstaller myAssemblyInstaller = new AssemblyInstaller() { UseNewContext = true, Path = filepath };
//                    myAssemblyInstaller.Install(stateSaver);
//                    myAssemblyInstaller.Commit(stateSaver);
//                    myAssemblyInstaller.Dispose();
//                    //--Start Service
//                    service.Start();
//                }

//                else
//                    if (service.Status != ServiceControllerStatus.Running && service.Status != ServiceControllerStatus.StartPending)
//                    service.Start();
//            }
//            catch
//            {
//                throw;
//            }
//        }
//#if NET40_OR_GREATER
//        /// <summary>
//        /// 安装服务
//        /// </summary>
//        /// <param name="path">指定服务文件路径</param>
//        /// <param name="serviceName">返回安装完成后的服务名</param>
//        /// <returns>安装成功返回true，失败返回false</returns>
//        public static bool InsertService(string path, ref string serviceName)
//        {
//            if (!File.Exists(path)) return false;
//            serviceName = "";
//            FileInfo insertFile = new FileInfo(path);
//            IDictionary savedState = new Hashtable();
//            try
//            {
//                //加载一个程序集，并运行其中的所有安装程序。
//                AssemblyInstaller assemblyInstaller = new AssemblyInstaller(path, new string[] { string.Format("/LogFile={0}//{1}.log", insertFile.DirectoryName, insertFile.Name.Substring(0, insertFile.Name.Length - insertFile.Extension.Length)) }) { UseNewContext = true };
//                assemblyInstaller.Install(savedState);
//                assemblyInstaller.Commit(savedState);
//                Type[] typeList = assemblyInstaller.Assembly.GetTypes();//获取安装程序集类型集合
//                for (int i = 0; i != typeList.Length; i++)
//                {
//                    if (typeList[i].BaseType.FullName == "System.Configuration.Install.Installer")
//                    {
//                        //找到System.Configuration.Install.Installer 类型
//                        object _InsertObject = System.Activator.CreateInstance(typeList[i]);//创建类型实列
//                        FieldInfo[] _FieldList = typeList[i].GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
//                        for (int z = 0; z != _FieldList.Length; z++)
//                        {
//                            if (_FieldList[z].FieldType.FullName == "System.ServiceProcess.ServiceInstaller")
//                            {
//                                object _ServiceInsert = _FieldList[z].GetValue(_InsertObject);
//                                if (_ServiceInsert != null)
//                                    serviceName = ((ServiceInstaller)_ServiceInsert).ServiceName;
//                            }
//                        }
//                    }
//                }
//                return true;
//            }
//            catch
//            {
//                return false;
//            }
//        }
//#endif
//        #endregion

        //#region  卸载服务 
        ///// <summary>
        ///// 卸载服务
        ///// </summary>
        ///// <param name="filepath">服务路径</param>
        ///// <param name="serviceName">服务名称</param>
        ///// <returns>安装成功返回true，失败返回false</returns>
        //public static bool UnInstallService(string filepath, string serviceName)
        //{
        //    try
        //    {
        //        if (ServiceExist(serviceName))
        //        {
        //            //UnInstall Service
        //            AssemblyInstaller myAssemblyInstaller = new AssemblyInstaller() { UseNewContext = true, Path = filepath };
        //            myAssemblyInstaller.Uninstall(null);
        //            myAssemblyInstaller.Dispose();
        //        }
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //    return true;
        //}
        //#endregion

        #region  启动服务 
        /// <summary>
        /// 启动服务，尝试多次。
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <param name="tryNum">尝试次数</param>
        /// <returns>成功返回true，失败返回false</returns>
        public static bool StartServiceTry(string serviceName, uint tryNum = 5)
        {
            bool state = false;
            if (ServiceExist(serviceName))
            {
                ServiceController service = new ServiceController(serviceName);
                if (service.Status != ServiceControllerStatus.Running && service.Status != ServiceControllerStatus.StartPending)
                {
                    service.Start();
                    for (int i = 0; i < tryNum; i++)
                    {
                        service.Refresh();
                        System.Threading.Thread.Sleep(1000);
                        if (service.Status == ServiceControllerStatus.Running)
                        {
                            state = true;
                            break;
                        }
                    }
                }

            }
            return state;
        }
        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <param name="timeOut">超时时间 秒</param>
        /// <returns>成功返回true，失败返回false</returns>
        public static bool ServiceStart(string serviceName, int timeOut = 10)
        {
            try
            {
                ServiceController service = new ServiceController(serviceName);
                if (service.Status == ServiceControllerStatus.Running)
                {
                    return true;
                }
                else
                {
                    TimeSpan timeout = TimeSpan.FromMilliseconds(1000 * timeOut);
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion

        #region  停止服务 
        /// <summary>
        /// 停止服务
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <param name="tryNum">尝试次数</param>
        /// <returns>成功返回true，失败返回false</returns>
        public static bool StopServiceTry(string serviceName, int tryNum = 5)
        {
            bool state = false;
            if (ServiceExist(serviceName))
            {
                ServiceController service = new ServiceController(serviceName);
                if (service.Status == ServiceControllerStatus.Running)
                {
                    service.Stop();
                    for (int i = 0; i < tryNum; i++)
                    {
                        service.Refresh();
                        System.Threading.Thread.Sleep(1000);
                        if (service.Status == ServiceControllerStatus.Stopped)
                        {
                            state = true;
                            break;
                        }
                    }
                }
            }
            return state;
        }
        /// <summary>
        /// 停止服务
        /// </summary>
        /// <param name="serviseName">服务名称</param>
        /// <param name="timeOut">超时时间 秒</param>
        /// <returns>成功返回true，失败返回false</returns>
        public static bool ServiceStop(string serviseName, int timeOut = 10)
        {
            try
            {
                ServiceController service = new ServiceController(serviseName);
                if (service.Status == ServiceControllerStatus.Stopped)
                {
                    return true;
                }
                else
                {
                    TimeSpan timeout = TimeSpan.FromMilliseconds(1000 * timeOut);
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion

        #region  修改服务的启动模式 
        /// <summary>
        /// 修改服务的启动模式 2为自动,3为手动
        /// </summary>
        /// <param name="startType">启动方式 2为自动,3为手动</param>
        /// <param name="serviceName">服务名称</param>
        /// <returns>成功返回true，失败返回false</returns>
        public static bool ChangeServiceStartType(int startType, string serviceName)
        {
            try
            {
                RegistryKey regist = Registry.LocalMachine;
                RegistryKey sysReg = regist.OpenSubKey("SYSTEM");
                RegistryKey currentControlSet = sysReg.OpenSubKey("CurrentControlSet");
                RegistryKey services = currentControlSet.OpenSubKey("Services");
                RegistryKey servicesName = services.OpenSubKey(serviceName, true);
                servicesName.SetValue("Start", startType);
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion

        #region  获取服务启动类型 
        /// <summary>
        /// 获取服务启动模式 2为自动 3为手动 4 为禁用
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns>返回 2为自动 3为手动 4 为禁用</returns>
        public static string GetServiceStartType(string serviceName)
        {
            try
            {
                RegistryKey regist = Registry.LocalMachine;
                RegistryKey sysReg = regist.OpenSubKey("SYSTEM");
                RegistryKey currentControlSet = sysReg.OpenSubKey("CurrentControlSet");
                RegistryKey services = currentControlSet.OpenSubKey("Services");
                RegistryKey servicesName = services.OpenSubKey(serviceName, true);
                return servicesName.GetValue("Start").ToString();
            }
            catch
            {
                return string.Empty;
            }
        }
        #endregion

        #region  验证服务是否启动 
        /// <summary>
        /// 验证服务是否启动
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns>启动返回true，未启动返回false</returns>
        public static bool ServiceIsRunning(string serviceName)
        {
            ServiceController service = new ServiceController(serviceName);
            if (service.Status == ServiceControllerStatus.Running)
                return true;
            else
                return false;
        }
        #endregion

        #region  服务是否存在 
        /// <summary>
        /// 服务是否存在
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns>启动返回true，未启动返回false</returns>
        public static bool ServiceExist(string serviceName)
        {
            try
            {
                string m_ServiceName = serviceName;
                ServiceController[] services = ServiceController.GetServices();
                foreach (ServiceController s in services)
                    if (s.ServiceName.Trim() == m_ServiceName.Trim())//s.ServiceName.ToLower() == m_ServiceName.ToLower()
                        return true;
                return false;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        /// <summary>
        /// 获取系统所有服务对象
        /// </summary>
        /// <returns></returns>
        public ArrayList GetAllsystemServices()
        {
            ArrayList arryServices = new ArrayList();
            ServiceController[] services = ServiceController.GetServices();
            foreach (ServiceController a in services)
            {
                arryServices.Add(a);
            }

            return arryServices;
        }
        /// <summary>
        /// 获取所有服务名
        /// </summary>
        /// <returns>返回服务名集合</returns>
        public List<string> GetAllServiceNames()
        {
            List<string> listNames = new List<string>();
            ServiceController[] services = ServiceController.GetServices();
            foreach (ServiceController a in services)
            {
                listNames.Add(a.DisplayName);
            }
            return listNames;
        }
        /// <summary>
        /// 设置指定服务
        /// </summary>
        /// <param name="serviceName">服务显示名</param>
        /// <param name="oper">操作类型</param>
        /// <returns>true 设置成功，false设置失败</returns>
        public static bool SetService(string serviceName, ServiceControllerStatus oper)
        {
            bool isok = false;
            ServiceController[] allServices = ServiceController.GetServices();
            foreach (ServiceController sc in allServices)
            {
                if (sc.ServiceName.Trim() == serviceName.Trim())//sc.DisplayName.Trim() == serviceName.Trim()
                {
                    switch (oper)
                    {
                        case ServiceControllerStatus.ContinuePending:
                            sc.Continue();
                            if (sc.Status == ServiceControllerStatus.Running) isok = true;
                            break;
                        case ServiceControllerStatus.PausePending:
                        case ServiceControllerStatus.Paused:
                            sc.Pause();
                            if (sc.Status == ServiceControllerStatus.Paused) isok = true;
                            break;
                        case ServiceControllerStatus.Running:
                        case ServiceControllerStatus.StartPending:
                            sc.Start();
                            if (sc.Status == ServiceControllerStatus.Running) isok = true;
                            break;
                        case ServiceControllerStatus.StopPending:
                        case ServiceControllerStatus.Stopped:
                            sc.Stop();
                            if (sc.Status == ServiceControllerStatus.Stopped) isok = true;
                            break;
                        default:
                            break;
                    }
                }
            }
            return isok;
        }
        /// <summary>
        /// 检测服务是否运行
        /// </summary>
        /// <param name="serviceName">服务名</param>
        /// <returns>运行返回true，否则返回false</returns>
        public static bool CheckServiceIsRunning(string serviceName)
        {
            ServiceController[] allServices = ServiceController.GetServices();
            bool runing = false;
            foreach (ServiceController sc in allServices)
            {
                if (sc.ServiceName.Trim() == serviceName.Trim())//if (sc.DisplayName.Trim() == serviceName.Trim())
                {
                    if (sc.Status == ServiceControllerStatus.Running)
                    {
                        runing = true;
                    }
                }
            }
            return runing;
        }
        /// <summary>
        /// 获取服务状态
        /// </summary>
        /// <param name="serviceName">服务显示名</param>
        /// <returns>返回服务状态</returns>
        public static ServiceControllerStatus GetStatue(string serviceName)
        {
            ServiceController[] allServices = ServiceController.GetServices();

            foreach (ServiceController sc in allServices)
            {
                if (sc.ServiceName.Trim() == serviceName.Trim())//sc.DisplayName.Trim() == serviceName.Trim()
                {
                    return sc.Status;
                }
            }
            return ServiceControllerStatus.Stopped;
        }
        /// <summary>
        /// 获取已安装服务的版本信息
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="registryView"></param>
        /// <param name="throwErrorIfNonExisting"></param>
        /// <returns></returns>
        public static string GetExecutablePathForService(string serviceName, RegistryView registryView, bool throwErrorIfNonExisting)
        {
            string registryPath = @"SYSTEM\CurrentControlSet\Services\" + serviceName;
            RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView).OpenSubKey(registryPath);
            if (key == null)
            {
                if (throwErrorIfNonExisting)
                    throw new ArgumentException("Non-existent service: " + serviceName, nameof(serviceName));
                else
                    return null;
            }
            string value = key.GetValue("ImagePath").ToString();
            key.Close();
            if (value.StartsWith("\""))
            {
                value = Regex.Match(value, "\"([^\"]+)\"").Groups[1].Value;
            }

            return Environment.ExpandEnvironmentVariables(value);
        }
        ///// <summary>
        ///// 操作
        ///// </summary>
        //public enum OpertType
        //{
        //    /// <summary>
        //    /// 启动
        //    /// </summary>
        //    Start,
        //    /// <summary>
        //    /// 停止
        //    /// </summary>
        //    Stop,
        //    /// <summary>
        //    /// 暂停
        //    /// </summary>
        //    Pause,
        //    /// <summary>
        //    /// 重启
        //    /// </summary>
        //    ReStart
        //}
    }
}
