namespace Khazen.Application.UseCases.SalesModule.SalesInvoicesUseCases.Commands.Void
{
    public record VoidSalesInvoiceCommand(Guid Id, byte[] RowVersion, string CurrentUserId) : IRequest<bool>;
}
