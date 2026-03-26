namespace HeThongChungCu.Application.Common.Interfaces.Services;

public interface IHasherService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
    string HashToken(string token);
}
