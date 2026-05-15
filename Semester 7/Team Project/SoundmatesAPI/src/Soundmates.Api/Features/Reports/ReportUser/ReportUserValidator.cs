using FluentValidation;
using Soundmates.Api.Common.Validation.Rules;

namespace Soundmates.Api.Features.Reports.ReportUser;

internal sealed class ReportUserValidator : AbstractValidator<ReportUserRequest>
{
    public ReportUserValidator()
    {
        RuleFor(x => x.ReportedUserId).NotEmpty().ValidGuid();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
    }
}
