using MediatR;

namespace Nexticz.Module.Sign.Settings.Contracts.Constants.Queries;

public record GetEmailsForMissingRequiredDataDuringTransferConstantValueQuery() : IRequest<string[]>;