using ErrorOr;
using Microsoft.AspNetCore.Http;
using Nexticz.Lib.Shared.FileHandling.Models;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Commands.SignLoadingDocument;

internal record SignLoadingDocumentCommand(
    string LoadingDocumentCode, bool ShouldAlsoSignLoadingDocument, string[] DeliveryDocumentCodes,
    string DriverName, string LicensePlate, string UserName, string UserFullName, DateTimeOffset SignedAt,
    FileResult UserWhoSentDocumentsSignatureFile, FileResult DriverSignatureFile) : IDocumentManagerCommand<ErrorOr<LoadingDocument>>;