using FluentValidation;
using WajeSmartAssessment.Application.Features.Auth.Commands;

namespace WajeSmartAssessment.Application.Features.Auth.Validators;
public class RegisterAuthorCommandValidator : AbstractValidator<RegisterAuthorCommand>
{
    public RegisterAuthorCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email is not a valid email address.");

        RuleFor(x => x.Password)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,15}$")
            .WithMessage("Password must be 8-15 characters long and must contain at least one uppercase letter, one lowercase letter, and one number.");

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters long.")
            .MaximumLength(15).WithMessage("Username must not exceed 15 characters.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MinimumLength(3).WithMessage("First name must be at least 3 characters long.")
            .MaximumLength(15).WithMessage("First name must not exceed 15 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MinimumLength(3).WithMessage("Last name must be at least 3 characters long.")
            .MaximumLength(15).WithMessage("Last name must not exceed 15 characters.");
    }
}
