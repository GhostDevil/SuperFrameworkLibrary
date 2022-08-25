using System.Text;

namespace System.Collections.Generic
{
    public static class ListEx
    {
        /// <summary>
        /// 把string型的List按照分隔符组装成string字符串
        /// </summary>
        /// <param name="list">string型的Lsit</param>
        /// <param name="speater">分隔符</param>
        /// <returns>返回字符串</returns>
        public static string GetArrayStr(this List<string> list, string speater)
        {
            StringBuilder sb = new StringBuilder();
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
        /// 得到数组列表以逗号分隔的字符串
        /// </summary>
        /// <param name="list">int型List</param>
        /// <returns>返回字符串</returns>
        public static string GetArrayStr(this List<int> list)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                if (i == list.Count - 1)
                {
                    sb.Append(list[i]);
                }
                else
                {
                    sb.Append(list[i]);
                    sb.Append(',');
                }
            }
            return sb.ToString();
        }
    }
}
