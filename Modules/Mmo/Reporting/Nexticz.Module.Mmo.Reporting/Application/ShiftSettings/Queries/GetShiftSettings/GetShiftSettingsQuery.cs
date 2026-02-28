using ErrorOr;
using MediatR;

namespace Nexticz.Module.Mmo.Reporting.Application.ShiftSettings.Queries.GetShiftSettings;

internal record GetShiftSettingsQuery() : IRequest<ErrorOr<ShiftSetting[]>>;