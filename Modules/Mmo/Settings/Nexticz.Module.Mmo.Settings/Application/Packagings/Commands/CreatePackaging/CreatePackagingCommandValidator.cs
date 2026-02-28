using FluentValidation;
using Nexticz.Module.Mmo.Settings.Contracts.Packagings;
using Nexticz.Module.Mmo.Settings.Domain.PackagingAggregate;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Settings.Application.Packagings.Commands.CreatePackaging;

internal class CreatePackagingCommandValidator : AbstractValidator<CreatePackagingCommand>
{
    public CreatePackagingCommandValidator()
    {
        RuleFor(x => x.CreatePackagingRequest.DepositorCode)
            .NotEmpty()
            .WithState(x => new CustomErrorState(PackagingErrors.ValidationDepositorCodeIsEmpty));
        
        RuleFor(x => x.CreatePackagingRequest.PackagingTypeCode)
            .NotEmpty()
            .WithState(x => new CustomErrorState(PackagingErrors.ValidationPackagingTypeIsEmpty));
        
        RuleFor(x => x.CreatePackagingRequest.PackagingCirculationCode)
            .NotEmpty()
            .WithState(x => new CustomErrorState(PackagingErrors.ValidationPackagingCirculationIsEmpty));
        
        RuleFor(x => x.CreatePackagingRequest.CustomerNumber)
            .NotEmpty()
            .WithState(x => new CustomErrorState(PackagingErrors.ValidationCustomerNumberIsEmpty));
        
        RuleFor(x => x.CreatePackagingRequest.Dimensions.Depth)
            .GreaterThan(0)
            .WithState(x => new CustomErrorState(PackagingErrors.ValidationPackagingDepthGraterThan0));
        
        RuleFor(x => x.CreatePackagingRequest.Dimensions.Width)
            .GreaterThan(0)
            .WithState(x => new CustomErrorState(PackagingErrors.ValidationPackagingWidthGraterThan0));
        
        RuleFor(x => x.CreatePackagingRequest.Dimensions.Height)
            .GreaterThan(0)
            .WithState(x => new CustomErrorState(PackagingErrors.ValidationPackagingHeightGraterThan0));
        
        RuleFor(x => x.CreatePackagingRequest.Weight)
            .GreaterThan(0)
            .WithState(x => new CustomErrorState(PackagingErrors.ValidationPackagingWeightGraterThan0));
        
        RuleFor(x => x.CreatePackagingRequest.WashingMachineSpeeds)
            .NotNull()
            .NotEmpty()
            .WithState(x => new CustomErrorState(PackagingErrors.ValidationWashingMachineSpeedsIsRequired));
    }
}