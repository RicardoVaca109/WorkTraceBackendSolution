using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkTrace.Application.DTOs.UserDTO.Information;
using WorkTrace.Application.DTOs.UserDTO.Login;
using WorkTrace.Application.Services;
using WorkTrace.Data.Models;

namespace WorkTrace.Api.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class UserController(IUserService userService) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async  Task<List<UserInformationResponse>> GetAll() => 
        await userService.GetAllAsync();

    [Authorize]
    [HttpPost]
    public async Task<User> Create(User user) =>
        await userService.CreateAsync(user);
    [HttpPost]
    public async Task<LoginResponse> Login(LoginRequest request) =>
        await userService.LoginAsync(request.Email, request.Password);

    [Authorize]
    [HttpPut]
    public async Task<UserInformationResponse> Update(string id, UpdateUserRequest request) =>
        await userService.UdpateAsync(id, request);
}