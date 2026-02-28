using System.Net.Mail;

namespace Nexticz.Module.Auth.Application.Authentications.Common;

public static class AuthenticationEmails
{
    public static Email EmailConfirmation => new Email
    {
        To = [],
        Subject = "Potvrzení registrace nového uživatele",
        Body = @"
                <h2>Pro potvrzení registrace nového účtu klikněna na odkaz níže.</h2>
                <div><a href='{0}/api/auth/authentications/emailConfirmation?token={1}&email={2}'>
                        Kliknutím na tento odkaz potvrdíte Váš email a dokončíte novou registraci.</a>
                </div>
                ",
        Attachments = []
    };
}

public record Email
{
    public required List<string> To { get; set; }
    public required string Subject { get; set; }
    public required string Body { get; set; }
    public required List<Attachment> Attachments { get; set; }
} 