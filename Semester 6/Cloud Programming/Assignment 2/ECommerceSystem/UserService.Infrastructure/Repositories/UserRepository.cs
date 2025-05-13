using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    private readonly static string secretKey = "super-secret-key123";

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserById(Guid id)
    {
        return await _context.Users.FirstOrDefaultAsync(user => user.Id == id);
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(user => user.Email == email);
    }

    public async Task<string?> GetUserEmail(Guid id)
    {
        return await _context.Users
            .Where(user => user.Id == id)
            .Select(user => user.Email)
            .FirstOrDefaultAsync();
    }

    public async Task<List<User>> GetAllUsers()
    {
        return await _context.Users.ToListAsync();
    }
    public async Task AddUser(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task<User?> GetUserLogin(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
            return null;

        var hasher = new PasswordHasher<object>();
        var result = hasher.VerifyHashedPassword(new object(), user.PasswordHash, password);

        return result == PasswordVerificationResult.Success ? user : null;
    }

    public async Task<string> GetSecretKey()
    {
        return secretKey;
    }

}
