namespace UserService.Domain.DTO;

public class UserDto
{
    public required Guid Id { get; set; }
    public required string Email { get; set; }
}