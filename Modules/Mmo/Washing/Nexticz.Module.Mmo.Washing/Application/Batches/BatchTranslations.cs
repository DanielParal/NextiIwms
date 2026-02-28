using Nexticz.Lib.Shared.Translations;

namespace Nexticz.Module.Mmo.Washing.Application.Batches;

internal static class BatchTranslations
{
    public static readonly Translation NoUserAtLineLogoutNeeded = new($"MMO-washing-{nameof(NoUserAtLineLogoutNeeded)}", "Žádný uživatel není přihlášen na linku.");
    public static readonly Translation SpecialInformationConfirmationNeededFromUser = new($"MMO-washing-{nameof(SpecialInformationConfirmationNeededFromUser)}", "Nejprve musíte potvrdit speciální informaci.");
    public static readonly Translation SisterBatchDoesNotExist = new($"MMO-washing-{nameof(SisterBatchDoesNotExist)}", "Sesterská dávka neexistuje.");
    public static readonly Translation NoUserAtAnotherLineWeNeedToWait = new($"MMO-washing-{nameof(NoUserAtAnotherLineWeNeedToWait)}", "Na druhé lajně není přihlášen žádný uživatel. Musíme počkat na pracovníka na druhé lajně.");
    public static readonly Translation SpecialInformationConfirmationNeededFromUserFromOtherLine = new($"MMO-washing-{nameof(SpecialInformationConfirmationNeededFromUserFromOtherLine)}", "Nejprve musí potvrdit speciální informaci pracovník na druhé lajně.");
}