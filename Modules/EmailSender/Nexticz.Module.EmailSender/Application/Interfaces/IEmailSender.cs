using MimeKit;

namespace Nexticz.Module.EmailSender.Application.Interfaces;

internal interface IEmailSender
{
    Task<string> SendAsync(MimeMessage message, CancellationToken token);
}