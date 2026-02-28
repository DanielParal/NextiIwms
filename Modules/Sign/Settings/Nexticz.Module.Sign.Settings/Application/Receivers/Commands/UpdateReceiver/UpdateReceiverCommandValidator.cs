using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;

namespace Nexticz.Module.Sign.Settings.Application.Receivers.Commands.UpdateReceiver;

internal class UpdateReceiverCommandValidator : AbstractValidator<UpdateReceiverCommand>
{
    public UpdateReceiverCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithState(x => new CustomErrorState(ReceiverErrors.ValidationCodeIsRequired));
        
        RuleFor(x => x.PartnerCode)
            .NotEmpty()
            .WithState(x => new CustomErrorState(ReceiverErrors.ValidationPartnerCodeIsRequired));
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithState(x => new CustomErrorState(ReceiverErrors.ValidationNameIsRequired));
    }
}