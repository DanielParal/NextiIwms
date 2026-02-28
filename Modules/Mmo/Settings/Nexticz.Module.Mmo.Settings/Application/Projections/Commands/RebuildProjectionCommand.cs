using ErrorOr;

namespace Nexticz.Module.Mmo.Settings.Application.Projections.Commands;

internal record RebuildProjectionCommand(string ProjectionTypeString) : ISettingsCommand<bool>;