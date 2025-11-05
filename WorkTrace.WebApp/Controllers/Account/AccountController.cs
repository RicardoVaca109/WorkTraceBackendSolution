using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using WorkTrace.Application.DTOs.UserDTO.Login;
using WorkTrace.Application.Enums;

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
            var errorContent = await response.Content.ReadAsStringAsync();

            // Try to parse error response
            try
            {
                using var jsonDoc = System.Text.Json.JsonDocument.Parse(errorContent);
                if (jsonDoc.RootElement.TryGetProperty("error", out var errorMsg))
                {
                    ViewBag.Error = errorMsg.GetString() ?? "Invalid Credentials. Please Try Again";
                }
                else
                {
                    ViewBag.Error = "Invalid Credentials. Please Try Again";
                }
            }
            catch
            {
                ViewBag.Error = "Invalid Credentials. Please Try Again";
            }

            return View();
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (loginResponse == null || string.IsNullOrEmpty(loginResponse.Token))
        {
            ViewBag.Error = "Error processing login response.";
            return View();
        }

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(loginResponse.Token);

        var isActiveClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "IsActive")?.Value;
        if (!bool.TryParse(isActiveClaim, out var isActive) || !isActive)
        {
            ViewBag.Error = "User account is inactive. Contact your administrator.";
            return View();
        }

        var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

        if (!Enum.TryParse<UserRoles>(roleClaim, out var userRole))
        {
            ViewBag.Error = "Invalid role in token.";
            return View();
        }

        var allowedRoles = new[] { UserRoles.Administrador };
        if (!allowedRoles.Contains(userRole))
        {
            ViewBag.Error = "Your role does not have access to this web application.";
            return View();
        }

        HttpContext.Session.SetString("JWToken", loginResponse.Token);
        HttpContext.Session.SetString("UserRole", userRole.ToString());
        HttpContext.Session.SetString("UserName",
            jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "");

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Account");
    }
}