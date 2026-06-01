using FluentValidation;
using Soundmates.Api.Common.Constants;
using Soundmates.Api.Common.Validation.Rules;

namespace Soundmates.Api.Features.Messages.SendMessage;

internal sealed class SendMessageValidator : AbstractValidator<SendMessageRequest>
{
    public SendMessageValidator()
    {
        RuleFor(x => x.ReceiverId).NotEmpty().ValidGuid();
        RuleFor(x => x.Content).NotEmpty().MaximumLength(ApplicationConstants.MaximumMessageContentLength);
    }
}
