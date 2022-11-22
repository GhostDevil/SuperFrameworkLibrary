using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SuperFramework.SuperEncrypt
{

    /// <summary>
    /// 日 期:2014-09-15
    /// 作 者:不良帥
    /// 描 述:DES对称算法加密解密类
    /// </summary>
    public class DesEncrypt
    {
        #region  默认对称算法密钥向量 
        //默认密钥向量
        private static byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        #endregion

        #region  获得DSC对称算法密钥 
        /// <summary>
        /// 获得DSC对称算法密钥
        /// </summary>
        /// <returns>密钥</returns>
        private static byte[] GetDSCKey(string key)
        {
            DES mydes = DES.Create();
            string sTemp = key;
            mydes.GenerateKey();
            byte[] bytTemp = mydes.Key;
            int KeyLength = bytTemp.Length;
            if (sTemp.Length > KeyLength)
                sTemp = sTemp.Substring(0, KeyLength);
            else if (sTemp.Length < KeyLength)
                sTemp = sTemp.PadRight(KeyLength, ' ');
            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }
        #endregion

        #region  DES对称加密字符串(DES) 
        /// <summary>
        /// DES对称加密字符串(DES)
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="key">加密密钥</param>
        /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
        public static string EncryptByDES(string encryptString, string key)
        {
            try
            {
                byte[] rgbKey = GetDSCKey(key);
                byte[] rgbIV = IV;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                DES dCSP = DES.Create();
                MemoryStream mStream = new();
                CryptoStream cStream = new(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray());
            }
            catch
            {
                return encryptString;
            }
        }
        /// <summary>
        /// DES对称加密字符串(DES)
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="key">加密密钥</param>
        /// <param name="iv">加密向量</param>
        /// <returns>加密成功返回加密后的字符串，失败返回错误信息</returns>
        public static string EncryptByDES(byte[] decryptString, byte[] key, byte[] iv)
        {
            try
            {
                DES dCSP = DES.Create();
                MemoryStream mStream = new();
                CryptoStream cStream = new(mStream, dCSP.CreateEncryptor(key, iv), CryptoStreamMode.Write);
                cStream.Write(decryptString, 0, decryptString.Length);
                cStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray());
            }
            catch (Exception ex)
            {
                return "Error :" + ex.Message;
            }
        }
        #endregion

        #region  DES对称解密字符串(DES) 
        /// <summary>
        /// DES对称解密字符串(DES)
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="key">解密密钥,和加密密钥相同</param>
        /// <returns>解密成功返回解密后的字符串，失败返源串</returns>
        public static string DecryptByDES(string decryptString, string key,string iv="")
        {
            try
            {
                byte[] rgbKey = GetDSCKey(key);
                byte[] rgbIV = IV;
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DES DCSP = DES.Create();
                MemoryStream mStream = new();
                CryptoStream cStream = new(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch(Exception)
            {
                return decryptString;
            }
        }
        
        /// <summary>
        /// DES对称解密字符串(DES)
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="key">解密密钥,和加密密钥相同</param>
        /// <param name="iv">解密向量密钥,和加密向量相同</param>
        /// <returns>解密成功返回解密后的字符串，失败错误信息</returns>
        public static string DecryptByDES(byte[] decryptString, byte[] key, byte[] iv )
        {
            try
            {
                DES DCSP = DES.Create();
                MemoryStream mStream = new();
                CryptoStream cStream = new(mStream, DCSP.CreateDecryptor(key, iv), CryptoStreamMode.Write);
                cStream.Write(decryptString, 0, decryptString.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch (Exception ex)
            {
                return "Error :"+ex.Message;
            }
        }

        /// <summary>
        /// 加密字符串   
        /// </summary>  
        /// <param name="str">要加密的字符串</param>  
        /// <returns>加密后的字符串</returns>  
        public static string Encrypt(string str,string keys)
            {
                //实例化加/解密类对象   
                var descsp = DES.Create();
                //定义字节数组，用来存储密钥    
                var key = Encoding.Unicode.GetBytes(keys);
                //定义字节数组，用来存储要加密的字符串  
                var data = Encoding.Unicode.GetBytes(str);
                //实例化内存流对象      
                var mStream = new MemoryStream();

                //使用内存流实例化加密流对象   
                var cStream = new CryptoStream(mStream, descsp.CreateEncryptor(key, key), CryptoStreamMode.Write);
                //向加密流中写入数据
                cStream.Write(data, 0, data.Length);
                //释放加密流      
                cStream.FlushFinalBlock();
                //返回加密后的字符串
                return System.Convert.ToBase64String(mStream.ToArray());
            }

        /// <summary>  
        /// 解密字符串   
        /// </summary>  
        /// <param name="str">要解密的字符串</param>  
        /// <returns>解密后的字符串</returns>  
        public static string Decrypt(string str, string keys)
        {
            //实例化加/解密类对象    
            var descsp = DES.Create();
            //定义字节数组，用来存储密钥    
            var key = Encoding.Unicode.GetBytes(keys);
            //定义字节数组，用来存储要解密的字符串  
            var data = System.Convert.FromBase64String(str);
            //实例化内存流对象      
            var mStream = new MemoryStream();
            //使用内存流实例化解密流对象       
            var cStream = new CryptoStream(mStream, descsp.CreateDecryptor(key, key), CryptoStreamMode.Write);
            //向解密流中写入数据
            cStream.Write(data, 0, data.Length);
            //释放解密流
            cStream.FlushFinalBlock();
            //返回解密后的字符串 
            return Encoding.Unicode.GetString(mStream.ToArray());
        }

        #endregion

        #region  DES对称加密文件(DES) 
        /// <summary>
        /// DES对称加密文件(DES)
        /// </summary>
        /// <param name="filePath">原文件路径</param>
        /// <param name="savePath">加密后文件路径</param>
        /// <param name="key">密匙</param>
        /// <returns>成功返回true，失败返回false</returns>
        private static bool EncryptByDES(string filePath, string savePath, string key)
        {
            bool state;
            try
            {
                //创建的文件流来处理输入和输出文件.
                FileStream fin = new(filePath, FileMode.Open, FileAccess.Read);
                FileStream fout = new(savePath, FileMode.OpenOrCreate, FileAccess.Write);
                fout.SetLength(0);
                byte[] desKey = GetDSCKey(key); ;
                //创建变量，以帮助读写。
                byte[] bin = new byte[100];  //这是中间存储为加密。
                long rdlen = 0;              //这是写入的字节总数。
                long totlen = fin.Length;    //这是输入文件的总长度。
                int len;                     //这是要被写入一次的字节数。
                DES des = DES.Create();
                CryptoStream encStream = new(fout, des.CreateEncryptor(desKey, IV), CryptoStreamMode.Write);
                //读取输入文件，然后加密并写入到输出文件中。
                while (rdlen < totlen)
                {
                    len = fin.Read(bin, 0, 100);
                    encStream.Write(bin, 0, len);
                    rdlen += len;
                }
                state = true;
                encStream.Close();
                fout.Close();
                fin.Close();
            }
            catch { throw; }
            return state;
        }
        #endregion

        #region  DES对称解密文件(DES) 

        /// <summary>
        /// DES对称解密文件(DES)
        /// </summary>
        /// <param name="inName">原文件路径</param>
        /// <param name="savePath">加密后文件路径</param>
        /// <param name="key">密匙</param>
        /// <returns>成功返回true，失败返回false</returns>
        private static bool DecryptByDEC(string inName, string savePath, string key)
        {
            bool state;
            try
            {
                //创建的文件流来处理输入和输出文件.
                FileStream fin = new(inName, FileMode.Open, FileAccess.Read);
                FileStream fout = new(savePath, FileMode.OpenOrCreate, FileAccess.Write);
                fout.SetLength(0);
                byte[] desKey = GetDSCKey(key);
                //创建变量，以帮助读写。
                byte[] bin = new byte[100];  //这是中间存储为加密。
                long rdlen = 0;              //这是写入的字节总数。
                long totlen = fin.Length;    //这是输入文件的总长度。
                int len;                     //这是要被写入一次的字节数。
                DES des = DES.Create();
                CryptoStream encStream = new(fout, des.CreateDecryptor(desKey, IV), CryptoStreamMode.Write);
                //读取输入文件，然后解密并写入到输出文件中。
                while (rdlen < totlen)
                {
                    len = fin.Read(bin, 0, 100);
                    encStream.Write(bin, 0, len);
                    rdlen += len;
                }
                state = true;
                encStream.Close();
                fout.Close();
                fin.Close();
            }
            catch { state = false; }
            return state;
        }
        #endregion

        //#region  DES对称加密字符串(DES) 
        ///// <summary>
        ///// DES对称加密字符串(DES)
        ///// </summary>
        ///// <param name="str">字符串</param>
        ///// <param name="key">密匙</param>
        ///// <returns>加密后的BASE64编码的字符串</returns>
        //public static string EncryptByDES(string str, string key)
        //{
        //    try
        //    {
        //        byte[] byKey = GetDSCKey(key);
        //        byte[] byIV = IV;
        //        DES cryptoProvider =  DES.Create();
        //        int i = cryptoProvider.KeySize;
        //        MemoryStream ms = new MemoryStream();
        //        CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(byKey, byIV), CryptoStreamMode.Write);
        //        StreamWriter sw = new StreamWriter(cst);
        //        sw.Write(str);
        //        sw.Flush();
        //        cst.FlushFinalBlock();
        //        sw.Flush();
        //        return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
        //    }
        //    catch { return string.Empty; }
        //}
        //#endregion
        //#region  DES对称解密字符串(DES) 
        ///// <summary>
        ///// DES对称解密字符串(DES)
        ///// </summary>
        ///// <param name="str">待解密字符串</param>
        ///// <param name="key">密匙</param>
        ///// <returns>解密后的字符串</returns>
        //public static string DecryptByDES(string str,string key)
        //{
        //    try
        //    {
        //        byte[] byKey = GetDSCKey(key);
        //        byte[] byIV = IV;
        //        byte[] byEnc;
        //        try
        //        {
        //            byEnc = Convert.FromBase64String(str);
        //        }
        //        catch
        //        {
        //            return null;
        //        }
        //        DES cryptoProvider = new DES();
        //        MemoryStream ms = new MemoryStream(byEnc);
        //        CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(byKey, byIV), CryptoStreamMode.Read);
        //        StreamReader sr = new StreamReader(cst);
        //        return sr.ReadToEnd();
        //    }
        //    catch { return string.Empty; }
        //}
        //#endregion
        #region  DES对称加密文件(DES,MD5) 
        /// <summary>
        /// DES对称加密文件 (DES,MD5)
        /// </summary>
        /// <param name="filePath">待加密文件路径</param>
        /// <param name="savePath">输出加密文件路径</param>
        /// <param name="password">密码</param>
        /// <returns>成功返回true，失败返回false</returns>
        public static bool EncryptByDESAndMD5(string filePath, string savePath, string password)
        {
            bool state = false;
            try
            {
                if (File.Exists(filePath) && savePath != "")
                {
                    if (File.Exists(savePath))
                        return false;
                    DES des =  DES.Create();
                    FileStream fs = File.OpenRead(filePath);
                    byte[] inputByteArray = new byte[fs.Length];
                    fs.Read(inputByteArray, 0, (int)fs.Length);
                    fs.Close();
                    byte[] mypasswordbyte = Encoding.Default.GetBytes(password);
                    MD5 get_md5 =  MD5.Create();
                    byte[] hash_byte = get_md5.ComputeHash(mypasswordbyte);
                    string resule = System.BitConverter.ToString(hash_byte);
                    resule = resule.Replace("-", "");
                    byte[] keyByteArray = Encoding.Default.GetBytes(resule.Substring(0, 8));
                    des.Key = keyByteArray;
                    des.IV = keyByteArray;
                    MemoryStream ms = new();
                    CryptoStream cs = new(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    fs = File.OpenWrite(savePath);
                    foreach (byte b in ms.ToArray())
                        fs.WriteByte(b);
                    state = true;
                    fs.Close();
                    cs.Close();
                    ms.Close();
                }
            }
            catch { state = false; }
            return state;
        }
        #endregion

        #region  DES对称解密文件(DES,MD5) 
        /// <summary>
        /// DES对称解密文件 (DES,MD5)
        /// </summary>
        /// <param name="filePath">待解密文件路径</param>
        /// <param name="savePath">输出文件路径</param>
        /// <param name="password">密码</param>
        /// <returns>成功返回true，失败返回false</returns>
        public static bool DecryptByDESAndMD5(string filePath, string savePath, string password)
        {
            bool state = false;
            try
            {
                if (File.Exists(filePath) && savePath != "")
                {
                    byte[] info;
                    FileInfo MyFileInfo = new(filePath);
                    StreamReader mp3info = new(filePath);
                    info = new byte[MyFileInfo.Length];
                    info = File.ReadAllBytes(filePath);
                    byte[] newjiamifile = new byte[MyFileInfo.Length - 1000];
                    if (File.Exists(savePath))
                        return false;
                    DES des =  DES.Create();
                    FileStream fs = File.OpenRead(filePath);
                    byte[] inputByteArray = new byte[fs.Length];
                    fs.Read(inputByteArray, 0, (int)fs.Length);
                    fs.Close();
                    byte[] mypasswordbyte = Encoding.Default.GetBytes(password);
                    MD5 get_md5 =  MD5.Create();
                    byte[] hash_byte = get_md5.ComputeHash(mypasswordbyte);
                    string resule = System.BitConverter.ToString(hash_byte);
                    resule = resule.Replace("-", "");
                    byte[] keyByteArray = Encoding.Default.GetBytes(resule.Substring(0, 8));
                    des.Key = keyByteArray;
                    des.IV = keyByteArray;
                    MemoryStream ms = new();
                    CryptoStream cs = new(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    fs = File.OpenWrite(savePath);
                    foreach (byte b in ms.ToArray())
                        fs.WriteByte(b);
                    state = true;
                    fs.Close();
                    cs.Close();
                    ms.Close();
                }
            }
            catch
            {
                state = false;
            }
            return state;
        }
        #endregion

        #region  DES对称加密byte[](DES) 
        /// <summary>
        /// DES对称加密byte[](DES)
        /// </summary>
        /// <param name="Source">待加密的byte数组</param>
        /// <param name="key">密匙</param>
        /// <returns>经过加密的byte数组</returns>
        public static byte[] EncryptByDES(byte[] Source, string key)
        {
            try
            {
                byte[] bytIn = Source;
                MemoryStream ms = new();
                byte[] byKey = GetDSCKey(key);
                byte[] byIV = IV;
                DES des = DES.Create();
                des.Key = byKey;
                des.IV = byIV;
                ICryptoTransform encrypto = des.CreateEncryptor();
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

        #region  DES对称解密byte[](DES) 
        /// <summary>
        /// DES对称解密byte[](DES)
        /// </summary>
        /// <param name="Source">待解密的byte数组</param>
        /// <param name="key">密匙</param>
        /// <returns>经过解密的byte数组</returns>
        public static byte[] DecryptByDES(byte[] Source, string key)
        {
            try
            {
                byte[] bytIn = Source;
                MemoryStream ms = new(bytIn, 0, bytIn.Length);
                byte[] byKey = GetDSCKey(key);
                byte[] byIV = IV;
                DES des = DES.Create();
                des.Key = byKey;
                des.IV = byIV;
                ICryptoTransform encrypto = des.CreateDecryptor();
                CryptoStream cs = new(ms, encrypto, CryptoStreamMode.Read);
                StreamReader sr = new(cs);
                return UTF8Encoding.UTF8.GetBytes(sr.ReadToEnd());
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion
    }
}
