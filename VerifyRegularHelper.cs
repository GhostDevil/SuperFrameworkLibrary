using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace SuperFramework
{
    /// <summary>
    /// 正则验证等验证辅助方法
    /// </summary>
    public static class VerifyRegularHelper
    {
        #region  验证邮箱 
        /// <summary>
        /// 验证邮箱
        /// </summary>
        /// <param name="source">要验证的字符串</param>
        /// <returns>符合返回true，不符合返回false</returns>
        public static bool IsEmail(string source)
        {
            return Regex.IsMatch(source, @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", RegexOptions.IgnoreCase);
        }
        /// <summary>
        /// 验证邮箱
        /// </summary>
        /// <param name="source">要验证的字符串</param>
        /// <returns>符合返回true，不符合返回false</returns>
        public static bool HasEmail(string source)
        {
            return Regex.IsMatch(source, @"[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})", RegexOptions.IgnoreCase);
        }
        #endregion

        #region  验证网址 
        /// <summary>
        /// 验证网址
        /// </summary>
        /// <param name="source">要验证的字符串</param>
        /// <returns>符合返回true，不符合返回false</returns>
        public static bool IsUrl(string source)
        {
            return Regex.IsMatch(source, @"^(((file|gopher|news|nntp|telnet|http|ftp|https|ftps|sftp)://)|(www\.))+(([a-zA-Z0-9\._-]+\.[a-zA-Z]{2,6})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(/[a-zA-Z0-9\&amp;%_\./-~-]*)?$", RegexOptions.IgnoreCase);
        }
        /// <summary>
        /// 验证网址
        /// </summary>
        /// <param name="source">要验证的字符串</param>
        /// <returns>符合返回true，不符合返回false</returns>
        public static bool HasUrl(string source)
        {
            return Regex.IsMatch(source, @"(((file|gopher|news|nntp|telnet|http|ftp|https|ftps|sftp)://)|(www\.))+(([a-zA-Z0-9\._-]+\.[a-zA-Z]{2,6})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(/[a-zA-Z0-9\&amp;%_\./-~-]*)?", RegexOptions.IgnoreCase);
        }
        #endregion

        #region  验证日期 
        /// <summary>
        /// 验证日期
        /// </summary>
        /// <param name="source">要验证的字符串</param>
        /// <returns>符合返回true，不符合返回false</returns>
        public static bool IsDateTime(string source)
        {
            try
            {
                DateTime time = Convert.ToDateTime(source);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region  验证手机号 
        /// <summary>
        /// 验证手机号
        /// </summary>
        /// <param name="source">要验证的手机号码</param>
        /// <returns>符合返回true，不符合返回false</returns>
        public static bool IsMobile(string source)
        {
            return Regex.IsMatch(source, @"^1[35]\d{9}$", RegexOptions.IgnoreCase);
        }
        /// <summary>
        /// 验证手机号
        /// </summary>
        /// <param name="source">要验证的手机号码</param>
        /// <returns>符合返回true，不符合返回false</returns>
        public static bool HasMobile(string source)
        {
            return Regex.IsMatch(source, @"1[35]\d{9}", RegexOptions.IgnoreCase);
        }
        #endregion

        #region  验证IP 
        /// <summary>
        /// 验证IP
        /// </summary>
        /// <param name="source">要验证的ip</param>
        /// <returns>符合返回true，不符合返回false</returns>
        public static bool IsIP(string source)
        {
            return Regex.IsMatch(source, @"^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])$", RegexOptions.IgnoreCase);
        }
        /// <summary>
        /// 验证IP
        /// </summary>
        /// <param name="source">要验证的ip</param>
        /// <returns>符合返回true，不符合返回false</returns>
        public static bool HasIP(string source)
        {
            return Regex.IsMatch(source, @"(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])", RegexOptions.IgnoreCase);
        }
        #endregion

        #region  验证身份证是否有效 
        /// <summary>
        /// 验证身份证是否有效
        /// </summary>
        /// <param name="Id">要验证的身份证号码</param>
        /// <returns>符合返回true，不符合返回false</returns>
        public static bool IsIDCard(string Id)
        {
            if (Id.Length == 18)
            {
                bool check = IsIDCard18(Id);
                return check;
            }
            else if (Id.Length == 15)
            {
                bool check = IsIDCard15(Id);
                return check;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 验证身份证是否有效--18位
        /// </summary>
        /// <param name="Id">要验证的身份证号码</param>
        /// <returns>符合返回true，不符合返回false</returns>
        public static bool IsIDCard18(string Id)
        {
            long n;
            if (long.TryParse(Id.Remove(17), out n) == false || n < Math.Pow(10, 16) || long.TryParse(Id.Replace('x', '0').Replace('X', '0'), out n) == false)
            {
                return false;//数字验证
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(Id.Remove(2)) == -1)
            {
                return false;//省份验证
            }
            string birth = Id.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证
            }
            string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
            string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
            char[] Ai = Id.Remove(17).ToCharArray();
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
            }
            int y;
            Math.DivRem(sum, 11, out y);
            if (arrVarifyCode[y] != Id.Substring(17, 1).ToLower())
            {
                return false;//校验码验证
            }
            return true;//符合GB11643-1999标准
        }
        /// <summary>
        /// 验证身份证是否有效--15位
        /// </summary>
        /// <param name="Id">要验证的身份证号码</param>
        /// <returns>符合返回true，不符合返回false</returns>
        public static bool IsIDCard15(string Id)
        {
            long n;
            if (long.TryParse(Id, out n) == false || n < Math.Pow(10, 14))
            {
                return false;//数字验证
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(Id.Remove(2)) == -1)
            {
                return false;//省份验证
            }
            string birth = Id.Substring(6, 6).Insert(4, "-").Insert(2, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证
            }
            return true;//符合15位身份证标准
        }
        #endregion

        #region  验证是否是Int型 
        /// <summary>
        /// 验证是否是Int型
        /// </summary>
        /// <param name="source">要验证的字符串</param>
        /// <returns>符合返回true，不符合返回false</returns>
        public static bool IsInt(string source)
        {
            Regex regex = new Regex(@"^(-){0,1}\d+$");
            if (regex.Match(source).Success)
            {
                if ((long.Parse(source) > 0x7fffffffL) || (long.Parse(source) < -2147483648L))
                {
                    return false;
                }
                return true;
            }
            return false;
        }
        #endregion

        #region  验证字符串的长度是不是在限定数之间 一个中文为两个字符 
        /// <summary>
        /// 验证字符串的长度是不是在限定数之间 一个中文为两个字符
        /// </summary>
        /// <param name="source">要验证的字符串</param>
        /// <param name="begin">大于等于</param>
        /// <param name="end">小于等于</param>
        /// <returns>符合返回true，不符合返回false</returns>
        public static bool IsLengthStr(string source, int begin, int end)
        {
            int length = Regex.Replace(source, @"[^\x00-\xff]", "OK").Length;
            if ((length <= begin) && (length >= end))
            {
                return false;
            }
            return true;
        }
        #endregion

        #region  验证是否是中国电话，格式010-85849685。 
        /// <summary>
        /// 验证是否是中国电话，格式010-85849685。
        /// </summary>
        /// <param name="source">要验证的电话号码</param>
        /// <returns>符合返回true，不符合返回false</returns>
        public static bool IsTel(string source)
        {
            return Regex.IsMatch(source, @"^\d{3,4}-?\d{6,8}$", RegexOptions.IgnoreCase);
        }
        #endregion

        #region  匹配3位或4位区号的电话号码，其中区号可以用小括号括起来， 也可以不用，区号与本地号间可以用连字号或空格间隔， 也可以没有间隔。 
        /// <summary>
        /// 匹配3位或4位区号的电话号码，其中区号可以用小括号括起来， 也可以不用，区号与本地号间可以用连字号或空格间隔， 也可以没有间隔。
        /// \(0\d{2}\)[- ]?\d{8}|0\d{2}[- ]?\d{8}|\(0\d{3}\)[- ]?\d{7}|0\d{3}[- ]?\d{7}
        /// </summary>
        /// <param name="input">要验证的电话号码</param>
        /// <returns>符合返回true，不符合返回false</returns>
        public static bool IsPhone(string input)
        {
            string pattern = "^\\(0\\d{2}\\)[- ]?\\d{8}$|^0\\d{2}[- ]?\\d{8}$|^\\(0\\d{3}\\)[- ]?\\d{7}$|^0\\d{3}[- ]?\\d{7}$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(input);
        }
        #endregion

        #region  验证邮政编码 6个数字 
        /// <summary>
        /// 验证邮政编码 6个数字
        /// </summary>
        /// <param name="source">要验证的邮政编码</param>
        /// <returns>符合返回true，不符合返回false</returns>
        public static bool IsPostCode(string source)
        {
            return Regex.IsMatch(source, @"^\d{6}$", RegexOptions.IgnoreCase);
        }
        #endregion

        #region  验证是否中文 
        /// <summary>
        /// 验证是否中文
        /// </summary>
        /// <param name="source">要验证的字符串</param>
        /// <returns>符合返回true，不符合返回false</returns>
        public static bool IsChinese(string source)
        {
            return Regex.IsMatch(source, @"^[\u4e00-\u9fa5]+$", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 验证是否中文
        /// </summary>
        /// <param name="source">要验证的字符串</param>
        /// <returns>符合返回true，不符合返回false</returns>
        public static bool HasChinese(string source)
        {
            return Regex.IsMatch(source, @"[\u4e00-\u9fa5]+", RegexOptions.IgnoreCase);
        }
        #endregion

        #region  检测含有中文字符串的实际长度 
        /// <summary> 
        /// 检测含有中文字符串的实际长度 
        /// </summary> 
        /// <param name="inputData">字符串</param> 
        public static int GetCHZNLength(string inputData)
        {
            ASCIIEncoding n = new ASCIIEncoding();
            byte[] bytes = n.GetBytes(inputData);

            int length = 0; // l 为字符串之实际长度 
            for (int i = 0; i <= bytes.Length - 1; i++)
            {
                if (bytes[i] == 63) //判断是否为汉字或全脚符号 
                {
                    length++;
                }
                length++;
            }
            return length;

        }
        #endregion

        #region  验证是不是正常字符 字母，数字，下划线的组合。 
        /// <summary>
        /// 验证是不是正常字符 字母，数字，下划线的组合。
        /// </summary>
        /// <param name="source">要验证的字符串</param>
        /// <returns>符合返回true，不符合返回false</returns>
        public static bool IsNormalChar(string source)
        {
            return Regex.IsMatch(source, @"[\w\d_]+", RegexOptions.IgnoreCase);
        }
        #endregion

        #region  使用dd-mm-yy 的日期形式代替 mm/dd/yy 的日期形式 
        /// <summary>
        ///使用dd-mm-yy 的日期形式代替 mm/dd/yy 的日期形式
        /// </summary>
        /// <param name="input">要转换的日期字符串</param>
        /// <returns>符合返回true，不符合返回false</returns>
        public static string MDYToDMY(string input)
        {
            return Regex.Replace(input, "\\b(?\\d{1,2})/(?\\d{1,2})/(?\\d{2,4})\\b", "${day}-${month}-${year}");
        }
        #endregion

        #region  验证是否为小数 
        /// <summary>
        ///验证是否为小数 
        /// </summary>
        /// <param name="strIn">要验证的字符串</param>
        /// <returns>符合返回true，不符合返回false</returns>
        public static bool IsValidDecimal(string strIn)
        {
            return Regex.IsMatch(strIn, @"[0].\d{1,2}|[1]");
        }
        #endregion

        #region  验证年月日 
        /// <summary>
        ///验证年月日 
        /// </summary>
        /// <param name="strIn">要验证的年月日字符串</param>
        /// <returns>符合返回true，不符合返回false</returns>
        public static bool IsValidDate(string strIn)
        {
            return Regex.IsMatch(strIn, @"^2\d{3}-(?:0?[1-9]|1[0-2])-(?:0?[1-9]|[1-2]\d|3[0-1])(?:0?[1-9]|1\d|2[0-3]):(?:0?[1-9]|[1-5]\d):(?:0?[1-9]|[1-5]\d)$");
        }
        #endregion

        #region  验证后缀名 
        /// <summary>
        ///验证后缀名 
        /// </summary>
        /// <param name="strIn">要验证的字符串</param>
        /// <returns>符合返回true，不符合返回false</returns>
        static bool IsValidPostfix(string strIn)
        {
            return Regex.IsMatch(strIn, @"\.(?i:gif|jpg)$");
        }
        #endregion

        #region  验证字符是否在4至12之间 
        /// <summary>
        ///验证字符是否在4至12之间
        /// </summary>
        /// <param name="strIn">要验证的字符串</param>
        /// <returns>符合返回true，不符合返回false</returns>

        public static bool IsValidByte(string strIn)
        {
            return Regex.IsMatch(strIn, @"^[a-z]{4,12}$");
        }
        #endregion

        #region  判断输入的字符串是否是一个超链接 
        /// <summary>
        /// 判断输入的字符串是否是一个超链接
        /// </summary>
        /// <param name="input">要验证的超链接</param>
        /// <returns>符合返回true，不符合返回false</returns>
        public static bool IsHerf(string input)
        {
            string pattern = @"^[a-zA-Z]+://(\w+(-\w+)*)(\.(\w+(-\w+)*))*(\?\S*)?$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(input);
        }
        #endregion

        #region  验证输入的字符串是否只包含数字和英文字母 
        /// <summary>
        /// 验证输入的字符串是否只包含数字和英文字母
        /// </summary>
        /// <param name="input">要验证的字符串</param>
        /// <returns>符合返回true，不符合返回false</returns>
        public static bool IsNumAndEnCh(string input)
        {
            string pattern = @"^[A-Za-z0-9]+$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(input);
        }
        #endregion

        #region  验证输入的字符串字包含英文字母 
        /// <summary>
        /// 验证输入的字符串字包含英文字母
        /// </summary>
        /// <param name="input">要验证的字符串</param>
        /// <returns>符合返回true，不符合返回false</returns>
        public static bool IsEnglisCh(string input)
        {
            Regex regex = new Regex("^[A-Za-z]+$");
            return regex.IsMatch(input);
        }
        #endregion

        #region  验证正整数 
        /// <summary>
        /// 验证正整数
        /// </summary>
        /// <param name="input">要验证的字符串</param>
        /// <returns>符合返回true，不符合返回false</returns>
        public static bool IsUint(string input)
        {
            Regex regex = new Regex("^[0-9]*[1-9][0-9]*$");
            return regex.IsMatch(input);
        }
        #endregion

        #region  验证非负整数 
        /// <summary>
        /// 验证非负整数
        /// </summary>
        /// <param name="input">要验证的字符串</param>
        /// <returns>符合返回true，不符合返回false</returns>
        public static bool IsNotNagtive(string input)
        {
            Regex regex = new Regex(@"^\d+$");
            return regex.IsMatch(input);
        }
        #endregion

        #region  验证输入的字符串只包含数字，可以匹配整数和浮点数。 
        /// <summary>
        /// 验证输入的字符串只包含数字，可以匹配整数和浮点数。
        /// ^-?\d+$|^(-?\d+)(\.\d+)?$
        /// </summary>
        /// <param name="input">要验证的字符串</param>
        /// <returns>符合返回true，不符合返回false</returns>
        public static bool IsNumber(string input)
        {
            string pattern = "^-?\\d+$|^(-?\\d+)(\\.\\d+)?$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(input);
        }
        #endregion

        #region  非空验证 
        /// <summary>
        /// 非空验证
        /// </summary>
        /// <param name="input">要验证的字符串</param>
        /// <returns>符合返回true，不符合返回false</returns>
        public static bool IsNull(string input)
        {
            Regex regex = new Regex("^\\S+$");
            return regex.IsMatch(input);
        }
        #endregion

        #region  验证文本框输入为传真号码 
        /// <summary>
        /// 验证文本框输入为传真号码
        /// </summary>
        /// <param name="strFax">要验证的字符串</param>
        /// <returns>符合返回true，不符合返回false</returns>
        public static bool validateFax(string strFax)
        {
            return Regex.IsMatch(strFax, @"86-\d{2,3}-\d{7,8}");
        }
        #endregion

        #region  验证字符串是否是yy-mm-dd字符串 
        /// <summary>
        /// 验证字符串是否是yy-mm-dd字符串
        /// </summary>
        /// <param name="str">要验证的字符串</param>
        /// <returns>符合返回true，不符合返回false</returns>
        public static bool IsDateString(string str)
        {
            return Regex.IsMatch(str, @"（\d{4}）-（\d{1,2}）-（\d{1,2}）");
        }
        #endregion

        #region  正则验证 
        /// <summary>
        ///正则验证
        /// </summary>
        /// <param name="str">要验证的字符串</param>
        /// <param name="regexStr">正则表达式</param>
        /// <returns>符合返回true，不符合返回false</returns>
        public static bool GetRegexInput(string str, string regexStr)
        {
            Regex rStr = new Regex(regexStr);
            bool bResult = false;
            if (rStr.IsMatch(str))
            {
                bResult = true;
            }
            return bResult;
        }
        #endregion

        #region  验证注册密码格式（至少为6位，由非纯数字或字母组成） 
        /// <summary>
        ///验证注册密码格式（至少为6位，由非纯数字或字母组成） 
        /// </summary>
        /// <param name="str">要验证的字符串</param>
        /// <returns>符合返回true，不符合返回false</returns>
        public static bool ChangePassword(string str)
        {
            string regSfzh = @"^(?=.*?[A-Za-z])(?=.*?[0-9])[0-9A-Za-z]{6,32}$";
            if (!GetRegexInput(str, regSfzh))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region  验证字符串全为字母 
        /// <summary>
        /// 验证字符串全为字母
        /// </summary>
        /// <param name="str">要验证的字符串</param>
        /// <returns>符合返回true，不符合返回false</returns>
        public static bool IsLetter(string str)
        {
            Regex rex = new Regex("[a-z|A-Z]{8}");
            Match ma = rex.Match(str);
            if (ma.Success)
                return true;
            else
                return false;
        }
        #endregion

        #region  验证字符串全为数字 
        /// <summary>
        /// 验证字符串全为数字
        /// </summary>
        /// <param name="str">要验证的字符串</param>
        /// <returns>符合返回true，不符合返回false</returns>
        public static bool IsAllNumber(string str)
        {
            Regex rex = new Regex("[0-9]{8}");
            Match ma = rex.Match(str);
            if (ma.Success)
                return true;
            else
                return false;
        }
        #endregion

        #region 验证文件真假--不含文本文件 
        /// <summary>
        /// 验证文件真假--不含文本文件
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <returns>符合返回true，不符合返回false</returns>
        public static bool FileExtension(string fileName, FileType type)
        {
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        string fileType = string.Empty;
                        byte data = br.ReadByte();
                        fileType += data.ToString();
                        data = br.ReadByte();
                        fileType += data.ToString();
                        FileType fp;
                        fp = (FileType)Enum.Parse(typeof(FileType), fileType);
                        if (fp.ToString() == type.ToString())
                            return true;
                        else
                            return false;
                    }
                }
            }
            catch { return false; }
        }
        #endregion

        #region 验证是否是文本文件 
        /// <summary>
        /// 验证是否是文本文件
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <returns>符合返回true，不符合返回false</returns>
        public static bool CheckIsTextFile(string fileName)
        {
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    bool isTextFile = true;
                    int i = 0;
                    int length = (int)fs.Length;
                    byte data;
                    while (i < length && isTextFile)
                    {
                        data = (byte)fs.ReadByte();
                        isTextFile = (data != 0);
                        i++;
                    }
                    return isTextFile;
                }
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region  判断图片文件格式是否正确 
        /// <summary>
        /// 判断图片文件格式是否正确
        /// </summary>
        /// <param name="filesName">图片路径</param>
        /// <param name="imageformat">图片格式</param>
        /// <returns>返回true格式正确，fales格式不正确</returns>
        public static bool getImageFormat(string filesName, System.Drawing.Imaging.ImageFormat imageformat)
        {
            bool IsTrue = false;
            using (System.Drawing.Image image1 = System.Drawing.Image.FromFile(filesName))
            {
                if (image1.RawFormat.Equals(imageformat))
                    IsTrue = true;
            }
            return IsTrue;
        }
        #endregion

        #region  获得文件真实格式 
        /// <summary>
        /// 获得文件真实格式
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <returns>返回文件格式</returns>
        public static string GetFileExtension(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    string fileType = string.Empty;
                    byte data = br.ReadByte();
                    fileType += data.ToString();
                    data = br.ReadByte();
                    fileType += data.ToString();
                    FileType fp;
                    fp = (FileType)Enum.Parse(typeof(FileType), fileType);
                    return fp.ToString();
                }
            }
        }
        #endregion

        #region  文件头-十进制 
        /// <summary>
        /// 文件头-十进制
        /// </summary>
        public enum FileType
        {
            JPG = 255216,
            GIF = 7173,
            BMP = 6677,
            PNG = 13780,
            COM = 7790,
            EXE = 7790,
            DLL = 7790,
            RAR = 8297,
            ZIP = 8075,
            XML = 6063,
            HTML = 6033,
            ASPX = 239187,
            CS = 117115,
            //JS=4742,
            JS = 119105,
            //TXT = 210187,
            //4946/104116 \5150 txt
            //TXT=4946,//不准
            SQL = 255254,
            BAT = 64101,
            BTSEED = 10056,
            RDP = 255254,
            PSD = 5666,
            PDF = 3780,
            CHM = 7384,
            LOG = 70105,
            REG = 8269,
            HLP = 6395,
            DOC = 208207,
            XLS = 208207,
            DOCX = 208207,
            XLSX = 208207,
            VALIDFILE = 9999999,

            MIDI = 7784,
            MKV = 2669,
            RMVB = 4682,
            XV = 8876,
            LRC = 91118,
            MP3 = 255251,
            APE = 7765,
            MDF = 115,
            LDF = 115,
            WAV = 8273,
            BT种子 = 10056,
            ACCDB = 01,
            MDB = 01,
        }
        #endregion

        #region  判断对象是否为空 
        /// <summary>
        /// 判断对象是否为空，为空返回true
        /// </summary>
        /// <typeparam name="T">要验证的对象的类型</typeparam>
        /// <param name="data">要验证的对象</param>        
        public static bool IsNullOrEmpty<T>(T data)
        {
            //如果为null
            if (data == null)
            {
                return true;
            }

            //如果为""
            if (data.GetType() == typeof(string))
            {
                if (string.IsNullOrEmpty(data.ToString().Trim()))
                {
                    return true;
                }
            }

            //如果为DBNull
            if (data.GetType() == typeof(DBNull))
            {
                return true;
            }

            //不为空
            return false;
        }

        /// <summary>
        /// 判断对象是否为空，为空返回true
        /// </summary>
        /// <param name="data">要验证的对象</param>
        public static bool IsNullOrEmpty(object data)
        {
            //如果为null
            if (data == null)
            {
                return true;
            }

            //如果为""
            if (data.GetType() == typeof(string))
            {
                if (string.IsNullOrEmpty(data.ToString().Trim()))
                {
                    return true;
                }
            }

            //如果为DBNull
            if (data.GetType() == typeof(DBNull))
            {
                return true;
            }

            //不为空
            return false;
        }
        #endregion

        #region  检测客户的输入中是否有危险字符串 
        /// <summary>
        /// 检测客户输入的字符串是否有效,并将原始字符串修改为有效字符串或空字符串。
        /// 当检测到客户的输入中有攻击性危险字符串,则返回false,有效返回true。
        /// </summary>
        /// <param name="input">要检测的字符串</param>
        public static bool IsValidInput(ref string input)
        {
            try
            {
                if (IsNullOrEmpty(input))
                {
                    //如果是空值,则跳出
                    return true;
                }
                else
                {
                    //替换单引号
                    input = input.Replace("'", "''").Trim();

                    //检测攻击性危险字符串
                    string testString = "and |or |exec |insert |select |delete |update |count |chr |mid |master |truncate |char |declare ";
                    string[] testArray = testString.Split('|');
                    foreach (string testStr in testArray)
                    {
                        if (input.ToLower().IndexOf(testStr) != -1)
                        {
                            //检测到攻击字符串,清空传入的值
                            input = "";
                            return false;
                        }
                    }

                    //未检测到攻击字符串
                    return true;
                }
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region  验证输入字符串是否与模式字符串匹配 
        /// <summary>
        /// 验证输入字符串是否与模式字符串匹配
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <param name="pattern">模式字符串</param> 
        /// <returns>匹配返回true，返回false不匹配</returns>
       
        public static bool IsMatch(string input, string pattern)
        {
            return IsMatch(input, pattern, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 验证输入字符串是否与模式字符串匹配
        /// </summary>
        /// <param name="input">输入的字符串</param>
        /// <param name="pattern">模式字符串</param>
        /// <param name="options">筛选条件</param>
        /// <returns>匹配返回true，返回false不匹配</returns>
        public static bool IsMatch(string input, string pattern, RegexOptions options)
        {
            return Regex.IsMatch(input, pattern, options);
        }
        #endregion

        #region  密码有效性 
        /// <summary>
        /// 密码有效性
        /// </summary>
        /// <param name="password">待验证密码</param>
        /// <returns>有效返回true，否则无效</returns>
        public static bool IsValidPassword(string password)
        {
            return Regex.IsMatch(password, @"^[A-Za-z_0-9]{6,16}$");
        }
        #endregion

        #region  日期检查 

        /// <summary>
        /// 判断输入的字符是否为日期
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public static bool IsDate(string strValue)
        {
            return Regex.IsMatch(strValue, @"^((\d{2}(([02468][048])|([13579][26]))[\-\/\s]?((((0?[13578])|(1[02]))[\-\/\s]?((0?[1-9])|([1-2][0-9])|(3[01])))|(((0?[469])|(11))[\-\/\s]?((0?[1-9])|([1-2][0-9])|(30)))|(0?2[\-\/\s]?((0?[1-9])|([1-2][0-9])))))|(\d{2}(([02468][1235679])|([13579][01345789]))[\-\/\s]?((((0?[13578])|(1[02]))[\-\/\s]?((0?[1-9])|([1-2][0-9])|(3[01])))|(((0?[469])|(11))[\-\/\s]?((0?[1-9])|([1-2][0-9])|(30)))|(0?2[\-\/\s]?((0?[1-9])|(1[0-9])|(2[0-8]))))))");
        }

        /// <summary>
        /// 判断输入的字符是否为日期,如2004-07-12 14:25|||1900-01-01 00:00|||9999-12-31 23:59
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public static bool IsDateHourMinute(string strValue)
        {
            return Regex.IsMatch(strValue, @"^(19[0-9]{2}|[2-9][0-9]{3})-((0(1|3|5|7|8)|10|12)-(0[1-9]|1[0-9]|2[0-9]|3[0-1])|(0(4|6|9)|11)-(0[1-9]|1[0-9]|2[0-9]|30)|(02)-(0[1-9]|1[0-9]|2[0-9]))\x20(0[0-9]|1[0-9]|2[0-3])(:[0-5][0-9]){1}$");
        }

        #endregion

        #region  检查字符串最大长度，返回指定长度的字符串 
        /// <summary>
        /// 检查字符串最大长度，返回指定长度的串
        /// </summary>
        /// <param name="inputData">输入字符串</param>
        /// <param name="maxLength">最大长度</param>
        /// <returns>返回指定长度字符串</returns>         
        public static string CheckMathLength(string inputData, int maxLength)
        {
            if (inputData != null && inputData != string.Empty)
            {
                inputData = inputData.Trim();
                if (inputData.Length > maxLength)//按最大长度截取字符串
                {
                    inputData = inputData.Substring(0, maxLength);
                }
            }
            return inputData;
        }
        #endregion

        #region  返回字符串真实长度 
        /// <summary>
        /// 返回字符串真实长度
        /// </summary>
        /// <returns>字符长度</returns>
        public static int GetStringLength(string stringValue)
        {
            return Encoding.Default.GetBytes(stringValue).Length;
        }
        #endregion
    }
}
