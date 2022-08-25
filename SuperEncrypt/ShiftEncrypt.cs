using System;
using System.Text;

namespace SuperFramework.SuperEncrypt
{
    /// <summary>
    /// <para>日 期:2014-09-15</para>
    /// <para>作 者:不良帥</para>
    /// <para>描 述:位移加密算法加密解密类</para>
    /// </summary>
    public class ShiftEncrypt
    {
        #region  位移和异或组合加密，数字密匙 
        /// <summary>
        /// 位移和异或组合加密，数字密匙
        /// </summary>
        /// <param name="str">加密字符串</param>
        /// <param name="key">数字密匙 范围0-999999999</param>
        /// <returns>加密后的字符串</returns>
        public static string EncryptStr(string str, int key)
        {
            char[] array = str.ToCharArray();
            int num = 2031709749 + key;
            int i = 0;
            while (i < array.Length)
            {
                byte b1 = (byte)((int)(array[i] & 'ÿ') ^ num++);//‘ÿ’=0xFF，这里是取低8位进行异或运算。
                byte b2 = (byte)((int)((int)array[i] >> 8) ^ num++);//转int，右移8位，即取高8位进行运算。
                (b1, b2) = (b2, b1);
                //交换高低位
                array[i] = (char)((int)b2 << 8 | b1);//组合高低位
                i++;
            }
            return string.Intern(new string(array));
        }
        #endregion

        #region  位移和异或组合解密，数字密匙 
        /// <summary>
        /// 位移和异或组合解密，数字密匙
        /// </summary>
        /// <param name="str"></param>
        /// <param name="key">加密数字密匙</param>
        /// <returns>解密后的字符串</returns>
        public static string DecryptStr(string str, int key)
        {
            char[] array = str.ToCharArray();
            int num = 2031709749 + key;
            int i = 0;
            while (i < array.Length)
            {
                byte b1 = (byte)(int)(array[i] & 'ÿ');//取出低8位
                byte b2 = (byte)(int)((int)array[i] >> 8);//取出高8位
                (b1, b2) = (b2, b1);
                //交换高低位
                b1 = (byte)(b1 ^ num++);//低8位先进行异或
                b2 = (byte)(b2 ^ num++);//高8位进行异或
                array[i] = (char)((int)b2 << 8 | b1);//组合高低位
                i++;
            }
            return string.Intern(new string(array));
        }
        #endregion

        #region  位移和异或组合加密，组合密匙 
        /// <summary>
        /// 位移和异或组合加密，组合密匙
        /// </summary>
        /// <param name="characters">要加密的明文</param>
        /// <param name="key">密匙，例：330ead0b-4951-46a7-8db8-0e2569472fd1我</param>
        /// <returns>加密后的密文</returns>
        public static string EncryptStr(string characters, string key)
        {
            //string key = "330ead0b-4951-46a7-8db8-0e2569472fd1";
            //位移加密
            byte[] bStr = new UnicodeEncoding().GetBytes(characters);
            for (int i = 0; i < bStr.Length; i++)
            {
                byte b = (byte)(bStr[i] + 255);
                bStr[i] = b;
            }
            byte[] bKey = new UnicodeEncoding().GetBytes(key);//加密锁
            //异或加密
            for (int i = 0; i < bStr.Length; i += 2)
                for (int j = 0; j < bKey.Length; j += 3)
                    bStr[i] = Convert.ToByte(bStr[i] ^ bKey[j]);
            return new UnicodeEncoding().GetString(bStr).TrimEnd('\0');
        }
        #endregion

        #region  位移和异或组合解密，组合密匙 
        /// <summary>
        /// 位移和异或组合解密，组合密匙
        /// </summary>
        /// <param name="ciphertext">要解密的密文</param>
        /// <param name="key">加密密匙</param>
        /// <returns>解密后的明文</returns>
        public static string DecryptStr(string ciphertext, string key)
        {
            //string key = "330ead0b-4951-46a7-8db8-0e2569472fd1";
            byte[] bStr = new UnicodeEncoding().GetBytes(ciphertext);
            byte[] bKey = new UnicodeEncoding().GetBytes(key);//加密锁
            //异或解密
            for (int i = 0; i < bStr.Length; i += 2)
                for (int j = 0; j < bKey.Length; j += 3)
                    bStr[i] = Convert.ToByte(bStr[i] ^ bKey[j]);
            //位移解密
            for (int i = 0; i < bStr.Length; i++)
            {
                byte b = (byte)(bStr[i] - 255);
                bStr[i] = b;
            }
            return new UnicodeEncoding().GetString(bStr).TrimEnd('\0');
        }

        #endregion
    }
}
