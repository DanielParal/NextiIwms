using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.RemoveBatch;

internal class RemoveBatchCommandValidator : AbstractValidator<RemoveBatchCommand>
{
    public RemoveBatchCommandValidator()
    {
        RuleFor(x => x.LineQueueCode)
            .NotEmpty()
            .WithState(x => new CustomErrorState(WashingMachineErrors.ValidationLineQueueCodeIsRequired));
        
        RuleFor(x => x.BatchId)
            .NotEmpty()
            .WithState(x => new CustomErrorState(WashingMachineErrors.ValidationBatchIdIsRequired));
    }
}