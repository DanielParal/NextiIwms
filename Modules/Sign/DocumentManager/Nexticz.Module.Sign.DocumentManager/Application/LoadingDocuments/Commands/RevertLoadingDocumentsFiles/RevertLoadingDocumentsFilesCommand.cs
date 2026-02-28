using ErrorOr;
using Nexticz.Module.Sign.DocumentManager.Domain.SigningDeviceAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Commands.RevertLoadingDocumentsFiles;

internal record RevertLoadingDocumentsFilesCommand(RevertLoadingDocumentFileJob[] RevertLoadingDocumentJobs) : IDocumentManagerCommand<Success>;