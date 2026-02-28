using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Application.NonDispensingActivities.Common.Models;
using Nexticz.Module.Vh.Contracts.NonDispensingActivities;


namespace Nexticz.Module.Vh.Application.NonDispensingActivities.Queries.GetNonDispensingActivityById;

public class GetNonDispensingActivityByIdQuery : IRequest<ErrorOr<NonDispensingActivityResponse>>
{
    public required Guid Id { get; set; }
}