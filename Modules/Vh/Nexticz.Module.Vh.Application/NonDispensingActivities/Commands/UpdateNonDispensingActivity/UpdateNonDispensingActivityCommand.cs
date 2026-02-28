using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.NonDispensingActivities;

namespace Nexticz.Module.Vh.Application.NonDispensingActivities.Commands.UpdateNonDispensingActivity;

public class UpdateNonDispensingActivityCommand : IRequest<ErrorOr<Updated>>
{
    public required Guid Id { get; set; }
    public required UpdateNonDispensingActivityRequest UpdateNonDispensingActivityRequest { get; set; }
}