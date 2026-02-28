using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Nexticz.Module.Vh.Application.SagDynamicsActivities.Command.LoadSagDynamicsActivities;

public record LoadSagDynamicsActivitiesCommand(IFormCollection FormCollection) : IRequest<ErrorOr<Created>>;