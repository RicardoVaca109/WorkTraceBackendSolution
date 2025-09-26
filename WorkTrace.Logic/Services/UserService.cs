using WorkTrace.Application.DTOs.UserDTO.Login;
using WorkTrace.Application.Repositories;
using WorkTrace.Application.Services;
using WorkTrace.Data.Models;

namespace WorkTrace.Logic.Services;

public class UserService(IUserRepository userRepository, IJwtService jwtService) : IUserService
{
    public async Task<List<User>> GetAllAsync()
    {
        return await userRepository.GetAsync();
    }

    public async Task<User?> GetByIdAsync(string id) //Buscar por Id Especifico 
    {
        return await userRepository.GetAsync(id);
    }

    public async Task<User> CreateAsync(User user)
    {
        var existingUsers = await userRepository.GetByDocumentNumberAndEmailAsync(user.DocumentNumber, user.Email);

        if (existingUsers.Any(u => u.DocumentNumber == user.DocumentNumber))
            throw new Exception("There is already a user with this document number");
        if (existingUsers.Any(u => u.Email == user.Email))
            throw new Exception("There is already a user with this email");

        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
        user.IsActive = true;

        await userRepository.CreateAsync(user);
        return user;
    }

    public async Task<LoginResponse> LoginAsync(string email, string password)
    {
        var user = await userRepository.GetByEmailAsync(email);
        if (user == null)
            throw new Exception("Wrong Credentials");

        var isValidPassword = BCrypt.Net.BCrypt.Verify(password, user.Password);

        if (!isValidPassword)
            throw new Exception("Wrong Credentials");

        var token = jwtService.GenerateToken(user);

        return new LoginResponse 
        {
            Token = token,
            ExpireAt = DateTime.UtcNow.AddMinutes(30)
        };
    }
}