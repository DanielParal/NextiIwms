using Microsoft.AspNetCore.Http;
using Nexticz.Lib.Shared.FormCollections;

namespace Nexticz.Module.Cuzk.Contracts.Imports;

public class UploadImportFormRequest (UploadImportRequest request, IFormFile file)
    : BaseFormWithFile<UploadImportRequest>(request, file);