using System.Text;

namespace TinyFSLib
{
    /// <summary>
    /// Represents a file in a TinyFS container
    /// </summary>
    public class FileData : ICloneable
    {
        /// <summary>
        /// The maximum length in bytes of the data
        /// </summary>
        public const ushort MaxDataSize = ushort.MaxValue;

        /// <summary>
        /// The maximum number of bytes an entry occupies in a TinyFS container.
        /// This is basically the maximum index size plus the maximum data size
        /// </summary>
        internal const int MaxEntrySize =
            //Flags
            sizeof(byte) +
            //Data length
            sizeof(ushort) +
            //Name length
            sizeof(byte) +
            //Max name length
            byte.MaxValue +
            //Max data length
            MaxDataSize;

        /// <summary>
        /// The maximum size a full TinyFS container can occupy (minus header part preceeding the entries)
        /// </summary>
        internal const int MaxAllEntrySizes = (MaxEntrySize * byte.MaxValue) + 1;

        private string name;
        private FileFlags flags;
        private byte[] data;

        /// <summary>
        /// Gets or sets the file name
        /// </summary>
        /// <remarks>
        /// The name is treated as UTF-8, and limited to 255 bytes.
        /// Check using <see cref="UTF8Encoding.GetByteCount(string)"/> if necessary.
        /// </remarks>
        public string Name
        {
            get => name;
            set
            {
                var old = name;
                try
                {
                    name = value;
                    Validate();
                }
                catch
                {
                    name = old;
                    throw;
                }
            }
        }

        /// <summary>
        /// Gets or sets the flags for this file
        /// </summary>
        /// <remarks>This must not be set to unimplemented flags</remarks>
        public FileFlags Flags
        {
            get => flags;
            set
            {
                var old = flags;
                try
                {
                    flags = value;
                    Validate();
                }
                catch
                {
                    flags = old;
                    throw;
                }
            }
        }

        /// <summary>
        /// Gets or sets the data
        /// </summary>
        /// <remarks>This always is the decoded data, regardless of the <see cref="Flags"/> value</remarks>
        public byte[] Data
        {
            get => data;
            set
            {
                var old = data;
                try
                {
                    data = value;
                    Validate();
                }
                catch
                {
                    data = old;
                    throw;
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the compression flag is set
        /// </summary>
        public bool IsCompressed
        {
            get => flags.HasFlag(FileFlags.GZip);
            set
            {
                if (value)
                {
                    flags |= FileFlags.GZip;
                }
                else
                {
                    flags &= ~FileFlags.GZip;
                }
            }
        }

        internal FileData(string name, ushort size, FileFlags flags) : this(name, new byte[size], flags)
        { }

        internal FileData(string name, byte[] data, FileFlags flags)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
            }

            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            this.name = name;
            this.data = data;
            this.flags = flags;

            try
            {
                Validate();
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Arguments do not parse into valid data", ex);
            }
        }

        internal FileData(BinaryReader BR, bool UseUTF8)
        {
            flags = (FileFlags)BR.ReadByte();
            data = new byte[BR.ReadUInt16()];
            name = (UseUTF8 ? Encoding.UTF8 : Encoding.ASCII).GetString(BR.ReadBytes(BR.ReadByte()));
            Validate();
        }

        internal FileData(Stream s, bool UseUTF8) : this(new BinaryReader(s, Encoding.UTF8, true), UseUTF8)
        {

        }

        /// <summary>
        /// Checks if decompression would reduce the data size
        /// </summary>
        /// <returns>true, if <see cref="Flags"/> should be set to enable compression, false otherwise.</returns>
        public bool IsCompressionRecommended()
        {
            return Compression.GetCompressionGain(data) > 0;
        }

        internal void WriteHeader(BinaryWriter BW, bool UseUTF8)
        {
            Validate();
            BW.Write((byte)Flags);
            BW.Write((ushort)Compress(Data).Length);
            var nameBytes = (UseUTF8 ? Encoding.UTF8 : Encoding.ASCII).GetBytes(Name);
            BW.Write((byte)nameBytes.Length);
            BW.Write(nameBytes);
        }

        internal void WriteHeader(Stream s, bool UseUTF8)
        {
            Validate();
            using var BW = new BinaryWriter(s, Encoding.UTF8, true);
            WriteHeader(BW, UseUTF8);
        }

        internal void WriteData(BinaryWriter BW)
        {
            Validate();
            BW.Write(Compress(Data));
        }

        internal void WriteData(Stream s)
        {
            Validate();
            s.Write(Compress(Data));
        }

        internal void Validate()
        {
            if (Data == null)
            {
                throw new InvalidOperationException("Data is not set");
            }
            if (Compress(Data).Length > ushort.MaxValue)
            {
                if (!IsCompressed)
                {
                    if (Compression.Compress(Data).Length <= ushort.MaxValue)
                    {
                        throw new InvalidOperationException("Data is too long for TinyFS but a compression test has shown that enabling compression would make the data fit.");
                    }
                    throw new InvalidOperationException("Data is too long for TinyFS");
                }
                throw new InvalidOperationException("Data is too long for TinyFS");
            }
            if (string.IsNullOrEmpty(Name))
            {
                throw new InvalidOperationException("Name is not set");
            }
            if (Encoding.UTF8.GetByteCount(Name) > byte.MaxValue)
            {
                throw new InvalidOperationException("Name is too long");
            }
            if (Flags != (Flags & (FileFlags.GZip | FileFlags.Encrypted)))
            {
                throw new InvalidOperationException("Unknown flags are set");
            }
            if (Flags.HasFlag(FileFlags.Encrypted))
            {
                throw new InvalidOperationException("File level encryption flag is not currently supported");
            }
        }

        internal void SetRawData(byte[] data)
        {
            Validate();
            Data = Decompress(data);
        }

        private byte[] Compress(byte[] data)
        {
            if (flags.HasFlag(FileFlags.GZip))
            {
                return Compression.Compress(data);
            }
            return data;
        }

        private byte[] Decompress(byte[] data)
        {
            if (flags.HasFlag(FileFlags.GZip))
            {
                return Compression.Decompress(data);
            }
            return data;
        }

        /// <summary>
        /// Creates a copy of this instance
        /// </summary>
        /// <returns>Copy</returns>
        public object Clone()
        {
            var copy = (FileData)MemberwiseClone();
            copy.data = (byte[])copy.data.Clone();
            return copy;
        }
    }
}
