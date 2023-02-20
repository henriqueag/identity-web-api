using System.Text.Json;

namespace IdentityWebApi.Controllers.Util;

public static class JsonSerializerExtensions
{
    private static readonly JsonSerializerOptions s_options = new JsonSerializerOptions
    {
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public static string ToJsonStringify(this object target)
        => JsonSerializer.Serialize(target, s_options);
}