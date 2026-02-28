using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Sign.Settings.Application.DocumentTemplates;


internal abstract class DocumentTemplateErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "sign-settings-api-DocumentTemplateService-";
    
    public static Error CodeNotFound => Error.NotFound(
        ComponentSlug + "codeNotFound",
        "Šablona s tímto kódem neexistuje.");
    
    public static Error ValidationCodeAlreadyExists = Error.Validation(
        ComponentSlug + "ValidationCodeAlreadyExists",
        "Kód šablony již existuje.");
    
    public static Error ValidationCodeDoesNotExist = Error.Validation(
        ComponentSlug + "ValidationCodeDoesNotExist",
        "Kód šablony neexistuje.");
    
    public static Error ValidationDocumentTemplateIsUsedInDepositors = Error.Validation(
        ComponentSlug + "ValidationDocumentTemplateIsUsedInDepositors",
        "Kód šablony je stále použit v ukladatelích.");
    
}