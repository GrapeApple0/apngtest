using apngtest.Chunks;
using apngtest.Utils;

namespace apngtest
{
    public class Program
    {
        public static List<Frame> LoadAPNG(byte[] bin)
        {
            if (!Enumerable.SequenceEqual(bin.Take(8).ToArray(), new byte[] { 0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a }))
            {
                throw new InvalidDataException("Binary is not a PNG data or signature is invalid.");
            }
            int loc = 8;
            bool finish = false;
            fcTL fctl = default;
            PLTE plte = default;
            IHDR ihdr = default;
            pHYs phys = default;
            var fdATs = new List<fdAT>();
            var IDATs = new List<IDAT>();
            var frames = new List<Frame>();
            while (!finish)
            {
                Console.WriteLine($"loc: {loc}");
                var header = new Header(bin.Skip(loc).Take(8).ToArray());
                //ヘッダー、チャンクデータ本体とCRCを含める
                var chunk = new ChunkData(header, bin.Skip(loc).Take(8 + (int)header.Length + 4).ToArray());
                switch (header.ChunkType)
                {
                    case ChunkType.IHDR:
                        ihdr = new IHDR(chunk.Data);
                        break;
                    case ChunkType.PLTE:
                        plte = new PLTE(chunk.Data);
                        break;
                    case ChunkType.acTL:
                        var actl = new acTL(chunk.Data);
                        break;
                    case ChunkType.fcTL:
                        if (fdATs.Count > 0)
                        {
                            var png = PNGExtractor.PNGFromfdAT(fctl, plte, ihdr, phys, fdATs);
                            var frame = new Frame(png, fctl.DelayNum, fctl.Width, fctl.Height, fctl.XOffset, fctl.YOffset);
                            frames.Add(frame);
                        }
                        if (IDATs.Count > 0) {
                            var png = PNGExtractor.PNGFromIDAT(fctl, plte, ihdr, phys, IDATs);
                            var frame = new Frame(png, fctl.DelayNum, fctl.Width, fctl.Height, fctl.XOffset, fctl.YOffset);
                            frames.Add(frame);
                        }
                        fctl = new fcTL(chunk.Data);
                        fdATs.Clear();
                        IDATs.Clear();
                        break;
                    case ChunkType.fdAT:
                        var fdat = new fdAT(chunk.Data);
                        fdATs.Add(fdat);
                        break;
                    case ChunkType.pHYs:
                        phys = new pHYs(chunk.Data);
                        break;
                    case ChunkType.IDAT:
                        IDAT idat = new IDAT(chunk.Data);
                        IDATs.Add(idat);
                        break;
                    case ChunkType.IEND:
                        finish = true;
                        break;
                    case ChunkType.tEXt:
                    case ChunkType.Unknown:
                    default:
                        break;
                }
                // crc検証時はlengthの部分を取り除く
                var dataCRC = CRC32.Calculate(bin.Skip(loc).Skip(4).Take(4 + (int)header.Length).ToArray());
                if (dataCRC == chunk.CRC) Console.WriteLine($"CRC check OK");
                else Console.WriteLine("CRC check failed");
                Console.WriteLine($"CRC: chunk: {chunk.CRC}/data: {dataCRC}");
                loc += (int)header.Length + 12;
            }
            return frames;
        }

        public static void Main()
        {
            var filename = "elephant.png";
            var bin = File.ReadAllBytes(filename);
            var frames = LoadAPNG(bin);
            if (!Directory.Exists(filename + "-frames"))
            {
                Directory.CreateDirectory(filename + "-frames");
            }
            for (int i = 0; i < frames.Count; i++)
            {
                var frame = frames[i];
                File.WriteAllBytes($"{filename}-frames/frame{i}.png", frame.Data);
                var info = $"Width: {frame.Width}\nHeight: {frame.Height}\nX Offset: {frame.XOffset}\nY Offset: {frame.YOffset}\nDelay: {frame.Delay}";
                File.WriteAllText($"{filename}-frames/frame{i}.txt", info);
            }
        }
    }
}
