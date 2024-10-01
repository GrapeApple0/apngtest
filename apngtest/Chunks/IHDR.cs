using System.Text;

namespace apngtest.Chunks
{
    public struct IHDR
    {
        public uint Width { get; set; }
        public uint Height { get; set; }
        public byte BitDepth { get; set; }
        public byte ColorType { get; set; }
        public byte Compress { get; set; }
        public byte Filter { get; set; }
        public byte Interlace { get; set; }
        public readonly byte[] Raw
        {
            get
            {
                var raw = new byte[13];
                Array.Copy(BitConverter.GetBytes(Width).Reverse().ToArray(), 0, raw, 0, 4);
                Array.Copy(BitConverter.GetBytes(Height).Reverse().ToArray(), 0, raw, 4, 4);
                raw[8] = BitDepth;
                raw[9] = ColorType;
                raw[10] = Compress;
                raw[11] = Filter;
                raw[12] = Interlace;
                return raw;
            }
        }

        public IHDR(uint width, uint height, byte bitDepth, byte colorType, byte compress, byte filter, byte interlace)
        {
            this.Width = width;
            this.Height = height;
            this.BitDepth = bitDepth;
            this.ColorType = colorType;
            this.Compress = compress;
            this.Filter = filter;
            this.Interlace = interlace;
        }

        public IHDR(byte[] data)
        {
            if (data.Length != 13)
            {
                throw new InvalidDataException($"Invalid IHDR length\nCurrent length: {data.Length}");
            }
            this.Width = BitConverter.ToUInt32(data.Take(4).Reverse().ToArray());
            this.Height = BitConverter.ToUInt32(data.Skip(4).Take(4).Reverse().ToArray());
            this.BitDepth = data[8];
            this.ColorType = data[9];
            this.Compress = data[10];
            this.Filter = data[11];
            this.Interlace = data[12];
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("IHDR: {");
            sb.AppendLine($"  Width: {this.Width}");
            sb.AppendLine($"  Height: {this.Height}");
            sb.AppendLine($"  BitDepth: {this.BitDepth}");
            sb.AppendLine($"  ColorType: {this.ColorType}");
            sb.AppendLine($"  Compress: {this.Compress}");
            sb.AppendLine($"  Filter: {this.Filter}");
            sb.AppendLine($"  Interlace: {this.Interlace}");
            sb.Append('}');
            return sb.ToString();
        }
    }
}
