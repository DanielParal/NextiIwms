using ErrorOr;

namespace Nexticz.Module.Sign.Settings.Application.EmailTemplates.Commands.DeleteEmailTemplate;

internal record DeleteEmailTemplateCommand(string Code) : ISettingsCommand<ErrorOr<Success>>;