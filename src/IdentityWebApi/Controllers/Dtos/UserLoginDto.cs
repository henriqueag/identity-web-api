namespace IdentityWebApi.Controllers.Dtos;

public record ChangePasswordDto
{
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
}

public record UserLoginDto
{
    public string Username { get; set; }
    public string Password { get; set; }
}