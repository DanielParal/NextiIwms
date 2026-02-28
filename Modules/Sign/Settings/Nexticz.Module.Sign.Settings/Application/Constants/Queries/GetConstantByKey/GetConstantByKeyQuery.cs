using ErrorOr;
using MediatR;
using Nexticz.Module.Sign.Settings.Domain.ConstantAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Constants.Queries.GetConstantByKey;

internal record GetConstantByKeyQuery(string Key) : IRequest<ErrorOr<Constant>>;