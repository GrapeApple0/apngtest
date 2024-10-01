namespace apngtest.Chunks
{
    public struct Header
    {
        public uint Length;
        public ChunkType ChunkType;

        public Header(uint length, ChunkType chunkType)
        {
            Length = length;
            ChunkType = chunkType;
        }

        /// <param name="bytes">このデータにはヘッダーとCRCを含めたデータが入っている</param>
        /// <exception cref="InvalidDataException"></exception>
        public Header(byte[] bytes)
        {
            if (bytes.Length != 8)
            {
                throw new InvalidDataException("Invalid header length");
            }
            this.Length = BitConverter.ToUInt32(bytes.Take(4).Reverse().ToArray());
            var type = bytes.Skip(4).Take(4).ToArray();
            ChunkType chunkType = 0;
            if (Enumerable.SequenceEqual(type, "IHDR"u8.ToArray())) chunkType = ChunkType.IHDR;
            if (Enumerable.SequenceEqual(type, "PLTE"u8.ToArray())) chunkType = ChunkType.PLTE;
            if (Enumerable.SequenceEqual(type, "acTL"u8.ToArray())) chunkType = ChunkType.acTL;
            if (Enumerable.SequenceEqual(type, "fcTL"u8.ToArray())) chunkType = ChunkType.fcTL;
            if (Enumerable.SequenceEqual(type, "pHYs"u8.ToArray())) chunkType = ChunkType.pHYs;
            if (Enumerable.SequenceEqual(type, "IDAT"u8.ToArray())) chunkType = ChunkType.IDAT;
            if (Enumerable.SequenceEqual(type, "fdAT"u8.ToArray())) chunkType = ChunkType.fdAT;
            if (Enumerable.SequenceEqual(type, "IEND"u8.ToArray())) chunkType = ChunkType.IEND;
            if (Enumerable.SequenceEqual(type, "tEXt"u8.ToArray())) chunkType = ChunkType.tEXt;
            this.ChunkType = chunkType;
        }
    }
}
