using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Settings.Contracts.InactivityTypes;

public record InactivityTypeResponse(
    [property: Required] Guid Id,
    [property: Required] string Name, 
    [property: Required] bool AffectProductivity,
    [property: Required] bool IsCommentNeededForReview);