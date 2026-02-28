using ErrorOr;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.Common.Interfaces;


namespace Nexticz.Module.Vh.Application.Partners.Queries.GetPartners;

public class GetPartnersQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetPartnersQuery, ErrorOr<FilteredResult>>
{
    public async Task<ErrorOr<FilteredResult>> Handle(GetPartnersQuery query, CancellationToken cancellationToken)
    {
        return await unitOfWork.PartnersRepository.GetPartnersAsync(query.FilteringParams, cancellationToken);
    }
}