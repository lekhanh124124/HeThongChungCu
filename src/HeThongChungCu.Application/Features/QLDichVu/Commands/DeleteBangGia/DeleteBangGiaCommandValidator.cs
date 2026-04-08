namespace HeThongChungCu.Application.Features.QLDichVu.Commands.DeleteBangGia;

public class DeleteBangGiaCommandValidator : AbstractValidator<DeleteBangGiaCommand>
{
    public DeleteBangGiaCommandValidator()
    {
        RuleFor(x => x.DichVuId)
            .NotEmpty().WithMessage("DichVuId không được để trống.");

        RuleFor(x => x.Ids)
            .NotEmpty().WithMessage("Danh sách ID không được để trống.")
            .Must(x => x.All(id => id > 0)).WithMessage("Id không hợp lệ.");
    }
}