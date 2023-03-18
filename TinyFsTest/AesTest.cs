namespace TinyFsTest
{
    public class AesTest
    {
        [Test]
        public void EncryptWithKey()
        {
            AES.Encrypt(new byte[ushort.MaxValue], Utils.GetPredictableRandomData(32));
        }

        [Test]
        public void EncryptWithPassword()
        {
            AES.Encrypt(new byte[ushort.MaxValue], "test");
        }

        [Test]
        public void DecryptWithKey()
        {
            var data = Utils.GetPredictableRandomData(ushort.MaxValue);
            var key = data.Skip(100).Take(32).ToArray();
            var enc = AES.Encrypt(data, key);
            Assert.That(AES.Decrypt(enc, key), Is.EquivalentTo(data));
        }

        [Test]
        public void DecryptWithPassword()
        {
            var data = Utils.GetPredictableRandomData(ushort.MaxValue);
            var enc = AES.Encrypt(data, "test");
            Assert.That(AES.Decrypt(enc, "test"), Is.EquivalentTo(data));
        }
    }
}
