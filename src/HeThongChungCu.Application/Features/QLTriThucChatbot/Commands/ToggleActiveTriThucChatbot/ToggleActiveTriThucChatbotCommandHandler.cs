using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.QLTriThucChatbot.Commands.ToggleActiveTriThucChatbot;

public class ToggleActiveTriThucChatbotCommandHandler : ICommandHandler<ToggleActiveTriThucChatbotCommand, bool>
{
    private readonly ITriThucChatbotCommandRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ToggleActiveTriThucChatbotCommandHandler(
        ITriThucChatbotCommandRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(
        ToggleActiveTriThucChatbotCommand request,
        CancellationToken cancellationToken)
    {
        var triThuc = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (triThuc is null)
            return Result.Failure<bool>(TriThucChatbotErrors.NotFound);

        if (request.Activate)
            triThuc.Activate();
        else
            triThuc.Deactivate();

        _repository.Update(triThuc);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}
