using MediatR;

namespace Nexticz.Module.Mmo.Settings.Contracts.Constants.Queries;

public record GetShiftSettingsConstantValueQuery() : IRequest<string>;