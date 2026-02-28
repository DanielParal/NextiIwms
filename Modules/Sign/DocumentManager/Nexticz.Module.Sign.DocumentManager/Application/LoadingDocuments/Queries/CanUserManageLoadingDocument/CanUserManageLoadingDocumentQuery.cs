using MediatR;
using Nexticz.Module.Sign.Settings.Contracts.Users;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Queries.CanUserManageLoadingDocument;

internal record CanUserManageLoadingDocumentQuery(LoadingDocument LoadingDocument, string[] UsersDepositorCodes) : IRequest<bool>;