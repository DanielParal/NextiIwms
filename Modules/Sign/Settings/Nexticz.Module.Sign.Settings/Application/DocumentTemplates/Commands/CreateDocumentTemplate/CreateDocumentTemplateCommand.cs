using ErrorOr;
using Nexticz.Module.Sign.Settings.Contracts.DocumentTemplates;
using Nexticz.Module.Sign.Settings.Domain.DocumentTemplateAggregate;

namespace Nexticz.Module.Sign.Settings.Application.DocumentTemplates.Commands.CreateDocumentTemplate;

internal record CreateDocumentTemplateCommand(string Code, TextOffsetContract[] TextOffsets, TextBackgroundContract[] TextBackgrounds) 
    : ISettingsCommand<ErrorOr<DocumentTemplate>>;