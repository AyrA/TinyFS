namespace TinyFSLib
{
    /// <summary>
    /// Flags applying to an entire TinyFS container
    /// </summary>
    [Flags]
    public enum FsFlags : byte
    {
        /// <summary>
        /// TinyFS container has no special properties
        /// </summary>
        None = 0,
        /// <summary>
        /// File names are to be treated in a case insensitive manner
        /// </summary>
        CaseInsensitive = 1,
        /// <summary>
        /// Container data is encrypted
        /// </summary>
        Encrypted = CaseInsensitive << 1,
        /// <summary>
        /// Reserved for future use
        /// </summary>
        UTF8 = Encrypted << 1,
        /// <summary>
        /// Reserved for future use
        /// </summary>
        Flag4 = UTF8 << 1,
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
