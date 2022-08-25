using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using System;
using System.Text;

namespace SuperFramework.SuperEncrypt
{
    /// <summary>
    /// 消息认证码算法（英文：Message Authentication Codes，缩写为MAC） 含有密钥的散列函数算法，兼容了MD和SHA算法的特性，并在此基础上加上了密钥。因此MAC算法也经常被称作HMAC算法。消息的散列值由只有通信双方知道的密钥来控制。此时Hash值称作MAC。
    /// HMAC是密钥相关的散列运算消息认证码（Hash-based Message Authentication Code）的缩写，由H.Krawezyk，M.Bellare，R.Canetti于1996年提出的一种基于Hash函数和密钥进行消息认证的方法，并于1997年作为RFC2104被公布，并在IPSec和其他网络协议（如SSL）中得以广泛应用，现在已经成为事实上的Internet安全标准。它可以与任何迭代散列函数捆绑使用。
    /// </summary>
    public class HMAC
    {
        public class Algorithms
        {
            public const string HMacSHA1 = "HMAC-SHA1";
            public const string HMacMD5 = "HMAC-MD5";
            public const string HMacMD4 = "HMAC-MD4";
            public const string HMacMD2 = "HMAC-MD2";
            public const string HMacSHA224 = "HMAC-SHA224";
            public const string HMacSHA256 = "HMAC-SHA256";
            public const string HMacSHA384 = "HMAC-SHA384";
            public const string HMacSHA512_224 = "HMAC-SHA512/224";
            public const string HMacSHA512_256 = "HMAC-SHA512/256";
            public const string HMacRIPEMD128 = "HMAC-RIPEMD128";
            public const string HMacRIPEMD160 = "HMAC-RIPEMD160";
            public const string HMacTIGER = "HMAC-TIGER";
            public const string HMacKECCAK224 = "HMAC-KECCAK224";
            public const string HMacKECCAK256 = "HMAC-KECCAK256";
            public const string HMacKECCAK288 = "HMAC-KECCAK288";
            public const string HMacKECCAK384 = "HMAC-KECCAK384";
            public const string HMacSHA3512 = "HMAC-SHA3-512";
            public const string HMacGOST3411_2012256 = "HMAC-GOST3411-2012-256";
            public const string HMacGOST3411_2012_512 = "HMAC-GOST3411-2012-512";
        }

        /// <summary>
        /// 生成密钥KEY
        /// </summary>
        /// <param name="algorithm">密文算法，参考Algorithms.cs中提供的HMac algorithm</param>
        /// <returns>密钥KEY</returns>
        public static byte[] GeneratorKey(string algorithm)
        {
            var kGen = GeneratorUtilities.GetKeyGenerator(algorithm);
            return kGen.GenerateKey();
        }

        /// <summary>
        /// 哈希计算
        /// </summary>
        /// <param name="data">输入字符串</param>
        /// <param name="key">密钥KEY</param>
        /// <param name="algorithm">密文算法，参考Algorithms.cs中提供的HMac algorithm</param>
        /// <returns>哈希值</returns>
        public static byte[] Compute(string data, byte[] key, string algorithm)
        {
            if (string.IsNullOrEmpty(data))
            {
                throw new ArgumentNullException(nameof(data));
            }

            var keyParameter = new Org.BouncyCastle.Crypto.Parameters.KeyParameter(key);
            var input = Encoding.UTF8.GetBytes(data);
            var mac = MacUtilities.GetMac(algorithm);
            mac.Init(keyParameter);
            mac.BlockUpdate(input, 0, input.Length);
            return MacUtilities.DoFinal(mac);
        }

