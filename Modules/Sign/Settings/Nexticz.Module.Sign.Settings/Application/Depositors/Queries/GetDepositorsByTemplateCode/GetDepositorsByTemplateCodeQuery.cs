using MediatR;
using Nexticz.Module.Sign.Settings.Domain.DepositorAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Depositors.Queries.GetDepositorsByTemplateCode;

internal record GetDepositorsByTemplateCodeQuery(string DocumentTemplateCode) : IRequest<Depositor[]>;