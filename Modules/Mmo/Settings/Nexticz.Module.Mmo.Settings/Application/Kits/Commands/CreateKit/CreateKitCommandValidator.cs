using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Settings.Application.Kits.Commands.CreateKit;

internal class CreateKitCommandValidator : AbstractValidator<CreateKitCommand>
{
    public CreateKitCommandValidator()
    {
        RuleFor(x => x.CreateKitRequest.DepositorCode)
            .NotEmpty()
            .WithState(x => new CustomErrorState(KitErrors.ValidationDepositorCodeIsEmpty));
        
        RuleFor(x => x.CreateKitRequest.KitTypeCode)
            .NotEmpty()
            .WithState(x => new CustomErrorState(KitErrors.ValidationKitTypeIsEmpty));
        
        RuleFor(x => x.CreateKitRequest.ManufactureCode)
            .NotEmpty()
            .WithState(x => new CustomErrorState(KitErrors.ValidationManufactureIsEmpty));
        
        RuleFor(x => x.CreateKitRequest.KitNumber)
            .NotEmpty()
            .WithState(x => new CustomErrorState(KitErrors.ValidationKitNumberIsEmpty));
        
        RuleFor(x => x.CreateKitRequest.DefiningPackagingCode)
            .NotEmpty()
            .WithState(x => new CustomErrorState(KitErrors.ValidationDefiningPackagingCodeIsEmpty));
        
        RuleFor(x => x.CreateKitRequest.DryingTime)
            .GreaterThan(0)
            .WithState(x => new CustomErrorState(KitErrors.ValidationDryingTimeGraterThan0));
        
        RuleFor(x => x.CreateKitRequest.PackagingQuantities)
            .Must(x => x is not null && x.Length != 0)
            .WithState(x => new CustomErrorState(KitErrors.ValidationPackagingCodeQuantitiesIsRequired));
    }
}