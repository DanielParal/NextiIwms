using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Reporting.Contracts.LineItems;

public record ChangeCommentRequest(
    [property: Required] string Comment);