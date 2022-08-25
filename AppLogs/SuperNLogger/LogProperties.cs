/******************************************
 * 
 * ģ�����ƣ��ڲ��� LogProperties
 * ��ǰ�汾��1.0
 * ������Ա��������
 * ����ʱ�䣺2011-06-30
 * �汾��ʷ��
 * 
 * Copyright (C) ������ 2011-2015
 * 
 ******************************************/
using System;
using System.Text;
using System.Collections;
using System.IO;

namespace SuperFramework.SuperNLogger
{
    /// <summary>
    /// Property�ַ��������б���ʽ: [key=value]
    /// </summary>
    internal class LogProperties : Hashtable
    {
        /// <summary>
        /// Ĭ�Ϲ��캯��
        /// </summary>
        public LogProperties()
        {
        }

        /// <summary>
        /// ����һ������Ĭ�ϼ�ֵ���б��Property����: [key=value]
        /// </summary>
        /// <param name="defaults">Ĭ�ϼ�ֵ���б��Property</param>
        public LogProperties(LogProperties defaults) 
        {
            foreach (DictionaryEntry de in defaults)
            {
                Add(de.Key, de.Value);
            }
        }

        /// <summary>
        /// �����е�HashProperties���������뵽��ǰHashProperties������
        /// </summary>
        /// <param name="hashProperties">���е�HashProperties����</param>
        public void Load(LogProperties hashProperties)
        {
            foreach (DictionaryEntry de in hashProperties)
            {
                this[de.Key] = de.Value;
            }
        }

        /// <summary>
        /// ��ָ����Property�ļ����ض�Ӧ��Ԫ��(Ĭ�ϲ���ϵͳ�ĵ�ǰ ANSI ����ҳ�ı���)
        /// </summary>
        /// <param name="propertiesFileName">Ҫ��ȡ�������ļ�·��</param>
        public void Load(string propertiesFileName)
        {
            Load(propertiesFileName, Encoding.Default);
        }

        /// <summary>
        /// ��ָ�������ʽ��Property�ļ����ض�Ӧ��Ԫ��
        /// </summary>
        /// <param name="propertiesFileName">Ҫ��ȡ�������ļ�·��</param>
        /// <param name="encoding">�ַ������ʽ</param>
        public void Load(string propertiesFileName, Encoding encoding)
        {
            StreamReader objReader = new StreamReader(propertiesFileName, encoding);

            Load(objReader);

            objReader.Close();
            objReader.Dispose();
        }

        /// <summary>
        /// ��ָ����Property�������м��ض�Ӧ��Ԫ��
        /// </summary>
        /// <param name="streamReader">Property������</param>
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
        /// ����Property�еļ�ֵԪ���б�д�뵽ָ����Property�ļ�(Ĭ�ϲ���ϵͳ�ĵ�ǰ ANSI ����ҳ�ı���)��
        /// �����Ӧ���ļ��������򴴽�������ļ��Ѵ����򸲸ǡ�
        /// </summary>
        /// <param name="propertiesFileName">��д���Property�ļ�����ȫ�޶���������ļ���</param>
        public void Store(string propertiesFileName)
        {
            Store(propertiesFileName, Encoding.Default);
        }

        /// <summary>
        /// ����Property�еļ�ֵԪ���б�д�뵽ָ�������ʽ��Property�ļ���
        /// �����Ӧ���ļ��������򴴽�������ļ��Ѵ����򸲸ǡ�
        /// </summary>
        /// <param name="propertiesFileName">��д���Property�ļ�����ȫ�޶���������ļ���</param>
        /// <param name="encoding">�ַ������ʽ</param>
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
        /// ����Property�еļ�ֵԪ���б�д�뵽ָ���ı�������
        /// </summary>
        /// <param name="streamWriter">ָ���ı�����</param>
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
        /// ��ȡָ���ļ��������ֵ�����û����Ϊnull��
        /// </summary>
        /// <param name="key">ָ���ļ�</param>
        /// <returns>ָ���ļ��������ֵ�����û����Ϊnull��</returns>
        public string GetProperty(string key)
        {
            return (this[key] == null) ? null : this[key].ToString();
        }

        /// <summary>
        /// ��ȡָ���ļ��������ֵ�����û���򷵻�Ĭ��ֵ��
        /// </summary>
        /// <param name="key">ָ���ļ�</param>
        /// <param name="defaultValue">��û��ָ���ļ�ʱĬ�Ϸ���ֵ</param>
        /// <returns>ָ���ļ��������ֵ�����û���򷵻�Ĭ��ֵ��</returns>
        public string GetProperty(string key, string defaultValue)
        {
            string val = GetProperty(key);
            return (val == null) ? defaultValue : val;
        }

        /// <summary>
        /// ���/����Ԫ�ض�Ӧ�ļ�/ֵ��
        /// </summary>
        /// <param name="key">Ҫ���/���õ�Ԫ�صļ�������Ϊnull</param>
        /// <param name="value">Ҫ���/���õ�Ԫ�ص�ֵ</param>
        public void SetProperty(string key, string value) 
        {
            if (value != null) value = value.Trim();
            this[key.Trim()] = value;
        }

        /// <summary>
        /// ��Ӷ�Ӧ�ļ�/ֵ�ԣ������Ӧkey�Ѵ�������׳��쳣
        /// </summary>
        /// <param name="key">Ҫ��ӵ�Ԫ�صļ�������Ϊnull</param>
        /// <param name="value">Ҫ��ӵ�Ԫ�ص�ֵ</param>
        public void AddProperty(string key, string value)
        {
            if (value != null) value = value.Trim();
            Add(key.Trim(), value);
        }
    }
}
