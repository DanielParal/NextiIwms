using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.Partners;

namespace Nexticz.Module.Vh.Application.Partners.Commands.UpdatePartner;

public class UpdatePartnerCommand : IRequest<ErrorOr<Updated>>
{
    public required Guid Id { get; set; }
    public required UpdatePartnerRequest UpdatePartnerRequest { get; set; }
}