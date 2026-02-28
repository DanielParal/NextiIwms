using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Washing.Domain.LastEnteredWorkerOnLineAggregate;

namespace Nexticz.Module.Mmo.Washing.Application.LastEnteredWorkerOnLines.Queries.GetLastWorkerByLineCode;

internal record GetLastWorkerByLineCodeQuery(string LineCode) : IRequest<ErrorOr<LastEnteredWorkerOnLine>>;