using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Mmo.Reporting.Application.LineItems;

internal abstract class LineItemErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "mmo-reporting-api-lineItemsService-";
    
    public static Error LineItemNotFound => Error.NotFound(
        ComponentSlug + "LineItemNotFound",
        $"Aktivita nebyla nalezena."
    );
    
    public static Error ValidationLineItemTypeIsNotValid => Error.Validation(
        ComponentSlug + "ValidationLineItemTypeIsNotValid",
        "Typ aktivity neexistuje."
    );
    
    public static Error ValidationCannotSplitToKit => Error.Validation(
        ComponentSlug + "ValidationCannotSplitToKit",
        "Nemůžeme rozdělit aktivitu na kit."
    );
    
    public static Error ValidationCutTimeNotWithinItemInterval => Error.Validation(
        ComponentSlug + "ValidationCutTimeNotWithinItemInterval",
        "Nemůžeme změnit aktivitu, protože čas na rozdělení nesedí do intervalu aktivity."
    );
    
    public static Error ValidationAddOnlyBreakOrShutdown => Error.Validation(
        ComponentSlug + "ValidationAddOnlyBreakOrShutdown",
        "Může být vložena buď pauza nebo odstávka."
    );
    
    public static Error ValidationEndDateHasToBeGraterThanStartDate => Error.Validation(
        ComponentSlug + "ValidationEndDateHasToBeGraterThanStartDate",
        "Začátek aktivity musí být před koncem aktivity."
    );
    
    public static Error ValidationNoLineCodesProvided => Error.Validation(
        ComponentSlug + "ValidationNoLineCodesProvided",
        "Musíte specifikovat alespoň jednu lajnu."
    );
    
    public static Error ValidationItemTimeRangeMustBeWithinShiftTimeRange => Error.Validation(
        ComponentSlug + "ValidationItemTimeRangeMustBeWithinShiftTimeRange",
        "Časový inerval aktivity musí pasovat do směny."
    );
    
    public static Error ValidationNoValidLineCodesProvided => Error.Validation(
        ComponentSlug + "ValidationNoValidLineCodesProvided",
        "Musíte specifikovat alespoň jednu lajnu."
    );
    
    public static Error ValidationOtherItemsInTimeRange => Error.Validation(
        ComponentSlug + "ValidationOtherItemsInTimeRange",
        "V tomto časovém rozmezí jsou již jiné aktivity."
    );
}