using budget_ac_backend.App.Auth.Requests.Data;
using FluentValidation;

namespace budget_ac_backend.App.Auth.Validation;

public class LoginRequestDataValidator : AbstractValidator<LoginUserRequestData> {
    public LoginRequestDataValidator() {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email required")
            .EmailAddress().WithMessage("Email incorrect");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password required")
            .MinimumLength(6).WithMessage("Password should contains at least 6 characters");
    }
}