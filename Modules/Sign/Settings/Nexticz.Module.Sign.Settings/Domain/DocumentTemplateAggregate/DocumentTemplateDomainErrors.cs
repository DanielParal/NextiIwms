using System.Runtime.InteropServices.JavaScript;
using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Sign.Settings.Domain.DocumentTemplateAggregate;

internal abstract class DocumentTemplateDomainErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "sign-settings-domain-documentTemplate-";

    public static Error ValidationCodeIsRequired = Error.Validation(
        ComponentSlug + "ValidationCodeIsRequired",
        "Kód šablony je povinné pole.");
    
    public static Error ValidationDuplicateTextOffsetNames = Error.Validation(
        ComponentSlug + "ValidationDuplicateTextOffsetNames",
        "Šablona obsahuje duplikovaná jména pro text offsety.");
    
    public static Error ValidationEmptyTextOffsetNames = Error.Validation(
        ComponentSlug + "ValidationEmptyTextOffsetNames",
        "Šablona neobsahuje jména pro text offsety.");
    
    public static Error ValidationDuplicateTextBackgroundNames = Error.Validation(
        ComponentSlug + "ValidationDuplicateTextBackgroundNames",
        "Šablona obsahuje duplikovaná jména pro text pozadí.");
    
    public static Error ValidationEmptyTextBackgroundNames = Error.Validation(
        ComponentSlug + "ValidationEmptyTextBackgroundNames",
        "Šablona neobsahuje jména pro text offsety.");
}