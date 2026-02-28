using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.DocumentManager.Contracts.Emails;

public record SendEmailsRequest(
    [property: Required] string[] Recipients,
    [property: Required] SendEmailJobContract[] SendEmailJobs);