namespace Khazen.Application.UseCases.SalesModule.CustomerUsecases.Commands.Delete
{
    public record ToggleCustomerCommand(Guid Id, string CurrentUserId) : IRequest<bool>;
}
