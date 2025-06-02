using budget_ac_backend.App.Auth.Requests.Data;
using FluentValidation;

namespace budget_ac_backend.App.Auth.Validation;

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequestData> {
    public RefreshTokenRequestValidator() {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}