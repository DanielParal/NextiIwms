using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Vh.Domain.BandRewards;

public abstract class BandRewardsErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "vh-api-bandRewardsService-";

    public static Error BandRewardWithIdDoesnotExist => Error.Validation(
        ComponentSlug + "bandRewardWithIdDoesnotExist",
        "Pásmová odměna s tímto ID neexistuje");
}