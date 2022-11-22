using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SuperFramework.SuperEncrypt
{
    /// <summary>
    /// 日 期:2014-09-15
    /// 作 者:不良帥
    /// 描 述:Rc2对称算法加密解密类
    /// </summary>
    public class Rc2Encrypt
    {
        #region  默认对称算法密钥向量 
        //默认密钥向量
        private static byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        #endregion

        #region  获得RC2对称算法密钥 
        /// <summary>
        /// 获得RC2对称算法密钥
        /// </summary>
        /// <param name="key">密匙</param>
        /// <returns>对称算法密钥</returns>
        private static byte[] GetRC2Key(string key)
        {
            RC2 rc = RC2.Create();
            string sTemp = key;
            rc.GenerateKey();
            byte[] bytTemp = rc.Key;
            int KeyLength = bytTemp.Length;
            if (sTemp.Length > KeyLength)
                sTemp = sTemp.Substring(0, KeyLength);
            else if (sTemp.Length < KeyLength)
                sTemp = sTemp.PadRight(KeyLength, ' ');
            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }
        #endregion

        #region  RC2对称加密字符串 
        /// <summary>
        /// RC2对称加密字符串
        /// </summary>
        /// <param name="source">待加密的字符串</param>
        /// <param name="key">密匙</param>
        /// <returns>经过加密的字符串</returns>
        public static string EncryptByRC2(string source, string key)
        {
            try
            {
                RC2 rc = RC2.Create();
                byte[] bytIn = UTF8Encoding.UTF8.GetBytes(source);
                MemoryStream ms = new();
                rc.Key = GetRC2Key(key);
                rc.IV = IV;
                ICryptoTransform encrypto = rc.CreateEncryptor();
                CryptoStream cs = new(ms, encrypto, CryptoStreamMode.Write);
                cs.Write(bytIn, 0, bytIn.Length);
                cs.FlushFinalBlock();
                ms.Close();
                byte[] bytOut = ms.ToArray();
                return Convert.ToBase64String(bytOut);
            }
            catch(Exception)
            {
                throw;
            }
        }
        #endregion

        #region  RC2对称解密字符串 
        /// <summary>
        /// RC2对称解密字符串
        /// </summary>
        /// <param name="source">待解密的字符串</param>
        /// <param name="key">密匙</param>
        /// <returns>经过解密的字符串</returns>
        public static string DecryptByRC2(string source, string key)
        {
            try
            {
                RC2 rc = RC2.Create();
                byte[] bytIn = Convert.FromBase64String(source);
                MemoryStream ms = new(bytIn, 0, bytIn.Length);
                rc.Key = GetRC2Key(key);
                rc.IV = IV;
                ICryptoTransform encrypto = rc.CreateDecryptor();
                CryptoStream cs = new(ms, encrypto, CryptoStreamMode.Read);
                StreamReader sr = new(cs);
                return sr.ReadToEnd();
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region  RC2对称加密byte[] 
        /// <summary>
        /// RC2对称加密byte[]
        /// </summary>
        /// <param name="source">待加密的byte数组</param>
        /// <param name="key">密匙</param>
        /// <returns>经过加密的byte数组</returns>
        public static byte[] EncryptByRC2(byte[] source, string key)
        {
            try
            {
                RC2 rc = RC2.Create();
                byte[] bytIn = source;
                MemoryStream ms = new();
                rc.Key = GetRC2Key(key);
                rc.IV = IV;
                ICryptoTransform encrypto = rc.CreateEncryptor();
                CryptoStream cs = new(ms, encrypto, CryptoStreamMode.Write);
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

        #region  RC2对称解密byte[] 
        /// <summary>
        /// RC2对称解密byte[]
        /// </summary>
        /// <param name="source">待解密的byte数组</param>
        /// <param name="key">密匙</param>
        /// <returns>经过解密的byte数组</returns>
        public static byte[] DecryptByRC2(byte[] source, string key)
        {
            try
            {
                RC2 rc = RC2.Create();
                byte[] bytIn = source;
                MemoryStream ms = new(bytIn, 0, bytIn.Length);
                rc.Key = GetRC2Key(key);
                rc.IV = IV;
                ICryptoTransform encrypto = rc.CreateDecryptor();
                CryptoStream cs = new(ms, encrypto, CryptoStreamMode.Read);
                StreamReader sr = new(cs);
                return UTF8Encoding.UTF8.GetBytes(sr.ReadToEnd());
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region  RC2对称加密文件 
        /// <summary>
        /// RC2对称加密文件
        /// </summary>
        /// <param name="inFileName">待加密文件的路径</param>
        /// <param name="outFileName">待加密后文件的输出路径</param>
        /// <param name="key">密匙</param>
        /// <returns>成功返回true，失败返回false</returns>

        public static bool EncryptByRC2(string inFileName, string outFileName, string key)
        {
            bool state;
            try
            {
                RC2 rc = RC2.Create();
                FileStream fin = new(inFileName, FileMode.Open, FileAccess.Read);
                FileStream fout = new(outFileName, FileMode.OpenOrCreate, FileAccess.Write);
                fout.SetLength(0);

                rc.Key = GetRC2Key(key); ;
                rc.IV = IV;

                byte[] bin = new byte[100];
                long rdlen = 0;
                long totlen = fin.Length;
                int len;

                ICryptoTransform encrypto = rc.CreateEncryptor();
                CryptoStream cs = new(fout, encrypto, CryptoStreamMode.Write);
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
            catch
            {
                state = false;
            }
            return state;
        }
        #endregion

        #region  RC2对称解密文件 
        /// <summary>
        /// RC2对称解密文件
        /// </summary>
        /// <param name="inFileName">待解密文件的路径</param>
        /// <param name="outFileName">待解密后文件的输出路径</param>
        /// <param name="key">密匙</param>
        /// <returns>成功返回true，失败返回false</returns>
        public static bool DecryptByRC2(string inFileName, string outFileName, string key)
        {
            bool state;
            try
            {
                RC2 rc = RC2.Create();
                FileStream fin = new(inFileName, FileMode.Open, FileAccess.Read);
                FileStream fout = new(outFileName, FileMode.OpenOrCreate, FileAccess.Write);
                fout.SetLength(0);

                byte[] bin = new byte[100];
                long rdlen = 0;
                long totlen = fin.Length;
                int len;
                rc.Key = GetRC2Key(key);
                rc.IV = IV;
                ICryptoTransform encrypto = rc.CreateDecryptor();
                CryptoStream cs = new(fout, encrypto, CryptoStreamMode.Write);
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
            catch
            {
                state = false;
            }
            return state;
        }
        #endregion
    }
}
