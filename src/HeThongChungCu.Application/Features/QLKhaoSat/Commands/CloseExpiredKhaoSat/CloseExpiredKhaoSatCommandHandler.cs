using System;
using System.Threading;
using System.Threading.Tasks;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Domain.Common;

namespace HeThongChungCu.Application.Features.QLKhaoSat.Commands.CloseExpiredKhaoSat;

public class CloseExpiredKhaoSatCommandHandler : ICommandHandler<CloseExpiredKhaoSatCommand, int>
{
    private readonly IKhaoSatCommandRepository _khaoSatRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CloseExpiredKhaoSatCommandHandler(
        IKhaoSatCommandRepository khaoSatRepository,
        IUnitOfWork unitOfWork)
    {
        _khaoSatRepository = khaoSatRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> Handle(CloseExpiredKhaoSatCommand command, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var expiredCampaigns = await _khaoSatRepository.GetExpiredCampaignsAsync(now, cancellationToken);

        if (expiredCampaigns.Count == 0)
        {
            return Result.Success(0);
        }

        int closedCount = 0;
        foreach (var campaign in expiredCampaigns)
        {
            var endResult = campaign.EndCampaign();
            if (endResult.IsSuccess)
            {
                _khaoSatRepository.Update(campaign);
                closedCount++;
            }
        }

        if (closedCount > 0)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Success(closedCount);
    }
}
