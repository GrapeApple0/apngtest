using System.Linq;

namespace apngtest
{
    internal class Program
    {
        public struct IHDR
        {
            public uint Width;
            public uint Height;
            public byte BitDepth;
            public byte ColorType;
            public byte Compress;
            public byte Filter;
            public byte Interlace;
            public byte[] CRC;
        }

        public struct acTL
        {
            public uint NumFrames;
            public uint NumPlays;
            public byte[] CRC;
        }

        public struct fcTL
        {
            public uint SequenceNumber;
            public uint Width;
            public uint Height;
            public uint XOffset;
            public uint YOffset;
            public ushort DelayNum;
            public ushort DelayDen;
            public byte DisposeOp;
            public byte BlendOp;
            public byte[] CRC;
        }

        public struct pHYs
        {
            public uint X;
            public uint Y;
            public byte Unit;
            public byte[] CRC;
        }

        public static IHDR ParseIHDR(byte[] data)
        {
            var ihdr = new IHDR();
            if (data.Length != 17) return ihdr;
            ihdr.Width = (uint)(data[0] << 24 | data[1] << 16 | data[2] << 8 | data[3]);
            ihdr.Height = (uint)(data[4] << 24 | data[5] << 16 | data[6] << 8 | data[7]);
            ihdr.BitDepth = data[8];
            ihdr.ColorType = data[9];
            ihdr.Compress = data[10];
            ihdr.Filter = data[11];
            ihdr.Interlace = data[12];
            ihdr.CRC = data.Skip(13).Take(4).ToArray();
            return ihdr;
        }

        public static acTL ParseacTL(byte[] data)
        {
            var actl = new acTL();
            if (data.Length != 12) return actl;
            actl.NumFrames = (uint)(data[0] << 24 | data[1] << 16 | data[2] << 8 | data[3]);
            actl.NumPlays = (uint)(data[4] << 24 | data[5] << 16 | data[6] << 8 | data[7]);
            actl.CRC = data.Skip(8).Take(4).ToArray();
            return actl;
        }

        public static fcTL ParsefcTL(byte[] data)
        {
            var fctl = new fcTL();
            if (data.Length != 30) return fctl;
            fctl.SequenceNumber = (uint)(data[0] << 24 | data[1] << 16 | data[2] << 8 | data[3]);
            fctl.Width = (uint)(data[4] << 24 | data[5] << 16 | data[6] << 8 | data[7]);
            fctl.Height = (uint)(data[8] << 24 | data[9] << 16 | data[10] << 8 | data[11]);
            fctl.XOffset = (uint)(data[12] << 24 | data[13] << 16 | data[14] << 8 | data[15]);
            fctl.YOffset = (uint)(data[16] << 24 | data[17] << 16 | data[18] << 8 | data[19]);
            fctl.DelayNum = (ushort)(data[20] << 8 | data[21]);
            fctl.DelayDen = (ushort)(data[22] << 8 | data[23]);
            fctl.DisposeOp = data[24];
            fctl.BlendOp = data[25];
            fctl.CRC = data.Skip(26).Take(4).ToArray();
            return fctl;
        }

        public static pHYs ParsepHYs(byte[] data)
        {
            var phys = new pHYs();
            if (data.Length != 13) return phys;
            phys.X = (uint)(data[0] << 24 | data[1] << 16 | data[2] << 8 | data[3]);
            phys.Y = (uint)(data[4] << 24 | data[5] << 16 | data[6] << 8 | data[7]);
            phys.Unit = data[8];
            phys.CRC = data.Skip(9).Take(4).ToArray();
            return phys;
        }

        public enum ChunkType
        {
            Unknown,
            IHDR,
            acTL,
            fcTL,
            fdAT,
            pHYs,
            IDAT,
            IEND,
            tEXt,
        }

        public struct Header
        {
            public uint Length;
            public ChunkType chunkType;
        }

        public static Header CheckChunkType(byte[] data)
        {
            var type = data.Skip(4).Take(4).ToArray();
            ChunkType chunkType = 0;
            if (Enumerable.SequenceEqual(type, "IHDR"u8.ToArray())) chunkType = ChunkType.IHDR;
            if (Enumerable.SequenceEqual(type, "acTL"u8.ToArray())) chunkType = ChunkType.acTL;
            if (Enumerable.SequenceEqual(type, "fcTL"u8.ToArray())) chunkType = ChunkType.fcTL;
            if (Enumerable.SequenceEqual(type, "pHYs"u8.ToArray())) chunkType = ChunkType.pHYs;
            if (Enumerable.SequenceEqual(type, "IDAT"u8.ToArray())) chunkType = ChunkType.IDAT;
            if (Enumerable.SequenceEqual(type, "fdAT"u8.ToArray())) chunkType = ChunkType.fdAT;
            if (Enumerable.SequenceEqual(type, "IEND"u8.ToArray())) chunkType = ChunkType.IEND;
            if (Enumerable.SequenceEqual(type, "tEXt"u8.ToArray())) chunkType = ChunkType.tEXt;
            if (chunkType == 0)
            {
                foreach (var item in data)
                {
                    Console.Write("0x" + item.ToString("X4") + $"/({(char)item}), ");
                }
                Console.WriteLine();
            }
            return new Header
            {
                Length = (uint)(data[0] << 24 | data[1] << 16 | data[2] << 8 | data[3]),
                chunkType = chunkType
            };
        }

