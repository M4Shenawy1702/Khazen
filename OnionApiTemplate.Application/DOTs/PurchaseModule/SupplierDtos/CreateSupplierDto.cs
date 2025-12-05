namespace Khazen.Application.DOTs.PurchaseModule.SupplierDtos;
public class CreateSupplierDto
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
}
