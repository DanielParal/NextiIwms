using Nexticz.Module.EmailSender.Publishers;

namespace Nexticz.Module.Auth.Application.MasstransitPublishers.EmailPublishers;

internal interface IAuthEmailPublisher : IEmailPublisher
{
    Task PublishForgotPasswordEmailAsync(string email, string resetToken, CancellationToken cancellationToken);
    Task PublishResetPasswordEmailAsync(string email, CancellationToken cancellationToken);
}