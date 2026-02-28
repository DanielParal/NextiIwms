namespace Nexticz.Module.Vh.Application.LoadedActivities.PdaReaderRawEvents;

public interface IPdaReaderRawEventsHandlerSelector
{
    IPdaReaderRawEventsHandler? GetHandler(PdaReaderRawEvents pdaReaderRawEvents);
}

public class PdaReaderRawEventsHandlerSelector(IEnumerable<IPdaReaderRawEventsHandler> pdaReaderRawEventsHandlers) : IPdaReaderRawEventsHandlerSelector
{
    public IPdaReaderRawEventsHandler? GetHandler(PdaReaderRawEvents pdaReaderRawEvents)
    {
        foreach (var pdaReaderRawEventsHandler in pdaReaderRawEventsHandlers)
        {
            if (pdaReaderRawEventsHandler.CanHandle(pdaReaderRawEvents.PdaReaderRawEventsSource))
            {
                return pdaReaderRawEventsHandler;
            }
        }

        return null;
    }
}