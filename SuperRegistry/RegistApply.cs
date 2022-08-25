using Microsoft.Win32;
using System;
using System.Drawing;

namespace SuperFramework.SuperRegistry
{
    /// <summary>
    /// 说明：注册表应用
    /// 作者：不良帥
    /// 日期：2015-12-17
    /// </summary>
    public static class RegistApply
    {
        #region  程序开机运行 

        /// <summary> 
        /// 设置应用程序开机是否自动运行
        /// </summary>          
        /// <param name="filePath">应用程序的全路径</param>          
        /// <param name="isAutoRun">是否自动运行，为false时，取消自动运行</param>         
        /// <exception cref="设置不成功时抛出异常">设置不成功时抛出异常</exception>         

        public static void SetAutoRun(string filePath, bool isAutoRun)
        {
            RegistryKey reg = null;
            try
            {
                if (!System.IO.File.Exists(filePath))
                    throw new Exception("该程序不存在!");
                string name = filePath.Substring(filePath.LastIndexOf(@"\") + 1);
                name=name.Substring(0,name.LastIndexOf(@"."));
                reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                if (reg == null)
                    reg = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                if (isAutoRun)
                    reg.SetValue(name, filePath);
                else
                    reg.SetValue(name, false);

            }
            catch(Exception e)
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
                catch(Exception)
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
                catch(Exception)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 程序是否自动运行
        /// </summary>
        /// <param name="filePath">应用程序的全路径</param>      
        /// <returns>true：自动运行，false：非自动运行</returns>
        /// <exception cref="获取不成功时抛出异常">获取不成功时抛出异常</exception> 
        public static bool IsAutoRun(string filePath)
        {
            RegistryKey reg = null;
            try
            {

                string name = filePath.Substring(filePath.LastIndexOf(@"\") + 1);
                name = name.Substring(0,name.LastIndexOf(@"."));
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
            catch(Exception e)
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
            catch(Exception)
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

        #region  窗口位置 
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
                return new Point(Convert.ToInt16(regisOne.GetValue("SuperOne")), Convert.ToInt16(regisTwo.GetValue("SuperTwo")));//设置窗体的显示位置
            }
            catch (Exception ex)
            {

                new Exception(ex.Message);
            }
            return new Point(0, 0);
        }
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
                regisOne.SetValue("SuperOne", p.X.ToString());//将窗体当前X坐标写入注册表
                regisTwo.SetValue("SuperTwo", p.Y.ToString());//将当前窗体的Y坐标写入注册表
                status = true;
            }
            catch (Exception ex)
            {
                //status = false;
                new Exception(ex.Message);
            }
            return status;
        }
        #endregion
    }
}
