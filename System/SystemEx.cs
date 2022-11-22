using Microsoft.VisualBasic;
using SuperFramework.SuperConvert;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace System
{
    public static class SystemEx
    {
        #region byte
        /// <summary>
        /// 获取字节中指定bit位的值：1 或 0
        /// </summary>
        /// <param name="pStatus">判断的参数</param>
        /// <param name="index">第几位</param>
        /// <returns>返回字节中指定bit位的值：1 或 0</returns>
        public static byte GetBitValue(this byte pStatus, int index)
        {
           
            //转换高低字节
            uint a = (uint)(1 << (index + 1));
            uint b = (uint)(1 << index);
            if (pStatus % a >= b)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// 设置byte某一位的值 ：1 或 0
        /// </summary>
        /// <param name="data">byte对象</param>
        /// <param name="index">要设置的位， 值从低到高为 1-8</param>
        /// <param name="flag">要设置的值 true / false</param>
        /// <returns>返回新的byte</returns>
        public static byte SetBitValue(this byte data, int index, bool flag)
        {
            if (index > 8 || index < 1)
                throw new ArgumentOutOfRangeException();
            int v = index < 2 ? index : (2 << (index - 2));
            return flag ? (byte)(data | v) : (byte)(data & ~v);
        }
        /// <summary>
        /// 将字节数据转换为BCD编码
        /// </summary>
        /// <param name="rawByte">字节数据</param>
        /// <param name="isHight">是否转换为高位字节，默认为false</param>
        /// <returns></returns>
        public static byte ByteToBCD(this byte rawByte, bool isHight = false)
        {
            byte data = isHight ? (byte)(rawByte >> 4) : (byte)(rawByte & 0x0f);
            data = (data >= 0 && data <= 9) ? (byte)(48 + data) : (byte)(55 + data);
            return data;
        }

        /// <summary>
        /// 将byte转换为一个长度为8的byte数组，数组每个值代表bit 
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static byte[] GetBooleanArray(this byte b)
        {
            byte[] array = new byte[8];
            for (int i = 7; i >= 0; i--)
            {
                array[i] = (byte)(b & 1);
                b = (byte)(b >> 1);
            }
            return array;
        }
        /// <summary>
        /// 把byte转为bit字符串
        /// </summary>
        /// <param name="b">byte 值</param>
        /// <returns>返回bit字符串</returns>
        public static string ByteToBit(this byte b)
        {
            return ""
            + (byte)((b >> 7) & 0x1) + (byte)((b >> 6) & 0x1)
            + (byte)((b >> 5) & 0x1) + (byte)((b >> 4) & 0x1)
            + (byte)((b >> 3) & 0x1) + (byte)((b >> 2) & 0x1)
            + (byte)((b >> 1) & 0x1) + (byte)((b >> 0) & 0x1);
        }
        #region 对byte逐位异或进行偶校验并返回校验结果
        /// <summary>
        /// 对byte逐位异或进行偶校验并返回校验结果
        /// </summary>
        /// <param name="AByte">要取bit值的byte，一个byte有8个bit</param>
        /// <returns>如果byte里有偶数个1则返回true，如果有奇数个1则返回false</returns>
        public static bool EvenParityCheck(this byte AByte)
        {
            return !GetBit(AByte, 0) ^ GetBit(AByte, 1) ^ GetBit(AByte, 2) ^ GetBit(AByte, 3)
            ^ GetBit(AByte, 4) ^ GetBit(AByte, 5) ^ GetBit(AByte, 6) ^ GetBit(AByte, 7);
        }
        #endregion

        #region 取一个byte中的第几个bit的值
        /// <summary>
        /// 取一个byte中的第几个bit的值
        /// </summary>
        /// <param name="AByte">要取bit值的byte，一个byte有8个bit</param>
        /// <param name="iIndex">在byte中要取bit的位置，一个byte从左到右的位置分别是0,1,2,3,4,5,6,7</param>
        /// <returns>返回bit的值</returns>
        public static bool GetBit(this byte AByte, int iIndex)
        {
            //将要取的bit右移到第一位，再与1与运算将其它位置0
            return (AByte >> (7 - iIndex) & 1) != 0;
        }
        #endregion


        #endregion

        #region enum
        /// <summary>
        /// 根据描述获取枚举对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="description"></param>
        /// <returns></returns>
        public static T FideByDesc<T>(this string description) where T : Enum
        {
            System.Reflection.FieldInfo[] fields = typeof(T).GetFields();
            foreach (System.Reflection.FieldInfo field in fields)
            {
                object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);    //获取描述属性
                if (objs.Length > 0 && (objs[0] as DescriptionAttribute).Description == description)
                {
                    return (T)field.GetValue(null);
                }
            }
            return default;
        }

        /// <summary>
        /// 获取枚举特性值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static dynamic GetAttribute<T>(this Enum obj) where T : Attribute
        {
            string value = obj.ToString();
            FieldInfo field = obj.GetType().GetField(value);
            object[] objs = field.GetCustomAttributes(typeof(T), false);    //获取描述属性
            if (objs == null || objs.Length == 0)    //当描述属性没有时，直接返回名称
                return value;
            T descriptionAttribute = (T)objs[0];
            string fieldName = (descriptionAttribute.TypeId as dynamic).Name;
            fieldName = fieldName.Replace("Attribute", "");
            return descriptionAttribute.GetValueEx(fieldName);
        }
        /// <summary>
        /// 获取枚举描述值
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum enumValue)
        {
            string value = enumValue.ToString();
            FieldInfo field = enumValue.GetType().GetField(value);
            object[] objs = field?.GetCustomAttributes(typeof(DescriptionAttribute), false);    //获取描述属性
            if (objs == null || objs.Length == 0)    //当描述属性没有时，直接返回名称
                return value;
            DescriptionAttribute descriptionAttribute = (DescriptionAttribute)objs[0];
            return descriptionAttribute.Description;
        }
        #endregion
        #region int
        /// <summary>
        /// 把数字转换成字母
        /// </summary>
        /// <param name="number">要转换成字母的数字（数字范围在闭区间[1,36] [65,90]）</param>
        /// <returns>返回字母</returns>
        public static string ToChar(this int number)
        {
            if ((1 <= number && 36 >= number))
            {
                int num = number + 64;
                ASCIIEncoding asciiEncoding = new();
                byte[] btNumber = new byte[] { (byte)num };
                return asciiEncoding.GetString(btNumber);
            }
            if (65 <= number && 90 >= number)
            {
                ASCIIEncoding asciiEncoding = new();
                byte[] btNumber = new byte[] { (byte)number };
                return asciiEncoding.GetString(btNumber);
            }
            return "数字不在转换范围内";
        }

        /// <summary>
        /// 获取来自int64的byte
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetByteFromInt64(this ulong value)
        {
            byte[] int64Byte = new byte[8];
            for (int i = 0; i < 8; i++)
            {
                int64Byte[i] = (byte)(value % 0x100);
                value /= 0x100;
            }
            return int64Byte;
        }
        /// <summary>
        /// 获取来自int16的byte
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        static public byte[] GetByteFromInt16(this ushort value)
        {
            byte[] int16Byte = new byte[2];
            for (int i = 0; i < 2; i++)
            {
                int16Byte[i] = (byte)(value % 0x100);
                value /= 0x100;
            }
            return int16Byte;
        }
        #endregion

        #region string
        /// <summary>
        /// 添加字符串内容
        /// </summary>
        /// <param name="strMsg">源字符串，分割后并输出</param>
        /// <param name="splitNum">分割长度</param>
        /// <param name="insertStr">分割字符</param>
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
                w = w[..^insertStr.Length];
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

        #region 检查格式
        /// <summary>
        /// 判断是否是十六进制格式字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsHexadecimal(string str)
        {
            const string PATTERN = @"[A-Fa-f0-9]+$";
            return Regex.IsMatch(str, PATTERN);
        }
        /// <summary>
        /// 是否为ip
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIP(this string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }
        /// <summary>
        /// 检查一个字符串是否是纯数字构成
        /// </summary>
        /// <param name="_value">需验证的字符串</param>
        /// <returns>是否合法的bool值。</returns>
        public static bool IsNumberId(this string _value)
        {
            return QuickValidate("^[0-9]*$", _value);
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
            Regex myRegex = new(_express);
            if (_value.Length == 0)
            {
                return false;
            }
            return myRegex.IsMatch(_value);
        }

        /// <summary>
        /// 检测是否符合email格式
        /// </summary>
        /// <param name="strEmail">要判断的email字符串</param>
        /// <returns></returns>
        public static bool IsEmail(this string strEmail)
        {
            return Regex.IsMatch(strEmail, @"^[\w\.]+([-]\w+)*@[A-Za-z0-9-_]+[\.][A-Za-z0-9-_]");
        }
        public static bool IsDoEmail(this string strEmail)
        {
            return Regex.IsMatch(strEmail, @"^@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        /// <summary>
        /// 检测是否是正确的Url
        /// </summary>
        /// <param name="strUrl">要验证的Url</param>
        /// <returns>判断结果</returns>
        public static bool IsURL(string strUrl)
        {
            return Regex.IsMatch(strUrl, @"^(http|https)\://([a-zA-Z0-9\.\-]+(\:[a-zA-Z0-9\.&%\$\-]+)*@)*((25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])|localhost|([a-zA-Z0-9\-]+\.)*[a-zA-Z0-9\-]+\.(com|edu|gov|int|mil|net|org|biz|arpa|info|name|pro|aero|coop|museum|[a-zA-Z]{1,10}))(\:[0-9]+)*(/($|[a-zA-Z0-9\.\,\?\'\\\+&%\$#\=~_\-]+))*$");
        }
        #endregion

        #region 检测是否有Sql危险字符
        /// <summary>
        /// 检测是否有Sql危险字符
        /// </summary>
        /// <param name="str">要判断字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsSafeSql(this string str)
        {
            return !Regex.IsMatch(str, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
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

        /// <summary>
        /// 查找某一字符串在目标字符串里出现的次数
        /// </summary>
        /// <param name="enstr">源字符串</param>
        /// <param name="Destr">目标字符串</param>
        /// <returns>0 不存在 -1 查找失败 >0出现次数</returns>
        public static int FindStringCount(this string enstr, string Destr)
        {
            int count = 0;
            try
            {
                string a = enstr;
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
        public static string GetNewStyle(this string str, string newStyle, string splitString)
        {
            string ReturnValue;
            //如果输入空值，返回空，并给出错误提示
            if (str == null)
            {
                ReturnValue = "";
               
            }
            else
            {
                //检查传入的字符串长度和样式是否匹配,如果不匹配，则说明使用错误。给出错误信息并返回空值
                int strListLength = str.Length;
                int NewStyleLength = newStyle.Replace(splitString, "").Length;
                if (strListLength != NewStyleLength)
                {
                    ReturnValue = "";
                    
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
                }
            }
            return ReturnValue;
        }
       
        #region  全角 半角 (SBC case) 
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
        /// 二进制字符串转byte 
        /// </summary>
        /// <param name="byteStr">二进制字符串</param>
        /// <returns>返回相应的byte值</returns>
        public static byte BinaryString(this string byteStr)
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
            {
                // 8 bit处理  
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
        public static byte[] ToBCDByteArray(this string str)
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
        public static byte[] GetBytes(this string s, int Length, Encoding encoding)
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
        /// 16进制字符串转字节数组 
        /// </summary> 
        /// <param name="hexString"></param> 
        /// <returns></returns> 
        public static byte[] ToHexBytes(this string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2).Trim(), 16);
            return returnBytes;

        }
        /// <summary> 
        /// 换到16进制   
        /// </summary> 
        /// <param name="s"></param> 
        /// <param name="charset">编码,如"utf-8","gb2312"</param> 
        /// <param name="splitStr">分隔符</param> 
        /// <returns></returns> 
        public static string ToHex(this string s, Encoding charset, string splitStr=" ")
        {
            if ((s.Length % 2) != 0)
            {
                s += " ";//空格
            }
            //Encoding chs = System.Text.Encoding.GetEncoding(charset);
            byte[] bytes = charset.GetBytes(s);
            string str = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                str += string.Format("{0:X}", bytes[i]);
                if ((i != bytes.Length - 1))
                {
                    str += string.Format("{0}", splitStr);
                }
            }
            return str.ToLower();
        }
        /// <summary>
        /// 转换为 Hex码（即16进制数组）
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] ToHex(this string data)
        {
            int len = data.Length;
            if (len % 2 != 0)
            {
                data += " ";//空格  
            }
            byte[] bytes = new byte[len / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                try
                {
                    // 每两个字符是一个 byte。  
                    bytes[i] = byte.Parse(data.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
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
        /// 从16进制转换成汉字 
        /// </summary> 
        /// <param name="hex"></param> 
        /// <param name="charset">编码,如"utf-8","gb2312"</param> 
        /// <returns></returns>  
        public static string UnHex(this string hex, Encoding charset)
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
                    bytes[i] = byte.Parse(hex.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
                }
                catch
                {
                    // Rethrow an exception with custom message.             
                    throw new ArgumentException("hex is not a valid hex number!", nameof(hex));
                }
            }
            //Encoding chs = System.Text.Encoding.GetEncoding(charset);
            return charset.GetString(bytes);
        }
        /// <summary>
        /// 转换为 key value
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ToDictionary(this string text, string splitStr = "&")
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;
            var kvs = Regex.Split(text, splitStr);
            if ((kvs?.Length ?? 0) <= 0)
                return null;
            return kvs.ToDictionary(item => Regex.Split(item, "=")[0], item => WebUtility.UrlDecode(Regex.Split(item, "=")[1]));
        }

        /// <summary>
        /// Hex转Byte数组
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static byte[] HexToBytes(this string text)
        {
            var list = new List<string>();
            if (text.Contains(' '))
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
            return list.Select(item => Convert.ToByte(item, 16)).ToArray();
        }

        ///// <summary>
        ///// Hex转Byte数组（无空格)
        ///// </summary>
        ///// <param name="hexstr"></param>
        ///// <returns></returns>
        //public static byte[] HexToBytes(this string text)
        //{
        //    string[] list = System.Text.RegularExpressions.Regex.Replace(text, @"(\w{2})", "$1-").Trim('-').Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
        //    byte[] dataArray = new byte[list.Length];
        //    for (int i = 0; i < list.Length; i++)
        //    {
        //        dataArray[i] = Convert.ToByte(Convert.ToInt32(list[i], 16));
        //    }
        //    return dataArray;
        //}

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
        /// 字符串转Ascii码数组
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] ToAscIIByte(this string str)
        {
            char[] charBuf = str.ToArray();    // 将字符串转换为字符数组
            ASCIIEncoding charToASCII = new();
            byte[] TxdBuf = new byte[charBuf.Length];    // 定义发送缓冲区；
            TxdBuf = charToASCII.GetBytes(charBuf);    // 转换为各字符对应的ASCII
            return TxdBuf;
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


        #endregion

    }
}
