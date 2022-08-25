#if NET40_OR_GREATER
using System.Configuration;
using System.Linq;
using System.Web.Configuration;

namespace SuperFramework.SuperConfig.Config
{
    /// <summary>
    /// WebConfig配置文件操作
    /// </summary>
    public class WebConfig
    {
        #region  添加WebConfig中AppSettings的节点 
        /// <summary>
        /// 添加WebConfig中AppSettings的节点
        /// </summary>
        /// <param name="key">节点名</param>
        /// <param name="value">节点值</param>
        /// <returns>返回true为成功，false为失败</returns>
        public static bool AddSection(string key, string value)
        {
            try
            {
                Configuration config = WebConfigurationManager.OpenWebConfiguration(null);
                AppSettingsSection app = config.AppSettings;
                app.Settings.Add(key, value);
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch { return false; }
            return true;
        }
        #endregion

        #region  更新WebConfig中AppSettings的节点 
        /// <summary>
        /// 更新WebConfig中AppSettings的节点
        /// </summary>
        /// <param name="key">节点名</param>
        /// <param name="value">节点值</param>
        /// <returns>返回true为成功，false为失败</returns>
        public static bool UpdateSection(string key, string value)
        {
            try
            {
                Configuration config = WebConfigurationManager.OpenWebConfiguration(null);
                AppSettingsSection app = config.AppSettings;
                app.Settings[key].Value = value;
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch { return false; }
            return true;
        }
        #endregion

        #region  删除WebConfig中AppSettings的节点 
        /// <summary>
        /// 删除WebConfig中AppSettings的节点
        /// </summary>
        /// <param name="key">节点名</param>
        /// <returns>返回true为成功，false为失败</returns>
        public static bool DeleteSection(string key)
        {
            try
            {
                Configuration config = WebConfigurationManager.OpenWebConfiguration(null);
                AppSettingsSection app = config.AppSettings;
                app.Settings.Remove(key);
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch { return false; }
            return true;
        }
        #endregion

        #region  获取WebConfig中AppSettings的节点值 
        /// <summary>
        /// 获取WebConfig中AppSettings的节点值
        /// </summary>
        /// <param name="key">节点名</param>
        /// <returns>返回节点值</returns>
        public static string GetSection(string key)
        {
            string val = string.Empty;
            if (WebConfigurationManager.AppSettings.AllKeys.Contains(key))
                val = WebConfigurationManager.AppSettings[key];
            return val;
        }
        #endregion

        #region  依据连接串名字connectionName返回数据连接字符串  
        ///<summary> 
        ///依据连接串名字connectionName返回数据连接字符串  
        ///</summary> 
        ///<param name="connectionName">连接字符串名称</param> 
        ///<returns>连接字符串值</returns> 
        public static string GetConnectionStr(string connectionName)
        {
            try
            {
                return WebConfigurationManager.ConnectionStrings[connectionName].ConnectionString.ToString();
            }
            catch { return null; }
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
                if (WebConfigurationManager.ConnectionStrings[newName] != null)
                    isModified = true;
                //新建一个连接字符串实例      
                ConnectionStringSettings mySettings = new ConnectionStringSettings(newName, newConString, newProviderName);
                // 打开可执行的配置文件*.exe.config      
                Configuration config = WebConfigurationManager.OpenWebConfiguration(null);
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
    }
}
#endif
