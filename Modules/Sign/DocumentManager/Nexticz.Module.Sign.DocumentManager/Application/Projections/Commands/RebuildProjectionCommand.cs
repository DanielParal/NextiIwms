namespace Nexticz.Module.Sign.DocumentManager.Application.Projections.Commands;

internal record RebuildProjectionCommand(string ProjectionTypeString) : IDocumentManagerCommand<bool>;