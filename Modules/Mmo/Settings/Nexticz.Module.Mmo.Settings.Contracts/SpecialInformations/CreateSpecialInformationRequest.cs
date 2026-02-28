using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Settings.Contracts.SpecialInformations;

public record CreateSpecialInformationRequest(
    [property: Required] string Title,
    [property: Required] string Description
    );