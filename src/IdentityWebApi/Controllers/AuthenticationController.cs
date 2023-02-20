using System.Net;
using System.Security.Claims;
using IdentityWebApi.Controllers.Dtos;
using IdentityWebApi.Controllers.Util;
using IdentityWebApi.Models;
using IdentityWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityWebApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthenticationController : ControllerBase
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ITokenService tokenService, ILogger<AuthenticationController> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUserAsync([FromBody] RegisterUserDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Falha ao cadastrar usuário");
        }

        _logger.LogInformation("Operação {OperationName} solicitada com payload {NewLine}{Payload}",
            nameof(RegisterUserAsync), Environment.NewLine, request.ToJsonStringify());

        var identityUser = new ApplicationUser(request.Name, request.Username, request.Email)
        {
            EmailConfirmed = true
        };

        var createdResult = await _userManager.CreateAsync(identityUser, request.Password);
        if (!createdResult.Succeeded)
        {
            return BadRequest(
                new ErrorMessage
                {
                    Code = "failed-to-register-user",
                    Message = "A criação do usuário falhou",
                    Details = createdResult.Errors.Select(x => new ErrorMessage { Code = x.Code, Message = x.Description })
                }
            );
        }

        return Created("api/auth/register", new
        {
            identityUser.Id,
            identityUser.UserName,
            identityUser.Email
        });
    }

    [AllowAnonymous]
    [HttpPost("register-admin")]
    public async Task<IActionResult> RegisterAdminAsync([FromBody] RegisterUserDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Falha ao cadastrar usuário");
        }

        _logger.LogInformation("Operação {OperationName} solicitada com payload {NewLine}{Payload}",
            nameof(RegisterUserAsync), Environment.NewLine, request.ToJsonStringify());

        var adminUser = new ApplicationUser(request.Name, request.Username, request.Email)
        {
            EmailConfirmed = true
        };

        var createdResult = await _userManager.CreateAsync(adminUser, request.Password);
        if (!createdResult.Succeeded)
        {
            return BadRequest(
                new ErrorMessage
                {
                    Code = "failed-to-register-user",
                    Message = "A criação do usuário falhou",
                    Details = createdResult.Errors.Select(x => new ErrorMessage { Code = x.Code, Message = x.Description })
                }
            );
        }

        if(!(await _roleManager.RoleExistsAsync("Admin"))) 
        {
            await _roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        await _userManager.AddToRoleAsync(adminUser, "Admin");

        return Created("api/auth/register-admin", new
        {
            adminUser.Id,
            adminUser.UserName,
            adminUser.Email
        });
    }

    [AllowAnonymous]
    [HttpPost("signin")]
    public async Task<IActionResult> SignInAsync([FromBody] UserLoginDto request)
    {
        var identityUser = await _userManager.FindByNameAsync(request.Username);
        if (identityUser is null)
        {
            return NotFound();
        }

        var passwordIsMatch = await _userManager.CheckPasswordAsync(identityUser, request.Password);
        if (!passwordIsMatch)
        {
            return BadRequest(new ErrorMessage
            {
                Code = "password-mismatch",
                Message = "Senha inválida"
            });
        }

        var userClaims = await _userManager.GetRolesAsync(identityUser);
        var claims = userClaims.Select(userClaim => new Claim(ClaimTypes.Role, userClaim));

        var securityToken = _tokenService.GetSecurityToken(identityUser, claims);

        _logger.LogInformation("Usuário {Username} realizou o login com sucesso", identityUser.UserName);

        return Ok(securityToken);
    }

    [Authorize]
    [HttpPut("manage-users/{userId:Guid}/change-password")]
    public async Task<IActionResult> ChangePassword([FromRoute] string userId, [FromBody] ChangePasswordDto request)
    {
        var identityUser = await _userManager.FindByIdAsync(userId);
        if(identityUser is null)
        {
            return NotFound();
        }

        var identityResult = await _userManager.ChangePasswordAsync(identityUser, request.CurrentPassword, request.NewPassword);
        if(!identityResult.Succeeded)
        {
            return BadRequest(new ErrorMessage
            {
                Code = "failed-to-change-password",
                Message = "A alteração da senha falhou",
                Details = identityResult.Errors.Select(x => new ErrorMessage { Code = x.Code, Message = x.Description })
            });
        }

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("manage-users")]
    public async Task<IActionResult> GetUsersAsync()
    {
        _logger.LogInformation("Operação {OperationName} solicitada", nameof(GetUsersAsync));

        var users = await Task.Run(() => _userManager.Users
            .Select((appUser) => new UserResponse(appUser.Id, appUser.UserName, appUser.Email)));

        return Ok(users);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("manage-users/{userId:Guid}/roles")]
    public async Task<IActionResult> UpdateUserRoleAsync([FromRoute] string userId, [FromBody] string[] roles)
    {
        var identityUser = await _userManager.FindByIdAsync(userId);
        if(identityUser is null)
        {
            return NotFound();
        }

        if(roles.Length <= 0)
        {
            return BadRequest("É necessário informar pelo menos uma role para atribuir ao usuário");
        }

        var identityResult = await _userManager.AddToRolesAsync(identityUser, roles);
        if(!identityResult.Succeeded)
        {
            return BadRequest(new ErrorMessage
            {
                Code = "failed-to-add-user-role",
                Message = "A operação de atribuição de role falhou",
                Details = identityResult.Errors.Select(x => new ErrorMessage { Code = x.Code, Message = x.Description })
            });
        }

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("role")]
    public async Task<IActionResult> CreateRoleAsync([FromBody] string[] roles)
    {
        if (roles.Length < 0)
        {
            return BadRequest("Deve ser informado ao menos uma role para continuar a operação");
        }

        foreach (var role in roles)
        {
            await _roleManager.CreateAsync(new IdentityRole(role));
        }

        return new StatusCodeResult((int)HttpStatusCode.Created);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("role")]
    public async Task<IActionResult> GetRolesAsync(CancellationToken cancellationToken)
    {
        var roles = await Task.Run(() =>
            _roleManager.Roles
                .Select(x => new { x.Id, x.Name })
                .ToArray(),
            cancellationToken
            );

        return Ok(roles);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("role/{roleId:Guid}")]
    public async Task<IActionResult> GetRoleById([FromRoute] string roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role is null)
        {
            return NotFound();
        }

        return Ok(new
        {
            role.Id,
            role.Name
        });
    }
}