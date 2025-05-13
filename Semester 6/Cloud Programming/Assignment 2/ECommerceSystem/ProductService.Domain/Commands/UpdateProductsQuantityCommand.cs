using Common.Domain.Commands;
using Common.Domain.DTO;

namespace ProductService.Domain.Commands;

public class UpdateProductsQuantityCommand : Command
{
    public List<ItemQuantityDto> Products { get; set; }

    public UpdateProductsQuantityCommand(List<ItemQuantityDto> products)
    {
        Products = products;
    }
}