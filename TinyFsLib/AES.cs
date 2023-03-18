using System.Security.Cryptography;

namespace TinyFSLib
{
    /// <summary>
    /// Provides TinyFS specific AES encryption and decryption
    /// </summary>
    /// <remarks>Developers are advised to instead use the appropriate <see cref="FS"/> constructor</remarks>
    public static class AES
    {
        /// <summary>
        /// Maximum number of bytes data will grow when encrypting
        /// </summary>
        internal const int MaxGrowth =
            //Nonce
            12 +
            //Tag
            16 +
            //Salt
            16;

        /// <summary>
        /// Length for the salt
        /// </summary>
        /// <remarks>
        /// This is written to the encrypted data, meaning a longer salt also increases data size
        /// </remarks>
        private const int SaltLength = 16;

        /// <summary>
        /// The number of iterations for <see cref="Rfc2898DeriveBytes"/>
        /// </summary>
        private const int Iterations = 100000;

        /// <summary>
        /// Checks for AES support
        /// </summary>
        /// <exception cref="NotSupportedException">AES GCM is unavailable on the current platform</exception>
        static AES()
        {
            if (!AesGcm.IsSupported)
            {
                throw new NotSupportedException($"AES GCM is unavailable on this platform. OS type: {Environment.OSVersion.Platform}");
            }
        }

        /// <summary>
        /// Decrypts data using the given password
        /// </summary>
        /// <param name="data">Ciphertext</param>
        /// <param name="password">Password</param>
        /// <returns>Plaintext</returns>
        public static byte[] Decrypt(byte[] data, string password)
        {
            var salt = data.Take(SaltLength).ToArray();
            return Decrypt(data.Skip(salt.Length).ToArray(), GetKey(password, salt));
        }

        /// <summary>
        /// Decrypts data using the given AES key
        /// </summary>
        /// <param name="data">Ciphertext</param>
        /// <param name="key">AES key</param>
        /// <returns>Plaintext</returns>
        public static byte[] Decrypt(byte[] data, byte[] key)
        {
            using var alg = new AesGcm(key);
            var nonce = data.Take(AesGcm.NonceByteSizes.MaxSize).ToArray();
            var tag = data.Skip(nonce.Length).Take(AesGcm.TagByteSizes.MaxSize).ToArray();
            var ciphertext = data.Skip(nonce.Length + tag.Length).ToArray();
            var plaintext = new byte[ciphertext.Length];
            alg.Decrypt(nonce, ciphertext, tag, plaintext);
            return plaintext;
        }

        /// <summary>
        /// Encrypts data using the given password
        /// </summary>
        /// <param name="data">Plaintext</param>
        /// <param name="password">Password</param>
        /// <returns>Ciphertext</returns>
        public static byte[] Encrypt(byte[] data, string password)
        {
            var salt = GetSalt();
            RandomNumberGenerator.Fill(salt);
            var key = GetKey(password, salt);
            return salt.Concat(Encrypt(data, key)).ToArray();
        }

        /// <summary>
        /// Encrypts data using the given AES key
        /// </summary>
        /// <param name="data">Plaintext</param>
        /// <param name="key">AES key</param>
        /// <returns>Ciphertext</returns>
        public static byte[] Encrypt(byte[] data, byte[] key)
        {
            using var alg = new AesGcm(key);
            var nonce = new byte[AesGcm.NonceByteSizes.MaxSize];
            RandomNumberGenerator.Fill(nonce);
            var ciphertext = new byte[data.Length];
            var tag = new byte[AesGcm.TagByteSizes.MaxSize];
            alg.Encrypt(nonce, data, ciphertext, tag);
            return nonce.Concat(tag).Concat(ciphertext).ToArray();
        }

        /// <summary>
        /// Converts password and salt into an AES key
        /// </summary>
        /// <param name="password">Password</param>
        /// <param name="salt">Salt</param>
        /// <returns>AES key (32 bytes)</returns>
        private static byte[] GetKey(string password, byte[] salt)
        {
            return Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, 32);
        }

        /// <summary>
        /// Get a salt value for <see cref="GetKey(string, byte[])"/>
        /// </summary>
        /// <returns>Salt value</returns>
        private static byte[] GetSalt()
        {
            var salt = new byte[SaltLength];
            RandomNumberGenerator.Fill(salt);
            return salt;
        }
    }
}
