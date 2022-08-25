using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Xml.Serialization;
using SuperFramework.SuperConfig.Config;
namespace SuperFramework.SuperObjectPersistence
{
    /// <summary> 
    /// 将实体对象持久化的基类，子类将继承此基类即可实现持久化的功能
    /// </summary>
    /// <typeparam name="T"> 子类对象</typeparam>
    [Serializable]
    public class ObjectPersistence<T> where T : ObjectPersistence<T>, new()  
    {
        private static EventHandler eventHandler = null;
        #region XML
        /// <summary> 加载XML文件
        /// </summary>
        /// <returns> 返回对象</returns>
        public static T LoadFromXML()
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            string path = string.Format("{0}{1}.xml", AppDomain.CurrentDomain.BaseDirectory, typeof(T).Name);
            if (!File.Exists(path))
            {
                File.Create(path);
                T c = new T();
                c.SaveAsXML();
            }
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    return xs.Deserialize(sr) as T;
                }
            }
            catch
            {
                return new T();
            }
        }
        /// <summary> 加载XML文件
        /// </summary>
        /// <returns> 返回对象</returns>
        public static T LoadFromXMLWithoutCatch()
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            string path = string.Format("{0}{1}.xml", AppDomain.CurrentDomain.BaseDirectory, typeof(T).Name);
            if (!File.Exists(path))
            {
               File.Create(path);
                T c = new T();
                c.SaveAsXML();
            }
            using (StreamReader sr = new StreamReader(path))
            {
                return xs.Deserialize(sr) as T;
            }
        }

        /// <summary> 加载XML文件
        /// </summary>
        /// <returns> 返回对象</returns>
        public static T LoadFromXML(string path)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            if (!File.Exists(path))
            {
               File.Create(path);
                T c = new T();
                c.SaveAsXML(path);
            }
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    return xs.Deserialize(sr) as T;
                }
            }
            catch
            {
                return new T();
            }
        }

        /// <summary> 加载XML文件
        /// </summary>
        /// <returns> 返回对象</returns>
        public static T LoadFromXMLWithoutCatch(string path)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            if (!File.Exists(path))
            {
              File.Create(path);
                T c = new T();
                c.SaveAsXML(path);
            }
            using (StreamReader sr = new StreamReader(path))
            {
                return xs.Deserialize(sr) as T;
            }
        }

        /// <summary> 保存为XML文件
        /// </summary>
        public void SaveAsXML()
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            string path = string.Format("{0}{1}.xml", AppDomain.CurrentDomain.BaseDirectory, typeof(T).Name);
            using (StreamWriter sw = new StreamWriter(path))
            {
                xs.Serialize(sw, this);
            }
        }

        /// <summary> 保存为XML文件
        /// </summary>
        public void SaveAsXML(string path)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            using (StreamWriter sw = new StreamWriter(path))
            {
                xs.Serialize(sw, this);
            }
        }
        #endregion

        //#region Json

        ///// <summary> 将对象转换为Json字符串（该对象必需标记为可序列化）
        ///// </summary>
        ///// <returns> 返回Json字符串 </returns>
        //public string GetJsonString()
        //{
        //    //DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
        //    //using (MemoryStream stream = new MemoryStream())
        //    //{
        //    //    serializer.WriteObject(stream, this);
        //    //    byte[] dataBytes = new byte[stream.Length];
        //    //    stream.Position = 0;
        //    //    stream.Read(dataBytes, 0, (int)stream.Length);
        //    //    return Encoding.UTF8.GetString(dataBytes);
        //    //}
        //    try
        //    {
        //        return JsonConvert.SerializeObject(this);
        //    }
        //    catch
        //    {
        //        return "Error";
        //    }
        //}
        ///// <summary> 将对象转换为Json字符串（该对象必需标记为可序列化）
        ///// </summary>
        ///// <returns> 返回Json字符串 </returns>
        //public string GetJsonStringWithoutCatch()
        //{
        //    return JsonConvert.SerializeObject(this);
        //}

        ///// <summary> 加载Json字符串，并转化为对象（该对象必需标记为可序列化）
        ///// </summary>
        ///// <param name="jsonString"> Json字符串 </param>
        ///// <returns> 返回对象</returns>
        //public static T LoadJsonString(string jsonString)
        //{
        //    //JsonConvert.DeserializeObject<T>(
        //    //DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
        //    //using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
        //    //{
        //    //    return serializer.ReadObject(stream) as T;
        //    //}
        //    try
        //    {
        //        return JsonConvert.DeserializeObject(jsonString, typeof(T)) as T;
        //    }
        //    catch
        //    {
        //        return new T();
        //    }
        //}
        ///// <summary> 加载Json字符串，并转化为对象（该对象必需标记为可序列化）
        ///// </summary>
        ///// <param name="jsonString"> Json字符串 </param>
        ///// <returns> 返回对象</returns>
        //public static T LoadJsonStringWithoutCatch(string jsonString)
        //{
        //    return JsonConvert.DeserializeObject(jsonString, typeof(T)) as T;
        //}

        ///// <summary> 加载Json文件（该对象必需标记为可序列化）
        ///// </summary>
        ///// <returns> 返回对象</returns>
        //public static T LoadFromJson()
        //{
        //    string path = string.Format("{0}{1}.json", AppDomain.CurrentDomain.BaseDirectory, typeof(T).Name);
        //    if (!File.Exists(path))
        //    {
        //        using (FileStream fs = File.Create(path))
        //        {
        //        }
        //        T c = new T();
        //        c.SaveAsJson();
        //    }
        //    try
        //    {
        //        using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
        //        {
        //            using (StreamReader sr = new StreamReader(fs))
        //            {
        //                sr.BaseStream.Seek(0, SeekOrigin.Begin);
        //                string jsonString = sr.ReadToEnd();
        //                return LoadJsonString(jsonString) as T;
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        return new T();
        //    }
        //}

        ///// <summary> 加载Json文件（该对象必需标记为可序列化）
        ///// </summary>
        ///// <returns> 返回对象</returns>
        //public static T LoadFromJsonWithoutCatch()
        //{
        //    string path = string.Format("{0}{1}.json", AppDomain.CurrentDomain.BaseDirectory, typeof(T).Name);
        //    if (!File.Exists(path))
        //    {
        //        using (FileStream fs = File.Create(path))
        //        {
        //        }
        //        T c = new T();
        //        c.SaveAsJson();
        //    }
        //    using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
        //    {
        //        using (StreamReader sr = new StreamReader(fs))
        //        {
        //            sr.BaseStream.Seek(0, SeekOrigin.Begin);
        //            string jsonString = sr.ReadToEnd();
        //            return LoadJsonString(jsonString) as T;
        //        }
        //    }
        //}

        ///// <summary> 加载Json文件（该对象必需标记为可序列化）
        ///// </summary>
        ///// <returns> 返回对象</returns>
        //public static T LoadFromJson(string path)
        //{
        //    if (!File.Exists(path))
        //    {
        //        using (FileStream fs = File.Create(path))
        //        {
        //        }
        //        T c = new T();
        //        c.SaveAsJson(path);
        //    }
        //    try
        //    {
        //        using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
        //        {
        //            using (StreamReader sr = new StreamReader(fs))
        //            {
        //                sr.BaseStream.Seek(0, SeekOrigin.Begin);
        //                string jsonString = sr.ReadToEnd();
        //                return LoadJsonString(jsonString) as T;
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        return new T();
        //    }
        //}
        ///// <summary> 加载Json文件（该对象必需标记为可序列化）
        ///// </summary>
        ///// <returns> 返回对象</returns>
        //public static T LoadFromJsonWithoutCatch(string path)
        //{
        //    if (!File.Exists(path))
        //    {
        //        using (FileStream fs = File.Create(path))
        //        {
        //        }
        //        T c = new T();
        //        c.SaveAsJson(path);
        //    }
        //    using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
        //    {
        //        using (StreamReader sr = new StreamReader(fs))
        //        {
        //            sr.BaseStream.Seek(0, SeekOrigin.Begin);
        //            string jsonString = sr.ReadToEnd();
        //            return LoadJsonString(jsonString) as T;
        //        }
        //    }
        //}

        ///// <summary> 保存为Json文件（该对象必需标记为可序列化）注：如果字段有DateTime类型,务必要赋初值
        ///// </summary>
        //public void SaveAsJson()
        //{
        //    string path = string.Format("{0}{1}.json", AppDomain.CurrentDomain.BaseDirectory, typeof(T).Name);
        //    using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
        //    {
        //        using (StreamWriter sw = new StreamWriter(fs))
        //        {
        //            fs.SetLength(0);
        //            sw.Write(GetJsonString());
        //        }
        //    }
        //}

        ///// <summary> 保存为Json文件（该对象必需标记为可序列化）注：如果字段有DateTime类型,务必要赋初值
        ///// </summary>
        //public void SaveAsJson(string path)
        //{
        //    using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
        //    {
        //        using (StreamWriter sw = new StreamWriter(fs))
        //        {
        //            fs.SetLength(0);
        //            sw.Write(GetJsonString());
        //        }
        //    }
        //}

        //#endregion

        #region Binary

        /// <summary> 加载二进制流文件（该对象必需标记为可序列化）
        /// </summary>
        /// <returns> 返回对象</returns>
        public static T LoadFromBinary()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + typeof(T).Name + ".binary";
            if (!File.Exists(path))
            {
                using (FileStream fs = File.Create(path))
                {
                }
                T c = new T();
                c.SaveAsBinary();
            }
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    BinaryFormatter format = new BinaryFormatter();
                    return format.Deserialize(fs) as T;
                }
            }
            catch
            {
                return new T();
            }
        }
        /// <summary> 加载二进制流文件（该对象必需标记为可序列化）
        /// </summary>
        /// <returns> 返回对象</returns>
        public static T LoadFromBinaryWithoutCatch()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + typeof(T).Name + ".binary";
            if (!File.Exists(path))
            {
                using (FileStream fs = File.Create(path))
                {
                }
                T c = new T();
                c.SaveAsBinary();
            }
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                BinaryFormatter format = new BinaryFormatter();
                return format.Deserialize(fs) as T;
            }
        }

        /// <summary> 加载二进制流文件（该对象必需标记为可序列化）
        /// </summary>
        /// <returns> 返回对象</returns>
        public static T LoadFromBinary(string path)
        {
            if (!File.Exists(path))
            {
                using (FileStream fs = File.Create(path))
                {
                }
                T c = new T();
                c.SaveAsBinary(path);
            }
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    BinaryFormatter format = new BinaryFormatter();
                    return format.Deserialize(fs) as T;
                }
            }
            catch
            {
                return new T();
            }
        }

        /// <summary> 加载二进制流文件（该对象必需标记为可序列化）
        /// </summary>
        /// <returns> 返回对象</returns>
        public static T LoadFromBinaryWithoutCatch(string path)
        {
            if (!File.Exists(path))
            {
                using (FileStream fs = File.Create(path))
                {
                }
                T c = new T();
                c.SaveAsBinary(path);
            }
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                BinaryFormatter format = new BinaryFormatter();
                return format.Deserialize(fs) as T;
            }
        }

        /// <summary> 保存为二进制流文件（该对象必需标记为可序列化）
        /// </summary>
        public void SaveAsBinary()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter format = new BinaryFormatter();
                format.Serialize(stream, this);
                string path = AppDomain.CurrentDomain.BaseDirectory + typeof(T).Name + ".binary";
                byte[] fileByte = stream.GetBuffer();
                stream.Read(fileByte, 0, fileByte.Length);
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
                {
                    fs.Write(fileByte, 0, fileByte.Length); //创建这个文件
                }
            }
        }

        /// <summary> 保存为二进制流文件（该对象必需标记为可序列化）
        /// </summary>
        public void SaveAsBinary(string path)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter format = new BinaryFormatter();
                format.Serialize(stream, this);
                byte[] fileByte = stream.GetBuffer();
                stream.Read(fileByte, 0, fileByte.Length);
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
                {
                    fs.Write(fileByte, 0, fileByte.Length); //创建这个文件
                }
            }
        }

        /// <summary> 将对象转换为二进制流字节数组（该对象必需标记为可序列化）
        /// </summary>
        /// <returns> 返回字节数组 </returns>
        public byte[] GetBinaryBytes()
        {
            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    BinaryFormatter format = new BinaryFormatter();
                    format.Serialize(stream, this);
                    return stream.GetBuffer();
                }
            }
            catch
            {
                return null;
            }
        }
        /// <summary> 将对象转换为二进制流字节数组（该对象必需标记为可序列化）
        /// </summary>
        /// <returns> 返回字节数组 </returns>
        public byte[] GetBinaryBytesWithoutCatch()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter format = new BinaryFormatter();
                format.Serialize(stream, this);
                return stream.GetBuffer();
            }
        }

        /// <summary> 加载二进制流字节数组，并转化为对象（该对象必需标记为可序列化）
        /// </summary>
        /// <param name="bytes"> 二进制流字节数组 </param>
        /// <returns> 返回对象</returns>
        public static T LoadBinaryBytes(byte[] bytes)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream(bytes))
                {
                    BinaryFormatter format = new BinaryFormatter();
                    return format.Deserialize(stream) as T;
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary> 加载二进制流字节数组，并转化为对象（该对象必需标记为可序列化）
        /// </summary>
        /// <param name="bytes"> 二进制流字节数组 </param>
        /// <returns> 返回对象</returns>
        public static T LoadBinaryBytesWithoutCatch(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                BinaryFormatter format = new BinaryFormatter();
                return format.Deserialize(stream) as T;
            }
        }

        #endregion

        #region INI
        /// <summary> 
        /// 加载ini文件转成对象
        /// </summary>
        /// <returns>返回配置文件对象</returns>
        public static T LoadFromINI()
        {
            string path = string.Format("{0}{1}.ini", AppDomain.CurrentDomain.BaseDirectory, typeof(T).Name);
            if (!File.Exists(path))
            {
                File.Create(path);
                T c = new T();
                c.SaveAsINI(path);
                return c;
            }
            try
            {
                T t = new T();
                Dictionary<string, string> d = INIConfig.GetAllKeyValues(INIConfig.GetAllSectionNames(path).ToList().FirstOrDefault(f => f == typeof(T).Name), path);
                AnalysisINI2Object(t, typeof(T), d, path);
                return t;
            }
            catch
            {
                return new T();
            }
        }

        /// <summary> 
        /// 加载ini文件转成对象
        /// </summary>
        /// <param name="path">配置文件路径</param>
        /// <returns>返回配置文件对象</returns>
        public static T LoadFromINI(string path)
        {
            if (!File.Exists(path))
            {
                File.Create(path);
                T c = new T();
                c.SaveAsINI(path);
                return c;
            }
            try
            {
                T t = new T();
                Dictionary<string, string> d = INIConfig.GetAllKeyValues(INIConfig.GetAllSectionNames(path).ToList().FirstOrDefault(f => f == typeof(T).Name), path);
                AnalysisINI2Object(t, typeof(T), d, path);
                return t;
            }
            catch
            {
                return new T();
            }
        }
        /// <summary> 
        /// 加载ini文件转成对象
        /// </summary>
        /// <returns>返回配置文件对象</returns>
        public static T LoadFromINIWithoutCatch()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + typeof(T).Name + ".ini";
            if (!File.Exists(path))
            {
                File.Create(path);
                T c = new T();
                c.SaveAsINI(path);
                return c;
            }
            T t = new T();

            Dictionary<string, string> d = INIConfig.GetAllKeyValues(INIConfig.GetAllSectionNames(path).ToList().FirstOrDefault(f => f == typeof(T).Name), path);
            AnalysisINI2Object(t, typeof(T), d, path);
            return t;
        }
        /// <summary> 
        /// 加载ini文件转成对象
        /// </summary>
        /// <param name="path">配置文件路径</param>
        /// <returns>返回配置文件对象</returns>
        public static T LoadFromINIWithoutCatch(string path)
        {
            if (!File.Exists(path))
            {
                File.Create(path);
                T c = new T();
                c.SaveAsINI(path);
                return c;
            }
            T t = new T();
         
            Dictionary<string, string> d = INIConfig.GetAllKeyValues(INIConfig.GetAllSectionNames(path).ToList().FirstOrDefault(f => f == typeof(T).Name), path);
            AnalysisINI2Object(t, typeof(T), d, path);
            return t;
        }

        private static object AnalysisINI2Object(object obj, Type t, Dictionary<string, string> d,string path)
        {
            PropertyInfo[] ps = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo p in ps)
            {
                if (p.PropertyType.FullName == typeof(string).FullName && p.CanWrite)
                {
                    var keyValuePair = d.FirstOrDefault(f => f.Key == p.Name);
                    if (keyValuePair.Key != null)
                    {
                        p.SetValue(obj, d.FirstOrDefault(f => f.Key == p.Name).Value, null);
                    }
                }
                else if (p.PropertyType.FullName == typeof(char).FullName || p.PropertyType.FullName == typeof(char?).FullName)
                {
                    char result;
                    var keyValuePair = d.FirstOrDefault(f => f.Key == p.Name);
                    if (keyValuePair.Key != null && char.TryParse(keyValuePair.Value, out result) && p.CanWrite)
                    {
                        p.SetValue(obj, result, null);
                    }
                }
                else if (p.PropertyType.FullName == typeof(byte).FullName || p.PropertyType.FullName == typeof(byte?).FullName)
                {
                    byte result;
                    var keyValuePair = d.FirstOrDefault(f => f.Key == p.Name);
                    if (keyValuePair.Key != null && byte.TryParse(keyValuePair.Value, out result) && p.CanWrite)
                    {
                        p.SetValue(obj, result, null);
                    }
                }
                else if (p.PropertyType.FullName == typeof(short).FullName || p.PropertyType.FullName == typeof(short?).FullName)
                {
                    short result;
                    var keyValuePair = d.FirstOrDefault(f => f.Key == p.Name);
                    if (keyValuePair.Key != null && short.TryParse(keyValuePair.Value, out result) && p.CanWrite)
                    {
                        p.SetValue(obj, result, null);
                    }
                }
                else if (p.PropertyType.FullName == typeof(ushort).FullName || p.PropertyType.FullName == typeof(ushort?).FullName)
                {
                    ushort result;
                    var keyValuePair = d.FirstOrDefault(f => f.Key == p.Name);
                    if (keyValuePair.Key != null && ushort.TryParse(keyValuePair.Value, out result) && p.CanWrite)
                    {
                        p.SetValue(obj, result, null);
                    }
                }
                else if (p.PropertyType.FullName == typeof(int).FullName || p.PropertyType.FullName == typeof(int?).FullName)
                {
                    int result;
                    var keyValuePair = d.FirstOrDefault(f => f.Key == p.Name);
                    if (keyValuePair.Key != null && int.TryParse(keyValuePair.Value, out result) && p.CanWrite)
                    {
                        p.SetValue(obj, result, null);
                    }
                }
                else if (p.PropertyType.FullName == typeof(uint).FullName || p.PropertyType.FullName == typeof(uint?).FullName)
                {
                    uint result;
                    var keyValuePair = d.FirstOrDefault(f => f.Key == p.Name);
                    if (keyValuePair.Key != null && uint.TryParse(keyValuePair.Value, out result) && p.CanWrite)
                    {
                        p.SetValue(obj, result, null);
                    }
                }
                else if (p.PropertyType.FullName == typeof(long).FullName || p.PropertyType.FullName == typeof(long?).FullName)
                {
                    long result;
                    var keyValuePair = d.FirstOrDefault(f => f.Key == p.Name);
                    if (keyValuePair.Key != null && long.TryParse(keyValuePair.Value, out result) && p.CanWrite)
                    {
                        p.SetValue(obj, result, null);
                    }
                }
                else if (p.PropertyType.FullName == typeof(ulong).FullName || p.PropertyType.FullName == typeof(ulong?).FullName)
                {
                    ulong result;
                    var keyValuePair = d.FirstOrDefault(f => f.Key == p.Name);
                    if (keyValuePair.Key != null && ulong.TryParse(keyValuePair.Value, out result) && p.CanWrite)
                    {
                        p.SetValue(obj, result, null);
                    }
                }
                else if (p.PropertyType.FullName == typeof(float).FullName || p.PropertyType.FullName == typeof(float?).FullName)
                {
                    float result;
                    var keyValuePair = d.FirstOrDefault(f => f.Key == p.Name);
                    if (keyValuePair.Key != null && float.TryParse(keyValuePair.Value, out result) && p.CanWrite)
                    {
                        p.SetValue(obj, result, null);
                    }
                }
                else if (p.PropertyType.FullName == typeof(double).FullName || p.PropertyType.FullName == typeof(double?).FullName)
                {
                    double result;
                    var keyValuePair = d.FirstOrDefault(f => f.Key == p.Name);
                    if (keyValuePair.Key != null && double.TryParse(keyValuePair.Value, out result) && p.CanWrite)
                    {
                        p.SetValue(obj, result, null);
                    }
                }
                else if (p.PropertyType.FullName == typeof(bool).FullName || p.PropertyType.FullName == typeof(bool?).FullName)
                {
                    bool result;
                    var keyValuePair = d.FirstOrDefault(f => f.Key == p.Name);
                    if (keyValuePair.Key != null && bool.TryParse(keyValuePair.Value, out result) && p.CanWrite)
                    {
                        p.SetValue(obj, result, null);
                    }
                }
                else if (p.PropertyType.FullName == typeof(DateTime).FullName || p.PropertyType.FullName == typeof(DateTime?).FullName)
                {
                    DateTime result;
                    var keyValuePair = d.FirstOrDefault(f => f.Key == p.Name);
                    if (keyValuePair.Key != null && DateTime.TryParse(keyValuePair.Value, out result) && p.CanWrite)
                    {
                        p.SetValue(obj, result, null);
                    }
                }
                else if (p.PropertyType.FullName == typeof(Rectangle).FullName || p.PropertyType.FullName == typeof(Rectangle?).FullName)
                {
                    var keyValuePair = d.FirstOrDefault(f => f.Key == p.Name);
                    if (keyValuePair.Key != null && keyValuePair.Value != null && keyValuePair.Value != "")
                    {
                        string[] value = keyValuePair.Value.Replace("{", "").Replace("}", "").Split(',');
                        int x, y, width, height;
                        if (int.TryParse(value[0].Split('=')[1], out x) && int.TryParse(value[1].Split('=')[1], out y) && int.TryParse(value[2].Split('=')[1], out width) && int.TryParse(value[3].Split('=')[1], out height) && p.CanWrite)
                        {
                            p.SetValue(obj, new Rectangle(x, y, width, height), null);
                        }
                    }
                }
                else if (p.PropertyType.FullName == typeof(RectangleF).FullName || p.PropertyType.FullName == typeof(RectangleF?).FullName)
                {
                    var keyValuePair = d.FirstOrDefault(f => f.Key == p.Name);
                    if (keyValuePair.Key != null && keyValuePair.Value != null && keyValuePair.Value != "")
                    {
                        string[] value = keyValuePair.Value.Replace("{", "").Replace("}", "").Split(',');
                        float x, y, width, height;
                        if (float.TryParse(value[0].Split('=')[1], out x) && float.TryParse(value[1].Split('=')[1], out y) && float.TryParse(value[2].Split('=')[1], out width) && float.TryParse(value[3].Split('=')[1], out height) && p.CanWrite)
                        {
                            p.SetValue(obj, new RectangleF(x, y, width, height), null);
                        }
                    }
                }
                else if (p.PropertyType.FullName == typeof(Point).FullName || p.PropertyType.FullName == typeof(Point?).FullName)
                {
                    var keyValuePair = d.FirstOrDefault(f => f.Key == p.Name);
                    if (keyValuePair.Key != null && keyValuePair.Value != null && keyValuePair.Value != "")
                    {
                        string[] value = keyValuePair.Value.Replace("{", "").Replace("}", "").Split(',');
                        int x, y;
                        if (int.TryParse(value[0].Split('=')[1], out x) && int.TryParse(value[1].Split('=')[1], out y) && p.CanWrite)
                        {
                            p.SetValue(obj, new Point(x, y), null);
                        }
                    }
                }
                else if (p.PropertyType.FullName == typeof(PointF).FullName || p.PropertyType.FullName == typeof(PointF?).FullName)
                {
                    var keyValuePair = d.FirstOrDefault(f => f.Key == p.Name);
                    if (keyValuePair.Key != null && keyValuePair.Value != null && keyValuePair.Value != "")
                    {
                        string[] value = keyValuePair.Value.Replace("{", "").Replace("}", "").Split(',');
                        float x, y;
                        if (float.TryParse(value[0].Split('=')[1], out x) && float.TryParse(value[1].Split('=')[1], out y) && p.CanWrite)
                        {
                            p.SetValue(obj, new PointF(x, y), null);
                        }
                    }
                }
                else if (p.PropertyType.FullName == typeof(Size).FullName || p.PropertyType.FullName == typeof(Size?).FullName)
                {
                    var keyValuePair = d.FirstOrDefault(f => f.Key == p.Name);
                    if (keyValuePair.Key != null && keyValuePair.Value != null && keyValuePair.Value != "")
                    {
                        string[] value = keyValuePair.Value.Replace("{", "").Replace("}", "").Split(',');
                        int width, height;
                        if (int.TryParse(value[0].Split('=')[1], out width) && int.TryParse(value[1].Split('=')[1], out height) && p.CanWrite)
                        {
                            p.SetValue(obj, new Size(width, height), null);
                        }
                    }
                }
                else if (p.PropertyType.FullName == typeof(SizeF).FullName || p.PropertyType.FullName == typeof(SizeF?).FullName)
                {
                    var keyValuePair = d.FirstOrDefault(f => f.Key == p.Name);
                    if (keyValuePair.Key != null && keyValuePair.Value != null && keyValuePair.Value != "")
                    {
                        string[] value = keyValuePair.Value.Replace("{", "").Replace("}", "").Split(',');
                        float width, height;
                        if (float.TryParse(value[0].Split('=')[1], out width) && float.TryParse(value[1].Split('=')[1], out height) && p.CanWrite)
                        {
                            p.SetValue(obj, new SizeF(width, height), null);
                        }
                    }
                }
                else if (p.PropertyType.FullName == typeof(byte[]).FullName)
                {
                    var keyValuePair = d.FirstOrDefault(f => f.Key == p.Name);
                    if (keyValuePair.Key != null && keyValuePair.Value != null && keyValuePair.Value != "" && p.CanWrite)
                    {
                        p.SetValue(obj, Convert.FromBase64String(keyValuePair.Value), null);
                    }
                }
                else if (p.PropertyType.FullName == typeof(Color).FullName || p.PropertyType.FullName == typeof(Color?).FullName)
                {
                    var keyValuePair = d.FirstOrDefault(f => f.Key == p.Name);
                    if (keyValuePair.Key != null && keyValuePair.Value != null && keyValuePair.Value != "" && p.CanWrite)
                    {
                        p.SetValue(obj, ColorTranslator.FromHtml(keyValuePair.Value), null);
                    }
                }
                else if (p.PropertyType.FullName == typeof(Image).FullName)
                {
                    if (p.CanWrite)
                    {
                        p.SetValue(obj, null, null);
                    }
                }
                else if (p.PropertyType.Name == typeof(List<>).Name && p.CanWrite)
                {
                    var keyValuePair = d.FirstOrDefault(f => f.Key == p.Name);
                    if (keyValuePair.Key != null && keyValuePair.Value != null && keyValuePair.Value != "")
                    {
                        string[] listValue = keyValuePair.Value.Split('|');
                        IList list = (IList)p.GetValue(obj, null);
                        for (int i = 0; i < listValue.Length; i++)
                        {
                            Dictionary<string, string> dd = INIConfig.GetAllKeyValues(listValue[i], path);
                            if (list.GetType().IsGenericType)
                            {
                                if (list.GetType().GetGenericArguments().Length > 0)
                                {
                                    Type tType = list.GetType().GetGenericArguments()[0];
                                    if (tType.FullName == typeof(string).FullName ||
                                        tType.FullName == typeof(char).FullName ||
                                        tType.FullName == typeof(byte).FullName ||
                                        tType.FullName == typeof(short).FullName ||
                                        tType.FullName == typeof(ushort).FullName ||
                                        tType.FullName == typeof(int).FullName ||
                                        tType.FullName == typeof(uint).FullName ||
                                        tType.FullName == typeof(long).FullName ||
                                        tType.FullName == typeof(ulong).FullName ||
                                        tType.FullName == typeof(float).FullName ||
                                        tType.FullName == typeof(decimal).FullName ||
                                        tType.FullName == typeof(double).FullName ||
                                        tType.FullName == typeof(bool).FullName ||
                                        tType.FullName == typeof(byte[]).FullName ||
                                        tType.FullName == typeof(char?).FullName ||
                                        tType.FullName == typeof(byte?).FullName ||
                                        tType.FullName == typeof(short?).FullName ||
                                        tType.FullName == typeof(ushort?).FullName ||
                                        tType.FullName == typeof(int?).FullName ||
                                        tType.FullName == typeof(uint?).FullName ||
                                        tType.FullName == typeof(long?).FullName ||
                                        tType.FullName == typeof(ulong?).FullName ||
                                        tType.FullName == typeof(float?).FullName ||
                                        tType.FullName == typeof(decimal?).FullName ||
                                        tType.FullName == typeof(double?).FullName ||
                                        tType.FullName == typeof(bool?).FullName)
                                    {
                                        if (dd.FirstOrDefault(f => f.Key == tType.Name).Value != null && dd.FirstOrDefault(f => f.Key == tType.Name).Value != "")
                                        { list.Add(dd.FirstOrDefault(f => f.Key == tType.Name).Value); }
                                    }
                                    else
                                    {
                                        list.Add(AnalysisINI2Object(Activator.CreateInstance(tType), tType, dd, path));
                                    }
                                }
                            }
                        }
                        p.SetValue(obj, list, null);
                    }
                }
                else
                {
                    p.SetValue(obj, AnalysisINI2Object(Activator.CreateInstance(p.PropertyType), p.PropertyType, d, path), null);
                }
            }
            return obj;
        }

        /// <summary> 
        /// 将对象转换为ini文件
        /// </summary>
        /// <param name="path">配置文件路径</param>
        public void SaveAsINI(string path)
        {
           
            AnalysisObject2INI(typeof(T), typeof(T).Name, "", this,path);
        }

        private static void AnalysisObject2INI(Type t, string sessionName, string name, object o,string path)
        {
            if (t.FullName == typeof(string).FullName ||
                t.FullName == typeof(char).FullName ||
                t.FullName == typeof(byte).FullName ||
                t.FullName == typeof(short).FullName ||
                t.FullName == typeof(ushort).FullName ||
                t.FullName == typeof(int).FullName ||
                t.FullName == typeof(uint).FullName ||
                t.FullName == typeof(long).FullName ||
                t.FullName == typeof(ulong).FullName ||
                t.FullName == typeof(float).FullName ||
                t.FullName == typeof(decimal).FullName ||
                t.FullName == typeof(double).FullName ||
                t.FullName == typeof(bool).FullName ||
                t.FullName == typeof(DateTime).FullName ||
                t.FullName == typeof(Rectangle).FullName ||
                t.FullName == typeof(RectangleF).FullName ||
                t.FullName == typeof(Point).FullName ||
                t.FullName == typeof(PointF).FullName ||
                t.FullName == typeof(Size).FullName ||
                t.FullName == typeof(SizeF).FullName)
            {
                if (o != null)
                {
                    INIConfig.Write(sessionName, name, o.ToString(),path);
                }
            }
            else if (t.FullName == typeof(char?).FullName ||
                     t.FullName == typeof(byte?).FullName ||
                     t.FullName == typeof(short?).FullName ||
                     t.FullName == typeof(ushort?).FullName ||
                     t.FullName == typeof(int?).FullName ||
                     t.FullName == typeof(uint?).FullName ||
                     t.FullName == typeof(long?).FullName ||
                     t.FullName == typeof(ulong?).FullName ||
                     t.FullName == typeof(float?).FullName ||
                     t.FullName == typeof(decimal?).FullName ||
                     t.FullName == typeof(double?).FullName ||
                     t.FullName == typeof(bool?).FullName ||
                     t.FullName == typeof(DateTime?).FullName ||
                     t.FullName == typeof(Rectangle?).FullName ||
                     t.FullName == typeof(RectangleF?).FullName ||
                     t.FullName == typeof(Point?).FullName ||
                     t.FullName == typeof(PointF?).FullName ||
                     t.FullName == typeof(Size?).FullName ||
                     t.FullName == typeof(SizeF?).FullName)
            {
                if (o != null)
                {
                    INIConfig.Write(sessionName, name, o.ToString(),path);
                }
            }
            else if (t.FullName == typeof(byte[]).FullName)
            {
                if (o != null)
                {
                    INIConfig.Write(sessionName, name, Convert.ToBase64String((byte[])o),path);
                }
            }
            else if (t.FullName == typeof(Color).FullName)
            {
                if (o != null)
                {
                    INIConfig.Write(sessionName, name, ColorTranslator.ToHtml((Color)o),path);
                }
            }
            else if (t.FullName == typeof(Color?).FullName)
            {
                if (o != null)
                {
                    INIConfig.Write(sessionName, name, ColorTranslator.ToHtml((Color)o),path);
                }
            }
            else if (t.Name == typeof(List<>).Name)
            {
                IList list = (IList)o;
                string value = "";
                for (int i = 1; i <= list.Count; i++)
                {
                    value += sessionName + "*" + name + i + "|";
                }
                if (value != "")
                {
                    value = value.Remove(value.Length - 1);
                }
                INIConfig.Write(sessionName, name, value,path);
                for (int i = 1; i <= list.Count; i++)
                {
                    AnalysisObject2INI(((object)list[i - 1]).GetType(), sessionName + "*" + name + i, ((object)list[i - 1]).GetType().Name, ((object)list[i - 1]),path);
                }
            }
            else if (t.Name == typeof(Image).Name)
            {
                return;
            }
            else
            {
                if (o != null)
                {
                    PropertyInfo[] ps = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (PropertyInfo p in ps)
                    {
                        AnalysisObject2INI(p.PropertyType, sessionName, p.Name, p.GetValue(o, null),path);
                    }
                }
            }
        }

        #endregion

        #region Registry

        /// <summary> 
        /// 加载注册表转成对象
        /// </summary>
        /// <returns>返回注册表信息对象</returns>
        public static T LoadFromRegistry()
        {
            if (Registry.CurrentUser.OpenSubKey("SOFTWARE", true).GetSubKeyNames().Count(s => s ==  typeof(T).Name) == 0)
            {
                Registry.CurrentUser.OpenSubKey("SOFTWARE", true).CreateSubKey( typeof(T).Name);
                T c = new T();
                c.SaveAsRegistry();
                return c;
            }
            try
            {
                T t = new T();
                Dictionary<string, string> d = new Dictionary<string, string>();
                string[] valueNames = Registry.CurrentUser.OpenSubKey("SOFTWARE", true).OpenSubKey( typeof(T).Name, true).GetValueNames();
                for (int i = 0; i < valueNames.Length; i++)
                {
                    d.Add(valueNames[i], Registry.CurrentUser.OpenSubKey("SOFTWARE", true).OpenSubKey( typeof(T).Name, true).GetValue(valueNames[i]).ToString());
                }
                AnalysisRegistry2Object(t, typeof(T), d, Registry.CurrentUser.OpenSubKey("SOFTWARE", true).OpenSubKey( typeof(T).Name, true));
                return t;
            }
            catch
            {
                return new T();
            }
        }
        /// <summary> 
        /// 加载注册表转成对象
        /// </summary>
        /// <returns>返回注册表信息对象</returns>
        public static T LoadFromRegistryWithoutCatch()
        {
            if (Registry.CurrentUser.OpenSubKey("SOFTWARE", true).GetSubKeyNames().Count(s => s ==  typeof(T).Name) == 0)
            {
                Registry.CurrentUser.OpenSubKey("SOFTWARE", true).CreateSubKey( typeof(T).Name);
                T c = new T();
                c.SaveAsRegistry();
                return c;
            }
            T t = new T();
            Dictionary<string, string> d = new Dictionary<string, string>();
            string[] valueNames = Registry.CurrentUser.OpenSubKey("SOFTWARE", true).OpenSubKey( typeof(T).Name, true).GetValueNames();
            for (int i = 0; i < valueNames.Length; i++)
            {
                d.Add(valueNames[i], Registry.CurrentUser.OpenSubKey("SOFTWARE", true).OpenSubKey( typeof(T).Name, true).GetValue(valueNames[i]).ToString());
            }
            AnalysisRegistry2Object(t, typeof(T), d, Registry.CurrentUser.OpenSubKey("SOFTWARE", true).OpenSubKey( typeof(T).Name, true));
            return t;
        }

        private static object AnalysisRegistry2Object(object obj, Type t, Dictionary<string, string> d, RegistryKey registryKey)
        {
            PropertyInfo[] ps = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo p in ps)
            {
                if (p.PropertyType.FullName == typeof(string).FullName)
                {
                    p.SetValue(obj, d.FirstOrDefault(f => f.Key == p.Name).Value, null);
                }
                else if (p.PropertyType.FullName == typeof(char).FullName || p.PropertyType.FullName == typeof(char?).FullName)
                {
                    char result;
                    var aaa = d.FirstOrDefault(f => f.Key == p.Name).Value;
                    if (char.TryParse(d.FirstOrDefault(f => f.Key == p.Name).Value, out result))
                    {
                        p.SetValue(obj, result, null);
                    }
                }
                else if (p.PropertyType.FullName == typeof(byte).FullName || p.PropertyType.FullName == typeof(byte?).FullName)
                {
                    byte result;
                    if (byte.TryParse(d.FirstOrDefault(f => f.Key == p.Name).Value, out result))
                    {
                        p.SetValue(obj, result, null);
                    }
                }
                else if (p.PropertyType.FullName == typeof(short).FullName || p.PropertyType.FullName == typeof(short?).FullName)
                {
                    short result;
                    if (short.TryParse(d.FirstOrDefault(f => f.Key == p.Name).Value, out result))
                    {
                        p.SetValue(obj, result, null);
                    }
                }
                else if (p.PropertyType.FullName == typeof(ushort).FullName || p.PropertyType.FullName == typeof(ushort?).FullName)
                {
                    ushort result;
                    if (ushort.TryParse(d.FirstOrDefault(f => f.Key == p.Name).Value, out result))
                    {
                        p.SetValue(obj, result, null);
                    }
                }
                else if (p.PropertyType.FullName == typeof(int).FullName || p.PropertyType.FullName == typeof(int?).FullName)
                {
                    int result;
                    if (int.TryParse(d.FirstOrDefault(f => f.Key == p.Name).Value, out result))
                    {
                        p.SetValue(obj, result, null);
                    }
                }
                else if (p.PropertyType.FullName == typeof(uint).FullName || p.PropertyType.FullName == typeof(uint?).FullName)
                {
                    uint result;
                    if (uint.TryParse(d.FirstOrDefault(f => f.Key == p.Name).Value, out result))
                    {
                        p.SetValue(obj, result, null);
                    }
                }
                else if (p.PropertyType.FullName == typeof(long).FullName || p.PropertyType.FullName == typeof(long?).FullName)
                {
                    long result;
                    if (long.TryParse(d.FirstOrDefault(f => f.Key == p.Name).Value, out result))
                    {
                        p.SetValue(obj, result, null);
                    }
                }
                else if (p.PropertyType.FullName == typeof(ulong).FullName || p.PropertyType.FullName == typeof(ulong?).FullName)
                {
                    ulong result;
                    if (ulong.TryParse(d.FirstOrDefault(f => f.Key == p.Name).Value, out result))
                    {
                        p.SetValue(obj, result, null);
                    }
                }
                else if (p.PropertyType.FullName == typeof(float).FullName || p.PropertyType.FullName == typeof(float?).FullName)
                {
                    float result;
                    if (float.TryParse(d.FirstOrDefault(f => f.Key == p.Name).Value, out result))
                    {
                        p.SetValue(obj, result, null);
                    }
                }
                else if (p.PropertyType.FullName == typeof(double).FullName || p.PropertyType.FullName == typeof(double?).FullName)
                {
                    double result;
                    if (double.TryParse(d.FirstOrDefault(f => f.Key == p.Name).Value, out result))
                    {
                        p.SetValue(obj, result, null);
                    }
                }
                else if (p.PropertyType.FullName == typeof(bool).FullName || p.PropertyType.FullName == typeof(bool?).FullName)
                {
                    bool result;
                    if (bool.TryParse(d.FirstOrDefault(f => f.Key == p.Name).Value, out result))
                    {
                        p.SetValue(obj, result, null);
                    }
                }
                else if (p.PropertyType.FullName == typeof(DateTime).FullName || p.PropertyType.FullName == typeof(DateTime?).FullName)
                {
                    DateTime result;
                    if (DateTime.TryParse(d.FirstOrDefault(f => f.Key == p.Name).Value, out result))
                    {
                        p.SetValue(obj, result, null);
                    }
                }
                else if (p.PropertyType.FullName == typeof(Rectangle).FullName || p.PropertyType.FullName == typeof(Rectangle?).FullName)
                {
                    if (d.FirstOrDefault(f => f.Key == p.Name).Value != null && d.FirstOrDefault(f => f.Key == p.Name).Value != "")
                    {
                        string[] value = d.FirstOrDefault(f => f.Key == p.Name).Value.Replace("{", "").Replace("}", "").Split(',');
                        int x, y, width, height;
                        if (int.TryParse(value[0].Split('=')[1], out x) && int.TryParse(value[1].Split('=')[1], out y) && int.TryParse(value[2].Split('=')[1], out width) && int.TryParse(value[3].Split('=')[1], out height))
                        {
                            p.SetValue(obj, new Rectangle(x, y, width, height), null);
                        }
                    }
                }
                else if (p.PropertyType.FullName == typeof(RectangleF).FullName || p.PropertyType.FullName == typeof(RectangleF?).FullName)
                {
                    if (d.FirstOrDefault(f => f.Key == p.Name).Value != null && d.FirstOrDefault(f => f.Key == p.Name).Value != "")
                    {
                        string[] value = d.FirstOrDefault(f => f.Key == p.Name).Value.Replace("{", "").Replace("}", "").Split(',');
                        float x, y, width, height;
                        if (float.TryParse(value[0].Split('=')[1], out x) && float.TryParse(value[1].Split('=')[1], out y) && float.TryParse(value[2].Split('=')[1], out width) && float.TryParse(value[3].Split('=')[1], out height))
                        {
                            p.SetValue(obj, new RectangleF(x, y, width, height), null);
                        }
                    }
                }
                else if (p.PropertyType.FullName == typeof(Point).FullName || p.PropertyType.FullName == typeof(Point?).FullName)
                {
                    if (d.FirstOrDefault(f => f.Key == p.Name).Value != null && d.FirstOrDefault(f => f.Key == p.Name).Value != "")
                    {
                        string[] value = d.FirstOrDefault(f => f.Key == p.Name).Value.Replace("{", "").Replace("}", "").Split(',');
                        int x, y;
                        if (int.TryParse(value[0].Split('=')[1], out x) && int.TryParse(value[1].Split('=')[1], out y))
                        {
                            p.SetValue(obj, new Point(x, y), null);
                        }
                    }
                }
                else if (p.PropertyType.FullName == typeof(PointF).FullName || p.PropertyType.FullName == typeof(PointF?).FullName)
                {
                    if (d.FirstOrDefault(f => f.Key == p.Name).Value != null && d.FirstOrDefault(f => f.Key == p.Name).Value != "")
                    {
                        string[] value = d.FirstOrDefault(f => f.Key == p.Name).Value.Replace("{", "").Replace("}", "").Split(',');
                        float x, y;
                        if (float.TryParse(value[0].Split('=')[1], out x) && float.TryParse(value[1].Split('=')[1], out y))
                        {
                            p.SetValue(obj, new PointF(x, y), null);
                        }
                    }
                }
                else if (p.PropertyType.FullName == typeof(Size).FullName || p.PropertyType.FullName == typeof(Size?).FullName)
                {
                    if (d.FirstOrDefault(f => f.Key == p.Name).Value != null && d.FirstOrDefault(f => f.Key == p.Name).Value != "")
                    {
                        string[] value = d.FirstOrDefault(f => f.Key == p.Name).Value.Replace("{", "").Replace("}", "").Split(',');
                        int width, height;
                        if (int.TryParse(value[0].Split('=')[1], out width) && int.TryParse(value[1].Split('=')[1], out height))
                        {
                            p.SetValue(obj, new Size(width, height), null);
                        }
                    }
                }
                else if (p.PropertyType.FullName == typeof(SizeF).FullName || p.PropertyType.FullName == typeof(SizeF?).FullName)
                {
                    if (d.FirstOrDefault(f => f.Key == p.Name).Value != null && d.FirstOrDefault(f => f.Key == p.Name).Value != "")
                    {
                        string[] value = d.FirstOrDefault(f => f.Key == p.Name).Value.Replace("{", "").Replace("}", "").Split(',');
                        float width, height;
                        if (float.TryParse(value[0].Split('=')[1], out width) && float.TryParse(value[1].Split('=')[1], out height))
                        {
                            p.SetValue(obj, new SizeF(width, height), null);
                        }
                    }
                }
                else if (p.PropertyType.FullName == typeof(byte[]).FullName)
                {
                    if (d.FirstOrDefault(f => f.Key == p.Name).Value != null && d.FirstOrDefault(f => f.Key == p.Name).Value != "")
                    {
                        p.SetValue(obj, Convert.FromBase64String(d.FirstOrDefault(f => f.Key == p.Name).Value), null);
                    }
                }
                else if (p.PropertyType.FullName == typeof(Color).FullName || p.PropertyType.FullName == typeof(Color?).FullName)
                {
                    if (d.FirstOrDefault(f => f.Key == p.Name).Value != null && d.FirstOrDefault(f => f.Key == p.Name).Value != "")
                    {
                        p.SetValue(obj, ColorTranslator.FromHtml(d.FirstOrDefault(f => f.Key == p.Name).Value), null);
                    }
                }
                else if (p.PropertyType.Name == typeof(List<>).Name)
                {
                    if (d.FirstOrDefault(f => f.Key == p.Name).Value != null && d.FirstOrDefault(f => f.Key == p.Name).Value != "")
                    {
                        string[] listValue = d.FirstOrDefault(f => f.Key == p.Name).Value.Split('|');
                        IList list = (IList)p.GetValue(obj, null);
                        for (int i = 0; i < listValue.Length; i++)
                        {
                            Dictionary<string, string> dd = new Dictionary<string, string>();
                            string[] valueNames = registryKey.OpenSubKey(listValue[i], true).GetValueNames();
                            for (int vi = 0; vi < valueNames.Length; vi++)
                            {
                                dd.Add(valueNames[vi], registryKey.OpenSubKey(listValue[i], true).GetValue(valueNames[vi]).ToString());
                            }
                            if (list.GetType().IsGenericType)
                            {
                                if (list.GetType().GetGenericArguments().Length > 0)
                                {
                                    Type tType = list.GetType().GetGenericArguments()[0];
                                    if (tType.FullName == typeof(string).FullName ||
                                        tType.FullName == typeof(char).FullName ||
                                        tType.FullName == typeof(byte).FullName ||
                                        tType.FullName == typeof(short).FullName ||
                                        tType.FullName == typeof(ushort).FullName ||
                                        tType.FullName == typeof(int).FullName ||
                                        tType.FullName == typeof(uint).FullName ||
                                        tType.FullName == typeof(long).FullName ||
                                        tType.FullName == typeof(ulong).FullName ||
                                        tType.FullName == typeof(float).FullName ||
                                        tType.FullName == typeof(decimal).FullName ||
                                        tType.FullName == typeof(double).FullName ||
                                        tType.FullName == typeof(bool).FullName ||
                                        tType.FullName == typeof(byte[]).FullName ||
                                        tType.FullName == typeof(char?).FullName ||
                                        tType.FullName == typeof(byte?).FullName ||
                                        tType.FullName == typeof(short?).FullName ||
                                        tType.FullName == typeof(ushort?).FullName ||
                                        tType.FullName == typeof(int?).FullName ||
                                        tType.FullName == typeof(uint?).FullName ||
                                        tType.FullName == typeof(long?).FullName ||
                                        tType.FullName == typeof(ulong?).FullName ||
                                        tType.FullName == typeof(float?).FullName ||
                                        tType.FullName == typeof(decimal?).FullName ||
                                        tType.FullName == typeof(double?).FullName ||
                                        tType.FullName == typeof(bool?).FullName)
                                    {
                                        if (dd.FirstOrDefault(f => f.Key == tType.Name).Value != null && dd.FirstOrDefault(f => f.Key == tType.Name).Value != "")
                                        { list.Add(dd.FirstOrDefault(f => f.Key == tType.Name).Value); }
                                    }
                                    else
                                    {
                                        list.Add(AnalysisRegistry2Object(Activator.CreateInstance(tType), tType, dd, registryKey.OpenSubKey(listValue[i], true)));
                                    }
                                }
                            }
                        }
                        p.SetValue(obj, list, null);
                    }
                }
                else
                {
                }
            }
            return obj;
        }

        /// <summary> 将对象转为注册表
        /// </summary>
        public void SaveAsRegistry()
        {
            if (Registry.CurrentUser.OpenSubKey("SOFTWARE", true).GetSubKeyNames().Count(s => s ==  typeof(T).Name) == 0)
            {
                Registry.CurrentUser.OpenSubKey("SOFTWARE", true).CreateSubKey( typeof(T).Name);
            }
            AnalysisObject2Registry(typeof(T), Registry.CurrentUser.OpenSubKey("SOFTWARE", true).OpenSubKey( typeof(T).Name, true), "", this);
        }

        private static void AnalysisObject2Registry(Type t, RegistryKey registryKey, string key, object o)
        {
            if (t.FullName == typeof(string).FullName ||
                t.FullName == typeof(char).FullName ||
                t.FullName == typeof(byte).FullName ||
                t.FullName == typeof(short).FullName ||
                t.FullName == typeof(ushort).FullName ||
                t.FullName == typeof(int).FullName ||
                t.FullName == typeof(uint).FullName ||
                t.FullName == typeof(long).FullName ||
                t.FullName == typeof(ulong).FullName ||
                t.FullName == typeof(float).FullName ||
                t.FullName == typeof(decimal).FullName ||
                t.FullName == typeof(double).FullName ||
                t.FullName == typeof(bool).FullName ||
                t.FullName == typeof(DateTime).FullName ||
                t.FullName == typeof(Rectangle).FullName ||
                t.FullName == typeof(RectangleF).FullName ||
                t.FullName == typeof(Point).FullName ||
                t.FullName == typeof(PointF).FullName ||
                t.FullName == typeof(Size).FullName ||
                t.FullName == typeof(SizeF).FullName)
            {
                if (o != null)
                {
                    registryKey.SetValue(key, o.ToString());
                }
            }
            else if (t.FullName == typeof(char?).FullName ||
               t.FullName == typeof(byte?).FullName ||
               t.FullName == typeof(short?).FullName ||
               t.FullName == typeof(ushort?).FullName ||
               t.FullName == typeof(int?).FullName ||
               t.FullName == typeof(uint?).FullName ||
               t.FullName == typeof(long?).FullName ||
               t.FullName == typeof(ulong?).FullName ||
               t.FullName == typeof(float?).FullName ||
               t.FullName == typeof(decimal?).FullName ||
               t.FullName == typeof(double?).FullName ||
               t.FullName == typeof(bool?).FullName ||
               t.FullName == typeof(DateTime?).FullName ||
               t.FullName == typeof(Rectangle?).FullName ||
               t.FullName == typeof(RectangleF?).FullName ||
               t.FullName == typeof(Point?).FullName ||
               t.FullName == typeof(PointF?).FullName ||
               t.FullName == typeof(Size?).FullName ||
               t.FullName == typeof(SizeF?).FullName)
            {
                if (o != null)
                {
                    registryKey.SetValue(key, o.ToString());
                }
            }
            else if (t.FullName == typeof(byte[]).FullName)
            {
                if (o != null)
                {
                    registryKey.SetValue(key, Convert.ToBase64String((byte[])o));
                }
            }
            else if (t.FullName == typeof(Color).FullName)
            {
                if (o != null)
                {
                    registryKey.SetValue(key, ColorTranslator.ToHtml((Color)o));
                }
            }
            else if (t.FullName == typeof(Color?).FullName)
            {
                if (o != null)
                {
                    registryKey.SetValue(key, ColorTranslator.ToHtml((Color)o));
                }
            }
            else if (t.Name == typeof(List<>).Name)
            {
                IList list = (IList)o;
                string value = "";
                for (int i = 1; i <= list.Count; i++)
                {
                    value += Path.GetFileName(registryKey.Name) + "*" + key + i + "|";
                }
                if (value != "")
                {
                    value = value.Remove(value.Length - 1);
                }
                registryKey.SetValue(key, value);
                for (int i = 1; i <= list.Count; i++)
                {
                    if (registryKey.GetSubKeyNames().Count(s => s == Path.GetFileName(registryKey.Name) + "*" + key + i) == 0)
                    {
                        registryKey.CreateSubKey(Path.GetFileName(registryKey.Name) + "*" + key + i);
                    }
                    AnalysisObject2Registry(((object)list[i - 1]).GetType(), registryKey.OpenSubKey(Path.GetFileName(registryKey.Name) + "*" + key + i, true), ((object)list[i - 1]).GetType().Name, ((object)list[i - 1]));
                }
            }
            else
            {
                PropertyInfo[] ps = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo p in ps)
                {
                    AnalysisObject2Registry(p.PropertyType, registryKey, p.Name, p.GetValue(o, null));
                }
            }
        }

        /// <summary> 清除改对象占用的注册表信息
        /// </summary>
        public static void ClearRegistry()
        {
            if (Registry.CurrentUser.OpenSubKey("SOFTWARE", true).GetSubKeyNames().Count(s => s ==  typeof(T).Name) != 0)
            {
                Registry.CurrentUser.OpenSubKey("SOFTWARE", true).DeleteSubKeyTree( typeof(T).Name);
            }
        }
        #endregion

        #region Control
        /// <summary> 将界面上的控件的值加载到对象中，（注：对象的字段必须打上Control2PropertyAttibutes标签）
        /// </summary>
        /// <param name="parentControl"></param>
        /// <returns></returns>
        public static T LoadFromControl(Control parentControl)
        {
            try
            {
                T t = new T();
                List<Control> controlList = ObjectPersistenceTools.GetControlList(parentControl);
                t.GetType().GetProperties().Cast<PropertyInfo>().ToList().ForEach(p =>
                {
                    Control2PropertyAttibutes cpa = p.GetCustomAttributes(typeof(Control2PropertyAttibutes), true).Cast<Control2PropertyAttibutes>().FirstOrDefault();
                    if (cpa != null)
                    {
                        Control control = controlList.Where(c => c.GetType().FullName == cpa.ControlType.FullName && c.Name == cpa.ControlName).FirstOrDefault();
                        if (control != null)
                        {
                            PropertyInfo pi = control.GetType().GetProperties().Where(c => c.Name == cpa.PropertyName).FirstOrDefault();
                            if (p.CanWrite && pi != null)
                            {
                                object value = pi.GetValue(control, null);
                                try
                                {
                                    p.SetValue(t, value, null);
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                });
                return t;
            }
            catch
            {
                return new T();
            }
        }
        /// <summary> 
        /// 将界面上的控件的值加载到对象中，（注：对象的字段必须打上Control2PropertyAttibutes标签）
        /// </summary>
        /// <param name="parentControl"></param>
        /// <returns></returns>
        public static T LoadFromControlWithoutCatch(Control parentControl)
        {
            T t = new T();
            List<Control> controlList = ObjectPersistenceTools.GetControlList(parentControl);
            t.GetType().GetProperties().Cast<PropertyInfo>().ToList().ForEach(p =>
            {
                Control2PropertyAttibutes cpa = p.GetCustomAttributes(typeof(Control2PropertyAttibutes), true).Cast<Control2PropertyAttibutes>().FirstOrDefault();
                if (cpa != null)
                {
                    Control control = controlList.Where(c => c.GetType().FullName == cpa.ControlType.FullName && c.Name == cpa.ControlName).FirstOrDefault();
                    if (control != null)
                    {
                        PropertyInfo pi = control.GetType().GetProperties().Where(c => c.Name == cpa.PropertyName).FirstOrDefault();
                        if (p.CanWrite && pi != null)
                        {
                            object value = pi.GetValue(control, null);
                            try
                            {
                                p.SetValue(t, value, null);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            });
            return t;
        }

        /// <summary> 
        /// 将界面上的控件的值加载到对象中，（注：对象的字段必须打上Control2PropertyAttibutes标签）
        /// </summary>
        /// <param name="parentControl"></param>
        /// <returns></returns>
        public void SetValueByControl(Control parentControl)
        {
            List<Control> controlList = ObjectPersistenceTools.GetControlList(parentControl);
            GetType().GetProperties().Cast<PropertyInfo>().ToList().ForEach(p =>
            {
                Control2PropertyAttibutes cpa = p.GetCustomAttributes(typeof(Control2PropertyAttibutes), true).Cast<Control2PropertyAttibutes>().FirstOrDefault();
                if (cpa != null)
                {
                    Control control = controlList.Where(c => c.GetType().FullName == cpa.ControlType.FullName && c.Name == cpa.ControlName).FirstOrDefault();
                    if (control != null)
                    {
                        PropertyInfo pi = control.GetType().GetProperties().Where(c => c.Name == cpa.PropertyName).FirstOrDefault();
                        if (p.CanWrite && pi != null)
                        {
                            object value = pi.GetValue(control, null);
                            try
                            {
                                p.SetValue(this, value, null);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            });
        }

        /// <summary> 
        /// 将对象的值加载到界面上的控件中，（注：对象的字段必须打上Control2PropertyAttibutes标签）
        /// </summary>
        /// <param name="parentControl"></param>
        /// <returns></returns>
        public void SaveAsControl(Control parentControl)
        {
            List<Control> controlList = ObjectPersistenceTools.GetControlList(parentControl);
            GetType().GetProperties().Cast<PropertyInfo>().ToList().ForEach(p =>
            {
                Control2PropertyAttibutes cpa = p.GetCustomAttributes(typeof(Control2PropertyAttibutes), true).Cast<Control2PropertyAttibutes>().FirstOrDefault();
                if (cpa != null)
                {
                    Control control = controlList.Where(c => c.GetType().FullName == cpa.ControlType.FullName && c.Name == cpa.ControlName).FirstOrDefault();
                    if (control != null)
                    {

                        PropertyInfo pi = control.GetType().GetProperties().Where(c => c.Name == cpa.PropertyName).FirstOrDefault();
                        if (pi.CanWrite && pi != null)
                        {
                            try
                            {
                                pi.SetValue(control, p.GetValue(this, null), null);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            });
        }

        /// <summary> 
        /// 将对象的值绑定到界面上的控件中，使两边的值保持一致，（注：对象的字段必须打上Control2PropertyAttibutes标签）
        /// </summary>
        /// <param name="parentControl"></param>
        /// <returns></returns>
        public void BindingControl(Control parentControl)
        {
            UnBindingControl(parentControl);
            SaveAsControl(parentControl);
            eventHandler = new EventHandler((sender, e) =>
            {
                GetType().GetProperties().Cast<PropertyInfo>().ToList().ForEach(p =>
                {
                    Control2PropertyAttibutes cpa = p.GetCustomAttributes(typeof(Control2PropertyAttibutes), true).Cast<Control2PropertyAttibutes>().FirstOrDefault();
                    if (cpa != null)
                    {
                        if (((Control)sender).GetType().FullName == cpa.ControlType.FullName && ((Control)sender).Name == cpa.ControlName)
                        {
                            PropertyInfo pi = sender.GetType().GetProperties().Where(c => c.Name == cpa.PropertyName).FirstOrDefault();
                            if (pi.CanWrite && pi != null)
                            {
                                try
                                {
                                    p.SetValue(this, pi.GetValue(sender, null), null);
                                    return;
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                });
            });
            ObjectPersistenceTools.AddControlEvent(parentControl, "ValueChanged", eventHandler);
            ObjectPersistenceTools.AddControlEvent(parentControl, "CheckedChanged", eventHandler);
            ObjectPersistenceTools.AddControlEvent(parentControl, "TextChanged", eventHandler);
        }

        /// <summary> 
        /// 将对象的值绑定到界面上的控件中，使两边的值保持一致，（注：对象的字段必须打上Control2PropertyAttibutes标签）
        /// </summary>
        /// <param name="parentControl"></param>
        /// <returns></returns>
        public void BindingControl(Control parentControl, EventHandler eh)
        {
            UnBindingControl(parentControl);
            SaveAsControl(parentControl);
            eventHandler = new EventHandler((sender, e) =>
            {
                GetType().GetProperties().Cast<PropertyInfo>().ToList().ForEach(p =>
                {
                    Control2PropertyAttibutes cpa = p.GetCustomAttributes(typeof(Control2PropertyAttibutes), true).Cast<Control2PropertyAttibutes>().FirstOrDefault();
                    if (cpa != null)
                    {
                        if (((Control)sender).GetType().FullName == cpa.ControlType.FullName && ((Control)sender).Name == cpa.ControlName)
                        {
                            PropertyInfo pi = sender.GetType().GetProperties().Where(c => c.Name == cpa.PropertyName).FirstOrDefault();
                            if (pi.CanWrite && pi != null)
                            {
                                try
                                {
                                    p.SetValue(this, pi.GetValue(sender, null), null);
                                    return;
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                });
                if (eh != null)
                {
                    eh(sender, e);
                }
            });
            ObjectPersistenceTools.AddControlEvent(parentControl, "ValueChanged", eventHandler);
            ObjectPersistenceTools.AddControlEvent(parentControl, "CheckedChanged", eventHandler);
            ObjectPersistenceTools.AddControlEvent(parentControl, "TextChanged", eventHandler);
        }

        /// <summary> 
        /// 将对象的值取消绑定到界面上的控件中
        /// </summary>
        /// <param name="parentControl"></param>
        /// <returns></returns>
        public void UnBindingControl(Control parentControl)
        {
            if (eventHandler != null)
            {
                ObjectPersistenceTools.RemoveControlEvent(parentControl, "ValueChanged", eventHandler);
                ObjectPersistenceTools.RemoveControlEvent(parentControl, "CheckedChanged", eventHandler);
                ObjectPersistenceTools.RemoveControlEvent(parentControl, "TextChanged", eventHandler);
                eventHandler = null;
            }
        }

        #endregion

        #region Object

        /// <summary> 
        /// 将其他对象的值加载到当前对象，对象字段必须一模一样，找不到的字段会自动忽略
        /// </summary>
        /// <return>返回合并后对象</return>
        public static T LoadFromObject(object obj)
        {
            if (obj == null)
            {
                return new T();
            }
            try
            {
                T t = new T();
                t.GetType().GetProperties().Cast<PropertyInfo>().ToList().ForEach(p =>
                {
                    PropertyInfo pi = obj.GetType().GetProperties().Where(o => o.Name == p.Name && o.PropertyType == p.PropertyType).FirstOrDefault();
                    if (p.CanWrite && pi != null)
                    {
                        try
                        {
                            p.SetValue(t, pi.GetValue(obj, null), null);
                        }
                        catch(Exception) { }
                    }
                });
                return t;
            }
            catch
            {
                return new T();
            }
        }

        /// <summary> 
        /// 将其他对象的值加载到当前对象，对象字段必须一模一样，找不到的字段会自动忽略
        /// </summary>
        /// <return>返回合并后对象</return>
        public static T LoadFromObjectWithoutCatch(object obj)
        {
            if (obj == null)
            {
                return new T();
            }
            T t = new T();
            t.GetType().GetProperties().Cast<PropertyInfo>().ToList().ForEach(p =>
            {
                PropertyInfo pi = obj.GetType().GetProperties().Where(o => o.Name == p.Name && o.PropertyType == p.PropertyType).FirstOrDefault();
                if (p.CanWrite && pi != null)
                {
                    try
                    {
                        p.SetValue(t, pi.GetValue(obj, null), null);
                    }
                    catch(Exception) { }
                }
            });
            return t;
        }

        /// <summary> 
        /// 将其他对象的值加载到当前对象，对象字段必须一模一样，找不到的字段会自动忽略
        /// </summary>
        public void SetValueByObject(object obj)
        {
            GetType().GetProperties().Cast<PropertyInfo>().ToList().ForEach(p =>
            {
                PropertyInfo pi = obj.GetType().GetProperties().Where(o => o.Name == p.Name && o.PropertyType == p.PropertyType).FirstOrDefault();
                if (p.CanWrite && pi != null)
                {
                    try
                    {
                        p.SetValue(this, pi.GetValue(obj, null), null);
                    }
                    catch(Exception) { }
                }
            });
        }

        /// <summary> 
        /// 将当前对象的值加载到其他对象，对象字段必须一模一样，找不到的字段会自动忽略
        /// </summary>
        public void SaveAsObject(object obj)
        {
            if (obj == null)
            {
                return;
            }
            obj.GetType().GetProperties().Cast<PropertyInfo>().ToList().ForEach(p =>
            {
                PropertyInfo pi = GetType().GetProperties().Where(t => t.Name == p.Name && t.PropertyType == p.PropertyType).FirstOrDefault();
                if (p.CanWrite && pi != null)
                {
                    try
                    {
                        p.SetValue(obj, pi.GetValue(this, null), null);
                    }
                    catch(Exception) { }
                }
            });
        }

        #endregion


        public override bool Equals(object obj)
        {
            if (!(obj is ObjectPersistence<T>))
            {
                return false;
            }
            byte[] byteData1 = ((ObjectPersistence<T>)obj).GetBinaryBytes();
            if (byteData1 == null)
            {
                return false;
            }
            byte[] byteData2 = GetBinaryBytes();
            if (byteData2 == null)
            {
                return false;
            }
            return byteData1.SequenceEqual(byteData2);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
