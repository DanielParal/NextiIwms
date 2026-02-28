using Microsoft.AspNetCore.Http;
using Nexticz.Lib.Shared.FormCollections;

namespace Nexticz.Module.Sign.DocumentManager.Contracts.LoadingDocuments;

public class ManuallySignDocumentFormResponse(ManuallySignDocumentRequest request, IFormFile file)
    : BaseFormWithFile<ManuallySignDocumentRequest>(request, file);