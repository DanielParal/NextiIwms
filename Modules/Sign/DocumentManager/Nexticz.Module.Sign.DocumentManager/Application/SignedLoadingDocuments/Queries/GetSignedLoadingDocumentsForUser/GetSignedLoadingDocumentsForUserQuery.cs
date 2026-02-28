using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Sign.DocumentManager.Domain.Views;

namespace Nexticz.Module.Sign.DocumentManager.Application.SignedLoadingDocuments.Queries.GetSignedLoadingDocumentsForUser;

internal record GetSignedLoadingDocumentsForUserQuery(SignedLoadingDocumentFilteringParams FilteringParams) : IRequest<FilteredResult<SignedLoadingDocumentView>>;