using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.Settings.Contracts.Receivers;

public record UpdateReceiverRequest([property: Required] string Name);