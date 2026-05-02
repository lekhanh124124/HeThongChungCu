namespace HeThongChungCu.Application.Features.QLChiSoTieuThu.Commands.UploadChiSoImagesBatch;

public class UploadChiSoImagesBatchCommand : ICommand<int>
{
    public Stream ZipStream { get; }
    public string FileName { get; }

    public UploadChiSoImagesBatchCommand(Stream zipStream, string fileName)
    {
        ZipStream = zipStream;
        FileName = fileName;
    }
}
