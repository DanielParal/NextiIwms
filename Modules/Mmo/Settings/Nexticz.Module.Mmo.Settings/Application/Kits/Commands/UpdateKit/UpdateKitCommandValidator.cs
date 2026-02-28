using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Settings.Application.Kits.Commands.UpdateKit;

internal class UpdateKitCommandValidator : AbstractValidator<UpdateKitCommand>
{
    public UpdateKitCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithState(x => new CustomErrorState(KitErrors.ValidationCodeIsRequired));
        
        RuleFor(x => x.UpdateKitRequest.ManufactureCode)
            .NotEmpty()
            .WithState(x => new CustomErrorState(KitErrors.ValidationManufactureIsEmpty));
        
        RuleFor(x => x.UpdateKitRequest.DefiningPackagingCode)
            .NotEmpty()
            .WithState(x => new CustomErrorState(KitErrors.ValidationDefiningPackagingCodeIsEmpty));
        
        RuleFor(x => x.UpdateKitRequest.DryingTime)
            .GreaterThan(0)
            .WithState(x => new CustomErrorState(KitErrors.ValidationDryingTimeGraterThan0));
        
        RuleFor(x => x.UpdateKitRequest.PackagingQuantities)
            .Must(x => x is not null && x.Length != 0)
            .WithState(x => new CustomErrorState(KitErrors.ValidationPackagingCodeQuantitiesIsRequired));
    }
}