using ErrorOr;
using Nexticz.Module.Sign.Settings.Domain.EmailTemplateAggregate;

namespace Nexticz.Module.Sign.Settings.Application.EmailTemplates.Commands.CreateEmailTemplate;

internal record CreateEmailTemplateCommand(string Code, string Name, string Subject, string HtmlBody, string TextBody) : ISettingsCommand<ErrorOr<EmailTemplate>>;