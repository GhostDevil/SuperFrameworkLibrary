using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SuperFramework.SuperConfig.Xml
{
    /// <summary>
    /// 日 期:2015-08-18
    /// 作 者:不良帥
    /// 描 述:XMl序列化辅助类
    /// </summary>
    public static class XmlSerialization
    {
       
        /// <summary>
        /// 文本XML序列化
        /// </summary>
        /// <param name="item">对象</param>
        public static string XmlSerialize<T>(T item)
        {
            XmlSerializer serializer = new XmlSerializer(item.GetType());
            StringBuilder sb = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(sb))
            {
                serializer.Serialize(writer, item);
                return sb.ToString();
            }
        }


        #region  xml反序列化 
        /// <summary>
        /// xml反序列化
        /// * 调用示列:
        /// * XmlMethod.XmlDeserialize(typeof(myclass), xmlstring) as myclass
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="xml">XML字符串</param>
        /// <returns>返回object</returns>
        public static object XmlDeserialize(Type type, string xml)
        {
            try
            {
                using (StringReader sr = new StringReader(xml))
                {
                    XmlSerializer xmldes = new XmlSerializer(type);
                    return xmldes.Deserialize(sr);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// XML字符串反序列化为对象
        /// </summary>
        /// <typeparam name="T">类对象</typeparam>
        /// <param name="xmlOfObject">需要反序列化的xml字符串</param>
        /// <returns>反序列化后的类对象</returns>
        public static T XmlDeserialize<T>(string xmlOfObject) where T : class
        {

            try
            {
                using (XmlReader xmlReader = XmlReader.Create(new StringReader(xmlOfObject), new XmlReaderSettings()))
                {
                    return (T)new XmlSerializer(typeof(T)).Deserialize(xmlReader);
                }
            }
            catch (Exception) { return default; }
        }
        #endregion

        #region  xml序列化 
        /// <summary>
        /// xml序列化
        /// * 调用示列:
        /// * XmlMethod.XmlSerializer(typeof(myclass), mycl)
        /// * XmlMethod.XmlSerializer(typeof(DataTable), mytable)
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="obj">对象</param>
        /// <returns>返回xml字符串</returns>
        public static string XmlSerializer(Type type, object obj)
        {
            using (MemoryStream Stream = new MemoryStream())
            {
                XmlSerializer xml = new XmlSerializer(type);
                try
                {
                    //序列化对象
                    xml.Serialize(Stream, obj);
                }
                catch
                {
                    throw;
                }
                Stream.Position = 0;
                using (StreamReader sr = new StreamReader(Stream))
                {
                    return sr.ReadToEnd();
                    //sr.Dispose();
                    //return str;
                }
            }
        }

        /// <summary>
        /// Xml序列化
        /// </summary>
        /// <typeparam name="T">需要序列化的对象类型，必须声明[Serializable]特征</typeparam>
        /// <param name="obj">需要序列化的对象</param>
        /// <param name="omitXmlDeclaration">true:省略XML声明;否则为false.默认false，即编写 XML 声明。</param>
        /// <returns>序列化后的字符串</returns>
        public static string XmlSerialize<T>(T obj, bool omitXmlDeclaration = false)
        {
            /* 
            不能转换成Xml不能反序列化成为UTF8XML声明的情况，就是这个原因。
            */
            XmlWriterSettings xmlSettings = new XmlWriterSettings() { OmitXmlDeclaration = omitXmlDeclaration, Encoding = new UTF8Encoding(false) };
            using (MemoryStream stream = new MemoryStream())// var writer = new StringWriter();
            {
                using (XmlWriter xmlwriter = XmlWriter.Create(stream/*writer*/, xmlSettings))
                { //这里如果直接写成：Encoding = Encoding.UTF8 会在生成的xml中加入BOM(Byte-order Mark) 信息(Unicode 字节顺序标记) ， 所以new System.Text.UTF8Encoding(false)是最佳方式，省得再做替换的麻烦
                    XmlSerializerNamespaces xmlns = new XmlSerializerNamespaces();
                    xmlns.Add(string.Empty, string.Empty); //在XML序列化时去除默认命名空间xmlns:xsd和xmlns:xsi
                    XmlSerializer ser = new XmlSerializer(typeof(T));
                    ser.Serialize(xmlwriter, obj, xmlns);
                    return Encoding.UTF8.GetString(stream.ToArray());//writer.ToString()
                }
            }
        }
        /// <summary>
        /// Xml序列化为文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="obj">需要序列化的对象</param>
        /// <param name="omitXmlDeclaration">true:省略XML声明;否则为false.默认false，即编写 XML 声明。</param>
        /// <param name="removeDefaultNamespace">是否移除默认名称空间(如果对象定义时指定了:XmlRoot(Namespace = "http://www.xxx.com/xsd")则需要传false值进来)</param>
        public static void XmlSerialize<T>(string path, T obj, bool omitXmlDeclaration, bool removeDefaultNamespace)
        {
            XmlWriterSettings xmlSetings = new XmlWriterSettings() { OmitXmlDeclaration = omitXmlDeclaration };
            using (XmlWriter xmlwriter = XmlWriter.Create(path, xmlSetings))
            {
                XmlSerializerNamespaces xmlns = new XmlSerializerNamespaces();
                if (removeDefaultNamespace)
                    xmlns.Add(string.Empty, string.Empty); //在XML序列化时去除默认命名空间xmlns:xsd和xmlns:xsi
                XmlSerializer ser = new XmlSerializer(typeof(T));
                ser.Serialize(xmlwriter, obj, xmlns);
            }
        }
        #endregion

        #region  XML文件序列化存储 
        /// <summary>
        /// XML文件序列化存储
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="SavePath">存储文件路径</param>
        /// <param name="isDelOld"></param>
        public static bool SaveXmlSerializer(object obj, string SavePath, bool isDelOld = false)
        {

            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(SavePath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(SavePath));
                else
                {
                    try
                    {
                        if (File.Exists(SavePath) && isDelOld)
                            File.Delete(SavePath);
                    }
                    catch { }
                }
                using (FileStream fs = new FileStream(SavePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    XmlSerializer serializer = new XmlSerializer(obj.GetType());
                    serializer.Serialize(fs, obj);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        #endregion

        #region  XML文件反序列化加载 

        /// <summary>
        /// XML文件反序列化加载(读取为list时，文件父节点为ArrayOf类名)
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="filePath">加载文件路径</param>
        public static object LoadXmlDeserialize(Type type, string filePath)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    XmlSerializer serializer = new XmlSerializer(type);
                    return serializer.Deserialize(fs);
                }
            }
            catch (Exception ex)
            {
                if (ex is IOException)
                    return LoadXmlDeserialize(type, filePath);
                else
                    return null;
            }
        }

        #endregion

        #region  读取xml序列化为类 
        /// <summary>
        /// 读取xml序列化为类
        /// * 调用示例:
        /// * XmlMethod.ReadXMLFile（myclass)("d:\\atpConfig.xml")
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <param name="path">文件路径</param>
        /// <returns>序列化后类对象</returns>
        public static T ReadXmlFile<T>(string path)
        {
            XmlSerializer reader = new XmlSerializer(typeof(T));
            StreamReader file = new StreamReader(path);
            return (T)reader.Deserialize(file);
        }
        #endregion

        #region  将对象写入XML文件，失败时尝试5次. 
        /// <summary>
        /// 将对象写入XML文件,失败时尝试5次。
        /// </summary>
        /// <typeparam name="T">对象名</typeparam>
        /// <param name="obj">对象实例</param>
        /// <param name="xmlPath">路径 如C:\xmltest\201111send.xml</param>
        /// <returns>更新成功返回true，失败返回false</returns>
        public static bool WriteXML<T>(T obj, string xmlPath)
        {
            int i = 0;//控制写入文件的次数，
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            bool flag;
            while (true)
            {
                try
                {
                    //用filestream方式创建文件不会出现“文件正在占用中，用File.create”则不行
                    FileStream fs;
                    fs = File.Create(xmlPath);
                    fs.Close();
                    TextWriter writer = new StreamWriter(xmlPath, false, Encoding.UTF8);
                    XmlSerializerNamespaces xml = new XmlSerializerNamespaces();
                    xml.Add(string.Empty, string.Empty);
                    serializer.Serialize(writer, obj, xml);
                    writer.Flush();
                    writer.Close();
                    flag = true;
                    break;
                }
                catch
                {
                    if (i < 5)
                    {
                        i++;
                        continue;
                    }
                    else
                    {
                        flag = true;
                        break;
                    }
                }
            }
            return flag;
        }
        #endregion

        #region 从文件读取并反序列化为对象 （解决: 多线程或多进程下读写并发问题,失败时尝试5次。） 
        /// <summary>
        /// 从文件读取并反序列化为对象 （解决: 多线程或多进程下读写并发问题,失败时尝试5次。）
        /// </summary>
        /// <typeparam name="T">返回的对象类型</typeparam>
        /// <param name="path">文件地址</param>
        /// <returns>返回T对象</returns>
        public static T XmlFileDeserialize<T>(string path)
        {
            if (!File.Exists(path))
                return default;
            byte[] bytes = ShareReadFile(path);
            if (bytes.Length < 1)
            {
                //当文件正在被写入数据时，可能读出为0
                for (int i = 0; i < 5; i++)
                { 
                    //5次机会
                    bytes = ShareReadFile(path); // 采用这样诡异的做法避免独占文件和文件正在被写入时读出来的数据为0字节的问题。
                    if (bytes.Length > 0) break;
                    System.Threading.Thread.Sleep(50); //悲观情况下总共最多消耗1/4秒，读取文件
                }
            }
            XmlDocument doc = new XmlDocument();
            doc.Load(new MemoryStream(bytes));
            if (doc.DocumentElement != null)
                return (T)new XmlSerializer(typeof(T)).Deserialize(new XmlNodeReader(doc.DocumentElement));
            return default;
        }
        private static byte[] ShareReadFile(string filePath)
        {
            byte[] bytes;
            //避免"正由另一进程使用,因此该进程无法访问此文件"造成异常 共享锁 flieShare必须为ReadWrite，
            //但是如果文件不存在的话，还是会出现异常，所以这里不能吃掉任何异常，但是需要考虑到这些问题 
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                bytes = new byte[fs.Length];
                int numBytesToRead = (int)fs.Length;
                int numBytesRead = 0;
                while (numBytesToRead > 0)
                {
                    int n = fs.Read(bytes, numBytesRead, numBytesToRead);
                    if (n == 0)
                        break;
                    numBytesRead += n;
                    numBytesToRead -= n;
                }
            }
            return bytes;
        }
        #endregion
    }
}
