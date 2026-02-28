using Nexticz.Lib.Shared.Translations;
using Nexticz.Module.Cuzk.Domain.ImportAggregate;

namespace Nexticz.Module.Cuzk.Application.MasstransitPublishers;

internal static class NotificationTranslations
{
    public static readonly Translation ImportFinishedTitle = new($"CUZK-{nameof(ImportFinishedTitle)}", "Import nahrán");
    public static Translation ImportFinishedDescription(Guid importId, ImportType type) => 
        new($"CUZK-{nameof(ImportFinishedDescription)}", $"Import nahrán. Id: {importId}, typ: {type}");
    
    
    public static readonly Translation ImportFailedTitle = new($"CUZK-{nameof(ImportFailedTitle)}", "Nahrání importu selhalo");
    public static Translation ImportFailedDescription(Guid importId, ImportType type, string errorDescription, string logId) => 
        new($"CUZK-{nameof(ImportFailedDescription)}", $"Nahrání importu selhalo. Id: {importId}, typ: {type}, error: {errorDescription}, logId: {logId}");
}