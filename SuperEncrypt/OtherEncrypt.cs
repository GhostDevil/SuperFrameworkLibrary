using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SuperFramework.SuperEncrypt
{
    /// <summary>
    /// 日 期:2014-09-15
    /// 作 者:不良帥
    /// 描 述:其他加密解密方法类
    /// </summary>
    public class OtherEncrypt
    {
        #region  加密所用的伪随机数 
        /// <summary>
        /// 加密所用的伪随机数
        /// </summary>
        private string randStr = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
        #endregion

        #region  伪随机数加密 
        /// <summary>
        /// 伪随机数加密
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>加密后的字符串</returns>
        private string EncryptPwd(string str)
        {
            byte[] btData = Encoding.Default.GetBytes(str);
            int j, k, m;
            int len = randStr.Length;
            StringBuilder sb = new();
            Random rand = new();
            for (int i = 0; i < btData.Length; i++)
            {
                j = (byte)rand.Next(6);
                btData[i] = (byte)((int)btData[i] ^ j);
                k = (int)btData[i] % len;
                m = (int)btData[i] / len;
                m = m * 8 + j;
                sb.Append(randStr.Substring(k, 1) + randStr.Substring(m, 1));
            }
            return sb.ToString();
        }
        #endregion

        #region  伪随机数解密 
        /// <summary>
        /// 伪随机数解密
        /// </summary>
        /// <param name="str">加密的字符串</param>
        /// <returns>解密后的字符串</returns>
        private string DecryptPwd(string str)
        {
            try
            {
                int j, k, m, n = 0;
                int len = randStr.Length;
                byte[] btData = new byte[str.Length / 2];
                for (int i = 0; i < str.Length; i += 2)
                {
                    k = randStr.IndexOf(str[i]);
                    m = randStr.IndexOf(str[i + 1]);
                    j = m / 8;
                    m -= j * 8;
                    btData[n] = (byte)(j * len + k);
                    btData[n] = (byte)((int)btData[n] ^ m);
                    n++;
                }
                return Encoding.Default.GetString(btData);
            }
            catch { return ""; }
        }
        #endregion

        #region  加密字符串(普通) 
        /// <summary>
        /// 加密字符串(普通)
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <returns>加密字符串</returns>
        public static string EnCodeString(string str)
        {
            string htext = "";
            for (int i = 0; i < str.Length; i++)
                htext += (char)(str[i] + 10 - 1 * 2);
            return htext;
        }
        #endregion

        #region  解密字符串(普通) 
        /// <summary>
        /// 解密字符串(普通)
        /// </summary>
        /// <param name="str">加密字符串</param>
        /// <returns>解密后的字符串</returns>
        public static string DeCodeString(string str)
        {
            string dtext = "";
            for (int i = 0; i < str.Length; i++)
                dtext += (char)(str[i] - 10 + 1 * 2);
            return dtext;
        }
        #endregion

        #region  异或加密、解密数字 
        /// <summary>
        /// 异或加密、解密字符串
        /// </summary>
        /// <param name="str">需要加密或解密的字符串</param>
        /// <param name="key">密匙，例：330ead0b-4951-46a7-8db8-0e2569472fd1我</param>
        /// <returns>解密或加密后的字符串</returns>
        public static string EnCodeOrDeCodeNumber(string str, string key)
        {
            //byte[] bs = Encoding.Default.GetBytes(str);
            //for (int i = 0; i < bs.Length; i++)
            //{
            //    bs[i] = (byte)(bs[i] ^ 123);
            //}
            //return Encoding.Default.GetString(bs);
            byte[] bStr = (new UnicodeEncoding()).GetBytes(str);
            byte[] bKey = (new UnicodeEncoding()).GetBytes(key);//加密锁
            //异或解密
            for (int i = 0; i < bStr.Length; i += 2)
                for (int j = 0; j < bKey.Length; j += 3)
                    bStr[i] = Convert.ToByte(bStr[i] ^ bKey[j]);
            return (new UnicodeEncoding()).GetString(bStr).TrimEnd('\0');
        }
        #endregion

        #region  ROT13弱解密或加密英文 
        /// <summary>
        /// ROT13弱解密或加密英文
        /// </summary>
        /// <param name="InputText">英文字符串</param>
        /// <returns>加密或解密后的字符串</returns>
        public static string ROT13EnCodeAndDeCode(string InputText)
        {
            char tem_Character;
            int UnicodeChar;
            string EncodedText = "";
            for (int i = 0; i < InputText.Length; i++)//遍历字符串中的所有字符
            {
                tem_Character = System.Convert.ToChar(InputText.Substring(i, 1));//获取字符串中指定的字符
                UnicodeChar = (int)tem_Character;//获取当前字符的Unicode编码
                if (UnicodeChar >= 97 && UnicodeChar <= 109)//对字符进行加密
                    UnicodeChar += 13;
                else if (UnicodeChar >= 110 && UnicodeChar <= 122)//对字符进行解密
                    UnicodeChar -= 13;
                else if (UnicodeChar >= 65 && UnicodeChar <= 77)//对字符进行加密
                    UnicodeChar += 13;
                else if (UnicodeChar >= 78 && UnicodeChar <= 90)//对字符进行解密
                    UnicodeChar -= 13;
                EncodedText += (char)UnicodeChar;//返回设置后的字符
            }
            return EncodedText;
        }
        #endregion

        #region  计算字符串MD5值 
        /// <summary>
        /// 计算字符串MD5值
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>返回哈希作为32字符的十六进制字符串</returns>
        public static string GetStringMd5(string str)
        {
            // 建立MD5对象的一个新实例.
            MD5 md5Hasher = MD5.Create();
            // 输入字符串转换为字节数组，计算哈希值。
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(str));
            // 创建一个新的StringBuilder收集字节，创建一个字符串。
            StringBuilder sBuilder = new();
            // 通过散列数据的每个字节循环
            // 并格式化每一个作为十六进制字符串
            for (int i = 0; i < data.Length; i++)
                sBuilder.Append(data[i].ToString("x2"));
            // 返回十六进制字符串.
            return sBuilder.ToString();
        }
        #endregion

        #region  计算文件MD5值 
        /// <summary>
        /// 计算文件MD5值
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>返回哈希作为32字符的十六进制字符串</returns>
        public static string GetFileMd5(string path)
        {
            try
            {
                using (FileStream get_file = new(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    // 建立MD5对象的一个新实例.
                    MD5 md5Hasher = MD5.Create();
                    // 输入字符串转换为字节数组，计算哈希值。
                    byte[] data = md5Hasher.ComputeHash(get_file);
                    // 创建一个新的StringBuilder收集字节，创建一个字符串。
                    StringBuilder sBuilder = new();
                    // 通过散列数据的每个字节循环
                    // 并格式化每一个作为十六进制字符串
                    for (int i = 0; i < data.Length; i++)
                        sBuilder.Append(data[i].ToString("x2"));
                    // 返回十六进制字符串.
                    return sBuilder.ToString();
                }
            }
            catch { return "error"; }
        }
        #endregion

        #region  计算字符串Hash值 
        /// <summary>
        /// 计算字符串Hash值
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>返回哈希作为32字符的十六进制字符串</returns>
        public static string GetStringHash(string str)
        {
            // 建立HashAlgorithm对象的一个新实例.
            HashAlgorithm algorithm = SHA1.Create();
            // 输入字符串转换为字节数组，计算哈希值。
            byte[] data = algorithm.ComputeHash(Encoding.Default.GetBytes(str));
            // 创建一个新的StringBuilder收集字节，创建一个字符串。
            StringBuilder sBuilder = new();
            // 通过散列数据的每个字节循环
            // 并格式化每一个作为十六进制字符串
            for (int i = 0; i < data.Length; i++)
                sBuilder.Append(data[i].ToString("x2"));
            // 返回十六进制字符串.
            return sBuilder.ToString();
        }
        #endregion

        #region  计算文件Hash值 
        /// <summary>
        /// 计算文件Hash值
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>返回哈希作为32字符的十六进制字符串</returns>
        public static string GetFileHash(string path)
        {
            try
            {
                if (!System.IO.File.Exists(path))
                    return string.Empty;
                using (FileStream get_file = new(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    // 建立HashAlgorithm对象的一个新实例.
                    HashAlgorithm algorithm = SHA1.Create();
                    // 输入字符串转换为字节数组，计算哈希值。
                    byte[] data = algorithm.ComputeHash(get_file);
                    // 创建一个新的StringBuilder收集字节，创建一个字符串。
                    StringBuilder sBuilder = new();
                    // 通过散列数据的每个字节循环
                    // 并格式化每一个作为十六进制字符串
                    for (int i = 0; i < data.Length; i++)
                        sBuilder.Append(data[i].ToString("x2"));
                    // 返回十六进制字符串.
                    return sBuilder.ToString();
                }
            }
            catch { return "error"; }
        }
        #endregion
    }
}
