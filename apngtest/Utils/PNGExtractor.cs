using apngtest.Chunks;
using System.Diagnostics;

namespace apngtest.Utils
{
    public class PNGExtractor
    {
        public static byte[] PNGFromIDAT(fcTL fctl, PLTE plte, IHDR ihdr, pHYs phys, List<IDAT> IDATs)
        {
            var IDATTotalLength = 0;
            for (int i = 0; i < IDATs.Count; i++)
            {
                IDATTotalLength += 8 + IDATs[i].Data.Length + 4;
            }
            int total = 33 + IDATTotalLength + 12;
            if (phys.Raw != null)
            {
                total += 4 + 4 + 9 + 4;
            }
            if (plte.Data != null)
            {
                total += 4 + 4 + plte.Data.Length + 4;
            }
            var bytes = new byte[total];
            Buffer.BlockCopy(new byte[] { 0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a }, 0, bytes, 0, 8);
            //write IHDR
            Buffer.BlockCopy(BitConverter.GetBytes(ihdr.Raw.Length).Reverse().ToArray(), 0, bytes, 8, 4);
            Buffer.BlockCopy("IHDR"u8.ToArray(), 0, bytes, 12, 4);
            ihdr.Width = fctl.Width;
            ihdr.Height = fctl.Height;
            Buffer.BlockCopy(ihdr.Raw, 0, bytes, 16, 13);
            Buffer.BlockCopy(BitConverter.GetBytes(CRC32.Calculate("IHDR"u8.ToArray().Concat(ihdr.Raw).ToArray())).Reverse().ToArray(), 0, bytes, 29, 4);
            int loc = 33;
            if (phys.Raw != null)
            {
                //write pHYs
                Buffer.BlockCopy(BitConverter.GetBytes(phys.Raw.Length).Reverse().ToArray(), 0, bytes, loc, 4);
                loc += 4;
                Buffer.BlockCopy("pHYs"u8.ToArray(), 0, bytes, loc, 4);
                loc += 4;
                Buffer.BlockCopy(phys.Raw, 0, bytes, loc, 9);
                loc += 9;
                Buffer.BlockCopy(BitConverter.GetBytes(CRC32.Calculate("pHYs"u8.ToArray().Concat(phys.Raw).ToArray())).Reverse().ToArray(), 0, bytes, loc, 4);
                loc += 4;
            }
            if (plte.Data != null)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(plte.Data.Length).Reverse().ToArray(), 0, bytes, loc, 4);
                loc += 4;
                Buffer.BlockCopy("PLTE"u8.ToArray(), 0, bytes, loc, 4);
                loc += 4;
                Buffer.BlockCopy(plte.Data, 0, bytes, loc, plte.Data.Length);
                loc += plte.Data.Length;
                Buffer.BlockCopy(BitConverter.GetBytes(CRC32.Calculate("PLTE"u8.ToArray().Concat(plte.Data).ToArray())).Reverse().ToArray(), 0, bytes, loc, 4);
                loc += 4;
            }
            //write IDAT
            for (int i = 0; i < IDATs.Count; i++)
            {
                var rawLength = BitConverter.GetBytes((uint)IDATs[i].Data.Length).Reverse().ToArray();
                var rawType = "IDAT"u8.ToArray();
                var rawCRC = BitConverter.GetBytes(CRC32.Calculate([.. rawType, .. IDATs[i].Data])).Reverse().ToArray();
                Buffer.BlockCopy(rawLength, 0, bytes, loc, 4);
                loc += 4;
                Buffer.BlockCopy(rawType, 0, bytes, loc, 4);
                loc += 4;
                Debug.WriteLine($"i: {i},loc: {loc},length: {IDATs[i].Data.Length},total: {bytes.Length}");
                Buffer.BlockCopy(IDATs[i].Data, 0, bytes, loc, IDATs[i].Data.Length);
                loc += IDATs[i].Data.Length;
                Buffer.BlockCopy(rawCRC, 0, bytes, loc, 4);
                loc += 4;
            }
            //write IEND
            Buffer.BlockCopy(new byte[] { 0x0, 0x0, 0x0, 0x0 }, 0, bytes, loc, 4);
            loc += 4;
            Buffer.BlockCopy("IEND"u8.ToArray(), 0, bytes, loc, 4);
            loc += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(CRC32.Calculate("IEND"u8.ToArray())).Reverse().ToArray(), 0, bytes, loc, 4);
            return bytes;
        }

        public static byte[] PNGFromfdAT(fcTL fctl, PLTE plte, IHDR ihdr, pHYs phys, List<fdAT> fdATs)
        {
            var fdATTotalLength = 0;
            for (int i = 0; i < fdATs.Count; i++)
            {
                fdATTotalLength += fdATs[i].Data.Length + 12;
            }
            int total = 33 + fdATTotalLength + 12;
            if (phys.Raw != null)
            {
                total += 4 + 4 + 9 + 4;
            }
            if (plte.Data != null)
            {
                total += 4 + 4 + plte.Data.Length + 4;
            }
            var bytes = new byte[total];
            //write PNG signature
            Buffer.BlockCopy(new byte[] { 0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a }, 0, bytes, 0, 8);
            ihdr.Width = fctl.Width;
            ihdr.Height = fctl.Height;
            //write IHDR
            Buffer.BlockCopy(BitConverter.GetBytes(ihdr.Raw.Length).Reverse().ToArray(), 0, bytes, 8, 4);
            Buffer.BlockCopy("IHDR"u8.ToArray(), 0, bytes, 12, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(fctl.Width).Reverse().ToArray(), 0, ihdr.Raw, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(fctl.Height).Reverse().ToArray(), 0, ihdr.Raw, 4, 4);
            Buffer.BlockCopy(ihdr.Raw, 0, bytes, 16, 13);
            Buffer.BlockCopy(BitConverter.GetBytes(CRC32.Calculate("IHDR"u8.ToArray().Concat(ihdr.Raw).ToArray())).Reverse().ToArray(), 0, bytes, 29, 4);
            int loc = 33;
            if (phys.Raw != null)
            {
                //write pHYs
                Buffer.BlockCopy(BitConverter.GetBytes(phys.Raw.Length).Reverse().ToArray(), 0, bytes, loc, 4);
                loc += 4;
                Buffer.BlockCopy("pHYs"u8.ToArray(), 0, bytes, loc, 4);
                loc += 4;
                Buffer.BlockCopy(phys.Raw, 0, bytes, loc, 9);
                loc += 9;
                Buffer.BlockCopy(BitConverter.GetBytes(CRC32.Calculate("pHYs"u8.ToArray().Concat(phys.Raw).ToArray())).Reverse().ToArray(), 0, bytes, loc, 4);
                loc += 4;
            }
            if (plte.Data != null)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(plte.Data.Length).Reverse().ToArray(), 0, bytes, loc, 4);
                loc += 4;
                Buffer.BlockCopy("PLTE"u8.ToArray(), 0, bytes, loc, 4);
                loc += 4;
                Buffer.BlockCopy(plte.Data, 0, bytes, loc, plte.Data.Length);
                loc += plte.Data.Length;
                Buffer.BlockCopy(BitConverter.GetBytes(CRC32.Calculate("PLTE"u8.ToArray().Concat(plte.Data).ToArray())).Reverse().ToArray(), 0, bytes, loc, 4);
                loc += 4;
            }
            //write fdATs as IDAT
            for (int i = 0; i < fdATs.Count; i++)
            {
                var rawLength = BitConverter.GetBytes((uint)fdATs[i].Raw.Length - 4).Reverse().ToArray();
                var rawType = "IDAT"u8.ToArray();
                var rawCRC = BitConverter.GetBytes(CRC32.Calculate(rawType.Concat(fdATs[i].Raw.Skip(4).ToArray()).ToArray())).Reverse().ToArray();
                Buffer.BlockCopy(rawLength, 0, bytes, loc, 4);
                loc += 4;
                Buffer.BlockCopy(rawType, 0, bytes, loc, 4);
                loc += 4;
                Buffer.BlockCopy(fdATs[i].Raw.Skip(4).ToArray(), 0, bytes, loc, fdATs[i].Raw.Length - 4);
                loc += fdATs[i].Raw.Length - 4;
                Buffer.BlockCopy(rawCRC, 0, bytes, loc, 4);
                loc += 4;
            }
            Buffer.BlockCopy(new byte[] { 0x0, 0x0, 0x0, 0x0 }, 0, bytes, loc, 4);
            loc += 4;
            Buffer.BlockCopy("IEND"u8.ToArray(), 0, bytes, loc, 4);
            loc += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(CRC32.Calculate("IEND"u8.ToArray())).Reverse().ToArray(), 0, bytes, loc, 4);
            return bytes;
        }

    }
}
