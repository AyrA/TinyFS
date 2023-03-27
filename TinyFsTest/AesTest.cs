namespace TinyFsTest
{
    /// <summary>
    /// Generic AES tests
    /// </summary>
    public class AesTest
    {
        /// <summary>
        /// Encrypt with a key
        /// </summary>
        [Test]
        public void EncryptWithKey()
        {
            AES.Encrypt(new byte[ushort.MaxValue], Utils.GetPredictableRandomData(32));
        }

        /// <summary>
        /// Encrypt with a password
        /// </summary>
        [Test]
        public void EncryptWithPassword()
        {
            AES.Encrypt(new byte[ushort.MaxValue], "test");
        }

        /// <summary>
        /// Decrypt with a key
        /// </summary>
        [Test]
        public void DecryptWithKey()
        {
            var data = Utils.GetPredictableRandomData(ushort.MaxValue);
            var key = data.Skip(100).Take(32).ToArray();
            var enc = AES.Encrypt(data, key);
            Assert.That(AES.Decrypt(enc, key), Is.EquivalentTo(data));
        }

        /// <summary>
        /// Decrypt with a password
        /// </summary>
        [Test]
        public void DecryptWithPassword()
        {
            var data = Utils.GetPredictableRandomData(ushort.MaxValue);
            var enc = AES.Encrypt(data, "test");
            Assert.That(AES.Decrypt(enc, "test"), Is.EquivalentTo(data));
        }
    }
}
