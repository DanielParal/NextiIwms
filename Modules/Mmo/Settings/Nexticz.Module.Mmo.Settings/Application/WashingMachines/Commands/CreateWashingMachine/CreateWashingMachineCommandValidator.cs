using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Settings.Application.WashingMachines.Commands.CreateWashingMachine;

internal class CreateWashingMachineCommandValidator : AbstractValidator<CreateWashingMachineCommand>
{
    public CreateWashingMachineCommandValidator()
    {
        RuleFor(x => x.CreateWashingMachineRequest.Code)
            .NotEmpty()
            .WithState(x => new CustomErrorState(WashingMachineErrors.ValidationCodeIsEmpty));
        
        RuleFor(x => x.CreateWashingMachineRequest.Length)
            .GreaterThan(0)
            .WithState(x => new CustomErrorState(WashingMachineErrors.ValidationLengthGraterThanZero));
        
        RuleFor(x => x.CreateWashingMachineRequest.MinWidth)
            .GreaterThan(0)
            .WithState(x => new CustomErrorState(WashingMachineErrors.ValidationMinWidthGraterThanZero));
        
        RuleFor(x => x.CreateWashingMachineRequest.MaxWidth)
            .GreaterThan(0)
            .GreaterThanOrEqualTo(x => x.CreateWashingMachineRequest.MinWidth)
            .WithState(x => new CustomErrorState(WashingMachineErrors.ValidationMaxWidthGraterThanZero));
        
        RuleFor(x => x.CreateWashingMachineRequest.MaxHeight)
            .GreaterThan(0)
            .WithState(x => new CustomErrorState(WashingMachineErrors.ValidationMaxHeightGraterThanZero));
        
        RuleFor(x => x.CreateWashingMachineRequest.MaxWaterTemperature)
            .GreaterThan(0)
            .WithState(x => new CustomErrorState(WashingMachineErrors.ValidationMaxWaterTemperatureGraterThanZero));
        
        RuleFor(x => x.CreateWashingMachineRequest.MaxAirTemperature)
            .GreaterThan(0)
            .WithState(x => new CustomErrorState(WashingMachineErrors.ValidationMaxAirTemperatureGraterThanZero));
        
        RuleFor(x => x.CreateWashingMachineRequest.NumberOfLines)
            .InclusiveBetween(1, 2)
            .WithState(x => new CustomErrorState(WashingMachineErrors.ValidationNumberOfLinesEither1Or2));
        
        RuleFor(x => x.CreateWashingMachineRequest.Speed1)
            .GreaterThan(0)
            .WithState(x => new CustomErrorState(WashingMachineErrors.ValidationSpeed1GraterThanZero));
        
        RuleFor(x => x.CreateWashingMachineRequest.Speed2)
            .GreaterThan(0)
            .WithState(x => new CustomErrorState(WashingMachineErrors.ValidationSpeed2GraterThanZero));
        
        RuleFor(x => x.CreateWashingMachineRequest.Speed3)
            .GreaterThan(0)
            .WithState(x => new CustomErrorState(WashingMachineErrors.ValidationSpeed3GraterThanZero));
    }
}