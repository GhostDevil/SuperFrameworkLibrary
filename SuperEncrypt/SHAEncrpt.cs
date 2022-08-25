using System;
using System.Security.Cryptography;

namespace SuperFramework.SuperEncrypt
{
    /// <summary>
    /// SHA加密 - 盐
    /// </summary>
    public static class SHAEncrpt
    {
        /// <summary>
        /// 加密SHA256
        /// </summary>
        /// <param name="Text">加密数据</param>
        /// <returns>string[0]:加密数据 string[1]:加密密钥</returns>
        public static string[] EncryptSHA256(string Text)
        {
            try
            {
                string GUIDKEY = Guid.NewGuid().ToString();
                byte[] hashBytes = SHA256.Create().ComputeHash(System.Text.Encoding.UTF8.GetBytes(Text + GUIDKEY));
                return new string[] { Convert.ToBase64String(hashBytes), GUIDKEY };
            }
            catch
            {
                throw new Exception("Encrypt Error!");
            }
        }

        /// <summary>
        /// 加密SHA1
        /// </summary>
        /// <param name="Text">加密数据</param>
        /// <returns>string[0]:加密数据 string[1]:加密密钥</returns>
        public static string[] EncryptSHA1(string Text)
        {
            try
            {
                string GUIDKEY = Guid.NewGuid().ToString();
                byte[] hashBytes = SHA1.Create().ComputeHash(System.Text.Encoding.UTF8.GetBytes(Text + GUIDKEY));
                return new string[] { Convert.ToBase64String(hashBytes), GUIDKEY };
            }
            catch
            {
                throw new Exception("Encrypt Error!");
            }
        }

        #region 加盐(Salt)加密,文件相同加密后文本却不同
        private const int saltLength = 8;
        /// <summary>  
        /// 对密码进行Hash 和 Salt  
        /// </summary>  
        /// <param name="Password">用户输入的密码</param>  
        /// <returns>string[0]:加密数据 string[1]:加密密钥</returns>
        public static string[] EncryptAndSalt(string Text)
        {
            try
            {
                string GUIDKEY = Guid.NewGuid().ToString();
                byte[] hashTextBytes = SHA1.Create().ComputeHash(System.Text.Encoding.UTF8.GetBytes(Text + GUIDKEY));

                byte[] saltValue = new byte[saltLength];//随机盐(Salt)混淆数据
                RandomNumberGenerator.Create().GetBytes(saltValue);
                return new string[] { Convert.ToBase64String(CreateSaltedText(saltValue, hashTextBytes)), GUIDKEY };
            }
            catch
            {
                throw new Exception("Encrypt Error!");
            }
        }

        /// <summary>
        /// 加盐算法(私有)
        /// </summary>
        /// <param name="saltValue">盐</param>
        /// <param name="unsaltedText">加密字符</param>
        /// <returns>byte[]</returns>  
        private static byte[] CreateSaltedText(byte[] saltValue, byte[] unsaltedText)
        {
            try
            {
                byte[] rawSalted = new byte[unsaltedText.Length + saltValue.Length];//加盐
                saltValue.CopyTo(rawSalted, 0);
                unsaltedText.CopyTo(rawSalted, saltValue.Length);

                byte[] saltedText = SHA1.Create().ComputeHash(rawSalted);

                byte[] dbText = new byte[saltedText.Length + saltValue.Length];//保存盐
                saltValue.CopyTo(dbText, 0);
                saltedText.CopyTo(dbText, saltValue.Length);

                return dbText;
            }
            catch
            {
                throw new Exception("Encrypt Error!");
            }
        }

        /// <summary>
        /// 对比盐字符串
        /// </summary>
        /// <param name="OldText">保存的字符</param>
        /// <param name="Text">字符</param>
        /// <returns>true：相等 false：不等</returns>  
        public static string CompareSaltText(string dbText, string Text, string GUIDKEY)
        {
            try
            {
                byte[] TextBytes = SHA1.Create().ComputeHash(System.Text.Encoding.UTF8.GetBytes(Text + GUIDKEY));
                byte[] dbTextBytes = Convert.FromBase64String(dbText);

                byte[] saltValue = new byte[saltLength];//盐(Salt)获取
                for (int i = 0; i < saltLength; i++)
                {
                    saltValue[i] = dbTextBytes[i];
                }
                return Convert.ToBase64String(CreateSaltedText(saltValue, TextBytes));
            }
            catch
            {
                throw new Exception("Encrypt Error!");
            }
        }
        #endregion

    }

}
