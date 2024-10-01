namespace apngtest
{
    public class Frame
    {
        public byte[] Data { get; }
        public uint Delay { get; }
        public uint Width { get; }
        public uint Height { get; }
        public uint XOffset { get; }
        public uint YOffset { get; }

        public Frame(byte[] data, uint delay, uint width, uint height, uint xOffset, uint yOffset)
        {
            Data = data;
            Delay = delay;
            Width = width;
            Height = height;
            XOffset = xOffset;
            YOffset = yOffset;
        }
    }
}
