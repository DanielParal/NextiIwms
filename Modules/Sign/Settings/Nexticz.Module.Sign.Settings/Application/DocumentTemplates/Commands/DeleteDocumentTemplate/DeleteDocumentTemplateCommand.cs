using ErrorOr;

namespace Nexticz.Module.Sign.Settings.Application.DocumentTemplates.Commands.DeleteDocumentTemplate;

internal record DeleteDocumentTemplateCommand(string Code) : ISettingsCommand<ErrorOr<Deleted>>;