using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HeThongChungCu.Application.Features.QLChiSoTieuThu.Commands.UploadChiSoImagesBatch;

public class UploadChiSoImagesBatchCommandHandler : ICommandHandler<UploadChiSoImagesBatchCommand, int>
{
    private readonly IChiSoTieuThuCommandRepository _chiSoRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IZipService _zipService;
    private readonly ITepTaiLieuCommandRepository _tepTaiLieuRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public UploadChiSoImagesBatchCommandHandler(
        IChiSoTieuThuCommandRepository chiSoRepository,
        IFileStorageService fileStorageService,
        IZipService zipService,
        ITepTaiLieuCommandRepository tepTaiLieuRepository,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _chiSoRepository = chiSoRepository;
        _fileStorageService = fileStorageService;
        _zipService = zipService;
        _tepTaiLieuRepository = tepTaiLieuRepository;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> Handle(UploadChiSoImagesBatchCommand request, CancellationToken cancellationToken)
    {
        var extractedFiles = await _zipService.ExtractFilesAsync(request.ZipStream, cancellationToken);
        if (extractedFiles.Count == 0)
        {
            return Result.Failure<int>(new Error("Zip.EmptyImages", "Không tìm thấy file ảnh nào trong file zip."));
        }

        var matchingEntries = await GetMatchingEntriesAsync(extractedFiles, cancellationToken);
        if (matchingEntries.Count == 0)
        {
            return Result.Failure<int>(new Error("Zip.NoMatches", "Không tìm thấy chỉ số tiêu thụ nào khớp hoặc các chỉ số đã bị khóa."));
        }

        var uploadData = PrepareUploadData(matchingEntries);

        var uploadResult = await _fileStorageService.UploadFilesAsync(
            uploadData,
            FileCategory.MeterReading,
            cancellationToken);

        if (uploadResult.IsFailure)
        {
            return Result.Failure<int>(uploadResult.Errors.First());
        }

        int savedCount = await SaveTaiLieuAndMapChiSoAsync(matchingEntries, uploadResult.Value, uploadData, cancellationToken);

        return Result.Success(savedCount);
    }

    private async Task<List<(string OriginalName, MemoryStream Stream, ChiSoTieuThu ChiSo)>> GetMatchingEntriesAsync(
        List<(string FileName, MemoryStream Content)> entries,
        CancellationToken cancellationToken)
    {
        var maTraCuus = entries
            .Select(e => Path.GetFileNameWithoutExtension(e.FileName))
            .Distinct()
            .ToList();

        var chiSos = await _chiSoRepository.GetByMaTraCuusAsync(maTraCuus, cancellationToken);
        var chiSoMap = chiSos.ToDictionary(x => x.MaTraCuu!, x => x);

        var matchingEntries = new List<(string OriginalName, MemoryStream Stream, ChiSoTieuThu ChiSo)>();

        foreach (var (FileName, Content) in entries)
        {
            var maTraCuu = Path.GetFileNameWithoutExtension(FileName);
            if (chiSoMap.TryGetValue(maTraCuu, out var chiSo) && chiSo.TrangThaiChiSoId != TrangThaiChiSo.Locked)
            {
                matchingEntries.Add((FileName, Content, chiSo));
            }
        }

        return matchingEntries;
    }

    private List<(Stream Stream, string FileName, string ContentType)> PrepareUploadData(
        List<(string OriginalName, MemoryStream Stream, ChiSoTieuThu ChiSo)> matchingEntries)
    {
        var uploadData = new List<(Stream Stream, string FileName, string ContentType)>();

        foreach (var match in matchingEntries)
        {
            var extension = Path.GetExtension(match.OriginalName).ToLowerInvariant();
            var uniqueFileName = _fileStorageService.UrlNormalization(
                $"{Guid.NewGuid():N}{extension}",
                _dateTimeProvider.UtcNow.DateTime);

            var contentType = extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".heic" => "image/heic",
                _ => "application/octet-stream"
            };

            uploadData.Add((match.Stream, uniqueFileName, contentType));
        }

        return uploadData;
    }

    private async Task<int> SaveTaiLieuAndMapChiSoAsync(
        List<(string OriginalName, MemoryStream Stream, ChiSoTieuThu ChiSo)> matchingEntries,
        List<string> fileUrls,
        List<(Stream Stream, string FileName, string ContentType)> uploadData,
        CancellationToken cancellationToken)
    {
        var tepTaiLieus = new List<TepTaiLieu>();

        for (int i = 0; i < fileUrls.Count; i++)
        {
            var url = fileUrls[i];
            var originalName = matchingEntries[i].OriginalName;
            var contentType = uploadData[i].ContentType;
            var streamSize = matchingEntries[i].Stream.Length;

            var tepTaiLieu = new TepTaiLieu(originalName, url, streamSize, contentType);
            tepTaiLieu.MarkAsUsed();
            tepTaiLieus.Add(tepTaiLieu);
        }

        await _tepTaiLieuRepository.AddRangeAsync(tepTaiLieus, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        for (int i = 0; i < tepTaiLieus.Count; i++)
        {
            var chiSo = matchingEntries[i].ChiSo;
            var tepTaiLieu = tepTaiLieus[i];
            chiSo.SetAnhDongHo(tepTaiLieu.Id);
            _chiSoRepository.Update(chiSo);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return tepTaiLieus.Count;
    }
}
