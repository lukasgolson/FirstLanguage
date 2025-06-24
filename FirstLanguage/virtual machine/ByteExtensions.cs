namespace FirstLanguage.virtual_machine;

public static class ByteExtensions
{
    public static byte[] ToBytes(this long value)
    {
        var bytes = BitConverter.GetBytes(value);
        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes); // Ensure Little Endian byte order
        }

        return bytes;
    }

    public static long ToLong(this byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);

        if (bytes.Length < sizeof(long))
        {
            throw new ArgumentException($"Byte array must be at least {sizeof(long)} bytes long.", nameof(bytes));
        }

        var
            localBytes =
                (byte[])bytes.Clone(); // Work on a copy to avoid modifying the original array if it's used elsewhere
        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(localBytes, 0, sizeof(long)); // Ensure Little Endian byte order before conversion
        }

        return BitConverter.ToInt64(localBytes, 0);
    }
}