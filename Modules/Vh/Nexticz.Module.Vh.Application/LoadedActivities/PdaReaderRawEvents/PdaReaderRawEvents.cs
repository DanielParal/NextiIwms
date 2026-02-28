namespace Nexticz.Module.Vh.Application.LoadedActivities.PdaReaderRawEvents;

public record PdaReaderRawEvents(object Data, PdaReaderRawEventsSource PdaReaderRawEventsSource);

public enum PdaReaderRawEventsSource
{
    MyStockDb,
    MyStockXlsx,
    SagDb,
    SagCsv,
    IwmsDb,
    IwmsCsv
}