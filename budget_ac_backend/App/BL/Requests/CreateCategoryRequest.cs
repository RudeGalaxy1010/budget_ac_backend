using budget_ac_backend.App.BL.Requests.Data;
using budget_ac_backend.App.Data;
using budget_ac_backend.App.Repository;
using budget_ac_backend.App.Utils;
using FluentValidation;
using FluentValidation.Results;

namespace budget_ac_backend.App.BL.Requests;

public class CreateCategoryRequest : IdentifiedRequest<CreateCategoryRequestData> {
    private readonly IValidator<CreateCategoryRequestData> _validator;
    private readonly ICategoryRepository _categoryRepository;

    public CreateCategoryRequest(
        ICategoryRepository categoryRepository,
        IValidator<CreateCategoryRequestData> validator) {
        _categoryRepository = categoryRepository.ThrowIfArgumentNull();
        _validator = validator.ThrowIfArgumentNull();
    }

    protected override async Task<IResult> OnHandle(int userId, CreateCategoryRequestData request) {
        ValidationResult validationResult = await _validator.ValidateAsync(request);

        if (!validationResult.IsValid) {
            return Results.BadRequest(new { error = ErrorMessages.InvalidData });
        }

        Category? category = await _categoryRepository.GetCategoryByName(request.Name);

        if (category != null && category.UserId == userId) {
            return Results.BadRequest(new { error = ErrorMessages.CategoryAlreadyExists });
        }

        await _categoryRepository.CreateCategory(request.Name, request.Description, userId);
        return Results.Ok();
    }
}