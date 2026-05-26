using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Soundmates.Api.Common.Constants;
using Soundmates.Api.Common.Entities;
using Soundmates.Api.Common.Validation.Rules;

namespace Soundmates.Api.Features.Auth.Register;

internal sealed class RegisterValidator : AbstractValidator<RegisterRequest>
{
    public RegisterValidator(UserManager<User> userManager)
    {
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(ApplicationConstants.MaximumUserEmailLength)
            .EmailAddress()
            .MustAsync(async (email, ct) => await userManager.FindByEmailAsync(email) is null)
                .WithMessage("Email is already in use.");

        RuleFor(x => x.Password).Password();
    }
}
