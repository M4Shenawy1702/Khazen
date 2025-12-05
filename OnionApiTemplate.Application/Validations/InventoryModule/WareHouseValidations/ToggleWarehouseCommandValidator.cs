using Khazen.Application.UseCases.InventoryModule.WarehouseUseCases.Commands.Delete;

namespace Khazen.Application.Validations.InventoryModule.WareHouseValidations
{
    public class ToggleWarehouseCommandValidator : AbstractValidator<ToggleWarehouseCommand>
    {
        public ToggleWarehouseCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Warehouse ID is required.");
        }
    }

}
