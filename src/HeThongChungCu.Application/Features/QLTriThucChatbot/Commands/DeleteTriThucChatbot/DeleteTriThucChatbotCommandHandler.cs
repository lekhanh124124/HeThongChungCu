using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Application.Features.QLTriThucChatbot.Commands.DeleteTriThucChatbot;

public class DeleteTriThucChatbotCommandHandler : ICommandHandler<DeleteTriThucChatbotCommand, bool>
{
    private readonly ITriThucChatbotCommandRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTriThucChatbotCommandHandler(
        ITriThucChatbotCommandRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(
        DeleteTriThucChatbotCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Ids == null || !request.Ids.Any())
            return Result.Failure<bool>(new Error("TriThucChatbot.DeleteEmpty", "Danh sách ID không được rỗng."));

        // 1. Load tất cả record trước
        var records = new List<TriThucChatbot>();
        foreach (var id in request.Ids)
        {
            var triThuc = await _repository.GetByIdAsync(id, cancellationToken);
            if (triThuc is not null)
                records.Add(triThuc);
        }

        // 2. Fail-fast: không cho xóa nếu bất kỳ record nào đang active
        if (records.Any(r => r.IsActive))
            return Result.Failure<bool>(TriThucChatbotErrors.CannotDeleteActive);

        // 3. Xóa tất cả (soft-delete qua EF interceptor)
        foreach (var triThuc in records)
            _repository.Remove(triThuc);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(true);
    }
}
