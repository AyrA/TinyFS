using System.Security.Cryptography;

namespace TinyFsTest
{
    public class Utils
    {
        private static readonly byte[] key;

        public static readonly byte[] Byte16 = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

        static Utils()
        {
            key = new byte[32];
            RandomNumberGenerator.Fill(key);
        }

        public static byte[] GetPredictableRandomData(int count)
        {
            using var aes = new AesGcm(key);
            var plaintext = new byte[count];
            var ciphertext = new byte[count];
            var nonce = key.Take(AesGcm.NonceByteSizes.MaxSize).ToArray();
            aes.Encrypt(nonce, plaintext, ciphertext, new byte[AesGcm.TagByteSizes.MaxSize]);
            return ciphertext;
        }
    }
}
