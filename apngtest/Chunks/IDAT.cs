namespace apngtest.Chunks
{
    public struct IDAT(byte[] data)
    {
        public byte[] Data { get; set; } = data;
    }
}
