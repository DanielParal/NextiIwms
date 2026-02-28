using Nexticz.Lib.Shared.Translations;

namespace Nexticz.Module.Sign.DocumentManager.Application.MasstransitPublishers.SignalRNotifications;

internal static class NotificationTranslations
{
    public static readonly Translation DocumentSentToSigningDeviceTitle = new($"SIGN-manager-{nameof(DocumentSentToSigningDeviceTitle)}", "Dokumenty poslány na zařízení");
    public static readonly Translation DocumentSentToSigningDeviceDescription = new($"SIGN-manager-{nameof(DocumentSentToSigningDeviceDescription)}", "Dokumenty v SIGN modulu poslány na zařízení");
    
    public static readonly Translation DocumentReturnedFromSigningDeviceTitle = new($"SIGN-manager-{nameof(DocumentReturnedFromSigningDeviceTitle)}", "Dokumenty vráceny ze zařízení");
    public static readonly Translation DocumentReturnedFromSigningDeviceDescription = new($"SIGN-manager-{nameof(DocumentReturnedFromSigningDeviceDescription)}", "Dokumenty v SIGN modulu vráceny zařízení");
    
    public static readonly Translation DocumentsPrintingStartedTitle = new($"SIGN-manager-{nameof(DocumentsPrintingStartedTitle)}", "Dokumenty zařezeny do tiskové fronty");
    public static readonly Translation DocumentsPrintingStartedDescription = new($"SIGN-manager-{nameof(DocumentsPrintingStartedDescription)}", "Dokumenty v SIGN modulu se zařadily do tiskové fronty");
    
    public static readonly Translation DocumentsPrintingSucceededTitle = new($"SIGN-manager-{nameof(DocumentsPrintingSucceededTitle)}", "Dokumenty úspěšně poslány na tiskárnu");
    public static readonly Translation DocumentsPrintingSucceededDescription = new($"SIGN-manager-{nameof(DocumentsPrintingSucceededDescription)}", "Dokumenty v SIGN modulu se úspěšně poslali na tiskárnu");
    
    public static readonly Translation DocumentsPrintingFailedTitle = new($"SIGN-manager-{nameof(DocumentsPrintingFailedTitle)}", "Tisk dokumentů selhal");
    public static readonly Translation DocumentsPrintingFailedDescription = new($"SIGN-manager-{nameof(DocumentsPrintingFailedDescription)}", "Tisk dokumentů v SIGN modulu selhal");
    
    public static readonly Translation DocumentsPrintingFailedButWithRetryTitle = new($"SIGN-manager-{nameof(DocumentsPrintingFailedButWithRetryTitle)}", "Tisk dokumentů se nezdařil, zkusíme to znovu");
    public static readonly Translation DocumentsPrintingFailedButWithRetryDescription = new($"SIGN-manager-{nameof(DocumentsPrintingFailedButWithRetryDescription)}", "Tisk dokumentů se nezdařil, zkusíme to znovu");

    
    public static readonly Translation DocumentsSigningSucceededTitle = new($"SIGN-manager-{nameof(DocumentsSigningSucceededTitle)}", "Dokumenty jsou podepsány");
    public static readonly Translation DocumentsSigningSucceededDescription = new($"SIGN-manager-{nameof(DocumentsSigningSucceededDescription)}", "Dokumenty v SIGN modulu se úspěšně podepsaly");
    
    public static readonly Translation DocumentsSigningFailedTitle = new($"SIGN-manager-{nameof(DocumentsSigningFailedTitle)}", "Podepisování dokumentů selhalo");
    public static Translation DocumentsSigningFailedDescription(string errorMessage) => new($"SIGN-manager-{nameof(DocumentsSigningFailedDescription)}", $"Podepisování dokumentů v SIGN modulu selhalo. Error: {errorMessage}");
    
    public static readonly Translation DocumentsSigningFailedUnexpectedlyTitle = new($"SIGN-manager-{nameof(DocumentsSigningFailedUnexpectedlyTitle)}", "Podepisování dokumentů neočekávaně selhalo");
    public static Translation DocumentsSigningFailedUnexpectedlyDescription(string correlationId) => new($"SIGN-manager-{nameof(DocumentsSigningFailedUnexpectedlyDescription)}", $"Podepisování dokumentů v SIGN modulu neočekávaně selhalo. Kontaktujte podporu s tímto id: {correlationId}");
}