using ErrorOr;
using MediatR;

namespace Nexticz.Module.Vh.Application.Assortments.Commands.DeleteAssortment;

public class DeleteAssortmentCommand : IRequest<ErrorOr<Deleted>>
{
    public required Guid Id { get; set; }
}