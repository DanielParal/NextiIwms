using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Sign.Settings.Domain.EmailConfigurationAggregate;

internal abstract class EmailConfigurationDomainErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "sign-settings-domain-emailConfiguration-";
    
    public static Error ValidationInvalidCodesCombination = Error.Validation(
        ComponentSlug + "ValidationInvalidCodesCombination",
        "Špatná kombinace kódů."
    );
    
    public static Error ValidationForLoadingDocumentOnlyDepositorCodeMustBeFilledIn = Error.Validation(
        ComponentSlug + "ValidationForLoadingDocumentOnlyDepositorCodeMustBeFilledIn",
        "Pro posílání nakládkových listů musí být vyplněn pouze kód ukladatele. Kód partnera a kód příjemce musí být prázdné."
    );
    
    public static Error ValidationDeliveryDocumentCannotSendLoadingDocument = Error.Validation(
        ComponentSlug + "ValidationLoadingDocumentCanBeSentOnlyForLoadingConfiguration",
        "Pro konfiguraci posílání dodacích listů nesmí být nastaveno posílání nakládkových listů."
    );
    
    public static Error ValidationAtLeastOneOptionForSendingEmailMustBeFilledIn = Error.Validation(
        ComponentSlug + "ValidationAtLeastOneOptionForSendingEmailMustBeFilledIn",
        "Musí být vyplněna alespoň jedna možnost pro odesílání dokumentů."
    );
    
    public static Error ValidationAtLeastOneEmailAddressMustBeFilledInd = Error.Validation(
        ComponentSlug + "ValidationAtLeastOneEmailAddressMustBeFilledInd",
        "Musíte vyplnit alespoň jednu e-mailovou adresu."
    );
    
    public static Error ValidationInvalidEmailAddress(string email) => Error.Validation(
        ComponentSlug + "ValidationInvalidEmailAddress",
        $"Tato e-mailová adresa není platná: {email}."
    );
}