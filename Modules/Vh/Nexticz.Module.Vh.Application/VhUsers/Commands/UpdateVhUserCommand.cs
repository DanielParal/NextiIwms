using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.VhUsers;

namespace Nexticz.Module.Vh.Application.VhUsers.Commands;

public record UpdateVhUserCommand(UpdateVhUserRequest UpdateVhUserRequest) : IRequest<ErrorOr<Updated>>;