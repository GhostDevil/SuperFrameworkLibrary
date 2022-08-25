/********************************************************************
 * * 使本项目源码前请仔细阅读以下协议内容，如果你同意以下协议才能使用本项目所有的功能,
 * * 否则如果你违反了以下协议，有可能陷入法律纠纷和赔偿，作者保留追究法律责任的权利。
 * *
 * * Copyright (C) 厦门中科亚创有限责任公司.
 * * 作者： 煎饼的归宿    QQ：375324644   
 * * 请保留以上版权信息，否则作者将保留追究法律责任。
 * * 创建时间：2015-12-15
********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
namespace ObjectPersistenceLib
{
    /// <summary> 读写INI文件的类。
    /// </summary>
    internal class INIHelper
    {
        private static string path = "";

        /// <summary> 设置ini路径
        /// </summary>
        public static string Path
        {
            get { return INIHelper.path; }
            set { INIHelper.path = value; }
        }

        // 读写INI文件相关。
        [DllImport("kernel32.dll", EntryPoint = "WritePrivateProfileString", CharSet = CharSet.Ansi)]
        public static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileString", CharSet = CharSet.Ansi)]
        public static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        [DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileSectionNames", CharSet = CharSet.Ansi)]
        public static extern int GetPrivateProfileSectionNames(IntPtr lpszReturnBuffer, int nSize, string filePath);
        [DllImport("kernel32.DLL ", EntryPoint = "GetPrivateProfileSection", CharSet = CharSet.Ansi)]
        public static extern int GetPrivateProfileSection(string lpAppName, byte[] lpReturnedString, int nSize, string filePath);

        /// <summary> 向INI写入数据。
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Write(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, path);
        }

        /// <summary> 读取INI数据。
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Read(string section, string key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(section, key, "", temp, 255, path);
            return temp.ToString();
        }

        /// <summary> 读取一个ini里面所有的节
        /// </summary>
        /// <returns></returns>
        public static string[] GetAllSectionNames()
        {
            int MAX_BUFFER = 32767;
            IntPtr pReturnedString = Marshal.AllocCoTaskMem(MAX_BUFFER);
            int bytesReturned = GetPrivateProfileSectionNames(pReturnedString, MAX_BUFFER, path);
            if (bytesReturned == 0)
            {
                return new string[0];
            }
            string local = Marshal.PtrToStringAnsi(pReturnedString, (int)bytesReturned).ToString();
            Marshal.FreeCoTaskMem(pReturnedString);
            return local.Substring(0, local.Length - 1).Split('\0');
        }

        /// <summary> 得到某个节点下面所有的key和value组合
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetAllKeyValues(string section)
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
                    d.Add(result[i],"");
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
    }
}
