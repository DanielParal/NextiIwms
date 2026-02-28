using ErrorOr;
using MediatR;

namespace Nexticz.Module.Vh.Application.LoadedActivities.Commands.CreateLoadedActivities;

public record CreateLoadedActivitiesCommand(PdaReaderRawEvents.PdaReaderRawEvents PdaReaderRawEvents) : IRequest<ErrorOr<Created>>;