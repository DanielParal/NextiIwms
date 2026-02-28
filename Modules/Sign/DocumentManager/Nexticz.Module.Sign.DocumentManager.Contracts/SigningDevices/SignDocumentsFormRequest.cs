using Microsoft.AspNetCore.Http;
using Nexticz.Lib.Shared.FormCollections;

namespace Nexticz.Module.Sign.DocumentManager.Contracts.SigningDevices;

public class SignDocumentsFormRequest(SignDocumentsRequest request, IFormFile file)
    : BaseFormWithFile<SignDocumentsRequest>(request, file);