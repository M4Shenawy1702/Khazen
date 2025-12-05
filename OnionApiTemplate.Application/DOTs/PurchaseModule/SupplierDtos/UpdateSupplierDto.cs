namespace Khazen.Application.DOTs.PurchaseModule.SupplierDtos;
public class UpdateSupplierDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
}
