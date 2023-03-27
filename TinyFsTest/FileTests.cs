namespace TinyFsTest
{
    /// <summary>
    /// TinyFS container tests
    /// </summary>
    public class FileTests
    {
        /// <summary>
        /// Create a blank TinyFS file
        /// </summary>
        [Test]
        public void NewTinyFsFile()
        {
            _ = new FS();
            Assert.Pass();
        }

        /// <summary>
        /// Add a file to a TinyFS container
        /// </summary>
        [Test]
        public void AddExistingFile()
        {
            var fs = new FS();
            var data = Utils.GetPredictableRandomData(128);
            fs.SetFile("test", data);
            Assert.That(fs.GetFile("test").Data, Is.EquivalentTo(data));
        }

        /// <summary>
        /// Ensure that a <see cref="ushort.MaxValue"/> length file fits
        /// </summary>
        [Test]
        public void MaxFileSizeLimit()
        {
            var fs = new FS();
            var data = Utils.GetPredictableRandomData(ushort.MaxValue);
            fs.SetFile("test", data);
            Assert.That(fs.GetFile("test").Data, Is.EquivalentTo(data));
            Assert.Throws<InvalidOperationException>(() => fs.SetFile("test", new byte[ushort.MaxValue + 1]));
        }

        /// <summary>
        /// Ensure that zero length files can be added and read
        /// </summary>
        [Test]
        public void CanAddZeroByteFile()
        {
            var fs = new FS();
            fs.SetFile("test", Array.Empty<byte>());
            Assert.That(fs.GetFile("test").Data, Is.EquivalentTo(Array.Empty<byte>()));
        }

        /// <summary>
        /// Ensure that <see cref="byte.MaxValue"/> byte long file names work
        /// </summary>
        [Test]
        public void CheckMaxNameLength()
        {
            var fs = new FS();
            fs.SetFile(string.Empty.PadRight(byte.MaxValue, 'a'), new byte[1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => fs.SetFile(string.Empty.PadRight(byte.MaxValue + 1, 'a'), new byte[1]));
        }

        /// <summary>
        /// Test saving a container to byte array and stream
        /// </summary>
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

        /// <summary>
        /// Test loading a container from byte array
        /// </summary>
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

        /// <summary>
        /// Test overwriting of TinyFS files
        /// </summary>
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

        /// <summary>
        /// Save a compressed container
        /// </summary>
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

        /// <summary>
        /// Load a compressed container
        /// </summary>
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

        /// <summary>
        /// Encrypt a conainer using a key
        /// </summary>
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

        /// <summary>
        /// Load a container encrypted with a key
        /// </summary>
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

        /// <summary>
        /// Encrypt a container with a password
        /// </summary>
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

        /// <summary>
        /// Load a container encrypted with a password
        /// </summary>
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

        /// <summary>
        /// Ensure wrong keys do not work
        /// </summary>
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

        /// <summary>
        /// Ensure wrong passwords do not work
        /// </summary>
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

        /// <summary>
        /// Create a container of maximum possible size
        /// </summary>
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

        /// <summary>
        /// Encrypt a container of maximum possible size
        /// </summary>
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