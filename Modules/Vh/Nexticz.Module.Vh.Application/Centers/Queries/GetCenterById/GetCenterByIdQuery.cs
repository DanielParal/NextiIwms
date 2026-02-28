using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.Centers;

namespace Nexticz.Module.Vh.Application.Centers.Queries.GetCenterById;

public class GetCenterByIdQuery : IRequest<ErrorOr<CenterResponse>>
{
    public required Guid Id { get; set; }
}