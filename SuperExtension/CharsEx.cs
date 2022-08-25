namespace System.Collections.Generic
{
    public static class CharsEx
    {
        /// <summary>
        /// char数组转换字符串
        /// </summary>
        /// <param name="chr">char数组</param>
        /// <returns>返回字符串</returns>        
        public static string CharArrayToString(this char[] chr)
        {
            string out_str;
            out_str = new string(chr);
            int i = out_str.IndexOf('\0', 0);
            if (i == -1)
                i = 16;
            return out_str.Substring(0, i);
        }
    }
}
