using MediatR;
using Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments;
using Nexticz.Module.Sign.Settings.Contracts.Depositors.Queries;
using Nexticz.Module.Sign.Settings.Contracts.Users.Queries;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Sign.DocumentManager.Application.Interfaces;
using Nexticz.Module.Sign.DocumentManager.Domain.Views;

namespace Nexticz.Module.Sign.DocumentManager.Application.SignedLoadingDocuments.Queries.GetSignedLoadingDocumentsForUser;

internal class GetSignedLoadingDocumentsForUserQueryHandler(
    IDocumentManagerReadOnlyEventStoreRepository readOnlyEventStoreRepository,
    ICurrentUserProvider currentUserProvider,
    ISender sender) 
    : IRequestHandler<GetSignedLoadingDocumentsForUserQuery, FilteredResult<SignedLoadingDocumentView>>
{
    public async Task<FilteredResult<SignedLoadingDocumentView>> Handle(GetSignedLoadingDocumentsForUserQuery request, CancellationToken cancellationToken)
    {
        var currentUser = currentUserProvider.GetCurrentUser();
        var user = await sender.Send(new GetUserResponseByUserNameQuery(currentUser.UserName), cancellationToken);
        if (user.IsError)
        {
            return new FilteredResult<SignedLoadingDocumentView>
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
        
        return await readOnlyEventStoreRepository.GetSignedLoadingDocumentsForUserAsync(
            request.FilteringParams, 
            depositorCodes, 
            request.FilteringParams.PartnersOrderNumber,
            request.FilteringParams.RznoCode,
            request.FilteringParams.CombinedRznoCode,
            request.FilteringParams.PartnerName,
            request.FilteringParams.ReceiverName,
            request.FilteringParams.FullTextFilter,
            cancellationToken);
    }
}