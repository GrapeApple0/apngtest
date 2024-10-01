using System.Text;

namespace apngtest.Chunks
{
    public struct fcTL
    {
        public uint SequenceNumber { get; set; }
        public uint Width { get; set; }
        public uint Height { get; set; }
        public uint XOffset { get; set; }
        public uint YOffset { get; set; }
        public ushort DelayNum { get; set; }
        public ushort DelayDen { get; set; }
        public byte DisposeOp { get; set; }
        public byte BlendOp { get; set; }
        public readonly byte[] Raw {
            get
            {
                var bytes = new byte[26];
                BitConverter.GetBytes(SequenceNumber).Reverse().ToArray().CopyTo(bytes, 0);
                BitConverter.GetBytes(Width).Reverse().ToArray().CopyTo(bytes, 4);
                BitConverter.GetBytes(Height).Reverse().ToArray().CopyTo(bytes, 8);
                BitConverter.GetBytes(XOffset).Reverse().ToArray().CopyTo(bytes, 12);
                BitConverter.GetBytes(YOffset).Reverse().ToArray().CopyTo(bytes, 16);
                BitConverter.GetBytes(DelayNum).Reverse().ToArray().CopyTo(bytes, 20);
                BitConverter.GetBytes(DelayDen).Reverse().ToArray().CopyTo(bytes, 22);
                bytes[24] = DisposeOp;
                bytes[25] = BlendOp;
                return bytes;
            } 
        }

        public fcTL(uint sequenceNumber, uint width, uint height, uint xOffset, uint yOffset, ushort delayNum, ushort delayDen, byte disposeOp, byte blendOp)
        {
            this.SequenceNumber = sequenceNumber;
            this.Width = width;
            this.Height = height;
            this.XOffset = xOffset;
            this.YOffset = yOffset;
            this.DelayNum = delayNum;
            this.DelayDen = delayDen;
            this.DisposeOp = disposeOp;
            this.BlendOp = blendOp;
        }

        public fcTL(byte[] bytes)
        {
            if (bytes.Length != 26)
            {
                throw new InvalidDataException($"Invalid fcTL length\nCurrent length: {bytes.Length}");
            }
            this.SequenceNumber = BitConverter.ToUInt32(bytes.Take(4).Reverse().ToArray());
            this.Width = BitConverter.ToUInt32(bytes.Skip(4).Take(4).Reverse().ToArray());
            this.Height = BitConverter.ToUInt32(bytes.Skip(8).Take(4).Reverse().ToArray());
            this.XOffset = BitConverter.ToUInt32(bytes.Skip(12).Take(4).Reverse().ToArray());
            this.YOffset = BitConverter.ToUInt32(bytes.Skip(16).Take(4).Reverse().ToArray());
            this.DelayNum = BitConverter.ToUInt16(bytes.Skip(20).Take(2).Reverse().ToArray());
            this.DelayDen = BitConverter.ToUInt16(bytes.Skip(22).Take(2).Reverse().ToArray());
            this.DisposeOp = bytes[24];
            this.BlendOp = bytes[25];
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("fcTL: {");
            sb.AppendLine($"  SequenceNumber: {SequenceNumber}");
            sb.AppendLine($"  Width: {Width}");
            sb.AppendLine($"  Height: {Height}");
            sb.AppendLine($"  XOffset: {XOffset}");
            sb.AppendLine($"  YOffset: {YOffset}");
            sb.AppendLine($"  DelayNum: {DelayNum}");
            sb.AppendLine($"  DelayDen: {DelayDen}");
            sb.AppendLine($"  DisposeOp: {DisposeOp}");
            sb.AppendLine($"  BlendOp: {BlendOp}");
            sb.Append('}');
            return sb.ToString();
        }
    }
}
