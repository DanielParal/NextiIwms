using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Settings.Domain.ConstantEntity;

namespace Nexticz.Module.Mmo.Settings.Application.Constants.Queries.GetConstantByKey;

internal record GetConstantByKeyQuery(string Key) : IRequest<ErrorOr<Constant>>;