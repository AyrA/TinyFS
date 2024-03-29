﻿using System.IO.Compression;

namespace TinyFSLib
{
    /// <summary>
    /// Provides compression and decompression utilities
    /// </summary>
    /// <remarks><see cref="FS"/> and <see cref="FileData"/> handle this automatically</remarks>
    public class Compression
    {
        /// <summary>
        /// Minimum data size to achieve a positive effect when compressing
        /// </summary>
        /// <remarks>
        /// Compression adds a few bytes for header, dictionary, and checksum (18+ bytes).
        /// Data shorter than this number will inflate the data rather than shrink it.
        /// </remarks>
        public const int MinDataSizeForCompression = 24;

        /// <summary>
        /// Gets the number of bytes saved by compression
        /// </summary>
        /// <param name="data">Decompressed data</param>
        /// <returns>Number of bytes saved. Zero if none or inflated</returns>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> is null</exception>
        public static int GetCompressionGain(byte[] data)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            if (data.Length < MinDataSizeForCompression)
            {
                return 0;
            }
            return Math.Max(0, data.Length - Compress(data).Length);
        }

        /// <summary>
        /// Compresses the given data using GZip
        /// </summary>
        /// <param name="data">Uncompressed data</param>
        /// <returns>Compressed data</returns>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> is null</exception>
        public static byte[] Compress(byte[] data)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            if (data.Length == 0)
            {
                return Array.Empty<byte>();
            }

            using var dataIn = new MemoryStream(data, false);
            using var dataOut = new MemoryStream();
            using var compressor = new GZipStream(dataOut, CompressionLevel.Optimal);
            dataIn.CopyTo(compressor);
            compressor.Close();
            return dataOut.ToArray();
        }

        /// <summary>
        /// Decompresses the given data using GZip
        /// </summary>
        /// <param name="data">Compressed data</param>
        /// <returns>Decompressed data</returns>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> is null</exception>
        public static byte[] Decompress(byte[] data)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            if (data.Length == 0)
            {
                return Array.Empty<byte>();
            }

            using var dataIn = new MemoryStream(data, false);
            using var dataOut = new MemoryStream();
            using var compressor = new GZipStream(dataIn, CompressionMode.Decompress);
            compressor.CopyTo(dataOut);
            return dataOut.ToArray();
        }

        /// <summary>
        /// Compress a stream of data with respect to TinyFS size limitations
        /// </summary>
        /// <param name="s">Data stream</param>
        /// <returns>Compressed data</returns>
        /// <exception cref="ArgumentException">Compressed data size exceeds TinyFS specs</exception>
        public static byte[] CompressTiny(Stream s)
        {
            using var MS = new MemoryStream();
            using var c = new GZipStream(MS, CompressionLevel.Optimal);
            while (true)
            {
                byte[] data = new byte[ushort.MaxValue + 1];
                int read = s.Read(data, 0, data.Length);
                if (read > 0)
                {
                    c.Write(data, 0, read);
                    if (MS.Length > ushort.MaxValue)
                    {
                        throw new ArgumentException("Compressed data size exceeds TinyFS specs");
                    }
                }
                else
                {
                    c.Flush();
                    c.Close();
                    var ret = MS.ToArray();
                    if (ret.Length > ushort.MaxValue)
                    {
                        throw new ArgumentException("Compressed data size exceeds TinyFS specs");
                    }
                    return ret;
                }
            }
        }

        /// <summary>
        /// Decompresses a stream of data with respect to TinyFS size limitations
        /// </summary>
        /// <param name="s">Compressed data</param>
        /// <returns>Decompressed data</returns>
        /// <exception cref="ArgumentException">Compressed data size exceeds TinyFS specs</exception>
        public static byte[] DecompressTiny(Stream s)
        {
            using var MS = new MemoryStream();
            while (true)
            {
                byte[] data = new byte[ushort.MaxValue + 1];
                int read = s.Read(data, 0, data.Length);
                if (read == 0)
                {
                    break;
                }
                MS.Write(data, 0, read);
                if (MS.Length > ushort.MaxValue)
                {
                    throw new ArgumentException("Compressed data size exceeds TinyFS specs");
                }
            }
            return Decompress(MS.ToArray());
        }
    }
}
