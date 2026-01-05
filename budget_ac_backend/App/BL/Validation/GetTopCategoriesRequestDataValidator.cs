using budget_ac_backend.App.BL.Requests.Data;
using FluentValidation;

namespace budget_ac_backend.App.BL.Validation;

public class GetTopCategoriesRequestDataValidator : AbstractValidator<GetTopCategoriesRequestData> {
    public GetTopCategoriesRequestDataValidator() {
        RuleFor(x => x.Year)
            .NotNull()
            .WithMessage("Year cannot be null.");

        RuleFor(x => x.Count)
            .NotNull()
            .WithMessage("Count cannot be null.");

        RuleFor(x => x.Count)
            .GreaterThan(0)
            .WithMessage("Count must be greater than zero.");
    }
}
