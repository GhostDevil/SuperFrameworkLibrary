using Microsoft.Win32;
using System;

namespace SuperFramework.SuperRegistry
{
    /// <summary>
    /// 日 期:2015-12-17
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

        #region 获得根节点注册表
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
        #endregion

        #region 在指定注册表项下创建注册表子项


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
        #endregion

        #region 删除注册表子项
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
        #endregion


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
