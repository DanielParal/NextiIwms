using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.ConstantEntity;

namespace Nexticz.Module.Mmo.Settings.Application.Constants.Queries.GetConstantByKey;

internal class GetConstantByKeyQueryHandler (IConstantReadOnlyRepository repository)
    : IRequestHandler<GetConstantByKeyQuery, ErrorOr<Constant>>
{
    public async Task<ErrorOr<Constant>> Handle(GetConstantByKeyQuery request, CancellationToken cancellationToken)
    {
        var mmoConstant = await repository.GetByKeyAsync(request.Key, cancellationToken);

        if (mmoConstant is null)
            return ConstantErrors.KeyDoesNotExist;
        
        return mmoConstant;
    }
}