using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Mmo.Planning.Domain.BatchEntity;

internal abstract class BatchErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "mmo-planning-domain-batchEntity-";
    
    public static Error ValidationKitsCountHasToBeGreaterThan0 => Error.Validation(
        ComponentSlug + "ValidationKitsCountHasToBeGreaterThan0",
        $"Počet kitů musí být větší než 0.");
    
    public static Error ValidationNotEnoughKitsLeftForDecrease(int kitsCountLeft) => Error.Validation(
        ComponentSlug + "ValidationNotEnoughKitsLeftForDecrease",
        $"Dávka nemá požadované množství kitů na ponížení. V dávce zbývá už pouze počet kitů: {kitsCountLeft}");
}