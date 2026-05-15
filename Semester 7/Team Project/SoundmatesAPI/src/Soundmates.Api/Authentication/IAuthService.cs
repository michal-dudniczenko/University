namespace Soundmates.Api.Authentication;

internal interface IAuthService
{
    string GetPasswordHash(string password);
    string GetRefreshTokenHash(string refreshToken);
    string GenerateAccessToken(Guid userId);
    string GenerateRefreshToken(Guid userId);
    bool VerifyPasswordHash(string password, string passwordHash);
    bool VerifyRefreshTokenHash(string refreshToken, string refreshTokenHash);
}
