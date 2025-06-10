using budget_ac_backend.App.BL.Requests.Data;
using budget_ac_backend.App.Data;
using budget_ac_backend.App.Repository;
using budget_ac_backend.App.Utils;
using FluentValidation;
using FluentValidation.Results;

namespace budget_ac_backend.App.BL.Requests;

public class GetCategoriesRequest : IdentifiedRequest<GetCategoriesRequestData> {
    private readonly ICategoryRepository _categoryRepository;
    private readonly IValidator<GetCategoriesRequestData> _validator;

    public GetCategoriesRequest(ICategoryRepository categoryRepository, IValidator<GetCategoriesRequestData> validator) {
        _categoryRepository = categoryRepository.ThrowIfArgumentNull();
        _validator = validator.ThrowIfArgumentNull();
    }

    protected override async Task<IResult> OnHandle(int userId, GetCategoriesRequestData request) {
        ValidationResult validationResult = await _validator.ValidateAsync(request);

        if (!validationResult.IsValid) {
            return Results.BadRequest(new { error = ErrorMessages.InvalidData });
        }

        List<Category> categories = await _categoryRepository.GetCategories(userId);

        return Results.Ok(new {
            categories = categories
                .Skip(request.Offset)
                .Take(request.Limit)
                .Select(category => new {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                })
        });
    }
}