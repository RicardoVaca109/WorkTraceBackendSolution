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
    //[SwaggerOperation(Summary = "Obtiene todos los Usuarios del Sistema")]
    //[SwaggerResponse(200, "Usuarios Encontrados Correctamente", typeof(List<User>))]
    //[SwaggerResponse(401, "Usuario no Autorizado")]
    //[SwaggerResponse(404, "Recurso no Encontrado")]
    public async  Task<List<UserInformationResponse>> GetAll() => 
        await userService.GetAllAsync();

    [Authorize]
    [HttpPost]
    public async Task<User> Create(User user) =>
        await userService.CreateAsync(user);
    [HttpPost]
    public async Task<LoginResponse> Login(LoginRequest request) =>
        await userService.LoginAsync(request.Email, request.Password);

}