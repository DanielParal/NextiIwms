using Nexticz.Module.EmailSender.Publishers;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Application.MasstransitPublishers.EmailPublishers;

internal interface IDocumentManagerEmailPublisher : IEmailPublisher
{
    Task PublishEmailsBasedOnConfigurationAsync(DocumentEmailMessage[] documentEmailMessages, CancellationToken cancellationToken);
    Task PublishEmailsAsync(string[] recipients, DocumentEmailMessage[] documentEmailMessages, CancellationToken cancellationToken);
    Task PublishEmailsAboutMissingDepositorAsync(string[] recipients, string depositorCode, string loadingDocumentCode, string createdByUserNameInWms, CancellationToken cancellationToken);
    Task PublishEmailsAboutMissingDeliveryMethodAsync(string[] recipients, string[] deliveryMethodCodes, string loadingDocumentCode, string createdByUserNameInWms, CancellationToken cancellationToken);
}