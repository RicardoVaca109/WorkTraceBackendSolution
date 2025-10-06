using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using WorkTrace.Application.DTOs.UserDTO.Information;

namespace WorkTrace.WebApp.Controllers.User;

public class UserController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public UserController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var token = HttpContext.Session.GetString("JWToken");
        if (string.IsNullOrEmpty(token))
        {
            // If not logged in, redirect to login page
            return RedirectToAction("Login", "Account");
        }

        var client =  _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(_configuration.GetValue<string>("APIConfigurations:ApiUrl").ToString());

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("User/GetAll");

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = "Error al obtener a los empleados";
            return View(new List<UserInformationResponse>());
        }

        var json = await response.Content.ReadAsStringAsync();

        var employees = JsonSerializer.Deserialize<List<UserInformationResponse>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true});

        return View(employees);
    }
}