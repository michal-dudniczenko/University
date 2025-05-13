namespace UserService.Infrastructure.Helpers;

public interface ITokenService
{
    Task<string> GenerateToken(string userEmail);
    Task<bool> ValidateToken(string token);
}
