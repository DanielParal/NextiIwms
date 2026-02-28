using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Application.Common.Interfaces;
using Nexticz.Module.Vh.Application.LoadedActivities.PdaReaderRawEvents;

namespace Nexticz.Module.Vh.Application.LoadedActivities.Commands.CreateLoadedActivities;

public class CreateLoadedActivitiesCommandHandler(
    IUnitOfWork unitOfWork,
    IPdaReaderRawEventsHandlerSelector pdaReaderRawEventsHandlerSelector)
    : IRequestHandler<CreateLoadedActivitiesCommand, ErrorOr<Created>>
{
    public async Task<ErrorOr<Created>> Handle(CreateLoadedActivitiesCommand command,
        CancellationToken cancellationToken)
    {
        var handler = pdaReaderRawEventsHandlerSelector.GetHandler(command.PdaReaderRawEvents);

        if (handler == null) return Error.Validation("No handler registered for events source.");

        var loadedActivities = await handler.Handle(command.PdaReaderRawEvents, cancellationToken);

        if (!loadedActivities.IsError) unitOfWork.AddRange(loadedActivities.Value);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Created;
    }
}