using System.Text.Json.Serialization;

namespace WorkTrace.WebApp.Models;

public class ServiceViewModel
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("installationSteps")]
    public List<InstallationStepViewModel> InstallationSteps { get; set; } = new();
}
