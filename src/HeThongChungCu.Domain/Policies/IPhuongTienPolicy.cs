using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Domain.Policies;

public interface IPhuongTienPolicy
{
    void ValidateAddThe(string maThe, PhuongTien phuongTien);
}
