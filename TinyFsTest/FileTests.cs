namespace TinyFsTest
{
    public class FileTests
    {
        [Test]
        public void NewTinyFsFile()
        {
            _ = new FS();
            Assert.Pass();
        }

        [Test]
        public void AddExistingFile()
        {
            var fs = new FS();
            var data = Utils.GetPredictableRandomData(128);
            fs.SetFile("test", data);
            Assert.That(fs.GetFile("test").Data, Is.EquivalentTo(data));
        }

        [Test]
        public void MaxFileSizeLimit()
        {
            var fs = new FS();
            var data = Utils.GetPredictableRandomData(ushort.MaxValue);
            fs.SetFile("test", data);
            Assert.That(fs.GetFile("test").Data, Is.EquivalentTo(data));
            Assert.Throws<InvalidOperationException>(() => fs.SetFile("test", new byte[ushort.MaxValue + 1]));
        }

        [Test]
        public void CanAddZeroByteFile()
        {
            var fs = new FS();
            fs.SetFile("test", Array.Empty<byte>());
            Assert.That(fs.GetFile("test").Data, Is.EquivalentTo(Array.Empty<byte>()));
        }

        [Test]
        public void CheckMaxNameLength()
        {
            var fs = new FS();
            fs.SetFile(string.Empty.PadRight(byte.MaxValue, 'a'), new byte[1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => fs.SetFile(string.Empty.PadRight(byte.MaxValue + 1, 'a'), new byte[1]));
        }

        [Test]
        public void Save()
        {
            var fs = new FS();
            fs.SetFile("test", new byte[128]);
            byte[] tinyfs = fs.ToByteArray();
            using var MS = new MemoryStream();
            fs.Write(MS);
            Assert.That(tinyfs, Is.EquivalentTo(MS.ToArray()));
        }

        [Test]
        public void Load()
        {
            var data1 = Utils.GetPredictableRandomData(128);
            var fs1 = new FS();
            fs1.SetFile("test", data1);
            byte[] tinyfs = fs1.ToByteArray();
            var fs2 = new FS(tinyfs);
            var data2 = fs2["test"];
            Assert.That(data1, Is.EquivalentTo(data2.Data));
        }

        [Test]
        public void Overwrite()
        {
            var fs = new FS();
            var data1 = Utils.GetPredictableRandomData(128);
            var data2 = Utils.GetPredictableRandomData(256);
            fs.SetFile("test", data1);
            fs.SetFile("test", data2);
            Assert.That(fs.GetFile("test").Data, Is.EquivalentTo(data2));
        }

        [Test]
        public void CompressionSave()
        {
            //Get a repeatable pattern for easy compression
            var data = Enumerable.Repeat(Utils.Byte16, 100).SelectMany(m => m).ToArray();
            var fs = new FS();
            fs.SetFile("compress1", data).IsCompressed = true;
            fs.SetFile("compress2", data).IsCompressed = true;
            var serialized = fs.ToByteArray();
            TestContext.Out.WriteLine("Raw data: {0} bytes; Compressed TinyFS: {1} bytes", data.Length, serialized.Length);
            Assert.That(serialized, Has.Length.LessThan(data.Length));
        }

        [Test]
        public void CompressionLoad()
        {
            var data1 = Enumerable.Repeat(Utils.Byte16, 100).SelectMany(m => m).ToArray();
            var data2 = data1.Reverse().ToArray();
            var fs = new FS();
            fs.SetFile("compress1", data1).IsCompressed = true;
            fs.SetFile("compress2", data2).IsCompressed = true;
            var serialized = fs.ToByteArray();
            fs = new FS(serialized);

            var decomp1 = fs.GetFile("compress1").Data;
            var decomp2 = fs.GetFile("compress2").Data;

            Assert.Multiple(() =>
            {
                Assert.That(decomp1, Is.EquivalentTo(data1));
                Assert.That(decomp2, Is.EquivalentTo(data2));
            });
        }

        [Test]
        public void EncryptedSaveWithKey()
        {
            var key = Utils.GetPredictableRandomData(32);
            var fs = new FS
            {
                IsEncrypted = true
            };
            fs.SetFile("test", Utils.Byte16);
            using var MS = new MemoryStream();
            fs.Write(MS, key);
        }

        [Test]
        public void EncryptedLoadWithKey()
        {
            var key = Utils.GetPredictableRandomData(32);
            var fs = new FS
            {
                IsEncrypted = true
            };
            fs.SetFile("test", Utils.Byte16);
            using var MS = new MemoryStream();
            fs.Write(MS, key);
            MS.Position = 0;
            fs = new(MS, key);
        }

        [Test]
        public void EncryptedSaveWithPassword()
        {
            var fs = new FS
            {
                IsEncrypted = true
            };
            fs.SetFile("test", Utils.Byte16);
            using var MS = new MemoryStream();
            fs.Write(MS, "Password.1");
        }

        [Test]
        public void EncryptedLoadWithPassword()
        {
            var fs = new FS
            {
                IsEncrypted = true
            };
            fs.SetFile("test", Utils.Byte16);
            using var MS = new MemoryStream();
            fs.Write(MS, "Password.1");
            MS.Position = 0;
            fs = new(MS, "Password.1");
        }

        [Test]
        public void EncryptedWrongKey()
        {
            var key = Utils.GetPredictableRandomData(32);
            var fs = new FS
            {
                IsEncrypted = true
            };
            fs.SetFile("test", Utils.Byte16);
            using var MS = new MemoryStream();
            fs.Write(MS, key);
            ++key[0];
            MS.Position = 0;
            Assert.Throws<ArgumentException>(() => fs = new(MS, key));
        }

        [Test]
        public void EncryptedWrongPassword()
        {
            var fs = new FS
            {
                IsEncrypted = true
            };
            fs.SetFile("test", Utils.Byte16);
            using var MS = new MemoryStream();
            fs.Write(MS, "Password.1");
            MS.Position = 0;
            Assert.Throws<ArgumentException>(() => fs = new(MS, "Password.2"));
        }

        [Test]
        public void MaxContainerSize()
        {
            var fs = new FS();
            var data = Utils.GetPredictableRandomData(ushort.MaxValue);
            for (var i = 0; i < byte.MaxValue; i++)
            {
                fs.SetFile($"{i + 1}_".PadRight(byte.MaxValue, '#'), data);
            }
            var result = fs.ToByteArray();
            TestContext.Out.WriteLine("File size is {0} bytes", result.Length);
            //Should be able to load it again
            _ = new FS(result);
            //Can use this for UI integration tests
            File.WriteAllBytes("C:\\temp\\max.tfs", result);
            Assert.Pass();
        }

        [Test]
        public void MaxContainerSizeEncrypted()
        {
            var fs = new FS();
            var data = Utils.GetPredictableRandomData(ushort.MaxValue);
            for (var i = 0; i < byte.MaxValue; i++)
            {
                fs.SetFile($"{i + 1}_".PadRight(byte.MaxValue, '#'), data);
            }
            fs.IsEncrypted = true;
            var result = fs.ToByteArray("Password.1");
            TestContext.Out.WriteLine("File size is {0} bytes", result.Length);
            _ = new FS(result, "Password.1");
            Assert.Pass();
        }
    }
}