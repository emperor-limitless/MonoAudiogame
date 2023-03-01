using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
namespace Data
{
    public class Encryption
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;
        private readonly Encoding _encoding = Encoding.UTF8;
        public Encryption(string key)
        {
            _key = _encoding.GetBytes(key);
            _iv = GenerateIV(key);
        }
        private byte[] GenerateIV(string key)
        {
            var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(_encoding.GetBytes(key));
            var iv = new byte[16];
            Array.Copy(hash, iv, 16);
            return iv;
        }
        public byte[] Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = _key;
            aes.IV = _iv;
            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var memoryStream = new MemoryStream();
            using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            using var streamWriter = new StreamWriter(cryptoStream);
            streamWriter.Write(plainText);
            streamWriter.Flush();
            cryptoStream.FlushFinalBlock();
            return memoryStream.ToArray();
        }
        public string Decrypt(byte[] cipherText)
        {
            using var aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = _key;
            aes.IV = _iv;
            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var memoryStream = new MemoryStream(cipherText);
            using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            using var streamReader = new StreamReader(cryptoStream);
            return streamReader.ReadToEnd();
        }
    }
}
