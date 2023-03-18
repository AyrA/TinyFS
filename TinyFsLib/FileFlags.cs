namespace TinyFSLib
{
    /// <summary>
    /// File specific properties
    /// </summary>
    [Flags]
    public enum FileFlags : byte
    {
        /// <summary>
        /// File is binary as-is
        /// </summary>
        None = 0,
        /// <summary>
        /// File is encrypted
        /// </summary>
        /// <remarks>
        /// This is currently not supported.
        /// Use <see cref="FsFlags.Encrypted"/> on the entire container instead
        /// </remarks>
        Encrypted = 1,
        /// <summary>
        /// Data is gzip encoded
        /// </summary>
        GZip = Encrypted << 1,
        /// <summary>
        /// Reserved for future use
        /// </summary>
        Flag3 = GZip << 1,
        /// <summary>
        /// Reserved for future use
        /// </summary>
        Flag4 = Flag3 << 1,
        /// <summary>
        /// Reserved for future use
        /// </summary>
        Flag5 = Flag4 << 1,
        /// <summary>
        /// Reserved for future use
        /// </summary>
        Flag6 = Flag5 << 1,
        /// <summary>
        /// Reserved for future use
        /// </summary>
        Flag7 = Flag6 << 1,
        /// <summary>
        /// Reserved for future use
        /// </summary>
        Flag8 = Flag7 << 1
    }
}
