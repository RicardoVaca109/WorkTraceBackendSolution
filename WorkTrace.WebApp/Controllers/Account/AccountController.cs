using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using WorkTrace.Application.DTOs.UserDTO.Login;

namespace WorkTrace.WebApp.Controllers.Account;

public class AccountController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public AccountController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginRequest());
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(_configuration.GetValue<string>("APIConfigurations:ApiUrl").ToString());

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("User/Login", content);

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = "Invalid Credentials";
            return View();
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        HttpContext.Session.SetString("JWToken", loginResponse.Token);

        return RedirectToAction("Index", "Home");
    }
}