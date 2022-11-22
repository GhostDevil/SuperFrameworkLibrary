using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace SuperFramework.SuperRegistry
{
    /// <summary>
    /// 日 期:2015-04-24
    /// 作 者:不良帥
    /// 描 述:注册表操作辅助方法类
    /// </summary>
    public static class RegistHelper
    {
        #region  读取注册表HKEY_LOCAL_MACHINE/SOFTWARE目录下的XXX目录中名称为name的注册表值 
        /// <summary>
        /// 读取注册表HKEY_LOCAL_MACHINE/SOFTWARE目录下的XXX目录中名称为name的注册表值
        /// </summary>
        /// <param name="name">注册表名称</param>
        /// <returns>返回注册表值</returns>

        public static string GetRegistData(string name)
        {
            string registData;
            RegistryKey aimdir = Registry.LocalMachine.OpenSubKey("SOFTWARE", true);
            registData = aimdir.GetValue(name).ToString();
            aimdir.Close();
            return registData;
        }
        #endregion

        #region  向注册表中HKEY_LOCAL_MACHINE/SOFTWARE目录下新建XXX目录并在此目录下创建名称为name值为tovalue的注册表项 
        /// <summary>
        /// 向注册表中HKEY_LOCAL_MACHINE/SOFTWARE目录下新建XXX目录并在此目录下创建名称为name值为tovalue的注册表项
        /// </summary>
        /// <param name="name">注册表名称</param>
        /// <param name="tovalue">值</param>

        public static void WTRegedit(string name, string tovalue)
        {
            RegistryKey aimdir = Registry.LocalMachine.OpenSubKey("SOFTWARE", true);
            aimdir.SetValue(name, tovalue);
            aimdir.Close();
        }
        #endregion

        #region  删除注册表HKEY_LOCAL_MACHINE/SOFTWARE目录下名称为name注册表项 
        /// <summary>
        /// 删除注册表HKEY_LOCAL_MACHINE/SOFTWARE目录下名称为name注册表项
        /// </summary>
        /// <param name="name">注册表名称</param>

        public static void DeleteRegist(string name)
        {
            string[] aimnames;
            RegistryKey aimdir = Registry.LocalMachine.OpenSubKey("SOFTWARE", true);
            aimnames = aimdir.GetSubKeyNames();
            foreach (string aimKey in aimnames)
            {
                if (aimKey == name)
                    aimdir.DeleteSubKeyTree(name);
            }
            aimdir.Close();
        }
        #endregion

        #region  判断HKEY_LOCAL_MACHINE/SOFTWARE目录下名称为name注册表项是否存在 
        /// <summary>
        /// 判断HKEY_LOCAL_MACHINE/SOFTWARE目录下名称为name注册表项是否存在
        /// </summary>
        /// <param name="name">注册表名称</param>
        /// <returns>true：存在，false：不存在</returns>
        public static bool IsRegeditExit(string name)
        {
            bool _exit = false;
            string[] subkeyNames;
            RegistryKey aimdir = Registry.LocalMachine.OpenSubKey("SOFTWARE", true);
            subkeyNames = aimdir.GetSubKeyNames();
            foreach (string keyName in subkeyNames)
            {
                if (keyName == name)
                {
                    _exit = true;
                    return _exit;
                }
            }
            aimdir.Close();
            return _exit;
        }
        #endregion

        #region  设置应用程序开机是否自动运行 

        /// <summary> 
        /// 设置应用程序开机是否自动运行
        /// </summary>          
        /// <param name="filePath">应用程序的全路径</param>          
        /// <param name="isAutoRun">是否自动运行，为false时，取消自动运行</param>         
        /// <exception cref="Exception">设置不成功时抛出异常</exception>         

        public static void SetAutoRun(string filePath, bool isAutoRun)
        {
            RegistryKey reg = null;
            try
            {
                string s = Path.GetFileNameWithoutExtension(filePath);
                string s2 = Path.GetDirectoryName(filePath);
                string file = s2 +"\\" +s+".exe";//filePath.Substring(0,filePath.IndexOf(".exe")+4);
                if (!System.IO.File.Exists(file))
                    throw new Exception("该程序不存在!");
                string name = filePath.Substring(filePath.LastIndexOf(@"\") + 1);
                reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                if (reg == null)
                    reg = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                if (isAutoRun)
                    reg.SetValue(name, filePath);
                else
                    reg.SetValue(name, false);
              
            }
            catch(Exception ex)
            {
                throw;
            }
            finally
            {
                if (reg != null)
                    reg.Close();
            }
        }
        /// <summary>
        /// 设置程序是否开机运行
        /// </summary>
        /// <param name="started">是否开机运行</param>
        /// <param name="exeName">要运行的EXE程序名称（不要拓展名）</param>
        /// <param name="path">要运行的EXE程序路径</param>
        /// <returns>true：成功，false：失败</returns>
        public static bool SetAutoRun(bool started, string exeName, string path)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);//打开注册表子项
            if (key == null)//如果该项不存在的话，则创建该子项
            {
                key = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
            }
            if (started == true)
            {
                try
                {
                    key.SetValue(exeName, path);//设置为开机启动
                    key.Close();
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                try
                {
                    key.DeleteValue(exeName);//取消开机启动
                    key.Close();
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region 程序是否自动运行
        /// <summary>
        /// 程序是否自动运行
        /// </summary>
        /// <param name="filePath">应用程序的全路径</param>      
        /// <returns>true：自动运行，false：非自动运行</returns>
        /// <exception cref="Exception">获取不成功时抛出异常</exception> 
        public static bool IsAutoRun(string filePath)
        {
            RegistryKey reg = null;
            try
            {
               
                string name = filePath.Substring(filePath.LastIndexOf(@"\") + 1);
                reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                if (reg == null)
                   return false;
                object obj = reg.GetValue(name);
                if (obj == null)
                    return false;
                else
                {
                    if (obj.ToString().ToLower() == "false")
                        return false;
                    else
                        return true;
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (reg != null)
                    reg.Close();
            }
        }
        #endregion

        #region  是否屏蔽开始菜单功能 
        /// <summary>
        /// 开始菜单功能
        /// </summary>
        public enum StartMenu
        {
            /// <summary>
            /// 查找
            /// </summary>
            NoFind,
            /// <summary>
            /// 运行
            /// </summary>
            NoRun,
            /// <summary>
            /// 控制面板和打印机
            /// </summary>
            NoSetFolders,
            /// <summary>
            /// 关闭系统
            /// </summary>
            NoClose,
            /// <summary>
            /// 任务栏和开始菜单
            /// </summary>
            NoSetTaskBar,
            /// <summary>
            /// 注销
            /// </summary>
            NoLogOff,
            /// <summary>
            /// 文件
            /// </summary>
            NoRecentDocsMenu,
            /// <summary>
            /// 我的音乐
            /// </summary>
            NoStartMenuMyMusic,
            /// <summary>
            /// 网上邻居
            /// </summary>
            NoStartMenuNetworkFlaces,
            /// <summary>
            /// 网络连接
            /// </summary>
            NoNetworkConnections




        }
        /// <summary>
        /// 是否屏蔽开始菜单中的功能（重启起作用）
        /// </summary>
        /// <param name="m">开始菜单功能</param>
        /// <param name="isShield">是否屏蔽</param>
        /// <returns>true：成功，false：失败</returns>
        public static bool ShieldStartMenu(StartMenu m, bool isShield = true)
        {
            try
            {
                RegistryKey software = Registry.CurrentUser.OpenSubKey("SOFTWARE", true);
                RegistryKey pcdesk = software.CreateSubKey("Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer");
                if (isShield)
                    pcdesk.SetValue(m.ToString(), 1);
                else
                    pcdesk.SetValue(m.ToString(), 0);
                return true;
            }
            catch { return false; }
        }
        #endregion

        #region  创建快捷启动 
        /// <summary>   
        /// 创建快捷启动   
        /// </summary>   
        /// <param name="name">快捷启动名</param>   
        /// <param name="filePath">文件路径</param>   
        public static void CreateFastStart(string name, string filePath)
        {
            RegistryKey reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths", true);
            RegistryKey key = reg.CreateSubKey(name);
            key.SetValue("", filePath, RegistryValueKind.String);
            string path = filePath.Substring(0, filePath.LastIndexOf(@"\") + 1);
            key.SetValue("Path", path, RegistryValueKind.String);
        }
        #endregion

        #region  检查快捷启动是否存在 

        /// <summary>   
        /// 检查快捷启动是否存在   
        /// </summary>   
        /// <param name="name">快捷启动名</param>   
        /// <returns>true：成功，false：失败</returns>   
        public static bool ExistFastStart(string name)
        {
            RegistryKey reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths");
            string[] subKeys = reg.GetSubKeyNames();
            foreach (string str in subKeys)
            {
                if (str.ToLower().Equals(name.ToLower()))
                    return true;
            }
            return false;
        }
        #endregion

        #region  解禁注册表 
        /// <summary>
        /// 解禁注册表
        /// </summary>
        /// <returns>true：成功，false：失败</returns>

        public static bool EnableRegedit()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\policies\\system", true);//打开注册表子项
            if (key == null)//如果该项不存在的话，则创建该子项
            {
                key = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\policies\\system");
            }
            try
            {
                key.SetValue("disableregistrytools", 0, RegistryValueKind.DWord);
                key.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region  禁用注册表 
        /// <summary>
        /// 禁用注册表
        /// </summary>
        /// <returns>true：成功，false：失败</returns>

        public static bool NotEnableRegedit()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\policies\\system", true);//打开注册表子项
            if (key == null)//如果该项不存在的话，则创建该子项
            {
                key = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\policies\\system");
            }
            try
            {
                key.SetValue("disableregistrytools", 1, RegistryValueKind.DWord);
                key.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region  注册控件 
        /// <summary>
        /// 注册控件
        /// </summary>
        /// <param name="dllIdValue">控件注册后对应的键值</param>
        /// <returns>true：成功，false：失败</returns>

        public static bool RegDll(string dllIdValue)
        {
            try
            {
                RegistryKey key = Registry.ClassesRoot.OpenSubKey(@"CLSTD\" + dllIdValue, true);//打开注册表子项
                if (key == null)//如果该项不存在的话，则创建该子项
                {
                    key = Registry.ClassesRoot.CreateSubKey(@"CLSTD\" + dllIdValue);
                }
                return true;
            }
            catch { return false; }
        }
        #endregion

        #region  解禁任务管理器 
        /// <summary>
        /// 解禁任务管理器
        /// </summary>
        /// <returns>true：成功，false：失败</returns>

        public static bool EnableTaskmgr()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\policies\\system", true);//打开注册表子项
            if (key == null)//如果该项不存在的话，则创建该子项
            {
                key = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\policies\\system");
            }
            try
            {
                key.SetValue("disabletaskmgr", 0, RegistryValueKind.DWord);
                key.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region  禁用任务管理器 
        /// <summary>
        /// 禁用任务管理器
        /// </summary>
        /// <returns>true：成功，false：失败</returns>

        public static bool NotEnableTaskmgr()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\policies\\system", true);//打开注册表子项
            if (key == null)//如果该项不存在的话，则创建该子项
            {
                key = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\policies\\system");
            }
            try
            {
                key.SetValue("disabletaskmgr", 1, RegistryValueKind.DWord);
                key.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region  通过注册表启用USB 
        /// <summary> 
        /// 通过注册表启用USB 
        /// </summary> 
        /// <returns>true：成功，false：失败</returns> 
        public static bool RegToRunUSB()
        {
            bool b;
            try
            {
                RegistryKey regKey = Registry.LocalMachine; //读取注册列表HKEY_LOCAL_MACHINE 
                string keyPath = @"SYSTEM\CurrentControlSet\Services\USBSTOR"; //USB 大容量存储驱动程序 
                RegistryKey openKey = regKey.OpenSubKey(keyPath, true);
                openKey.SetValue("Start", 3); //设置键值对（3）为开启USB（4）为关闭 
                openKey.Close(); //关闭注册列表读写流 
                b = true;
            }
            catch
            {
                throw;
            }
            return b;
        }
        #endregion

        #region  通过注册表禁用USB 
        /// <summary> 
        /// 通过注册表禁用USB 
        /// </summary> 
        /// <returns>true：成功，false：失败</returns> 
        public static bool RegToStopUSB()
        {
            try
            {
                RegistryKey regKey = Registry.LocalMachine;
                string keyPath = @"SYSTEM\CurrentControlSet\Services\USBSTOR";
                RegistryKey openKey = regKey.OpenSubKey(keyPath, true);
                openKey.SetValue("Start", 4);
                openKey.Close();
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion

        #region  获取注册表保存的最后一次窗口位置 
        /// <summary>
        /// 获取注册表保存的最后一次窗口位置，在第一次加载窗体的时候注册表里是读取不到值的，返回null
        /// </summary>
        /// <returns>返回窗口位置坐标</returns>
        public static Point GetStartFormLocation()
        {
            RegistryKey regisOne;
            RegistryKey regisTwo;//声明注册表对象
            regisOne = Registry.CurrentUser;//获取当前用户的注册表基项
            try
            {
                regisTwo = regisOne.CreateSubKey("Software\\MySoft");//在注册表中创建子项
                return new Point(Convert.ToInt16(regisOne.GetValue("不良帥one")), Convert.ToInt16(regisTwo.GetValue("不良帥two")));//设置窗体的显示位置
            }
            catch (Exception ex)
            {

                new Exception(ex.Message);
            }
            return new Point(0, 0);
        }
        #endregion 

        #region  注册窗体关闭时的最后位置 
        /// <summary>
        /// 注册窗体关闭时的最后位置
        /// </summary>
        /// <param name="p">窗体坐标</param>
        /// <returns>true：成功，false：失败</returns>
        public static bool SetEndFormLocation(Point p)
        {
            bool status = false;
            RegistryKey regisOne, regisTwo;//声明注册表对象
            regisOne = Registry.CurrentUser;//获取当前用户的注册表基项
            regisTwo = regisOne.CreateSubKey("Software\\MySoft");//在注册表中创建子项
            try
            {
                regisOne.SetValue("不良帥one", p.X.ToString());//将窗体当前X坐标写入注册表
                regisTwo.SetValue("不良帥two", p.Y.ToString());//将当前窗体的Y坐标写入注册表
                status = true;
            }
            catch (Exception ex)
            {
                new Exception(ex.Message);
            }
            return status;
        }
        #endregion

        #region 获取本机所有应用程序安装信息
        /// <summary>
        /// 获取本机所有应用程序安装信息
        /// </summary>
        /// <returns>返回信息列表</returns>
        public static List<AppInfo> GetInstallAppsInfo()
        {
            List<AppInfo> ObservableObj = new();
            RegistryKey pregkey = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall");//获取指定路径下的键
            try
            {
                foreach (string item in pregkey.GetSubKeyNames())               //循环所有子键
                {
                    RegistryKey currentKey = pregkey.OpenSubKey(item);
                    object displayName = currentKey.GetValue("DisplayName");
                    object PublishName = currentKey.GetValue("Publisher");
                    object oInstallTime = currentKey.GetValue("InstallDate");
                    object oSize = currentKey.GetValue("Size");
                    object oVersion = currentKey.GetValue("DisplayVersion");
                    object uninstallString = currentKey.GetValue("UninstallString");
                    object releaseType = currentKey.GetValue("ReleaseType");
                    bool isSecurityUpdate = false;
                    if (releaseType != null)
                    {
                        string tempType = releaseType.ToString();
                        if (tempType == "Security Update" || tempType == "Update")
                            isSecurityUpdate = true;
                    }
                    if (!isSecurityUpdate && displayName != null && uninstallString != null)
                    {
                        ObservableObj.Add(new AppInfo()
                        {
                            InstallData = CheckedIsNull(oInstallTime),
                            Name = CheckedIsNull(displayName),
                            PublishName = CheckedIsNull(PublishName),
                            ReleaseType = CheckedIsNull(releaseType),
                            SoftSize = CheckedIsNull(oSize),
                            SoftVersion = CheckedIsNull(oVersion),
                            UninstallString = CheckedIsNull(uninstallString)
                        });
                    }
                }
            }
            catch (Exception)
            { }
            pregkey.Close();
            return ObservableObj;
        }
        /// <summary>
        /// App 信息
        /// </summary>
        public class AppInfo
        {
            /// <summary>
            /// App 名称
            /// </summary>
            public string Name {get;set;}
            /// <summary>
            /// App 发布者名称
            /// </summary>
            public string PublishName { get; set; }
            /// <summary>
            /// App 安装日期
            /// </summary>
            public string InstallData { get; set; }
            /// <summary>
            /// App 大小
            /// </summary>
            public string SoftSize { get; set; }
            /// <summary>
            /// app 版本
            /// </summary>
            public string SoftVersion { get; set; }
            /// <summary>
            /// 卸载字符串
            /// </summary>
            public string UninstallString { get; set; }
            /// <summary>
            /// 发行类型：Security Update为安全更新,Update为更新
            /// </summary>
            public string ReleaseType { get; set; }

        }
      
        private static string CheckedIsNull(object objectInput)
        {
            if (objectInput == null)
            {
                return "";
            }
            else
            {
                return objectInput.ToString();
            }
        }
        #endregion

        /// <summary> 
        /// 获得根节点注册表
        /// </summary>
        /// <param name="rootKeyType">根节点类型</param>
        /// <returns>注册表项对象</returns>
        public static RegistryKey GetRootRegisterKey(WRegisterRootKeyType rootKeyType)
        {
            switch (rootKeyType)
            {
                case WRegisterRootKeyType.HKEY_CLASSES_ROOT:
                    return Registry.ClassesRoot;
                case WRegisterRootKeyType.HKEY_CURRENT_CONFIG:
                    return Registry.CurrentConfig;
                case WRegisterRootKeyType.HKEY_CURRENT_USER:
                    return Registry.CurrentUser;
                case WRegisterRootKeyType.HKEY_LOCAL_MACHINE:
                    return Registry.LocalMachine;
                default:
                    throw new Exception("注册表类型未定义！");
            }
        }

        /// <summary> 
        /// 在指定注册表项下创建注册表子项
        /// </summary>
        /// <param name="fatherKey">父注册表项</param>
        /// <param name="keyPath">注册表路径</param>
        /// <returns>注册表项对象</returns>
        public static RegistryKey CreateRegistryKey(RegistryKey fatherKey, string keyPath)
        {
            RegistryKey returnKey = fatherKey.CreateSubKey(keyPath);
            return returnKey;
        }

        /// <summary> 
        /// 在指定注册表项下创建注册表子项
        /// </summary>
        /// <param name="fatherKey">父注册表项</param>
        /// <param name="keyPath">注册表路径</param>
        /// <param name="keyValueName">要添加的注册表项值名称</param>
        /// <param name="keyValue">要添加的注册表项值</param>
        /// <returns>注册表项对象</returns>
        public static RegistryKey CreateRegistryKey(RegistryKey fatherKey, string keyPath, string keyValueName, string keyValue)
        {
            RegistryKey returnKey = CreateRegistryKey(fatherKey, keyPath);
            returnKey.SetValue(keyValueName, keyValue);
            return returnKey;
        }

        /// <summary> 
        /// 在指定注册表项下创建注册表子项
        /// </summary>
        /// <param name="rootKeyType">注册表根节点项类型</param>
        /// <param name="keyPath">注册表路径</param>
        /// <returns>注册表项对象</returns>
        public static RegistryKey CreateRegistryKey(WRegisterRootKeyType rootKeyType, string keyPath)
        {
            RegistryKey rootKey = GetRootRegisterKey(rootKeyType);
            return CreateRegistryKey(rootKey, keyPath);
        }

        /// <summary> 
        /// 在指定注册表项下创建注册表子项
        /// </summary>
        /// <param name="rootKeyType">注册表根节点项类型</param>
        /// <param name="keyPath">注册表路径</param>
        /// <param name="keyValueName">要添加的注册表项值名称</param>
        /// <param name="keyValue">要添加的注册表项值</param>
        /// <returns>注册表项对象</returns>
        public static RegistryKey CreateRegistryKey(WRegisterRootKeyType rootKeyType, string keyPath, string keyValueName, string keyValue)
        {
            RegistryKey returnKey = CreateRegistryKey(rootKeyType, keyPath);
            returnKey.SetValue(keyValueName, keyValue);
            return returnKey;
        }

        /// <summary> 
        /// 删除注册表子项
        /// </summary>
        /// <param name="rootKeyType">注册表根节点项类型</param>
        /// <param name="keyPath">注册表路径</param>
        /// <returns>true：成功，false：失败</returns>
        public static bool DeleteRegistryKey(WRegisterRootKeyType rootKeyType, string keyPath, string keyName)
        {
            RegistryKey rootKey = GetRootRegisterKey(rootKeyType);
            RegistryKey deleteKey = rootKey.OpenSubKey(keyPath, true);
            if (IsKeyHaveSubKey(deleteKey, keyName))
            {
                deleteKey.DeleteSubKey(keyName);
                return true;
            }
            return false;
        }

        /// <summary> 
        /// 获取注册表项
        /// </summary>
        /// <param name="rootKeyType">注册表根节点项类型</param>
        /// <param name="keyPath">注册表路径</param>
        /// <returns>注册表项对象</returns>
        public static RegistryKey GetRegistryKey(WRegisterRootKeyType rootKeyType, string keyPath)
        {
            RegistryKey rootKey = GetRootRegisterKey(rootKeyType);
            return rootKey.OpenSubKey(keyPath);
        }

        /// <summary> 
        /// 获取注册表项指定值
        /// </summary>
        /// <param name="rootKeyType">注册表根节点项类型</param>
        /// <param name="keyPath">注册表路径</param>
        /// <param name="keyName">要获得值的注册表值名称</param>
        /// <returns>null：指定项不存在</returns>
        public static string GetRegistryValue(WRegisterRootKeyType rootKeyType, string keyPath, string keyName)
        {
            RegistryKey key = GetRegistryKey(rootKeyType, keyPath);
            if (IsKeyHaveValue(key, keyName))
            {
                return key.GetValue(keyName).ToString();
            }
            return null;
        }

        /// <summary> 设置注册表项值
        /// </summary>
        /// <param name="key">注册表项</param>
        /// <param name="keyValueName">值名称</param>
        /// <param name="keyValue">值</param>
        /// <returns></returns>
        public static bool SetRegistryValue(RegistryKey key, string keyValueName, string keyValue)
        {
            key.SetValue(keyValueName, keyValue);
            return true;
        }

        /// <summary> 
        /// 设置注册表项值
        /// </summary>
        /// <param name="rootKeyType">注册表根节点项类型</param>
        /// <param name="keyPath">注册表路径</param>
        /// <param name="keyValueName">值名称</param>
        /// <param name="keyValue">值</param>
        /// <returns>true：成功，false：失败</returns>
        public static bool SetRegistryValue(WRegisterRootKeyType rootKeyType, string keyPath, string keyValueName, string keyValue)
        {
            RegistryKey key = GetRegistryKey(rootKeyType, keyPath);
            key.SetValue(keyValueName, keyValue);
            return true;
        }

        /// <summary> 
        /// 删除注册表项值
        /// </summary>
        /// <param name="key">注册表项</param>
        /// <param name="keyValueName">值名称</param>
        /// <returns>true：成功，false：失败</returns>
        public static bool DeleteRegistryValue(RegistryKey key, string keyValueName)
        {
            if (IsKeyHaveValue(key, keyValueName))
            {
                key.DeleteValue(keyValueName);
                return true;
            }
            return false;
        }

        /// <summary>
        ///  删除注册表项值
        /// </summary>
        /// <param name="rootKeyType">注册表根节点项类型</param>
        /// <param name="keyPath">注册表路径</param>
        /// <param name="keyValueName">值名称</param>
        /// <returns>true：成功，false：失败</returns>
        public static bool DeleteRegistryValue(WRegisterRootKeyType rootKeyType, string keyPath, string keyValueName)
        {
            RegistryKey key = GetRegistryKey(rootKeyType, keyPath);
            if (IsKeyHaveValue(key, keyValueName))
            {
                key.DeleteValue(keyValueName);
                return true;
            }
            return false;
        }

        /// <summary> 
        /// 判断注册表项是否有指定的注册表值
        /// </summary>
        /// <param name="key">注册表项</param>
        /// <param name="valueName">注册表值</param>
        /// <returns>true：存在值，false：不存在</returns>
        public static bool IsKeyHaveValue(RegistryKey key, string valueName)
        {
            string[] keyNames = key.GetValueNames();
            foreach (string keyName in keyNames)
            {
                if (keyName.Trim() == valueName.Trim())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary> 
        /// 判断注册表项是否有指定的子项
        /// </summary>
        /// <param name="key">注册表项</param>
        /// <param name="keyName">子项名称</param>
        /// <returns>true：有子项，false：无子项</returns>
        public static bool IsKeyHaveSubKey(RegistryKey key, string keyName)
        {
            string[] subKeyNames = key.GetSubKeyNames();
            foreach (string subKeyName in subKeyNames)
            {
                if (keyName.Trim() == subKeyName.Trim())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary> 
        /// 根据注册表键路径获得注册表键名称
        /// </summary>
        /// <param name="keyPath">注册表键路径</param>
        /// <returns>键名称</returns>
        public static string GetKeyNameByPath(string keyPath)
        {
            int keyIndex = keyPath.LastIndexOf("/");
            return keyPath.Substring(keyIndex + 1);
        }
        /// <summary> 
        /// 注册表根节点类型
        /// </summary>
        public enum WRegisterRootKeyType
        {
            HKEY_CLASSES_ROOT = 0,
            HKEY_CURRENT_USER = 1,
            HKEY_LOCAL_MACHINE = 2,
            HKEY_USERS = 3,
            HKEY_CURRENT_CONFIG = 4
        }
    }
}
