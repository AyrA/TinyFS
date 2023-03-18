using System.Text;

namespace TinyFSLib
{
    /// <summary>
    /// Represents a TinyFS container
    /// </summary>
    public class FS
    {
        internal const int MaxFileSize =
            //Magic
            sizeof(int) +
            //Flags
            sizeof(byte) +
            //Entry count
            sizeof(byte) +
            FileData.MaxAllEntrySizes + AES.MaxGrowth;

        private const int Magic = 0x594E4954;
        private readonly List<FileData> filesystem;

        private FsFlags flags;

        /// <summary>
        /// Gets or sets whether names are treated to be case insensitive or not.
        /// </summary>
        /// <remarks>
        /// Changing this has an immediate effect on the data already loaded in memory.
        /// Enabling this can thus potentially create a situation where some file names become unreachable.
        /// You cannot save until all name problems are resolved.
        /// </remarks>
        public bool IsCaseInsensitive
        {
            get => flags.HasFlag(FsFlags.CaseInsensitive);
            set
            {
                if (value)
                {
                    var orig = flags;
                    flags |= FsFlags.CaseInsensitive;
                    if (filesystem.Any(m => filesystem.Count(n => CompareName(n.Name, m.Name)) > 1))
                    {
                        flags = orig;
                        throw new InvalidOperationException("Enabling case insensitive mode would render at least one file unavailable");
                    }
                }
                else
                {
                    flags &= ~FsFlags.CaseInsensitive;
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the container is encrypted
        /// </summary>
        /// <remarks>
        /// Enabling encryption requires you to supply an encryption key to the respective writer methods
        /// </remarks>
        public bool IsEncrypted
        {
            get => flags.HasFlag(FsFlags.Encrypted);
            set
            {
                if (value)
                {
                    flags |= FsFlags.Encrypted;
                }
                else
                {
                    flags &= ~FsFlags.Encrypted;
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the container uses UTF-8 for file names
        /// </summary>
        /// <remarks>
        /// Changing this to false may fail to encode file names with characters
        /// outside of plain ASCII properly
        /// </remarks>
        public bool UseUTF8
        {
            get => flags.HasFlag(FsFlags.UTF8);
            set
            {
                if (value)
                {
                    flags |= FsFlags.UTF8;
                }
                else
                {
                    flags &= ~FsFlags.UTF8;
                }
            }
        }

        /// <summary>
        /// Gets all file names from the container
        /// </summary>
        public string[] Names => filesystem.Select(m => m.Name).ToArray();

        /// <summary>
        /// Gets or sets file data in the container.
        /// This is a simplified way to access
        /// <see cref="GetFile(string)"/> and <see cref="SetFile(FileData)"/>
        /// </summary>
        /// <param name="name">File name</param>
        /// <returns>File data</returns>
        public FileData this[string name]
        {
            get => GetFile(name);
            set => SetFile(value);
        }

        /// <summary>
        /// Creates a new, empty container
        /// </summary>
        public FS()
        {
            filesystem = new();
            flags = FsFlags.None;
        }

        /// <summary>
        /// Loads a container from the given file
        /// </summary>
        /// <param name="path">File path</param>
        /// <param name="encryptionKey">AES key (if container is encrypted)</param>
        public FS(string path, byte[]? encryptionKey = null) : this()
        {
            //Create empty TinyFS container if file cannot be found
            if (File.Exists(path))
            {
                using var f = File.OpenRead(path);
                Load(f, encryptionKey);
            }
        }

        /// <summary>
        /// Loads a container from the given stream
        /// </summary>
        /// <param name="data"></param>
        /// <param name="encryptionKey">AES key (if container is encrypted)</param>
        /// <remarks>The stream is only read exactly as far as necessary</remarks>
        public FS(Stream data, byte[]? encryptionKey = null) : this()
        {
            Load(data, encryptionKey);
        }

        /// <summary>
        /// Loads a container from existing data
        /// </summary>
        /// <param name="data">TinyFS container data</param>
        /// <param name="encryptionKey">AES key (if container is encrypted)</param>
        public FS(byte[] data, byte[]? encryptionKey = null) : this()
        {
            using var MS = new MemoryStream(data, false);
            Load(MS, encryptionKey);
        }

        /// <summary>
        /// Loads a container from the given file
        /// </summary>
        /// <param name="path">File path</param>
        /// <param name="encryptionKey">Password</param>
        public FS(string path, string encryptionKey) : this()
        {
            //Create empty TinyFS container if file cannot be found
            if (File.Exists(path))
            {
                using var f = File.OpenRead(path);
                Load(f, encryptionKey);
            }
        }

        /// <summary>
        /// Loads a container from the given stream
        /// </summary>
        /// <param name="data"></param>
        /// <param name="encryptionKey">Password</param>
        /// <remarks>The stream is only read exactly as far as necessary</remarks>
        public FS(Stream data, string encryptionKey) : this()
        {
            Load(data, encryptionKey);
        }

        /// <summary>
        /// Loads a container from existing data
        /// </summary>
        /// <param name="data">TinyFS container data</param>
        /// <param name="encryptionKey">Password</param>
        public FS(byte[] data, string encryptionKey) : this()
        {
            using var MS = new MemoryStream(data, false);
            Load(MS, encryptionKey);
        }

        private void WriteFsData(Stream output)
        {
            //Create a copy to avoid problems of other threads potentially changing entries after validation passed.
            var entries = filesystem.Select(m => (FileData)m.Clone()).ToList();
            foreach (var entry in entries)
            {
                try
                {
                    entry.Validate();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Cannot save data. Entry '{entry.Name}' failed validation. Reason: {ex.Message}", ex);
                }
                if (entries.Count(m => CompareName(m.Name, entry.Name)) > 1)
                {
                    throw new InvalidOperationException($"Duplicate file name: '{entry.Name}'");
                }
            }

            using var BW = new BinaryWriter(output, Encoding.UTF8, true);
            BW.Write((byte)entries.Count);

            var sorted = entries.OrderBy(m => m.Name.ToUpper()).ThenBy(m => m.Name).ToList();

            //Write FAT
            foreach (var entry in sorted)
            {
                entry.WriteHeader(BW, UseUTF8);
            }

            //Write data
            foreach (var entry in sorted)
            {
                entry.WriteData(BW);
            }
        }

        /// <summary>
        /// Writes the container data to a stream
        /// </summary>
        /// <param name="output">Stream to write to</param>
        /// <param name="encryptionKey">AES key (if container is encrypted)</param>
        /// <exception cref="ArgumentNullException"><paramref name="output"/> is null</exception>
        /// <exception cref="InvalidOperationException">Encryption key argument is null but encryption flag is set</exception>
        /// <remarks>This writes only as much as needed and will not seek the stream. The stream is not disposed</remarks>
        public void Write(Stream output, byte[]? encryptionKey = null)
        {
            if (output is null)
            {
                throw new ArgumentNullException(nameof(output));
            }
            if (encryptionKey is null && IsEncrypted)
            {
                throw new InvalidOperationException($"'{nameof(IsEncrypted)}' property is set but not key was provided in '{nameof(encryptionKey)}' argument. You must provide a key or disable encryption.");
            }

            using var BW = new BinaryWriter(output, Encoding.UTF8, true);
            BW.Write(Magic);
            BW.Write((byte)flags);

            using var MS = new MemoryStream();
            WriteFsData(MS);
            if (IsEncrypted)
            {
                var data = MS.ToArray();
                data = AES.Encrypt(data, encryptionKey!);
                BW.Write7BitEncodedInt(data.Length);
                BW.Write(data);
            }
            else
            {
                BW.Write(MS.ToArray());
            }
        }

        /// <summary>
        /// Writes the container data to a stream
        /// </summary>
        /// <param name="output">Stream to write to</param>
        /// <param name="encryptionKey">Password</param>
        /// <exception cref="ArgumentNullException"><paramref name="output"/> is null</exception>
        /// <exception cref="ArgumentException">
        /// Encryption key argument is null or empty</exception>
        /// <remarks>This writes only as much as needed and will not seek the stream. The stream is not disposed</remarks>
        public void Write(Stream output, string encryptionKey)
        {
            if (output is null)
            {
                throw new ArgumentNullException(nameof(output));
            }
            if (string.IsNullOrEmpty(encryptionKey))
            {
                throw new ArgumentException($"No key was provided in '{nameof(encryptionKey)}' argument. You must provide a key or disable encryption.");
            }

            using var BW = new BinaryWriter(output, Encoding.UTF8, true);
            BW.Write(Magic);
            BW.Write((byte)flags);

            using var MS = new MemoryStream();
            WriteFsData(MS);
            if (IsEncrypted)
            {
                var data = MS.ToArray();
                data = AES.Encrypt(data, encryptionKey);
                BW.Write7BitEncodedInt(data.Length);
                BW.Write(data);
            }
            else
            {
                BW.Write(MS.ToArray());
            }
        }

        /// <summary>
        /// Writes the container data to a file
        /// </summary>
        /// <param name="path">File path</param>
        /// <param name="encryptionKey">AES key (if container is encrypted)</param>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is null</exception>
        public void Write(string path, byte[]? encryptionKey = null)
        {
            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            using var f = File.Create(path);
            Write(f, encryptionKey);
        }

        /// <summary>
        /// Writes the container data to a file
        /// </summary>
        /// <param name="path">File path</param>
        /// <param name="encryptionKey">AES key</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="path"/> or <paramref name="encryptionKey"/> is null
        /// </exception>
        public void Write(string path, string encryptionKey)
        {
            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }
            using var f = File.Create(path);
            Write(f, encryptionKey);
        }

        /// <summary>
        /// Writes the container data to a byte array
        /// </summary>
        /// <param name="encryptionKey">AES key (if container is encrypted)</param>
        /// <returns>Container data</returns>
        public byte[] ToByteArray(byte[]? encryptionKey = null)
        {
            using var MS = new MemoryStream();
            Write(MS, encryptionKey);
            return MS.ToArray();
        }

        /// <summary>
        /// Writes the container data to a byte array using the given encryption key
        /// </summary>
        /// <param name="encryptionKey">Encryption key</param>
        /// <returns></returns>
        public byte[] ToByteArray(string encryptionKey)
        {
            using var MS = new MemoryStream();
            Write(MS, encryptionKey);
            return MS.ToArray();
        }

        /// <summary>
        /// Gets a file from the container
        /// </summary>
        /// <param name="name">File name</param>
        /// <returns>File data</returns>
        /// <exception cref="ArgumentException">Name is invalid</exception>
        public FileData GetFile(string name)
        {
            return
                filesystem.FirstOrDefault(f => CompareName(f.Name, name)) ??
                throw new ArgumentException($"File with this name was not found: '{name}'");
        }

        /// <summary>
        /// Adds or replaces a file in the container
        /// </summary>
        /// <param name="name">File name</param>
        /// <param name="fileData">File data</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"><paramref name="name"/> is null or empty</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="fileData"/> is too large</exception>
        /// <exception cref="ArgumentNullException"><paramref name="fileData"/> is null</exception>
        public FileData SetFile(string name, byte[] fileData)
        {
            bool forceCompression = false;
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
            }
            if (Encoding.UTF8.GetByteCount(name) > byte.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(name), $"'{nameof(name)}' can be at most 255 bytes");
            }

            if (fileData is null)
            {
                throw new ArgumentNullException(nameof(fileData));
            }
            if (fileData.Length > ushort.MaxValue)
            {
                if (Compression.Compress(fileData).Length > ushort.MaxValue)
                {
                    throw new ArgumentOutOfRangeException(nameof(fileData), $"Data length can be at most {ushort.MaxValue} bytes");
                }
                forceCompression = true;
            }

            var data = filesystem.FirstOrDefault(m => CompareName(m.Name, name));
            if (data == null)
            {
                if (filesystem.Count == byte.MaxValue)
                {
                    throw new InvalidOperationException("File table is full");
                }
                data = new FileData(name, fileData, forceCompression ? FileFlags.GZip : FileFlags.None);
                filesystem.Add(data);
            }
            else
            {
                data.Data = fileData;
                data.IsCompressed |= forceCompression;
            }
            return data;
        }

        /// <summary>
        /// Adds or replaces a file in the container
        /// </summary>
        /// <param name="fd">File data</param>
        /// <exception cref="ArgumentNullException">Argument is null</exception>
        /// <exception cref="ArgumentException">File data failed validation</exception>
        /// <exception cref="Exception">File data instance already exists in this container</exception>
        public void SetFile(FileData fd)
        {
            if (fd is null)
            {
                throw new ArgumentNullException(nameof(fd));
            }
            try
            {
                fd.Validate();
            }
            catch (Exception ex)
            {
                throw new ArgumentException("The file data entry is not valid.", nameof(fd), ex);
            }
            if (filesystem.Contains(fd))
            {
                throw new Exception("A reference to this entry is already in this TinyFS container");
            }

            var data = filesystem.FirstOrDefault(m => CompareName(m.Name, fd.Name));
            if (data == null)
            {
                if (filesystem.Count == byte.MaxValue)
                {
                    throw new InvalidOperationException("File table is full");
                }
                filesystem.Add(fd);
            }
            else
            {
                data.Data = (byte[])fd.Data.Clone();
                data.Flags = fd.Flags;
            }
        }

        /// <summary>
        /// Removes a file from the container
        /// </summary>
        /// <param name="name">File name</param>
        /// <exception cref="InvalidOperationException">Attempted to remove the last file</exception>
        /// <exception cref="ArgumentException"><paramref name="name"/> does not exist in this container</exception>
        /// <returns>Removed entry</returns>
        public FileData DeleteFile(string name)
        {
            if (filesystem.Count == 1)
            {
                throw new InvalidOperationException("Cannot delete last file.");
            }
            var f =
                filesystem.FirstOrDefault(m => CompareName(m.Name, name)) ??
                throw new ArgumentException($"Invalid file name: '{name}'");
            filesystem.Remove(f);
            return f;
        }

        /// <summary>
        /// Checks if a file with the given name exists in the container
        /// </summary>
        /// <param name="name">File name</param>
        /// <returns>true, if a file with this name exists</returns>
        /// <exception cref="ArgumentException"><paramref name="name"/> is invalid</exception>
        public bool HasFile(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
            }
            return filesystem.Any(f => CompareName(f.Name, name));
        }

        private void Load(Stream S, byte[]? encryptionKey = null)
        {
            //Cannot use a "using" here because we're potentially reassigning this
            //Using try{}finally{} instead
            var BR = new BinaryReader(S, Encoding.UTF8, true);
            try
            {
                //Check header
                if (BR.ReadInt32() != Magic)
                {
                    throw new InvalidDataException("Invalid TinyFS header");
                }
                //Read file system flags and decrypt if encryption flag is set
                flags = (FsFlags)BR.ReadByte();
                if (flags.HasFlag(FsFlags.Encrypted))
                {
                    if (encryptionKey == null)
                    {
                        throw new ArgumentNullException(nameof(encryptionKey), "Encryption key cannot be null. TinyFS has encryption flag set.");
                    }
                    var size = BR.Read7BitEncodedInt();
                    if (size <= 0)
                    {
                        throw new InvalidDataException("Encrypted prefix is invalid");
                    }
                    if (size > MaxFileSize)
                    {
                        throw new InvalidDataException($"Encrypted payload specified to be {size} bytes but this exceeds maximum possible size of {FileData.MaxAllEntrySizes + AES.MaxGrowth}");
                    }

                    byte[] decrypted;
                    try
                    {
                        decrypted = AES.Decrypt(BR.ReadBytes(size), encryptionKey);
                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentException("Cannot decrypt data using the given key. See inner exception for details", nameof(encryptionKey), ex);
                    }
                    BR.Dispose();
                    BR = new BinaryReader(new MemoryStream(decrypted, false));
                }
                int fileCount = BR.ReadByte();
                var files = new FileData[fileCount];
                //Read FAT
                for (var i = 0; i < fileCount; i++)
                {
                    var data = new FileData(BR, UseUTF8);
                    if (files.Any(m => m != null && CompareName(m.Name, data.Name)))
                    {
                        throw new Exception($"Duplicate file name: {data.Name}");
                    }
                    files[i] = data;
                }
                //Read data
                for (var i = 0; i < fileCount; i++)
                {
                    files[i].SetRawData(BR.ReadBytes(files[i].Data.Length));
                }
                filesystem.AddRange(files);
            }
            finally
            {
                BR.Dispose();
            }
        }

        private void Load(Stream S, string encryptionKey)
        {
            //Cannot use a "using" here because we're potentially reassigning this
            //Using try{}finally{} instead
            var BR = new BinaryReader(S, Encoding.UTF8, true);
            try
            {
                //Check header
                if (BR.ReadInt32() != Magic)
                {
                    throw new InvalidDataException("Invalid TinyFS header");
                }
                //Read file system flags and decrypt if encryption flag is set
                flags = (FsFlags)BR.ReadByte();
                if (flags.HasFlag(FsFlags.Encrypted))
                {
                    if (encryptionKey == null)
                    {
                        throw new ArgumentNullException(nameof(encryptionKey), "Encryption key cannot be null. TinyFS has encryption flag set.");
                    }
                    var size = BR.Read7BitEncodedInt();
                    if (size <= 0)
                    {
                        throw new InvalidDataException("Encrypted prefix is invalid");
                    }
                    if (size > MaxFileSize)
                    {
                        throw new InvalidDataException($"Encrypted payload specified to be {size} bytes but this exceeds maximum possible size of {FileData.MaxAllEntrySizes + AES.MaxGrowth}");
                    }
                    byte[] decrypted;
                    try
                    {
                        decrypted = AES.Decrypt(BR.ReadBytes(size), encryptionKey);
                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentException("Cannot decrypt data using the given key. See inner exception for details", nameof(encryptionKey), ex);
                    }
                    BR.Dispose();
                    BR = new BinaryReader(new MemoryStream(decrypted, false));
                }
                int fileCount = BR.ReadByte();
                var files = new FileData[fileCount];
                //Read FAT
                for (var i = 0; i < fileCount; i++)
                {
                    var data = new FileData(BR, UseUTF8);
                    if (files.Any(m => m != null && CompareName(m.Name, data.Name)))
                    {
                        throw new Exception($"Duplicate file name: {data.Name}");
                    }
                    files[i] = data;
                }
                //Read data
                for (var i = 0; i < fileCount; i++)
                {
                    files[i].SetRawData(BR.ReadBytes(files[i].Data.Length));
                }
                filesystem.AddRange(files);
            }
            finally
            {
                BR.Dispose();
            }
        }

        private bool CompareName(string name1, string name2)
        {
            if (IsCaseInsensitive)
            {
                return name1.ToUpper() == name2.ToUpper();
            }
            return name1 == name2;
        }

        /// <summary>
        /// Gets the flags of a TinyFS container
        /// </summary>
        /// <param name="path">TinyFS container file</param>
        /// <returns><see cref="FsFlags"/></returns>
        /// <exception cref="InvalidDataException">Not a TinyFS file</exception>
        public static FsFlags GetInfo(string path)
        {
            using var FS = File.OpenRead(path);
            return GetInfo(FS);
        }

        /// <summary>
        /// Gets the flags of the TinyFS container contained in <paramref name="data"/>
        /// </summary>
        /// <param name="data">Container data</param>
        /// <returns><see cref="FsFlags"/></returns>
        /// <exception cref="InvalidDataException">Not a TinyFS file</exception>
        /// <remarks><paramref name="data"/> must contain a TinyFS header</remarks>
        public static FsFlags GetInfo(byte[] data)
        {
            using var MS = new MemoryStream(data, false);
            return GetInfo(MS);
        }

        /// <summary>
        /// Gets the flags of the TinyFS container that <paramref name="data"/> points to
        /// </summary>
        /// <param name="data">Data reader</param>
        /// <returns><see cref="FsFlags"/></returns>
        /// <exception cref="InvalidDataException">Not a TinyFS file</exception>
        /// <remarks><paramref name="data"/> must point to the start of a TinyFS container</remarks>
        public static FsFlags GetInfo(Stream data)
        {
            using var BR = new BinaryReader(data, Encoding.UTF8, true);
            return GetInfo(BR);
        }

        /// <summary>
        /// Gets the flags of the TinyFS container that <paramref name="data"/> points to
        /// </summary>
        /// <param name="data">Data reader</param>
        /// <returns><see cref="FsFlags"/></returns>
        /// <exception cref="InvalidDataException">Not a TinyFS file</exception>
        /// <remarks><paramref name="data"/> must point to the start of a TinyFS container</remarks>
        public static FsFlags GetInfo(BinaryReader data)
        {
            if (data.ReadInt32() != Magic)
            {
                throw new InvalidDataException("Not a TinyFS file");
            }
            return (FsFlags)data.ReadByte();
        }
    }
}
