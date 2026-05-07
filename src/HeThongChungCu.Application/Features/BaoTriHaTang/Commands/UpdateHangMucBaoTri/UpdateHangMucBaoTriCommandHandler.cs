using System.Text.Json;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;
using HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetHangMucBaoTriById;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Commands.UpdateHangMucBaoTri;

public class UpdateHangMucBaoTriCommandHandler : ICommandHandler<UpdateHangMucBaoTriCommand, HangMucBaoTriDetailResponse>
{
    private readonly IThietBiCommandRepository _thietBiRepository;
    private readonly IHangMucBaoTriQueryRepository _queryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateHangMucBaoTriCommandHandler(
        IThietBiCommandRepository thietBiRepository,
        IHangMucBaoTriQueryRepository queryRepository,
        IUnitOfWork unitOfWork)
    {
        _thietBiRepository = thietBiRepository;
        _queryRepository = queryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<HangMucBaoTriDetailResponse>> Handle(UpdateHangMucBaoTriCommand request, CancellationToken cancellationToken)
    {
        var hangMuc = await _thietBiRepository.GetHangMucByIdAsync(request.Id, cancellationToken);
        if (hangMuc is null || hangMuc.IsDeleted)
            return Result.Failure<HangMucBaoTriDetailResponse>(BaoTriHaTangErrors.HangMucNotFoundById(request.Id));

        var checklistJson = JsonSerializer.Serialize(request.ChecklistTieuChuan ?? new());

        hangMuc.Update(
            request.TenHangMuc,
            request.MoTa,
            request.ThoiGianUocTinhPhut,
            request.ChiPhiUocTinh,
            checklistJson);

        _thietBiRepository.UpdateHangMuc(hangMuc);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var spec = new GetHangMucBaoTriByIdSpecification(hangMuc.Id);
        var response = await _queryRepository.GetByIdAsync(spec, cancellationToken);

        return response is not null 
            ? Result.Success(response)
            : Result.Failure<HangMucBaoTriDetailResponse>(BaoTriHaTangErrors.HangMucNotFoundById(hangMuc.Id));
    }
}
