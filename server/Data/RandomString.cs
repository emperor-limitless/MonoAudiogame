using System;
using System.Text;
using System.Security.Cryptography;
namespace Data
{
    public static class RandomString
    {
        public static string GenerateRandomString(int length = 32)
        {
            const string allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_=-";
            var rng = RandomNumberGenerator.Create();
            var result = new StringBuilder(length);

            var buffer = new byte[sizeof(uint)];
            while (length > 0)
            {
                rng.GetBytes(buffer);
                var randomNumber = BitConverter.ToUInt32(buffer, 0);
                var charIndex = randomNumber % allowedChars.Length;
                var randomChar = allowedChars[(int)charIndex];
                result.Append(randomChar);
                length--;
            }

            return result.ToString();
        }
    }
}
