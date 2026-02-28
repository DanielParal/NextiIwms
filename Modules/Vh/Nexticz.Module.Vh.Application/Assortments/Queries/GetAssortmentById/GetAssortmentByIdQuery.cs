using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.Assortments;

namespace Nexticz.Module.Vh.Application.Assortments.Queries.GetAssortmentById;

public class GetAssortmentByIdQuery : IRequest<ErrorOr<AssortmentResponse>>
{
    public required Guid Id { get; set; }
}