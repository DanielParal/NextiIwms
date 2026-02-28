using ErrorOr;
using MediatR;
using Nexticz.Module.Sign.Settings.Domain.EmailConfigurationAggregate;

namespace Nexticz.Module.Sign.Settings.Application.EmailConfigurations.Queries.GetEmailConfigurationByExactCodes;

internal record GetEmailConfigurationByExactCodesQuery(string[] DepositorCodes, string[] PartnerCodes, string[] ReceiverAndPartnerCombinationCodes) : IRequest<ErrorOr<EmailConfiguration>>;