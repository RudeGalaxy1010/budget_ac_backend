using budget_ac_backend.App.BL.Requests.Data;
using FluentValidation;

namespace budget_ac_backend.App.BL.Validation;

public class CreateCategoryRequestDataValidator : AbstractValidator<CreateCategoryRequestData> {
    public CreateCategoryRequestDataValidator() {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(64).WithMessage("Name must not exceed 64 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(256).WithMessage("Description must not exceed 256 characters.");
    }
}