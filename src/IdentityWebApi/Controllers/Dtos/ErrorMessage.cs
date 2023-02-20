using System.Text.Json.Serialization;

namespace IdentityWebApi.Controllers.Dtos;

public record ErrorMessage
{
    [JsonPropertyOrder(1)]
    public string Code { get; set; }

    [JsonPropertyOrder(2)]
    public string Message { get; set; }

    [JsonPropertyOrder(3)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<ErrorMessage> Details { get; set; }
}
