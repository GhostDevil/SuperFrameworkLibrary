using System;
using System.Collections;
using System.Text;

namespace SuperFramework.SuperConvert
{
    /// <summary>
    /// 进制转换类
    /// </summary>
    public static class Hexadecimal
    {
        #region  各进制数间转换 
        /// <summary>
        /// 实现各进制数间的转换。ConvertBase("15",10,16)表示将十进制数15转换为16进制的数。
        /// </summary>
        /// <param name="value">要转换的值,即原值</param>
        /// <param name="from">原值的进制,只能是2,8,10,16四个值。</param>
        /// <param name="to">要转换到的目标进制，只能是2,8,10,16四个值。</param>
        /// <returns></returns>
        public static string ConvertBase(string value, int from, int to)
        {
            try
            {
                if(string.IsNullOrWhiteSpace(value))
                    return "";
                int intValue = Convert.ToInt32(value, from);  //先转成10进制
                string result = Convert.ToString(intValue, to);  //再转成目标进制
                if (to == 2)
                {
                    int resultLength = result.Length;  //获取二进制的长度
                    switch (resultLength)
                    {
                        case 7:
                            result = "0" + result;
                            break;
                        case 6:
                            result = "00" + result;
                            break;
                        case 5:
                            result = "000" + result;
                            break;
                        case 4:
                            result = "0000" + result;
                            break;
                        case 3:
                            result = "00000" + result;
                            break;
                    }
                }
                if (to == 16)
                {
                    if (result.Length == 1)
                        result = "0" + result;
                }
                
                return result;
            }
            catch
            {
                return "0";
            }
        }
        #endregion

        #region  十进制转换为二进制 
        /// <summary>
        /// 十进制转换为二进制
        /// </summary>
        /// <param name="x">10进制字符串</param>
        /// <returns>返回2进制字符串</returns>
        public static string DecToBin(string x)
        {
            int X = Convert.ToInt32(x);
            int i = 0;
            long a, b = 0;
            while (X > 0)
            {
                a = X % 2;
                X /= 2;
                b += a * Pow(10, i);
                i++;
            }
            string z = Convert.ToString(b);
            return z;
        }
        #endregion

        #region  16进制转ASCII码 
        /// <summary>
        /// 16进制转ASCII码
        /// </summary>
        /// <param name="hexString">16进制字符串</param>
        /// <returns>返回ASSCII码</returns>
        public static string HexToAscii(string hexString)
        {
            StringBuilder sb = new();
            for (int i = 0; i <= hexString.Length - 2; i += 2)
            {
                sb.Append(
                    Convert.ToString(
                        Convert.ToChar(int.Parse(hexString.Substring(i, 2),
                                                   System.Globalization.NumberStyles.HexNumber))));
            }
            return sb.ToString();
        }
        #endregion

        #region  十进制转换为八进制 
        /// <summary>
        /// 十进制转换为八进制
        /// </summary>
        /// <param name="x">10进制字符串</param>
        /// <returns>返回8进制字符串</returns>
        public static string DecToOtc(string x)
        {
            int X = Convert.ToInt32(x);
            int i = 0;
            long a, b = 0;
            while (X > 0)
            {
                a = X % 8;
                X /= 8;
                b += a * Pow(10, i);
                i++;
            }
            string z = Convert.ToString(b);
            return z;
        }
        #endregion

        #region  十进制转换为十六进制 
        /// <summary>
        /// 十进制转换为十六进制
        /// </summary>
        /// <param name="x">10进制字符串</param>
        /// <returns>返回16进制字符串</returns>
        public static string DecToHex(string x)
        {
            if (string.IsNullOrEmpty(x))
            {
                return "0";
            }
            string z = null;
            int X = Convert.ToInt32(x);
            Stack a = new();
            int i = 0;
            while (X > 0)
            {
                a.Push(Convert.ToString(X % 16));
                X /= 16;
                i++;
            }
            while (a.Count != 0)
                z += ToHex(Convert.ToString(a.Pop()));
            if (string.IsNullOrEmpty(z))
            {
                z = "00";
            }
            else if (z.Length == 1)
                z = "0" + z;
            return z;
        }
        #endregion

