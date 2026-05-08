using System.Threading;
using System.Threading.Tasks;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.QLKhaoSat.DTOs;
using HeThongChungCu.Application.Features.QLKhaoSat.Queries.GetKhaoSatById;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.QLKhaoSat.Commands.PublishKhaoSat;

public class PublishKhaoSatCommandHandler : ICommandHandler<PublishKhaoSatCommand, KhaoSatResponse>
{
    private readonly IKhaoSatCommandRepository _khaoSatCommandRepository;
    private readonly IKhaoSatQueryRepository _khaoSatQueryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PublishKhaoSatCommandHandler(
        IKhaoSatCommandRepository khaoSatCommandRepository,
        IKhaoSatQueryRepository khaoSatQueryRepository,
        IUnitOfWork unitOfWork)
    {
        _khaoSatCommandRepository = khaoSatCommandRepository;
        _khaoSatQueryRepository = khaoSatQueryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<KhaoSatResponse>> Handle(PublishKhaoSatCommand command, CancellationToken cancellationToken)
    {
        // 1. Fetch survey with questions to check publish capability
        var khaoSat = await _khaoSatCommandRepository.GetByIdWithQuestionsAndChoicesAsync(command.Id, cancellationToken);
        if (khaoSat == null)
            return Result.Failure<KhaoSatResponse>(KhaoSatErrors.NotFoundById(command.Id));

        // 2. Perform domain transition
        var publishResult = khaoSat.PublicCampaign();
        if (publishResult.IsFailure)
            return publishResult.Errors;

        // 3. Persistence
        _khaoSatCommandRepository.Update(khaoSat);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 4. Query full response details
        var response = await _khaoSatQueryRepository.GetByIdAsync(new GetKhaoSatByIdSpecification(khaoSat.Id), cancellationToken);

        return response != null
            ? Result.Success<KhaoSatResponse>(response)
            : Result.Failure<KhaoSatResponse>(KhaoSatErrors.NotFound);
    }
}
