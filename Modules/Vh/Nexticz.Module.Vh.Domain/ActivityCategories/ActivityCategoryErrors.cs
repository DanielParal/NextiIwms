using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Vh.Domain.ActivityCategories;

public abstract class ActivityCategoryErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "vh-api-activityCategoryService-";

    public static Error ActivityCategoryWithIdDoesnotExist => Error.Validation(
        ComponentSlug + "activityCategoryWithIdDoesnotExist",
        "Kategorie činnosti s tímto ID neexistuje");

    public static Error CreateActivityCategoryError => Error.Validation(
        ComponentSlug + "createActivityCategoryError",
        "Kategorie činnosti se nepodařilo vytvořit");

    public static Error UpdateActivityCategoryError => Error.Validation(
        ComponentSlug + "updateActivityCategoryError",
        "Kategorie činnosti se nepodařilo upravit");

    public static Error DeleteActivityCategoryError => Error.Validation(
        ComponentSlug + "deleteActivityCategoryError",
        "Kategorie činnosti se nepodařilo smazat");
}