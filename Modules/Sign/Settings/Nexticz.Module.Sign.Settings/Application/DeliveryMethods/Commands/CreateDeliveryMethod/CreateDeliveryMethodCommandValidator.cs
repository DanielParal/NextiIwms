using FluentValidation;
using Nexticz.Module.Sign.Settings.Application.DepositorGroups;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Sign.Settings.Application.DeliveryMethods.Commands.CreateDeliveryMethod;

internal class CreateDeliveryMethodCommandValidator : AbstractValidator<CreateDeliveryMethodCommand>
{
    public CreateDeliveryMethodCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithState(x => new CustomErrorState(DeliveryMethodErrors.ValidationCodeIsRequired));
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithState(x => new CustomErrorState(DeliveryMethodErrors.ValidationNameIsRequired));
    }
}