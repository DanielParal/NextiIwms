using ErrorOr;
using Nexticz.Module.Sign.Settings.Contracts.DocumentTemplates;

namespace Nexticz.Module.Sign.Settings.Application.DocumentTemplates.Commands.UpdateDocumentTemplate;

internal record UpdateDocumentTemplateCommand(
    string Code, TextOffsetContract[] TextOffsetContracts, TextBackgroundContract[] TextBackgroundContracts) 
    : ISettingsCommand<ErrorOr<Updated>>;