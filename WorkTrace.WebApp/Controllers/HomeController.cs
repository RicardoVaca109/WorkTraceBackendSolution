using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;
using WorkTrace.Application.DTOs.ClientDTO.Information;
using WorkTrace.Application.DTOs.ServiceMgmtDTO.Management;
using WorkTrace.WebApp.Filters;
using WorkTrace.WebApp.Models;

namespace WorkTrace.WebApp.Controllers;

[AuthorizeSession]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public async Task<IActionResult> Clientes()
    {
        var token = HttpContext.Session.GetString("JWToken");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(_configuration.GetValue<string>("APIConfigurations:ApiUrl").ToString());
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("Client/GetAll");

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = "Error al obtener los clientes";
            return View(new List<ClientInformationResponse>());
        }

        var json = await response.Content.ReadAsStringAsync();
        var clients = JsonSerializer.Deserialize<List<ClientInformationResponse>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return View(clients ?? new List<ClientInformationResponse>());
    }

    public IActionResult UsuarioNomina()
    {
        return View();
    }

    public IActionResult Estadistica()
    {
        return View();
    }

    public IActionResult CalendarioActividades()
    {
        return View();
    }

    public IActionResult Seguimiento()
    {
        return View();
    }

    public async Task<IActionResult> Servicios()
    {
        var token = HttpContext.Session.GetString("JWToken");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(_configuration.GetValue<string>("APIConfigurations:ApiUrl").ToString());
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("ServiceandInstallationSteps/GetAll");

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error = "Error al obtener los servicios";
            return View(new List<ServiceViewModel>());
        }

        var json = await response.Content.ReadAsStringAsync();
        var services = JsonSerializer.Deserialize<List<ServiceViewModel>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return View(services ?? new List<ServiceViewModel>());
    }

    [HttpPost]
    public async Task<IActionResult> UpdateClient([FromBody] ClientInformationResponse model)
    {
        var token = HttpContext.Session.GetString("JWToken");
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized();
        }

        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(_configuration.GetValue<string>("APIConfigurations:ApiUrl").ToString());
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateRequest = new
        {
            fullName = model.FullName,
            documentNumber = model.DocumentNumber,
            phoneNumber = model.PhoneNumber,
            email = model.Email
        };

        var jsonContent = JsonSerializer.Serialize(updateRequest);
        var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

        var response = await client.PutAsync($"Client/Update?id={model.Id}", content);

        if (!response.IsSuccessStatusCode)
        {
            return BadRequest(new { error = "Error al actualizar el cliente" });
        }

        return Ok(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateService([FromBody] ServiceViewModel model)
    {
        var token = HttpContext.Session.GetString("JWToken");
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized();
        }

        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(_configuration.GetValue<string>("APIConfigurations:ApiUrl").ToString());
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var jsonContent = JsonSerializer.Serialize(new { id = model.Id, name = model.Name, description = model.Description });
        var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

        var response = await client.PutAsync("ServiceandInstallationSteps/UpdateService", content);

        if (!response.IsSuccessStatusCode)
        {
            return BadRequest(new { error = "Error al actualizar el servicio" });
        }

        return Ok(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateInstallationStep([FromBody] dynamic model)
    {
        var token = HttpContext.Session.GetString("JWToken");
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized();
        }

        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(_configuration.GetValue<string>("APIConfigurations:ApiUrl").ToString());
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var jsonContent = JsonSerializer.Serialize(model);
        var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

        var response = await client.PutAsync("ServiceandInstallationSteps/UpdateInstallationStep", content);

        if (!response.IsSuccessStatusCode)
        {
            return BadRequest(new { error = "Error al actualizar el paso de instalación" });
        }

        return Ok(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> CreateClient([FromBody] CreateClientRequest model)
    {
        var token = HttpContext.Session.GetString("JWToken");
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized();
        }

        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(_configuration.GetValue<string>("APIConfigurations:ApiUrl").ToString());
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var jsonContent = JsonSerializer.Serialize(model);
        var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

        var response = await client.PostAsync("Client/Create", content);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            return BadRequest(errorContent);
        }

        return Ok(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> CreateService([FromBody] CreateServiceRequest model)
    {
        var token = HttpContext.Session.GetString("JWToken");
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized();
        }

        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(_configuration.GetValue<string>("APIConfigurations:ApiUrl").ToString());
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var jsonContent = JsonSerializer.Serialize(model);
        var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

        var response = await client.PostAsync("ServiceandInstallationSteps/Create", content);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            return BadRequest(errorContent);
        }

        return Ok(new { success = true });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}