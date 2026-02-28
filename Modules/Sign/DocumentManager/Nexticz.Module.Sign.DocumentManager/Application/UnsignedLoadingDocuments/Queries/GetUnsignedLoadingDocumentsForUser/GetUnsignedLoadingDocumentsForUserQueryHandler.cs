using MediatR;
using Nexticz.Module.Sign.Settings.Contracts.Depositors.Queries;
using Nexticz.Module.Sign.Settings.Contracts.Users.Queries;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Sign.DocumentManager.Application.Interfaces;
using Nexticz.Module.Sign.DocumentManager.Domain.Views;

namespace Nexticz.Module.Sign.DocumentManager.Application.UnsignedLoadingDocuments.Queries.GetUnsignedLoadingDocumentsForUser;

internal class GetUnsignedLoadingDocumentsForUserQueryHandler(
    IDocumentManagerReadOnlyEventStoreRepository readOnlyEventStoreRepository,
    ICurrentUserProvider currentUserProvider,
    ISender sender) 
    : IRequestHandler<GetUnsignedLoadingDocumentsForUserQuery, FilteredResult<UnsignedLoadingDocumentView>>
{
    public async Task<FilteredResult<UnsignedLoadingDocumentView>> Handle(GetUnsignedLoadingDocumentsForUserQuery request, CancellationToken cancellationToken)
    {
        var currentUser = currentUserProvider.GetCurrentUser();
        var user = await sender.Send(new GetUserResponseByUserNameQuery(currentUser.UserName), cancellationToken);
        if (user.IsError)
        {
            return new FilteredResult<UnsignedLoadingDocumentView>
            {
                Data = [],
                GroupCount = 0,
                TotalCount = 0
            };
        }
        
        var depositors = await sender.Send(
            new GetDepositorResponsesByCodesAndGroupCodesQuery(user.Value.DepositorCodes, user.Value.DepositorGroupCodes), 
            cancellationToken);
        
        var depositorCodes = depositors.Select(dc => dc.Code).ToArray();
        
        return await readOnlyEventStoreRepository.GetUnsignedLoadingDocumentsForUserAsync(request.FilteringParams, depositorCodes, cancellationToken);
    }
}