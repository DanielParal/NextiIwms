using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.SystemActivities;

namespace Nexticz.Module.Vh.Application.SystemActivities.Queries.GetSystemActivityById;

public class GetSystemActivityByIdQuery : IRequest<ErrorOr<SystemActivityResponse>>
{
    public required Guid Id { get; set; }
}