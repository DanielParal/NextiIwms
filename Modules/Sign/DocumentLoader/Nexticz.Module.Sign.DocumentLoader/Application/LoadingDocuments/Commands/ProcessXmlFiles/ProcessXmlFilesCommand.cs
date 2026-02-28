using ErrorOr;

namespace Nexticz.Module.Sign.DocumentLoader.Application.LoadingDocuments.Commands.ProcessXmlFiles;

internal record ProcessXmlFilesCommand(string RoundKey) : IDocumentLoaderCommand<Success>;