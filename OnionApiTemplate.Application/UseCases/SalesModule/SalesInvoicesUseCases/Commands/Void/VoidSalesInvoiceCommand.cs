namespace Khazen.Application.UseCases.SalesModule.SalesInvoicesUseCases.Commands.Void
{
    public record VoidSalesInvoiceCommand(Guid Id, string CurrentUserId) : IRequest<bool>;
}
