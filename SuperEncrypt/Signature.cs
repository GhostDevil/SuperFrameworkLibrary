using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Security.Cryptography;
using System.Text;

namespace SuperFramework.SuperEncrypt
{
    /// <summary>
    /// 数字签名算法
    /// </summary>
    public class Signature
    {
        /// <summary>
        /// 生成签名
        /// </summary>
        public static string GenerateSignature(string data, RSAParameters privateKey, HashAlgorithmName algorithmName)
        {
            if (string.IsNullOrEmpty(data))
            {
                throw new ArgumentNullException(nameof(data));
            }

            using (var rsa = System.Security.Cryptography.RSA.Create())
            {
                rsa.ImportParameters(privateKey);
                return Base64.ToBase64String(rsa.SignData(Encoding.UTF8.GetBytes(data), algorithmName, RSASignaturePadding.Pkcs1));
            }
        }

        /// <summary>
        /// 验证签名
        /// </summary>
        public static bool VerifySignature(string data, string sign, RSAParameters publicKey, HashAlgorithmName algorithmName)
        {
            if (string.IsNullOrEmpty(data))
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (string.IsNullOrEmpty(sign))
            {
                throw new ArgumentNullException(nameof(sign));
            }

            using (var rsa = System.Security.Cryptography.RSA.Create())
            {
                rsa.ImportParameters(publicKey);
                return rsa.VerifyData(Encoding.UTF8.GetBytes(data), Base64.Decode(sign), algorithmName, RSASignaturePadding.Pkcs1);
            }
        }

        /// <summary>
        /// 生成签名
        /// </summary>
        public static string GenerateSignature(string data, AsymmetricKeyParameter privateKey,string signerName= "SHA1WithRSA")
        {
            var byteData = Encoding.UTF8.GetBytes(data);
            var normalSig = SignerUtilities.GetSigner(signerName);
            normalSig.Init(true, privateKey);
            normalSig.BlockUpdate(byteData, 0, data.Length);
            var normalResult = normalSig.GenerateSignature();
            return Base64.ToBase64String(normalResult);
        }

        /// <summary>
        /// 验证签名
        /// </summary>
        public static bool VerifySignature(string data, string sign, AsymmetricKeyParameter publicKey, string signerName = "SHA1WithRSA")
        {
            var signBytes = Base64.Decode(sign);
            var plainBytes = Encoding.UTF8.GetBytes(data);
            var verifier = SignerUtilities.GetSigner(signerName);
            verifier.Init(false, publicKey);
            verifier.BlockUpdate(plainBytes, 0, plainBytes.Length);

            return verifier.VerifySignature(signBytes);
        }
    }
}
