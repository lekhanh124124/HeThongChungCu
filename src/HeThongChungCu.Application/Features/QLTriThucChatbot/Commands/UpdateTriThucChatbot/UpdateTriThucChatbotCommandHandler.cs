using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLTriThucChatbot.Commands.CreateTriThucChatbot;
using HeThongChungCu.Application.Features.QLTriThucChatbot.DTOs;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.QLTriThucChatbot.Commands.UpdateTriThucChatbot;

public class UpdateTriThucChatbotCommandHandler : ICommandHandler<UpdateTriThucChatbotCommand, TriThucChatbotResponse>
{
    private readonly ITriThucChatbotCommandRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTriThucChatbotCommandHandler(
        ITriThucChatbotCommandRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TriThucChatbotResponse>> Handle(
        UpdateTriThucChatbotCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Lấy entity
        var triThuc = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (triThuc is null)
            return Result.Failure<TriThucChatbotResponse>(TriThucChatbotErrors.NotFound);

        // 2. Gọi domain method — business validation bên trong entity
        var updateResult = triThuc.Update(
            request.TieuDe,
            request.NoiDung,
            request.DanhMuc,
            request.ThuTuHienThi);

        if (updateResult.IsFailure)
            return Result.Failure<TriThucChatbotResponse>(updateResult.Errors[0]);

        // 3. Persist
        _repository.Update(triThuc);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(CreateTriThucChatbotCommandHandler.MapToResponse(triThuc));
    }
}
