using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.DocumentManager.Contracts.LoadingDocuments;

public record DownloadDocumentsRequest(
    [property: Required] DownloadDocumentJobContract[] DownloadDocumentJobs);