namespace HeThongChungCu.Application.Features.BaoTriHaTang.Commands.UpdateLichBaoTri;

public class UpdateLichBaoTriCommandValidator : AbstractValidator<UpdateLichBaoTriCommand>
{
    public UpdateLichBaoTriCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("ID lịch bảo trì không hợp lệ.");

        RuleFor(x => x.TanSuatBaoTriId)
            .GreaterThan(0).WithMessage("ID tần suất bảo trì không hợp lệ.");

        RuleFor(x => x.NgayBatDau)
            .NotEmpty().WithMessage("Ngày bắt đầu không được để trống.");
    }
}
