using ErrorOr;
using MediatR;

namespace Nexticz.Module.Vh.Application.Partners.Commands.DeletePartner;

public class DeletePartnerCommand : IRequest<ErrorOr<Deleted>>
{
    public required Guid Id { get; set; }
}