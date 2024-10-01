namespace apngtest.Chunks
{
    public enum ChunkType
    {
        Unknown,
        IHDR,
        PLTE,
        acTL,
        fcTL,
        fdAT,
        pHYs,
        IDAT,
        IEND,
        tEXt,
    }

    public struct ChunkData
    {
        //データ の長さ
        public uint Length { get; set; }
        // チャンクの種類
        public ChunkType ChunkType { get; set; }
        //純粋なデータ
        public byte[] Data { get; set; }
        //CRC
        public uint CRC { get; set; }

        public ChunkData(uint length, ChunkType chunkType, byte[] data, uint crc)
        {
            Length = length;
            ChunkType = chunkType;
            Data = data;
            CRC = crc;
        }

        public ChunkData(Header header, byte[] bytes)
        {
            if (bytes.Length != header.Length + 4 + 4 + 4)
            {
                throw new InvalidDataException("Bytes length is not match length data of header");
            }
            this.Length = header.Length;
            this.ChunkType = header.ChunkType;
            this.Data = bytes.Skip(8).Take((int)header.Length).ToArray();
            this.CRC = BitConverter.ToUInt32(bytes.Skip(8).Skip((int)header.Length).Take(4).Reverse().ToArray());
        }
    }
}
