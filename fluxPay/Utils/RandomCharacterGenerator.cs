using System;
using System.Security.Cryptography;
using System.Text;

namespace fluxPay.Utils
{
    public class RandomCharacterGenerator
    {
        public static string GenerateRandomString(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];

                while (length-- > 0)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    res.Append(valid[(int)(num % (uint)valid.Length)]);
                }
            }

            return res.ToString();
        }

        public static int RandomNumber(int numberOfDigit)
        {
            Random random = new Random();
            int min = (int)Math.Pow(10, numberOfDigit - 1);
            int max = (int)Math.Pow(10, numberOfDigit) - 1;
            return random.Next(min, max);
        }
    }

}