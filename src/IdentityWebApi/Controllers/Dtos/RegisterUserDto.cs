using System.ComponentModel.DataAnnotations;

namespace IdentityWebApi.Controllers.Dtos;

public record RegisterUserDto
{
    [Required]
    [MinLength(4)]
    [MaxLength(128)]
    public string Name { get; set; }
 
    [Required]
    [MinLength(4)]
    [MaxLength(64)]
    public string Username { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(4)]
    [MaxLength(64)]
    public string Password { get; set; }

    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; }
}