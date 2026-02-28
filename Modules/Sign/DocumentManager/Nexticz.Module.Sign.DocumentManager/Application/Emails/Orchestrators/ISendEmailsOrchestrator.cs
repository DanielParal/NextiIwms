using ErrorOr;
using Nexticz.Module.Sign.DocumentManager.Contracts.Emails;

namespace Nexticz.Module.Sign.DocumentManager.Application.Emails.Orchestrators;

internal interface ISendEmailsOrchestrator
{
    Task<ErrorOr<Success>> OrchestrateAsync(string[] recipients, SendEmailJobContract[] sendEmailJobContracts, CancellationToken cancellationToken);
}