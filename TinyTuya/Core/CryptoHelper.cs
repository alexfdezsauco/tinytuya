using System;
using System.Security.Cryptography;
using System.Text;

namespace TinyTuya.Core
{
    /// <summary>
    /// Helper class for cryptographic operations
    /// </summary>
    public class CryptoHelper
    {
        private readonly byte[] _key;
        private readonly RijndaelManaged _aes;

        /// <summary>
        /// Initializes a new instance of the <see cref="CryptoHelper"/> class
        /// </summary>
        /// <param name="key">The encryption key</param>
        public CryptoHelper(byte[] key)
        {
            _key = key;
            _aes = new RijndaelManaged
            {
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7,
                KeySize = 128,
                Key = key
            };
        }

        /// <summary>
        /// Encrypts data using AES-ECB
        /// </summary>
        /// <param name="data">The data to encrypt</param>
        /// <returns>The encrypted data</returns>
        public byte[] Encrypt(byte[] data)
        {
            using (var encryptor = _aes.CreateEncryptor())
            {
                return encryptor.TransformFinalBlock(data, 0, data.Length);
            }
        }

        /// <summary>
        /// Decrypts data using AES-ECB
        /// </summary>
        /// <param name="data">The data to decrypt</param>
        /// <returns>The decrypted data</returns>
        public byte[] Decrypt(byte[] data)
        {
            using (var decryptor = _aes.CreateDecryptor())
            {
                return decryptor.TransformFinalBlock(data, 0, data.Length);
            }
        }

        /// <summary>
        /// Computes the MD5 hash of a string
        /// </summary>
        /// <param name="input">The input string</param>
        /// <returns>The MD5 hash as a byte array</returns>
        public static byte[] MD5Hash(string input)
        {
            using (var md5 = MD5.Create())
            {
                return md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            }
        }

        /// <summary>
        /// Computes the MD5 hash of a string and returns it as a hexadecimal string
        /// </summary>
        /// <param name="input">The input string</param>
        /// <returns>The MD5 hash as a hexadecimal string</returns>
        public static string MD5HashString(string input)
        {
            var hash = MD5Hash(input);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        /// <summary>
        /// Computes the HMAC-SHA256 of a message using a key
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="message">The message</param>
        /// <returns>The HMAC-SHA256 as a byte array</returns>
        public static byte[] HMACSHA256(byte[] key, byte[] message)
        {
            using (var hmac = new HMACSHA256(key))
            {
                return hmac.ComputeHash(message);
            }
        }

        /// <summary>
        /// Computes the HMAC-SHA256 of a message using a key and returns it as a hexadecimal string
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="message">The message</param>
        /// <returns>The HMAC-SHA256 as a hexadecimal string</returns>
        public static string HMACSHA256String(byte[] key, byte[] message)
        {
            var hash = HMACSHA256(key, message);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        /// <summary>
        /// Computes the HMAC-SHA256 of a message using a key and returns it as a hexadecimal string
        /// </summary>
        /// <param name="key">The key as a string</param>
        /// <param name="message">The message as a string</param>
        /// <returns>The HMAC-SHA256 as a hexadecimal string</returns>
        public static string HMACSHA256String(string key, string message)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var messageBytes = Encoding.UTF8.GetBytes(message);
            return HMACSHA256String(keyBytes, messageBytes);
        }
    }
}

