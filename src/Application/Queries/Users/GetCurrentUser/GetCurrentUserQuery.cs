using AuthHub.Application.Dtos.Users;
using MediatR;

namespace AuthHub.Application.Queries.Users.GetCurrentUser;

public record GetCurrentUserQuery : IRequest<UserResponse>;