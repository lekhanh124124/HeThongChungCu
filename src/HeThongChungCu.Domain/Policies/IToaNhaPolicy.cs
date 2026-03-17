using HeThongChungCu.Domain.Entities;

namespace HeThongChungCu.Domain.Policies;

public interface IToaNhaPolicy
{
    void ValidateAddTang(string maTang, ToaNha toaNha);
}
