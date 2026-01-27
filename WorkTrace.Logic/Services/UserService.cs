using AutoMapper;
using MongoDB.Driver;
using WorkTrace.Application.DTOs.UserDTO.Information;
using WorkTrace.Application.DTOs.UserDTO.Login;
using WorkTrace.Application.Repositories;
using WorkTrace.Application.Services;
using WorkTrace.Data.Models;

namespace WorkTrace.Logic.Services;

public class UserService(IUserRepository _userRepository, IJwtService _jwtService, IMapper _mapper) : IUserService
{
    public async Task<List<UserInformationResponse>> GetAllAsync()
    {
        var systemUsers = await _userRepository.GetAsync();
        return _mapper.Map<List<UserInformationResponse>>(systemUsers);
    }

    public async Task<UserInformationResponse> GetByIdAsync(string id)
    {
        var userById = await _userRepository.GetAsync(id);
        if (userById == null)
            throw new Exception("Usuario no Encontrado");

        var response = _mapper.Map<UserInformationResponse>(userById);
        return response;
    }

    public async Task<UserInformationResponse> CreateAsync(CreateUserRequest userCreate)
    {
        if (await _userRepository.GetByDocumentNumberAsync(userCreate.DocumentNumber) != null)
            throw new Exception("Ya existe un usuario con este número de documento");

        if (await _userRepository.GetByEmailAsync(userCreate.Email) != null)
            throw new Exception("Ya existe un usuario con este correo");

        userCreate.Password = BCrypt.Net.BCrypt.HashPassword(userCreate.Password);
        userCreate.IsActive = true;

        var userToDatabase = _mapper.Map<User>(userCreate);

        await _userRepository.CreateAsync(userToDatabase);
        return _mapper.Map<UserInformationResponse>(userToDatabase);
    }

    public async Task<LoginResponse> LoginAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
            throw new Exception("Credenciales Incorrectas");

        var isValidPassword = BCrypt.Net.BCrypt.Verify(password, user.Password);

        if (!isValidPassword)
            throw new Exception("Credenciales Incorrectas");

        var token = _jwtService.GenerateToken(user);

        return new LoginResponse
        {
            Token = token,
            ExpireAt = DateTime.UtcNow.AddMinutes(400)
        };
    }

    public async Task<UserInformationResponse> UpdateAsync(string id, UpdateUserRequest user)
    {
        var usertoUpdate = await _userRepository.GetAsync(id);
        if (usertoUpdate == null) throw new Exception("Usuario no encontrado");

        usertoUpdate.FullName = string.IsNullOrWhiteSpace(user.FullName) ? usertoUpdate.FullName : user.FullName;
        usertoUpdate.Email = string.IsNullOrWhiteSpace(user.Email) ? usertoUpdate.Email : user.Email;
        usertoUpdate.PhoneNumber = string.IsNullOrWhiteSpace(user.PhoneNumber) ? usertoUpdate.PhoneNumber : user.PhoneNumber;
        usertoUpdate.DocumentNumber = string.IsNullOrWhiteSpace(user.DocumentNumber) ? usertoUpdate.DocumentNumber : user.DocumentNumber;
        if (user.Role.HasValue) usertoUpdate.Role = user.Role.Value;
        if (user.IsActive.HasValue) usertoUpdate.IsActive = user.IsActive.Value;

        if (!string.IsNullOrEmpty(user.Password)) usertoUpdate.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

        await _userRepository.UpdateAsync(id, usertoUpdate);

        return _mapper.Map<UserInformationResponse>(usertoUpdate);
    }

    public async Task<bool> SetInactiveUser(string userId)
    {
        var userFilter = await _userRepository.GetAsync(userId);
        if (userFilter == null) throw new Exception("Usuario no encontrado");

        if (!userFilter.IsActive) return false;

        userFilter.IsActive = false;

        await _userRepository.UpdateAsync(userId, userFilter);

        return true;
    }
}