        #region  二进制转换为十进制 
        /// <summary>
        /// 二进制转换为十进制
        /// </summary>
        /// <param name="x">2进制字符串</param>
        /// <returns>返回10进制字符串</returns>
        public static string BinToDec(string x)
        {
            int X = Convert.ToInt32(x);
            int i = 0;
            long a, b = 0;
            while (X > 0)
            {
                a = X % 10;
                X /= 10;
                b += a * Pow(2, i);
                i++;
            }
            string z = Convert.ToString(b);
            return z;
        }
        #endregion

        #region  二进制转换为十进制，定长转换。 
        /// <summary>
        /// 二进制转换为十进制，定长转换。
        /// </summary>
        /// <param name="x">2进制字符串</param>
        /// <param name="iLength">定长长度</param>
        /// <returns>返回10进制字符串</returns>
        public static string BinToDec(string x, short iLength)
        {
            StringBuilder sb = new();
            int iCount = x.Length / iLength;
            if (x.Length % iLength > 0)
            {
                iCount += 1;
            }

            for (int i = 0; i < iCount; i++)
            {

                int X;
                if ((i + 1) * iLength > x.Length)
                {
                    X = Convert.ToInt32(x.Substring(i * iLength, (x.Length - iLength)));
                }
                else
                {
                    X = Convert.ToInt32(x.Substring(i * iLength, iLength));
                }
                int j = 0;
                long a, b = 0;
                while (X > 0)
                {
                    a = X % 10;
                    X /= 10;
                    b += a * Pow(2, j);
                    j++;
                }
                sb.AppendFormat("{0:D2}", b);
            }
            return sb.ToString();
        }
        #endregion

        #region  二进制转换为十六进制，定长转换。 
        /// <summary>
        /// 二进制转换为十六进制，定长转换。
        /// </summary>
        /// <param name="x">2进制字符串</param>
        /// <param name="iLength">定长长度</param>
        /// <returns>返回16进制字符串</returns>
        public static string BinToHex(string x, short iLength)
        {
            StringBuilder sb = new();
            int iCount = x.Length / iLength;
            if (x.Length % iLength > 0)
            {
                iCount += 1;
            }

            for (int i = 0; i < iCount; i++)
            {

                int X;
                if ((i + 1) * iLength > x.Length)
                {
                    X = Convert.ToInt32(x.Substring(i * iLength, (x.Length - iLength)));
                }
                else
                {
                    X = Convert.ToInt32(x.Substring(i * iLength, iLength));
                }
                int j = 0;
                long a, b = 0;
                while (X > 0)
                {
                    a = X % 10;
                    X /= 10;
                    b += a * Pow(2, j);
                    j++;
                }
                //前补0
                sb.Append(DecToHex(b.ToString()));
            }
            return sb.ToString();
        }
        #endregion

        #region  八进制转换为十进制 
        /// <summary>
        /// 八进制转换为十进制
        /// </summary>
        /// <param name="x">8进制字符串</param>
        /// <returns>返回10进制字符串</returns>
        public static string OctToDec(string x)
        {
            int X = Convert.ToInt32(x);
            int i = 0;
            long a, b = 0;
            while (X > 0)
            {
                a = X % 10;
                X /= 10;
                b += a * Pow(8, i);
                i++;
            }
            string z = Convert.ToString(b);
            return z;
        }
        #endregion

        #region  十六进制转换为十进制 
        /// <summary>
        /// 十六进制转换为十进制
        /// </summary>
        /// <param name="x">16进制字符串</param>
        /// <returns>返回10进制字符串</returns>
        public static string HexToDec(string x)
        {
            if (string.IsNullOrEmpty(x))
            {
                return "0";
            }

            Stack a = new();
            int i = 0, j = 0, l = x.Length;
            long Tong = 0;
            while (i < l)
            {
                a.Push(ToDec(Convert.ToString(x[i])));
                i++;
            }
            while (a.Count != 0)
            {
                Tong += Convert.ToInt64(a.Pop()) * Pow(16, j);
                j++;
            }
            string z = Convert.ToString(Tong);
            return z;
        }

        #endregion

        #region  将16进制BYTE数组转换成16进制字符串 
        /// <summary>
        /// 将16进制BYTE数组转换成16进制字符串
        /// </summary>
        /// <param name="bytes">16进制字节</param>
        /// <returns>返回16进制字符串</returns>
        public static string ToHexString(byte[] bytes) // 0xae00cf => "AE00CF "
        {
            string hexString = string.Empty;
            if (bytes != null)
            {
                StringBuilder strB = new();

                for (int i = 0; i < bytes.Length; i++)
                {
                    strB.Append(bytes[i].ToString("X2"));
                }
                hexString = strB.ToString();
            }
            return hexString;
        }
        #endregion

