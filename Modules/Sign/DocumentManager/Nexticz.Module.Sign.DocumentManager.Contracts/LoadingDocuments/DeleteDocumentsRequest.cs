using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.DocumentManager.Contracts.LoadingDocuments;

public record DeleteDocumentsRequest(
    [property: Required] string DeleteReason,
    [property: Required] DeleteDocumentJobContract[] DeleteDocumentJobs);