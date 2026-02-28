using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.Assortments;
using Nexticz.Module.Vh.Domain.Assortments;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.Assortments.Queries.GetAssortmentById;

public class GetAssortmentByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAssortmentByIdQuery, ErrorOr<AssortmentResponse>>
{
    public async Task<ErrorOr<AssortmentResponse>> Handle(GetAssortmentByIdQuery query, CancellationToken cancellationToken)
    {
        var assortment =
            await unitOfWork.AssortmentRepository.GetAssortmentResponseByIdAsync(query.Id, cancellationToken);

        if (assortment is null) return AssortmentErrors.AssortmentWithIdDoesnotExist;

        return assortment;
    }
}