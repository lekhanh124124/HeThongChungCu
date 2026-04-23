using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Common.Messaging;
using HeThongChungCu.Application.Features.YeuCauThiCong.DTOs;
using HeThongChungCu.Application.Features.YeuCauThiCong.Queries.GetYeuCauThiCongById;
using HeThongChungCu.Domain.Common;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.YeuCauThiCong.Commands.AddNhanSuThiCong;

public class AddNhanSuThiCongCommandHandler : ICommandHandler<AddNhanSuThiCongCommand, YeuCauThiCongResponse>
{
    private readonly IYeuCauThiCongCommandRepository _yctcCommandRepository;
    private readonly IYeuCauThiCongQueryRepository _yctcQueryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddNhanSuThiCongCommandHandler(
        IYeuCauThiCongCommandRepository yctcCommandRepository,
        IYeuCauThiCongQueryRepository yctcQueryRepository,
        IUnitOfWork unitOfWork)
    {
        _yctcCommandRepository = yctcCommandRepository;
        _yctcQueryRepository = yctcQueryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<YeuCauThiCongResponse>> Handle(AddNhanSuThiCongCommand command, CancellationToken cancellationToken)
    {
        var yctc = await _yctcCommandRepository.GetByIdAsync(command.Id, cancellationToken);
        if (yctc == null)
            return YeuCauThiCongErrors.NotFound;

        var result = yctc.AddNhanSu(command.HoTen, command.SoCCCD, command.SoDienThoai, command.VaiTro, command.GhiChu);
        if (result.IsFailure)
            return result.Errors;

        _yctcCommandRepository.Update(yctc);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = await _yctcQueryRepository.GetByIdAsync(new GetYeuCauThiCongByIdSpecification(yctc.Id), cancellationToken);
        return response != null ? response : YeuCauThiCongErrors.NotFound;
    }
}
