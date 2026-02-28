using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Mmo.Reporting.Application.Inactivities;

internal abstract class InactivityErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "mmo-reporting-api-inactivitiesService-";
    
    public static Error ValidationStartDateBeforeEndDate => Error.Validation(
        ComponentSlug + "ValidationStartDateBeforeEndDate",
        "Začátek aktivity nesmí být před koncem aktivity."
    );
    
    public static Error ValidationInactivityReasonDoesNotExistInSetting => Error.Validation(
        ComponentSlug + "ValidationInactivityReasonDoesNotExistInSetting",
        "Důvod pro neaktivitu neexistuje."
    );
}