namespace HeThongChungCu.Application.Features.BaoTriHaTang.Commands.CreateLichBaoTri;

public class CreateLichBaoTriCommandValidator : AbstractValidator<CreateLichBaoTriCommand>
{
    public CreateLichBaoTriCommandValidator()
    {
        RuleFor(x => x.ThietBiId)
            .GreaterThan(0).WithMessage("ID thiết bị không hợp lệ.");

        RuleFor(x => x.HangMucBaoTriId)
            .GreaterThan(0).WithMessage("ID hạng mục bảo trì không hợp lệ.");

        RuleFor(x => x.TanSuatBaoTriId)
            .GreaterThan(0).WithMessage("ID tần suất bảo trì không hợp lệ.");

        RuleFor(x => x.NgayBatDau)
            .NotEmpty().WithMessage("Ngày bắt đầu không được để trống.");
    }
}
