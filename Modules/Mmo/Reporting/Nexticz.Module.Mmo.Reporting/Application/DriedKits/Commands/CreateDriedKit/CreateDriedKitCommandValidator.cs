using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Reporting.Application.DriedKits.Commands.CreateDriedKit;

internal class CreateDriedKitCommandValidator : AbstractValidator<CreateDriedKitCommand>
{
    public CreateDriedKitCommandValidator()
    {
        RuleFor(x => x.KitIdFromDrying)
            .NotEmpty()
            .WithState(x => new CustomErrorState(DriedKitErrors.ValidationKitIdIsRequired));
        
        RuleFor(x => x.CompletedKitsCount)
            .GreaterThan(0)
            .WithState(x => new CustomErrorState(DriedKitErrors.ValidationCompletedKitsCountGreaterThanZero));
        
        RuleFor(x => x.BatchId)
            .NotEmpty()
            .WithState(x => new CustomErrorState(DriedKitErrors.ValidationBatchIdIsRequired));
        
        RuleFor(x => x.LineCode)
            .NotEmpty()
            .WithState(x => new CustomErrorState(DriedKitErrors.ValidationLineCodeIsRequired));
        
        RuleFor(x => x.KitCode)
            .NotEmpty()
            .WithState(x => new CustomErrorState(DriedKitErrors.ValidationKitCodeIsRequired));
        
        RuleFor(x => x.ExpectedDryingTime)
            .GreaterThan(0)
            .WithState(x => new CustomErrorState(DriedKitErrors.ValidationOptimalDryingTimeGreaterThanZero));
        
        RuleFor(x => x.DryingEnded)
            .GreaterThan(x => x.DryingStarted)
            .WithState(x => new CustomErrorState(DriedKitErrors.ValidationDryingEndedHasToBeAfterDryingStarted));
        
    }
}