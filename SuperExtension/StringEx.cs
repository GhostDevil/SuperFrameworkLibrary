using Microsoft.VisualBasic;
using SuperFramework.SuperConvert;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace System
{
    public static class StringEx
    {
        /// <summary>
        /// 添加字符串内容
        /// </summary>
        /// <param name="strMsg">源字符串，分割后并输出</param>
        /// <param name="splitNum">分割长度</param>
        /// <param name="insertStr">添加字符</param>
        /// <param name="b">是否成功</param>
        /// <returns>新字符串</returns>
        public static string InsertSplit(this string strMsg, int splitNum, out bool b, string insertStr = " ")
        {
            string w = "";
            try
            {
                int x = 0;

                foreach (char item in strMsg)
                {
                    if (x == splitNum - 1)
                    {
                        w += item + insertStr;
                        x = 0;
                    }
                    else
                    {
                        w += item;
                        x++;
                    }
                }
                w = w.Substring(0, w.Length - insertStr.Length);
                b = true;
            }
            catch
            {
                b = false;
            }
            return w;
        }
        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <param name="strMsg">源字符串，分割后并输出</param>
        /// <param name="splitNum">分割长度</param>
        /// <param name="splitStr">分割符号</param>
        /// <param name="b">是否成功</param>
        /// <returns>新字符串</returns>
        public static string SplitString(this string strMsg, int splitNum, out bool b, string splitStr = " ")
        {
            string str = "";
            try
            {
                int count = strMsg.Length / splitNum;
                for (int i = 1; i < count; i++)
                {
                    str = strMsg.Insert(splitNum * i + splitStr.Length * (i - 1), splitStr);
                }
                b = true;
            }
            catch (Exception) { b = false; }

            return str;
        }

        #region 检查一个字符串是否是纯数字构成的，一般用于查询字符串参数的有效性验证。
        /// <summary>
        /// 检查一个字符串是否是纯数字构成的，一般用于查询字符串参数的有效性验证。(0除外)
        /// </summary>
        /// <param name="_value">需验证的字符串。。</param>
        /// <returns>是否合法的bool值。</returns>
        public static bool IsNumberId(this string _value)
        {
            return QuickValidate("^[1-9]*[0-9]*$", _value);
        }

        /// <summary>
        /// 快速验证一个字符串是否符合指定的正则表达式。
        /// </summary>
        /// <param name="_express">正则表达式的内容。</param>
        /// <param name="_value">需验证的字符串。</param>
        /// <returns>是否合法的bool值。</returns>
        public static bool QuickValidate(this string _express, string _value)
        {
            if (_value == null) return false;
            Regex myRegex = new Regex(_express);
            if (_value.Length == 0)
            {
                return false;
            }
            return myRegex.IsMatch(_value);
        }
        #endregion


        #region GB2312 and UTF8
        /// <summary>
        /// GB2312转换成UTF8
        /// </summary>
        /// <param name="text">GB2312字符串</param>
        /// <returns>返回UTF8字符串</returns>
        public static string GB2312ToUTF8(this string text)
        {
            //声明字符集   
            Encoding utf8, gb2312;
            //gb2312   
            gb2312 = System.Text.Encoding.GetEncoding("gb2312");
            //utf8   
            utf8 = System.Text.Encoding.GetEncoding("utf-8");
            byte[] gb;
            gb = gb2312.GetBytes(text);
            gb = System.Text.Encoding.Convert(gb2312, utf8, gb);
            //返回转换后的字符   
            return utf8.GetString(gb);
        }

        /// <summary>
        /// UTF8转换成GB2312
        /// </summary>
        /// <param name="text">UTF8字符串</param>
        /// <returns>返回GB2312字符串</returns>
        public static string UTF8ToGB2312(this string text)
        {
            //声明字符集   
            Encoding utf8, gb2312;
            //utf8   
            utf8 = System.Text.Encoding.GetEncoding("utf-8");
            //gb2312   
            gb2312 = System.Text.Encoding.GetEncoding("gb2312");
            byte[] utf;
            utf = utf8.GetBytes(text);
            utf = System.Text.Encoding.Convert(utf8, gb2312, utf);
            //返回转换后的字符   
            return gb2312.GetString(utf);
        }
        #endregion

        /// <summary>  
        /// 取字符串中出现最多次数的字符  
        /// </summary>  
        /// <param name="inputString">带检查字符串</param>  
        /// <param name="number">出现次数</param>  
        /// <returns>出现最多的字符</returns>  
        public static string GetChar(this string inputString, out int number)
        {
            char[] chars = inputString.ToCharArray();
            number = int.MinValue;
            int originalLength = inputString.Length;
            string cstr = "";
            foreach (var c in chars)
            {
                int len = inputString.Replace(c.ToString(CultureInfo.InvariantCulture), string.Format("{0}1", c)).Length - originalLength;
                if (len > number)
                {
                    number = len;
                    cstr = c.ToString(CultureInfo.InvariantCulture);
                }
            }
            return cstr;
        }
        #region 查找某一字符串在目标字符串里出现的次数
        /// <summary>
        /// 查找某一字符串在目标字符串里出现的次数
        /// </summary>
        /// <param name="Enstr">源字符串</param>
        /// <param name="Destr">目标字符串</param>
        /// <returns>0 不存在 -1 查找失败 >0出现次数</returns>
        public static int FindStringCount(this string _, string Enstr, string Destr)
        {
            int count = 0;
            try
            {
                string a = Enstr;
                string b = Destr;
                string tempstr = a;
                tempstr = tempstr.Replace(b, "");
                int charcount = (a.Length - tempstr.Length);

                if (charcount > 0)
                    count = charcount / b.Length;
            }
            catch (Exception) { return -1; }
            return count;
        }

        #endregion

        #region  简繁转换 
        /// <summary>
        /// 转换为简体中文
        /// </summary>
        /// <param name="str">繁体中文</param>
        /// <returns>返回简体中文</returns>
        public static string ToSChinese(this string str)
        {
            return Strings.StrConv(str, VbStrConv.SimplifiedChinese, 0);

        }

        /// <summary>
        /// 转换为繁体中文
        /// </summary>
        /// <param name="str">简体中文</param>
        /// <returns>返回繁体中文</returns>
        public static string ToTChinese(this string str)
        {
            return Strings.StrConv(str, VbStrConv.TraditionalChinese, 0);

        }
        #endregion

        /// <summary>
        /// 将字符串转换为新样式
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <param name="newStyle">新样式</param>
        /// <param name="splitString">分隔符</param>
        /// <param name="error">错误消息</param>
        /// <returns>返回新样式字符串</returns>
        public static string GetNewStyle(this string str, string newStyle, string splitString, out string error)
        {
            string ReturnValue;
            //如果输入空值，返回空，并给出错误提示
            if (str == null)
            {
                ReturnValue = "";
                error = "请输入需要划分格式的字符串";
            }
            else
            {
                //检查传入的字符串长度和样式是否匹配,如果不匹配，则说明使用错误。给出错误信息并返回空值
                int strListLength = str.Length;
                int NewStyleLength = newStyle.Replace(splitString, "").Length;
                if (strListLength != NewStyleLength)
                {
                    ReturnValue = "";
                    error = "样式格式的长度与输入的字符长度不符，请重新输入";
                }
                else
                {
                    //检查新样式中分隔符的位置
                    string Lengstr = "";
                    for (int i = 0; i < newStyle.Length; i++)
                    {
                        if (newStyle.Substring(i, 1) == splitString)
                        {
                            Lengstr = Lengstr + "," + i;
                        }
                    }
                    if (Lengstr != "")
                    {
                        Lengstr = Lengstr.Substring(1);
                    }
                    //将分隔符放在新样式中的位置
                    string[] st = Lengstr.Split(',');
                    foreach (string bb in st)
                    {
                        str = str.Insert(int.Parse(bb), splitString);
                    }
                    //给出最后的结果
                    ReturnValue = str;
                    //因为是正常的输出，没有错误
                    error = "";
                }
            }
            return ReturnValue;
        }
        /// <summary>
        /// 把字符串按照指定分隔符装成 List 去除重复
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="sepeater">分隔符</param>
        /// <returns>返回stringList</returns>
        public static List<string> GetSubStringList(this string str, char sepeater)
        {
            List<string> list = new List<string>();
            string[] ss = str.Split(sepeater);
            foreach (string s in ss)
            {
                if (!string.IsNullOrEmpty(s) && s != sepeater.ToString())
                {
                    list.Add(s);
                }
            }
            return list;
        }

        #region  转全角(SBC case) 
        /// <summary>
        /// 转全角(SBC case)
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <returns>返回全角字符串</returns>
        public static string ToSBC(this string input)
        {
            //半角转全角：
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 32)
                {
                    c[i] = (char)12288;
                    continue;
                }
                if (c[i] < 127)
                    c[i] = (char)(c[i] + 65248);
            }
            return new string(c);
        }
        #endregion

        #region  转半角(SBC case) 
        /// <summary>
        ///  转半角(SBC case)
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <returns>返回半角字符串</returns>
        public static string ToDBC(this string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new string(c);
        }
        #endregion

        /// <summary>
        /// 删除最后结尾的指定字符后的字符
        /// </summary>
        public static string TrimEnd(this string str, string strchar)
        {
            return str.Substring(0, str.LastIndexOf(strchar));
        }
        #region 字符串转集合
        /// <summary>
        /// 把字符串按照分隔符转换成List
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <param name="speater">分隔符</param>
        /// <param name="toLower">是否转换为小写</param>
        /// <returns>返回字符串List</returns>
        public static List<string> SplitString(this string str, char speater, bool toLower)
        {
            List<string> list = new List<string>();
            string[] ss = str.Split(speater);
            foreach (string s in ss)
            {
                if (!string.IsNullOrEmpty(s) && s != speater.ToString())
                {
                    string strVal = s;
                    if (toLower)
                    {
                        strVal = s.ToLower();
                    }
                    list.Add(strVal);
                }
            }
            return list;
        }
        /// <summary>
        /// 把字符串按照分隔符转换成List
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="splitstr">分隔符</param>
        /// <returns>返回字符串数组</returns>
        public static string[] SplitString(this string str, string splitstr)
        {
            string[] strArray = null;
            if ((str != null) && (str != ""))
            {
                strArray = new Regex(splitstr).Split(str);
            }
            return strArray;
        }
        /// <summary>
        /// 把字符串按照,分割换为数组
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>返回分割后的数组</returns>
        public static string[] SplitString(this string str)
        {
            return str.Split(new char[] { ',' });
        }
        #endregion

        /// <summary>
        /// 指定字符串的固定长度，如果字符串小于固定长度，
        /// 则在字符串的前后补足指定内容
        /// </summary>
        /// <param name="text">原始字符串</param>
        /// <param name="limitedLength">字符串的固定长度</param>
        /// <param name="isLeft">是否填补在前边</param>
        /// <param name="inString">填充的内容</param>
        /// <returns>返回新字符串</returns>>
        public static string RepairZero(this string text, int limitedLength, bool isLeft = true, string inString = "0")
        {
            //补足0的字符串
            string temp = "";
            //补足0
            for (int i = 0; i < limitedLength - text.Length; i++)
                temp += inString;
            //连接text
            if (isLeft)
                temp += text;
            else
                text += temp;
            //返回补足的字符串
            return temp;
        }

        /// <summary>
        /// 二进制字符串转byte 
        /// </summary>
        /// <param name="byteStr">二进制字符串</param>
        /// <returns>返回相应的byte值</returns>
        public static byte DecodeBinaryString(this string byteStr)
        {
            int re, len;
            if (null == byteStr)
            {
                return 0;
            }
            len = byteStr.Length;
            if (len != 4 && len != 8)
            {
                return 0;
            }
            if (len == 8)
            {// 8 bit处理  
                if (byteStr.ToCharArray()[0] == '0')
                {// 正数  
                    re = int.Parse(Hexadecimal.ConvertBase(byteStr, 2, 10));
                }
                else
                {// 负数  
                    re = int.Parse(Hexadecimal.ConvertBase(byteStr, 2, 10)) - 256;
                }
            }
            else
            {// 4 bit处理  
                re = int.Parse(Hexadecimal.ConvertBase(byteStr, 2, 10));
            }
            return (byte)re;
        }

        /// <summary>
        /// 将字符串转换为BCD字节数组
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static byte[] StringToBCDByteArray(this string str)
        {
            byte[] strByte = Encoding.Default.GetBytes(str);
            byte[] bcdByte = new byte[strByte.Length * 2];
            for (int i = 0; i < strByte.Length; i++)
            {
                bcdByte[2 * i] = strByte[i].ByteToBCD(true);
                bcdByte[2 * i + 1] = strByte[i].ByteToBCD();
            }
            return bcdByte;
        }


        /// <summary>
        /// 从字符串获得指定长度的byte数组
        /// </summary>
        /// <param name="s">字符串</param>
        /// <param name="Length">返回长度</param>
        /// <param name="encoding">编码格式</param>
        /// <returns></returns>
        public static byte[] GetByteByString(this string s, int Length, Encoding encoding)
        {
            byte[] temp = encoding.GetBytes(s);
            byte[] ret = new byte[Length];
            if (temp.Length > Length)
                Array.Copy(temp, ret, Length);
            else
                Array.Copy(temp, ret, temp.Length);
            ret[Length - 1] = 0;
            return ret;
        }

        /// <summary>
        /// String 转换为Hex码（即16进制数组）
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] TOHex(this string data)  //  
        {
            int l = data.Length;
            if (data.Length % 2 != 0)
            {
                data += " ";//空格 
                //throw new ArgumentException("hex is not a valid number!", "hex");       
            }
            byte[] bytes = new byte[data.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                try
                {
                    // 每两个字符是一个 byte。  
                    bytes[i] = byte.Parse(data.Substring(i * 2, 2),
                    System.Globalization.NumberStyles.HexNumber);
                }
                catch
                {
                    // Rethrow an exception with custom message.  
                    throw new ArgumentException("hex is not a valid hex number!", "hex");
                }
            }
            return bytes;
        }
        /// <summary> 
        /// 16进制字符串转字节数组 
        /// </summary> 
        /// <param name="hexString"></param> 
        /// <returns></returns> 
        public static byte[] StrToToHexByte(this string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2).Trim(), 16); return returnBytes;

        }
        /// <summary> 
        /// 从汉字转换到16进制   
        // </summary> 
        /// <param name="s"></param> 
        /// <param name="charset">编码,如"utf-8","gb2312"</param> 
        /// <param name="fenge">是否每字符用逗号分隔</param> 
        /// <returns></returns> 
        public static string ToHex(this string s, string charset, bool fenge)
        {
            if ((s.Length % 2) != 0)
            {
                s += " ";//空格         
                //throw new ArgumentException("s is not valid chinese string!");     
            }
            Encoding chs = System.Text.Encoding.GetEncoding(charset);
            byte[] bytes = chs.GetBytes(s);
            string str = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                str += string.Format("{0:X}", bytes[i]);
                if (fenge && (i != bytes.Length - 1))
                {
                    str += string.Format("{0}", ",");
                }
            }
            return str.ToLower();
        }

        /// <summary> 
        /// 从16进制转换成汉字 
        /// </summary> 
        /// <param name="hex"></param> 
        /// <param name="charset">编码,如"utf-8","gb2312"</param> 
        /// <returns></returns>  
        public static string UnHex(this string hex, string charset)
        {
            if (hex == null)
                throw new ArgumentNullException(nameof(hex));
            hex = hex.Replace(",", "");
            hex = hex.Replace("/n", "");
            hex = hex.Replace("//", "");
            hex = hex.Replace(" ", "");
            if (hex.Length % 2 != 0)
            {
                hex += "20";//空格     
            }
            // 需要将 hex 转换成 byte 数组。     
            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                try
                {
                    // 每两个字符是一个 byte。             
                    bytes[i] = byte.Parse(hex.Substring(i * 2, 2),
                    System.Globalization.NumberStyles.HexNumber);
                }
                catch
                {
                    // Rethrow an exception with custom message.             
                    throw new ArgumentException("hex is not a valid hex number!", nameof(hex));
                }
            }
            Encoding chs = System.Text.Encoding.GetEncoding(charset);
            return chs.GetString(bytes);
        }

        public static Dictionary<string, string> ToDictionary(this string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            var kvs = Regex.Split(text, "&");
            if ((kvs?.Count() ?? 0) <= 0) return null;
            return kvs.ToDictionary(item => Regex.Split(item, "=")[0], item => WebUtility.UrlDecode(Regex.Split(item, "=")[1]));
        }

        /// <summary>
        /// Hex转Byte数组（带空格)
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static byte[] HexToBytesSpace(this string text)
        {
            var list = new List<string>();
            if (text.Contains(" "))
            {
                list.AddRange(text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            }
            else if (text.Length / 2 == 0)
            {
                for (int i = 0; i < text.Length / 2; i += 2)
                {
                    list.Add(text.Substring(i, 2));
                }
            }
            return list.Select(item => Convert.ToByte(item, 160)).ToArray();
        }

        /// <summary>
        /// Hex转Byte数组（无空格)
        /// </summary>
        /// <param name="hexstr"></param>
        /// <returns></returns>
        public static byte[] HexToBytes(this string text)
        {
            string[] list = System.Text.RegularExpressions.Regex.Replace(text, @"(\w{2})", "$1-").Trim('-').Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            byte[] dataArray = new byte[list.Length];
            for (int i = 0; i < list.Length; i++)
            {
                dataArray[i] = Convert.ToByte(Convert.ToInt32(list[i], 16));
            }
            return dataArray;
        }

        /// <summary>
        /// 将一条十六进制字符串转换为ASCII
        /// 好用可以转中文
        /// </summary>
        /// <param name="hexstring"></param>
        /// <returns></returns>
        /// <remarks>不带空格</remarks>
        public static string HexToAscII(this string hexstring)
        {
            byte[] buff = new byte[hexstring.Length / 2];
            int index = 0;
            for (int i = 0; i < hexstring.Length; i += 2)
            {
                buff[index] = Convert.ToByte(hexstring.Substring(i, 2), 16);
                ++index;
            }
            string result = Encoding.Default.GetString(buff);
            return result;
        }


        /// <summary>
        /// 字符串校验（累加字符串取与255模的余数）,返回一字节的校验和
        /// </summary>
        /// <param name="str">被校验字符串</param>
        /// <returns></returns>
        public static string CRC8(this string str)
        {
            string[] str_l = str.Split(" ".ToCharArray());
            byte ret = byte.MinValue;
            foreach (string s in str_l)
            {
                ret += Convert.ToByte(s, 16);
            }
            string str_a = ret.ToString("X2");
            return str_a;
        }

        /// <summary>
        /// 字符串转Ascii码数组
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] StringToAscIIByte(this string str)
        {
            char[] charBuf = str.ToArray();    // 将字符串转换为字符数组
            ASCIIEncoding charToASCII = new ASCIIEncoding();
            byte[] TxdBuf = new byte[charBuf.Length];    // 定义发送缓冲区；
            TxdBuf = charToASCII.GetBytes(charBuf);    // 转换为各字符对应的ASCII
            return TxdBuf;
        }

    }
}
