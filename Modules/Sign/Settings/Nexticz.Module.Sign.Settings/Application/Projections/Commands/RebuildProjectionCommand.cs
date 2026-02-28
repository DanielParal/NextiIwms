namespace Nexticz.Module.Sign.Settings.Application.Projections.Commands;

internal record RebuildProjectionCommand(string ProjectionTypeString) : ISettingsCommand<bool>;