using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;


namespace Nexticz.Module.Mmo.Settings.Application.PackagingCirculations.Commands.CreatePackagingCirculation
{
    internal class CreatePackagingCirculationCommandValidator : AbstractValidator<CreatePackagingCirculationCommand>
    {
        public CreatePackagingCirculationCommandValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty()
                .WithState(x => new CustomErrorState(PackagingCirculationErrors.ValidationCodeIsRequired));
            
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithState(x => new CustomErrorState(PackagingCirculationErrors.ValidationNameIsRequired));
        }
    }
}