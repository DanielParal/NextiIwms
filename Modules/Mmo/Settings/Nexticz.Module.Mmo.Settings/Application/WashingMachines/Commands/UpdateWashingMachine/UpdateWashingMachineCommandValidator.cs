using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Settings.Application.WashingMachines.Commands.UpdateWashingMachine;

internal class UpdateWashingMachineCommandValidator : AbstractValidator<UpdateWashingMachineCommand>
{
    public UpdateWashingMachineCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithState(x => new CustomErrorState(WashingMachineErrors.ValidationCodeIsEmpty));
        
        RuleFor(x => x.UpdateWashingMachineRequest.Speed1)
            .GreaterThan(0)
            .WithState(x => new CustomErrorState(WashingMachineErrors.ValidationSpeed1GraterThanZero));
        
        RuleFor(x => x.UpdateWashingMachineRequest.Speed2)
            .GreaterThan(0)
            .WithState(x => new CustomErrorState(WashingMachineErrors.ValidationSpeed2GraterThanZero));
        
        RuleFor(x => x.UpdateWashingMachineRequest.Speed3)
            .GreaterThan(0)
            .WithState(x => new CustomErrorState(WashingMachineErrors.ValidationSpeed3GraterThanZero));
        
        RuleFor(x => x.UpdateWashingMachineRequest.MaxWaterTemperature)
            .GreaterThan(0)
            .WithState(x => new CustomErrorState(WashingMachineErrors.ValidationMaxWaterTemperatureGraterThanZero));
        
        RuleFor(x => x.UpdateWashingMachineRequest.MaxAirTemperature)
            .GreaterThan(0)
            .WithState(x => new CustomErrorState(WashingMachineErrors.ValidationMaxAirTemperatureGraterThanZero));
    }
}