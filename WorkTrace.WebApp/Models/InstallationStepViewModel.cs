using System.Text.Json.Serialization;

namespace WorkTrace.WebApp.Models;

public class InstallationStepViewModel
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("steps")]
    public int Steps { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }
}
