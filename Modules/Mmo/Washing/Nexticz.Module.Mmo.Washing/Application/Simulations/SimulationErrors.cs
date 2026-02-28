using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Mmo.Washing.Application.Simulations;

internal abstract class SimulationErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "mmo-washing-api-simulationService-";
    
    public static Error ValidationWorkersForSimulationDoesNotExist(int pin1, int pin2) => Error.Validation(
        ComponentSlug + "ValidationWorkersForSimulationDoesNotExist",
        $"Musíte vytvořit 2 pracovníky s piny: {pin1} a {pin2}, abychom mohli začít simulaci."
    );
}