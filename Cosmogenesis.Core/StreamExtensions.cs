using System;
using System.IO;

namespace Cosmogenesis.Core
{
    static class StreamExtensions
    {
        public static Span<byte> ToSpan(this Stream stream)
        {
            if (stream is MemoryStream ms)
            {
                return ms.GetDataSpan();
            }

            var backupStream = new MemoryStream((int)stream.Length);
            stream.CopyTo(backupStream);
            return backupStream.GetDataSpan();
        }
    }
}