        /// <summary>
        /// 哈希计算
        /// </summary>
        /// <param name="data">输入字符串</param>
        /// <param name="key">密钥KEY</param>
        /// <param name="digest"></param>
        /// <returns>哈希值</returns>
        public static byte[] Compute(string data, byte[] key, IDigest digest)
        {
            if (string.IsNullOrEmpty(data))
            {
                throw new ArgumentNullException(nameof(data));
            }

            var keyParameter = new Org.BouncyCastle.Crypto.Parameters.KeyParameter(key);
            var input = Encoding.UTF8.GetBytes(data);
            IMac mac = new Org.BouncyCastle.Crypto.Macs.HMac(digest);
            mac.Init(keyParameter);
            mac.BlockUpdate(input, 0, input.Length);
            return MacUtilities.DoFinal(mac);
        }
        //public class HMACSHA256
        //{
        //    /// <summary>
        //    /// 生成签名
        //    /// </summary>
        //    public static byte[] GeneratorKey()
        //    {
        //        return HMAC.GeneratorKey(Algorithms.HMacSHA256);
        //    }

        //    /// <summary>
        //    /// 哈希计算（使用BouncyCastle）ke
        //    /// </summary>
        //    public static byte[] Compute(string data, byte[] key)
        //    {

        //        return HMAC.Compute(data, key, Algorithms.HMacSHA256);

        //        //or
        //        //return HMAC.Compute(data, key, new Sha256Digest());
        //    }

        //    /// <summary>
        //    /// 哈希计算（不使用BouncyCastle）
        //    /// </summary>
        //    public static byte[] Compute2(string data, byte[] key)
        //    {
        //        if (string.IsNullOrEmpty(data))
        //        {
        //            throw new ArgumentNullException(nameof(data));
        //        }

        //        using (var hmacSha256 = new System.Security.Cryptography.HMACSHA256(key))
        //        {
        //            return hmacSha256.ComputeHash(Encoding.UTF8.GetBytes(data));
        //        }
        //    }
        //}
        //public class HMACSHA1
        //{
        //    /// <summary>
        //    /// 生成密钥KEY
        //    /// </summary>
        //    /// <returns></returns>
        //    public static byte[] GeneratorKey()
        //    {
        //        return HMAC.GeneratorKey(Algorithms.HMacSHA1);
        //    }

        //    /// <summary>
        //    /// 哈希计算（使用BouncyCastle）
        //    /// </summary>
        //    public static byte[] Compute(string data, byte[] key)
        //    {
        //        return HMAC.Compute(data, key, Algorithms.HMacSHA1);
        //        //or
        //        //return HMAC.Compute(data, key, new Sha1Digest());
        //    }

        //    /// <summary>
        //    /// 哈希计算（不使用BouncyCastle）
        //    /// </summary>
        //    public static byte[] Compute2(string data, byte[] key)
        //    {
        //        if (string.IsNullOrEmpty(data))
        //        {
        //            throw new ArgumentNullException(nameof(data));
        //        }

        //        using (var hmacSha1 = new System.Security.Cryptography.HMACSHA1(key))
        //        {
        //            return hmacSha1.ComputeHash(Encoding.UTF8.GetBytes(data));
        //        }
        //    }
        //}
        //public class HMACMD5
        //{
        //    /// <summary>
        //    /// 生成密钥KEY
        //    /// </summary>
        //    public static byte[] GeneratorKey()
        //    {
        //        return HMAC.GeneratorKey(Algorithms.HMacMD5);
        //    }

        //    /// <summary>
        //    /// 哈希计算（使用BouncyCastle）
        //    /// </summary>
        //    public static byte[] Compute(string data, byte[] key)
        //    {
        //        return HMAC.Compute(data, key, Algorithms.HMacMD5);
        //        //or
        //        //return HMAC.Compute(data, key, new MD5Digest());
        //    }

        //    /// <summary>
        //    /// 哈希计算（不使用BouncyCastle）
        //    /// </summary>
        //    public static byte[] Compute2(string data, string key)
        //    {
        //        if (string.IsNullOrEmpty(data))
        //        {
        //            throw new ArgumentNullException(nameof(data));
        //        }

        //        if (string.IsNullOrEmpty(key))
        //        {
        //            throw new ArgumentNullException(nameof(key));
        //        }

        //        using (var hmacMd5 = new System.Security.Cryptography.HMACMD5(Encoding.UTF8.GetBytes(key)))
        //        {
        //            return hmacMd5.ComputeHash(Encoding.UTF8.GetBytes(data));
        //        }
        //    }
        //}
    }
}
