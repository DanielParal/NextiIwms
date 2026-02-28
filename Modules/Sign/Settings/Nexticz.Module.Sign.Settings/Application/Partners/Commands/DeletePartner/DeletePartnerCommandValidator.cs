using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Sign.Settings.Application.Partners.Commands.DeletePartner;

internal class DeletePartnerCommandValidator : AbstractValidator<DeletePartnerCommand>
{
    public DeletePartnerCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithState(x => new CustomErrorState(PartnerErrors.ValidationCodeIsRequired));
    }
}