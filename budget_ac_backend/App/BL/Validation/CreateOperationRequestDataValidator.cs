using budget_ac_backend.App.BL.Requests.Data;
using FluentValidation;

namespace budget_ac_backend.App.BL.Validation;

public class CreateOperationRequestDataValidator : AbstractValidator<CreateOperationsRequestData> {
    public CreateOperationRequestDataValidator() {
        RuleFor(x => x.CategoryName)
            .NotNull().WithMessage("Category cannot be null.");

        RuleFor(x => x.Money)
            .NotNull().WithMessage("Money cannot be null.")
            .NotEqual(0).WithMessage("Money cannot be zero.");

        RuleFor(x => x.Date)
            .NotNull()
            .WithMessage("Description must not exceed 256 characters.");
    }
}