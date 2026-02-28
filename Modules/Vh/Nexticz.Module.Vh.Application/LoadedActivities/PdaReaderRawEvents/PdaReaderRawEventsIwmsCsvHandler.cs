using ErrorOr;
using Nexticz.Module.Vh.Domain.LoadedActivities;

namespace Nexticz.Module.Vh.Application.LoadedActivities.PdaReaderRawEvents;

public class PdaReaderRawEventsIwmsCsvHandler : IPdaReaderRawEventsHandler
{
    public Task<ErrorOr<List<LoadedActivity>>> Handle(PdaReaderRawEvents pdaReaderRawEvents, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public bool CanHandle(PdaReaderRawEventsSource pdaReaderRawEventsSource)
    {
        return pdaReaderRawEventsSource == PdaReaderRawEventsSource.IwmsCsv;
    }
} 