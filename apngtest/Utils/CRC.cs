namespace apngtest.Utils
{
    public static class CRC32
    {
        public static uint[] MakeCrcTable()
        {
            uint[] a = new uint[256];
            for (uint i = 0; i < a.Length; i++)
            {
                uint c = i;
                for (int j = 0; j < 8; j++)
                {
                    c = (c & 1) != 0 ? 0xEDB88320 ^ c >> 1 : c >> 1;
                }
                a[i] = c;
            }
            return a;
        }

        public static uint Calculate(byte[] buf)
        {
            return Calculate(buf, 0, buf.Length);
        }

        public static uint Calculate(byte[] buf, int start, int len)
        {
            uint c = 0xFFFFFFFF;
            checked
            {
                if (len < 0)
                {
                    throw new ArgumentException();
                }
                if (start < 0 || start + len > buf.Length)
                {
                    throw new IndexOutOfRangeException();
                }
            }
            for (int i = 0; i < len; i++)
            {
                c = MakeCrcTable()[(c ^ buf[start + i]) & 0xFF] ^ c >> 8;
            }
            return c ^ 0xFFFFFFFF;
        }
    }
}
