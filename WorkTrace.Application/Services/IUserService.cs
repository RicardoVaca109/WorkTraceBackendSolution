using WorkTrace.Application.DTOs.UserDTO.Login;
using WorkTrace.Data.Models;

namespace WorkTrace.Application.Services;

public interface IUserService
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(string id);
    Task<User> CreateAsync(User user);
    Task<LoginResponse> LoginAsync(string email, string password);
}
