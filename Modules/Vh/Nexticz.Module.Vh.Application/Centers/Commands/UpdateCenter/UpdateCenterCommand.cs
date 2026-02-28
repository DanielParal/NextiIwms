using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.Centers;

namespace Nexticz.Module.Vh.Application.Centers.Commands.UpdateCenter;

public class UpdateCenterCommand : IRequest<ErrorOr<Updated>>
{
    public required Guid Id { get; set; }
    public required UpdateCenterRequest UpdateCenterRequest { get; set; }
}