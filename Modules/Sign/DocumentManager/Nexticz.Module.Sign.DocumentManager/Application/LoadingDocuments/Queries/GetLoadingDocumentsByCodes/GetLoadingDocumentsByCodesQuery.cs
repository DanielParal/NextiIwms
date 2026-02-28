using MediatR;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Queries.GetLoadingDocumentsByCodes;

internal record GetLoadingDocumentsByCodesQuery(string[] LoadingDocumentCodes) : IRequest<LoadingDocument[]>;