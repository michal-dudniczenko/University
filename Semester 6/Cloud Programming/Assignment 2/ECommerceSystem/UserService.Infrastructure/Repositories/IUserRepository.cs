using UserService.Domain.Entities;

namespace UserService.Infrastructure.Repositories;

public interface IUserRepository
{
    Task<User?> GetUserById(Guid id);

    Task<User?> GetUserByEmail(string email);

    Task<List<User>> GetAllUsers();

    Task AddUser(User user);

    Task<User?> GetUserLogin(string email, string password);

    Task<string> GetSecretKey();
}
