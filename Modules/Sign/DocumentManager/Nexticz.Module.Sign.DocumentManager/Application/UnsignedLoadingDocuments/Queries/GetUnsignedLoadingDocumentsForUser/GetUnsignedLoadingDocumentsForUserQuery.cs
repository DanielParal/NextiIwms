using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Sign.DocumentManager.Domain.Views;

namespace Nexticz.Module.Sign.DocumentManager.Application.UnsignedLoadingDocuments.Queries.GetUnsignedLoadingDocumentsForUser;

internal record GetUnsignedLoadingDocumentsForUserQuery(BaseFilteringParams FilteringParams) : IRequest<FilteredResult<UnsignedLoadingDocumentView>>;