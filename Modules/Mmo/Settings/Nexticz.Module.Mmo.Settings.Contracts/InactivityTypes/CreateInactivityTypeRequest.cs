using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Settings.Contracts.InactivityTypes;

public record CreateInactivityTypeRequest(
    [property: Required] string Name,
    [property: Required] bool AffectProductivity,
    [property: Required] bool IsCommentNeededForReview);