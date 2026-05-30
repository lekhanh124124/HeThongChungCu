using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLTriThucChatbot.DTOs;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Application.Features.QLTriThucChatbot.Commands.CreateTriThucChatbot;

public class CreateTriThucChatbotCommandHandler : ICommandHandler<CreateTriThucChatbotCommand, TriThucChatbotResponse>
{
    private readonly ITriThucChatbotCommandRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTriThucChatbotCommandHandler(
        ITriThucChatbotCommandRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TriThucChatbotResponse>> Handle(
        CreateTriThucChatbotCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Tạo domain entity qua factory method (có validation)
        var createResult = TriThucChatbot.CreateTriThucChatbot(
            request.TieuDe,
            request.NoiDung,
            request.DanhMuc,
            request.ThuTuHienThi);

        if (createResult.IsFailure)
            return Result.Failure<TriThucChatbotResponse>(createResult.Errors[0]);

        var triThuc = createResult.Value;

        // 2. Persist
        _repository.Add(triThuc);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 3. Map và trả về response
        return Result.Success(MapToResponse(triThuc));
    }

    internal static TriThucChatbotResponse MapToResponse(TriThucChatbot t) => new()
    {
        Id           = t.Id,
        TieuDe       = t.TieuDe,
        NoiDung      = t.NoiDung,
        DanhMuc      = t.DanhMuc,
        ThuTuHienThi = t.ThuTuHienThi,
        IsActive     = t.IsActive,
        IsSynced     = t.IsSynced,
        LastSyncedAt = t.LastSyncedAt,
        CreatedAt    = t.CreatedAt,
        UpdatedAt    = t.ModifiedAt,
        CreatedBy    = t.CreatedBy.ToString()
    };
}
