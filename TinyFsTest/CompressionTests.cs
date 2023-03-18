namespace TinyFsTest
{
    public class CompressionTests
    {
        [Test]
        public void TestCompression()
        {
            for (var i = 1; ; i++)
            {
                var data = new byte[i];
                var compressed = Compression.Compress(data);
                TestContext.Out.WriteLine("Compressed {0} bytes into {1} bytes", data.Length, compressed.Length);
                TestContext.Out.WriteLine("data: {0}", string.Join(' ', compressed.Select(m => m.ToString("X2"))));
                if (compressed.Length < data.Length)
                {
                    break;
                }
            }
            Assert.Pass();
        }

        [Test]
        public void TestDecompression()
        {
            var data = new byte[100];
            var compressed = Compression.Compress(data);
            var decompressed = Compression.Decompress(compressed);
            Assert.That(data, Is.EquivalentTo(decompressed));
        }
    }
}
