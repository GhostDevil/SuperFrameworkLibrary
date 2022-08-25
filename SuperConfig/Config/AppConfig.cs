using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace SuperFramework.SuperConfig.Config
{
    /// <summary>
    /// 日 期:2014-12-10
    /// 作 者:不良帥
    /// 描 述:ApplictionConfig配置文件操作
    /// </summary>
    public class AppConfig
    {
        #region  添加AppConfig中的AppSetting节点值 
        /// <summary>
        /// 添加AppConfig中的AppSetting节点值
        /// </summary>
        /// <param name="key">节点名</param>
        /// <param name="value">节点值</param>
        /// <returns>返回true为成功，false为失败</returns>
        public static bool AddSection(string key, string value)
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                AppSettingsSection app = config.AppSettings;
                app.Settings.Add(key, value);
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch { return false; }
            return true;
        }
        #endregion

        #region  更新AppConfig中的AppSetting节点值 
        /// <summary>
        /// 更新AppConfig中的AppSetting节点值
        /// </summary>
        /// <param name="key">节点名</param>
        /// <param name="value">节点值</param>
        /// <returns>成功返回true，失败返回false</returns>
        public static bool UpdateSection(string key, string value)
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                AppSettingsSection app = config.AppSettings;
                app.Settings[key].Value = value;
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch { return false; }
            return true;
        }
        #endregion

        #region  删除AppConfig中的AppSetting节点值 
        /// <summary>
        /// 删除AppConfig中的AppSetting节点值
        /// </summary>
        /// <param name="key">节点名</param>
        /// <returns>成功返回true，失败返回false</returns>
        public static bool DeleteSection(string key)
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                AppSettingsSection app = config.AppSettings;
                app.Settings.Remove(key);
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch { return false; }
            return true;
        }
        #endregion

        #region  根据键获取AppConfig配置节点 
        /// <summary>
        /// 根据键获取AppConfig配置文件
        /// </summary>
        /// <param name="key">节点名</param>
        /// <returns>返回节点值</returns>
        public static string GetAppConfig(string key)
        {
            string val = string.Empty;
            if (ConfigurationManager.AppSettings.AllKeys.Contains(key))
                val = ConfigurationManager.AppSettings[key];
            return val;
        }
        #endregion

        #region  获取所有AppConfig配置节点 
        /// <summary>
        /// 获取所有AppConfig配置节点
        /// </summary>
        /// <returns>Dictionary（key val）</returns>
        public static Dictionary<string, string> GetAllSection()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (string key in ConfigurationManager.AppSettings.AllKeys)
                dict.Add(key, ConfigurationManager.AppSettings[key]);
            return dict;
        }
        #endregion

        #region  写AppConfig配置文件,如果节点不存在则自动创建 
        /// <summary>
        /// 写AppConfig配置文件,如果节点不存在则自动创建
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns>成功返回true，失败返回false</returns>
        public static bool SetSection(string key, string value)
        {

            try
            {
                Configuration conf = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (!conf.AppSettings.Settings.AllKeys.Contains(key))
                    conf.AppSettings.Settings.Add(key, value);
                else
                    conf.AppSettings.Settings[key].Value = value;
                conf.Save();
                return true;
            }
            catch { return false; }
        }
        #endregion

        #region  写AppConfig配置文件(用键值创建),如果节点不存在则自动创建 
        /// <summary>
        /// 写AppConfig配置文件(用键值创建),如果节点不存在则自动创建
        /// </summary>
        /// <param name="dict">键值集合</param>
        /// <returns>成功返回true，失败返回false</returns>

        public static bool SetSeciton(Dictionary<string, string> dict)
        {
            try
            {
                if (dict == null || dict.Count == 0)
                    return false;
                Configuration conf = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                foreach (string key in dict.Keys)
                {
                    if (!conf.AppSettings.Settings.AllKeys.Contains(key))
                        conf.AppSettings.Settings.Add(key, dict[key]);
                    else
                        conf.AppSettings.Settings[key].Value = dict[key];
                }
                conf.Save();
                return true;
            }
            catch { return false; }
        }
        #endregion

        #region  更新连接字符串 
        ///<summary>
        ///更新连接字符串
        ///</summary> 
        ///<param name="newName">连接字符串名称</param> 
        ///<param name="newConString">连接字符串内容</param> 
        ///<param name="newProviderName">数据提供程序名称</param> 
        /// <returns>成功返回true，失败返回false</returns>
        public static bool UpdateConnectionStr(string newName, string newConString, string newProviderName)
        {
            try
            {
                bool isModified = false;  //记录该连接串是否已经存在      
                //如果要更改的连接串已经存在      
                if (ConfigurationManager.ConnectionStrings[newName] != null)
                    isModified = true;
                //新建一个连接字符串实例      
                ConnectionStringSettings mySettings = new ConnectionStringSettings(newName, newConString, newProviderName);
                // 打开可执行的配置文件*.exe.config      
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                // 如果连接串已存在，首先删除它      
                if (isModified)
                    config.ConnectionStrings.ConnectionStrings.Remove(newName);
                // 将新的连接串添加到配置文件中.      
                config.ConnectionStrings.ConnectionStrings.Add(mySettings);
                // 保存对配置文件所作的更改      
                config.Save(ConfigurationSaveMode.Modified);
                // 强制重新载入配置文件的ConnectionStrings配置节      
                ConfigurationManager.RefreshSection("ConnectionStrings");
                return true;
            }
            catch { return false; }
        }
        #endregion

        #region 依据连接串名字connectionName返回数据连接字符串
        ///<summary> 
        ///依据连接串名字connectionName返回数据连接字符串  
        ///</summary> 
        ///<param name="connectionName"></param> 
        ///<returns></returns> 
        public static string GetConnectionStr(string connectionName) => ConfigurationManager.ConnectionStrings[connectionName].ConnectionString.ToString();
        #endregion

        #region 获取配置节点集合
        /// <summary>
        /// 获取配置节点集合
        /// </summary>
        /// <param name="ch">开始关键字</param>
        /// <returns></returns>
        public static List<string> GetAppConfigList(string ch)
        {
            List<string> listKeyStr = new List<string>();
            listKeyStr.Clear();
            foreach (string key in ConfigurationManager.AppSettings)
            {
                if (key.Length > ch.Length)
                {
                    if (key.Substring(0, ch.Length).Equals(ch))
                        listKeyStr.Add(string.Format("{0}：{1}", key, ConfigurationManager.AppSettings[key]));
                }
            }
            return listKeyStr;
        }
        #endregion

        #region 在＊.exe.config文件中appSettings配置节增加一对键、值对
        ///<summary>  
        ///在＊.exe.config文件中appSettings配置节增加一对键、值对  
        ///</summary>  
        ///<param name="newKey"></param>  
        ///<param name="newValue"></param>  
        public static void UpdateAppConfig(string newKey, string newValue)
        {
            bool isModified = false;
            foreach (string key in ConfigurationManager.AppSettings)
            {
                if (key == newKey)
                {
                    isModified = true;
                }
            }

            // 打开应用程序配置文件 
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //删除对象
            if (isModified)
            {
                config.AppSettings.Settings.Remove(newKey);
            }
            // 添加节点
            config.AppSettings.Settings.Add(newKey, newValue);
            // 保存app.config文件
            config.Save(ConfigurationSaveMode.Modified);
            //强制重新加载
            ConfigurationManager.RefreshSection("appSettings");
        }
        #endregion
    }
}
