using budget_ac_backend.App.Operations.Requests;
using budget_ac_backend.App.Operations.Requests.Data;
using budget_ac_backend.App.Operations.Validation;
using budget_ac_backend.App.Repository;
using budget_ac_backend.App.Utils;
using FluentValidation;

namespace budget_ac_backend.App.Operations;

public class OperationsMap(WebApplication app) {
    private const string CreateOperationRoute = "/operations/create";
    private const string GetOperationsRoute = "/operations/get";
    private const string EditOperationRoute = "/operations/edit";
    private const string DeleteOperationRoute = "/operations/delete";

    public void MapRoutes() {
        app.MapPost(CreateOperationRoute, CreateOperation);
        app.MapPost(GetOperationsRoute, GetOperations);
        app.MapPost(EditOperationRoute, EditOperation);
        app.MapPost(DeleteOperationRoute, DeleteOperation);
    }

    private async static Task<IResult> CreateOperation(
        CreateOperationRequestData request,
        HttpContext context,
        IValidator<CreateOperationRequestData> createOperationRequestDataValidator,
        IOperationRepository operationRepository) {
        CreateOperationRequest createOperationRequest = new CreateOperationRequest(
            operationRepository,
            createOperationRequestDataValidator);
        return await createOperationRequest.Handle(context, request);
    }

    private async static Task<IResult> GetOperations(
        GetOperationsRequestData request,
        HttpContext context,
        IValidator<GetOperationsRequestData> getOperationsRequestDataValidator,
        IOperationRepository operationRepository) {
        GetOperationsRequest getOperationsRequest = new GetOperationsRequest(
            operationRepository,
            getOperationsRequestDataValidator);
        return await getOperationsRequest.Handle(context, request);
    }

    private async static Task<IResult> EditOperation(
        EditOperationRequestData request,
        HttpContext context,
        IValidator<EditOperationRequestData> editOperationRequestDataValidator,
        IOperationRepository operationRepository) {
        EditOperationRequest editOperationRequest = new EditOperationRequest(
            operationRepository,
            editOperationRequestDataValidator);
        return await editOperationRequest.Handle(context, request);
    }

    private async static Task<IResult> DeleteOperation(
        DeleteOperationRequestData request,
        HttpContext context,
        IValidator<DeleteOperationRequestData> deleteOperationRequestDataValidator,
        IOperationRepository operationRepository) {
        DeleteOperationRequest deleteOperationRequest = new DeleteOperationRequest(
            operationRepository,
            deleteOperationRequestDataValidator);
        return await deleteOperationRequest.Handle(context, request);
    }
}
