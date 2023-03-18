namespace TinyFS
{
    internal enum ArgumentMode
    {
        None = 0,
        Help = None + 1,
        Add = Help + 1,
        Save = Add + 1,
        Remove = Save + 1,
        Encrypt = Remove + 1,
        Decrypt = Encrypt + 1
    }
}
