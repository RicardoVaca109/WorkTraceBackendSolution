using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkTrace.Application.DTOs.UserDTO.Information;
using WorkTrace.Application.DTOs.UserDTO.Login;
using WorkTrace.Application.Services;

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
    public async Task<UserInformationResponse> Create(CreateUserRequest user) =>
        await userService.CreateAsync(user);
    [HttpPost]
    public async Task<LoginResponse> Login(LoginRequest request) =>
        await userService.LoginAsync(request.Email, request.Password);

    [Authorize]
    [HttpPut]
    public async Task<UserInformationResponse> Update(string id, UpdateUserRequest request) =>
        await  userService.UpdateAsync(id, request);

    [Authorize]
    [HttpPut]
    public async Task<IActionResult> DeactivateUser(string id)
    {
        var succes = await userService.SetInactiveUser(id);
        if (!succes) return NotFound("User Not Found or Inactive");
        return Ok("User set to inactive succesfully");
    }
}