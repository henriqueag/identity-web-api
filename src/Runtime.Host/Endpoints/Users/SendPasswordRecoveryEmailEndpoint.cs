using AuthHub.Application.Commands.Users.SendPasswordRecoveryEmail;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AuthHub.Runtime.Host.Endpoints.Users;

public class SendPasswordRecoveryEmailEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost("api/users/password/forgot", SendPasswordRecoveryEmailAsync)
            .WithTags("Users")
            .WithOpenApi();
    }

    private static async Task<IResult> SendPasswordRecoveryEmailAsync(HttpContext context, [FromServices] ISender sender, SendPasswordRecoveryEmailCommand command, CancellationToken cancellationToken)
    {
        command = command with { Link = $"{context.Request.Scheme}://{context.Request.Host}/api/users/password/recovery" };
        await sender.Send(command, cancellationToken);
        return Results.Accepted();
    }
}