using ErrorOr;

namespace Nexticz.Module.Sign.Settings.Application.EmailTemplates.Commands.UpdateEmailTemplate;

internal record UpdateEmailTemplateCommand(string Code, string Name, string Subject, string HtmlBody, string TextBody) : ISettingsCommand<ErrorOr<Success>>;