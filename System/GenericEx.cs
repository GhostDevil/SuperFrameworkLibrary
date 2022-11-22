using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace System.Collections.Generic
{
    public static class Generic
    {
        /// <summary>
        /// 转换字段为字典
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="isNotNull">是否不为null值</param>
        /// <returns></returns>
        public static Dictionary<string, object> ToDictionaryFields<T>(this object obj, bool isNotNull = true) where T : class
        {
            var d = obj.GetType().GetFields()
               .ToDictionary(q => q.Name, q => q.GetValue(obj));
            if (isNotNull) return d.Where(o => o.Value != null).ToDictionary(x => x.Key, x => x.Value);
            else return d;
        }
        /// <summary>
        /// 转换属性为字典
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="isNotNull">是否不为null值</param>
        /// <returns></returns>
        public static Dictionary<string, object> ToDictionaryProperties<T>(this object obj, bool isNotNull = true) where T : class
        {
            var d = obj.GetType().GetProperties()
               .ToDictionary(q => q.Name, q => q.GetValue(obj));
            if (isNotNull) return d.Where(o => o.Value != null).ToDictionary(x => x.Key, x => x.Value);
            else return d;
        }

        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> values)
        {
            if (values != null)
            {
                foreach (var i in values)
                {
                    collection.Add(i);
                }
            }
            //OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, collection.ToList()));
        }
        public static void RemoveRange<T>(this ObservableCollection<T> collection, IEnumerable<T> values)
        {
            if (values != null)
            {
                foreach (var i in values)
                {
                    collection.Remove(i);
                }
            }

            //OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, collection.ToList()));
        }
        /// <summary>
        /// 获取指定元素存在的开始位置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="arrayFind">查找块</param>
        /// <param name="startIndex">排除索引</param>
        /// <returns>所在索引</returns>
        public static int IndexOfBlock<T>(this T array, T arrayFind, int startIndex = -1) where T : IEnumerable
        {

            var arrayLen = (array as Array)?.Length ?? 0;
            var array1Len = (arrayFind as Array)?.Length ?? 0;
            if (arrayLen < array1Len)
                return -1;
            FIND_FRAME:
            var index = Array.IndexOf(array as Array, (arrayFind as Array).GetValue(0), startIndex + 1);
            if (index > -1 && index + array1Len <= arrayLen)
            {
                startIndex = index;
                for (int i = 0; i < array1Len; i++)
                {
                    if (!(arrayFind as Array).GetValue(i).Equals((array as Array).GetValue(startIndex + i)))
                        goto FIND_FRAME;
                }
            }
            return startIndex;
        }
        /// <summary>
        /// 得到数组列表以分隔符分隔的字符串
        /// </summary>
        /// <param name="speater">分隔符</param>
        /// <returns>返回字符串</returns>
        //public static string ToString<T>(this IEnumerable<T> list, string speater) where T : IComparable => GetArrayStr(list.ToList().ConvertAll(s => (object)s), speater);

        ///// <summary>
        ///// 得到数组列表以分隔符分隔的字符串
        ///// </summary>
        ///// <param name="speater">分隔符</param>
        ///// <returns>返回字符串</returns>
        public static string ToString<T>(this T list, string speater) where T : IEnumerable => list.ToListDynamic().GetArrayStr(speater);
        private static string GetArrayStr(this List<object> list, string speater)
        {
            StringBuilder sb = new();
            for (int i = 0; i < list.Count; i++)
            {
                if (i == list.Count - 1)
                {
                    sb.Append(list[i]);
                }
                else
                {
                    sb.Append(list[i]);
                    sb.Append(speater);
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// 将集合指定的键值对数据转 匿名对象 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="keyName"></param>
        /// <param name="valName"></param>
        /// <returns></returns>
        public static dynamic GetSingleObject<T>(this List<T> list, string keyName = "name", string valName = "value") where T : class
        {
            if (list == null) return new object();
            Collections.Generic.IDictionary<string, object> para = new System.Dynamic.ExpandoObject();
            foreach (var item in list)
            {
                var name = item.GetValueEx(keyName);
                var val = item.GetValueEx(valName);
                if (name == null) continue;

                para[name] = val;
            }
            return para;
        }
        /// <summary>
        /// json文件写入或修改
        /// </summary>
        /// <param name="sectionInfo">Json对象</param>
        /// <param name="configFilePath">文件路径</param>
        /// <param name="configFileName"></param>
        /// <returns></returns>
        public static bool SaveJson(this Dictionary<string, object> sectionInfo, string configFilePath, string configFileName = "appsettings.json")
        {
            if (sectionInfo.Count == 0)
                return false;

            try
            {
                var filePath = Path.Combine(configFilePath, configFileName);
                JObject jsonObject;

                if (File.Exists(filePath))
                {
                    using (StreamReader file = new StreamReader(filePath))
                    {
                        using (JsonTextReader reader = new JsonTextReader(file))
                        {
                            jsonObject = (JObject)JToken.ReadFrom(reader);
                        }
                    }
                }
                else
                {
                    jsonObject = new JObject();
                }

                foreach (var key in sectionInfo.Keys)
                {
                    jsonObject[key] = JObject.FromObject(sectionInfo[key]);
                }

                using (var writer = new StreamWriter(filePath))
                using (JsonTextWriter jsonwriter = new JsonTextWriter(writer)
                {
                    Formatting = Formatting.Indented,//格式化缩进
                    Indentation = 4,  //缩进四个字符
                    IndentChar = ' '  //缩进的字符是空格
                })
                {
                    jsonObject.WriteTo(jsonwriter);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
