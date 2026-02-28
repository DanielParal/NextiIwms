using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.NonDispensingActivities;

namespace Nexticz.Module.Vh.Application.NonDispensingActivities.Queries.GetNonDispensingActivityBySlug;

public class GetNonDispensingActivityBySlugQuery : IRequest<ErrorOr<NonDispensingActivityResponse>>
{
    public required string Slug { get; set; }   
}