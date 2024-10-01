using System.Text;

namespace apngtest.Chunks
{
    public struct acTL
    {
        public uint NumFrames { get; set; }
        public uint NumPlays { get; set; }
        public readonly byte[] Raw {
            get
            {
                byte[] raw = new byte[8];
                Buffer.BlockCopy(BitConverter.GetBytes(NumFrames), 0, raw, 0, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(NumPlays), 0, raw, 4, 4);
                return raw;
            }
        }

        public acTL(uint numFrames, uint numPlays) 
        {
            this.NumFrames = numFrames;
            this.NumPlays = numPlays;
        }

        public acTL(byte[] bytes)
        {
            if (bytes.Length != 8)
            {
                throw new InvalidDataException($"Invalid acTL length\nCurrent length: {bytes.Length}");
            }
            this.NumFrames = BitConverter.ToUInt32(bytes.Take(4).Reverse().ToArray());
            this.NumPlays = BitConverter.ToUInt32(bytes.Skip(4).Take(4).Reverse().ToArray());
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("acTL: {");
            sb.AppendLine($"  NumFrames: {this.NumFrames}");
            sb.AppendLine($"  NumPlays: {this.NumPlays}");
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}
