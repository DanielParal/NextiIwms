using MimeKit;
using Nexticz.Lib.Shared.FileHandling.Models;

namespace Nexticz.Module.EmailSender.Application.EmailComposers;

internal class EmailComposer(EmailSendersSettings emailSendersSettings) : IEmailComposer
{
    private readonly EmailSenderSettings _defaultEmailSenderSettings = emailSendersSettings.Default;
    
    public MimeMessage Compose(string[] toRecipients, string[] ccRecipients, string[] bccRecipients, string subject, string textBody, string htmlBody, FileResult[] attachments)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_defaultEmailSenderSettings.DisplayName, _defaultEmailSenderSettings.SendFrom));

        foreach (var to in toRecipients)
        {
            message.To.Add(new MailboxAddress(to, to));
        }
        
        foreach (var cc in ccRecipients)
        {
            message.Cc.Add(new MailboxAddress(cc, cc));
        }
        
        foreach (var bcc in bccRecipients)
        {
            message.Bcc.Add(new MailboxAddress(bcc, bcc));
        }
        
        message.Subject = subject;
        var bb = new BodyBuilder {
            HtmlBody = htmlBody,
            TextBody = textBody
        };

        foreach (var attachment in attachments)
        {
            bb.Attachments.Add(attachment.FileName, attachment.ContentBytes, ContentType.Parse(attachment.ContentType));
        }
        
        message.Body = bb.ToMessageBody();
        return message;
    }
}