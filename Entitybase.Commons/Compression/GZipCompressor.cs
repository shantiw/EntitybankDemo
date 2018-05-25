using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XData.IO.Compression
{
    public class GZipCompressor
    {
        public string Compress(string str)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(str);
            byte[] bytes = Compress(buffer);
            return Convert.ToBase64String(bytes);
        }

        public string Decompress(string base64String)
        {
            byte[] bytes = Convert.FromBase64String(base64String);
            byte[] buffer = Decompress(bytes);
            return Encoding.UTF8.GetString(buffer);
        }

        public byte[] Compress(byte[] data)
        {
            MemoryStream originalStream = new MemoryStream(data);
            MemoryStream compressedStream = new MemoryStream();
            using (GZipStream compressionStream = new GZipStream(compressedStream, CompressionMode.Compress))
            {
                originalStream.CopyTo(compressionStream);
            }
            return compressedStream.ToArray();
        }

        public byte[] Decompress(byte[] compressed)
        {
            MemoryStream originalStream = new MemoryStream(compressed);
            MemoryStream decompressedStream = new MemoryStream();
            using (GZipStream decompressionStream = new GZipStream(originalStream, CompressionMode.Decompress))
            {
                decompressionStream.CopyTo(decompressedStream);
            }
            return decompressedStream.ToArray();
        }


    }
}
