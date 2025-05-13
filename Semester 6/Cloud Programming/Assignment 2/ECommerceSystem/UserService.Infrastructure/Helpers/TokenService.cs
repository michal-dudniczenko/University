using Azure.Core;
using Common.Domain.Bus;
using Common.Domain.Events.UserService;
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

        var secretKey = await _userRepository.GetSecretKey();

        var userId = (await _userRepository.GetUserByEmail(userEmail)).Id;

        var tokenExpireUnixTime = ((DateTimeOffset)DateTime.UtcNow.AddSeconds(3600)).ToUnixTimeSeconds();
        var token = $"{userId}.{tokenExpireUnixTime}.";

        token += hasher.HashPassword(new object(), $"{userId}{tokenExpireUnixTime}{secretKey}");

        return token;
    }

    public async Task<bool> ValidateToken(string token)
    {
        var hasher = new PasswordHasher<object>();

        var secretKey = await _userRepository.GetSecretKey();

        var tokenParts = token.Split('.');

        /* debugging
        if (tokenParts.Length != 3)
        {
            Console.WriteLine("wrong length");
        }

        if (hasher.VerifyHashedPassword(new object(), tokenParts[2], $"{tokenParts[0]}{tokenParts[1]}{secretKey}") != PasswordVerificationResult.Success)
        {
            Console.WriteLine("wrong hash");
        }

        if (long.Parse(tokenParts[1]) <= DateTimeOffset.UtcNow.ToUnixTimeSeconds())
        {
            Console.WriteLine("expired");
        }

        if ((await _userRepository.GetUserById(Guid.Parse(tokenParts[0]))) == null)
        {
            Console.WriteLine("user not found");
        }
        */

        if (
            (tokenParts.Length != 3) ||
            (hasher.VerifyHashedPassword(new object(), tokenParts[2], $"{tokenParts[0]}{tokenParts[1]}{secretKey}") != PasswordVerificationResult.Success) ||
            (long.Parse(tokenParts[1]) <= DateTimeOffset.UtcNow.ToUnixTimeSeconds()) ||
            ((await _userRepository.GetUserById(Guid.Parse(tokenParts[0]))) == null)
        )
        {
            return false;
        }
        return true;
    }
}