using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.Assortments;

namespace Nexticz.Module.Vh.Application.Assortments.Commands.UpdateAssortment;

public class UpdateAssortmentCommand : IRequest<ErrorOr<Updated>>
{
    public required Guid Id { get; set; }
    public required UpdateAssortmentRequest UpdateAssortmentRequest { get; set; }
}