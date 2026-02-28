using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.Partners;

namespace Nexticz.Module.Vh.Application.Partners.Queries.GetPartnerById;

public class GetPartnerByIdQuery : IRequest<ErrorOr<PartnerResponse>>
{
    public required Guid Id { get; set; }
}