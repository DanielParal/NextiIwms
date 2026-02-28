using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.NonDispensingActivities;

namespace Nexticz.Module.Vh.Application.NonDispensingActivities.Commands.CreateNonDispensingActivity;

public class CreateNonDispensingActivityCommand : IRequest<ErrorOr<Created>>
{
    public required CreateNonDispensingActivityRequest CreateNonDispensingActivityRequest { get; set; }
}