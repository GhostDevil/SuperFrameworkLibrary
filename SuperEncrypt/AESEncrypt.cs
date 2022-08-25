using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SuperFramework.SuperEncrypt
{
    /// <summary>
    /// 日 期:2014-09-15
    /// 作 者:不良帥
    /// 描 述:高级加密标准 （AES） 加密解密类
    /// </summary>
    public class AESEncrypt
    {
        #region  默认对称算法密钥向量 
        //默认密钥向量
        private static byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        #endregion

        #region  获得Rijndael对称算法密钥 
        /// <summary>
        /// 获得Rijndael对称算法密钥
        /// </summary>
        /// <param name="key">密匙</param>
        /// <returns>对称算法密钥</returns>
        private static byte[] GetRijndaelKey(string key)
        {
            Aes myRijndael = Aes.Create();
            string sTemp = key;
            myRijndael.GenerateKey();
            byte[] bytTemp = myRijndael.Key;
            int KeyLength = bytTemp.Length;
            if (sTemp.Length > KeyLength)
                sTemp = sTemp.Substring(0, KeyLength);
            else if (sTemp.Length < KeyLength)
                sTemp = sTemp.PadRight(KeyLength, ' ');
            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }
        #endregion

        #region  Rijndael对称加密字符串(Aes) 
        /// <summary>
        /// Rijndael对称加密字符串(Aes)
        /// </summary>
        /// <param name="source">待加密的字符串</param>
        /// <param name="key">密匙</param>
        /// <returns>经过加密的字符串</returns>
        public static string EncryptByRijndael(string source, string key)
        {
            try
            {
                Aes myRijndael = Aes.Create();
                byte[] bytIn = UTF8Encoding.UTF8.GetBytes(source);
                MemoryStream ms = new MemoryStream();
                myRijndael.Key = GetRijndaelKey(key);
                myRijndael.IV = IV;
                ICryptoTransform encrypto = myRijndael.CreateEncryptor();
                CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);
                cs.Write(bytIn, 0, bytIn.Length);
                cs.FlushFinalBlock();
                ms.Close();
                byte[] bytOut = ms.ToArray();
                return Convert.ToBase64String(bytOut);
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region  Rijndael对称解密字符串(Aes) 

        /// <summary>
        /// Rijndael对称解密字符串(Aes)
        /// </summary>
        /// <param name="source">待解密的串</param>
        /// <param name="key">密匙</param>
        /// <returns>经过解密的串</returns>
        public static string DecryptByRijndael(string source, string key)
        {
            try
            {
                Aes myRijndael = Aes.Create();
                byte[] bytIn = Convert.FromBase64String(source);
                MemoryStream ms = new MemoryStream(bytIn, 0, bytIn.Length);
                myRijndael.Key = GetRijndaelKey(key);
                myRijndael.IV = IV;
                ICryptoTransform encrypto = myRijndael.CreateDecryptor();
                CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read);
                StreamReader sr = new StreamReader(cs);
                return sr.ReadToEnd();
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region  Rijndael对称加密byte[](Aes) 
        /// <summary>
        /// Rijndael对称加密byte[](Aes)
        /// </summary>
        /// <param name="source">待加密的byte数组</param>
        /// <param name="key">密匙</param>
        /// <returns>经过加密的byte数组</returns>
        public static byte[] EncryptByRijndael(byte[] source, string key)
        {
            try
            {
                Aes myRijndael = Aes.Create();
                byte[] bytIn = source;
                MemoryStream ms = new MemoryStream();
                myRijndael.Key = GetRijndaelKey(key);
                myRijndael.IV = IV;
                ICryptoTransform encrypto = myRijndael.CreateEncryptor();
                CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);
                cs.Write(bytIn, 0, bytIn.Length);
                cs.FlushFinalBlock();
                ms.Close();
                byte[] bytOut = ms.ToArray();
                return bytOut;
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region  Rijndael对称解密byte[](Aes) 
        /// <summary>
        /// Rijndael对称解密byte[](Aes)
        /// </summary>
        /// <param name="source">待解密的byte数组</param>
        /// <param name="key">密匙</param>
        /// <returns>经过解密的byte数组</returns>
        public static byte[] DecryptByRijndael(byte[] source, string key)
        {
            try
            {
                Aes myRijndael = Aes.Create();
                byte[] bytIn = source;
                MemoryStream ms = new MemoryStream(bytIn, 0, bytIn.Length);
                myRijndael.Key = GetRijndaelKey(key);
                myRijndael.IV = IV;
                ICryptoTransform encrypto = myRijndael.CreateDecryptor();
                CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read);
                StreamReader sr = new StreamReader(cs);
                return UTF8Encoding.UTF8.GetBytes(sr.ReadToEnd());
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region  Rijndael对称加密文件(Aes) 
        /// <summary>
        /// Rijndael对称加密文件(Aes)
        /// </summary>
        /// <param name="inFileName">待加密文件的路径</param>
        /// <param name="outFileName">待加密后文件的输出路径</param>
        /// <param name="key">密匙</param>
        /// <returns>成功返回true，失败返回false</returns>

        public static bool EncryptByRijndael(string inFileName, string outFileName, string key)
        {
            bool state;
            try
            {
                Aes myRijndael = Aes.Create();
                FileStream fin = new FileStream(inFileName, FileMode.Open, FileAccess.Read);
                FileStream fout = new FileStream(outFileName, FileMode.OpenOrCreate, FileAccess.Write);
                fout.SetLength(0);
                myRijndael.Key = GetRijndaelKey(key);
                myRijndael.IV = IV;
                byte[] bin = new byte[100];
                long rdlen = 0;
                long totlen = fin.Length;
                int len;
                ICryptoTransform encrypto = myRijndael.CreateEncryptor();
                CryptoStream cs = new CryptoStream(fout, encrypto, CryptoStreamMode.Write);
                while (rdlen < totlen)
                {
                    len = fin.Read(bin, 0, 100);
                    cs.Write(bin, 0, len);
                    rdlen += len;
                }
                state = true;
                cs.Close();
                fout.Close();
                fin.Close();
            }
            catch(Exception)
            {
                state = false;
            }
            return state;
        }
        #endregion

        #region  Rijndael对称解密文件(Aes) 
        /// <summary>
        /// Rijndael对称解密文件
        /// </summary>
        /// <param name="inFileName">待解密文件的路径</param>
        /// <param name="outFileName">待解密后文件的输出路径</param>
        /// <param name="key">密匙</param>
        /// <returns>成功返回true，失败返回false</returns>
        public static bool DecryptByRijndael(string inFileName, string outFileName, string key)
        {
            bool state;
            try
            {
                Aes myRijndael = Aes.Create();
                FileStream fin = new FileStream(inFileName, FileMode.Open, FileAccess.Read);
                FileStream fout = new FileStream(outFileName, FileMode.OpenOrCreate, FileAccess.Write);
                fout.SetLength(0);
                byte[] bin = new byte[100];
                long rdlen = 0;
                long totlen = fin.Length;
                int len;
                myRijndael.Key = GetRijndaelKey(key);
                myRijndael.IV = IV;
                ICryptoTransform encrypto = myRijndael.CreateDecryptor();
                CryptoStream cs = new CryptoStream(fout, encrypto, CryptoStreamMode.Write);
                while (rdlen < totlen)
                {
                    len = fin.Read(bin, 0, 100);
                    cs.Write(bin, 0, len);
                    rdlen += len;
                }
                state = true;
                cs.Close();
                fout.Close();
                fin.Close();
            }
            catch(Exception)
            {
                state = false;
            }
            return state;
        }
        #endregion
    }
}
