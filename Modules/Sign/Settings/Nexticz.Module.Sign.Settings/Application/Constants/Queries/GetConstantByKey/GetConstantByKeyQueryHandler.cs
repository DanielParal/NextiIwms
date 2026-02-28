using ErrorOr;
using MediatR;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.ConstantAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Constants.Queries.GetConstantByKey;

internal class GetConstantByKeyQueryHandler (ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository)
    : IRequestHandler<GetConstantByKeyQuery, ErrorOr<Constant>>
{
    public async Task<ErrorOr<Constant>> Handle(GetConstantByKeyQuery request, CancellationToken cancellationToken)
    {
        var mmoConstant = await readOnlyEventStoreRepository.GetFirstByConditionAsync<Constant>(
            x => x.Key.Equals(request.Key, StringComparison.InvariantCultureIgnoreCase), cancellationToken);;

        if (mmoConstant is null)
            return ConstantErrors.KeyDoesNotExist;
        
        return mmoConstant;
    }
}