using Microsoft.AspNetCore.Identity;
using UserService.Infrastructure.Repositories;

namespace UserService.Infrastructure.Helpers;

public class TokenService : ITokenService
{
    private readonly IUserRepository _userRepository;

    public TokenService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<string> GenerateToken(string userEmail)
    {
        var hasher = new PasswordHasher<object>();

        var userId = (await _userRepository.GetUserByEmail(userEmail)).Id;

        var tokenExpireUnixTime = ((DateTimeOffset)DateTime.UtcNow.AddSeconds(30)).ToUnixTimeSeconds();
        var token = $"{userId}.{tokenExpireUnixTime}.";

        token += hasher.HashPassword(new object(), $"{userId}{tokenExpireUnixTime}{_userRepository.GetSecretKey()}");

        return token;
    }
}