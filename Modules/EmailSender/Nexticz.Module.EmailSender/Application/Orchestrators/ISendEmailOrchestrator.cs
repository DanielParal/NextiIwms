using Nexticz.Module.EmailSender.Contracts;

namespace Nexticz.Module.EmailSender.Application.Orchestrators;

internal interface ISendEmailOrchestrator
{
    Task SendEmailAsync(SendEmailMessage sendEmailMessage);
    Task SendScheduledEmailsAsync(string roundKey, CancellationToken cancellationToken);
}