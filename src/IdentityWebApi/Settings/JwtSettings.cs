using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace IdentityWebApi.Settings;

public class JwtSettings
{
    private const string _secretKey = "640ca0879a614b77ab35a799f0fbd2d6";

    public const string SectionKey = nameof(JwtSettings);

    public static readonly SymmetricSecurityKey SecurityKey = new(Encoding.UTF8.GetBytes(_secretKey));

    public string Issuer { get; set; } = "";
    public string Audience { get; set; } = "";
    public int LifetimeInSeconds { get; set; }
}