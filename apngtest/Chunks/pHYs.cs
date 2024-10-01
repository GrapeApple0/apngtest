using System.Text;

namespace apngtest.Chunks
{
    public struct pHYs
    {
        public uint X { get; set; }
        public uint Y { get; set; }
        public byte Unit { get; set; }
        public readonly byte[] Raw {
            get
            {
                var bytes = new byte[9];
                BitConverter.GetBytes(X).Reverse().ToArray().CopyTo(bytes, 0);
                BitConverter.GetBytes(Y).Reverse().ToArray().CopyTo(bytes, 4);
                bytes[8] = Unit;
                return bytes;
            }
        }

        public pHYs(uint x, uint y, byte unit)
        {
            this.X = x;
            this.Y = y;
            this.Unit = unit;
        }

        public pHYs(byte[] bytes)
        {
            if (bytes.Length != 9)
            {
                throw new InvalidDataException($"Invalid pHYs length\nCurrent length: {bytes.Length}");
            }
            this.X = BitConverter.ToUInt32(bytes.Skip(0).Take(4).Reverse().ToArray());
            this.Y = BitConverter.ToUInt32(bytes.Skip(4).Take(4).Reverse().ToArray());
            this.Unit = bytes[8];
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("pHYs: {");
            sb.AppendLine($"  X: {this.X}");
            sb.AppendLine($"  Y: {this.Y}");
            sb.AppendLine($"  Unit: {this.Unit}");
            sb.Append('}');
            return sb.ToString();
        }
    }
}
