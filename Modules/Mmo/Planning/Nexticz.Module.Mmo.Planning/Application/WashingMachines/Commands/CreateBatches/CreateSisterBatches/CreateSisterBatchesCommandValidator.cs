using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Planning.Application.WashingMachines.Commands.CreateBatches.CreateSisterBatches;

internal class CreateSisterBatchesCommandValidator : AbstractValidator<CreateSisterBatchesCommand>
{
    public CreateSisterBatchesCommandValidator()
    {
        RuleFor(x => x.LineQueueCode)
            .NotEmpty()
            .WithState(x => new CustomErrorState(WashingMachineErrors.ValidationLineQueueCodeIsRequired));
        
        RuleFor(x => x.KitCode)
            .NotEmpty()
            .WithState(x => new CustomErrorState(WashingMachineErrors.ValidationKitCodeIsRequired));
        
        RuleFor(x => x.PackagingCode)
            .NotEmpty()
            .WithState(x => new CustomErrorState(WashingMachineErrors.ValidationPackagingCodeIsRequired));
        
        RuleFor(x => x.SisterPackagingCode)
            .NotEmpty()
            .WithState(x => new CustomErrorState(WashingMachineErrors.ValidationSisterPackagingCodeIsRequired));
        
        RuleFor(x => x.KitsCount)
            .GreaterThan(0)
            .WithState(x => new CustomErrorState(WashingMachineErrors.ValidationKitsCountGreaterThan0));
    }
}