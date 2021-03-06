﻿using System.IO;

namespace IoTBackend.Infrastructure.Core.Providers
{
    public interface IStreamReaderProvider
    {
        StreamReader GetReader(Stream stream);
    }

    public class StreamReaderProvider : IStreamReaderProvider
    {
        public StreamReader GetReader(Stream stream)
        {
            return new StreamReader(stream);
        }
    }
}