        public static void LoadAPNG(byte[] bin)
        {
            int loc;
            if (!Enumerable.SequenceEqual(bin.Take(8).ToArray(), new byte[] { 0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a }))
            {
                Console.WriteLine("Not a PNG file");
                return;
            }
            loc = 8;
            bool finish = false;
            while (!finish)
            {
                Console.WriteLine($"loc: {loc}");
                var header = CheckChunkType(bin.Skip(loc).Take(8).ToArray());
                switch (header.chunkType)
                {
                    case ChunkType.IHDR:
                        var ihdr = ParseIHDR(bin.Skip(loc + 8).Take(17).ToArray());// skip 4 byte signature and 4 byte IHDR length
                        Console.WriteLine("Load IHDR chunk");
                        //Console.WriteLine($"Width: {ihdr.Width}");
                        //Console.WriteLine($"Height: {ihdr.Height}");
                        //Console.WriteLine($"Bit Depth: {ihdr.BitDepth}");
                        //Console.WriteLine($"Color Type: {ihdr.ColorType}");
                        //Console.WriteLine($"Compress: {ihdr.Compress}");
                        //Console.WriteLine($"Filter: {ihdr.Filter}");
                        //Console.WriteLine($"Interlace: {ihdr.Interlace}");
                        loc += 25;
                        break;
                    case ChunkType.acTL:
                        var actl = ParseacTL(bin.Skip(loc + 8).Take(12).ToArray());
                        Console.WriteLine("Load acTL chunk");
                        //Console.WriteLine($"Num Frames: {actl.NumFrames}");
                        //Console.WriteLine($"Num Plays: {actl.NumPlays}");
                        loc += 20;
                        break;
                    case ChunkType.fcTL:
                        var fctl = ParsefcTL(bin.Skip(loc + 8).Take(30).ToArray());
                        Console.WriteLine("Load fcTL chunk");
                        Console.WriteLine($"Width: {fctl.Width}");
                        Console.WriteLine($"Height: {fctl.Height}");
                        Console.WriteLine($"X Offset: {fctl.XOffset}");
                        Console.WriteLine($"Y Offset: {fctl.YOffset}");
                        //Console.WriteLine($"Delay Num: {fctl.DelayNum}");
                        //Console.WriteLine($"Delay Den: {fctl.DelayDen}");
                        //Console.WriteLine($"Dispose Op: {fctl.DisposeOp}");
                        //Console.WriteLine($"Blend Op: {fctl.BlendOp}");
                        loc += 38;
                        break;
                    case ChunkType.pHYs:
                        var phys = ParsepHYs(bin.Skip(loc + 8).Take(13).ToArray());
                        Console.WriteLine("Load pHYs chunk");
                        //Console.WriteLine($"X: {phys.X}");
                        //Console.WriteLine($"Y: {phys.Y}");
                        //Console.WriteLine($"Unit: {phys.Unit}");
                        loc += 21;
                        break;
                    case ChunkType.IDAT:
                        Console.WriteLine("Load IDAT chunk");
                        loc += 8 + (int)header.Length + 4;
                        break;
                    case ChunkType.fdAT:
                        Console.WriteLine("Load fdAT chunk");
                        loc += 8 + (int)header.Length + 4;
                        break;
                    case ChunkType.IEND:
                        Console.WriteLine("Load IEND chunk");
                        finish = true;
                        break;
                    case ChunkType.tEXt:
                        Console.WriteLine("Load tEXt chunk");
                        loc += 8 + (int)header.Length + 4;
                        break;
                    case ChunkType.Unknown:
                        Console.WriteLine("Unknown chunk");
                        finish = true;
                        break;
                    default:
                        break;
                }
            }
        }

        public static void Main(string[] args)
        {
            var bin = File.ReadAllBytes("polarbear.png");
            LoadAPNG(bin);
            bin = File.ReadAllBytes("elephant.png");
            LoadAPNG(bin);
        }
    }
}
