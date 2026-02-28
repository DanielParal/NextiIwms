using MediatR;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.DepositorAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Depositors.Queries.GetDepositorsByTemplateCode;

internal class GetDepositorsByTemplateCodeQueryHandler(ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : IRequestHandler<GetDepositorsByTemplateCodeQuery, Depositor[]>
{
    public async Task<Depositor[]> Handle(GetDepositorsByTemplateCodeQuery request, CancellationToken cancellationToken)
    {
        var depositors = 
            await readOnlyEventStoreRepository.GetAllByConditionAsync<Depositor>(
                x => 
                    string.Equals(x.DeliveryTemplateCode, request.DocumentTemplateCode, StringComparison.InvariantCultureIgnoreCase) || 
                    string.Equals(x.LoadingTemplateCode, request.DocumentTemplateCode, StringComparison.InvariantCultureIgnoreCase), cancellationToken);
        
        return depositors.ToArray();   
    }
}