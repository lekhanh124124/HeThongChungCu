using System.Text.Json;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Commands;
using HeThongChungCu.Application.Common.Interfaces.Persistences.Queries;
using HeThongChungCu.Application.Features.BaoTriHaTang.DTOs;
using HeThongChungCu.Application.Features.BaoTriHaTang.Queries.GetHangMucBaoTriById;
using HeThongChungCu.Domain.Entities;
using HeThongChungCu.Domain.Errors;

namespace HeThongChungCu.Application.Features.BaoTriHaTang.Commands.CreateHangMucBaoTri;

public class CreateHangMucBaoTriCommandHandler : ICommandHandler<CreateHangMucBaoTriCommand, HangMucBaoTriDetailResponse>
{
    private readonly IThietBiCommandRepository _thietBiRepository;
    private readonly IHangMucBaoTriQueryRepository _queryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateHangMucBaoTriCommandHandler(
        IThietBiCommandRepository thietBiRepository,
        IHangMucBaoTriQueryRepository queryRepository,
        IUnitOfWork unitOfWork)
    {
        _thietBiRepository = thietBiRepository;
        _queryRepository = queryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<HangMucBaoTriDetailResponse>> Handle(CreateHangMucBaoTriCommand request, CancellationToken cancellationToken)
    {
        var exists = await _thietBiRepository.MaHangMucExistsAsync(request.MaHangMuc, cancellationToken);
        if (exists)
            return Result.Failure<HangMucBaoTriDetailResponse>(BaoTriHaTangErrors.MaHangMucAlreadyExists);

        var checklistJson = JsonSerializer.Serialize(request.ChecklistTieuChuan ?? new());

        var hangMuc = HangMucBaoTri.Create(
            request.MaHangMuc,
            request.TenHangMuc,
            request.MoTa,
            request.ThoiGianUocTinhPhut,
            request.ChiPhiUocTinh,
            checklistJson);

        await _thietBiRepository.AddHangMucAsync(hangMuc, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var spec = new GetHangMucBaoTriByIdSpecification(hangMuc.Id);
        var response = await _queryRepository.GetByIdAsync(spec, cancellationToken);

        return response is not null 
            ? Result.Success(response)
            : Result.Failure<HangMucBaoTriDetailResponse>(BaoTriHaTangErrors.HangMucNotFoundById(hangMuc.Id));
    }
}
