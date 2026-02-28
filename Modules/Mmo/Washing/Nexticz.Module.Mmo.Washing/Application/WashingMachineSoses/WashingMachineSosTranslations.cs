using Nexticz.Lib.Shared.Translations;

namespace Nexticz.Module.Mmo.Washing.Application.WashingMachineSoses;

internal static class WashingMachineSosTranslations
{
    public static Translation SosCalledSignalRMessageTitle() => new($"MMO-washing-{nameof(SosCalledSignalRMessageTitle)}", $"Pomoc vyžádána");
    public static Translation SosCalledSignalRMessageDescription(string washingMachineCode) => new($"MMO-washing-{nameof(SosCalledSignalRMessageDescription)}", $"Na myčce {washingMachineCode} je vyžadována pomoc.");
    
    public static Translation SosResolvedSignalRMessageTitle() => new($"MMO-washing-{nameof(SosResolvedSignalRMessageTitle)}", $"Pomoc vyřešena");
    public static Translation SosResolvedSignalRMessageDescription(string washingMachineCode) => new($"MMO-washing-{nameof(SosResolvedSignalRMessageDescription)}", $"Na myčce {washingMachineCode} je problém vyřešen.");
}