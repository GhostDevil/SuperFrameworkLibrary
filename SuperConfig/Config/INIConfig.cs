using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace SuperFramework.SuperConfig.Config
{
    /// <summary>
    /// 日期:2022-08-23
    /// 作者:不良帥
    /// 说明:INI配置文件操作辅助方法类
    /// </summary>
    public static class INIConfig
    {
        #region  Ini操作API声明 

        /// <summary>
        /// 将指定的键值对写到指定的节点，如果已经存在则替换。
        /// </summary>
        /// <param name="lpAppName">节点，如果不存在此节点，则创建此节点</param>
        /// <param name="lpString">Item键值对，多个用\0分隔,形如key1=value1\0key2=value2
        /// <para>如果为string.Empty，则删除指定节点下的所有内容，保留节点</para>
        /// <para>如果为null，则删除指定节点下的所有内容，并且删除该节点</para>
        /// </param>
        /// <param name="lpFileName">INI文件</param>
        /// <returns>是否成功写入</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool WritePrivateProfileSection(string lpAppName, string lpString, string lpFileName);
        ///<summary> 
        /// Ini的文件写入，创建,修改键的值 
        /// </summary> 
        /// <param name="section">节点，没有就创建</param> 
        /// <param name="key">键，没有就创建</param> 
        /// <param name="val">值，有就改</param> 
        /// <param name="iniPath">文件路径和名字,没有则创建</param> 
        /// <returns></returns> 
        [DllImport("kernel32.dll")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string iniPath);

        /// <summary>
        /// 读取INI文件中指定的Key的值
        /// </summary>
        /// <param name="lpAppName">节点名称。如果为null,则读取INI中所有节点名称,每个节点名称之间用\0分隔</param>
        /// <param name="lpKeyName">Key名称。如果为null,则读取INI中指定节点中的所有KEY,每个KEY之间用\0分隔</param>
        /// <param name="lpDefault">读取失败时的默认值</param>
        /// <param name="lpReturnedString">读取的内容缓冲区，读取之后，多余的地方使用\0填充</param>
        /// <param name="nSize">内容缓冲区的长度</param>
        /// <param name="lpFileName">INI文件名</param>
        /// <returns>实际读取到的长度</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern uint GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, [In, Out] char[] lpReturnedString, uint nSize, string lpFileName);

        ///// <summary>
        ///// 读取INI文件中指定的Key的值，再一种声明，使用string作为缓冲区的类型同char[]。
        ///// </summary>
        ///// <param name="lpAppName">节点名称。如果为null,则读取INI中所有节点名称,每个节点名称之间用\0分隔</param>
        ///// <param name="lpKeyName">Key名称。如果为null,则读取INI中指定节点中的所有KEY,每个KEY之间用\0分隔</param>
        ///// <param name="lpDefault">读取失败时的默认值</param>
        ///// <param name="lpReturnedString">读取的内容缓冲区，读取之后，多余的地方使用\0填充</param>
        ///// <param name="nSize">内容缓冲区的长度</param>
        ///// <param name="lpFileName">INI文件名</param>
        ///// <returns>实际读取到的长度</returns>

        //[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        //private static extern uint GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, string lpReturnedString, uint nSize, string lpFileName);
        /// <summary>   
        /// 读取INI文件中指定的Key的值   
        /// </summary>   
        /// <param name="lpAppName">节点名称。如果为null,则读取INI中所有节点名称,每个节点名称之间用\0分隔</param>   
        /// <param name="lpKeyName">Key名称。如果为null,则读取INI中指定节点中的所有KEY,每个KEY之间用\0分隔</param>   
        /// <param name="lpDefault">读取失败时的默认值</param>   
        /// <param name="retVal">读取的内容缓冲区，读取之后，多余的地方使用\0填充</param>   
        /// <param name="nSize">内容缓冲区的长度</param>   
        /// <param name="lpFileName">INI文件名</param>   
        /// <returns>实际读取到的长度</returns>   
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern uint GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder retVal, uint nSize, string lpFileName);
        /// <summary>   
        /// 获取ini所有节点名称  
        /// </summary>   
        /// <param name="lpszReturnBuffer">存放节点名称的内存地址,每个节点之间用\0分隔</param>   
        /// <param name="nSize">内存大小(characters)</param>   
        /// <param name="lpFileName">Ini文件</param>   
        /// <returns>内容的实际长度,为0表示没有内容,为nSize-2表示内存大小不够</returns>   
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern uint GetPrivateProfileSectionNames(IntPtr lpszReturnBuffer, uint nSize, string lpFileName);
        /// <summary>   
        /// 获取某个指定节点(Section)中所有键(Key)和值(Value)   
        /// </summary>   
        /// <param name="lpAppName">节点名称</param>   
        /// <param name="lpReturnedString">返回值的内存地址,每个之间用\0分隔</param>   
        /// <param name="nSize">内存大小(characters)</param>   
        /// <param name="lpFileName">Ini文件</param>   
        /// <returns>内容的实际长度,为0表示没有内容,为nSize-2表示内存大小不够</returns>   
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern uint GetPrivateProfileSection(string lpAppName, IntPtr lpReturnedString, uint nSize, string lpFileName);

        /// <summary>   
        /// 获取某个指定节点(Section)中所有键(Key)和值(Value)   
        /// </summary>   
        /// <param name="lpAppName">节点名称</param>   
        /// <param name="lpReturnedString">返回值数组</param>   
        /// <param name="nSize">内存大小(characters)</param>   
        /// <param name="filePath">Ini文件</param>   
        /// <returns>内容的实际长度,为0表示没有内容,为nSize-2表示内存大小不够</returns>   
        [DllImport("kernel32.DLL ", EntryPoint = "GetPrivateProfileSection", CharSet = CharSet.Ansi)]
        private static extern int GetPrivateProfileSection(string lpAppName, byte[] lpReturnedString, int nSize, string filePath);

        

        #endregion

        #region 读写INI文件
        /// <summary> 
        /// 向INI写入数据
        /// </summary>
        /// <param name="section">节点名</param>
        /// <param name="key">键名</param>
        /// <param name="value">键值</param>
        /// <param name="path">配置文件地址</param>
        public static void Write(string section, string key, string value,string path) => WritePrivateProfileString(section, key, value, path);

        /// <summary> 
        /// 读取INI数据
        /// </summary>
        /// <param name="section">节点名称</param>
        /// <param name="key">键名</param>
        /// <param name="path">配置文件地址</param>
        /// <returns>返回键的值</returns>
        public static string Read(string section, string key,string path)
        {
            StringBuilder temp = new StringBuilder(255);
            _ = GetPrivateProfileString(section, key, "", temp, 255, path);
            return temp.ToString();
        }
        #endregion

        #region  删除
        /// <summary> 
        /// 删除INI节点 
        /// </summary> 
        /// <param name="filePath">配置文件路径</param>
        /// <param name="section">节点</param> 
        public static void DeleteSection(string filePath,string section)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("必须指定INI文件全路径", nameof(filePath));
            if (string.IsNullOrWhiteSpace(section))
                throw new ArgumentException("必须指定节点名称", nameof(section));
            WritePrivateProfileString(section, null, null, filePath);
        }
      
        /// <summary>
        /// 删除INI指定的键和值 
        /// </summary> 
        /// <param name="filePath">配置文件路径</param>
        /// <param name="section">节点</param> 
        /// <param name="key">键</param> 
        public static void DeleteKey(string filePath,string section, string key)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("必须指定INI文件全路径", nameof(filePath));
            if (string.IsNullOrWhiteSpace(section))
                throw new ArgumentException("必须指定节点名称", nameof(section));
            WritePrivateProfileString(section, key, null, filePath);
        }
        
        /// <summary>
        /// 删除INI指定的键和值 
        /// </summary> 
        /// <param name="filePath">配置文件路径</param>
        /// <param name="section">节点</param> 
        public static void DeleteSectionAllKey(string filePath, string section)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("必须指定INI文件全路径", nameof(filePath));
            if (string.IsNullOrWhiteSpace(section))
                throw new ArgumentException("必须指定节点名称", nameof(section));
            WritePrivateProfileSection(section, string.Empty, filePath);
        }
        #endregion

        #region  获取
        /// <summary> 
        /// 得到某个节点下面所有的key和value组合
        /// </summary>
        /// <param name="section">节点名称</param>
        /// <param name="path">配置文件地址</param>
        /// <returns>返回节点下的键值对</returns>
        public static Dictionary<string, string> GetAllKeyValues(string section, string path)
        {
            Dictionary<string, string> d = new Dictionary<string, string>();
            byte[] b = new byte[65535];
            GetPrivateProfileSection(section, b, b.Length, path);
            string s = System.Text.Encoding.Default.GetString(b);
            string[] tmp = s.Split((char)0);
            List<string> result = new List<string>();
            foreach (string r in tmp)
            {
                if (r != string.Empty)
                    result.Add(r);
            }
            for (int i = 0; i < result.Count; i++)
            {
                int index = result[i].IndexOf("=");
                if (index == -1)
                {
                    d.Add(result[i], "");
                }
                if (index < result[i].Length - 1)
                {
                    d.Add(result[i].Substring(0, index), result[i].Substring(index + 1, result[i].Length - index - 1));
                }
                if (index == result[i].Length - 1)
                {
                    d.Add(result[i].Substring(0, index), "");
                }
            }
            return d;
        }
        /****************************************
             * 函数名称：GetKeyValue
             * 功能说明：读取一个配置文件的属性信息
             * 参    数：section:属性段,key:属性键,defaultValue:默认返回值,iniFilePath:配置文件路径
             * 调用示列：
             *            string section = "Settings";     
             *            string key = "property";
             *            string defaultValue = "config.ini";     
             *            string iniFilePath = Server.MapPath("config.ini");
             *            string value=GetIniValue(section, key, defaultValue,iniFilePath);
            *****************************************/
        /// <summary> 
        /// 获取INI文件中指定的Key的值  
        /// </summary> 
        /// <param name="filePath">配置文件路径</param>
        /// <param name="section">节点名称</param> 
        /// <param name="key">Key名称</param> 
        /// <param name="defaultValue">读取失败时的默认值</param> 
        /// <returns>返回配置节点的属性信息</returns> 
        public static string GetKeyValue(string filePath, string section, string key, string defaultValue)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("必须指定INI文件全路径", nameof(filePath));
            if (File.Exists(filePath))
            {
                string value = defaultValue;
                const int SIZE = 1024 * 10;
                if (string.IsNullOrWhiteSpace(section))
                    throw new ArgumentException("必须指定节点名称", nameof(section));
                if (string.IsNullOrWhiteSpace(key))
                    throw new ArgumentException("必须指定键名称", nameof(key));
                StringBuilder sb = new StringBuilder(SIZE);                
                uint bytesReturned = GetPrivateProfileString(section, key, defaultValue, sb, SIZE, filePath);
                if (bytesReturned != 0)
                {
                    value = sb.ToString();
                }
                return value;
            }
            else
                return null;
        }
     
        /// <summary>   
        /// 获取INI某个指定节点(Section)中所有键(Key)和值(Value)  
        /// </summary>   
        /// <param name="filePath">配置文件路径</param>
        /// <param name="section">节点名称</param>   
        /// <returns>指定节点中的所有项目数组,没有内容返回string[0]</returns>   
        public static string[] GetAllItems(string filePath, string section)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("必须指定INI文件全路径", nameof(filePath));
            //返回值形式为 key=value,例如 Color=Red   
            const uint MAX_BUFFER = 32767;    //默认为32767     
            string[] items = new string[0];      //返回值     
            //分配内存  
            IntPtr pReturnedString = Marshal.AllocCoTaskMem((int)MAX_BUFFER * sizeof(char));
            uint bytesReturned = GetPrivateProfileSection(section, pReturnedString, MAX_BUFFER, filePath);
            if (!(bytesReturned == MAX_BUFFER - 2) || (bytesReturned == 0))
            {
                string returnedString = Marshal.PtrToStringAuto(pReturnedString, (int)bytesReturned);
                items = returnedString.Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);
            }
            Marshal.FreeCoTaskMem(pReturnedString);     //释放内存      
            return items;
        }
      
        /// <summary>
        /// 获取INI文件中指定节点(Section)中的所有条目的Key列表
        /// </summary>
        /// <param name="filePath">配置文件路径</param>
        /// <param name="section">节点名称</param>
        /// <returns>如果没有内容,反回string[0]</returns>
        public static string[] GetAllItemKeys(string filePath, string section)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("必须指定INI文件全路径", nameof(filePath));
            if (string.IsNullOrWhiteSpace(section))
                throw new ArgumentException("必须指定节点名称", nameof(section));
            string[] value = new string[0];
            const int SIZE = 1024 * 10;
            char[] chars = new char[SIZE];
            uint bytesReturned = GetPrivateProfileString(section, null, null, chars, SIZE, filePath);
            if (bytesReturned != 0)
                value = new string(chars).Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);
            return value;
        }
      
        /// <summary> 
        /// 获取INI所有节点名称(Section) 
        /// </summary> 
        /// <param name="filePath">配置文件路径</param>
        /// <returns>返回所有节点名称数组</returns> 
        /// <exception cref="ArgumentException">必须指定INI文件全路径</exception>
        public static string[] GetAllSectionNames(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("必须指定INI文件全路径", nameof(filePath));
            const uint MAX_BUFFER = 32767;    //默认为32767    
            string[] sections = new string[0];      //返回值      
            //申请内存   
            IntPtr pReturnedString = Marshal.AllocCoTaskMem((int)MAX_BUFFER * sizeof(char));
            uint bytesReturned = GetPrivateProfileSectionNames(pReturnedString, MAX_BUFFER, filePath);
            if (bytesReturned != 0)
            {
                //读取指定内存的内容   
                string local = Marshal.PtrToStringAuto(pReturnedString, (int)bytesReturned);
                //每个节点之间用\0分隔,末尾有一个\0   
                sections = local.Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);
            }
            //释放内存   

            Marshal.FreeCoTaskMem(pReturnedString);
            return sections;
        }
        #endregion

        #region  Ini的文件写入，创建，修改键的值 
        /****************************************
              * 函数名称：CreateOrUpdate
              * 功能说明：写入一个配置文件的属性信息
              * 参    数：section:属性段,key:属性键,defaultValue:默认返回值,iniFilePath:配置文件路径
              * 调用示列：
              *            string section = "Settings";     
              *            string key = "property";
              *            string defaultValue = "config.ini";     
              *            string iniFilePath = Server.MapPath("config.ini");
              *            bool success=CreateOrUpdateIni(section, key, defaultValue, iniFilePath);
             *****************************************/
        /// <summary> 
        /// INI的文件写入，创建，修改键的值 
        /// </summary> 
        /// <param name="filePath">配置文件路径</param>
        /// <param name="section">节点，没有就创建</param> 
        /// <param name="key">键，没有就创建</param> 
        /// <param name="defaultValue">值，有就改</param>  
        /// <returns>成功返回true，失败返回false</returns>
        /// <exception cref="ArgumentException">必须指定INI文件全路径或节点名称</exception>
        public static bool CreateOrUpdate(string filePath,string section, string key, string defaultValue)
        {
            if (string.IsNullOrWhiteSpace(section))
                throw new ArgumentException("必须指定节点名称", nameof(section));
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("必须指定INI文件全路径", nameof(filePath));
            if (!File.Exists(filePath))
                File.Create(filePath);
            long OpStation = WritePrivateProfileString(section, key, defaultValue, filePath);
            if (OpStation == 0)
                return false;
            else
                return true;
        }
        /// <summary>
        /// INI的文件写入，将指定的键值对写到指定的节点，如果已经存在则替换，不存在则创建
        /// </summary>
        /// <param name="filePath">配置文件路径</param>
        /// <param name="section">节点，如果不存在此节点，则创建此节点</param>
        /// <param name="items">键值对，多个用\0分隔,形如key1=value1\0key2=value2</param>
        /// <returns>成功返回true，失败返回false</returns>
        /// <exception cref="ArgumentException">必须指定INI文件全路径或节点名称或指定键值对</exception>
        public static bool CreateOrUpdate(string filePath, string section, string items)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("必须指定INI文件全路径", nameof(filePath));
            if (string.IsNullOrWhiteSpace(section))
                throw new ArgumentException("必须指定节点名称", nameof(section));
            if (string.IsNullOrWhiteSpace(items))
                throw new ArgumentException("必须指定键值对", nameof(items));
            return WritePrivateProfileSection(section, items, filePath);
        }
        #endregion
    }
}
