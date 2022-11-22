using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace SuperFramework.SuperEncrypt
{
    /// <summary>
    /// 日 期:2014-09-15
    /// 作 者:不良帥
    /// 描 述:RSACryptoServiceProvider算法加密解密类
    /// </summary>
    public class RsaEncrypt
    {
        #region  获取RSC公钥和密钥，PublicKey为公钥用来加密，PrivateKey为私钥用来解密 
        /// <summary>
        /// 获取RSC公钥和密钥，PublicKey为公钥用来加密，PrivateKey为私钥用来解密
        /// </summary>
        /// <returns>Dictionary(密匙类型，密匙值)</returns>
        public static Dictionary<string, string> GetRSCKey()
        {
            Dictionary<string, string> key = new();
            RSACryptoServiceProvider rsa = new();
            string publicKey = rsa.ToXmlString(false);//公钥
            string privateKey = rsa.ToXmlString(true);//密钥
            key.Add("PublicKey", publicKey);
            key.Add("PrivateKey", privateKey);
            return key;
        }
        #endregion

        #region  RSA加密字符串 
        /// <summary>
        /// RSA加密字符串
        /// </summary>
        /// <param name="source">待加密字符串</param>
        /// <param name="xmlPublicKey">xml格式公钥</param>
        /// <returns>加密后的字符串</returns>
        public static string EncryptByRSA(string source, string xmlPublicKey)
        {
            RSACryptoServiceProvider rsa = new();
            rsa.FromXmlString(xmlPublicKey);
            byte[] done = rsa.Encrypt(Convert.FromBase64String(source), false);
            return Convert.ToBase64String(done);
        }
        #endregion

        #region  RSA解密字符串 

        /// <summary>
        /// RSA解密字符串
        /// </summary>
        /// <param name="source">待解密字符串</param>
        /// <param name="xmlPrivateKey">xml格式私钥</param>
        /// <returns>解密后的字符串</returns>
        public static string DecryptByRSA(string source, string xmlPrivateKey)
        {
            RSACryptoServiceProvider rsa = new();
            rsa.FromXmlString(xmlPrivateKey);
            byte[] done = rsa.Decrypt(Convert.FromBase64String(source), false);
            return Convert.ToBase64String(done);
        }
        #endregion

        #region  RSA加密byte[] 
        /// <summary>
        /// RSA加密byte[]
        /// </summary>
        /// <param name="source">待加密字符数组</param>
        /// <param name="xmlPublicKey">xml格式公钥</param>
        /// <returns></returns>
        public static byte[] EncryptByRSA(byte[] source, string xmlPublicKey)
        {
            RSACryptoServiceProvider rsa = new();
            rsa.FromXmlString(xmlPublicKey);
            return rsa.Encrypt(source, false);
        }
        #endregion

        #region  RSA解密byte[] 
        /// <summary>
        /// RSA解密byte[]
        /// </summary>
        /// <param name="source">待解密字符数组</param>
        /// <param name="xmlPrivateKey">xml格式私钥</param>
        /// <returns>返回解密后的数组</returns>
        public static byte[] DecryptByRSA(byte[] source, string xmlPrivateKey)
        {
            RSACryptoServiceProvider rsa = new();
            rsa.FromXmlString(xmlPrivateKey);
            return rsa.Decrypt(source, false);
        }
        #endregion

        #region  RSA加密文件 
        /// <summary>
        /// RSA加密文件
        /// </summary>
        /// <param name="inFileName">待加密文件路径</param>
        /// <param name="outFileName">加密后文件路径</param>
        /// <param name="xmlPublicKey">xml格式公钥</param>
        /// <returns>成功返回true，失败返回false</returns>
        public static bool EncryptByRSA(string inFileName, string outFileName, string xmlPublicKey)
        {
            bool state;
            try
            {
                RSACryptoServiceProvider rsa = new();
                rsa.FromXmlString(xmlPublicKey);
                FileStream fin = new(inFileName, FileMode.Open, FileAccess.Read);
                FileStream fout = new(outFileName, FileMode.OpenOrCreate, FileAccess.Write);
                fout.SetLength(0);
                byte[] bin = new byte[1000];
                long rdlen = 0;
                long totlen = fin.Length;
                int len;
                while (rdlen < totlen)
                {
                    len = fin.Read(bin, 0, 1000);
                    byte[] bout = rsa.Encrypt(bin, false);
                    fout.Write(bout, 0, bout.Length);
                    rdlen += len;
                }
                state = true;
                fout.Close();
                fin.Close();
            }
            catch(Exception) { state = false; }
            return state;
        }
        #endregion

        #region  RSA解密文件 
        /// <summary>
        /// RSA解密文件
        /// </summary>
        /// <param name="inFileName">待解密文件路径</param>
        /// <param name="outFileName">解密后文件路径</param>
        /// <param name="xmlPrivateKey">xml格式私钥</param>
        /// <returns>成功返回true，失败返回false</returns>
        public static bool DecryptbyRSA(string inFileName, string outFileName, string xmlPrivateKey)
        {
            bool state;
            try
            {
                RSACryptoServiceProvider rsa = new();
                rsa.FromXmlString(xmlPrivateKey);
                FileStream fin = new(inFileName, FileMode.Open, FileAccess.Read);
                FileStream fout = new(outFileName, FileMode.OpenOrCreate, FileAccess.Write);
                fout.SetLength(0);
                byte[] bin = new byte[1000];
                long rdlen = 0;
                long totlen = fin.Length;
                int len;
                while (rdlen < totlen)
                {
                    len = fin.Read(bin, 0, 1000);
                    byte[] bout = rsa.Decrypt(bin, false);
                    fout.Write(bout, 0, bout.Length);
                    rdlen += len;
                }
                state = true;
                fout.Close();
                fin.Close();
            }
            catch(Exception) { state = false; }
            return state;
        }
        #endregion
    }
}
