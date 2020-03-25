using System;
using System.Collections.Generic;
using System.IO;
using Wolfpack.Core.Interfaces;
using System.Linq;

namespace Wolfpack.Core.Artifacts
{
    public static class ArtifactExtensions
    {
        public static IArtifactFormatter Find(this IEnumerable<IArtifactFormatter> formatters, string contentType)
        {
            var formatter = formatters.FirstOrDefault(f => 
                string.Compare(f.ContentType, contentType, StringComparison.OrdinalIgnoreCase) == 0);

            if (formatter == null)
                throw new ApplicationException(string.Format("No artifact formatter available for content type '{0}'", contentType));

            return formatter;
        }

        public static void ChunkedOperation(this Stream stream, int chunkSize, Action<byte[], int> action)
        {
            var buffer = new byte[chunkSize];

            long position = 0;

            while (position < stream.Length)
            {
                var bufferSize = (int)Math.Min(stream.Length - position, chunkSize);
                stream.Read(buffer, 0, bufferSize);
                position += bufferSize;

                action(buffer, bufferSize);
            }
        }
    }
}