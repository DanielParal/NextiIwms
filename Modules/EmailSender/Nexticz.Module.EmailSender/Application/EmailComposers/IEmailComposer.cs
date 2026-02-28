using MimeKit;
using Nexticz.Module.EmailSender.Domain;
using Nexticz.Module.EmailSender.Domain.EmailMessageAggregate;
using Nexticz.Lib.Shared.FileHandling.Models;

namespace Nexticz.Module.EmailSender.Application.EmailComposers;

internal interface IEmailComposer
{
    MimeMessage Compose(string[] toRecipients, string[] ccRecipients, string[] bccRecipients, string subject, string textBody, string htmlBody, FileResult[] attachments);
}