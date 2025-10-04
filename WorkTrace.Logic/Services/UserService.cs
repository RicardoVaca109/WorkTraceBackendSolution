using Microsoft.AspNetCore.Http.HttpResults;
using MongoDB.Driver;
using WorkTrace.Application.DTOs.UserDTO.Information;
using WorkTrace.Application.DTOs.UserDTO.Login;
using WorkTrace.Application.Repositories;
using WorkTrace.Application.Services;
using WorkTrace.Data.Models;

namespace WorkTrace.Logic.Services;

public class UserService(IUserRepository _userRepository, IJwtService _jwtService) : IUserService
{
    public async Task<List<UserInformationResponse>> GetAllAsync()
    {
        var systemUsers = await _userRepository.GetAsync();

        return systemUsers.Select(user => new UserInformationResponse
        {
            FullName = user.FullName,
            DocumentNumber = user.DocumentNumber,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role,
        }).ToList();
    }

    public async Task<User?> GetByIdAsync(string id) //Buscar por Id Especifico 
    {
        return await _userRepository.GetAsync(id);
    }

    public async Task<UserInformationResponse> CreateAsync(CreateUserRequest userCreate)
    {
        var existingUsers = await _userRepository.GetByDocumentNumberAndEmailAsync(userCreate.DocumentNumber, userCreate.Email);

        if (existingUsers.Any(u => u.DocumentNumber == userCreate.DocumentNumber))
            throw new Exception("There is already a user with this document number");
        if (existingUsers.Any(u => u.Email == userCreate.Email))
            throw new Exception("There is already a user with this email");

        var userToDatabase = new User
        {
            FullName = userCreate.FullName,
            DocumentNumber = userCreate.DocumentNumber,
            Email = userCreate.Email,
            PhoneNumber = userCreate.PhoneNumber,
            Password = userCreate.Password = BCrypt.Net.BCrypt.HashPassword(userCreate.Password),
            IsActive = userCreate.IsActive = true,
            Role = userCreate.Role,
        };

        await _userRepository.CreateAsync(userToDatabase);
        return new UserInformationResponse
        {
            FullName = userToDatabase.FullName,
            DocumentNumber = userToDatabase.DocumentNumber,
            Email = userToDatabase.Email,
            Role = userToDatabase.Role,
            PhoneNumber = userToDatabase.PhoneNumber,
            IsActive = userToDatabase.IsActive,
        };
    }

    public async Task<LoginResponse> LoginAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
            throw new Exception("Wrong Credentials");

        var isValidPassword = BCrypt.Net.BCrypt.Verify(password, user.Password);

        if (!isValidPassword)
            throw new Exception("Wrong Credentials");

        var token = _jwtService.GenerateToken(user);

        return new LoginResponse
        {
            Token = token,
            ExpireAt = DateTime.UtcNow.AddMinutes(30)
        };
    }

    public async Task<UserInformationResponse> UdpateAsync(string id, UpdateUserRequest user)
    {
        var usertoUpdate = await _userRepository.GetAsync(id);
        if (usertoUpdate == null) throw new Exception("User not Found");

        usertoUpdate.FullName = string.IsNullOrWhiteSpace(user.FullName) ? usertoUpdate.FullName : user.FullName;
        usertoUpdate.Email = string.IsNullOrWhiteSpace(user.Email) ? usertoUpdate.Email : user.Email;
        usertoUpdate.PhoneNumber = string.IsNullOrWhiteSpace(user.PhoneNumber) ? usertoUpdate.PhoneNumber : user.PhoneNumber;
        usertoUpdate.DocumentNumber = string.IsNullOrWhiteSpace(user.DocumentNumber) ? usertoUpdate.DocumentNumber : user.DocumentNumber;
        if (user.Role.HasValue) usertoUpdate.Role = user.Role.Value;
        if (user.IsActive.HasValue) usertoUpdate.IsActive = user.IsActive.Value;

        if (!string.IsNullOrEmpty(user.Password)) usertoUpdate.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

        await _userRepository.UpdateAsync(id, usertoUpdate);

        return new UserInformationResponse
        {
            FullName = usertoUpdate.FullName,
            DocumentNumber = usertoUpdate.DocumentNumber,
            Email = usertoUpdate.Email,
            Role = usertoUpdate.Role,
            PhoneNumber = usertoUpdate.PhoneNumber,
            IsActive = usertoUpdate.IsActive,
        };
    }

    public async Task<bool> SetInactiveUser(string userId)
    {
        var userFilter = await _userRepository.GetAsync(userId);
        if (userFilter == null) throw new Exception("User Not Found");

        if (!userFilter.IsActive) return false;

        userFilter.IsActive = false;

        await _userRepository.UpdateAsync(userId, userFilter);

        return true;
    }
}