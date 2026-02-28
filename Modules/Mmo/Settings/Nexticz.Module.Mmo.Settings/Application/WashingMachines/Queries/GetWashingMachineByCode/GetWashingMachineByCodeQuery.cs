using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Settings.Domain.WashingMachineEntity;

namespace Nexticz.Module.Mmo.Settings.Application.WashingMachines.Queries.GetWashingMachineByCode;

internal record GetWashingMachineByCodeQuery(string Code) : IRequest<ErrorOr<WashingMachine>>;