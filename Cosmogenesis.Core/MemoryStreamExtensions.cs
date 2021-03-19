using System;
using System.IO;

namespace Cosmogenesis.Core
{
    static class MemoryStreamExtensions
    {
        /// <summary>
        /// Will return the MemoryStream's buffer directly, or a copy of it.
        /// </summary>
        public static ArraySegment<byte> GetDataBuffer(this MemoryStream memoryStream)
        {
            if (memoryStream is null)
            {
                throw new ArgumentNullException(nameof(memoryStream));
            }

            return memoryStream.TryGetBuffer(out var buf)
                ? buf
                : new ArraySegment<byte>(memoryStream.ToArray());
        }

        /// <summary>
        /// Will return the MemoryStream's buffer directly, or a copy of it.
        /// </summary>
        public static Span<byte> GetDataSpan(this MemoryStream memoryStream)
        {
            if (memoryStream is null)
            {
                throw new ArgumentNullException(nameof(memoryStream));
            }

            return memoryStream.TryGetBuffer(out var buf)
                ? buf.AsSpan()
                : memoryStream.ToArray().AsSpan();
        }
    }
}
