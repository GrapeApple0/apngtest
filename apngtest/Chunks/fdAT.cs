using System.Text;

namespace apngtest.Chunks
{
    public struct fdAT
    {
        public uint SequenceNumber { get; set; }
        public byte[] Data { get; set; }
        public readonly byte[] Raw
        {
            get
            {
                var raw = new byte[4 + this.Data.Length];
                BitConverter.GetBytes(SequenceNumber).Reverse().ToArray().CopyTo(raw, 0);
                this.Data.CopyTo(raw, 4);
                return raw;
            }
        }

        public fdAT(uint sequenceNumber, byte[] data)
        {
            SequenceNumber = sequenceNumber;
            Data = data;
        }

        public fdAT(byte[] bytes)
        {
            if (bytes.Length < 4)
            {
                throw new InvalidDataException("Too short fdAT length");
            }
            this.SequenceNumber = BitConverter.ToUInt32(bytes.Skip(0).Take(4).Reverse().ToArray());
            this.Data = bytes.Skip(4).Take(bytes.Length).ToArray();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("fdAT: {");
            sb.AppendLine($"  Sequence Number: {SequenceNumber}");
            sb.AppendLine($"  Data: {Data}");
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}
