using HeThongChungCu.Application.Common.Interfaces.Services;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HeThongChungCu.Infrastructure.Services;

public class ZipService : IZipService
{
    public bool IsValidZip(Stream stream)
    {
        if (stream == null || stream.Length == 0) return false;

        try
        {
            var currentPosition = stream.Position;
            if (stream.CanSeek)
            {
                stream.Position = 0;
            }

            using var archive = new ZipArchive(stream, ZipArchiveMode.Read, leaveOpen: true);
            var entries = archive.Entries;

            if (stream.CanSeek)
            {
                stream.Position = currentPosition;
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<(string FileName, MemoryStream Content)>> ExtractFilesAsync(Stream zipStream, CancellationToken cancellationToken = default)
    {
        var result = new List<(string FileName, MemoryStream Content)>();

        var currentPosition = zipStream.Position;
        if (zipStream.CanSeek)
        {
            zipStream.Position = 0;
        }

        using var archive = new ZipArchive(zipStream, ZipArchiveMode.Read, leaveOpen: true);

        var validEntries = archive.Entries
            .Where(e => !e.FullName.EndsWith("/") && !e.FullName.StartsWith("__MACOSX") && e.Length > 0)
            .ToList();

        foreach (var entry in validEntries)
        {
            var ms = new MemoryStream();
            using (var entryStream = entry.Open())
            {
                await entryStream.CopyToAsync(ms, cancellationToken);
            }
            ms.Position = 0;
            
            result.Add((entry.Name, ms));
        }

        if (zipStream.CanSeek)
        {
            zipStream.Position = currentPosition;
        }

        return result;
    }

    public async Task<MemoryStream> CreateZipAsync(IEnumerable<(string FileName, byte[] Content)> files, CancellationToken cancellationToken = default)
    {
        var zipMs = new MemoryStream();

        using (var archive = new ZipArchive(zipMs, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (var file in files)
            {
                var entry = archive.CreateEntry(file.FileName, CompressionLevel.Optimal);
                using var entryStream = entry.Open();
                await entryStream.WriteAsync(file.Content, 0, file.Content.Length, cancellationToken);
            }
        }

        zipMs.Position = 0;
        return zipMs;
    }
}
