using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Sign.Settings.Application.DepositorGroups;


internal abstract class DepositorGroupErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "sign-settings-api-depositorGroupService-";

    public static Error ValidationCodeIsRequired => Error.Validation(
        ComponentSlug + "validationDepositorGroupCodeIsRequired",
        "Kód skupiny ukladatelů není vyplněn."
    );
    
    public static Error ValidationNameIsRequired => Error.Validation(
        ComponentSlug + "validationDepositorGroupNameIsRequired",
        "Jméno skupiny ukladatelů není vyplněno."
    );
    
    public static Error CodeDoesNotExist => Error.NotFound(
        ComponentSlug + "codeDoesNotExist",
        "Skupina ukladatelů s tímto kódem neexistuje.");
    
    public static Error ValidationCodeDoesNotExist => Error.Validation(
        ComponentSlug + "validationCodeDoesNotExist",
        "Skupina ukladatelů s tímto kódem neexistuje.");
    
    public static Error ValidationCodeAlreadyExists => Error.Validation(
        ComponentSlug + "validationCodeAlreadyExists",
        "Skupina ukladatelů s tímto kódem již existuje."
    );
    
    public static Error ValidationCodeIsAssignedToUsers(string userNames) => Error.Validation(
        ComponentSlug + "validationCodeIsAssignedToUsers",
        $"Nemůžeme smazat skupinu ukladatelů, protože je stále nastavena u těchto uživatelů: {userNames}."
    );
    
    public static Error ValidationCodeIsUsedInDepositors(string depositorCodes) => Error.Validation(
        ComponentSlug + "validationCodeIsUsedInDepositors",
        $"Nemůžeme smazat skupinu ukladatelů, protože je stále nastavena u těchto ukladatelů: {depositorCodes}."
    );
}