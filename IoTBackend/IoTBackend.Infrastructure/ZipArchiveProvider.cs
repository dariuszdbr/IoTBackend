﻿using System.IO;
using System.IO.Compression;

namespace IoTBackend.Infrastructure
{
    public interface IZipArchiveProvider
    {
        ZipArchive GetZipArchive(Stream stream);
    }

    public class ZipArchiveProvider : IZipArchiveProvider
    {
        public ZipArchive GetZipArchive(Stream stream)
        {
            return new ZipArchive(stream);
        }
    }
}