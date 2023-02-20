namespace IdentityWebApi.Controllers.Dtos;

public record UserResponse
{
    public UserResponse(string id, string username, string email)
    {
        Id = id;
        Username = username;
        Email = email;
    }

    public string Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
}