        #region  10进制与62进制互转 

        private static string keys = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";//编码,可加一些字符也可以实现72,96等任意进制转换，但是有符号数据不直观，会影响阅读。
        private static int exponent = keys.Length;//幂数

        /// <summary>
        /// 十进制值类型为62编码
        /// </summary>
        /// <param name="value">十进制数，最大值不能大于17223472558080896352ule</param>
        /// <returns>返回一个指定的62编码的字符串</returns>
        public static string DecimalToStr(decimal value)//17223472558080896352ul
        {
            string result = string.Empty;
            do
            {
                decimal index = value % exponent;
                result = keys[(int)index] + result;
                value = (value - index) / exponent;
            }
            while (value > 0);

            return result;
        }


        /// <summary>
        /// 62编码的字符串到十进制
        /// </summary>
        /// <param name="value">62编码的字符串</param>
        /// <returns>返回一个指定的十进制数的字符串</returns>
        public static decimal StrToDecimal(string value)//bUI6zOLZTrj
        {
            decimal result = 0;
            for (int i = 0; i < value.Length; i++)
            {
                int x = value.Length - i - 1;
                result += keys.IndexOf(value[i]) * Pow(exponent, x);// Math.Pow(exponent, x);
            }
            return result;
        }

        /// <summary>
        /// 字符串转16进制字节数组
        /// </summary>
        /// <param name="hexString">字符串</param>
        /// <returns>返回16进制字节数组</returns>
        private static byte[] StrToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        /// <summary>
        /// 字节数组转16进制字符串
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <returns>返回16进制字符串</returns>
        public static string ByteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }
        /// <summary>
        /// 字节数组转16进制字符串
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="length">需要的长度</param>
        /// <returns>返回16进制字符串</returns>
        public static string ByteToHexStr(byte[] bytes,int length)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }

        /// <summary>
        /// 从汉字转换到16进制
        /// </summary>
        /// <param name="s"></param>
        /// <param name="charset">编码,如"utf-8","gb2312"</param>
        /// <param name="fenge">是否每字符用逗号分隔</param>
        /// <returns>返回16进制字符串</returns>
        public static string ToHex(string s, string charset, bool fenge)
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
        /// <returns>返回汉字</returns>
        public static string UnHex(string hex, string charset)
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

        /// <summary>
        /// 一个数据的N次方
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private static decimal Pow(decimal baseNo, decimal x)
        {
            decimal value = 1;////1 will be the result for any number's power 0.任何数的0次方，结果都等于1
            while (x > 0)
            {
                value *= baseNo;
                x--;
            }
            return value;
        }
        #endregion

        #region  十六进制字字符串转为节数组 
        /// <summary>
        /// 十六进制字字符串转为节数组
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static byte[] StringToHexByteArray(string s)
        {
            s = s.Replace(" ", "");
            if ((s.Length % 2) != 0)
                s += " ";
            byte[] returnBytes = new byte[s.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(s.Substring(i * 2, 2), 16);
            return returnBytes;
        }
        #endregion

        /// <summary>
        /// 十六进制字字符串转为节数组
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static byte[] HexStringToByteArray(string s)
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }

        #region  进制转换私有方法 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static long Pow(long x, long y)
        {
            int i = 1;
            long X = x;
            if (y == 0)
                return 1;
            while (i < y)
            {
                x *= X;
                i++;
            }
            return x;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private static string ToDec(string x)
        {
            switch (x)
            {
                case "A":
                    return "10";
                case "B":
                    return "11";
                case "C":
                    return "12";
                case "D":
                    return "13";
                case "E":
                    return "14";
                case "F":
                    return "15";
                default:
                    return x;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private static string ToHex(string x)
        {
            switch (x)
            {
                case "10":
                    return "A";
                case "11":
                    return "B";
                case "12":
                    return "C";
                case "13":
                    return "D";
                case "14":
                    return "E";
                case "15":
                    return "F";
                default:
                    return x;
            }
        }
        #endregion
    }
}
