using MediatR;
using ErrorOr;
using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Settings.Application.Manufactures.Commands.DeleteManufacture;

internal class DeleteManufactureCommandValidator : AbstractValidator<DeleteManufactureCommand>
{
    public DeleteManufactureCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithState(x => new CustomErrorState(ManufactureErrors.ValidationCodeIsRequired));
    }
}