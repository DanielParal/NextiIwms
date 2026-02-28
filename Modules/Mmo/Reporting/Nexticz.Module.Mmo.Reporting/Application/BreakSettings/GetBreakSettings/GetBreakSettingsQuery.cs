using ErrorOr;
using MediatR;

namespace Nexticz.Module.Mmo.Reporting.Application.BreakSettings.GetBreakSettings;

internal record GetBreakSettingsQuery() : IRequest<BreakSetting[]>;