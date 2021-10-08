using Google.Authenticator;
using masterCore.Entities;
using masterCore.Interfaces;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace masterShared.Services
{
    public class TwoFactorAuthService : ITwoFactorAuthService
    {
        public async Task<string> DecriptToken(string jwt, string key)
        {
            byte[] token = Convert.FromBase64String(jwt);
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] keys = md5.ComputeHash(Encoding.UTF8.GetBytes(key));
                using (TripleDESCryptoServiceProvider triD = new() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                {
                    ICryptoTransform transform = triD.CreateDecryptor();
                    byte[] result = transform.TransformFinalBlock(token, 0, token.Length);
                    jwt = Encoding.UTF8.GetString(result);
                }
            }

            return jwt;
        }

        public async Task<string> EncriptToken(string jwt, string key)
        {
            byte[] token = Encoding.UTF8.GetBytes(jwt);
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] keys = md5.ComputeHash(Encoding.UTF8.GetBytes(key));
                using (TripleDESCryptoServiceProvider triD = new() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                {
                    ICryptoTransform transform = triD.CreateEncryptor();
                    byte[] result = transform.TransformFinalBlock(token, 0, token.Length);
                    jwt = Convert.ToBase64String(result, 0, result.Length);
                }
            }

            return jwt;
        }

        public async Task<responseData> GenerateSetupCode(string issuer, string email, string key)
        {
            var responseData = new responseData();

            TwoFactorAuthenticator tfa = new();
            responseData.data = tfa.GenerateSetupCode(issuer, email, key, false, 3);

            return responseData;
        }

        public async Task<responseData> ValidateTwoFactorAuth(string key, string code)
        {
            var responseData = new responseData();

            TwoFactorAuthenticator tfa = new();
            responseData.data = tfa.ValidateTwoFactorPIN(key, code);

            return responseData;
        }
    }
}
