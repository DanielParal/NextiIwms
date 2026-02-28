using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Vh.Domain.Assortments;

public abstract class AssortmentErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "vh-api-assortmentService-";

    public static Error AssortmentWithIdDoesnotExist => Error.Validation(
        ComponentSlug + "assortmentWithIdDoesnotExist",
        "Sortiment s tímto ID neexistuje");

    public static Error CreateAssortmentError => Error.Validation(
        ComponentSlug + "createAssortmentError",
        "Sortiment se nepodařil vytvořit");

    public static Error UpdateAssortmentError => Error.Validation(
        ComponentSlug + "updateAssortmentError",
        "Sortiment se nepodařil upravit");

    public static Error DeleteAssortmentError => Error.Validation(
        ComponentSlug + "deleteAssortmentError",
        "Sortiment se nepodařil smazat");
}