using ErrorOr;
using Nexticz.Module.Vh.Domain.LoadedActivities;

namespace Nexticz.Module.Vh.Application.LoadedActivities.PdaReaderRawEvents;

public interface IPdaReaderRawEventsHandler
{
    Task<ErrorOr<List<LoadedActivity>>> Handle(PdaReaderRawEvents pdaReaderRawEvents, CancellationToken cancellationToken);
    bool CanHandle(PdaReaderRawEventsSource pdaReaderRawEventsSource);
}