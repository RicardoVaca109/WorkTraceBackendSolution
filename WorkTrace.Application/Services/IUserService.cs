using WorkTrace.Application.DTOs.UserDTO.Information;
using WorkTrace.Application.DTOs.UserDTO.Login;
using WorkTrace.Data.Models;

namespace WorkTrace.Application.Services;

public interface IUserService
{
    Task<List<UserInformationResponse>> GetAllAsync();
    Task<User?> GetByIdAsync(string id);
    Task<UserInformationResponse> CreateAsync(CreateUserRequest userCreate);
    Task<LoginResponse> LoginAsync(string email, string password);
    Task<UserInformationResponse> UdpateAsync(string id, UpdateUserRequest user);
    Task<bool> SetInactiveUser(string userId);
}