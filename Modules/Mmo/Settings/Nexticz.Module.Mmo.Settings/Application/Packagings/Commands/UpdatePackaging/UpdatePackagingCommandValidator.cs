using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Settings.Application.Packagings.Commands.UpdatePackaging;

internal class UpdatePackagingCommandValidator : AbstractValidator<UpdatePackagingCommand>
{
    public UpdatePackagingCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithState(x => new CustomErrorState(PackagingErrors.ValidationCodeIsRequired));
        
        RuleFor(x => x.UpdatePackagingRequest.PackagingTypeCode)
            .NotEmpty()
            .WithState(x => new CustomErrorState(PackagingErrors.ValidationPackagingTypeIsEmpty));
        
        RuleFor(x => x.UpdatePackagingRequest.PackagingCirculationCode)
            .NotEmpty()
            .WithState(x => new CustomErrorState(PackagingErrors.ValidationPackagingCirculationIsEmpty));
        
        RuleFor(x => x.UpdatePackagingRequest.Dimensions.Depth)
            .GreaterThan(0)
            .WithState(x => new CustomErrorState(PackagingErrors.ValidationPackagingDepthGraterThan0));
        
        RuleFor(x => x.UpdatePackagingRequest.Dimensions.Width)
            .GreaterThan(0)
            .WithState(x => new CustomErrorState(PackagingErrors.ValidationPackagingWidthGraterThan0));
        
        RuleFor(x => x.UpdatePackagingRequest.Dimensions.Height)
            .GreaterThan(0)
            .WithState(x => new CustomErrorState(PackagingErrors.ValidationPackagingHeightGraterThan0));
        
        RuleFor(x => x.UpdatePackagingRequest.Weight)
            .GreaterThan(0)
            .WithState(x => new CustomErrorState(PackagingErrors.ValidationPackagingWeightGraterThan0));
        
        RuleFor(x => x.UpdatePackagingRequest.WashingMachineSpeeds)
            .NotNull()
            .NotEmpty()
            .WithState(x => new CustomErrorState(PackagingErrors.ValidationWashingMachineSpeedsIsRequired));
    }
}