using ErrorOr;
using Nexticz.Module.Mmo.Washing.Domain.WorkerEntity;

namespace Nexticz.Module.Mmo.Washing.Application.LastEnteredWorkerOnLines.Commands.EnterLine;

internal record EnterLineCommand(string LineCode, int WorkerPin) : IWashingCommand<ErrorOr<Worker>>;