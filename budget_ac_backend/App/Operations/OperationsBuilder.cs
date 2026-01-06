using budget_ac_backend.App.Operations.Requests.Data;
using budget_ac_backend.App.Operations.Validation;
using budget_ac_backend.App.Repository;
using budget_ac_backend.App.Repository.SqlLite;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace budget_ac_backend.App.Operations;

public static class OperationsBuilder {
    public static void AddOperations(this WebApplicationBuilder builder) {
        builder.Services.TryAddSingleton<IValidator<CreateOperationRequestData>, CreateOperationRequestDataValidator>();
        builder.Services.TryAddSingleton<IValidator<DeleteOperationRequestData>, DeleteOperationRequestDataValidator>();
        builder.Services.TryAddSingleton<IValidator<EditOperationRequestData>, EditOperationRequestDataValidator>();
        builder.Services.TryAddSingleton<IValidator<GetOperationsRequestData>, GetOperationsRequestDataValidator>();
    }
}
