using MediatR;
using ErrorOr;

namespace Nexticz.Module.Mmo.Settings.Contracts.Packagings.Queries;

public record GetPackagingResponseByCodeQuery(string Code) : IRequest<ErrorOr<PackagingResponse>>;