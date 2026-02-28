using FluentValidation;
using Nexticz.Lib.Shared.Errors.Models;

namespace Nexticz.Module.Sign.Settings.Application.Receivers.Commands.DeleteReceiver;

internal class DeleteReceiverCommandValidator : AbstractValidator<DeleteReceiverCommand>
{
    public DeleteReceiverCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithState(x => new CustomErrorState(ReceiverErrors.ValidationCodeIsRequired));
        
        RuleFor(x => x.PartnerCode)
            .NotEmpty()
            .WithState(x => new CustomErrorState(ReceiverErrors.ValidationPartnerCodeIsRequired));
    }
}