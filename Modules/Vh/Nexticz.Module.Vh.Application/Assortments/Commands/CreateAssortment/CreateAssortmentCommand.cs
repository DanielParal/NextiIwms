using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.Assortments;

namespace Nexticz.Module.Vh.Application.Assortments.Commands.CreateAssortment;

public class CreateAssortmentCommand : IRequest<ErrorOr<Created>>
{
    public required CreateAssortmentRequest CreateAssortmentRequest { get; set; }
}