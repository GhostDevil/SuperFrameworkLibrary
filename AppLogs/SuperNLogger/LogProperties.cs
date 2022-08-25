/******************************************
 * 
 * 模块名称：内部类 LogProperties
 * 当前版本：1.0
 * 开发人员：不良帥
 * 开发时间：2011-06-30
 * 版本历史：
 * 
 * Copyright (C) 不良帥 2011-2015
 * 
 ******************************************/
using System;
using System.Text;
using System.Collections;
using System.IO;

namespace SuperFramework.SuperNLogger
{
    /// <summary>
    /// Property字符串属性列表，格式: [key=value]
    /// </summary>
    internal class LogProperties : Hashtable
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public LogProperties()
        {
        }

        /// <summary>
        /// 构造一个给定默认键值对列表的Property对象: [key=value]
        /// </summary>
        /// <param name="defaults">默认键值对列表的Property</param>
        public LogProperties(LogProperties defaults) 
        {
            foreach (DictionaryEntry de in defaults)
            {
                Add(de.Key, de.Value);
            }
        }

        /// <summary>
        /// 从已有的HashProperties对象中载入到当前HashProperties对象中
        /// </summary>
        /// <param name="hashProperties">已有的HashProperties对象</param>
        public void Load(LogProperties hashProperties)
        {
            foreach (DictionaryEntry de in hashProperties)
            {
                this[de.Key] = de.Value;
            }
        }

        /// <summary>
        /// 从指定的Property文件加载对应的元素(默认操作系统的当前 ANSI 代码页的编码)
        /// </summary>
        /// <param name="propertiesFileName">要读取的完整文件路径</param>
        public void Load(string propertiesFileName)
        {
            Load(propertiesFileName, Encoding.Default);
        }

        /// <summary>
        /// 从指定编码格式的Property文件加载对应的元素
        /// </summary>
        /// <param name="propertiesFileName">要读取的完整文件路径</param>
        /// <param name="encoding">字符编码格式</param>
        public void Load(string propertiesFileName, Encoding encoding)
        {
            StreamReader objReader = new StreamReader(propertiesFileName, encoding);

            Load(objReader);

            objReader.Close();
            objReader.Dispose();
        }

        /// <summary>
        /// 从指定的Property编码流中加载对应的元素
        /// </summary>
        /// <param name="streamReader">Property编码流</param>
        public void Load(StreamReader streamReader)
        {
            string line = streamReader.ReadLine(); //streamReader.EndOfStream
            while (line != null)
            {
                line = line.Trim();
                if (line.StartsWith("#") || line.StartsWith(@"\\") || line.StartsWith(@"//") || line.StartsWith(@"--"))
                {
                    line = streamReader.ReadLine();
                    continue;
                }
                int index = line.IndexOf("=");
                if (index < 0) // || index >= line.Length - 1
                {
                    line = streamReader.ReadLine();
                    continue;
                }
                string key = line.Substring(0, index).TrimEnd(null);
                string value = line.Substring(index + 1).TrimStart(null);
                this[key] = value;
                line = streamReader.ReadLine();
            }
        }

        /// <summary>
        /// 将该Property中的键值元素列表写入到指定的Property文件(默认操作系统的当前 ANSI 代码页的编码)。
        /// 如果对应的文件不存在则创建；如果文件已存在则覆盖。
        /// </summary>
        /// <param name="propertiesFileName">待写入的Property文件的完全限定名或相对文件名</param>
        public void Store(string propertiesFileName)
        {
            Store(propertiesFileName, Encoding.Default);
        }

        /// <summary>
        /// 将该Property中的键值元素列表写入到指定编码格式的Property文件。
        /// 如果对应的文件不存在则创建；如果文件已存在则覆盖。
        /// </summary>
        /// <param name="propertiesFileName">待写入的Property文件的完全限定名或相对文件名</param>
        /// <param name="encoding">字符编码格式</param>
        public void Store(string propertiesFileName, Encoding encoding)
        {
            FileInfo fi = new FileInfo(propertiesFileName);
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();//fi.DirectoryName :: fi.Name
            }
            StreamWriter objWriter = new StreamWriter(propertiesFileName, false, encoding);

            Store(objWriter);

            objWriter.Close();
            objWriter.Dispose();
        }

        /// <summary>
        /// 将该Property中的键值元素列表写入到指定的编码流中
        /// </summary>
        /// <param name="streamWriter">指定的编码流</param>
        public void Store(StreamWriter streamWriter)
        {
            int count = 0;
            streamWriter.WriteLine("#" + DateTime.Now.ToString());//comment
            foreach (DictionaryEntry de in this)
            {
                string strInfo = de.Key + "=" + de.Value;
                streamWriter.WriteLine(strInfo);
                count++;
                if (count > 1000)
                {
                    count = 0;
                    streamWriter.Flush();
                }
            }
            streamWriter.Flush();
        }

        /// <summary>
        /// 获取指定的键相关联的值。如果没有则为null。
        /// </summary>
        /// <param name="key">指定的键</param>
        /// <returns>指定的键相关联的值。如果没有则为null。</returns>
        public string GetProperty(string key)
        {
            return (this[key] == null) ? null : this[key].ToString();
        }

        /// <summary>
        /// 获取指定的键相关联的值。如果没有则返回默认值。
        /// </summary>
        /// <param name="key">指定的键</param>
        /// <param name="defaultValue">当没有指定的键时默认返回值</param>
        /// <returns>指定的键相关联的值。如果没有则返回默认值。</returns>
        public string GetProperty(string key, string defaultValue)
        {
            string val = GetProperty(key);
            return (val == null) ? defaultValue : val;
        }

        /// <summary>
        /// 添加/设置元素对应的键/值对
        /// </summary>
        /// <param name="key">要添加/设置的元素的键，不能为null</param>
        /// <param name="value">要添加/设置的元素的值</param>
        public void SetProperty(string key, string value) 
        {
            if (value != null) value = value.Trim();
            this[key.Trim()] = value;
        }

        /// <summary>
        /// 添加对应的键/值对，如果对应key已存在则会抛出异常
        /// </summary>
        /// <param name="key">要添加的元素的键，不能为null</param>
        /// <param name="value">要添加的元素的值</param>
        public void AddProperty(string key, string value)
        {
            if (value != null) value = value.Trim();
            Add(key.Trim(), value);
        }
    }
}
