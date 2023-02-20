using System.Text.Json.Serialization;

namespace IdentityWebApi.Services;

public record SecurityToken
{
    public SecurityToken(string accessToken, int expiresIn)
    {
        AccessToken = accessToken;
        ExpiresIn = expiresIn;
        TokenType = "Bearer";
    }

    [JsonPropertyName("access_token")]
    public string AccessToken { get; }

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; }

    [JsonPropertyName("token_type")]
    public string TokenType { get; }
}