using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.Partners;

namespace Nexticz.Module.Vh.Application.Partners.Commands.CreatePartner;

public class CreatePartnerCommand : IRequest<ErrorOr<Created>>
{
    public required CreatePartnerRequest CreatePartnerRequest { get; set; }
}