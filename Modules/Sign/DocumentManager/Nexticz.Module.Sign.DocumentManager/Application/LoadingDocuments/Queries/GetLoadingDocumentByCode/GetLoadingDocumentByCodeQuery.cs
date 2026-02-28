using ErrorOr;
using MediatR;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Queries.GetLoadingDocumentByCode;

internal record GetLoadingDocumentByCodeQuery(string LoadingDocumentCode) : IRequest<ErrorOr<LoadingDocument>>;