using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.ChangeBatchKitsCount;

internal class ChangeBatchKitsCountCommandValidator : AbstractValidator<ChangeBatchKitsCountCommand>
{
    public ChangeBatchKitsCountCommandValidator()
    {
        RuleFor(x => x.LineQueueCode)
            .NotEmpty()
            .WithState(x => new CustomErrorState(WashingMachineErrors.ValidationLineQueueCodeIsRequired));
        
        RuleFor(x => x.BatchId)
            .NotEmpty()
            .WithState(x => new CustomErrorState(WashingMachineErrors.ValidationBatchIdIsRequired));
        
        RuleFor(x => x.CountToChange)
            .NotEqual(0)
            .WithState(x => new CustomErrorState(WashingMachineErrors.ValidationKitsCountToChangeMustNotBeEqualTo0));
    }
}