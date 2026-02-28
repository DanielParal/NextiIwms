using System.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nexticz.Module.EmailSender.Publishers;
using Nexticz.Lib.Shared.BaseUrls;
using Nexticz.Lib.Shared.FileHandling.Assets;
using Nexticz.Module.Auth.Application.FileHandling;

namespace Nexticz.Module.Auth.Application.MasstransitPublishers.EmailPublishers;

internal class AuthEmailPublisher(
    IAuthPublisher authPublisher, 
    IAuthFileHandler fileHandler, 
    AssetsSettings assetsSettings,
    ILogger<AuthEmailPublisher> logger,
    IConfiguration configuration
    ) : EmailPublisher(authPublisher, fileHandler, assetsSettings, logger), IAuthEmailPublisher
{
    public async Task PublishForgotPasswordEmailAsync(string email, string resetToken, CancellationToken cancellationToken)
    {
        var baseUrlSettings = BaseUrlSettingsFactory.Create(configuration);
        var resetTokenUrlEncoded = HttpUtility.UrlEncode(resetToken);
        
        var message = $@"
                            <h2>Pro změnu hesla klikněna na odkaz níže.</h2>
                            <div><a href='{baseUrlSettings.App}/auth/forgotten-password?token={resetTokenUrlEncoded}&email={email}'>
                                    Kliknutím na tento odkaz budete přesměrování na změnu hesla.</a>
                            </div>";
        var subject = "Změna uživatelského hesla.";
        
        var publishEmailMessage =
            new PublishEmailMessage(
                [email],
                [],
                [],
                subject,
                message,
                message,
                "Auth",
                "AuthForgotPasswordEmail",
                false,
                new Dictionary<string, string>(),
                [],
                null
            );
        
        await PublishSendEmailAsync(publishEmailMessage, cancellationToken);
    }

    public async Task PublishResetPasswordEmailAsync(string email, CancellationToken cancellationToken)
    {
        var baseUrlSettings = BaseUrlSettingsFactory.Create(configuration);

        var message = $@"
                            <h2>Vaše heslo bylo změněno.</h2>
                            <div><a href='{baseUrlSettings.App}/auth/login'>
                                    Přihlaste se s novým heslem zde.</a>
                            </div>";
        var subject = "Změna uživatelského hesla - potvrzení.";
        
        var publishEmailMessage =
            new PublishEmailMessage(
                [email],
                [],
                [],
                subject,
                message,
                message,
                "Auth",
                "AuthResetPasswordEmail",
                false,
                new Dictionary<string, string>(),
                [],
                null
            );
        
        await PublishSendEmailAsync(publishEmailMessage, cancellationToken);
    }
}